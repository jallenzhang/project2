using System.Globalization;

namespace LilyServer
{

    using LiteLobby;
    using Photon.SocketServer;
    using Lite;
    using Lite.Operations;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;
    using DataPersist;
    using Helper;
    using DataPersist.HelperLib;
    using PokerWorld.Game;
    using System.Linq;
    using Events;
    using Lite.Events;
    using Lite.Messages;
    using System;
    using DataPersist.CardGame;
    using System.Threading;



    public class LilyGame : LiteLobbyGame
    {
        //private Dictionary<int, string> actorNrUserIdDic = new Dictionary<int, string>();

        private const int SettingThinkingtime = 15;
        private const int SettingMaxplayers = 9;
        private const bool SettingOnlyfriend = false;
        private readonly int _settingPiaozi = (int)ConfigurationHelp.CreateGameFEE;

        

        public bool OnlyFriend { get; private set; }
        public int BigBlind { get; private set; }
        private int MaxPlayers { get; set; }
        private TypeBet Limit { get; set; }
        private int ThinkingTime { get; set; }

        private GameType GameType { get; set; }

        public PokerGameUsers PokerGame { get; private set; }

        private readonly Dictionary<string, int> _countAchivment10Times = new Dictionary<string,int>();
        private readonly Dictionary<string, int> _countAchivment17Times = new Dictionary<string, int>();

        private void CreateGame() {
            //if (_pokergame==null)
            //{
                //_pokergame = new PokerGameUsers(new TableInfo(this.Name, this.bigBlind, Constants.MAX_PLAYERS, TypeBet.NoLimit), 1000, 1000, 5000);
            AbstractDealer dealer = new RandomDealer();


            if (ConfigurationManager.AppSettings["LilyDealer"]=="1")
            {
                dealer = new LilyTestDealer();
            }
            TableInfo newtable= new TableInfo(Name, BigBlind, MaxPlayers, Limit)
                                    {OnlyFriend = OnlyFriend, ThinkingTime = ThinkingTime};

            PokerGame = new PokerGameUsers(dealer,
                                                this, 
                                                newtable, 
                                                0, 
                                                0, 
                                                2000) {gameTax = ConfigurationHelp.GAMETAX};
            //InitDelegateEvent();
            //}
        }

        private void CreateCareerGame(GameGrade gg)
        {
            AbstractDealer dealer = new RandomDealer();
            PokerGame = new PokerGameCareer(dealer,
                                            this,
                                            Name,
                                            0,
                                            0,
                                            2000,gg) {gameTax = 0};
        }

        //private void InitRoomProperties() {
        //    var properties = this.Properties.GetProperties();
        //    this.OnlyFriend = (bool)properties["onlyfriend"];
        //    this.bigBlind = (int)properties["bigblind"];
        //    this.MaxPlayers = (int)properties["maxplayers"];
        //    this.ThinkingTime = (int)properties["thinkingtime"];
        //}


        private void InitDelegateEvent()
        {
            PokerGame.PlayerJoined += LilyGamePlayerJoined;
            PokerGame.PlayerLeaved += LilyGamePlayerLeaved;
            PokerGame.PlayerActionTaken += LilyGamePlayerActionTaken;
            PokerGame.PlayerActionNeeded += LilyGamePlayerActionNeeded;
            PokerGame.PlayerHoleCardsChanged += LilyGamePlayerHoleCardsChanged;
            PokerGame.PlayersShowCards += LilyGamePlayersShowCards;

            if(PokerGame.GameType==GameType.Career)
            {
                PokerGame.PlayerMoneyChanged += LilyGamePlayerMoneyChangedCareer;
                PokerGame.GameStarted += LilyGameCareerGameStarted;
            }
            else
                PokerGame.PlayerMoneyChanged += LilyGamePlayerMoneyChanged;

            PokerGame.PlayerWonPot += LilyGamePlayerWonPot;
            PokerGame.PlayerWonPotImprove += LilyGamePlayerWonPotImprove;

            PokerGame.GameBettingRoundEnded += LilyGameGameBettingRoundEnded;
            PokerGame.GameBettingRoundStarted += LilyGameGameBettingRoundStarted;
            PokerGame.GameBlindNeeded += LilyGameGameBlindNeeded;
            PokerGame.GameEnded += LilyGameGameEnded;

            //this.PorkerGame.GameGenerallyUpdated += new EventHandler(lilyGame_GameGenerallyUpdated);
            PokerGame.EverythingEnded += LilyGameEverythingEnded;

            PokerGame.DecideWinnersEnded+=lilyGame_DecideWinnersEnded;

            InitStatusTipsDelegateEvent();
            InitPokerGameCareer();
        }

        //private Dictionary<string, UserData> userDataDic = new Dictionary<string, UserData>();
        private void InitStatusTipsDelegateEvent()
        {
            PokerGame.GameEnded += StatusTipsMsgHander.lilyGame_GameEnded;
        }

        private void InitPokerGameCareer()
        {
            if (PokerGame is PokerGameCareer)
            {
                (PokerGame as PokerGameCareer).PlayerRankChanged += CareerGamePlayerRankChanged;
                (PokerGame as PokerGameCareer).PokerGameCareer_EverythingEnded += LilyGameEverythingEnded;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LilyGame"/> class.
        /// </summary>
        /// <param name="gameName">The name of the game.</param>
        /// <param name="lobbyName">The name of the Lobby </param>
        public LilyGame(string gameName, string lobbyName)
            : base(gameName, lobbyName)
        {
            PokerGame = null;
            //default setting
            OnlyFriend = SettingOnlyfriend;
            BigBlind = 10;
            MaxPlayers = SettingMaxplayers;
            Limit = TypeBet.NoLimit;
            ThinkingTime = SettingThinkingtime;
            GameType = GameType.User;
        }

        
       
        ///// <summary>
        ///// Called for each operation in the execution queue.
        ///// </summary>
        ///// <param name="peer">The peer.</param>
        ///// <param name="operationRequest">The operation request to execute.</param>
        ///// <param name="sendParameters"></param>
        ///// <remarks>
        ///// ExecuteOperation is overriden to handle our custom operations.
        ///// </remarks>
        //protected override void ExecuteOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        //{
        //    switch ((LilyOperationCodes)operationRequest.OperationCode)
        //    {
        //        case LilyOperationCodes.GameOperation:
        //            this.HandleLilyGameOperation(peer, operationRequest, sendParameters);
        //            break;

        //        default:
        //            // all other operations will be handled by the LiteGame implementation
        //            base.ExecuteOperation(peer, operationRequest, sendParameters);
        //            break;
        //    }
        //}

        //private void HandleLilyGameOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        //{
        //    var requestContract = new LilyGameRequest(peer.Protocol, operationRequest);
        //    requestContract.OnStart();

        //    var responseContract = new LilyGameResponse();
        //    responseContract.Response = "You are in game " + this.Name;

        //    var operationResponse = new OperationResponse(operationRequest.OperationCode, responseContract);
        //    peer.SendOperationResponse(operationResponse, sendParameters);

        //    requestContract.OnComplete();
        //}

        protected override void Dispose(bool disposing)
        {
            Log.DebugFormat("Room Dispose: name={0}", Name);
            try
            {
                if (PokerGame != null)
                {
                    TableInfo table = PokerGame.Table;
                    foreach (PlayerInfo player in table.PlayersAndBystander)
                    {
                        PokerGame.LeaveGame(player, PlayerLeaveType.Kick);
                    }
                    //GC.SuppressFinalize(PokerGame);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            base.Dispose(disposing);
        }

        protected override void PublishJoinEvent(LitePeer peer, JoinRequest joinRequest)
        {
            Actor actor = GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            var lilyPeer = peer as LilyPeer;
            if (lilyPeer != null)
            {
                string uid = lilyPeer.UserId;

                Hashtable joinedUser = null;
                if (uid != string.Empty)
                {
                    joinedUser = UserService.getInstance().QueryUserByUserId(uid).ToUserData().Tobyte();
                }

                var joinEvent = new LilyJoinEvent(actor.ActorNr, joinedUser);

                PublishEvent(joinEvent, Actors, new SendParameters());
            }
        }

        protected virtual bool TryAddPeerToGame(LitePeer peer, string userId, out Actor actor)
        {
            // check if the peer already exists in this game
            if (base.TryAddPeerToGame(peer, out actor) == false)
            {
                return false;
            }

            //UserData data = UserService.getInstance().QueryUserByUserId(userId);
            //if (data != null)
            //{
            //    userDataDic.Add(userId, data);
            //}

            return true;
        }

        protected override Actor HandleJoinOperation(LitePeer peer, JoinRequest joinRequest, SendParameters sendParamters)
        {
            Log.Info("---------------------------In LilyGame ThreadId:" + Thread.CurrentThread.ManagedThreadId);
            if (IsDisposed)
            {
                // join arrived after being disposed - repeat join operation                
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("Join operation on disposed game. GameName={0}", Name);
                }

                return null;
            }
            
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Join operation from IP: {0} to port: {1}", peer.RemoteIP, peer.LocalPort);
            }

            string cuserId=joinRequest.OperationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            // jack add
            ((LilyPeer)peer).UserId = cuserId;
            string gameid = joinRequest.GameId;
            if (cuserId!=gameid&& OnlyFriend && !FriendService.getInstance().ExistFriend(cuserId,gameid))
            {
                Dictionary<byte, object> parameters = new Dictionary<byte, object>();
                parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.OnlyFriendsCanJoin;
                peer.SendOperationResponse(new OperationResponse(joinRequest.OperationRequest.OperationCode, parameters), sendParamters);
                return null;
            }

            // create an new actor
            Actor actor;
            if (TryAddPeerToGame(peer, cuserId, out actor) == false)
            {
                peer.SendOperationResponse(
                    new OperationResponse
                    {
                        OperationCode = joinRequest.OperationRequest.OperationCode,
                        ReturnCode = -1,
                        DebugMessage = "Peer already joined the specified game."
                    },
                    sendParamters);
                return null;
            }

            LilyServer.Actors.Add(actor);

            // set game properties for join from the first actor
            if (Actors.Count == 1 && joinRequest.GameProperties != null)
            {
                Properties.SetProperties(joinRequest.GameProperties);
                //InitRoomProperties();
            }

            // set custom actor properties if defined
            if (joinRequest.ActorProperties != null)
            {
                actor.Properties.SetProperties(joinRequest.ActorProperties);
            }            

            // set operation return values and publish the response
            var joinResponse = new JoinResponse { ActorNr = actor.ActorNr };

            if (Properties.Count > 0)
            {
                joinResponse.CurrentGameProperties = Properties.GetProperties();
            }

            foreach (Actor t in Actors)
            {
                if (t.ActorNr != actor.ActorNr && t.Properties.Count > 0)
                {
                    if (joinResponse.CurrentActorProperties == null)
                    {
                        joinResponse.CurrentActorProperties = new Hashtable();
                    }

                    Hashtable actorProperties = t.Properties.GetProperties();
                    joinResponse.CurrentActorProperties.Add(t.ActorNr, actorProperties);
                }
            }
            if (string.IsNullOrEmpty(joinRequest.OperationRequest.Parameters[(byte)LilyOpKey.LobbyId] as string) == false)
            {
                Dictionary<byte, object> dic = new Dictionary<byte, object>();
                List<UserData> userDatas = new List<UserData>();
                foreach (string userId in Actors.Select(rs=>
                                                                 {
                                                                     var lilyPeer = rs.Peer as LilyPeer;
                                                                     return lilyPeer != null ? lilyPeer.UserId : null;
                                                                 }))
                {
                    UserData userData = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
                    if (PokerGame != null)
                        if (PokerGame.Table.Players.Any(rs =>
                                                                 {
                                                                     var data = rs as UserData;
                                                                     return data != null && data.UserId == userId;
                                                                 }))
                            userData.IsSitting = true;

                    userDatas.Add(userData);
                }

                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                //genenate roomData
                string roomName = Name;
                if (roomName.StartsWith("system"))
                    GameType = GameType.System;
                if (roomName.StartsWith("career"))
                    GameType = GameType.Career;

                UserData owner = GameType == GameType.User ?
                    UserService.getInstance().QueryUserByUserId(Name).ToUserData() :
                    new UserData { UserId=Name,LivingRoomType=RoomType.Common};
                RoomData roomData = new RoomData {Owner = owner, Users = userDatas, GameType = GameType};
                //roomData.RoomId = Name;
                //roomData.RoomType = owner==null?RoomType.Common:owner.LivingRoomType;

                Hashtable ht = SerializeHelper.Serialize(roomData);

                Hashtable tableinfo = null;
                if (PokerGame!=null)
                {
                    tableinfo = PokerGame.Table.ToHashtable();
                }


                dic.Add((byte)LilyOpKey.RoomData, ht);
                dic.Add((byte)LilyOpKey.TableInfo,tableinfo);

                if (roomName == cuserId&&roomData.Owner.Wins<20)
                {
                    if (UserMessageService.Singleton.TryGetMessages(cuserId))
                        dic.Add((byte)LilyOpKey.AppScore, true);
                }

                var operationResponse = new OperationResponse(joinRequest.OperationRequest.OperationCode, dic);
                peer.SendOperationResponse(operationResponse, sendParamters);
                

            }
            else
            {
                Dictionary<byte, object> parameters = new Dictionary<byte, object>();
                parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                peer.SendOperationResponse(new OperationResponse(joinRequest.OperationRequest.OperationCode, parameters), sendParamters);
            }
            // publish join event
            PublishJoinEvent(peer, joinRequest);

            PublishEventCache(peer);

            if (actor != null)
            {
                base.UpdateLobby();
            }

            return actor;
        }

        private Dictionary<byte, object> StandUp(LilyPeer peer, int seat) {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (PokerGame == null)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.GameEnded);
                return dic;
            }

            string userid = peer.UserId;
            PlayerInfo playinfo =seat>0?
                                PokerGame.Table.GetPlayer(seat):
                                PokerGame.Table.Players.Find(r => ((UserData) r).UserId == userid);
            if (playinfo != null)
                seat = playinfo.NoSeat;
            else
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NoResult);
                return dic;
            }


            //PlayerInfo playinfo = this.PokerGame.Table.GetPlayer(seat);

            //if (playinfo == null)
            //{
            //    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserNotExist);
            //    return dic;
            //}

            if (seat == PokerGame.Table.NoSeatCurrPlayer)
                PokerGame.PlayMoney(playinfo, -1);

            if (PokerGame.StandUp(playinfo))
            {
                UserService.getInstance().ChangeUserStatus(peer.UserId, UserStatus.Idle);
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserId, peer.UserId);
                dic.Add((byte)LilyOpKey.NoSeat, seat);
            }
            else {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.NoResult);
            }
            return dic;

        }


        private Dictionary<byte, object> LeaveTable(string userId,int seat)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (PokerGame == null)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                return dic;
            }

            if (seat==-1)
            {
                PlayerInfo stander = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
                PokerGame.LeaveGame(stander);
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserId, userId);
                dic.Add((byte)LilyOpKey.NoSeat, seat);
                return dic;
            }

            PlayerInfo playinfo = PokerGame.Table.GetPlayer(seat);

            if (playinfo == null) {
                dic.Add((byte)LilyOpKey.ErrorCode,ErrorCode.UserNotExist);
                return dic;
            }

            if (seat == PokerGame.Table.NoSeatCurrPlayer)
                PokerGame.PlayMoney(playinfo, -1);

            if (PokerGame.LeaveGame(playinfo))
            {               
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserId,userId);
                dic.Add((byte)LilyOpKey.NoSeat,seat);                
            }            

            return dic;
        }




        public Dictionary<byte, object> CreateSystemPokerGame(int bigBlind,long takenAmnt,UserData user,LitePeer peer) {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (bigBlind % 2 == 1) bigBlind++;
            BigBlind = bigBlind;
            ThinkingTime = SettingThinkingtime;
            GameType = GameType.System;
            CreateGame();
            BankService.getInstance().addRecord(0L, BankActionType.CreatGameSystem, user.UserId);

            if (PokerGame.State == TypeState.Init)
            {
                PokerGame.Start();
            }
            PokerGame.isFastForward = true;
            PlayerInfo curPlayer = user;
            Random x = new Random();
            PokerGame.stopFastForwardFlag = (TypeRound)x.Next(4);
            //this.PokerGame.RealPlayerCanJoinNow = () =>
            //{
            //    this.PokerGame.SitInGame(curPlayer);
            //};
            RobotService.Singleton.AddRobotInSystemGameBeforePlayerJoined(this);

            int waittimes = 0;
            while (PokerGame.isFastForward&&waittimes<10)
            {
                Log.DebugFormat("----CreateSystemPokerGame :  sleep ={0}",50);
                waittimes++;
                Thread.Sleep(50);
            }

            //Thread.Sleep(500);
            PokerGame.isFastForward = false;
            PokerGame.SitInGame(user);
            if (PokerGame.Table.Players.Count<2)
            {
                RobotService.Singleton.Request(this);
            }

            InitDelegateEvent();
            if (curPlayer.NoSeat != -1)
            {

                user.IsSitting = true;
                //this.PublishSitEvent(peer);
                UserService.getInstance().ChangeUserStatus(user.UserId, UserStatus.Playing);

                //this.PublishSitEvent(peer);
                PokerGame game = PokerGame;
                TableInfo gametable = game.Table.ToHashtable().ToTableInfo();
                foreach (PlayerInfo p in gametable.Players)
                {
                    if (curPlayer.NoSeat != p.NoSeat && !p.IsShowingCards)
                    {
                        p.Cards = new[] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                        //PlayerInfo curplayer = game.Table.GetPlayer(p.NoSeat);
                        //if (curplayer.IsPlaying || curplayer.IsAllIn)
                        //dic[(byte)LilyOpKey.PlayerCardId] = curplayer.Cards.Select(r => r.Id).ToArray();
                    }
                    //else { 
                    //        p.Cards = new GameCard[2] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                    //}
                }
                dic.Add((byte)LilyOpKey.TableInfo, gametable.ToHashtable());
                int[] gameCardIds = game.Table.Cards.Select(rs => rs.Id).ToArray();
                dic[(byte)LilyOpKey.GameId] = gameCardIds;
                int[] playingNoSeats = PokerGame.Table.Players.FindAll(rs => rs.IsPlaying).Select(rs => rs.NoSeat).ToArray();
                int[] allInNoSeats = PokerGame.Table.Players.FindAll(rs => rs.IsAllIn).Select(rs => rs.NoSeat).ToArray();
                dic[(byte)LilyOpKey.GameTypeState] = PokerGame.State;
                dic[(byte)LilyOpKey.PlayingNoSeats] = playingNoSeats;
                dic[(byte)LilyOpKey.AllInNoSeats] = allInNoSeats;
                dic[(byte)LilyOpKey.NoSeat] = curPlayer.NoSeat;
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                if (((LilyPeer) peer).OnlineAwards > 0)
                {
                    dic[(byte)LilyOpKey.OnlineAwards] = ((LilyPeer) peer).OnlineAwards;
                }
            }
            else
            {
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.TableFull;
            }
            //this.PokerGame.HurryUp = false;
            return dic;
        }


        public Dictionary<byte, object> TryJoinTable(UserData data)
        {
            LilyGame game = this;
            string gameId = Name;
            string userId = data.UserId;
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            //if (gameId != uid)
            //{
            if (gameId != userId && (game.PokerGame == null || game.PokerGame.State == TypeState.End))
            {
                //Log.DebugFormat("------------------------- JoinGameRequest: TableNotExist, request game {0}, userId {1}", gameId, uid);
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.TableNotExist);
                return dic;
            }

            int varBigBlind = PokerGame.Table.BigBlindAmnt;

            if (gameId == userId)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                return dic;
            }

            //PokerGame pokergame = PokerGame;

            int takenAmnt;

            if (data.Chips < 200)
            {
                dic.Clear();
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                return dic;
            }

            //if (or.Parameters.ContainsKey((byte)LilyOpKey.GameSettingBigBlind))
            //{
            //    int bigblind = (int)or.Parameters[(byte)LilyOpKey.GameSettingBigBlind];
            //    if (bigblind % 2 == 1) bigblind++;
            //    this.BigBlind = bigblind;
            //}


            if (data.Chips >= 200 && data.Chips <= 2000)
            {
                if (varBigBlind > 10)
                {
                    dic.Clear();
                    dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                    return dic;
                }
                takenAmnt = 200;
            }
            else
            {
                if (data.Chips < 100 * varBigBlind)
                {
                    dic.Clear();
                    dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                    return dic;
                }
                takenAmnt = (int)data.Chips / 10;
            }

            dic.Clear();
            dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            dic[(byte)LilyOpKey.TakenAmnt] = takenAmnt;
            return dic;
        }


        public Dictionary<byte, object> SitTable(UserData userData,OperationRequest or,LitePeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            //Lite.Caching.RoomReference gr;
            //PokerGame game = null;
            //if (LilyGameCache.Instance.TryGetRoomReference(gameId, out gr))
            //{
            //    game = gr.Room.PorkerGame;


            if (or.Parameters.ContainsKey((byte)LilyOpKey.GameSettingBigBlind))
            {
                int bigblind = (int)or.Parameters[(byte)LilyOpKey.GameSettingBigBlind];
                Log.DebugFormat("############## BigBlind: {0}", bigblind);
                if (bigblind % 2 == 1) bigblind++;
                BigBlind = Math.Max(bigblind, 4);
            }

            PokerGame game = PokerGame;
            string userId = userData.UserId;
            long takenAmnt;


            Log.DebugFormat("############## GameName: {0}",Name);

            if (GameType == GameType.User&& userId!=Name)
            {
                dic = TryJoinTable(userData);
                if ((int)dic[(byte)LilyOpKey.ErrorCode] != (int)ErrorCode.Sucess)
                    return dic;

                takenAmnt = Convert.ToInt64(dic[(byte)LilyOpKey.TakenAmnt]);
            }
            else {
                takenAmnt = userData.Chips / 10;
                //if (this.BigBlind == 4)
                //    takenAmnt = Math.Min(takenAmnt, 100);
            }

            #region creategame
            if ((PokerGame == null || PokerGame.State == TypeState.End) && userId == Name)
            {
                if (or.Parameters.ContainsKey((byte)LilyOpKey.GameSettingMaxPlayers))
                    MaxPlayers = (int)or.Parameters[(byte)LilyOpKey.GameSettingMaxPlayers];
                if (or.Parameters.ContainsKey((byte)LilyOpKey.GameSettingThinkingTime))
                    ThinkingTime = (int)or.Parameters[(byte)LilyOpKey.GameSettingThinkingTime];
                if (or.Parameters.ContainsKey((byte)LilyOpKey.GameSettingFriendsOnly))
                    OnlyFriend = (bool)or.Parameters[(byte)LilyOpKey.GameSettingFriendsOnly];


                int creatGamefee = 0;
                if (BigBlind >= _settingPiaozi)
                {
                    creatGamefee = _settingPiaozi;
                    userData.Chips = userData.Chips - _settingPiaozi;
                    UserService.getInstance().ChangeUserChips(userData.UserId, -_settingPiaozi);
                }

                BankService.getInstance().addRecord(creatGamefee, BankActionType.CreatGame, userId);

                CreateGame();
                InitDelegateEvent();
                GameType = GameType.User;
                game = PokerGame;
            }
            #endregion

            if (game.State== TypeState.Init)
            {
                game.Start();
                RobotService.Singleton.Request(this);
            }

            if (or.Parameters.ContainsKey((byte)LilyOpKey.TakenAmnt))
            {
                long takenAmntParam = (long)or.Parameters[((byte)LilyOpKey.TakenAmnt)];
                takenAmnt = takenAmntParam;
            }

            takenAmnt = Math.Min(userData.Chips, takenAmnt);

            if (takenAmnt < BigBlind)
                takenAmnt = Math.Min(userData.Chips, BigBlind * 10);

            userData.MoneyInitAmnt = takenAmnt;
            userData.MoneySafeAmnt = takenAmnt;

            PlayerInfo player = userData;

            if (or.Parameters.ContainsKey((byte)LilyOpKey.NoSeat))
            {
                player.NoSeat = (int)or.Parameters[(byte)LilyOpKey.NoSeat];
            }
            

            game.SitInGame(player);
           
            if (player.NoSeat != -1)
            {
                userData.IsSitting = true;

                UserService.getInstance().ChangeUserStatus(userId,UserStatus.Playing);

                //this.PublishSitEvent(peer);
                TableInfo gametable = game.Table.ToHashtable().ToTableInfo();
                foreach (PlayerInfo p in gametable.Players)
                {
                    if (player.NoSeat != p.NoSeat&&!p.IsShowingCards)
                    {
                        p.Cards = new[] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                        //PlayerInfo curplayer = game.Table.GetPlayer(p.NoSeat);
                        //if (curplayer.IsPlaying || curplayer.IsAllIn)
                            //dic[(byte)LilyOpKey.PlayerCardId] = curplayer.Cards.Select(r => r.Id).ToArray();
                    }
                    //else { 
                    //        p.Cards = new GameCard[2] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                    //}
                }
                dic.Add((byte)LilyOpKey.TableInfo, gametable.ToHashtable());
                int[] playingNoSeats = game.Table.Players.FindAll(rs => rs.IsPlaying).Select(rs => rs.NoSeat).ToArray();
                int[] allInNoSeats = game.Table.Players.FindAll(rs => rs.IsAllIn).Select(rs => rs.NoSeat).ToArray();

                int[] gameCardIds = game.Table.Cards.Select(rs => rs.Id).ToArray();
                dic[(byte)LilyOpKey.GameId] = gameCardIds;
                dic[(byte)LilyOpKey.GameTypeState] = game.State;
                dic[(byte)LilyOpKey.PlayingNoSeats] = playingNoSeats;
                dic[(byte)LilyOpKey.AllInNoSeats] = allInNoSeats;
                dic[(byte)LilyOpKey.NoSeat] = player.NoSeat;                
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                if (((LilyPeer) peer).OnlineAwards>0)
                {
                    dic[(byte)LilyOpKey.OnlineAwards] = ((LilyPeer) peer).OnlineAwards;
                }

            }
            else
            {
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.TableFull;
            }
            return dic;
        }


        public Dictionary<byte, object> SitCareerTable(UserData userData, OperationRequest or, LitePeer peer)
        {
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            int gradeid = (int)or.Parameters[(byte) LilyOpKey.GameGradeId];

            GameGrade gg = XmlResources.AllGameGrade.FirstOrDefault(r=>r.ID==gradeid) ??
                           XmlResources.AllGameGrade.First();

            string userId = userData.UserId;

            if (PokerGame==null)
            {
                CreateCareerGame(gg);
                InitDelegateEvent();
            }
            PokerGame game = PokerGame;
            
            
            if (game.State == TypeState.Init)
            {
                game.Start();
                //RobotService.Singleton.Request(this);
                
            }

            PlayerInfo player = userData;

            game.SitInGame(player);
            if (player.NoSeat != -1)
            {
                userData.IsSitting = true;
                UserService.getInstance().ChangeUserStatus(userId, UserStatus.Playing);
                TableInfo gametable = game.Table.ToHashtable().ToTableInfo();
                foreach (PlayerInfo p in gametable.Players)
                {
                    if (player.NoSeat != p.NoSeat && !p.IsShowingCards)
                    {
                        p.Cards = new[] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                    }
                }
                dic.Add((byte)LilyOpKey.TableInfo, gametable.ToHashtable());
                int[] playingNoSeats = game.Table.Players.FindAll(rs => rs.IsPlaying).Select(rs => rs.NoSeat).ToArray();
                int[] allInNoSeats = game.Table.Players.FindAll(rs => rs.IsAllIn).Select(rs => rs.NoSeat).ToArray();

                int[] gameCardIds = game.Table.Cards.Select(rs => rs.Id).ToArray();
                dic[(byte)LilyOpKey.GameId] = gameCardIds;
                dic[(byte)LilyOpKey.GameTypeState] = game.State;
                dic[(byte)LilyOpKey.PlayingNoSeats] = playingNoSeats;
                dic[(byte)LilyOpKey.AllInNoSeats] = allInNoSeats;
                dic[(byte)LilyOpKey.NoSeat] = player.NoSeat;
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                if (((LilyPeer)peer).OnlineAwards > 0)
                {
                    dic[(byte)LilyOpKey.OnlineAwards] = ((LilyPeer)peer).OnlineAwards;
                }

            }
            else
            {
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.TableFull;
            }
            return dic;
        }

        /// <summary>
        ///   Sends a <see cref = "JoinEvent" /> to all <see cref = "Actor" />s.
        /// </summary>
        /// <param name = "peer">
        ///   The peer.
        /// </param>
        protected virtual void PublishSitEvent(LilyPeer peer)
        {
            Actor actor = GetActorByPeer(peer);
            if (actor == null)
            {
                return;
            }

            // generate a join event and publish to all actors in the room
            var sitEvent = new SitEvent(actor.ActorNr);

            //if (joinRequest.BroadcastActorProperties)
            //{
            //    joinEvent.ActorProperties = joinRequest.ActorProperties;
            //}

            PublishEvent(sitEvent, Actors, new SendParameters());
        }

        protected override void HandleLeaveOperation(LitePeer peer, LeaveRequest leaveRequest, SendParameters sendParameters)
        {
            RemovePeerFromGame(peer);
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            peer.SendOperationResponse(new OperationResponse { OperationCode = leaveRequest.OperationRequest.OperationCode, Parameters = parameters }, sendParameters);
        
        }

        protected override int RemovePeerFromGame(LitePeer peer)
        {
            // raise leave event
            PublishLeaveEvent(peer, null);

            Actor actor = Actors.RemoveActorByPeer(peer);
            if (actor == null)
            {
                if (Log.IsWarnEnabled)
                {
                    Log.WarnFormat("RemovePeerFromGame - Actor to remove not found for peer: {0}", peer.ConnectionId);
                }

                return -1;
            }

            Log.DebugFormat("RemovePeerFromGame - Actor to remove for peer: {0}", ((LilyPeer)peer).UserId);

            cachedEvents.Remove(actor.ActorNr);



            LilyServer.Actors.RemoveActorByPeer(peer);

            if ((Actors.Count == 0))
            {
                //if (this.PokerGame!=null&&this.PokerGame.Table!=null&&this.PokerGame.Table.Players.Count > 0)
                //{
                //    foreach (PlayerInfo player in this.PokerGame.Table.Players)
                //    {
                //        this.PokerGame.LeaveGame(player);
                //    }
                //}
                //this.PokerGame = null;
            }
            //else
            //{
                //if (this.PokerGame != null)
                //{
                //    LilyPeer lilyPeer = peer as LilyPeer;
                //    if (lilyPeer == null)
                //    {
                //        return -1;
                //    }
                //    string uid = lilyPeer.UserId;
                //    UserData user = UserService.getInstance().QueryUserByUserId(uid).ToUserData();
                //    PlayerInfo peerPlayer = this.PokerGame.Table.GetPlayer(user.Name);
                //    if (peerPlayer != null)
                //    {
                //        //peerPlayer.IsZombie = true;

                //        if (this.PokerGame.State == TypeState.WaitForPlayers || this.PokerGame.State == TypeState.End)
                //        {
                //            //this.PokerGame.LeaveGame(peerPlayer,PlayerLeaveType.Kick);
                //        }
                //        else if (peerPlayer.NoSeat == this.PokerGame.Table.NoSeatCurrPlayer)
                //        {

                //            //if (this.PokerGame.Table.CanCheck(peerPlayer))
                //            //    this.PokerGame.PlayMoney(peerPlayer, 0);
                //            //else
                //            //    this.PokerGame.PlayMoney(peerPlayer, -1);
                //        }
                //    }
                //}
            //}

            return actor.ActorNr;
        }

        protected override void PublishLeaveEvent(LitePeer peer, LeaveRequest leaveRequest)
        {
            //base.PublishLeaveEvent(peer, leaveRequest);
            if (Actors.Count > 0 && peer is LilyPeer)
            {
                Actor actor = GetActorByPeer(peer);
                if (actor != null)
                {
                    //IEnumerable<int> actorNumbers = this.Actors.GetActorNumbers();
                    //var leaveEvent = new LeaveEvent(actor.ActorNr, actorNumbers.ToArray());
                    string userid=(peer as LilyPeer).UserId;
                    var leaveEvent = new LilyLeaveEvent(userid, actor.ActorNr,Name);

                    PublishEvent(leaveEvent, Actors.FindAll(r=>r.Peer!=peer), new SendParameters());
                }
            }
        }

        public void PublishMessageInGame(string message,int noseat)
        {
            BroadcastMessageInTableEvent messageEvent = new BroadcastMessageInTableEvent(message,noseat);
            PublishEventInGame(messageEvent);
        }


        #region PokerEvent

        private void ExperienceUp(PlayerInfo p,ExpType et) {

            UserData u = p as UserData;
            if(u==null) return;
            //if (u.UserType == UserType.Guest && u.Level >= 7)
            //    return;

            UserService.getInstance().addExp(u,et);

            if (p.IsRobot) return;

            if (u.LevelExp-(int)et<0&&u.Level>1)
            {
                //升级奖励
                long upgradeawards = ConfigurationHelp.UpgradeAwards;
                u.Chips = u.Chips + upgradeawards;
                //UserService.getInstance().ChipsChanged(u);
                UserService.getInstance().ChangeUserChips(u.UserId, upgradeawards);
                BankService.getInstance().addRecord(upgradeawards, BankActionType.LevelUp, u.UserId);

            }

            ExperienceAddedEvent experienceAddedEvent = new ExperienceAddedEvent(p.NoSeat, u.Level, u.LevelExp);
            //PublishEventInGame(experienceAddedEvent);
            PublishEventForPlayer(experienceAddedEvent, u.UserId);

        }

        #region PublishEvent
        void PublishEventInGameTest(LiteEventBase e)
        {
            if (PokerGame != null)
            {
                IEnumerable<PeerBase> peers = (from actor in Actors
                                              join playerInfo in
                                                  PokerGame.Table.PlayersAndBystander on ((LilyPeer) actor.Peer).UserId equals
                                                  ((UserData) playerInfo).UserId
                                              where playerInfo.IsRobot == false
                                              select actor.Peer).ToList();
                if (peers.Any())
                {
                    EventData eventData = new EventData(e.Code, e);
                    eventData.SendTo(peers.Distinct(), new SendParameters());
                }
            }
        }
        void PublishEventInGame(LiteEventBase e)
        {

            //OperationRequest op = new OperationRequest((byte)LilyOpCode.PublishInGame);

            //op.Parameters = new Dictionary<byte, object>();
            //op.Parameters.Add((byte)LilyEventKey.Message,e);

            //this.EnqueueOperation(null, op, new SendParameters());
            try
            {
                PublishEventInGameTest(e);
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex.Message, ex);
                PublishEventInGameTest(e);
            }
            //finally
            //{

            //}
        }
        void PublishEventInRoom(LiteEventBase e)
        {
            //if (e.GetType().Name=="PlayerLeavedEvent")
            //    peers = this.Actors.FindAll(r=>(r.Peer as LilyPeer).UserId!=userId).Select(actor => actor.Peer);
            //else
            IEnumerable<PeerBase> peers = Actors.Select(actor => actor.Peer).ToList();

            if (peers.Any())
            {
                EventData eventData = new EventData(e.Code, e);
                eventData.SendTo(peers, new SendParameters());
            }
        }

        void PublishEventForPlayer(LiteEventBase e, string userId)
        {
            if (PokerGame != null)
            {
                IEnumerable<PeerBase> peers = (from actor in Actors
                                              //join playerInfo in
                                              //    this.PokerGame.Table.Players on (actor.Peer as LilyPeer).UserId equals
                                              //    (playerInfo as UserData).UserId
                                              where ((LilyPeer) actor.Peer).UserId == userId //&& playerInfo.NoSeat==e.NoSeat
                                              select actor.Peer).ToList();
                if (peers.Any())
                {
                    EventData eventData = new EventData(e.Code, e);
                    eventData.SendTo(peers, new SendParameters());
                }
            }
        }
        void PublishEventForPlayerStand(LiteEventBase e)
        {
            if (PokerGame != null)
            {
                IEnumerable<PeerBase> peers = (from actor in Actors
                                              join playerInfo in
                                                  PokerGame.Table.Bystander on ((LilyPeer) actor.Peer).UserId equals
                                                  ((UserData) playerInfo).UserId
                                              where playerInfo.IsRobot == false
                                              select actor.Peer).ToList();
                if (peers.Any())
                {
                    EventData eventData = new EventData(e.Code, e);
                    eventData.SendTo(peers, new SendParameters());
                }
            }
        }
        #endregion


        void LilyGamePlayerActionTaken(object sender, PlayerActionEventArgs e)
        {
            PokerGameUsers pokerGame = sender as PokerGameUsers;
            if (pokerGame != null)
            {
                int stepid = pokerGame.GetActionId();
                PlayerAction pa = new PlayerAction(stepid, e.Action, e.Player.NoSeat, e.AmountPlayed, e.Player.MoneySafeAmnt, pokerGame.Table, pokerGame.Table.TotalRounds);

                PlayerTurnEndedEvent playerTurnEndedEvent = new PlayerTurnEndedEvent(e.Player.NoSeat, e.Player.MoneyBetAmnt, e.Player.MoneySafeAmnt, e.Player.IsPlaying, pokerGame.Table.TotalPotAmnt, e.AmountPlayed, e.Action, stepid);
                PublishEventInGame(playerTurnEndedEvent);
                pokerGame.AddAction(pa);

    #if DEBUG
                string cards=string.Empty;
                foreach (GameCard gc in e.Player.Cards)
	{
                    if(gc==null)continue;
                    cards+=gc.ToString();
	}

                Log.DebugFormat("@@@@@@@@@@@@@@@@ stepid:{0}-player:{1},playing: {2},Cards {3}---action:{4} ,betmoney: {5}   ,safemoney:{6} ,seat {7} , {8} --- gamestatus: {9}, tableName:{10}",
                                stepid, ((UserData)e.Player).UserId, e.Player.IsPlaying, cards, e.Action.ToString(), e.AmountPlayed, e.Player.MoneySafeAmnt, e.Player.NoSeat, pokerGame.Table.TotalRounds, pokerGame.State, pokerGame.Table.Name());
 
	#endif

                try
                {
                    foreach (PlayerInfo p in pokerGame.Table.Players)
                    {
                        if (!p.IsRobot)
                        {
                            PlayerInfo p1 = p;
                            Actor actor = Actors.FirstOrDefault((rs => ((LilyPeer)rs.Peer).UserId == ((UserData)p1).UserId));
                            if (actor != null)
                            {
                                LilyPeer peer = actor.Peer as LilyPeer;
                                AchievementManager.Singleton.TryAchieveHole(peer, ((UserData)p1).UserId, p.Cards);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
            }
            //Send(new PlayerTurnEndedCommand(p.NoSeat, p.MoneyBetAmnt, p.MoneySafeAmnt, m_Game.Table.TotalPotAmnt, e.Action, e.AmountPlayed, p.IsPlaying));
        }

        void LilyGameGameBettingRoundEnded(object sender, RoundEventArgs e)
        {
            PokerGame pokergame = sender as PokerGame;
            if(pokergame==null) return;
            List<MoneyPot> pots = new List<MoneyPot>(pokergame.Table.Pots);
            List<long> amounts = new List<long>();
            foreach (MoneyPot pot in pots)
            {
                amounts.Add(pot.Amount);
                Log.DebugFormat("LilyGameEvent GameBettingRoundEnded-MoneyPot: PotId:{0},PotAmnt:{1},AttachedPlayers:{2}", pot.Id,pot.Amount,string.Join("|",pot.AttachedPlayers.Select(r=>r.NoSeat)));

            }

            for (int i = pots.Count; i < pokergame.Table.NbMaxSeats(); i++)
            {
                amounts.Add(0);
            }

            Hashtable hashtable = SerializeHelper.Serialize(amounts);

            BetTurnEndedEvent betTurnEndedEvent = new BetTurnEndedEvent(e.Round, hashtable);
            PublishEventInGame(betTurnEndedEvent);
            //Send(new BetTurnEndedCommand(amounts, e.Round));
//#if DEBUG
//            int totalAmnt = 0;
//            foreach (int pot in amounts)
//            {
//                totalAmnt += pot;
//            }
//            Log.DebugFormat("LilyGameEvent GameBettingRoundEnded: Round {0}, TotalPotAmnt {1}", e.Round, totalAmnt);
//#endif
        }

        void LilyGameGameBlindNeeded(object sender, EventArgs e)
        {

            PokerGame pokergame = sender as PokerGame;
            if (pokergame != null)
            {
                
                //pokergame.PlayBlindMoney();
                TableInfo t = pokergame.Table;
                int[] playingNoSeats = t.Players.FindAll(rs => rs.IsPlaying).Select(rs => rs.NoSeat).ToArray();               

                GameStartedEvent gameStartedEvent = new GameStartedEvent(t.NoSeatDealer, t.NoSeatSmallBlind, t.NoSeatBigBlind, t.TotalRounds,playingNoSeats,t.BigBlindAmnt);
                if (t.m_NoSeatSmallBlind == -1 || t.m_NoSeatBigBlind == -1)
                    return;                
                PublishEventInGame(gameStartedEvent);

                List<UserData> us = t.PlayingPlayers().Select(rs => rs as UserData).ToList();
                UserService.getInstance().SetTotalGame(us);

                int cout = us.Count;
                for (int i = 0; i < cout; i++)
                {
                    UserData curuser = us[i];
                    if (curuser != null)
                        curuser.TotalGame++;
                }
            }
        }
        void LilyGamePlayersShowCards(object sender, PlayersShowCardsArgs e)
        {
            PlayersShowCardsEvent playersshowcardsevent = new PlayersShowCardsEvent {ShowCardPlayers = e.ShowPlayers};
            PublishEventInGame(playersshowcardsevent);
        }


        void LilyGamePlayerHoleCardsChanged(object sender, PlayerInfoEventArgs e)
        {
            PokerGame pokerGame = sender as PokerGame;
            if(pokerGame==null) return;
            PlayerInfo p = e.Player;
            int[] gameCardIds = p.Cards.Select(rs => rs.Id).ToArray();
            PlayerHoleCardsChangedEvent playerHoleCardsChangedEvent = new PlayerHoleCardsChangedEvent(p.NoSeat,p.IsPlaying,gameCardIds);            

            if (pokerGame.State == TypeState.Showdown)
            {
                PublishEventInGame(playerHoleCardsChangedEvent);
            }
            else
            {
                if (p.NoSeat > -1)
                    PublishEventForPlayer(playerHoleCardsChangedEvent, ((UserData) p).UserId);
                else
                {
                    //给站着的人发一个空的手牌事件
                    PlayerHoleCardsChangedEvent standEvent = new PlayerHoleCardsChangedEvent(-1, false, new[] { -1, -1 });
                    PublishEventForPlayerStand(standEvent);
                }
                
                
                //disable in v1.0
                //try
                //{
                //    if (!p.IsRobot)
                //    {
                //        Actor actor = this.Actors.FirstOrDefault((rs => (rs.Peer as LilyPeer).UserId == (p as UserData).UserId));
                //        if (actor != null)
                //        {
                //            LilyPeer peer = actor.Peer as LilyPeer;
                //            AchievementManager.Singleton.TryAchieveHole(peer, (p as UserData).UserId, p.Cards);
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{

                //    Log.Error(ex.Message, ex);
                //}
            }
            //GameCard[] holeCards;
            //if (p.NoSeat == m_Player.NoSeat)
            //    holeCards = p.Cards;
            //else
            //    holeCards = p.RelativeCards;
            //Send(new PlayerHoleCardsChangedCommand(p.NoSeat, p.IsPlaying, holeCards[0].Id, holeCards[1].Id));
        }

        void LilyGameGameEnded(object sender, GameEndEventArgs e)
        {
            GameEndedEvent gameEndedEvent = new GameEndedEvent(e.gameTax);
            PublishEventInGame(gameEndedEvent);
            PokerGame pokerGame = sender as PokerGame;
            if (pokerGame != null)
            {
                int duration = (int)DateTime.Now.Subtract(pokerGame.Table.GameStart).TotalSeconds;
                BankActionType bat = GameType == GameType.System ? BankActionType.GameTaxSystem : BankActionType.GameTax;
                BankService.getInstance().addRecord(e.gameTax, bat, string.Empty, duration);
                foreach (PlayerInfo p in pokerGame.Table.Players)
                {
                    if (p.exp != ExpType.Fold)
                    {
                        ExperienceUp(p, p.exp);
                    }
                    if (p.IsRobot) continue;
                    Actor actor = Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == ((UserData) p).UserId);
                    if (actor == null)
                    {
                        return;
                    }
                    //disable in v1.0
                    LilyPeer peer = Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == ((UserData) p).UserId).Peer as LilyPeer;
                    //if (!p.IsRobot && AchievementManager.Singleton.TryAchieve5((p as UserData).UserId,
                    //    pokerGame.Table.PlayingPlayers()))
                    if (!p.IsRobot && AchievementManager.Singleton.TryAchieve5(((UserData) p).UserId,
                        pokerGame.Table.Players)&&peer!=null)
                    {
                        peer.SendAchievementEvent(5);
                        if (AchievementManager.Singleton.TryAchieve6(peer.UserId))
                        {
                            UserData userData = UserService.getInstance().QueryUserByUserId(peer.UserId).ToUserData();
                            peer.SendAchievementEvent(6, userData.Chips, userData.Level, userData.LevelExp);

                            if (AchievementManager.Singleton.TryAchieve11(peer.UserId))
                            {
                                userData = UserService.getInstance().QueryUserByUserId(peer.UserId).ToUserData();
                                peer.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                            }
                        }
                    }
                    AchievementManager.Singleton.TryAchieveRounds(peer, p);

                    //disable in v1.0
                    try
                    {
                        if (!p.IsRobot)
                        {
                            UserData user = p as UserData;
                            if (user != null)
                            {
                                peer =Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == user.UserId).Peer as LilyPeer;
                                AchievementManager.Singleton.TryAchieve12(peer, user.UserId, user.Chips);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message, ex);

                    }
                }


                if(pokerGame.GameType!=DataPersist.GameType.Career)
                ExecutionFiber.Schedule(() => RobotService.Singleton.Request(this), (e.Delay + 2) * 1000);
                //RobotInfo[] robots = pokerGame.Table.Players.Where(rs => rs.IsRobot).Select(rs => rs as RobotInfo).ToArray();
                //int count = robots.Length;

                //Thread.Sleep(3000);

                //for (int i = 0; i < count; i++)
                //{
                //    if (--robots[i].Alive == 0 ||
                //        robots[i].MoneySafeAmnt < this.BigBlind ||
                //        (robots[i].HasPlayed && RobotHelper.ShouldLeaveTableByPlayerCount(pokerGame.Table.Players.Count)))
                //    {
                //        pokerGame.LeaveGame(robots[i]);
                //    }
                //}
            }


            //房主离开
            if (pokerGame != null && GameType == GameType.User)
            {
                if (pokerGame.Table.PlayersAndBystander.All(r => ((UserData) r).UserId != Name))
                {
                    foreach (PlayerInfo player in pokerGame.Table.Bystander)
                    {
                        PlayerLeavedEvent(pokerGame, new PlayerInfoEventArgs(player, PlayerLeaveType.Kick));
                    }
                    foreach (PlayerInfo player in pokerGame.Table.PlayersAndBystander)
                    {
                        pokerGame.LeaveGame(player,PlayerLeaveType.Kick);
                    }
                    PokerGame = null;
                    Log.DebugFormat("@@@@@@@@@@@@@@@@ 房主已经离开,房间类型：{0},房间号：{1}",GameType,Name);
                }
            }


#if DEBUG
            Log.Info("LilyGameEvent GameEnded");
#endif

            //Send(new GameEndedCommand());
        }

        void LilyGamePlayerWonPot(object sender, PotWonEventArgs e)
        {
            PlayerInfo p = e.Player;
            //PlayerWonPotEvent playerWonPotEvent = new PlayerWonPotEvent(p.NoSeat, e.Id, e.AmountWon, p.MoneySafeAmnt);
            //this.PublishEventInGame(playerWonPotEvent);

            PokerGame pokerGame = sender as PokerGame;
            if(pokerGame==null)return;
            UserData user = p as UserData;
            if(user==null) return;
            if (p.exp == ExpType.Win&&e.Id==0)
            {
                user.Wins++;
                user.BiggestWin = e.AmountWon;
                UserService.getInstance().SetWins(user);
                if (user.Wins == 1)
                {
                    UserMessageService.Singleton.SaveMessage(user.UserId,user.UserId,MessageType.AppScore);
                }
            }

            //Send(new PlayerWonPotCommand(p.NoSeat, e.Id, e.AmountWon, p.MoneySafeAmnt));
            Log.DebugFormat("LilyGameEvent PlayerWonPot: name {0},noSeat {1},amountWon {2},MoneySafeAmnt {3}", p.Name, p.NoSeat, e.AmountWon, p.MoneySafeAmnt);

            //disable in v1.0
            if (!p.IsRobot)
            {
                Actor actor = Actors.FirstOrDefault(rs =>
                                                             {
                                                                 LilyPeer lilyPeer = rs.Peer as LilyPeer;
                                                                 return (lilyPeer != null && lilyPeer.UserId == user.UserId);
                                                             });
                if (actor == null) return;
                LilyPeer peer = actor.Peer as LilyPeer;
                if(peer==null) return;
                if (!user.IsRobot && AchievementManager.Singleton.TryAchieve4(user.UserId, pokerGame.Table.Players))
                {
                    peer.SendAchievementEvent(4);
                    if (AchievementManager.Singleton.TryAchieve6(peer.UserId))
                    {
                        UserData userData = UserService.getInstance().QueryUserByUserId(peer.UserId).ToUserData();
                        peer.SendAchievementEvent(6, userData.Chips, userData.Level, userData.LevelExp);

                        if (AchievementManager.Singleton.TryAchieve11(peer.UserId))
                        {
                            userData = UserService.getInstance().QueryUserByUserId(peer.UserId).ToUserData();
                            peer.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                        }
                    }
                }

                bool winCondition = p.exp == ExpType.Win && e.Id == 0;

                AchievementManager.Singleton.TryAchieveWins(peer, user.UserId, p.MoneySafeAmnt - e.AmountWon, p.Cards, pokerGame.Table.Cards, winCondition);

                if(winCondition)
                    AchievementManager.Singleton.TryAchieveVictor(peer, user.UserId, p, pokerGame, _countAchivment10Times);
            }
        }

        void LilyGamePlayerWonPotImprove(object sender, PotWonImproveEventArgs e)
        {
            PlayerWonImprovePotEvent pwipe = new PlayerWonImprovePotEvent(e.Id, e.Winner, e.WinAmnt, e.AttachedPlayer);
            Log.DebugFormat("LilyGameEvent PlayerWonPotImprove: potid : {0} , winner:{1} ,winamnt:{2},attachedplayer {3}",e.Id,string.Join("|",e.Winner),string.Join("|",e.WinAmnt),string.Join("|",e.AttachedPlayer));

            PublishEventInGame(pwipe);
        }



        void LilyGameCareerGameStarted(object sender,EventArgs e)
        {
            PokerGameCareer game = sender as PokerGameCareer;
            if (game==null)
                return;

            int fee = game.GameGrade.Tickets+game.GameGrade.Tip;
            foreach (PlayerInfo player in game.Table.PlayingPlayers())
            {
                UserData user = player as UserData;
                if (user == null) continue;
                user.Chips = user.Chips - fee;
                //UserService.getInstance().ChipsChanged(user); 
                UserService.getInstance().ChipsChanged(user, -fee);
                PlayerMoneyChangedEvent playerMoneyChangedEvent = new PlayerMoneyChangedEvent(player.NoSeat, player.MoneySafeAmnt, user.Chips);
                PublishEventInGame(playerMoneyChangedEvent);
                //Send(new PlayerMoneyChangedCommand(p.NoSeat, p.MoneySafeAmnt));

            }

            PokerGame.GameStarted -= LilyGameCareerGameStarted;

        }

        /// <summary>
        /// 比赛用，虚拟筹码，不扣除用户真实筹码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LilyGamePlayerMoneyChangedCareer(object sender, PlayerMoneyChangedEventArgs e)
        {
            PlayerInfo p = e.Player;
            //UserData user = p as UserData;
            //user.Chips = user.Chips - e.ChangedAmnt;
            ////UserService.getInstance().ChipsChanged(user); 
            //long winAmnt = -e.ChangedAmnt;
            //if (winAmnt > 0 && user.WinAdds > 0)
            //{
            //    Log.DebugFormat("LilyGameEvent PlayerMoneyWinAdds:{0}", (long)(winAmnt * user.WinAdds));
            //    winAmnt = (long)(winAmnt * (1 + user.WinAdds));
            //}
            //UserService.getInstance().ChipsChanged(user, winAmnt);
            PlayerMoneyChangedEvent playerMoneyChangedEvent = new PlayerMoneyChangedEvent(p.NoSeat, p.MoneySafeAmnt, p.Chips);
            PublishEventInGame(playerMoneyChangedEvent);
            //Send(new PlayerMoneyChangedCommand(p.NoSeat, p.MoneySafeAmnt));

            Log.DebugFormat("LilyGameEvent PlayerMoneyChangedCareer: name {0},noSeat {1},ChangedAmnt {2},MoneySafeAmnt {3}", p.Name, p.NoSeat, e.ChangedAmnt, p.MoneySafeAmnt);
        }

        void LilyGamePlayerMoneyChanged(object sender, PlayerMoneyChangedEventArgs e)
        {
            PlayerInfo p = e.Player;
            UserData user = p as UserData;
            if (user == null) return;
            user.Chips = user.Chips - e.ChangedAmnt;
            //UserService.getInstance().ChipsChanged(user); 
            long winAmnt = -e.ChangedAmnt;
            if (winAmnt>0&&user.WinAdds>0)
            {
                Log.DebugFormat("LilyGameEvent PlayerMoneyWinAdds:{0}",(long)(winAmnt*user.WinAdds));
                winAmnt =(long)(winAmnt * (1 + user.WinAdds));
            }
            UserService.getInstance().ChipsChanged(user,winAmnt);
            PlayerMoneyChangedEvent playerMoneyChangedEvent = new PlayerMoneyChangedEvent(p.NoSeat, p.MoneySafeAmnt, user.Chips);
            PublishEventInGame(playerMoneyChangedEvent);
            //Send(new PlayerMoneyChangedCommand(p.NoSeat, p.MoneySafeAmnt));

            Log.DebugFormat("LilyGameEvent PlayerMoneyChanged: name {0},noSeat {1},ChangedAmnt {2},MoneySafeAmnt {3}", p.Name, p.NoSeat, e.ChangedAmnt, p.MoneySafeAmnt);
        }

        void LilyGameEverythingEnded(object sender, EventArgs e)
        {
            TableClosedEvent tableClosedEvent = new TableClosedEvent();
            PublishEventInGame(tableClosedEvent);
            Log.Debug("LilyGameEvent EverythingEnded");
            //Send(new TableClosedCommand());
            //m_IsConnected = false;
        }
        void lilyGame_DecideWinnersEnded(object sender, EventArgs e)
        {
            PokerGame pokerGame = sender as PokerGame;
            if(pokerGame==null) return;
            GameCard[] g = pokerGame.Table.Cards;
            if (g.Length < 5||g[0].Id==-1) return;
            foreach (PlayerInfo p in pokerGame.Table.Players)
            {
                if (p.Cards[0].Id == -1)
                    continue;
                try
                {
                    uint hv = (uint)p.HandValue;
                    ulong mask = p.EvaluateCardsMaskValue(g);
                    UserData u = (UserData)p;
                    uint ohv = Convert.ToUInt32(((UserData) p).BestHandValue);                    
                    if (hv > ohv)
                    {
                        string bestFiveCards = PokerWorld.HandEvaluator.Hand.BestFiveFromMask(mask);
                        if (hv != new PokerWorld.HandEvaluator.Hand(bestFiveCards, "").HandValue)
                        {
                            continue;
                        }

                        u.BestHand = bestFiveCards;
                        u.BestHandValue = (int)hv;
                        UserService.getInstance().SetBestHand(u);
                    }
                }
                catch(Exception ex)
                {
                    Log.ErrorFormat(ex.Message,ex);
                }
            }
        }       

        void LilyGamePlayerActionNeeded(object sender, HistoricPlayerInfoEventArgs e)
        {
            PlayerInfo p = e.Player;
            PlayerTurnBeganEvent playerTurnBeganEvent = new PlayerTurnBeganEvent(p.NoSeat, e.Last.NoSeat,e.HighBet);
            this.PublishEventInGame(playerTurnBeganEvent);


            //轮到该用户出牌
            if (!p.IsRobot)
            {
                UserData user = (UserData)p;
                LilyPN.SendNotification(user.UserId, NotificationType.ActionNeeded, string.Empty);
            }            

            //Send(new PlayerTurnBeganCommand(p.NoSeat, e.Last.NoSeat));
            if (true)
            {
                
                PokerGameUsers game = sender as PokerGameUsers;
                if(game==null) return;
                int stepid = game.GetActionId();
                TimerHelper.SetTimeout(this.ThinkingTime * 1000d+2300d, delegate
                {
                    if (game.Table.NoSeatCurrPlayer == e.Player.NoSeat && game.GetActionId() == stepid)
                    {
                        ExperienceUp(e.Player, ExpType.Fold);
                        //this.PokerGame.PlayMoney(e.Player, -1);
                        game.autoFold(e.Player);
                        //this.PokerGame.Table.GetPlayer(e.Player.NoSeat).autofold++;                        
                    }
                });
            }

            Log.Debug(string.Format("LilyGameEvent PlayerActionNeeded: "));

        }

        void LilyGameGameBettingRoundStarted(object sender, RoundEventArgs e)
        {
            PokerGameUsers pokerGame = sender as PokerGameUsers;
            if(pokerGame==null) return;
            int[] gameCardIds = pokerGame.Table.Cards.Select(rs => rs.Id).ToArray();
            
            int idCheck = 0;
            switch (e.Round)
            {
                case TypeRound.Preflop:
                    break;
                case TypeRound.Flop:
                    idCheck = gameCardIds[0];
                    break;
                case TypeRound.Turn:
                    idCheck = gameCardIds[3];
                    break;
                case TypeRound.River:
                    idCheck = gameCardIds[4];
                    break;
            }
            if (idCheck < 0)
                gameCardIds = pokerGame.GetGameCards(e.Round);

            Log.DebugFormat("----lilyGame_GameBettingRoundStarted :  5 cards:{0},--tableName:{1}", string.Join(",", pokerGame.Table.Cards.Select(r => r.ToString()).ToArray()),pokerGame.Table.Name());


            BetTurnStartedEvent betTurnStartedEvent = new BetTurnStartedEvent(e.Round, gameCardIds);
            if (e.Round == TypeRound.River || pokerGame.Table.NbPlaying()>1)
                this.PublishEventInGame(betTurnStartedEvent);

            //disable in v1.0
            try
            {
                //
                if (e.Round == TypeRound.River)
                {
                    IEnumerable<PlayerInfo> players = pokerGame.Table.Players.FindAll(r => r.Cards != null && r.Cards[0] != null && r.Cards[0].Id > -1);
                    foreach (PlayerInfo p in players)
                    {
                        if (p.IsRobot) return;
                        LilyPeer peer = this.Actors.Find(r => ((LilyPeer) r.Peer).UserId == ((UserData) p).UserId).Peer as LilyPeer;
                        AchievementManager.Singleton.TryAchieveKenDie(peer, p.Cards, pokerGame.Table.Cards, p.HandType, this._countAchivment17Times);
                    }
                }
            }
            catch (Exception ex)
            {

                Log.Error(ex.Message, ex);
            }


            //Send(new BetTurnStartedCommand(e.Round, c[0].Id, c[1].Id, c[2].Id, c[3].Id, c[4].Id));
            Log.DebugFormat("LilyGameEvent GameBettingRoundStarted: Round {0}",e.Round);

        }

        void LilyGamePlayerJoined(object sender, PlayerInfoEventArgs e)
        {
            PlayerJoinedEvent playerJoinedEvent = new PlayerJoinedEvent(e.Player.ToHashtable());
            this.PublishEventInRoom(playerJoinedEvent);
            Log.DebugFormat("LilyGameEvent PlayerJoined: Player {0}, totalplayers:", e.Player.Name,this.PokerGame.Table.Players.Count);
            //Send(new PlayerJoinedCommand(p.NoSeat, p.Name, p.MoneySafeAmnt));
        }

        private void PlayerLeavedEvent(PokerGame game, PlayerInfoEventArgs e) {
            if (game == this.PokerGame)
            {
                PlayerLeavedEvent playerLeavedEvent = new PlayerLeavedEvent(e.Player.NoSeat)
                                                          {
                                                              PlayerLeaveType = (byte) e.LeaveType,
                                                              MessageContent = e.LeaveMessage,
                                                              NickName = e.Player.Name
                                                          };
                //playerLeavedEvent.Bystander = e.Bystander;
                //playerLeavedEvent.IsKick = e.IsKick;
                this.PublishEventInRoom(playerLeavedEvent);

                //onlineawards
                if (!e.Player.IsRobot)
                {
                    if (e.Player.CanGetOnlineAwards())
                    {
                        PeerBase peer = this.Actors.Select(actor => actor.Peer).FirstOrDefault(r => ((LilyPeer) r).UserId == ((UserData) e.Player).UserId);
                        if (peer != null)
                            ((LilyPeer) peer).OnlineAwards = (int)e.Player.GetOnlineAwards();
                    }
                }
            }
        }

        void LilyGamePlayerLeaved(object sender, PlayerInfoEventArgs e)
        {
            PokerGame pokerGame = sender as PokerGame;

            PlayerLeavedEvent(pokerGame,e);

            if (e.Player.IsRobot)
            {
                RobotInfo robot = (RobotInfo)e.Player;
                robot.HasPlayed = false;
                RobotService.Singleton.RobotQueues[robot.QueueIndex].Enqueue(robot);
            }
            else if (pokerGame != null && !pokerGame.Table.PlayersAndBystander.Exists(rs => !rs.IsRobot))          
            {
                while (pokerGame.Table.Players.Count > 0)
                {
                    pokerGame.LeaveGame(pokerGame.Table.Players[0]);
                }
            }

            //房主离开
            if (((UserData) e.Player).UserId == this.Name)
            {
                if (pokerGame != null && pokerGame.Table.PlayersAndBystander.All(r => ((UserData) r).UserId != this.Name))
                {
                    switch (pokerGame.State)
                    {
                        case TypeState.Init:
                        case TypeState.WaitForPlayers:
                        case TypeState.End:
                            foreach (PlayerInfo player in pokerGame.Table.Bystander)
                            {
                                PlayerLeavedEvent(pokerGame,new PlayerInfoEventArgs(player,PlayerLeaveType.Kick));
                            }

                            foreach (PlayerInfo player in pokerGame.Table.PlayersAndBystander)
                            {
                                pokerGame.LeaveGame(player,PlayerLeaveType.Kick);
                            }
                            pokerGame = null;
                            break;
                    }
                }
            }

            //走光
            if (pokerGame != null && !pokerGame.Table.PlayersAndBystander.Exists(rs => !rs.IsRobot))
            {
                //pokerGame = null;
                PokerGame = null;
            }

            //Send(new PlayerLeftCommand(p.NoSeat));
            Log.DebugFormat("LilyGameEvent PlayerLeaved: Player {0}", e.Player.Name);

        }

        //void lilyGame_GameGenerallyUpdated(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        
        protected override void ExecuteOperation(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            Log.Info("--------------------ExecuteOperation In Thread:" + Thread.CurrentThread.ManagedThreadId);
            switch ((LilyOpCode)operationRequest.OperationCode)
            {
                case LilyOpCode.Fold:
                    {
                        int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
                        if (noSeat < 0) return;
                        PlayerInfo p = this.PokerGame.Table.GetPlayer(noSeat);
                        if (p == null)
                        {
                            string userId = ((LilyPeer) peer).UserId;
                            p = this.PokerGame.Table.Players.FirstOrDefault(r => ((UserData) r).UserId == userId);
                            if (p == null)
                                return;
                        }
                        if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.AutoFold)){
                            if(this.PokerGame.autoFold(p))
                            {
                                ExperienceUp(p, ExpType.Fold);
                            }
                        }
                        else{
                            if (this.PokerGame.PlayMoneyUser(p, -1)) {
                                ExperienceUp(p, ExpType.Fold);
                            }
                        }                    
                        break;
                    }
                case LilyOpCode.Call:
                    {
                        int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
                        PlayerInfo p = this.PokerGame.Table.GetPlayer(noSeat);
                        if (p == null)
                        {
                            string userId = ((LilyPeer) peer).UserId;
                            p = this.PokerGame.Table.Players.FirstOrDefault(r => ((UserData) r).UserId == userId);
                            if (p == null)
                                return;
                        }
                        this.PokerGame.PlayMoneyUser(p, this.PokerGame.Table.CallAmnt(p));
                        break;
                    }
                case LilyOpCode.Raise:
                    {
                        int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
                        long raiseMoney = (long)operationRequest.Parameters[(byte)LilyOpKey.Money];
                        PlayerInfo p = this.PokerGame.Table.GetPlayer(noSeat);
                        if (p == null)
                        {
                            string userId = ((LilyPeer) peer).UserId;
                            p = this.PokerGame.Table.Players.FirstOrDefault(r => ((UserData) r).UserId == userId);
                            if (p == null)
                                return;
                        }
                        this.PokerGame.PlayMoneyUser(p, raiseMoney);//- p.MoneyBetAmnt
                        break;
                    }
                case LilyOpCode.Check: {
                        int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
                        PlayerInfo p = this.PokerGame.Table.GetPlayer(noSeat);
                        if (p == null)
                        {
                            string userId = ((LilyPeer) peer).UserId;
                            p = this.PokerGame.Table.Players.FirstOrDefault(r => ((UserData) r).UserId == userId);
                            if (p == null)
                                return;
                        }
                        this.PokerGame.PlayMoneyUser(p, 0);
                        break;
                    }
                case LilyOpCode.SendChip:
                    {
                        LilyPeer lilyPeer =(LilyPeer)peer ;
                        string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
                        long chip = (long)operationRequest.Parameters[(byte)LilyOpKey.Chip];

                        string senderUserid = lilyPeer.UserId;
                        if (senderUserid == null) return;

                        UserData senderUser = UserService.getInstance().QueryUserByUserId(senderUserid).ToUserData();

                        chip = Math.Min(chip, senderUser.Chips);

                        long reciveChip = Convert.ToInt64(chip * (1-ConfigurationHelp.SENDCHIPSTAX));
                        long curchips=UserService.getInstance().ChangeUserChips(lilyPeer.UserId, -chip);
                        UserService.getInstance().ChangeUserChips(userId, reciveChip);
                        //if (this.PokerGame != null)
                        //{
                        //    if (this.PokerGame.Table.Players.Exists(rs => (rs as UserData).UserId == (peer as LilyPeer).UserId))
                        //    {
                        //        PlayerInfo player = this.PokerGame.Table.Players.FirstOrDefault(rs => (rs as UserData).UserId == (peer as LilyPeer).UserId);
                        //        player.MoneyInitAmnt -= (int)chip;
                        //        player.MoneyBetAmnt -= (int)chip;
                        //    }
                        //    if (this.PokerGame.Table.Players.Exists(rs => (rs as UserData).UserId == userId))
                        //    {
                        //        PlayerInfo player = this.PokerGame.Table.Players.FirstOrDefault(rs => (rs as UserData).UserId == userId);
                        //        player.MoneyInitAmnt += (int)chip;
                        //        player.MoneyBetAmnt += (int)chip;
                        //    }
                        //}
                        Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                        opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                        opParams[(byte)LilyOpKey.Chip] = curchips;
                        lilyPeer.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, opParams), sendParameters);
                        //disable in v1.0
                        if (AchievementManager.Singleton.TryAchieve2(userId))
                        {
                            lilyPeer.SendAchievementEvent(2);
                            if (AchievementManager.Singleton.TryAchieve6(lilyPeer.UserId))
                            {
                                UserData userData = UserService.getInstance().QueryUserByUserId(lilyPeer.UserId).ToUserData();
                                lilyPeer.SendAchievementEvent(6, userData.Chips, userData.Level, userData.LevelExp);

                                if (AchievementManager.Singleton.TryAchieve11(lilyPeer.UserId))
                                {
                                    userData = UserService.getInstance().QueryUserByUserId(lilyPeer.UserId).ToUserData();
                                    lilyPeer.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                                }
                            }
                        }

                        // receiver will change his achivement 12
                        Actor myActor = LilyServer.Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == userId);
                        LilyPeer receiverLilyPeer = null;
                        if (myActor != null)
                        {
                            receiverLilyPeer = myActor.Peer as LilyPeer;
                        }
                        long receiverResultChips = UserService.getInstance().QueryUserByUserId(userId).ToUserData().Chips;
                        AchievementManager.Singleton.TryAchieve12(receiverLilyPeer, userId, receiverResultChips);

                        if (LilyServer.Actors.Exists(rs => ((LilyPeer) rs.Peer).UserId == userId))
                        {
                            Hashtable userData = UserService.getInstance().QueryUserByUserId(lilyPeer.UserId).ToUserData().Tobyte();
                            long totalChip = UserService.getInstance().QueryUserByUserId(userId).chips ?? 0;
                            SendChipEvent e = new SendChipEvent(userData, reciveChip, totalChip);
                            EventData eventData = new EventData(e.Code, e);
                            PeerBase receiverPeer = LilyServer.Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == userId).Peer;
                            eventData.SendTo(new[] { receiverPeer }, sendParameters);
                        }
                        else
                        {
                            UserMessageService.Singleton.SaveMessage(lilyPeer.UserId, userId, MessageType.SendChip, reciveChip.ToString(CultureInfo.InvariantCulture));

                        }
                        //好友挂起或者离线apn
                        LilyPN.SendNotification(userId, NotificationType.SendChips, lilyPeer.UserId);
                        
                        break;
                    }
                case LilyOpCode.SendGift:
                    {
                        Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                        if (this.PokerGame != null)
                        {
                            int giftId=(int)operationRequest.Parameters[(byte)LilyOpKey.GiftId];
                            int unitPrice = GiftManager.Singleton.GiftPrices[giftId];
                            int price = operationRequest.Parameters.ContainsKey((byte)LilyOpKey.NoSeat) ? 
                                unitPrice : unitPrice * this.PokerGame.Table.Players.Count;
                            string senderId = ((LilyPeer) peer).UserId;
                            UserData sender = this.PokerGame.Table.Players.Find(rs => ((UserData) rs).UserId == senderId) as UserData;
                            if (sender != null && sender.Chips > price)
                            {
                                //if (sender.MoneySafeAmnt >= price)
                                //{
                                //    sender.MoneySafeAmnt -= price;
                                //}
                                //else
                                //{
                                //    sender.MoneySafeAmnt = 0;                                    
                                //}
                                sender.Chips = sender.Chips - price;
                                UserService.getInstance().ChangeUserChips(sender.UserId,-price);
                                if (sender.Chips<sender.MoneySafeAmnt)
                                {
                                    sender.MoneySafeAmnt = sender.Chips;
                                }

                                BankService.getInstance().addRecord(price, BankActionType.BuyGift, senderId,(byte)giftId);

                                SendGiftEvent sendGiftEvent = null;
                                if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.NoSeat))
                                {
                                    int noseat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
                                    PlayerInfo player = this.PokerGame.Table.GetPlayer(noseat);
                                    if (player != null)
                                    {
                                        player.GiftId = giftId;
                                        sendGiftEvent = new SendGiftEvent(sender.NoSeat, noseat, giftId, sender.MoneySafeAmnt, sender.Chips);
                                    }
                                }
                                else
                                {
                                     foreach (PlayerInfo player in this.PokerGame.Table.Players)
                                     {
                                         player.GiftId = giftId;
                                     }
                                     sendGiftEvent = new SendGiftEvent(sender.NoSeat, giftId, sender.MoneySafeAmnt, sender.Chips);
                                }
                                this.PublishEventInGame(sendGiftEvent);
                            }
                            else
                            {
                                opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                            }
                        }
                        else
                        {
                            opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.TableNotExist;
                        }
                        break;
                    }
                case LilyOpCode.JoinTable:
                    JoinGameRequest(peer, operationRequest, sendParameters);
                    break;
                case LilyOpCode.QuickLeaveTable:

                    QuickLeaveGameRequest(peer, operationRequest, sendParameters);
                    break;
                case LilyOpCode.LeaveTable:
                    LeaveGameRequest(peer, operationRequest, sendParameters);
                    break;
                case LilyOpCode.StandUp:
                    StandUpRequest(peer, operationRequest, sendParameters);
                    break;

                case LilyOpCode.PublishInGame:

                    LiteEventBase lilyevent=operationRequest.Parameters[(byte)LilyEventKey.Message] as LiteEventBase;
                    PublishEventInGameTest(lilyevent);

                    break;
                case LilyOpCode.SyncGameData:
                    SyncGameDataRequest(peer, operationRequest, sendParameters);
                    break;
                case LilyOpCode.GetOnlineAwards:
                    {
                        GetOnlineAwardsRequest(peer, operationRequest, sendParameters);
                        break;
                    }
                case LilyOpCode.KickPlayer:
                    KickPlayerRequest(peer, operationRequest, sendParameters);
                    break;
                //case LilyOpCode.AddTakenMoney:
                //    AddTakenMoneyRequest(peer, operationRequest, sendParameters);
                //    break;
                default:
                    base.ExecuteOperation(peer, operationRequest, sendParameters);
                    break;
            }
        }



        #region GameRequest

        //private void AddTakenMoneyRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        //{
        //    //牌局内添加筹码
        //    Dictionary<byte, object> opParams = new Dictionary<byte, object>();
        //    long addAmnt = (int)operationRequest.Parameters[(byte)LilyOpKey.TakenAmnt];
        //    string userId = (peer as LilyPeer).UserId;
        //    if (string.IsNullOrEmpty(userId))
        //        return;
        //    PokerGame pokerGame = this.PokerGame;
        //    if (pokerGame == null)
        //        return;

        //    PlayerInfo player = pokerGame.Table.Players.FirstOrDefault(r => (r as UserData).UserId == userId);
        //    if (player == null)
        //        return;

        //    player.MoneySafeAmnt += addAmnt;
        //    player.MoneyInitAmnt += addAmnt;

        //    //opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.Sucess);
        //    //opParams.Add((byte)LilyOpKey.NoSeat, player.NoSeat);
        //    //opParams.Add((byte)LilyOpKey.TakenAmnt,addAmnt);
        //    //peer.SendOperationResponse(new OperationResponse(operationRequest.OperationCode,opParams),sendParameters);
        //    //UserData curUser=UserService.getInstance(

        //    PlayerMoneyChangedEvent playerMoneyChangedEvent = new PlayerMoneyChangedEvent(player.NoSeat, player.MoneySafeAmnt, player.Chips);
        //    this.PublishEventInGame(playerMoneyChangedEvent);

        //}

        private void KickPlayerRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            //被踢玩家
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            string kicker = ((LilyPeer) peer).UserId;
            if (string.IsNullOrEmpty(kicker))
                return;
            PokerGame pokerGame = this.PokerGame;
            if (pokerGame==null)
                return;
            PlayerInfo getOut = pokerGame.Table.GetPlayer(noSeat);

            PlayerInfo fuckYou = pokerGame.Table.Players.FirstOrDefault(r => ((UserData) r).UserId ==kicker);

            if (getOut == null || fuckYou == null)
                return;
            if (fuckYou.Kicked >= ((UserData) fuckYou).VIP)
                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.KickErrorLimit);
            else if (((UserData) getOut).UserId == this.Name) 
                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.KickErrorOwner);
            else if ((getOut as UserData).VIP >= (fuckYou as UserData).VIP)
                opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.KickErrorLevel);
            else if (getOut.IsPlaying || getOut.IsAllIn)
                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.KickErrorPlaying);
            else {
                pokerGame.LeaveGame(getOut,PlayerLeaveType.KickByPlayer,fuckYou.Name);
                opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.Sucess);
                fuckYou.Kicked++;
            }
            peer.SendOperationResponse(new OperationResponse(operationRequest.OperationCode,opParams), sendParameters); 
        }
        private void GetOnlineAwardsRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            PokerGame pokerGame=this.PokerGame;
            if (pokerGame==null)
                return;
            PlayerInfo player = pokerGame.Table.PlayersAndBystander.FirstOrDefault(r => ((UserData) r).UserId == userId);
            if (player == null)
                return;
            long amnt;
            if (((LilyPeer) peer).OnlineAwards > 0)
            {
                amnt = ((LilyPeer) peer).OnlineAwards;
                ((LilyPeer) peer).OnlineAwards = 0;
            }else
                amnt = player.GetOnlineAwards();
            //if (amnt > 0) {
                Dictionary<byte, object> opParams = new Dictionary<byte, object>();
                opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.Sucess);
                opParams.Add((byte)LilyOpKey.OnlineAwards,player.OnlineAwardTimes);
                opParams.Add((byte)LilyOpKey.TakenAmnt, amnt);
                var operationResponse = new OperationResponse(operationRequest.OperationCode,opParams);
                peer.SendOperationResponse(operationResponse, sendParameters);

                if (amnt > 0)
                {
                    UserService.getInstance().ChangeUserChips(userId, amnt);
                    player.Chips += amnt;
                }
            //}            
        }

        private void SyncGameDataRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            PokerGame game = this.PokerGame;            
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            if (game != null&&game.Table.Players.Count>0)
            {
                string userId = ((LilyPeer) peer).UserId;
                //同步牌桌
                if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.TableInfo))
                {
                    TableInfo gametable = game.Table.ToHashtable().ToTableInfo();
                    foreach (PlayerInfo p in gametable.Players)
                    {
                        if (userId != ((UserData) p).UserId && !p.IsShowingCards)
                        {
                            p.Cards = new[] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                            //PlayerInfo curplayer = game.Table.GetPlayer(p.NoSeat);
                            //if (curplayer.IsPlaying || curplayer.IsAllIn)
                            //dic[(byte)LilyOpKey.PlayerCardId] = curplayer.Cards.Select(r => r.Id).ToArray();
                        }
                        //else { 
                        //        p.Cards = new GameCard[2] { new GameCard(GameCardSpecial.Null), new GameCard(GameCardSpecial.Null) };
                        //}
                    }
                    opParams.Add((byte)LilyOpKey.TableInfo, gametable.ToHashtable());
                    Log.DebugFormat("#################### PokerGame has created , id: {0}, players count: {1}", game.Table.Name(), game.Table.Players.Count);
                    int[] playingNoSeats = game.Table.Players.FindAll(rs => rs.IsPlaying).Select(rs => rs.NoSeat).ToArray();
                    int[] allInNoSeats = game.Table.Players.FindAll(rs => rs.IsAllIn).Select(rs => rs.NoSeat).ToArray();
                    opParams[(byte)LilyOpKey.PlayingNoSeats] = playingNoSeats;
                    opParams[(byte)LilyOpKey.AllInNoSeats] = allInNoSeats;
                    opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                }
                else
                {
                    //同步公牌，手牌                   
                    userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
                    int[] gameCardIds = game.Table.Cards.Select(r => r.Id).ToArray();
                    opParams.Add((byte)LilyOpKey.GameCardIds, gameCardIds);
                    PlayerInfo player = game.Table.Players.FirstOrDefault(r => ((UserData) r).UserId == userId);
                    if (player != null)
                    {
                        int[] playerCardIds = player.Cards.Select(r => r.Id).ToArray();
                        opParams.Add((byte)LilyOpKey.PlayerCardId, playerCardIds);
                    }

                    opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                }
            }
            else {
                opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.GameIdNotExists);
                return;
            }
            var operationResponse = new OperationResponse(operationRequest.OperationCode, opParams);
                peer.SendOperationResponse(operationResponse, sendParameters);
        }

        private void StandUpRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            //if (noSeat < 0) return;
            Dictionary<byte, object> dic = this.StandUp(peer as LilyPeer, noSeat);
            //dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(operationResponse, sendParameters);

            // after leave game, will check the achivement 15 and 16
            LilyPeer currentPeer = peer as LilyPeer;
            PlayerInfo currentPlayer = null;
            if (currentPeer != null)
                currentPlayer = UserService.getInstance().QueryUserByUserId(currentPeer.UserId).ToUserData();     
            if (currentPlayer != null)
                AchievementManager.Singleton.TryAchieveRounds(currentPeer, currentPlayer);
        }

        private void JoinGameRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            //gameid, lobbyid, userid, bigBlind                        
            //string lobbyid = operationRequest.Parameters[(byte)LilyOpKey.LoobyId] as string;
            string uid = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            UserData user = UserService.getInstance().QueryUserByUserId(uid).ToUserData();
            //int seat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            LilyGame game = this;
            Dictionary<byte, object> dic = game.SitTable(user, operationRequest, peer);
            //if (gameId != uid)
            //{
            //if (gameId != uid&&(game.PokerGame == null || game.PokerGame.State == PokerWorld.Game.PokerGame.TypeState.End))
            //    {
            //        Log.DebugFormat("------------------------- JoinGameRequest: TableNotExist, request game {0}, userId {1}", gameId, uid);
            //        dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.TableNotExist);
            //    }
            ////}
            //else
            //{
            //    SitRequest sitRequest = new SitRequest(peer.Protocol, operationRequest);
            //    dic = game.SitTable(peer as LilyPeer, uid, gameId, sitRequest, operationRequest);
            //}
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(operationResponse, sendParameters);

            if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
            {
                //disable in v1.0
                if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.InviterId))
                {
                    string inviterId = (string)operationRequest.Parameters[(byte)LilyOpKey.InviterId];
                    if (AchievementManager.Singleton.TryAchieve3(inviterId, uid))
                    {
                        Actor actor = this.Actors.Find(rs => ((LilyPeer) rs.Peer).UserId == inviterId);
                        if (actor != null)
                        {
                            LilyPeer inviter = actor.Peer as LilyPeer;
                            if (inviter != null)
                            {
                                inviter.SendAchievementEvent(3);
                                if (AchievementManager.Singleton.TryAchieve6(inviterId))
                                {
                                    UserData userData = UserService.getInstance().QueryUserByUserId(inviter.UserId).ToUserData();
                                    inviter.SendAchievementEvent(6, userData.Chips, userData.Level, userData.LevelExp);

                                    if (AchievementManager.Singleton.TryAchieve11(inviterId))
                                    {
                                        userData = UserService.getInstance().QueryUserByUserId(inviter.UserId).ToUserData();
                                        inviter.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                                    }
                                }
                            }
                        }
                    }
                }
                game.PokerGame.TryStartGame();
            }
        }

        private void QuickLeaveGameRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters) {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            LilyGame game = this;


            RemovePeerFromGame(peer);


            game.LeaveTable(userId, noSeat);
            //UserService.getInstance().ChangeUserStatus(userId, UserStatus.Idle);
            //dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;

            //PokerGame pokergame = this.PokerGame;
            //if (pokergame != null && pokergame.Table.Players.Count > 0 &&
            //    (!pokergame.Table.PlayersAndBystander.Exists(rs => !rs.IsRobot) || (userId == this.Name && pokergame.State == TypeState.WaitForPlayers)))
            //{
            //    foreach (PlayerInfo player in pokergame.Table.Bystander)
            //    {
            //        PlayerLeavedEvent(pokergame, new PlayerInfoEventArgs(player, PlayerLeaveType.Kick));
            //    }
            //    foreach (PlayerInfo player in pokergame.Table.Players)
            //    {
            //        pokergame.LeaveGame(player, PlayerLeaveType.Kick);
            //    }
            //}
        }

        private void LeaveGameRequest(LitePeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            int noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            LilyGame game = this;
            Dictionary<byte, object> dic = game.LeaveTable(userId, noSeat);
            UserService.getInstance().ChangeUserStatus(userId,UserStatus.Idle);
            //dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(operationResponse, sendParameters);

            PokerGame pokergame = this.PokerGame;
            if (pokergame != null && pokergame.Table.Players.Count > 0 &&
                (!pokergame.Table.PlayersAndBystander.Exists(rs => !rs.IsRobot)||(userId==this.Name&&pokergame.State==TypeState.WaitForPlayers)))
            {
                foreach (PlayerInfo player in pokergame.Table.Bystander)
                {
                    PlayerLeavedEvent(pokergame, new PlayerInfoEventArgs(player, PlayerLeaveType.Kick));
                }
                foreach (PlayerInfo player in pokergame.Table.Players)
                {
                    pokergame.LeaveGame(player,PlayerLeaveType.Kick);
                }
            }

            // after leave game, will check the achivement 15 and 16
            PlayerInfo currentPlayer = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            LilyPeer currentPeer = peer as LilyPeer;
            if(currentPlayer != null && currentPeer != null)
                AchievementManager.Singleton.TryAchieveRounds(currentPeer, currentPlayer);
        }

        void CareerGamePlayerRankChanged(byte[] ranklist)
        {
            PlayerRankChangedEvent playerRankEvent = new PlayerRankChangedEvent(ranklist);
            this.PublishEventInRoom(playerRankEvent);
            Log.DebugFormat("Lee test LilyCareerGameEvent RankChanged: RankList: {0} {1} {2} {3} {4} ", ranklist[0], ranklist[1], ranklist[2], ranklist[3], ranklist[4]);
        }

        #endregion


        protected override void  ProcessMessage(IMessage message)
        {
            Log.Info("--------------------ProcessMessage In Thread:" + Thread.CurrentThread.ManagedThreadId);
            switch ((LilyMessageCode)message.Action)
            {
                case LilyMessageCode.AddRobotInGame:

                    if (!this.OnlyFriend)
                    RobotService.Singleton.AddRobotInGame(message.Message as LilyGame);
                    break;
                case LilyMessageCode.RobotAction:
                    if (this.PokerGame == null)
                        break;
                    this.PokerGame.RobotLogic(message.Message as PlayerInfo);
                    break;
                case LilyMessageCode.RoomTypeChanged:
                    RoomType roomType = (RoomType)message.Message;
                    RoomTypeChangedEvent e = new RoomTypeChangedEvent(roomType);
                    this.PublishEvent(e, this.Actors, new SendParameters());
                    break;
                default:
                    base.ProcessMessage(message);
                    break;
            }
        }
        #endregion
    }
}