using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using DataPersist;
using Lite;
using LilyServer;

namespace Photon.LoadBalancing.GameServer
{
    public sealed class GameLobby
    {
        #region Constants and Fields

        //public readonly TimeSpan JoinTimeOut = TimeSpan.FromSeconds(10);

        //public readonly int MaxPlayersDefault;

        //protected readonly GameList GameList;

        //protected readonly LoadBalancer<IncomingGameServerPeer> LoadBalancer;

        //protected readonly HashSet<PeerBase> peers = new HashSet<PeerBase>();

        //private IDisposable schedule;

        #endregion

        #region Properties

        private PoolFiber ExecutionFiber { get; set; }

        #endregion

        #region Constructors and Destructors

        public GameLobby()
        {   
            this.ExecutionFiber = new PoolFiber();
            this.ExecutionFiber.Start();
        }

        #endregion


        #region Public Methods

        public void EnqueueOperation(GameClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            this.ExecutionFiber.Enqueue(() => this.ExecuteOperation(peer, operationRequest, sendParameters));
        }      

        #endregion


        #region Methods

        private void ExecuteOperation(GameClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            //OperationResponse response;
            //switch ((OperationCode)operationRequest.OperationCode)
            //{
                
              
            //}
            switch ((LilyOpCode)operationRequest.OperationCode)
            {
                case LilyOpCode.QuickStart:
                    QuickStartRequest(peer, operationRequest, sendParameters);
                    break;     
                case LilyOpCode.JoinCareerGame:
                    JoinCareerGameRequest(peer, operationRequest, sendParameters);
                    break;
                    //response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = -1, DebugMessage = "Unknown operation code" };
            }

            //if (response != null)
            //{
            //    peer.SendOperationResponse(response, sendParameters);
            //}
        }

        private void JoinCareerGameRequest(GameClientPeer peer, OperationRequest operationRequest, SendParameters sendParameters)
        {
            const string labby = "Lily_lobby";
            string userId = peer.UserId;
            if(string.IsNullOrEmpty(userId))
                return;
            var dic = new Dictionary<byte, object>();
            //从大厅加入比赛牌桌

            int gradeId = (int)operationRequest.Parameters[(byte) LilyOpKey.GameGradeId];

            GameGrade curgrade = XmlResources.GetGameGrade(gradeId);
            if(curgrade==null) return;

            UserData user = peer.GameQueryUserByUserId(userId);

            if (user.Chips>curgrade.Tickets+curgrade.Tip)
            {
                List<Room> allCareerGameRooms = GameCache.Instance.getAllRooms().FindAll(r =>
                                                                                             {
                                                                                                 var pokerGameCareer = ((LilyGame)r).PokerGame as PokerGameCareer;
                                                                                                 return pokerGameCareer != null && pokerGameCareer.GameType == GameType.Career
                                                                                                                                     && pokerGameCareer.GameGrade.ID == gradeId
                                                                                                                                     && !pokerGameCareer.IsRunning;

                                                                                             });
                if (allCareerGameRooms.Count>0)
                {
                    LilyGame careergame = (LilyGame)allCareerGameRooms.FirstOrDefault();
                    if (careergame != null)
                    {
                        dic = careergame.SitCareerTable(user, operationRequest, peer);
                        if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
                        {
                            OperationRequest operationJoin3 = new OperationRequest((byte)LilyOpCode.Join);
                            if (operationJoin3.Parameters == null)
                                operationJoin3.Parameters = new Dictionary<byte, object>();
                            operationJoin3.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
                            operationJoin3.Parameters.Add((byte)LilyOpKey.GameId, careergame.Name);
                            operationJoin3.Parameters.Add((byte)LilyOpKey.UserId, userId);
                            peer.GameOnOperationRequest(operationJoin3, sendParameters);

                            careergame.PokerGame.TryStartGame();
                        }
                    }
                }

                if (dic.Count==0)
                {
                    string randomname = Guid.NewGuid().ToString().Substring(0, 5);
                    string systemRoomName = string.Format("career{0}:{1}", userId, randomname);
                   
                    OperationRequest operationJoin2 = new OperationRequest((byte)LilyOpCode.Join);
                    if (operationJoin2.Parameters == null)
                        operationJoin2.Parameters = new Dictionary<byte, object>();
                    operationJoin2.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
                    operationJoin2.Parameters.Add((byte)LilyOpKey.GameId, systemRoomName);
                    operationJoin2.Parameters.Add((byte)LilyOpKey.UserId, userId);
                    peer.GameOnOperationRequest(operationJoin2, sendParameters);

                    LilyGame game = (LilyGame)peer.RoomReference.Room;
                    
                    dic = game.SitCareerTable(user,operationRequest, peer);

                    game.PokerGame.TryStartGame();
                }


            }else
            {
                dic.Add((byte)LilyOpKey.ErrorCode,ErrorCode.ChipsNotEnough);
            }
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(operationResponse, sendParameters);

        }

        private void QuickStartRequest(GameClientPeer peer,OperationRequest operationRequest, SendParameters sendParameters)
        {
            //log.Debug("quickstart from gameclientpeer");
            string labby = "Lily_lobby";
            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.LobbyId))
            {
                labby = operationRequest.Parameters[(byte)LilyOpKey.LobbyId] as string;
            }
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            string userId = peer.UserId;
            if (userId == null)
                return;

            //如果已经在牌桌内，先离开牌桌
            string lastGameId = string.Empty;
            int noSeat = -1;
            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.NoSeat))
                noSeat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            if (noSeat > -1)
            {
                OperationRequest leavereques = new OperationRequest((byte)LilyOpCode.QuickLeaveTable);
                leavereques.Parameters = new Dictionary<byte, object>();
                leavereques.Parameters.Add((byte)LilyOpKey.UserId, userId);
                leavereques.Parameters.Add((byte)LilyOpKey.NoSeat, noSeat);

                if (peer.RoomReference != null)
                    lastGameId = peer.RoomReference.Room.Name;

                peer.GameOnOperationRequest(leavereques, sendParameters);
            }


            UserData user = peer.GameQueryUserByUserId(userId);//UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            long takenAmnt = user.Chips / 10;
            //takenAmnt = Math.Min(takenAmnt, 200);




            if (user.Chips < 20)
            {
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                goto Response;
            }


            List<Room> allGameRooms = GameCache.Instance.getAllRooms().FindAll(r =>
                                                                ((LilyGame) r).OnlyFriend == false
                                                                && ((LilyGame) r).PokerGame != null&&((LilyGame)r).PokerGame.GameType!=GameType.Career);
            if (allGameRooms.Count == 0)
                goto CreateSystemRoom;

            List<Room> resultRoom;
            if (20 <= user.Chips && user.Chips <= 1600)
            {
                resultRoom = allGameRooms.FindAll(rs =>
                                                ((LilyGame) rs).PokerGame != null
                                             && ((LilyGame) rs).BigBlind == 4
                                             && ((LilyGame) rs).PokerGame.Table.Players.Count < 9
                                             && ((LilyGame) rs).PokerGame.Table.Players.FirstOrDefault(r => !r.IsRobot) != null);//.OrderBy(rs => Math.Abs((rs as LilyGame).BigBlind - baseChips)).ToList();
            }
            else
            {
                //int maxBigblind = (int)(user.Chips / 240);
                //int minBigblind = (int)(user.Chips / 400);

                long baseChips = RandomRange((int)user.Chips / 400, (int)user.Chips / 240);
                if (baseChips > 500000)
                {
                    baseChips = 500000;
                }
                int maxBigblind = (int)(user.Chips / 40);
                int minBigblind = (int)(user.Chips / 700);

                maxBigblind = Math.Min(maxBigblind, 700000);
                minBigblind = Math.Min(minBigblind, 40000);

                resultRoom = allGameRooms.FindAll(rs =>
                                 ((LilyGame) rs).PokerGame != null
                             && ((LilyGame) rs).BigBlind > minBigblind
                             && ((LilyGame) rs).BigBlind < maxBigblind
                             && ((LilyGame) rs).PokerGame.Table.Players.Count < 9
                             && ((LilyGame) rs).PokerGame.Table.Players.FirstOrDefault(r => !r.IsRobot) != null)
                             .OrderBy(rs => Math.Abs(((LilyGame) rs).PokerGame.Table.BigBlindAmnt - baseChips)).ToList();

            }
            if (!string.IsNullOrEmpty(lastGameId))
                resultRoom = resultRoom.FindAll(rs => rs.Name != lastGameId);


            if (resultRoom.Count == 0)
                goto CreateSystemRoom;

            if (resultRoom.Count > 10)
                resultRoom=resultRoom.GetRange(0, 10);


            int[] weight = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            //将符合条件的room排序
            int roomIndex = weight.Take(resultRoom.Count).Select(r => new KeyValuePair<int, int>(r, r + (new Random(Guid.NewGuid().GetHashCode())).Next(0, 18))).OrderBy(
                     h => h.Value
                     ).FirstOrDefault().Key;

            LilyGame joinGame = null;

            if (roomIndex > -1 && roomIndex < resultRoom.Count)
            {
                joinGame = resultRoom[roomIndex] as LilyGame;
            }

            if (joinGame == null)
                joinGame = (LilyGame)resultRoom.FirstOrDefault();



            //if (takenAmnt < joinGame.BigBlind * 10)
            //    goto CreateSystemRoom;
            //SitRequest sitRequest = new SitRequest(this.Protocol, operationRequest);
            if (joinGame != null) dic = joinGame.SitTable(user, operationRequest, peer);
            if ((int)dic[(byte)LilyOpKey.ErrorCode] != (int)ErrorCode.Sucess)
                goto CreateSystemRoom;

            OperationRequest operationJoin = new OperationRequest((byte)LilyOpCode.Join);
            if (operationJoin.Parameters == null)
                operationJoin.Parameters = new Dictionary<byte, object>();
            operationJoin.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
            operationJoin.Parameters.Add((byte)LilyOpKey.GameId, joinGame.Name);
            operationJoin.Parameters.Add((byte)LilyOpKey.UserId, userId);
            peer.GameOnOperationRequest(operationJoin, sendParameters);


            var or = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(or, sendParameters);

            joinGame.PokerGame.TryStartGame();
            return;

        CreateSystemRoom:

            string randomname = Guid.NewGuid().ToString().Substring(0, 5);
            string systemRoomName = string.Format("system{0}:{1}", userId, randomname);
            //Lite.Caching.RoomReference gr = LilyGameCache.Instance.GetRoomReference(systemRoomName, labby);
            //LilyGame game = gr.Room as LilyGame;
            //LilyGame game = new LilyGame(systemRoomName, labby,GameType.System);
            int userChip;
            if (user.Chips > int.MaxValue)
                userChip = int.MaxValue;
            else userChip = (int)user.Chips;

            int bigBlind;
            if (user.Chips <= 1600)
            {
                bigBlind = 4;
                takenAmnt = Math.Min(user.Chips, 100);
            }
            else if (user.Chips <= 4000)
            {
                bigBlind = RandomRange(userChip / 400, userChip / 240);
            }
            else
            {
                bigBlind = RandomRange(userChip / 400, userChip / 240);
                int length = bigBlind.ToString().Length - 1;
                if (length > 0)
                    bigBlind = bigBlind / (int)Math.Pow(10d, (double)length) * (int)Math.Pow(10d, length);

            }

            bigBlind = Math.Min(bigBlind, 500000);
            user.MoneyInitAmnt = takenAmnt;
            user.MoneySafeAmnt = takenAmnt;
            OperationRequest operationJoin2 = new OperationRequest((byte)LilyOpCode.Join);
            if (operationJoin2.Parameters == null)
                operationJoin2.Parameters = new Dictionary<byte, object>();
            operationJoin2.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
            operationJoin2.Parameters.Add((byte)LilyOpKey.GameId, systemRoomName);
            operationJoin2.Parameters.Add((byte)LilyOpKey.UserId, userId);
            peer.GameOnOperationRequest(operationJoin2, sendParameters);

            LilyGame game = peer.RoomReference.Room as LilyGame;
            dic = game.CreateSystemPokerGame(bigBlind, takenAmnt, user, peer);

        Response:
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            peer.SendOperationResponse(operationResponse, sendParameters);


        }

        private int RandomRange(int min, int max)
        {
            Random r = new Random();
            return r.Next(min, max);
        }
        #endregion
    }
}
