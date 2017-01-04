// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LilyPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LilyPeer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LilyServer
{
    using ExitGames.Logging;

    using LiteLobby;
    using LiteLobby.Operations;

    using Photon.SocketServer;

    using PhotonHostRuntimeInterfaces;

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using DataPersist;
    using Operations;
    using Events;
    using Helper;
    using DataPersist.HelperLib;
    using System.Linq;
    using Lite;
    using Lite.Messages;

    public class LilyPeer : LiteLobbyPeer
    {
        #region Constants and Fields

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public string UserId { get; set; }
        public string GameServerId { get; set; }
        public int OnlineAwards { get; set; }

        #endregion

        #region Constructors and Destructors

        public LilyPeer(IRpcProtocol rpcProtocol, IPhotonPeer peer)
            : base(rpcProtocol, peer)
        {
        }

        #endregion

        #region Methods

        //protected override RoomReference GetRoomReference(JoinRequest joinRequest)
        //{
        //    return LilyGameCache.Instance.GetRoomReference(joinRequest.GameId);
        //}
        #region Join Method
        public void RemovePeer()
        {
            this.RemovePeerFromCurrentRoom();
        }

        protected override void HandleJoinGameWithLobby(JoinRequest joinOperation, SendParameters sendParameters)
        {
            // remove the peer from current game if the peer
            // allready joined another game
            this.RemovePeerFromCurrentRoom();

            // get a game reference from the game cache 
            // the game will be created by the cache if it does not exists allready
            Lite.Caching.RoomReference gameReference = LilyGameCache.Instance.GetRoomReference(joinOperation.GameId, joinOperation.LobbyId);

            // save the game reference in peers state                    
            this.RoomReference = gameReference;

            // enqueue the operation request into the games execution queue
            gameReference.Room.EnqueueOperation(this, joinOperation.OperationRequest, sendParameters);
        }

        protected override void HandleJoinLobby(JoinRequest joinRequest, SendParameters sendParameters)
        {
            // remove the peer from current game if the peer
            // allready joined another game
            this.RemovePeerFromCurrentRoom();

            // get a lobby reference from the game cache 
            // the lobby will be created by the cache if it does not exists allready
            Lite.Caching.RoomReference lobbyReference = LilyRoomCache.Instance.GetRoomReference(joinRequest.GameId);

            // save the lobby(room) reference in peers state                    
            this.RoomReference = lobbyReference;

            // enqueue the operation request into the games execution queue
            lobbyReference.Room.EnqueueOperation(this, joinRequest.OperationRequest, sendParameters);
        }


        protected override void OnDisconnect()
        {
            if(this.UserId!=null)
                UserService.getInstance().ChangeUserStatus(this.UserId,UserStatus.Offline);
            base.OnDisconnect();
        }
        #endregion

        public override bool Equals(object obj)
        {
            LilyPeer lilyPeer = obj as LilyPeer;
            return lilyPeer != null && lilyPeer.UserId == this.UserId;
        }

        #region Request Method
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("OnOperationRequest. Code={0}", operationRequest.OperationCode);
            }
            //if (string.IsNullOrEmpty(this.UserId))
            //{
            //    if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.UserId))
            //    {
            //        this.UserId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            //    }
            //}
            switch ((LilyOpCode)operationRequest.OperationCode)
            {
                case LilyOpCode.Register:
                        RegisterRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.GuestUpgrade:
                        GuestUpgradeRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Logout:
                        LogoutRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Login:
                        LoginRequest(operationRequest, sendParameters);  
                        StatusTipsMsgHander.LoginRequest(operationRequest, sendParameters, this.UserId);
                        break;
                case LilyOpCode.QueryUserById:

                        QueryUserById(operationRequest, sendParameters);
                        break;
                case LilyOpCode.RequestFriend:
                        RequestFriendRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.AddFriend:
                        AddFriendRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.DeleteFriend:
                        DeleteFriendRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.InviteFriend:
                        InviteFriendRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.SearchUser:
                        SearchUserRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.FriendList:
                        FriendListRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.AccessFriend:
                        AccessFriendRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Feedback:
                        FeedbackRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.SaveUserInfo:
                        SaveUserInfoRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.SyncDate:
                        SyncDateRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Sit:
                        SitRequest(operationRequest, sendParameters);
                        break;
                //case LilyOpCode.StandUp:
                //case LilyOpCode.JoinTable:
                //case LilyOpCode.LeaveTable:
                //        this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
                //        break;
                case LilyOpCode.FindPassword:
                        FindPasswordRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.BroadcastMessageInTable:
                        BroadcastMessageInTableRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.BroadcastMessage:
                        BroadcacstMessageRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.RequestRobot:
                        RequestRobotRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.BuyItem:
                        BuyItemRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.BuyitemByChips:
                        
                        this.RequestFiber.Enqueue(()=>BuyitemByChipsRequest(operationRequest,sendParameters));                        
                        //BuyitemByChipsRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.QuickStart:
                        QuickStartRequest(operationRequest,sendParameters);
                        break;
                case LilyOpCode.GetClientVersion:
                        GetClientVersionRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Achieve:
                        AchieveRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.GetAward:
                        GetAwardRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.StandUp:
                case LilyOpCode.JoinTable:
                case LilyOpCode.LeaveTable:
                case LilyOpCode.Fold:
                case LilyOpCode.Check:
                case LilyOpCode.Call:
                case LilyOpCode.Raise:
                case LilyOpCode.SendGift:
                //case LilyOpCode.SendChip:
                case LilyOpCode.KickPlayer:
                case LilyOpCode.SyncGameData:
                case LilyOpCode.GetOnlineAwards:
                case LilyOpCode.AddTakenMoney:
                case LilyOpCode.QuickLeaveTable:
                        //LilyGameCache.Instance.TryGetRoomReference();
                        if (this.RoomReference != null)
                            this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
                        else
                            log.ErrorFormat("##################### OnOperationRequest --{0}, this.RoomReference is null", (LilyOpCode)operationRequest.OperationCode);                        
                        break;
                case LilyOpCode.SendChip:
                        SendChipRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.TryJoinTable:
                        TryJoinGameRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.Suspend:
                        
                        UserStatus us=(UserStatus)operationRequest.Parameters[(byte)LilyOpKey.UserStatus];
                        UserService.getInstance().ChangeUserStatus(this.UserId,us);
                        Dictionary<byte, object> dic = new Dictionary<byte, object>();
                        if (this.UserId == null)
                            dic.Add((byte)LilyOpKey.ErrorCode,ErrorCode.AuthenticationFail);
                        else
                            dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                        var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
                        this.SendOperationResponse(operationResponse, sendParameters);
                        break;
                case LilyOpCode.Setting:
                        //UserData user = ((Hashtable)operationRequest.Parameters[(byte)LilyOpKey.UserData]).ToUserData();
                        string userId=operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
                        bool systemNotify = (bool)operationRequest.Parameters[(byte)LilyOpKey.SystemNotification];
                        bool friendNotification = (bool)operationRequest.Parameters[(byte)LilyOpKey.FriendNotification];
                        UserService.getInstance().SystemSetting(userId, systemNotify, friendNotification);                       
                        Dictionary<byte, object> retdic = new Dictionary<byte, object>();                        
                        retdic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                        var operationResponseSetting = new OperationResponse(operationRequest.OperationCode, retdic);
                        this.SendOperationResponse(operationResponseSetting, sendParameters);
                        break;

                case LilyOpCode.SystemNotice:
                        SystemNoticeRequest(operationRequest, sendParameters);                        
                        break;
                case LilyOpCode.UpgradeUrl:
                        {
                            DeviceType deviceType = (DeviceType)operationRequest.Parameters[(byte)LilyOpKey.DeviceType];
                            int typeid = 10;
                            switch (deviceType)
                            {
                                case DeviceType.UNKNOW:
                                    typeid = 10;
                                    break;
                                case DeviceType.IOS:
                                    typeid = 10;
                                    break;
                                case DeviceType.ANDROID:
                                    typeid = 11;
                                    break;
                                default:
                                    typeid = 10;
                                    break;
                            }
                            string url = ConfigService.getInstance().upgradeurl(typeid);
                            Dictionary<byte, object> urldic = new Dictionary<byte, object>();
                            urldic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                            urldic.Add((byte)LilyOpKey.UpgradeUrl, url);
                            var operationResponseUrl = new OperationResponse(operationRequest.OperationCode, urldic);
                            this.SendOperationResponse(operationResponseUrl, sendParameters);
                            break;
                        }
                case LilyOpCode.CheckEmail:
                        CheckMailRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.GetOnlinePeopleNum:
                        SendOnlinePeopleNumber(operationRequest, sendParameters);
                        break;
                case LilyOpCode.DeviceInfo:
                        DeviceInfoRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.BindChannelId:
                        BindChannelIdRequest(operationRequest, sendParameters);
                        break;
                case LilyOpCode.GameGrades:
                        GetGameGradesRequest(operationRequest, sendParameters);
                    break;
                case LilyOpCode.JoinCareerGame:
                        JoinCareerGameRequest(operationRequest, sendParameters);
                    break;
                default:
                        // for this example all other operations will handled by the base class
                        base.OnOperationRequest(operationRequest, sendParameters);
                        return;
            }    
        }

        private void JoinCareerGameRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = this.UserId;
            if(string.IsNullOrEmpty(userId))
                return;

            UserData user = UserService.getInstance().QueryUserByUserId(userId).ToUserData();

            var opParams = ((LilyGame)this.RoomReference.Room).SitCareerTable(user, operationRequest, this);
            SendOperationResponse(new OperationResponse(operationRequest.OperationCode, opParams), sendParameters);
        }

        private void GetGameGradesRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            var opParams = new Dictionary<byte, object>();
            opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
            opParams.Add((byte)LilyOpKey.GameGrades,SerializeHelper.Serialize(XmlResources.AllGameGrade));
            SendOperationResponse(new OperationResponse(operationRequest.OperationCode, opParams), sendParameters);
        }

        private void BindChannelIdRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //Dictionary<byte,object> opParams=new Dictionary<byte,object>();
            string channelid = operationRequest.Parameters[(byte)LilyOpKey.ChannelId] as string;            
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            //BankService.getInstance().AddDeviceInfo(channelid, deviceToken, deviceType, osversion);
            UserService.getInstance().BindChannelID(channelid,userId);
        }
        private void DeviceInfoRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
           //Dictionary<byte,object> opParams=new Dictionary<byte,object>();
            string channelid = operationRequest.Parameters[(byte)LilyOpKey.ChannelId] as string;
            string deviceType = operationRequest.Parameters[(byte)LilyOpKey.DeviceType] as string;
            string deviceToken = operationRequest.Parameters[(byte)LilyOpKey.DeviceToken] as string;
            string osversion = operationRequest.Parameters[(byte)LilyOpKey.osVersion] as string;
            string gameVersion = operationRequest.Parameters[(byte)LilyOpKey.ClientVersion] as string;
            BankService.getInstance().AddDeviceInfo(channelid,deviceToken,deviceType,osversion);
        }

        private void GetAwardRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Dictionary<byte, object> opParams = AwardManager.Singleton.GetAwards(this.UserId);
            this.SendOperationResponse(new OperationResponse { OperationCode = operationRequest.OperationCode, Parameters = opParams }, sendParameters);
        }

        private void CheckMailRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string email = operationRequest.Parameters[(byte)LilyOpKey.Email] as string;
            Dictionary<byte,object> opParams=UserService.getInstance().CheckMail(email);
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode,opParams),sendParameters);
        }

        protected Dictionary<byte, object> TryJoinGameResult(Room room,UserData user) {
            LilyGame game = room as LilyGame;
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (game.PokerGame == null || game.PokerGame.State == TypeState.End)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.GameIdNotExists);
            }
            else
            {
                dic = game.TryJoinTable(user);
            }
            return dic;
        }

        private void TryJoinGameRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string gameId = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;

            log.DebugFormat("##############  TryJoinGameRequest, gameid : {0}",gameId);
            string uid = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            UserData user = UserService.getInstance().QueryUserByUserId(uid).ToUserData();
            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            Lite.Caching.RoomReference roomr=this.RoomReference;
            //LilyGameCache.Instance.TryGetRoomReference(gameId, out roomr);
            if (roomr == null)
            {
                dic.Add((byte)LilyOpKey.ErrorCode,ErrorCode.GameIdNotExists);
            }
            else {
                dic=TryJoinGameResult(roomr.Room, user);
            }
            //LilyGameCache.Instance.TryGetRoomReference();

            //dic = this.TryJoinTable(user);
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void SystemNoticeRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            string systemNotice = ConfigurationHelp.SystemNotice;
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
            dic.Add((byte)LilyOpKey.SystemNotice,systemNotice);
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        private void QuickStartRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            string labby = "Lily_lobby";
            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.LobbyId))
            {
                labby = operationRequest.Parameters[(byte)LilyOpKey.LobbyId] as string;
            }            
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            string userId = this.UserId;
            if (userId == null)
                return;
            UserData user = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            long baseChips =0;
            long takenAmnt = user.Chips / 10;
            //takenAmnt = Math.Min(takenAmnt, 200);
            if (user.Chips < 20) {
                dic[(byte)LilyOpKey.ErrorCode] = ErrorCode.ChipsNotEnough;
                goto Response;
            }


            List<Room> allGameRooms = LilyGameCache.Instance.getAllRooms().FindAll(r => 
                                                                (r as LilyGame).OnlyFriend == false
                                                                &&(r as LilyGame).PokerGame!=null);
            if (allGameRooms==null ||allGameRooms.Count==0)
                goto CreateSystemRoom;

            List<Room> resultRoom = new List<Room>();
            if (20 <= user.Chips && user.Chips <= 1600)
            {
                baseChips = 4;
                resultRoom = allGameRooms.FindAll(rs => 
                                                (rs as LilyGame).PokerGame != null 
                                             && (rs as LilyGame).BigBlind == 4 
                                             && (rs as LilyGame).PokerGame.Table.Players.Count < 9 
                                             && (rs as LilyGame).PokerGame.Table.Players.FirstOrDefault(r=>!r.IsRobot)!=null);//.OrderBy(rs => Math.Abs((rs as LilyGame).BigBlind - baseChips)).ToList();
            }
            else {
                int maxBigblind = (int)(user.Chips / 240);
                int minBigblind = (int)(user.Chips / 400);
                resultRoom = allGameRooms.FindAll(rs => 
                                                    (rs as LilyGame).PokerGame != null 
                                                 && (rs as LilyGame).BigBlind > minBigblind 
                                                 && (rs as LilyGame).BigBlind < maxBigblind 
                                                 && (rs as LilyGame).PokerGame.Table.Players.Count < 9 
                                                 && (rs as LilyGame).PokerGame.Table.Players.FirstOrDefault(r => !r.IsRobot) != null);//.OrderBy(rs => Math.Abs((rs as LilyGame).BigBlind - baseChips)).ToList();

            }            
            if(resultRoom.Count==0)
                goto CreateSystemRoom;

            LilyGame joinGame = resultRoom.FirstOrDefault() as LilyGame;
            //if (takenAmnt < joinGame.BigBlind * 10)
            //    goto CreateSystemRoom;
            SitRequest sitRequest = new SitRequest(this.Protocol, operationRequest);
            dic = joinGame.SitTable(user, operationRequest,this);
            if ((int)dic[(byte)LilyOpKey.ErrorCode] != (int)ErrorCode.Sucess)
                goto CreateSystemRoom;

            OperationRequest operationJoin = new OperationRequest((byte)LilyOpCode.Join);
            if (operationJoin.Parameters == null)
                operationJoin.Parameters = new Dictionary<byte, object>();
            operationJoin.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
            operationJoin.Parameters.Add((byte)LilyOpKey.GameId,joinGame.Name);
            operationJoin.Parameters.Add((byte)LilyOpKey.UserId,userId);
            base.OnOperationRequest(operationJoin, sendParameters);


            var or = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(or, sendParameters);

            joinGame.PokerGame.TryStartGame();
            return;

        CreateSystemRoom:

            string randomname=Guid.NewGuid().ToString().Substring(0,5);
            string systemRoomName = string.Format("system{0}:{1}", userId, randomname);
            //Lite.Caching.RoomReference gr = LilyGameCache.Instance.GetRoomReference(systemRoomName, labby);
            //LilyGame game = gr.Room as LilyGame;
            //LilyGame game = new LilyGame(systemRoomName, labby,GameType.System);
            int userChip = 0;
            if (user.Chips > int.MaxValue)
                userChip = int.MaxValue;
            else userChip = (int)user.Chips;

            int bigBlind = 0;
            if (user.Chips <= 1600)
            {
                bigBlind = 4;
                takenAmnt = Math.Min(user.Chips, 100);
            }
            else if (user.Chips <= 4000)
            {
                bigBlind = RobotHelper.RandomRange(userChip / 400, userChip / 240);
            }
            else
            {
                bigBlind = RobotHelper.RandomRange(userChip / 400, userChip / 240);
                int length = bigBlind.ToString().Length - 1;
                if (length > 0)
                    bigBlind = bigBlind / (int)Math.Pow(10d, (double)length) * (int)Math.Pow(10d, (double)length);
            }

            //bigBlind = Math.Min(bigBlind, 500000);
            user.MoneyInitAmnt = takenAmnt;
            user.MoneySafeAmnt = takenAmnt;
            OperationRequest operationJoin2 = new OperationRequest((byte)LilyOpCode.Join);
            if (operationJoin2.Parameters == null)
                operationJoin2.Parameters = new Dictionary<byte, object>();
            operationJoin2.Parameters.Add((byte)LilyOpKey.LobbyId, labby);
            operationJoin2.Parameters.Add((byte)LilyOpKey.GameId, systemRoomName);
            operationJoin2.Parameters.Add((byte)LilyOpKey.UserId, userId);
            base.OnOperationRequest(operationJoin2, sendParameters);

            LilyGame game = this.RoomReference.Room as LilyGame;
            dic=game.CreateSystemPokerGame(bigBlind, takenAmnt, user,this);

            Response:            
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
               

        }

        public void RegisterRequest(OperationRequest operationRequest,SendParameters sendParameters)
        {
            //Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.UserData];
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            UserData userData = new UserData();


            userData.Mail = operationRequest.Parameters[(byte)LilyOpKey.Mail] as string;
            userData.Password = operationRequest.Parameters[(byte)LilyOpKey.PassWord] as string;
            userData.NickName = operationRequest.Parameters[(byte)LilyOpKey.NickName] as string;
            userData.UserType = (UserType)operationRequest.Parameters[(byte)LilyOpKey.UserType];

            log.DebugFormat("######################RegisterRequest, mail:{0},nickname:{1}",userData.Mail,userData.NickName);

            if (string.IsNullOrEmpty(userData.Mail)&&userData.UserType==UserType.Normal)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.MailIsEmpty);
                this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, dic), sendParameters);
                return;
            }

            //if (string.IsNullOrEmpty(userData.Password))
            //{
            //    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.PassWordIsEmpty);
            //    this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, dic), sendParameters);
            //    return;
            //}

            userData.Avator = (byte)operationRequest.Parameters[(byte)LilyOpKey.Avator];
            //userData.UserType = UserType.Normal;//operationRequest.Parameters[(byte)LilyOpKey.Mail] as string;
            userData.DeviceType = (DeviceType)operationRequest.Parameters[(byte)LilyOpKey.DeviceType];
            userData.DeviceToken = operationRequest.Parameters[(byte)LilyOpKey.DeviceToken] as string;
            dic = UserService.getInstance().Register(userData, this);

            dic.Add((byte)LilyOpKey.Chip,0);
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);

            Actor actor = new Actor(this);
            if (!LilyServer.Actors.Exists(r => r.Peer.ConnectionId == this.ConnectionId))
            {
                LilyServer.Actors.Add(actor);
            } 
            //注册成功后，保存第一次选择的头像
            if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
            {
                PropItem buyitem = XmlResources.AllProps.FirstOrDefault(r => r.ItemType == (int)ItemType.Avator && r.ItemId == userData.Avator);
                if (buyitem != null)
                    UserService.getInstance().BuyPropsByRmb(this.UserId, buyitem);
            }

        }

        public void GuestUpgradeRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.UserData];
            UserData userData = new UserData();
            userData.UserId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            userData.Mail = operationRequest.Parameters[(byte)LilyOpKey.Mail] as string;
            userData.Password = operationRequest.Parameters[(byte)LilyOpKey.PassWord] as string;
            userData.NickName = operationRequest.Parameters[(byte)LilyOpKey.NickName] as string;
            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.UserType))
                userData.UserType = (UserType)operationRequest.Parameters[(byte)LilyOpKey.UserType];
            else
                userData.UserType = UserType.Normal;
            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.Avator))
                userData.Avator = (byte)operationRequest.Parameters[(byte)LilyOpKey.Avator];
            else
                userData.Avator = 0;
            Dictionary<byte, object> dic = UserService.getInstance().GuestUpgrade(userData, this);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);


            //注册成功后，保存第一次选择的头像
            if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess&&userData.Avator>0)
            {
                PropItem buyitem = XmlResources.AllProps.FirstOrDefault(r => r.ItemType == (int)ItemType.Avator && r.ItemId == userData.Avator);
                if(buyitem!=null)
                UserService.getInstance().BuyPropsByRmb(this.UserId,buyitem);
            }
        }

        public void LogoutRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            Dictionary<byte, object> dic = UserService.getInstance().Logout(userId);
            this.RemovePeerFromCurrentRoom();
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void QueryUserById(OperationRequest operationRequest, SendParameters sendParameters) {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            UserData data = UserService.getInstance().QueryUserOrBotsById(userId);
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (data == null)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.UserNotExist);
            }
            else
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                dic.Add((byte)LilyOpKey.UserData, data.Tobyte());
            }
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);

            log.DebugFormat("------------- Request {0} , userid: {1}", (LilyOpCode)operationRequest.OperationCode,data.UserId);
        }


        public void LoginRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {

            Dictionary<byte, object> dic = new Dictionary<byte, object>();

            if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.UserData))
            {

                Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.UserData];                
                dic = UserService.getInstance().Login(ht, this);                
            }
            else {
                string mail = operationRequest.Parameters[(byte)LilyOpKey.Mail] as string;
                string pwd = operationRequest.Parameters[(byte)LilyOpKey.PassWord] as string;
                UserType usertype = (UserType)operationRequest.Parameters[(byte)LilyOpKey.UserType];
                DeviceType devicetype = (DeviceType)operationRequest.Parameters[(byte)LilyOpKey.DeviceType];
                string devicetoken = operationRequest.Parameters[(byte)LilyOpKey.DeviceToken] as string;
                UserData user = new UserData();
                user.Mail = mail;
                user.Password = pwd;
                user.UserType = usertype;
                user.DeviceType = devicetype;
                user.DeviceToken = devicetoken;
                dic = UserService.getInstance().Login(user.ToHashtable(), this);
            }

            List<UserMessage> messages;
            if (UserMessageService.Singleton.TryGetMessages(this.UserId, out messages))
            {
                dic[(byte)LilyOpKey.Messages] = SerializeHelper.Serialize(messages);
            }

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);


            if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
            {
                List<Props> userpp = UserService.getInstance().GetUserProps(this.UserId);
                if (userpp != null)
                {
                    dic.Clear();
                    dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
                    dic.Add((byte)LilyOpKey.UserProps, SerializeHelper.Serialize(userpp));
                    this.SendOperationResponse(new OperationResponse((byte)LilyOpCode.GetUserProps, dic), sendParameters);
                }
            }
            else {
                return;
            }

            if ((int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
            {
                if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == this.UserId && rs.Peer.ConnectionId != this.ConnectionId))
                {
                    Actor actorToRemove = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == this.UserId&&rs.Peer.ConnectionId!=this.ConnectionId);
                    if (!(actorToRemove.Peer.RemoteIP == this.RemoteIP && actorToRemove.Peer.RemotePort == this.RemotePort))
                    {
                        log.DebugFormat("UserService Login,SameAccountLoginEvent");
                        Events.SameAccountLoginEvent e = new Events.SameAccountLoginEvent();
                        EventData eventData = new EventData(e.Code, e);
                        eventData.SendTo(new PeerBase[1] { actorToRemove.Peer }, new SendParameters());
                        LilyPeer peerToRemove = actorToRemove.Peer as LilyPeer;
                        peerToRemove.RemovePeerFromCurrentRoom();
                    }
                }

                Actor actor = new Actor(this);
                if (!LilyServer.Actors.Exists(r=>r.Peer.ConnectionId==this.ConnectionId))
                {
                    LilyServer.Actors.Add(actor);
                }   
            }

            //move this part to gameserver clientpeer
            //if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.StepID) && (int)dic[(byte)LilyOpKey.ErrorCode] == (int)ErrorCode.Sucess)
            //{
            //    int stepid = (int)operationRequest.Parameters[(byte)LilyOpKey.StepID];
            //    string gameid = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;
            //    int tableroud = (int)operationRequest.Parameters[(byte)LilyOpKey.TableRound];
            //    UserData user = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
            //    Room rrf = LilyGameCache.Instance.getAllRooms().FirstOrDefault(r=>r.Name==gameid);
            //    if (rrf!=null)
            //    {
            //        LilyGame game = rrf as LilyGame;
            //        if (game.PokerGame != null && game.PokerGame.HistoryActions != null && game.PokerGame.Table.Players.Any(p => p.Name == user.Name))
            //        {
            //            dic.Add((byte)LilyOpKey.HistoryAction, game.PokerGame.getHistoricActions(stepid, tableroud).Tobyte());
            //        }
            //    }
            //    //Lite.Caching.RoomReference rrf = null;
            //    //if (LilyGameCache.Instance.TryGetRoomReference(gameid, out rrf))
            //    //{
            //    //    LilyGame game = rrf.Room as LilyGame;
            //    //    if (game.PokerGame != null && game.PokerGame.HistoryActions != null && game.PokerGame.Table.Players.Any(p => p.Name == user.Name))
            //    //    {
            //    //        dic.Add((byte)LilyOpKey.HistoryAction, game.PokerGame.getHistoricActions(stepid, tableroud).Tobyte());
            //    //    }
            //    //}
            //}

            // Try to Call BuyItem request
            if (messages != null && messages.Count > 0)
            {
                UserMessage myAchivement8 = messages.Find(m => m.MessageType == MessageType.BuyChips);
                if (myAchivement8 != null) {
                    ReachAchivement8_11_12();
                }                
            }

            // Try to Check Achiment 12
            UserData achive12_UserData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
            AchievementManager.Singleton.TryAchieve12(this, this.UserId, achive12_UserData.Chips);
        }

        public void RequestFriendRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            string userIdB = this.UserId;
            parameters = FriendService.getInstance().CheckFriend(userId, userIdB);
            if ((ErrorCode)parameters[(byte)LilyOpKey.ErrorCode] != ErrorCode.Sucess)
                return;
            //parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, parameters), sendParameters);
            

            if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == userId))
            {
                PeerBase peer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == userId).Peer;
                UserData userData= UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                LilyRequestFriendEvent e = new LilyRequestFriendEvent(userData.Tobyte());
                EventData eventData = new EventData(e.Code, e);
                eventData.SendTo(new PeerBase[1] { peer }, sendParameters);
            }
            else
            {
                UserMessageService.Singleton.SaveMessage(this.UserId, userId, MessageType.RequestFriend);
            }

            //好友离线apn
            LilyPN.SendNotification(userId, NotificationType.AddFriend, this.UserId);
        }

        public void AddFriendRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.FriendData];
            FriendData friendData = ht.ToFriendData();
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            if (friendData.UserA==friendData.UserB)
            {
                dic.Add((byte)LilyOpKey.ErrorCode, ErrorCode.AuthenticationFail);
            }else
                dic = FriendService.getInstance().Add(friendData);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);

            if ((ErrorCode)dic[(byte)LilyOpKey.ErrorCode] == ErrorCode.Sucess)
            {
                if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == friendData.UserB))
                {
                    Hashtable userData = SerializeHelper.Serialize(UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData());
                    LilyAddFriendEvent e = new LilyAddFriendEvent(userData);
                    EventData eventData = new EventData(e.Code, e);
                    LilyPeer peer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == friendData.UserB).Peer as LilyPeer;
                    peer.SendEvent(eventData, sendParameters);
                    //disable in v1.0
                    if (AchievementManager.Singleton.TryAchieve1(friendData.UserB))
                    {
                        peer.SendAchievementEvent(1);
                        if (AchievementManager.Singleton.TryAchieve6(friendData.UserB))
                        {
                            UserData friend = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                            peer.SendAchievementEvent(6, friend.Chips, friend.Level, friend.LevelExp);

                            if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                            {
                                friend = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                                peer.SendAchievementEvent(11, friend.Chips, friend.Level, friend.LevelExp);
                            }
                        }
                    }
                }
                else
                {


                    //disable in v1.0
                    AchievementManager.Singleton.TryAchieve1(friendData.UserB);
                    AchievementManager.Singleton.TryAchieve6(friendData.UserB);
                    AchievementManager.Singleton.TryAchieve11(friendData.UserB);
                }
                //disable in v1.0
                if (AchievementManager.Singleton.TryAchieve1(this.UserId))
                {
                    this.SendAchievementEvent(1);
                    if (AchievementManager.Singleton.TryAchieve6(this.UserId))
                    {
                        UserData userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                        this.SendAchievementEvent(6, userData.Chips, userData.Level, userData.LevelExp);

                        if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                        {
                            userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                            this.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                        }
                    }
                }
            }
        }

        public void DeleteFriendRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            FriendData friendData = ((Hashtable)operationRequest.Parameters[(byte)LilyOpKey.FriendData]).ToFriendData();
            Dictionary<byte, object> parameters = FriendService.getInstance().Delete(friendData);
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, parameters), sendParameters);

            if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == friendData.UserB))
            {
                PeerBase peer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == friendData.UserB).Peer;
                LilyDeleteFriendEvent e = new LilyDeleteFriendEvent(this.UserId);
                EventData eventData = new EventData(e.Code, e);
                eventData.SendTo(new PeerBase[1] { peer }, sendParameters);
            }
        }

        public void InviteFriendRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string invitedUserId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            string destination = operationRequest.Parameters[(byte)LilyOpKey.Destination] as string;
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == invitedUserId))
            {
                PeerBase peer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == invitedUserId).Peer;
                UserData userData=UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                LilyInviteFriendEvent e = new LilyInviteFriendEvent(userData.Tobyte(), destination);
                if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.GameServerAddress))
                {
                    e.GameServerAddress = operationRequest.Parameters[(byte)LilyOpKey.GameServerAddress] as string;
                }
                else {
                    e.GameServerAddress = "";   
                }                
                EventData eventData = new EventData(e.Code, e);
                eventData.SendTo(new PeerBase[1] { peer }, sendParameters);

                parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            }
            else
            {
                parameters[(byte)LilyOpKey.ErrorCode] = ErrorCode.FriendOffline;

            }
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, parameters), sendParameters);

            NotificationType nt = destination.IndexOf("Room")>-1 ? NotificationType.InviteFriendRoom : NotificationType.InviteFriendGame;
            LilyPN.SendNotification(invitedUserId, nt, this.UserId);

        }

        public void SearchUserRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string nickName = operationRequest.Parameters[(byte)LilyOpKey.NickName] as string;
            Dictionary<byte, object> dic = FriendService.getInstance().Search(nickName, this.UserId);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void FriendListRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            Dictionary<byte, object> dic = FriendService.getInstance().List(userId);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void AccessFriendRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            Dictionary<byte, object> dic = FriendService.getInstance().Access(userId);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void FeedbackRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.Feedback];
            Dictionary<byte, object> dic = UserService.getInstance().Feedback(ht);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void SaveUserInfoRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //Hashtable hashtable = operationRequest.Parameters[(byte)LilyOpKey.UserData] as Hashtable;
            UserData userData = new UserData();//hashtable.ToUserData();
            userData.UserId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            userData.Avator = (byte)operationRequest.Parameters[(byte)LilyOpKey.Avator];
            userData.Password = operationRequest.Parameters[(byte)LilyOpKey.PassWord] as string;
            userData.BackgroundType = (byte)operationRequest.Parameters[(byte)LilyOpKey.BackGroundType];
            userData.LivingRoomType = (RoomType)operationRequest.Parameters[(byte)LilyOpKey.LivingRoomType];
            bool isLivingRoomTypeChanged;
            Dictionary<byte, object> parameters = UserService.getInstance().Save(userData,out isLivingRoomTypeChanged);
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, parameters), sendParameters);

            if (isLivingRoomTypeChanged)
            {
                Lite.Caching.RoomReference rrf;
                if (LilyGameCache.Instance.TryGetRoomReference(this.UserId, out rrf))
                {
                    rrf.Room.EnqueueMessage(new RoomMessage((byte)LilyMessageCode.RoomTypeChanged, userData.LivingRoomType));
                }
                else {
                    RoomType roomType = userData.LivingRoomType;
                    RoomTypeChangedEvent e = new RoomTypeChangedEvent(roomType);
                    var eventData = new EventData(e.Code, e);
                    this.SendEvent(eventData, new SendParameters());
                }
                //if (this.RoomReference != null)
                //    this.RoomReference.Room.EnqueueMessage(new RoomMessage((byte)LilyMessageCode.RoomTypeChanged, userData.LivingRoomType));
            }
            //disable in v1.0
            if (((ErrorCode)parameters[(byte)LilyOpKey.ErrorCode] == ErrorCode.Sucess) &&
                AchievementManager.Singleton.TryAchieve7(userData))
            {
                this.SendAchievementEvent(7);
                if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                {
                    userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                    this.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                }
            }
        }

        public void SyncDateRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Dictionary<byte, object> dic = UserService.getInstance().GetNow();

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void SitRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.Data];
            string lobbyId = operationRequest.Parameters[(byte)LilyOpKey.LobbyId] as string;
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            string gameId = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;
            int chairNumber = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];

            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            //Lite.Caching.RoomReference rr = this.RoomReference;
            LilyGame game = this.RoomReference.Room as LilyGame;

            UserData user = UserService.getInstance().QueryUserByUserId(userId).ToUserData();
            dic = game.SitTable(user,operationRequest,this);

            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);

            game.PokerGame.TryStartGame();
        }

        public void FindPasswordRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Hashtable ht = (Hashtable)operationRequest.Parameters[(byte)LilyOpKey.Data];
            string mail = ht["mail"].ToString();
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            dic = UserService.getInstance().GetPassword(mail);
            var operationResponse = new OperationResponse(operationRequest.OperationCode, dic);
            this.SendOperationResponse(operationResponse, sendParameters);
        }

        public void BroadcastMessageInTableRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string message = operationRequest.Parameters[(byte)LilyOpKey.Message] as string;
            int noseat = (int)operationRequest.Parameters[(byte)LilyOpKey.NoSeat];
            if (noseat == -1)
            {
                return;
            }

            LilyGame lilyGame = this.RoomReference.Room as LilyGame;
            if (lilyGame != null)
            {
                lilyGame.PublishMessageInGame(message,noseat);
            }
        }

        public void BroadcacstMessageRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string message = operationRequest.Parameters[(byte)LilyOpKey.Message] as string;

            string userId = this.UserId;
            if (string.IsNullOrEmpty(userId))
                return;
            

            UserData user = UserService.getInstance().QueryUserByUserId(userId).ToUserData();

            BroadcastMessageEvent e = new BroadcastMessageEvent(message, user.NickName);
            EventData eventData = new EventData(e.Code, e);
            eventData.SendTo(LilyServer.Actors.Select(rs => rs.Peer), new SendParameters());

            BankService.getInstance().addRecord(0,BankActionType.Broadcast,this.UserId);
        }

        public void RequestRobotRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //this.RoomReference.Room.EnqueueOperation(this, operationRequest, sendParameters);
            //string roomId = operationRequest.Parameters[(byte)LilyOpKey.GameId] as string;

            Lite.Room roomId = this.RoomReference.Room;
            RobotService.Singleton.Request(roomId);
        }

        public void BuyItemRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            ItemType itemType = (ItemType)operationRequest.Parameters[(byte)LilyOpKey.ItemType];
            string iapstring = operationRequest.Parameters[(byte)LilyOpKey.IAPString] as string;
            int iapmoney = (int)operationRequest.Parameters[(byte)LilyOpKey.IAPMoney];
            PayWay payWay = (PayWay)operationRequest.Parameters[(byte)LilyOpKey.PayWay];
            string userId = this.UserId;

            log.DebugFormat("OnOperationRequest. Code=128 , payWay={0}",payWay);
            if (payWay==PayWay.IAP)
            {
                if (BankService.getInstance().checkIAPString(iapstring,userId))
                {
                    log.DebugFormat("OnOperationRequest. Code=128 , checkIAPString={0},iapstring={1}", true, iapstring);
                    opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                    opParams[(byte)LilyOpKey.Feedback] = ErrorCode.Sucess;
                }
                else
                {
                    if (IAPReceiptHelper.IsValid(iapstring))
                    {
                        log.DebugFormat("OnOperationRequest. Code=128 , IAPReceiptHelper.IsValid={0},itemType={1}", true,itemType);
                        switch (itemType)
                        {
                            case ItemType.Room:
                                RoomType roomType = (RoomType)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                                opParams = UserService.getInstance().BuyRoom(this.UserId, roomType, null, iapmoney);
                                BankService.getInstance().addUserPayment(itemType, (long)roomType, iapmoney, iapstring, userId);
                                break;
                            case ItemType.Chip:
                                int chip = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                                opParams = UserService.getInstance().BuyChip(this.UserId, chip, iapmoney);
                                BankService.getInstance().addUserPayment(itemType, (long)chip, iapmoney, iapstring, userId);
                                break;
                            case ItemType.Avator:
                            case ItemType.Jade:
                            case ItemType.Lineage:
                                int itemid = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                                PropItem buyitem = XmlResources.AllProps.FirstOrDefault(r => r.ItemType == (int)itemType && r.ItemId == itemid);
                                opParams = UserService.getInstance().BuyPropsByRmb(userId, buyitem);
                                BankService.getInstance().addUserPayment(itemType, (long)itemid, iapmoney, iapstring, userId);
                                break;
                        }

                        StatusTipsMsgHander.BuyItemRequest(operationRequest, sendParameters, this.UserId);
                    }
                    else
                    {
                        log.DebugFormat("OnOperationRequest. Code=128 , IAPReceiptHelper.IsValid={0}", false);
                        opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.VerifyFail);
                    }
                }
            }

            if (payWay==PayWay.Alipay || payWay == PayWay.NineOnePay)
            {
                if (userId == null) return;
                switch (itemType)
                {
                    case ItemType.Room:
                        break;
                    case ItemType.Chip:                        
                        long chip = UserService.getInstance().QueryUserByUserId(userId).chips.Value;
                        opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                        opParams[(byte)LilyOpKey.Chip] = chip;
                        break;
                    case ItemType.Avator:
                    case ItemType.Jade:
                    case ItemType.Lineage:
                        int itemid = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                        PropItem buyitem = XmlResources.AllProps.FirstOrDefault(r => r.ItemType == (int)itemType && r.ItemId == itemid);
                        Props newProps = new Props() { Id=-1,Duration=buyitem.Duration,ItemId=buyitem.ItemId,ItemName=buyitem.ItemName,ItemType=(ItemType)buyitem.ItemType};
                        opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
                        opParams[(byte)LilyOpKey.UserProps] = SerializeHelper.Serialize(newProps);;
                        break;
                }

                StatusTipsMsgHander.BuyItemRequest(operationRequest, sendParameters, this.UserId);
            }

            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, opParams), sendParameters);
            #region Achievement
            //disable in v1.0
            if (itemType == ItemType.Chip &&
           ((ErrorCode)opParams[(byte)LilyOpKey.ErrorCode] == ErrorCode.Sucess))
            {
                this.ReachAchivement8_11_12();
                //if (AchievementManager.Singleton.TryAchieve8(this.UserId))
                //{
                //    this.SendAchievementEvent(8);
                //    if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                //    {
                //        UserData userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                //        this.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                //    }
                //}
                //UserData checkUser = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                //AchievementManager.Singleton.TryAchieve12(this, this.UserId, checkUser.Chips);


                //if (checkUser.Chips>=4000&&AchievementManager.Singleton.TryAchieve12(this.UserId))
                //{
                //    this.SendAchievementEvent(12);
                //    if (AchievementManager.Singleton.TryAchieve35(this.UserId))
                //    {
                //        UserData userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                //        this.SendAchievementEvent(35, userData.Chips, userData.Level, userData.LevelExp);
                //    }
                //}

            }
                
            #endregion
           
        }

        private void BuyitemByChipsRequest(OperationRequest operationRequest, SendParameters sendParameters) {

            
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            ItemType itemType = (ItemType)operationRequest.Parameters[(byte)LilyOpKey.ItemType];
            int itemId = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
            string userId = this.UserId;

            PropItem buyitem = XmlResources.AllProps.FirstOrDefault(r => r.ItemType == (int)itemType&&r.ItemId==itemId);
            if (buyitem == null)
            {
                opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.BuyItemNotFound);
            }
            else
            {
                long needchip = buyitem.Chip;
                switch (itemType)
                {
                    case ItemType.Room:
                        RoomType roomType = (RoomType)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                        opParams = UserService.getInstance().BuyRoom(this.UserId, roomType, needchip,null);
                        break;
                    case ItemType.Chip:
                        break;
                    case ItemType.Avator:
                    case ItemType.Jade:
                    case ItemType.Lineage:
                        //int itemid = (int)operationRequest.Parameters[(byte)LilyOpKey.ItemId];
                        needchip = 70000;
                        if (operationRequest.Parameters.ContainsKey((byte)LilyOpKey.Chip))
                        {
                            needchip = (long)operationRequest.Parameters[(byte)LilyOpKey.Chip];
                        }
                        opParams = UserService.getInstance().BuyPropsByChips(userId,buyitem,needchip);
                        break;
                    default:
                        break;
                }

                StatusTipsMsgHander.BuyItemRequest(operationRequest, sendParameters, this.UserId);
            }
            if(!opParams.ContainsKey((byte)LilyOpKey.ErrorCode))
                opParams.Add((byte)LilyOpKey.ErrorCode,ErrorCode.Sucess);
            this.SendOperationResponse(new OperationResponse((byte)LilyOpCode.BuyItem,opParams),sendParameters);
        }

        private void SendChipRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {     
            LilyPeer lilyPeer = this;
            string userId = operationRequest.Parameters[(byte)LilyOpKey.UserId] as string;
            long chip = (long)operationRequest.Parameters[(byte)LilyOpKey.Chip];

            string senderUserid = lilyPeer.UserId;
            if (senderUserid == null) return;

            UserData senderUser = UserService.getInstance().QueryUserByUserId(senderUserid).ToUserData();

            chip = Math.Min(chip, senderUser.Chips);

            long reciveChip = Convert.ToInt64(chip * (1 - ConfigurationHelp.SENDCHIPSTAX));
            long curchips = UserService.getInstance().ChangeUserChips(lilyPeer.UserId, -chip);
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
            if (AchievementManager.Singleton.TryAchieve2(senderUserid))
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
            Actor myActor = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == userId);
            LilyPeer receiverLilyPeer = null;
            if (myActor != null) {
                receiverLilyPeer = myActor.Peer as LilyPeer;
            }
            long receiverResultChips = UserService.getInstance().QueryUserByUserId(userId).ToUserData().Chips;
            AchievementManager.Singleton.TryAchieve12(receiverLilyPeer, userId, receiverResultChips);

            if (LilyServer.Actors.Exists(rs => (rs.Peer as LilyPeer).UserId == userId))
            {
                Hashtable userData = UserService.getInstance().QueryUserByUserId(lilyPeer.UserId).ToUserData().Tobyte();
                long totalChip = UserService.getInstance().QueryUserByUserId(userId).chips ?? 0;
                SendChipEvent e = new SendChipEvent(userData, reciveChip, totalChip);
                EventData eventData = new EventData(e.Code, e);
                PeerBase receiverPeer = LilyServer.Actors.Find(rs => (rs.Peer as LilyPeer).UserId == userId).Peer;
                eventData.SendTo(new PeerBase[1] { receiverPeer }, sendParameters);

            }
            else
            {
                UserMessageService.Singleton.SaveMessage(lilyPeer.UserId, userId, MessageType.SendChip, reciveChip.ToString());

            }
            //好友挂起或者离线apn
            LilyPN.SendNotification(userId, NotificationType.SendChips, lilyPeer.UserId);
        }

        public void GetClientVersionRequest(OperationRequest operationRequest,SendParameters sendParameters)
        {
            string clientVersion = ConfigurationHelp.ClientVersion;//ConfigurationManager.AppSettings["ClientVersion"].ToString();
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            opParams[(byte)LilyOpKey.ClientVersion] = clientVersion;
            opParams[(byte)LilyOpKey.ErrorCode] = ErrorCode.Sucess;
            this.SendOperationResponse(new OperationResponse(operationRequest.OperationCode, opParams), new SendParameters());
        }

        public void AchieveRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            byte achievementNumber = (byte)operationRequest.Parameters[(byte)LilyOpKey.AchievementNumber];
            switch (achievementNumber)
            {
                case 10:
                    if (AchievementManager.Singleton.TryAchieve10(this.UserId))
                    {
                        this.SendAchievementEvent(10);
                        if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                        {
                            UserData userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                            this.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                        }
                    }
                    break;
            }
        }

        public void SendAchievementEvent(byte achievementNumber)
        {
            AchievementEvent e = new AchievementEvent(achievementNumber);
            EventData eventData = new EventData(e.Code, e);
            this.SendEvent(eventData, new SendParameters());
        }

        public void SendAchievementEvent(byte achievementNumber, long chip, int level, long levelExp)
        {
            string content = chip + "|" + level + "|" + levelExp;
            AchievementEvent e = new AchievementEvent(achievementNumber, content);
            EventData eventData = new EventData(e.Code, e);
            this.SendEvent(eventData, new SendParameters());
        }

        public void SendOnlinePeopleNumber(OperationRequest operationRequest, SendParameters sendParameters)
        {
            int number = LilyServer.Actors.Count;
            Random rand = new Random();
            int result = (int)(Math.Sqrt(2.0 * 5800 * number) + number) + rand.Next(1, 50);

            Dictionary<byte, object> opParams = new Dictionary<byte, object>();           
            opParams.Add((byte)LilyOpKey.ErrorCode, ErrorCode.Sucess);
            opParams.Add((byte)LilyOpKey.OnlinePeopleNum, result);

            var opOnlineNum = new OperationResponse(operationRequest.OperationCode, opParams);
            this.SendOperationResponse(opOnlineNum, new SendParameters());
        }

        #endregion


        protected UserData QueryUserByUserId(string userId) {
            //UserData user = null;
            return UserService.getInstance().QueryUserByUserId(userId).ToUserData(); ;
        }

        private void ReachAchivement8_11_12() {
            if (AchievementManager.Singleton.TryAchieve8(this.UserId))
            {
                this.SendAchievementEvent(8);
                if (AchievementManager.Singleton.TryAchieve11(this.UserId))
                {
                    UserData userData = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
                    this.SendAchievementEvent(11, userData.Chips, userData.Level, userData.LevelExp);
                }
            }
            UserData checkUser = UserService.getInstance().QueryUserByUserId(this.UserId).ToUserData();
            AchievementManager.Singleton.TryAchieve12(this, this.UserId, checkUser.Chips);
        }

        #endregion
    }
}