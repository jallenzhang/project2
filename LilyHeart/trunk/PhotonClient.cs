using System;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Lite;
using System.Collections;
using System.Collections.Generic;
using DataPersist.HelperLib;
using DataPersist;
using UnityEngine;
using DataPersist.CardGame;
using System.Threading;
using System.Timers;
using System.Text.RegularExpressions;
using System.Text;

namespace LilyHeart
{
	public class SelfPeer:LitePeer
	{
		public SelfPeer(IPhotonPeerListener listener)
			:base(listener,ConnectionProtocol.Tcp)
		{
		}
		
		public string ServerName { get; set; }
		public string ApplicationName { get; set; }
		public byte NodeId  { get; set; }
	}	
	
	
	public enum GameServerStatus 
	{		
		DisconnetByInviter,
		GotoFriendRoom,
		Connet,
		AskForTheGameServer
	}
	
	public enum SignalValue
	{
		Perfact = 1,
		VeryGood,
		Good,
		Bad,
		VeryBad,
		NoSignal
	}
	
	public class PhotonClient:IPhotonPeerListener
	{
		#region Member Const Fields
 
		//private const string SERVER_NAME="10.0.1.163:4530";
		//private const string SERVER_NAME="test.toufe.com:4530";
 
//		private const string SERVER_NAME="10.0.1.8:5055";
	 	//private const string APPLICATION_NAME="LilyServer";
		//private const string APPLICATION_NAME="Master";
		private const string LOBBY_ID="Lily_lobby";	
		private const bool LOADBALANCE=true;
		
		public const string ACTION_CHECK = "CHECK";
		public const string ACTION_CALL = "CALL";
		public const string ACTION_BET = "BET";
		public const string ACTION_RAISE = "RAISE";
		public const string ACTION_FOLD = "FOLD";
		public const string ACTION_ALLIN = "AllIn";
		public const string ACTION_WIN="Win";
		public const int DISCONNECT_TIMEOUT=20000;
		#endregion
		
		#region Member Fields
		public string ServerName {get;set;}
		public string ApplicaitonName {get;set;}
		private static SelfPeer peer_toMasterServer = null;
		private static SelfPeer peer_toGameServer = null;
		private static PhotonClient singleton;
		private int actorCount=0;
		public Action CurrentAction;
		private Action CurrentActionAfterConnect;		
		private Action QuickStartAction;

		public bool isConnected=false;
		private object tempObj=null;
		public string InviterId {get;set;}
		public string InviterRoom{get;set;}
		//public string InviterServer{get;set;}
		public string InviterAppName{get;set;}
		public bool isFastStart = false;
		public SignalValue signalValue = SignalValue.Perfact;
		public GameServerStatus gameSeverStatus;
		public bool bNetworkError = false;
		public string bSendUserID = string.Empty;
		public long lSendchip = 0;
		public UserData PriRoomUser = null;
        public Action<int> SetOnlinePeopleNumberAction;
		public Action AutoLogin;
		public Action SavePersonalAccountInfo;
		public Action CloseMaskingTable;
		public Action RoomDataChanged;
		#endregion
		
		#region event fields
		private static Action searchUserFinished;
		private static Action<bool,TypeState> joinGameFinished;
		private static Action<ErrorCode,TypeState> joinGameEventFinished;
		
		private static Action betTurnEndedEvent;
		private static Action startGameEvent;
		private static Action<long> endGameEvent;
		private static Action betTurnStartEvent; 
		private static Action guestLevelLimitedEvent; 
		
		private static Action<int,long,int> playerWonPotEvent;
		
		private static  Action<PlayerInfo> toPlayerHoleCardsChangedEvent;
		
		public static Action<int,bool> playerTurnBeganEvent;
		
		public static Action<int,bool> playerTurnEndedEvent;
		
		private static  Action<int> playerJoinedEvent;
		
		private static  Action<int,bool,PlayerLeaveType> playerLeavedEvent;
		
		private static  Action  inRoomPlayerLeavedEvent;
		
		private static  Action<UserData>  leaveRoomEventFinished;
		
		private static  Action<int> standUpEvent;
		private static  Action<TypeState> sitDownEvent;
		
		private static  Action<int,int,int> sendGiftEvent;  
		
		private static  Action<int,string>broadcastMessageInTableEvent;
		
		private static  Action<UserData> queryUserEvent;
		
		private static  Action<bool> friendListEvent;
		
		private static  Action joinRoomEvent;
	
		private static  Action<bool> registerOrUpgradeEvent;
		
		private static  Action roomTypeChanged;
		
		private static  Action<int> levelUpEvent; 
		
		private static  Action<bool> tryJointGameEvent;
		
		private static  Action<bool> logoutEvent;
		
		private static  Action<UserData> joinRoomEventFinished;
		
		private static  Action sameAccountLoginEvent;
		
		private static  Action<int,int[],long[],int[]>	playerWonPotImproveEvent;
		
		private static  Action<string> upgradUrlEvent;
		
		private static  Action gotoBackgroundSceneEvent;
		private static  Action gotoLoadingSceneEvent;
		
		private static  Action<Dictionary<int,string>> didPlayersShowCardsFinished; 
		
		private static  Action<bool> syncGameDataTableInfoEvent;
		
		private static  Action leaveGameFinished;
		
		private static  Action maskingTablePopUPEvent;
		
		private static  Action<bool, ItemType> buyItemResponseEvent;
		
		private static  Action clickFriendInfoEvent;
		
		private static  Action<Props> buyPropFinishedEvent;
		
		private static  Action<long> onlineAwardsResponseEvent;
		
		private static  Action loginUserNameOrPasswordErrorEvent;
		
		private static  Action<ErrorCode> doNotSuccessKickPlayerEven;
		
		private static  Action sendChipMessageFinishEvent;
	
		private static  Action saveInfoSuccessEvent;
		
		private static  Action registerInSetupSuccessEvent;
		
		public static event Action SearchUserFinished { add { searchUserFinished+=value;} remove { searchUserFinished-=value;}}
		public static event Action<bool,TypeState> JoinGameFinished {add { joinGameFinished+=value;} remove { joinGameFinished-=value;}}
		public static event Action<ErrorCode,TypeState> JoinGameEventFinished {add { joinGameEventFinished+=value;} remove { joinGameEventFinished-=value;}}
		
		public static event Action BetTurnEndedEvent {add { betTurnEndedEvent+=value;} remove { betTurnEndedEvent-=value;}}
		public static event Action StartGameEvent {add { startGameEvent+=value;} remove { startGameEvent-=value;}}
		public static event Action<long> EndGameEvent {add { endGameEvent+=value;} remove { endGameEvent-=value;}}
		public static event Action BetTurnStartEvent {add { betTurnStartEvent+=value;} remove { betTurnStartEvent-=value;}}
		public static event Action GuestLevelLimitedEvent {add { guestLevelLimitedEvent+=value;} remove { guestLevelLimitedEvent-=value;}}
		
		public static event Action<int,long,int> PlayerWonPotEvent {add { playerWonPotEvent+=value;} remove { playerWonPotEvent-=value;}}
		
		public static event Action<PlayerInfo> ToPlayerHoleCardsChangedEvent {add { toPlayerHoleCardsChangedEvent+=value;} remove { toPlayerHoleCardsChangedEvent-=value;}}
		
		public static event Action<int,bool> PlayerTurnBeganEvent {add { playerTurnBeganEvent+=value;} remove { playerTurnBeganEvent-=value;}}
		
		public static event Action<int,bool> PlayerTurnEndedEvent {add { playerTurnEndedEvent+=value;} remove { playerTurnEndedEvent-=value;}}
		
		public static event Action<int> PlayerJoinedEvent {add { playerJoinedEvent+=value;} remove { playerJoinedEvent-=value;}}
		
		public static event Action<int,bool,PlayerLeaveType> PlayerLeavedEvent {add { playerLeavedEvent+=value;} remove { playerLeavedEvent-=value;}}
		
		public static event Action  InRoomPlayerLeavedEvent {add { inRoomPlayerLeavedEvent+=value;} remove { inRoomPlayerLeavedEvent-=value;}}
		
		public static event Action<UserData>  LeaveRoomEventFinished {add { leaveRoomEventFinished+=value;} remove { leaveRoomEventFinished-=value;}}
		
		public static event Action<int> StandUpEvent {add { standUpEvent+=value;} remove { standUpEvent-=value;}}
		public static event Action<TypeState> SitDownEvent {add { sitDownEvent+=value;} remove { sitDownEvent-=value;}}
		
		public static event Action<int,int,int> SendGiftEvent {add { sendGiftEvent+=value;} remove { sendGiftEvent-=value;}}
		
		public static event Action<int,string>BroadcastMessageInTableEvent {add { broadcastMessageInTableEvent+=value;} remove { broadcastMessageInTableEvent-=value;}}
		
		public static event Action<UserData> QueryUserEvent {add { queryUserEvent+=value;} remove { queryUserEvent-=value;}}
		
		public static event Action<bool> FriendListEvent {add { friendListEvent+=value;} remove { friendListEvent-=value;}}
		
		public static event Action JoinRoomEvent {add { joinRoomEvent+=value;} remove { joinRoomEvent-=value;}}
		
		public static event Action<bool> RegisterOrUpgradeEvent {add { registerOrUpgradeEvent+=value;} remove { registerOrUpgradeEvent-=value;}}
		
		public static event Action RoomTypeChanged {add { roomTypeChanged+=value;} remove { roomTypeChanged-=value;}}
		
		public static event Action<int> LevelUpEvent {add { levelUpEvent+=value;} remove { levelUpEvent-=value;}}
		
		public static event Action<bool> TryJointGameEvent {add { tryJointGameEvent+=value;} remove { tryJointGameEvent-=value;}}
		
		public static event Action<bool> LogoutEvent {add { logoutEvent+=value;} remove { logoutEvent-=value;}}
		
		public static event Action<UserData> JoinRoomEventFinished {add { joinRoomEventFinished+=value;} remove { joinRoomEventFinished-=value;}}
		
		public static event Action SameAccountLoginEvent {add { sameAccountLoginEvent+=value;} remove { sameAccountLoginEvent-=value;}}
		
		public static event Action<int,int[],long[],int[]>	PlayerWonPotImproveEvent {add { playerWonPotImproveEvent+=value;} remove { playerWonPotImproveEvent-=value;}}
		
		public static event Action<string> UpgradUrlEvent {add { upgradUrlEvent+=value;} remove { upgradUrlEvent-=value;}}
		
		public static event Action GotoBackgroundSceneEvent {add { gotoBackgroundSceneEvent+=value;} remove { gotoBackgroundSceneEvent-=value;}}
		public static event Action GotoLoadingSceneEvent {add { gotoLoadingSceneEvent+=value;} remove { gotoLoadingSceneEvent-=value;}}
		
		public static event Action<Dictionary<int,string>> DidPlayersShowCardsFinished {add { didPlayersShowCardsFinished+=value;} remove { didPlayersShowCardsFinished-=value;}}
		
		public static event Action<bool> SyncGameDataTableInfoEvent {add { syncGameDataTableInfoEvent+=value;} remove { syncGameDataTableInfoEvent-=value;}}
		
		public static event Action LeaveGameFinished {add { leaveGameFinished+=value;} remove { leaveGameFinished-=value;}}
		
		public static event Action MaskingTablePopUPEvent {add { maskingTablePopUPEvent+=value;} remove { maskingTablePopUPEvent-=value;}}
		
		public static event Action<bool, ItemType> BuyItemResponseEvent {add { buyItemResponseEvent+=value;} remove { buyItemResponseEvent-=value;}}
		
		public static event Action ClickFriendInfoEvent {add { clickFriendInfoEvent+=value;} remove { clickFriendInfoEvent-=value;}}
		
		public static event Action<Props> BuyPropFinishedEvent {add { buyPropFinishedEvent+=value;} remove { buyPropFinishedEvent-=value;}}
		
		public static event Action<long> OnlineAwardsResponseEvent {add { onlineAwardsResponseEvent+=value;} remove { onlineAwardsResponseEvent-=value;}}
		
		public static event Action LoginUserNameOrPasswordErrorEvent {add { loginUserNameOrPasswordErrorEvent+=value;} remove { loginUserNameOrPasswordErrorEvent-=value;}}
		
		public static event Action<ErrorCode> DoNotSuccessKickPlayerEven {add { doNotSuccessKickPlayerEven+=value;} remove { doNotSuccessKickPlayerEven-=value;}}
		
		public static event Action SendChipMessageFinishEvent {add { sendChipMessageFinishEvent+=value;} remove { sendChipMessageFinishEvent-=value;}}
		
		public static event Action SaveInfoSuccessEvent {add { saveInfoSuccessEvent+=value;} remove { saveInfoSuccessEvent-=value;}}
		
		public static event Action RegisterInSetupSuccessEvent {add { registerInSetupSuccessEvent+=value;} remove { registerInSetupSuccessEvent-=value;}}
		
		#endregion
		
		#region Member Properties
		public static PhotonClient Singleton{
			get {
				if(singleton==null)
				{
					singleton=new PhotonClient();
				}
				return singleton;
			}
		}
		
		public int ActorCount{get{return actorCount;}}
		public string ErrorMessage {get;private set;}
		public bool IsConnected {get {return isConnected;}}
		public bool Connected {get {return !(peer_toMasterServer.PeerState==PeerStateValue.Disconnected||peer_toGameServer.PeerState==PeerStateValue.Disconnected);}}
		#endregion
		
		#region Member Function
		#region Custom Method

	    private PhotonClient ()
		{
			peer_toMasterServer = new SelfPeer(this);
			peer_toMasterServer.DisconnectTimeout = 5000;
			this.ErrorMessage="Default Message";
			if (!LOADBALANCE) {
				peer_toGameServer=peer_toMasterServer;
			}
		}
		public void ReconnectOperation()
		{
			this.CurrentActionAfterConnect = AutoLogin;
			ReConnectToServer();
		}
		
		public static void ResetPhotonClient()
		{
			singleton = null;
		}
		
		public void Connect()
		{
			peer_toMasterServer.Connect(ServerName,ApplicaitonName);
		}
		
		public void ConnectToGameServer(){
			if(peer_toGameServer != null)
			{
				if(peer_toGameServer.PeerState == PeerStateValue.Disconnected)
				{					
					string t_address = peer_toGameServer.ServerName;
					string t_appid = peer_toGameServer.ApplicationName;
					byte t_nodeid = peer_toGameServer.NodeId;
					peer_toGameServer.Connect(t_address, t_appid, t_nodeid);
				}
			}			
		}
		
		public void PopUpMaskingTable()
		{
			if (maskingTablePopUPEvent != null && !User.Singleton.MaskingTableOpened)
			{
				//UtilityHelper.CloseMaskingTable();
				maskingTablePopUPEvent();
			}
		}
		
		public void Disconnect()
		{
			peer_toMasterServer.Disconnect();
			if(peer_toGameServer != null)
			{
				peer_toGameServer.Disconnect();
			}
		}
		
		public void ChangeUserStatus(UserStatus us){			
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserStatus]=us;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.Suspend,param,true);
		}
		
		public void UpgradeUrl(DataPersist.DeviceType deviceType){			
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.DeviceType]=deviceType;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.UpgradeUrl,param,true);
		}
		
		public void SystemNotice(){			
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]="";
			peer_toMasterServer.OpCustom((byte)LilyOpCode.SystemNotice,param,true);
		}
		
		public void SystemSetting(){
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			//param[(byte)LilyOpKey.UserData]=SerializeHelper.Serialize(User.Singleton.UserData);
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			param[(byte)LilyOpKey.SystemNotification]=User.Singleton.UserData.SystemNotify;
            param[(byte)LilyOpKey.FriendNotification]=User.Singleton.UserData.FriendNotify;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.Setting,param,true);		
		}
		
		public void QueryUserById(string userId){
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]=userId;			
			peer_toMasterServer.OpCustom((byte)LilyOpCode.QueryUserById,param,true);	
		}
		
		public void Register()
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			//param[(byte)LilyOpKey.UserData]=SerializeHelper.Serialize(User.Singleton.UserData);
			param[(byte)LilyOpKey.Mail]=User.Singleton.UserData.Mail;
            param[(byte)LilyOpKey.PassWord]=User.Singleton.UserData.Password;
            param[(byte)LilyOpKey.NickName]=User.Singleton.UserData.NickName;            
            param[(byte)LilyOpKey.DeviceType]=User.Singleton.UserData.DeviceType;
            param[(byte)LilyOpKey.DeviceToken]=User.Singleton.UserData.DeviceToken;
			param[(byte)LilyOpKey.Avator] = User.Singleton.UserData.Avator;
			param[(byte)LilyOpKey.UserType] = User.Singleton.UserData.UserType;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.Register,param,true);			
		}
		
		public void GuestUpgrade(){
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			//param[(byte)LilyOpKey.UserData]=SerializeHelper.Serialize(User.Singleton.UserData);	
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			param[(byte)LilyOpKey.Mail]=User.Singleton.UserData.Mail;
            param[(byte)LilyOpKey.PassWord]=User.Singleton.UserData.Password;
            param[(byte)LilyOpKey.NickName]=User.Singleton.UserData.NickName;
			param[(byte)LilyOpKey.Avator] = User.Singleton.UserData.Avator;
			param[(byte)LilyOpKey.UserType]=User.Singleton.UserData.UserType;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.GuestUpgrade,param,true);	
		}
		
		public void Login(UserData user){
				Dictionary<byte,object> param=new Dictionary<byte, object>();
				string roomId;
				int totalRounds,stepId;
				if(Room.Singleton.PokerGame.ReadDropCache(out roomId,out totalRounds,out stepId))
				{
					param[(byte)LilyOpKey.GameId]=roomId;
					param[(byte)LilyOpKey.TableRound]=totalRounds;
					param[(byte)LilyOpKey.StepID]=stepId;
					//param[(byte)LilyOpKey.Mail]=User.Singleton.UserData.Mail;
					tempObj=roomId;
				}
				param[(byte)LilyOpKey.Mail]=user.Mail;
				param[(byte)LilyOpKey.PassWord]=user.Password;
				param[(byte)LilyOpKey.DeviceType]=user.DeviceType;
				param[(byte)LilyOpKey.UserType]=user.UserType;
				//param[(byte)LilyOpKey.Notification]=SettingManager.Singleton.SystermNotification||SettingManager.Singleton.FriendActivityNotification;
				param[(byte)LilyOpKey.DeviceToken]=user.DeviceToken;
				
				Debug.Log("user.Mail"+user.Mail.ToString()+" user.Password"+user.Password.ToString()+" user.DeviceType"+user.DeviceType.ToString()+" user.UserType"+user.UserType.ToString()+" user.DeviceToken"+user.DeviceToken.ToString());
				peer_toMasterServer.OpCustom((byte)LilyOpCode.Login,param,true);
			
			
		}
		
		public void Login()
		{
			//try{
			GlobalManager.Log("Login");
			//string dropCacheContent=FileIOHelper.ReadFile(FileType.DropCache);
//			Debug.Log (dropCacheContent);
			
			Dictionary<byte,object> param=new Dictionary<byte, object>();
//			string roomId;
//			int totalRounds,stepId;
//			if(Room.Singleton.PokerGame.ReadDropCache(out roomId,out totalRounds,out stepId))
//			{
//				param[(byte)LilyOpKey.GameId]=roomId;
//				param[(byte)LilyOpKey.TableRound]=totalRounds;
//				param[(byte)LilyOpKey.StepID]=stepId;
//				tempObj=roomId;
//			}
			param[(byte)LilyOpKey.UserData]=SerializeHelper.Serialize(User.Singleton.UserData);
			//param[(byte)LilyOpKey.Notification]=SettingManager.Singleton.SystermNotification||SettingManager.Singleton.FriendActivityNotification;
		 	peer_toMasterServer.OpCustom((byte)LilyOpCode.Login,param,true);
//			}catch(Exception ex){
//				System.Text.StringBuilder sb=new System.Text.StringBuilder();
//				sb.AppendLine (ex.Message);
//				sb.Append (ex.StackTrace);
//				sb.AppendLine ();
//				sb.Append (ex.Source);
//				FileIOHelper.WriteFile (FileType.SerializeError,sb.ToString ());
//				
//				throw ex;				
//			}
			
		}
		
		public void Logout()
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.Logout,param,true);
		}
		
		public void FindPassword(string mail){
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			Hashtable ht = new Hashtable();
            ht["mail"] = mail;
			param[(byte)LilyOpKey.Data]=ht;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.FindPassword,param,true);
		}
		
		public void FindPasswordResponse(OperationResponse operationResponse)
		{
			GlobalManager.Singleton.Notifications.Enqueue(new FindPasswordNotification(true));
//#if UNITY_IPHONE
//			EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_TITLE"), LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_DESCRIPTION"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//#endif
//			
//#if UNITY_ANDROID
//			EtceteraAndroid.showAlert(LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_TITLE"), LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_DESCRIPTION"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//#endif
		}
		
		public void SearchUser(string nickname)
		{
			GlobalManager.Log(nickname);
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.NickName]=nickname;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.SearchUser,param,true);
		}
		
		public void RequestFriend(string userId)
		{
			GlobalManager.Log("UserId:"+userId);
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]=userId;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.RequestFriend,param,true);
		}
		
		public void AddFriend(string userId)
		{
			GlobalManager.Log("enter add friend function");
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			FriendData friendData=new FriendData();
			friendData.UserA=User.Singleton.UserData.UserId;
			friendData.UserB=userId;
			param[(byte)LilyOpKey.FriendData]=SerializeHelper.Serialize(friendData);
			peer_toMasterServer.OpCustom((byte)LilyOpCode.AddFriend,param,true);
		}
		
		public void DeleteFriend(string userId)
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			FriendData friendData=new FriendData{
				UserA=User.Singleton.UserData.UserId,
				UserB=userId
			} ;
			param[(byte)LilyOpKey.FriendData]=SerializeHelper.Serialize(friendData);
			tempObj=userId;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.DeleteFriend,param,true);
		}
		
		public void AcceptFriend(string userId)
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			FriendData friendData=new FriendData{
				UserA=User.Singleton.UserData.UserId,
				UserB=userId
			} ;
			param[(byte)LilyOpKey.FriendData]=friendData;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.AcceptFriend,param,true);
		}
		
		public void GetAward()
		{
			peer_toMasterServer.OpCustom((byte)LilyOpCode.GetAward,null,true);
		}
		
		public void InviteFriend(string userId,string destination)
		{
			var gameserver = peer_toGameServer.ServerAddress
						+ ";#" + peer_toGameServer.ApplicationName
						+ ";#" + peer_toGameServer.NodeId;
			
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]=userId;
			param[(byte)LilyOpKey.Destination]=destination+"|"+Room.Singleton.RoomData.Owner.UserId;
			param[(byte)LilyOpKey.GameServerAddress]=gameserver;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.InviteFriend,param,true);
		}
		
		public void Feedback(string suggestion)
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			FeedbackData feedback=new FeedbackData{
				UserId=User.Singleton.UserData.UserId,
				Content=suggestion
			} ;
			param[(byte)LilyOpKey.Feedback]=SerializeHelper.Serialize(feedback);
			peer_toMasterServer.OpCustom((byte)LilyOpCode.Feedback,param,true);
		}
		
		public void AccessFriend(string userId)
		{
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.UserId]=userId;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.AccessFriend,param,true);
		}
		
		public void FriendList()
		{
			GlobalManager.Log ("FriendList in PhotonClient");
			Dictionary<byte,object> param=new Dictionary<byte, object>();
//			Debug.Log("FriendList userid: " + User.Singleton.UserData.UserId);
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.FriendList,param,true);
		}
		
		
		
		public void JoinRoom(string roomId)
		{
 			isFastStart=false;
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.LobbyId]=LOBBY_ID;
			param[(byte)LilyOpKey.GameId]=roomId;
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			InviterRoom=roomId;			
			
			if(LOADBALANCE)
				peer_toMasterServer.OpCustom((byte)LilyOpCode.CreateGame,param,true);
			else
				peer_toGameServer.OpCustom((byte)LilyOpCode.Join,param,true);
		}
		
		public void JoinRoom(string roomId,bool login){
			
 
			isFastStart=false;
			if(string.IsNullOrEmpty(InviterRoom))
			InviterRoom=roomId;
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.LobbyId]=LOBBY_ID;
			param[(byte)LilyOpKey.GameId]=InviterRoom;
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			
						
			if(LOADBALANCE){
				//if(peer_toGameServer==null)
				if(peer_toGameServer==null||peer_toGameServer.PeerState!=PeerStateValue.Connected)
					peer_toMasterServer.OpCustom((byte)LilyOpCode.CreateGame,param,true);			
			}
			else
				peer_toGameServer.OpCustom((byte)LilyOpCode.Join,param,true);
		}
		
		private void doJoinRoomOnGameServer(){
			Dictionary<byte,object> param=new Dictionary<byte, object>();
			param[(byte)LilyOpKey.LobbyId]=LOBBY_ID;
			param[(byte)LilyOpKey.GameId]=InviterRoom;
			param[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.Join,param,true);
			InviterRoom=string.Empty;
		}
		
		
		public void GotoFriend(string roomId){

				JoinRoom(roomId);
		}
		
		public void LeaveRoom()
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.Leave,opParams,true);
		}
		public void QuickStart()
		{			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			Debug.LogWarning("QuickStart,noSeat:"+User.Singleton.UserData.NoSeat);
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			opParams[(byte)LilyOpKey.LobbyId]=LOBBY_ID;
			if(peer_toGameServer.PeerState==PeerStateValue.Connected)
				peer_toGameServer.OpCustom((byte)LilyOpCode.QuickStart,opParams,true);
			else
				this.QuickStartAction=QuickStart;
		}
		
		
		public void JoinGame()
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.GameId]=Room.Singleton.RoomData.Owner.UserId;
			if(!string.IsNullOrEmpty(InviterId))
			{
				opParams[(byte)LilyOpKey.InviterId]=InviterId;
			}
			peer_toGameServer.OpCustom((byte)LilyOpCode.JoinTable,opParams,true);
		}
		
		public void SyncGameData(){			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.SyncGameData,opParams,true);
		}
		
		public void SyncGameDataTableInfo(){
			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.TableInfo]=null;
			peer_toGameServer.OpCustom((byte)LilyOpCode.SyncGameData,opParams,true);
			
		}
		
		public void TryJoinGame(string friendId)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.GameId]=friendId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.TryJoinTable,opParams,true);
		}
		
		public void TryJoinGame()
		{
			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.GameId]=Room.Singleton.RoomData.Owner.UserId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.TryJoinTable,opParams,true);
		}
		
		public void JoinGame(int bigBlind,int maxPlayers,int thinkingTime,bool onlyFriend)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.GameId]=Room.Singleton.RoomData.Owner.UserId;
			opParams[(byte)LilyOpKey.GameSettingBigBlind]=bigBlind;
			opParams[(byte)LilyOpKey.GameSettingMaxPlayers]=maxPlayers;
			opParams[(byte)LilyOpKey.GameSettingThinkingTime]=thinkingTime;
			opParams[(byte)LilyOpKey.GameSettingFriendsOnly]=onlyFriend;
			peer_toGameServer.OpCustom((byte)LilyOpCode.JoinTable,opParams,true);
		}
		
		public void LeaveGame()
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			peer_toGameServer.OpCustom((byte)LilyOpCode.LeaveTable,opParams,true);
		}
		
		public void Standup(){
			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			peer_toGameServer.OpCustom((byte)LilyOpCode.StandUp,opParams,true);
		}
		
		public void BroadcastMessage(string message)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.Message]=message;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.BroadcastMessage,opParams,true);
		}
		
		public void BroadcastMessageInTable(string message)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.Message]=message;
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			peer_toGameServer.OpCustom((byte)LilyOpCode.BroadcastMessageInTable,opParams,true);
		}
		
		public void CheckEmail(string email)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.Email]=email;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.CheckEmail,opParams,true);
		}

        public void AskOnlinePeopleNumber()
        {
            peer_toMasterServer.OpCustom((byte)LilyOpCode.GetOnlinePeopleNum, null, true);
        }
		
		void UpdateSignalValue(int i)
		{
			if (peer_toMasterServer == null)
			{
				signalValue = SignalValue.VeryBad;
				return;
			}
			
			if (peer_toMasterServer.RoundTripTime > 0 && peer_toMasterServer.RoundTripTime <= 300)
			{
				signalValue = SignalValue.Perfact + i;
			}
			else if (peer_toMasterServer.RoundTripTime > 300 && peer_toMasterServer.RoundTripTime <= 600)
			{
				signalValue = SignalValue.VeryGood + i;
			}
			else if (peer_toMasterServer.RoundTripTime > 600 && peer_toMasterServer.RoundTripTime <=1000)
			{
				signalValue = SignalValue.Good + i;
			}
			else if (peer_toMasterServer.RoundTripTime > 1000 && peer_toMasterServer.RoundTripTime <= 2000)
			{
				signalValue = SignalValue.Bad + i;
			}
			else if (peer_toMasterServer.RoundTripTime > 2000 && peer_toMasterServer.RoundTripTime <=3000)
			{
				signalValue = SignalValue.VeryBad + i;
			}
			else if (peer_toMasterServer.RoundTripTime > 3000)
			{
				signalValue = SignalValue.NoSignal;
			}
		}
		
		public void CheckConnection()
		{
			long iSecond = peer_toMasterServer.LocalTimeInMilliSeconds-peer_toMasterServer.TimestampOfLastSocketReceive;
			//Debug.Log("peer_toMasterServer.RoundTripTime " + peer_toMasterServer.RoundTripTime + " iSecond " + iSecond);
			UpdateSignalValue((int)(iSecond / 1000));
			lock(this)
			{
				if (iSecond > 15000 && !User.Singleton.MaskingTableOpened)
				{
					bNetworkError = true;
					PopUpMaskingTable();
				}
				else if (iSecond < 15000 && User.Singleton.MaskingTableOpened && bNetworkError)
				{
					bNetworkError = false;
					CloseMaskingTable();
				}
			}
		}
		
		public void Update()
		{
			peer_toMasterServer.Service();

			if ( peer_toGameServer != null)
			{
				peer_toGameServer.Service();
			}
			
			if(peer_toMasterServer.PeerState==PeerStateValue.Connected&&
				peer_toMasterServer.LocalTimeInMilliSeconds-peer_toMasterServer.TimestampOfLastSocketReceive>DISCONNECT_TIMEOUT)
			{
					Disconnect();
			}
			
			if(LOADBALANCE&&
				peer_toGameServer!=null&&
				peer_toGameServer.PeerState==PeerStateValue.Connected&&
				peer_toGameServer.LocalTimeInMilliSeconds-peer_toGameServer.TimestampOfLastSocketReceive>DISCONNECT_TIMEOUT)
			{
				peer_toGameServer.Disconnect();
			}
		}
		
		public void Sit(long chips)
		{
			Dictionary<byte, object> opParams = new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.LobbyId]=LOBBY_ID;
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			opParams[(byte)LilyOpKey.GameId]=User.Singleton.UserData.UserId; // 
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			opParams[(byte)LilyOpKey.TakenAmnt]=chips;
			peer_toGameServer.OpCustom((byte)LilyOpCode.Sit, opParams, true);		
		}
		
	
		public void Save()
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			//opParams[(byte)LilyOpKey.UserData]=SerializeHelper.Serialize(User.Singleton.UserData);
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
            opParams[(byte)LilyOpKey.Avator]=User.Singleton.UserData.Avator;
            opParams[(byte)LilyOpKey.PassWord]=User.Singleton.UserData.Password;
            opParams[(byte)LilyOpKey.BackGroundType]=User.Singleton.UserData.BackgroundType;
            opParams[(byte)LilyOpKey.LivingRoomType]=User.Singleton.UserData.LivingRoomType;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.SaveUserInfo,opParams,true);
		}
		
		public void Fold()
		{			
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			if(peer_toGameServer.OpCustom((byte)LilyOpCode.Fold,opParams,true))
				fakePlayerTurnEndedEvent(TypeAction.Fold,-1);
		}
        public void AutoFold()
        {
            Dictionary<byte, object> opParams = new Dictionary<byte, object>();
            opParams[(byte)LilyOpKey.NoSeat] = User.Singleton.UserData.NoSeat;
            opParams[(byte) LilyOpKey.AutoFold] = true;
            if (peer_toGameServer.OpCustom((byte)LilyOpCode.Fold, opParams, true))
                fakePlayerTurnEndedEvent(TypeAction.Fold, -1);
        }
		
		public void Call()
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			if(peer_toGameServer.OpCustom((byte)LilyOpCode.Call,opParams,true))
				fakePlayerTurnEndedEvent(TypeAction.Call,0);
		}
		
		public void Raise(long raiseMoney)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			opParams[(byte)LilyOpKey.Money]=raiseMoney;
			if(peer_toGameServer.OpCustom((byte)LilyOpCode.Raise,opParams,true))
				fakePlayerTurnEndedEvent(TypeAction.Raise,raiseMoney);
		}
		
		
		private void fakePlayerTurnEndedEvent(TypeAction typeaction,long amnt){
			int noSeat=User.Singleton.UserData.NoSeat;
			if(noSeat<0)
				return;
			if(Room.Singleton.PokerGame.TableInfo==null)
				return;			
			
			TableInfo pokertable=Room.Singleton.PokerGame.TableInfo;
			PlayerInfo player=pokertable.GetPlayer (noSeat);
			
			if(pokertable.NoSeatCurrPlayer!=noSeat)
				return;
			
			int nbplayered=pokertable.NbPlayed;
			if(typeaction==TypeAction.Call){
				amnt=pokertable.CallAmnt(player);
				nbplayered++;
			}
			
			
			if (amnt<0) {
				player.IsPlaying=false;
			}else{				
				amnt=Math.Min(amnt,player.MoneySafeAmnt);
				player.MoneySafeAmnt-=amnt;
				player.MoneyBetAmnt+=amnt;
				if(player.MoneySafeAmnt<=0||getMaxAllinValue(pokertable,player.NoSeat)<=0)
					player.IsAllIn=true;
			}
			
			
			if (typeaction==TypeAction.Raise) {
				int count = pokertable.NbAllIn;
			    if (!player.IsAllIn)
			          count++;
			    nbplayered = count;	
			}
			
		
			long totalamnt=pokertable.TotalPotAmnt;
			
			EventData eventData=new EventData();
			eventData.Code=(byte)LilyEventCode.PlayerTurnEnded;
			eventData.Parameters=new Dictionary<byte, object>();
			eventData.Parameters.Add((byte)LilyEventKey.NoSeat,noSeat);
			eventData.Parameters.Add((byte)LilyEventKey.MoneyBetAmnt,player.MoneyBetAmnt);
			eventData.Parameters.Add((byte)LilyEventKey.MoneySafeAmnt,player.MoneySafeAmnt);
			eventData.Parameters.Add((byte)LilyEventKey.IsPlaying,player.IsPlaying);
			eventData.Parameters.Add((byte)LilyEventKey.TotalPortAmnt,totalamnt);
			eventData.Parameters.Add((byte)LilyEventKey.Action,typeaction);
			ProcessPlayerTurnEndedEvent(eventData,true);
			
			
			if (pokertable.NbPlayingAndAllIn() == 1 || nbplayered >= pokertable.NbPlayingAndAllIn())
			{
				pokertable.NoSeatCurrPlayer=-1;
				
			}else{
				EventData eventDataBegin=new EventData();
				eventDataBegin.Code=(byte)LilyEventCode.PlayerTurnBegan;
				eventDataBegin.Parameters=new Dictionary<byte, object>();
				eventDataBegin.Parameters.Add ((byte)LilyEventKey.NoSeat,pokertable.GetPlayingPlayerNextTo(noSeat).NoSeat);
				eventDataBegin.Parameters.Add((byte)LilyEventKey.LastNoSeat,noSeat);
				long hightbet=pokertable.HigherBet;
				hightbet=Math.Max(hightbet,player.MoneyBetAmnt);
				eventDataBegin.Parameters.Add((byte)LilyEventKey.HigherBet,hightbet);
				
				ProcessPlayerTurnBeganEvent(eventDataBegin,true);
			}
		}
		
		private long getMaxAllinValue(TableInfo info,int noSeat)
		{
		    //TableInfo info=Room.Singleton.PokerGame.TableInfo;
			if(info!=null)
			{
			 	PlayerInfo playerinforitem= info.GetPlayer(noSeat);
				if(playerinforitem==null)
					return 0;
				long max=0;
				
				foreach(PlayerInfo item in info.Players)
				{	
				   	if(max<(item.MoneySafeAmnt+item.MoneyBetAmnt) && item.NoSeat!=User.Singleton.UserData.NoSeat)
					{
						if(item.IsAllIn==true || item.IsPlaying==true)
							max=(item.MoneySafeAmnt+item.MoneyBetAmnt);
					}
				}
				
				if(max>=(playerinforitem.MoneySafeAmnt+playerinforitem.MoneyBetAmnt))
					max=playerinforitem.MoneySafeAmnt;
				else
					max=max-playerinforitem.MoneyBetAmnt;
				
				return max;
			}
			return 0;
		}
		
		
		public void Check(){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=User.Singleton.UserData.NoSeat;
			peer_toGameServer.OpCustom((byte)LilyOpCode.Check,opParams,true);			
		}
		
		public void RequestRobot()
		{
//			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
//			opParams[(byte)LilyOpKey.GameId]=Room.Singleton.RoomData.Owner.UserId;
//			peer_toGameServer.OpCustom((byte)LilyOpCode.RequestRobot,opParams,true);
		}
		
		public void BuyItem(ItemType itemType,int itemId , int money, string result, PayWay payWay)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.ItemType]=itemType;
			opParams[(byte)LilyOpKey.ItemId]=itemId;
			opParams[(byte)LilyOpKey.IAPMoney]=money;
			opParams[(byte)LilyOpKey.IAPString]=result;
			opParams[(byte)LilyOpKey.PayWay]= payWay;
			if (payWay == PayWay.IAP)
			{
				Debug.Log("itemId.ToString() is " + itemId.ToString());
				StringBuilder builder = new StringBuilder();
				int id = itemId;
				switch(itemType)
				{
				case ItemType.Jade:
				case ItemType.Lineage:
					id = (int)itemType * 10;
					id += itemId;
					break;
				case ItemType.Avator:
					id = (int)itemType * 100;
					id += itemId;
					break;
				}
				builder.Append(id.ToString());
				builder.Append("|");
				builder.Append(money.ToString());
				builder.Append("|");
				builder.Append(result);
				Shop.Singleton.CurrentIAP = builder.ToString();
			}
			else
				Shop.Singleton.CurrentIAP = string.Empty;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.BuyItem,opParams,true);
		}
		public void BuyItemByChips(ItemType itemtype,int itemId,long chips){
			Debug.Log("BuyItemByChips: " + itemtype + " " + itemId);
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.ItemType]=itemtype;
			opParams[(byte)LilyOpKey.ItemId]=itemId;
            opParams[(byte)LilyOpKey.Chip] = chips;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.BuyitemByChips,opParams,true);		
		}
		
		public void SendChip(string userId,long chip)
		{
			bSendUserID = userId;
			lSendchip = chip;
			this.tempObj=chip;
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=userId;
			opParams[(byte)LilyOpKey.Chip]=chip;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.SendChip,opParams,true);
			//peer_toGameServer.OpCustom((byte)LilyOpCode.SendChip,opParams,true);
		}
		
		public void SendGift(int giftId)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.GiftId]=giftId;
			peer_toGameServer.OpCustom ((byte)LilyOpCode.SendGift,opParams,true);
		}
		
		public void SendGift(int noseat,int giftId)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=noseat;
			opParams[(byte)LilyOpKey.GiftId]=giftId;
			peer_toGameServer.OpCustom ((byte)LilyOpCode.SendGift,opParams,true);
		}
		
		public void GetClientVersion()
		{
			peer_toMasterServer.OpCustom((byte)LilyOpCode.GetClientVersion,new Dictionary<byte,object>(),true);
		}
		
		public void Achieve(byte number)
		{
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.AchievementNumber]=number;
			peer_toGameServer.OpCustom((byte)LilyOpCode.Achieve,opParams,true);
		}
		
		public void GetOnlineAwards(){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.UserId]=User.Singleton.UserData.UserId;
			peer_toGameServer.OpCustom((byte)LilyOpCode.GetOnlineAwards,opParams,true);
		}
		
		public void KickPlayer(int noSeat){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.NoSeat]=noSeat;
			peer_toGameServer.OpCustom((byte)LilyOpCode.KickPlayer,opParams,true);
		}
		
		public void DeviceInfoReady(DeviceInfo deviceInfo){
			Debug.Log("DeviceInfoReady");
			if(deviceInfo == null) return;
			GlobalManager.Singleton.deviceInfo = deviceInfo;
			GlobalManager.Singleton.bIsSubmitDeviceInfo = true;
			if(this.isConnected)
			{
				 DeviceInfo();
			}
			else
				CurrentActionAfterConnect += DeviceInfo;
		}
		public void DeviceInfo(){
			if(GlobalManager.Singleton.deviceInfo == null || !GlobalManager.Singleton.bIsSubmitDeviceInfo)
				return;
			DeviceInfo deviceInfo = GlobalManager.Singleton.deviceInfo;
			GlobalManager.Singleton.deviceInfo = null;
			GlobalManager.Singleton.bIsSubmitDeviceInfo = false;
			Debug.Log("ChannelId:"+deviceInfo.strChannelId+" DeviceType:"+deviceInfo.strDeviceType+" DeviceToken:"+deviceInfo.strDeviceToken+" ClientVersion:"+deviceInfo.strClientVersion+" osVersion:"+deviceInfo.strOSVersion);
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.ChannelId]=deviceInfo.strChannelId;
			opParams[(byte)LilyOpKey.DeviceType]=deviceInfo.strDeviceType;
			opParams[(byte)LilyOpKey.DeviceToken]=deviceInfo.strDeviceToken;
			opParams[(byte)LilyOpKey.ClientVersion]=deviceInfo.strClientVersion;
			opParams[(byte)LilyOpKey.osVersion]=deviceInfo.strOSVersion;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.DeviceInfo,opParams,true);
		}
		
		public void AddTakenMoney(long addAmnt){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.TakenAmnt]=addAmnt;
			peer_toGameServer.OpCustom((byte)LilyOpCode.AddTakenMoney,opParams,true);
		}
		
		public void BindChannelId(string channelid,string userid){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams[(byte)LilyOpKey.ChannelId]=channelid;
			opParams[(byte)LilyOpKey.UserId]=userid;
			peer_toMasterServer.OpCustom((byte)LilyOpCode.BindChannelId,opParams,true);			
		}
		
		public void GetGameGrades(){						
			peer_toMasterServer.OpCustom((byte)LilyOpCode.GameGrades,null,true);	
		}
		
		public void JoinCareerGame(int gradeId){
			Dictionary<byte,object> opParams=new Dictionary<byte, object>();
			opParams.Add((byte)LilyOpKey.GameGradeId,gradeId);
			if(peer_toGameServer.OpCustom((byte)LilyOpCode.JoinCareerGame,opParams,true))
			{
			    isFastStart = true;
			}
		}
		
		#endregion
		
		#region IPhotonPeerListener Method
		public void DebugReturn(DebugLevel level,string messages)
		{
			Debug.Log ("DebugReturn,"+level.ToString()+":"+messages);
		}
		
		public void OnEvent(EventData eventData)
		{	
			//UtilityHelper.ResetTimer();
			//UtilityHelper.CloseMaskingTable();
			GlobalManager.Log("OnEvent in PhotonClient:"+(LilyEventCode)eventData.Code);
			switch((LilyEventCode)eventData.Code)
			{
			case LilyEventCode.Join:
				ProcessJoinEvent(eventData);
				break;
			case LilyEventCode.Leave:
				ProcessLeaveEvent(eventData);
				break;
			case LilyEventCode.RequestFriend:
				ProcessRequestFriendEvent(eventData);
				break;
			case LilyEventCode.AddFriend:
				ProcessAddFriendEvent(eventData);
				break;
			case LilyEventCode.DeleteFriend:
				ProcessDeleteFriendEvent(eventData);
				break;
			case LilyEventCode.InviteFriend:
				ProcessInviteFriendEvent(eventData);
				break;
			case LilyEventCode.BroadcastMessage:
				GlobalManager.Log (eventData.Parameters[(byte)LilyEventKey.Message] as string);
				string message = eventData.Parameters[(byte)LilyEventKey.Message] as string;
				string nickName = eventData.Parameters[(byte)LilyEventKey.NickName] as string;
				GlobalManager.Singleton.Notifications.Enqueue( new SystemNoticeNotification(message,nickName,StatusTipsType.BeVipPlayer,null,WorldMessageType.WorldSpeak));
				break;
			case LilyEventCode.BroadcastMessageInTable:
				ProcessBroadcastMessageInTable(eventData);
				break;
			case LilyEventCode.GameStarted:
				ProcessGameStartedEvent(eventData);
				break;
			case LilyEventCode.PlayerMoneyChanged:
				ProcessPlayerMoneyChangedEvent(eventData);
				break;
			case LilyEventCode.PlayerTurnEnded:
				ProcessPlayerTurnEndedEvent(eventData,false);
				break;
			case LilyEventCode.PlayerHoleCardsChanged:
				PlayerHoleCardsChangedEvent(eventData);
				break;
			case LilyEventCode.PlayersShowCards:
				PlayersShowCardsEvent(eventData);
				break;
			case LilyEventCode.BetTurnStarted:
				ProcessBetTurnStartedEvent(eventData);
				break;
			case LilyEventCode.BetTurnEnded:
				ProcessBetTurnEndedEvent(eventData);
				break;
			case LilyEventCode.GameEnded:
				ProcessGameEndedEvent(eventData);
				break;
			case LilyEventCode.PlayerWonPot:
				ProcessPlayerWonPotEvent(eventData);
				break;
			case LilyEventCode.PlayerWonPotImprove:
				ProcessPlayerWonPotImproveEvent(eventData);
				break;
			case LilyEventCode.TableClosed:
				ProcessTableClosedEvent(eventData);
				break;
			case LilyEventCode.PlayerTurnBegan:
				ProcessPlayerTurnBeganEvent(eventData,false);
				break;
			case LilyEventCode.PlayerJoined:
				ProcessPlayerJoinedEvent(eventData);
				break;
			case LilyEventCode.PlayerLeaved:
				ProcessPlayerLeavedEvent(eventData);
				break;
			case LilyEventCode.SendChip:
				ProcessSendChipEvent(eventData);
				break;
			case LilyEventCode.ExperienceAdded:
				
				int level=(int)eventData.Parameters[(byte)LilyEventKey.Level];
				long levelexp=(long)eventData.Parameters[(byte)LilyEventKey.LevelExp];				
//				Debug.Log(string.Format("level: {0} , exp : {1} ",level,levelexp));
				//remove UserType.Guest level limit
//				if (level == 7 && User.Singleton.UserData.UserType == UserType.Guest)
//				{
//					if (GuestLevelLimitedEvent != null)
//						GuestLevelLimitedEvent();
//				}
//				else 
				if (level != User.Singleton.UserData.Level)
				{
//					Debug.Log("level: "+level+"User.Singleton.UserData.Level:"+User.Singleton.UserData.Level);
					if (levelUpEvent != null)
						levelUpEvent(level);
					
					User.Singleton.UserData.Level=level;
					PlayerInfo player= Room.Singleton.PokerGame.TableInfo.Players.Find(r=>(r as UserData).UserId==User.Singleton.UserData.UserId);
					if(player!=null){
						player.Level=level;						
					}
					
				}
				
				break;
			case LilyEventCode.SendGift:
				ProcessSendGiftEvent(eventData);
				break;
			case LilyEventCode.Achievement:
				ProcessAchievementEvent(eventData);
				break;
			case LilyEventCode.RoomTypeChanged:
				ProcessRoomTypeChanged(eventData);
				break;
			case LilyEventCode.SameAccountLogin:
				if (sameAccountLoginEvent != null)
					sameAccountLoginEvent();
				break;
            case LilyEventCode.BroadcastStatusTipsMsg:
				StatusTipsType mytype = (StatusTipsType)(eventData.Parameters[(byte)LilyEventKey.StatusTipsMsgType]);
				var myparams = (eventData.Parameters[(byte)LilyEventKey.StatusTipsMsgParams] as object[]);
				GlobalManager.Singleton.Notifications.Enqueue( new SystemNoticeNotification(string.Empty,string.Empty,mytype,myparams,WorldMessageType.StatusTip));
				break;
			case LilyEventCode.PlayRankChanegd:		
				byte[] myRankList = (byte[])eventData.Parameters[(byte)LilyEventKey.PlayRankList];	
				Debug.Log ("lee test rank");
				GlobalManager.Singleton.Notifications.Enqueue(new MatchRankListChangedNotification(myRankList));
				break;
			}
		}
		
		public void OnOperationResponse(OperationResponse operationResponse)
		{
			//UtilityHelper.ResetTimer();
			GlobalManager.Log("In PhotonClient "+(LilyOpCode)operationResponse.OperationCode);
			
			if(IsOperationSuccess(operationResponse))
			{
				GlobalManager.Log ("Suceess");
				CloseMaskingTable();
				switch((LilyOpCode)operationResponse.OperationCode)
				{
				case LilyOpCode.CreateGame:
					CreateGameResponse(operationResponse);
					break;
				
				case LilyOpCode.GuestUpgrade:
				case LilyOpCode.Register:
					RegisterResponse(operationResponse);
					break;					
				case LilyOpCode.Login:
					LoginReponse(operationResponse);
					break;
				case LilyOpCode.Logout:
					LogoutReponse(operationResponse);
					break;
				case LilyOpCode.Join:
					JoinResponse(operationResponse);
					break;
				case LilyOpCode.FindPassword:
					FindPasswordResponse(operationResponse);
					break;
//				case LilyOpCode.Leave:
//					LeaveResponse(operationResponse);
//					break;
				case LilyOpCode.SearchUser:
					SearchUserResponse(operationResponse);
					break;
				case LilyOpCode.RequestFriend:
					RequestFriendResponse(operationResponse);
					break;
				case LilyOpCode.AddFriend:
					AddFriendReponse(operationResponse);
					break;
				case LilyOpCode.DeleteFriend:
					DeleteFriendResponse(operationResponse);
					break;
				case LilyOpCode.AcceptFriend:
					AcceptFriendResponse(operationResponse);
					break;
				case LilyOpCode.InviteFriend:
					InviteFriendResponse(operationResponse);
					break;
				case LilyOpCode.AccessFriend:
					AccessFriendResponse(operationResponse);
					break;
				case LilyOpCode.FriendList:
					FriendListResponse(operationResponse);
					break;
				case LilyOpCode.QuickStart:
					//this.QuickStartResponse(operationResponse);
					JoinGameResponse(operationResponse);
					break;
				case LilyOpCode.JoinTable:
					JoinGameResponse(operationResponse);
					break;
				case LilyOpCode.LeaveTable:
					LeaveGameResponse(operationResponse);
					break;
				case LilyOpCode.Sit:
					SitResponse(operationResponse);
					break;
				case LilyOpCode.StandUp:
					StandupResponse(operationResponse);
					break;
				case LilyOpCode.BuyItem:
					BuyItemResponse(operationResponse);
					break;		
				case LilyOpCode.SendChip:
					SendChipResponse(operationResponse);
					break;
				case LilyOpCode.GetClientVersion:
					GetClientVersionResponse(operationResponse);
					break;
				
				case LilyOpCode.QueryUserById:
				{
					UserData userData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
					Debug.Log("UserId in QueryUserById:"+userData.UserId);
					if(userData.UserId==Room.Singleton.RoomData.Owner.UserId)
					{
						Room.Singleton.RoomData.Owner=userData;
					}
					
					if(Room.Singleton.RoomData.Users.Exists(rs=>rs.UserId==userData.UserId))
					{
						int index=Room.Singleton.RoomData.Users.FindIndex(rs=>rs.UserId==userData.UserId);
						Room.Singleton.RoomData.Users[index] = userData;
						RoomDataChanged();
					}
					if(User.Singleton.UserData.UserId != userData.UserId&&Room.Singleton.RoomData.Owner.UserId == userData.UserId)
					{
						DidClickFriendInfoEvent();
					}
					if (userData.UserId == User.Singleton.UserData.UserId)
					{
						User.Singleton.UserData = userData;
					}
					DidQueryUserEvent(userData);					
					//todo
				}
					break;
					
				case LilyOpCode.Setting:
					//todo 
					
//					Debug.Log ("saved");
					
					break;
				case LilyOpCode.SystemNotice:
					string notice=operationResponse.Parameters[(byte)LilyOpKey.SystemNotice] as string;
					GlobalManager.Singleton.Notifications.Enqueue( new SystemNoticeNotification(notice,string.Empty,StatusTipsType.BeVipPlayer,null,WorldMessageType.SystemNotice));
					break;
				case LilyOpCode.TryJoinTable:
					PhotonClient.Singleton.ErrorMessage = ((ErrorCode)operationResponse.Parameters[(byte)LilyOpKey.ErrorCode]).ToString();
					
					JoinGameDidFinished(true,TypeState.Init);
					 
					//TryJoinGameResponse(operationResponse);
					DidTryJointGameEvent(true);
					break;
				case LilyOpCode.Suspend:
					//to do
					break;
				case LilyOpCode.UpgradeUrl:
					string url=operationResponse.Parameters[(byte)LilyOpKey.UpgradeUrl] as string;
//					Debug.Log (url);
					
					if (upgradUrlEvent != null)
						upgradUrlEvent(url);
					break;
				case LilyOpCode.SyncGameData:
					SyncGameDataResponse(operationResponse);
					break;
				case LilyOpCode.HistoryAction:
					HistoryActionResponse(operationResponse);
					break;
				case LilyOpCode.GetAward:
					GetAwardResponse(operationResponse);
					User.Singleton.UserInfoChanged();
					break;
				case LilyOpCode.GetUserProps:
					GetUserPropsResponse(operationResponse);
					break;
				case LilyOpCode.GetOnlineAwards:
					GetOnlineAwardsResponse(operationResponse);
					break;
				case LilyOpCode.KickPlayer:
					KickPlayerResponse(operationResponse);
					break;
				case LilyOpCode.CheckEmail:
					CheckEmailResponse(operationResponse);
					break;
				case LilyOpCode.SaveUserInfo:
					if (saveInfoSuccessEvent != null)
						saveInfoSuccessEvent();
					break;
                case LilyOpCode.GetOnlinePeopleNum:
                    SetOnlinePeopleNumber(operationResponse);
                    break;
				case LilyOpCode.GameGrades:
					SetGameGrades(operationResponse);
					break;
				case LilyOpCode.JoinCareerGame:
					JoinGameResponse(operationResponse);
					break;
				}
			}
			else
			{
				//GlobalManager.Log ("Error");
				User.Singleton.GameStatus=GameStatus.Error;
				ProcessError(operationResponse);
			}
		}
		private void SetGameGrades(OperationResponse operationResponse){				
			GameGrade[] allgrades=SerializeHelper.Deserialize((Hashtable)operationResponse.Parameters[(byte)LilyOpKey.GameGrades]) as GameGrade[];
			if (allgrades!=null) {
				// some code
			    GlobalManager.Singleton.GameGrades = allgrades;
                GlobalManager.Singleton.Notifications.Enqueue(new GetGameGradesNotification());
			}
		}
		
        private void SetOnlinePeopleNumber(OperationResponse operationResponse)
        {            
            int result = (int)operationResponse[(byte)LilyOpKey.OnlinePeopleNum];
            GlobalManager.Log("lee test result:" + result);

            if(SetOnlinePeopleNumberAction != null){
                SetOnlinePeopleNumberAction(result);
            }
        }
		
		private void CheckEmailResponse(OperationResponse operationResponse)
		{
			GlobalManager.Singleton.Notifications.Enqueue(new CheckEmailNotification(true));
		}
		
		private void KickPlayerResponse(OperationResponse operationResponse){
			
			
		}
		
		private void GetUserPropsResponse(OperationResponse operationResponse){
			
			
			List<Props> pp=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.UserProps] as Hashtable) as List<Props>;
			
			if (User.Singleton.Avators != null)
				User.Singleton.Avators.Clear();
			if (User.Singleton.VipProps != null)
				User.Singleton.VipProps.Clear();
			foreach (Props item in pp) {
				switch (item.ItemType) {
				case ItemType.Avator:
					if(User.Singleton.Avators==null)
						User.Singleton.Avators=new List<Props>();
					User.Singleton.Avators.Add (item);
					break;
				case ItemType.Jade:
				case ItemType.Lineage:
					if(User.Singleton.VipProps==null)
						User.Singleton.VipProps=new List<Props>();
					User.Singleton.VipProps.Add (item);
					break;
				default:
				break;
				}
			}			
		}
		
		private void GetAwardResponse(OperationResponse operationResponse)
		{
			Debug.Log("Award:"+((long)operationResponse.Parameters[(byte)LilyOpKey.Chip]-User.Singleton.UserData.Chips));
			User.Singleton.UserData.Chips=(long)operationResponse.Parameters[(byte)LilyOpKey.Chip];
			User.Singleton.UserData.Awards=Regex.Replace(User.Singleton.UserData.Awards,@"(?<num>(?=\d\,)[^0])",@"${num}0");
			Debug.Log("Awards:"+User.Singleton.UserData.Awards);
			
			GlobalManager.Singleton.Notifications.Enqueue(new GetAwardNotification());
		}
		private void GetOnlineAwardsResponse(OperationResponse operationResponse){
			long amnt=(long)operationResponse.Parameters[(byte)LilyOpKey.TakenAmnt];
			int times=(int)operationResponse.Parameters[(byte)LilyOpKey.OnlineAwards];
			if (amnt>0) {
				User.Singleton.UserData.Chips+=amnt;
				User.Singleton.canGetOnlineAwards=false;
				User.Singleton.UserData.OnlineAwardTimes=times;
				int noSeat=User.Singleton.UserData.NoSeat;
				if(noSeat>-1){
					PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayer(noSeat);
					if(player!=null){
						player.Chips+=amnt;
						player.OnlineAwardTimes=times;
					}
				}
				
				Debug.Log("amnt:"+amnt);
				if(onlineAwardsResponseEvent!=null)
				{
					onlineAwardsResponseEvent(amnt);
				}
			}
		}
		
		
		private void HistoryActionResponse(OperationResponse operationResponse){
			if(operationResponse.Parameters.ContainsKey((byte)LilyOpKey.HistoryAction))
			{
				
				List<PlayerAction> playerActions=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.HistoryAction] as Hashtable) as List<PlayerAction>;
//				Debug.Log (playerActions.Count);
				Room.Singleton.PokerGame.SetPlayerActions(playerActions);
				//JoinGame();
//				if(User.Singleton.GameStatus==GameStatus.InRoom)
//					JoinGame();
//				else
				string roomId=(string)operationResponse.Parameters[(byte)LilyOpKey.GameId];
				InviterRoom=roomId;
				Debug.LogWarning("HistoryActionResponse,roomId={0}"+roomId);
				this.CurrentAction=JoinGame; // todo: ASK TO lee
//				if(!LOADBALANCE)
//				this.JoinRoom(tempObj as string);
				return;
			}
		}
		

//		void AutoLogin()
//		{
//			if(PlatformHelper.CanAutoLogin())
//			{
//				PlatformHelper.AutoLogin();
//			}
//		}
		
		public void OnStatusChanged(StatusCode statusCode)
		{
			Debug.Log(statusCode);
			switch(statusCode)
			{
			case StatusCode.Connect:
				if(peer_toMasterServer.PeerState==PeerStateValue.Connected&&!this.isConnected){	
					
					this.isConnected=true;	
					//GlobalManager.Singleton.GetClientVersion();
					User.bShowGameSetting_Flag = false;
					
					if (CurrentActionAfterConnect != null)
					{
						CurrentActionAfterConnect();
						this.CurrentActionAfterConnect = null;
					}
					
					if(CurrentAction!=null)
					{
						CurrentAction();
						CurrentAction=null;
					}
				}
				if(peer_toGameServer!=null&&LOADBALANCE){
					if(peer_toGameServer.PeerState==PeerStateValue.Connected){
						//AskForTheGameServer
						if(gameSeverStatus==GameServerStatus.AskForTheGameServer)
						{
							User.Singleton.GameStatus = GameStatus.Connected;
							gameSeverStatus=GameServerStatus.Connet;
							this.doJoinRoomOnGameServer();
						}
						gameSeverStatus=GameServerStatus.Connet;
					}
				}
				break;
			case StatusCode.DisconnectByServer:
				peer_toMasterServer=new SelfPeer(this);
				ReconnectOperation();
				break;
			case StatusCode.ExceptionOnConnect:
				GlobalManager.Singleton.StartReconnect();
				break;
			case StatusCode.Disconnect:
				if(LOADBALANCE && peer_toGameServer!=null && peer_toGameServer.PeerState==PeerStateValue.Disconnected
				&&(gameSeverStatus==GameServerStatus.AskForTheGameServer))
				{
					ConnectToGameServer();
				}else
				{
					if(peer_toMasterServer.PeerState==PeerStateValue.Connected){
                        //if(User.Singleton.GameStatus==GameStatus.Logout)
                        //    break;
						peer_toMasterServer.Disconnect ();
						break;
					}					
					this.isConnected=false;
					ReconnectOperation();
				}
				break;
			}
		}
		
		public void ReConnectToServer()
		{
			if(peer_toMasterServer != null)
			{
				if(peer_toMasterServer.PeerState == PeerStateValue.Disconnected)
				{
					this.Connect();
				}
			}
			
			if(peer_toGameServer != null)
			{
				if(peer_toGameServer.PeerState == PeerStateValue.Disconnected)
				{					
					string t_address = peer_toGameServer.ServerName;
					string t_appid = peer_toGameServer.ApplicationName;
					byte t_nodeid = peer_toGameServer.NodeId;
					//peer_toGameServer.Connect(t_address, t_appid, t_nodeid);
				}
			}
		}
		
		#endregion
		
		#region Process Event Method
		private void ProcessRoomTypeChanged(EventData eventData)
		{
			Room.Singleton.RoomData.Owner.LivingRoomType=(RoomType)eventData[(byte)LilyEventKey.RoomType];
			if(roomTypeChanged!=null)
				roomTypeChanged();
		}
		
		private void ProcessJoinEvent(EventData eventData)
		{
			UserData userData=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
			GlobalManager.Log("UserData:"+userData.UserId);
			
					
			if(userData.UserId!=User.Singleton.UserData.UserId&&
				Room.Singleton.RoomData!=null&&
				Room.Singleton.RoomData.Users!=null&&
				!Room.Singleton.RoomData.Users.Exists(rs=>rs.UserId==userData.UserId))
			{
				Room.Singleton.RoomData.Users.Add(userData);
				
				DidJoinRoomEventFinished(userData);
			}
			else if (userData.UserId!=User.Singleton.UserData.UserId&&
				Room.Singleton.RoomData!=null&&
				Room.Singleton.RoomData.Users!=null&&
				Room.Singleton.RoomData.Users.Exists(rs=>rs.UserId==userData.UserId))//bug 1076, on table in this room before, and leave table now join this room now
			{
				DidJoinRoomEventFinished(userData);
			}
		}
		
		private void ProcessLeaveEvent(EventData eventData)
		{
			string userId=eventData.Parameters[(byte)LilyEventKey.UserId] as string;
//			Debug.Log("ProcessLeaveEvent " + userId);
			if(Room.Singleton.RoomData.Users.Exists(rs=>rs.UserId==userId))//&&Room.Singleton.RoomData.RoomId!=userId
			{
//				Debug.LogWarning("ProcessLeaveEvent " + userId);
				UserData userData=Room.Singleton.RoomData.Users.Find(rs=>rs.UserId==userId);
				Room.Singleton.RoomData.Users.Remove(userData);
				
				DidLeaveRoomEventFinished(userData);
			}
		}
		
		private void ProcessRequestFriendEvent(EventData eventData)
		{
			UserData userData=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
			Debug.Log ("super fuck");
			RequestFriendMessage requestFriendMessage=new RequestFriendMessage(userData);
			if(!GlobalManager.Singleton.Messages.Contains(requestFriendMessage))
			{
				Debug.Log ("first fuck");
				if(User.Singleton.CurrentMessage!=null)
				{
					Debug.Log("double fuck");
					if(!(User.Singleton.CurrentMessage.Equals(requestFriendMessage)&&User.Singleton.MessageOperating))
					{
						Debug.Log ("triple fuck");
						GlobalManager.Singleton.Messages.Enqueue(requestFriendMessage);
					}
				}
				else
				{
					GlobalManager.Singleton.Messages.Enqueue(requestFriendMessage);
				}
			}
		}
		
		private void ProcessAddFriendEvent(EventData eventData)
		{
			UserData userData=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
			if(User.Singleton.Friends==null)
			{
				User.Singleton.Friends=new List<UserData>();
			}
			if(!User.Singleton.Friends.Exists(rs=>rs.UserId==userData.UserId))
			{
				User.Singleton.Friends.Add(userData);
			}
			GlobalManager.Singleton.Messages.Enqueue(new AddFriendMessage(userData));
		}
		
		private void ProcessDeleteFriendEvent(EventData eventData)
		{
			string userId=eventData.Parameters[(byte)LilyEventKey.UserId] as string;
			if(User.Singleton.Friends.Exists(rs=>rs.UserId==userId))
			{
				UserData userData=User.Singleton.Friends.Find(rs=>rs.UserId==userId);
				User.Singleton.Friends.Remove(userData);
			}
		}
		
		private void ProcessInviteFriendEvent(EventData eventData)
		{						
			UserData userData=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
			string content=eventData.Parameters[(byte)LilyEventKey.MessageContent] as string;
			
			if ((userData.UserId == Room.Singleton.RoomData.Owner.UserId 
				&& content.Split('|')[0] == "Game") || userData.UserId != Room.Singleton.RoomData.Owner.UserId) //
			{
				InviteFriendMessage inviteFriendMessage=new InviteFriendMessage(userData,content);
				PhotonClient.Singleton.PriRoomUser = Room.Singleton.RoomData.Owner;
				Debug.Log("Room.Singleton.RoomData.Owner"+Room.Singleton.RoomData.Owner.UserName);
				if(!GlobalManager.Singleton.Messages.Contains(inviteFriendMessage))
				{
					Debug.Log ("first fuck");
					if(User.Singleton.CurrentMessage!=null)
					{
						Debug.Log("double fuck");
						if(!(User.Singleton.CurrentMessage.Equals(inviteFriendMessage)&&User.Singleton.MessageOperating))
						{
							Debug.Log ("triple fuck");
							GlobalManager.Singleton.Messages.Enqueue(inviteFriendMessage);
						}
					}
					else
					{
						GlobalManager.Singleton.Messages.Enqueue(inviteFriendMessage);
					}
				}
			}
		}
		private void ProcessGameStartedEvent(EventData eventData)
		{
			TableInfo tableInfo=Room.Singleton.PokerGame.TableInfo;
			if(tableInfo!=null)
			{
								
				tableInfo.NoSeatDealer=(int)eventData.Parameters[(byte)LilyEventKey.NoSeatDealer];
				tableInfo.NoSeatSmallBlind=(int)eventData.Parameters[(byte)LilyEventKey.NoSeatSmallBlind];
				tableInfo.NoSeatBigBlind=(int)eventData.Parameters[(byte)LilyEventKey.NoSeatBigBlind];
				int[] playingNoSeats=eventData.Parameters[(byte)LilyEventKey.PlayingNoSeats] as int[];
				
				tableInfo.NbAllIn=0;
				tableInfo.NbPlayed=0;
				
				int bigBlindAmnt=(int)eventData.Parameters[(byte)LilyEventKey.BigBlind];
				if(tableInfo.BigBlindAmnt!=bigBlindAmnt){
					tableInfo.BigBlindAmnt=bigBlindAmnt;
					tableInfo.SmallBlindAmnt=bigBlindAmnt/2;
					//bigBlindAmnt change event
					GlobalManager.Singleton.Notifications.Enqueue(new MatchBigBlindChangedNotification(bigBlindAmnt));
				}
//				Debug.LogWarning(string.Format ("################FUCK  NoSeatDealer:{0},NoSeatSmallBlind{1},NoSeatBigBlind{2}, tableinfbb,{3}",tableInfo.NoSeatDealer,tableInfo.NoSeatSmallBlind,tableInfo.NoSeatBigBlind,tableInfo.BigBlindAmnt));
				
				foreach(int noseat in playingNoSeats)
				{
					PlayerInfo player=tableInfo.GetPlayer(noseat);
					if(player==null)
					{
//						Debug.Log("Null player's noSeat:"+noseat);
					}
					else{
 						player.IsPlaying=true;
						player.PlayedInTable++;
					}
				}
				
				Room.Singleton.PokerGame.TotalRounds=(int)eventData.Parameters[(byte)LilyEventKey.TotalRounds];
				Room.Singleton.PokerGame.IsWin=false;
				GameCard[] gameCardsnull={new GameCard(GameCardSpecial.Null),new GameCard(GameCardSpecial.Null)};	
				foreach(PlayerInfo player in tableInfo.Players)
				{
//					Debug.Log ("Nickname:"+player.Name+",Noseat:"+player.NoSeat);
					Room.Singleton.PokerGame.PlayerActionNames[player.NoSeat]="";
					player.Cards=gameCardsnull;	
				}					
				 
			//	GlobalManager.Singleton.Notifications.Enqueue(new GameStartedNotification());
				
				DidStartGameEvent();
			}
		}
		
		private void ProcessGameEndedEvent(EventData eventData)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				Room.Singleton.PokerGame.TableInfo.NoSeatCurrPlayer = -1;
				
				long taxAmnt=(long)eventData.Parameters[(byte)LilyEventKey.TaxAmnt];
				
				DidEndGameEvent(taxAmnt);
				
				Room.Singleton.PokerGame.TableInfo.TotalPotAmnt=0;
				TableInfo tableInfo=Room.Singleton.PokerGame.TableInfo;
				foreach (PlayerInfo p in tableInfo.Players)
	            {
	                p.MoneyBetAmnt = 0;					
	            }
			}
		}
		
		private void ProcessPlayerWonPotImproveEvent(EventData eventData)
		{
			if (Room.Singleton.PokerGame.TableInfo!=null) {
				
				int potId=(int)eventData.Parameters[(byte)LilyEventKey.MoneyPotId];
				int[] winner=eventData.Parameters[(byte)LilyEventKey.WinnerSeats] as int[];
				long[] winamnt=eventData.Parameters[(byte)LilyEventKey.Amounts] as long[];
				int[] attachedplayer=eventData.Parameters[(byte)LilyEventKey.AttachedPlayerSeats] as int[];
				for (int i = 0; i < winner.Length; i++) {
					if(potId==0)
						Room.Singleton.PokerGame.PlayerActionNames[winner[i]]=ACTION_WIN;						
				}
				if (potId==0){
					foreach (int seat in winner) {
						PlayerInfo p=Room.Singleton.PokerGame.TableInfo.GetPlayer(seat);
						if(p!=null)
							p.WinsInTable++;
					}
				}
				
				DidPlayerWonPotImproveEvent(potId,winner,winamnt,attachedplayer);
				Room.Singleton.PokerGame.IsWin=true;	
				Room.Singleton.PokerGame.TableInfo.Round=TypeRound.Preflop;
			}
		}
		
		private void ProcessPlayerWonPotEvent(EventData eventData)
		{
			int playerPos=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
			if((Room.Singleton.PokerGame.TableInfo!=null)&&Room.Singleton.PokerGame.TableInfo.Players.Exists(rs=>rs.NoSeat==playerPos))
			{
				PlayerInfo playerInfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(playerPos);
//				Debug.Log("MoneySafeAmnt:"+(int)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt]);
//				playerInfo.MoneySafeAmnt=(int)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt];
				int potId=(int)eventData.Parameters[(byte)LilyEventKey.MoneyPotId];
				if(potId==0)
					Room.Singleton.PokerGame.PlayerActionNames[playerPos]=ACTION_WIN;
				Room.Singleton.PokerGame.IsWin=true;	
				Room.Singleton.PokerGame.TableInfo.Round=TypeRound.Preflop;
			//	GlobalManager.Singleton.Notifications.Enqueue(new PlayerWonPotNotification((long)eventData.Parameters[(byte)LilyEventKey.AmountWon],playerInfo.NoSeat));
				DidPlayerWonPotEvent(potId,(long)eventData.Parameters[(byte)LilyEventKey.AmountWon],playerInfo.NoSeat);
			}
		}
		
		private void ProcessPlayerMoneyChangedEvent(EventData eventData)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				int noSeat=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
				long chips=(long)eventData.Parameters[(byte)LilyEventKey.Chip];
				if(Room.Singleton.PokerGame.TableInfo.Players.Exists(rs=>rs.NoSeat==noSeat))
				{
//					Debug.Log("NoSeat:"+noSeat);
//					Debug.Log("MoneySafeAmnt:"+(long)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt]);
					
					Room.Singleton.PokerGame.TableInfo.GetPlayer(noSeat).MoneySafeAmnt=(long)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt];
					Room.Singleton.PokerGame.TableInfo.GetPlayer(noSeat).Chips=chips;
				}
				
				if(User.Singleton.UserData.NoSeat==noSeat){
					
					User.Singleton.UserData.Chips=chips;
				}
			}
		}
		
		private void ProcessPlayerTurnEndedEvent(EventData eventData,bool isClient)
		{
			
			if((Room.Singleton.PokerGame.TableInfo!=null))
			{
				TableInfo tableInfo=Room.Singleton.PokerGame.TableInfo;
				//PlayerInfo playerInfo=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.PlayerInfo] as Hashtable) as PlayerInfo;
				//tableInfo.Players[playerInfo.NoSeat]=playerInfo;
				int noSeat=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
				long moneyBetAmnt=(long)eventData.Parameters[(byte)LilyEventKey.MoneyBetAmnt];
				long moneySafeAmnt=(long)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt];
				bool isPlaying=(bool)eventData.Parameters[(byte)LilyEventKey.IsPlaying];
				tableInfo.TotalPotAmnt=(long)eventData.Parameters[(byte)LilyEventKey.TotalPortAmnt];
				
				if(tableInfo.Players.Exists(rs=>rs.NoSeat==noSeat))
				{
					TypeAction typeAction=(TypeAction)eventData.Parameters[(byte)LilyEventKey.Action];
					long amnt=(long)eventData.Parameters[(byte)LilyEventKey.MoneyBetAmnt];
					if(typeAction==TypeAction.Raise)
					{
						tableInfo.NoSeatLastRaise=noSeat;
					}
					
					PlayerInfo player=tableInfo.GetPlayer(noSeat);
					player.MoneyBetAmnt=moneyBetAmnt;
//					Debug.Log("MoneySafeAmnt:"+(long)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt]);
					player.MoneySafeAmnt=moneySafeAmnt;
					if(player.IsPlaying!=isPlaying)
					{
						player.IsPlaying=isPlaying;
					}					
					
					if((!player.IsPlaying&&moneySafeAmnt==0)||(player.IsPlaying&&getMaxAllinValue(tableInfo,noSeat)<=0))
					{
						player.IsAllIn=true;
					}
					
					
					if(typeAction==TypeAction.Call){
						tableInfo.NbPlayed++;
					}
					if(typeAction==TypeAction.Raise)
					{
						int count = tableInfo.NbAllIn;
			            if (!player.IsAllIn)
			                count++;
			            tableInfo.NbPlayed = count;			
					}
					
					
					
					string s = "";
		            switch (typeAction)
		            {
		                case TypeAction.Call:
		                    if (amnt == 0)
		                        s = ACTION_CHECK;
		                    else
		                        s = ACTION_CALL;
		                    break;
		                case TypeAction.Raise:
		                    if (amnt == -1)
		                        s = ACTION_BET;
		                    else
		                        s = ACTION_RAISE;
		                    break;
		                case TypeAction.Fold:
		                    s = ACTION_FOLD;
		                    break;
		            }
					if(player.IsAllIn)
					{
						s=ACTION_ALLIN;
					}
		            Room.Singleton.PokerGame.PlayerActionNames[noSeat] = s;
					if(eventData.Parameters.ContainsKey((byte)LilyEventKey.StepID)){
 						Room.Singleton.PokerGame.StepId=(int)eventData.Parameters[(byte)LilyEventKey.StepID];
						Room.Singleton.PokerGame.WriteDropCache();
					}
					if(tableInfo.HigherBet<moneyBetAmnt)
					{
						tableInfo.HigherBet=moneyBetAmnt;
					}
					
				//	GlobalManager.Singleton.Notifications.Enqueue(new PlayerTurnEndedNotification(noSeat));
					DidProssPlayerTurnEndedEvent(noSeat,isClient);
				}
				
			}
		}
		private void PlayersShowCardsEvent(EventData eventData){
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
			
				if (eventData.Parameters.ContainsKey((byte)LilyEventKey.PlayerInfo)) {
					TableInfo table=Room.Singleton.PokerGame.TableInfo;
					//Debug.LogWarning("####################into show cards event####################");	
					Dictionary<int,string> players=eventData.Parameters[(byte)LilyEventKey.PlayerInfo] as Dictionary<int,string>;
					//SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.PlayerInfo] as Hashtable) as Dictionary<int,string>;
					foreach (var item in players) {
						//Debug.LogWarning(string.Format("####################oh fuck,{0},{1} ####################",item.Key,item.Value));
						PlayerInfo curPlay=table.GetPlayer(item.Key);
						GameCard[] card=new GameCard[2];
						string[] strCards=item.Value.Split(' ');
						card[0]=new GameCard(PokerWorld.HandEvaluator.Hand.ParseCard(strCards[0]));
						card[1]=new GameCard(PokerWorld.HandEvaluator.Hand.ParseCard(strCards[1]));
						curPlay.Cards=card;
						
					}					
					
					DidPlayersShowCardsEvent(players);
				}
			}
		}
		
		private void PlayerHoleCardsChangedEvent(EventData eventData)
		{	
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				int playerPos=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
				
//				Debug.LogWarning ("###################################IsPlaying:HoleCardsChanged,,,"+playerPos+"..noSeat"+User.Singleton.UserData.NoSeat);				
				if(playerPos>-1){	
				if(Room.Singleton.PokerGame.TableInfo.Players.Exists(rs=>rs.NoSeat==playerPos))
				{
					PlayerInfo playerInfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(playerPos);
					bool isPlaying=(bool)eventData.Parameters[(byte)LilyEventKey.IsPlaying];
					if(playerInfo.IsPlaying!=isPlaying)
					{
						playerInfo.IsPlaying=isPlaying;
					}
					//Debug.Log ("IsPlaying:"+playerInfo.IsPlaying);
					int[] gameCardIds=eventData.Parameters[(byte)LilyEventKey.GameCardIds] as int[];
					
					
					
					GameCard[] gameCards=new GameCard[2];
					for(int i=0;i<2;i++)
					{
						gameCards[i]=new GameCard(gameCardIds[i]);
					}
					playerInfo.Cards=gameCards;
					
					 
				}
				}else{
					if(User.Singleton.UserData.NoSeat>-1)
						return;
				}
                //Debug.LogWarning("PlayerHoleCardsChangedEvent");
				DidToPlayerHoleCardsChangedEvent(null);
				
			}
		}
		
		private void ProcessBetTurnStartedEvent(EventData eventData)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				TableInfo curTable=Room.Singleton.PokerGame.TableInfo;
				curTable.NbPlayed=0+curTable.NbAllIn;
				
				int[] gameCardIds=eventData[(byte)LilyEventKey.GameCardIds] as int[];
				GameCard[] cards=new GameCard[5];
				for(int i=0;i<5;i++)
				{
					cards[i]=new GameCard(gameCardIds[i]);
				}

				Room.Singleton.PokerGame.TableInfo.SetCards(cards[0],cards[1],cards[2],cards[3],cards[4]);
				Room.Singleton.PokerGame.TableInfo.Round=(TypeRound)eventData[(byte)LilyEventKey.TypeRound];
				if(Room.Singleton.PokerGame.TableInfo.Round==TypeRound.Preflop)
				{

//					Debug.Log("In ProcessBetTurnStartedEvent NoSeatBigBlind:"+Room.Singleton.PokerGame.TableInfo.NoSeatBigBlind);
					PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayingPlayerNextTo(Room.Singleton.PokerGame.TableInfo.NoSeatBigBlind);
					Room.Singleton.PokerGame.TableInfo.NoSeatLastRaise=player.NoSeat;
				}
				else
				{
					Room.Singleton.PokerGame.TableInfo.NoSeatLastRaise=Room.Singleton.PokerGame.TableInfo.GetPlayingPlayerNextTo(Room.Singleton.PokerGame.TableInfo.NoSeatDealer).NoSeat;
				}    
				//GlobalManager.Singleton.Notifications.Enqueue(new BetTurnStartedNotification());
				DidBetTurnStartEvent();
			}
		}
		
		private void ProcessBetTurnEndedEvent(EventData eventData)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				List<long> amounts=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.Amounts] as Hashtable) as List<long>;
				TableInfo tableInfo=Room.Singleton.PokerGame.TableInfo;
				tableInfo.Pots.Clear();
				tableInfo.TotalPotAmnt=0;
				tableInfo.HigherBet=0;
				for (int i = 0; i < amounts.Count && amounts[i] > 0; ++i)
	            {
	                tableInfo.Pots.Add(new MoneyPot(i, amounts[i]));
	                tableInfo.TotalPotAmnt += amounts[i];
	            }
				//GlobalManager.Singleton.Notifications.Enqueue(new BetTurnEndedNotification());
				DidBetTurnEndedEvent();
				foreach (PlayerInfo p in tableInfo.Players)
	            {
	                p.MoneyBetAmnt = 0;
	            }
				
				
			}
		}
		
		private void ProcessPlayerJoinedEvent(EventData eventData)
		{
			 if(Room.Singleton.PokerGame.TableInfo==null)
			 	Room.Singleton.PokerGame.TableInfo=new TableInfo("diyihai",10,9, TypeBet.NoLimit);
			
//			if(Room.Singleton.PokerGame.TableInfo!=null)
//			{
				
				PlayerInfo playerInfo= SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.PlayerInfo] as Hashtable) as PlayerInfo;
	            if (playerInfo != null)
	            {
					Room.Singleton.PokerGame.PlayerActionNames[playerInfo.NoSeat]="";
	                Room.Singleton.PokerGame.TableInfo.ForceJoinTable(playerInfo, playerInfo.NoSeat);
				
					
					
					
//				 Debug.LogWarning("playerinfo:"+playerInfo.MoneySafeAmnt);
					//GlobalManager.Singleton.Notifications.Enqueue(new PlayJoinedNotification(playerInfo.NoSeat));
					DidPlayerJoinedEvent(playerInfo.NoSeat);
	            } 
			//}
//			Debug.Log("ProcessPlayerJoinedEvent");
		}
		
		private void ProcessPlayerLeavedEvent(EventData eventData)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				//int playerPos=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
				string playerName=eventData.Parameters[(byte)LilyEventKey.NickName] as string;
				string message=eventData.Parameters[(byte)LilyEventKey.MessageContent] as string;
//				bool doStand=(bool)eventData.Parameters[(byte)LilyEventKey.Bystander];
//				bool isKick=(bool)eventData.Parameters[(byte)LilyEventKey.IsKick];
				
				if (isFastStart) {
					return;
				}
				
				
			//	Debug.LogWarning("fuck you "+message);
				PlayerLeaveType leaveType=(PlayerLeaveType)eventData.Parameters[(byte)LilyEventKey.PlayerLeaveType];
				bool isKick=false,doStand=false;
				switch (leaveType) {
				case PlayerLeaveType.Kick:
				case PlayerLeaveType.KickFoldTwice:
					isKick=true;
					break;
				case PlayerLeaveType.KickByPlayer:
					if (playerName == User.Singleton.UserData.NickName)
					{
						isKick=true;
						GlobalManager.Singleton.Messages.Enqueue(new KickedByVIPMessage(message));
					}
					break;
				case PlayerLeaveType.Stand:
					doStand=true;
					break;					
					default:
						isKick=false;
						doStand=false;
					break;
				}
				
				 
				PlayerInfo playerInfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(playerName);
				int noseat=-1;
//				Debug.LogWarning ("In PlayerLeaved PlayerName:"+playerName);
				if(playerInfo!=null)
				{
					noseat=playerInfo.NoSeat;
					
					DidPlayerLeavedEvent(noseat,isKick,leaveType);
					if(User.Singleton.UserData.NoSeat==noseat&&User.Singleton.UserData.Name==playerName && !doStand)
					{
						User.Singleton.UserData.NoSeat=-1;
					}
					Room.Singleton.PokerGame.TableInfo.LeaveTable(playerInfo);
				}else{
				//	if(playerName==User.Singleton.UserData.NickName)
				//		JoinRoom(User.Singleton.UserData.UserId);	
				}
//				Debug.LogWarning ("In PlayerLeaved Noseat:"+User.Singleton.UserData.NoSeat);
				if(doStand)
				{
					if(playerInfo!=null)
					{
						playerInfo.GiftId=0;
						Room.Singleton.PokerGame.TableInfo.addBystander(playerInfo);
					}
				}
				else
				{
					Room.Singleton.PokerGame.TableInfo.deleteBystander(new PlayerInfo(playerName));
				}
				
				if(playerInfo!=null)
				{
					UserData user= Room.Singleton.RoomData.Users.Find (r=>r.NickName==playerInfo.Name);
					if(user!=null)
						user.IsSitting=false;
				}
				
				if(playerName==User.Singleton.UserData.NickName&&!isKick&&!doStand)
				{
					return;
				}
				//GlobalManager.Singleton.Notifications.Enqueue(new PlayerLeavedNotification());
				DidInRoomPlayerLeavedEvent();
			}
		}
		
		private void ProcessTableClosedEvent(EventData eventData)
		{
			GlobalManager.Log("Table Closed");
		}
		
		private void ProcessPlayerTurnBeganEvent(EventData eventData,bool isClient)
		{
			if(Room.Singleton.PokerGame.TableInfo!=null)
			{
				int playerPos=(int)eventData.Parameters[(byte)LilyEventKey.NoSeat];
				int lastPlayerNoSeat=(int)eventData.Parameters[(byte)LilyEventKey.LastNoSeat];
				long highBet=(long)eventData.Parameters[(byte)LilyEventKey.HigherBet];
				TableInfo tableInfo=Room.Singleton.PokerGame.TableInfo;
				tableInfo.HigherBet=highBet;
				if(tableInfo.Players.Exists(rs=>rs.NoSeat==playerPos)&&tableInfo.Players.Exists(rs=>rs.NoSeat==lastPlayerNoSeat))
				{
					tableInfo.NoSeatCurrPlayer=playerPos;
				//	GlobalManager.Singleton.Notifications.Enqueue(new PlayerTurnBeganNotification(playerPos));
					DidProcessPlayerTurnBeganEvent(playerPos,isClient);
				}
			}
		}
		
		private void ProcessSendChipEvent(EventData eventData)
		{
//			Debug.Log("ProcessSendChipEvent");
			UserData userData=SerializeHelper.Deserialize(eventData.Parameters[(byte)LilyEventKey.UserData] as Hashtable) as UserData;
			string content=eventData.Parameters[(byte)LilyEventKey.MessageContent] as string;
			GlobalManager.Singleton.Messages.Enqueue(new SendChipMessage(userData,content));
		}
		
		private void ProcessSendGiftEvent(EventData eventData)
		{
			int senderNoSeat=(int)eventData.Parameters[(byte)LilyEventKey.SendNoSeat];
			int receiverNoSeat=(int)eventData.Parameters[(byte)LilyEventKey.ReceiverNoSeat];
			int giftId=(int)eventData.Parameters[(byte)LilyEventKey.GiftId];
			long moneySafeAmnt=(long)eventData.Parameters[(byte)LilyEventKey.MoneySafeAmnt];
			long chips=(long)eventData.Parameters[(byte)LilyEventKey.Chip];
			PlayerInfo sender=Room.Singleton.PokerGame.TableInfo.GetPlayer(senderNoSeat);
			sender.MoneySafeAmnt=moneySafeAmnt;
			sender.Chips=chips;
			if(User.Singleton.UserData.NoSeat==senderNoSeat)
			{
				User.Singleton.UserData.Chips=chips;	
			}
			if(receiverNoSeat==-1)
			{
				foreach(PlayerInfo player in Room.Singleton.PokerGame.TableInfo.Players)
				{
					player.GiftId=giftId;
				}
			}
			else
			{
				PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayer(receiverNoSeat);
				player.GiftId=giftId;
			}
			//GlobalManager.Singleton.Notifications.Enqueue(new SendGiftNotification(senderNoSeat,receiverNoSeat,giftId));
 			DidSendGiftEvent(senderNoSeat,receiverNoSeat,giftId);
		}
		
		public void ProcessAchievementEvent(EventData eventData)
		{
			byte number=(byte)eventData.Parameters[(byte)LilyEventKey.AchievementNumber];
			string content=eventData.Parameters[(byte)LilyEventKey.MessageContent] as string;
			if(string.IsNullOrEmpty(content))
			{
				GlobalManager.Singleton.Messages.Enqueue(new AchievementMessage(number));
			}
			else
			{
				GlobalManager.Singleton.Messages.Enqueue(new AchievementMessage(number,content));
			}
		}
		
		private void ProcessBroadcastMessageInTable(EventData eventData)
		{
			int noseat=(int)(eventData.Parameters[(byte)LilyEventKey.NoSeat]);
			string message=(eventData.Parameters[(byte)LilyEventKey.Message] as string);
			GlobalManager.Log ("NoSeat:"+eventData.Parameters[(byte)LilyEventKey.NoSeat]+","+(eventData.Parameters[(byte)LilyEventKey.Message] as string));
			DidBroadcastMessageInTableEvent(noseat,message);
		}
		#endregion
		
		#region Operation Response Method
		private bool IsOperationSuccess(OperationResponse operationResponse)
		{
			try
			{
				return (int)operationResponse.Parameters[(byte)LilyOpKey.ErrorCode]==(int)ErrorCode.Sucess;
			}
			catch
			{
				Debug.LogError("The OperationCode in IsOperationSuccess() is:"+operationResponse.OperationCode);
				return false;
			}
		}
		
		private void ProcessError(OperationResponse operationResponse)
		{
			ErrorCode errorCode = (ErrorCode)operationResponse.Parameters[(byte)LilyOpKey.ErrorCode];
			InviterId=string.Empty;
			GlobalManager.Log ("ProcessError:"+errorCode);
//			Debug.Log ("ProcessError operation code: " + operationResponse.OperationCode);
			if ((LilyOpCode)operationResponse.OperationCode == LilyOpCode.Logout)
			{
				if (logoutEvent != null)
					logoutEvent(false);
			}
			else if ((LilyOpCode)operationResponse.OperationCode == LilyOpCode.UpgradeUrl)
			{
				if (upgradUrlEvent != null)
					upgradUrlEvent(null);
			}
			else if ((LilyOpCode)operationResponse.OperationCode == LilyOpCode.Join)
			{
				if (this.CurrentAction == null)
				{
					if (gotoBackgroundSceneEvent != null)
						gotoBackgroundSceneEvent();
				}
			}
			else 	if((LilyOpCode)operationResponse.OperationCode==LilyOpCode.CheckEmail)
			{
				GlobalManager.Singleton.Notifications.Enqueue(new CheckEmailNotification(false));
			}
			else if ((LilyOpCode)operationResponse.OperationCode==LilyOpCode.BuyItem)
			{
				CloseMaskingTable();
			}
			
			switch(errorCode)
			{
			case ErrorCode.The3rdUserNotRegistered:
				GlobalManager.Singleton.Notifications.Enqueue(new The3rdFirstLoginNotification());
				break;
			case ErrorCode.SystemError:
				//this.ErrorMessage=LocalizeHelper.Translate("LOGIN_SYSTEM_ERROR");
				GlobalManager.Singleton.Notifications.Enqueue(new ErrorNotification(errorCode));
				break;
			case ErrorCode.AuthenticationFail:
				//this.ErrorMessage=LocalizeHelper.Translate("LOGIN_AUTHENTICATION_FAIL");
				GlobalManager.Singleton.Notifications.Enqueue(new ErrorNotification(errorCode));
				DidLoginUserNameOrPasswordErrorEvent();
				break;
			case ErrorCode.MailIsEmpty:
			case ErrorCode.PassWordIsEmpty:
			case ErrorCode.UserExist:			
			case ErrorCode.MailExist:
			case ErrorCode.NickNameExist:
				//this.ErrorMessage=LocalizeHelper.Translate("LOGIN_USER_EXIST");
//				Debug.Log(this.ErrorMessage);
				GlobalManager.Singleton.Notifications.Enqueue(new ErrorNotification(errorCode));
				break;
			case ErrorCode.NoResult:
				switch((LilyOpCode)operationResponse.OperationCode)
				{
//				case LilyOpCode.Login:
//					this.ErrorMessage=LocalizeHelper.Translate("LOGIN_NO_RESULT");
//					break;
				case LilyOpCode.SearchUser:
					GlobalManager.Singleton.SearchUsers = null;
					SearchUserDidFinished();
					break;
				case LilyOpCode.FriendList:
					User.Singleton.Friends = null;
					OnFriendListFinish(true);
					break;
				}
				break;
			case ErrorCode.GameEnded:
			case ErrorCode.TableNotExist:
			case ErrorCode.ChipsNotEnough:
			case ErrorCode.GameIdNotExists:
			    if(User.Singleton.GameStatus==GameStatus.InRoom || User.Singleton.GameStatus == GameStatus.Error)
				{
					this.ErrorMessage = errorCode.ToString();
//					Debug.Log("ErrorCode.GameIdNotExists");
					JoinGameDidFinished(false,TypeState.Init);
					DidJoinGameEventFinished(ErrorCode.GameIdNotExists,TypeState.Init);
					DidTryJointGameEvent(false);
				}
				else
				{
//					Debug.Log("aaa TableNotExist or ChipsNotEnough");
					this.JoinRoom(User.Singleton.UserData.UserId);
				}
				
				//DidSyncGameDataTableInfoEvent(false); 
				break;
			case ErrorCode.UserNotExist:
				//EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("FIND_PASSWORD_FAILED_TITLE"), LocalizeHelper.Translate("FIND_PASSWORD_FAILED_DESCRIPTION"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
				GlobalManager.Singleton.Notifications.Enqueue(new FindPasswordNotification(false));
				break;
//			case ErrorCode.GameIdNotExists:
//				switch ((LilyOpCode)operationResponse.OperationCode) {
//				case LilyOpCode.TryJoinTable:
////					this.ErrorMessage = errorCode.ToString();
////					JoinGameDidFinished();	
//					
//					break;					
//				default:
//					break;
//				}
				
//				break;
				
			case ErrorCode.TableFull:
				switch ((LilyOpCode)operationResponse.OperationCode) {
				case LilyOpCode.JoinTable:
				case LilyOpCode.JoinGame:
				case LilyOpCode.TryJoinTable:
//					Debug.Log("game full");
					this.ErrorMessage = errorCode.ToString();
					JoinGameDidFinished(false,TypeState.Init);
					DidJoinGameEventFinished(ErrorCode.TableFull,TypeState.Init);
					break;
				case LilyOpCode.Sit:
//					Debug.Log("there is a sb already");
					break;					
				default:
					break;
				}
				
				break;	
			case ErrorCode.VerifyFail:
					Debug.Log("parment verifyfail");
					if (buyItemResponseEvent!=null)
						buyItemResponseEvent(false, ItemType.Room);
				break;
			
			case ErrorCode.KickErrorLevel:		
//				Debug.Log("fuck his level > your");
//				if(DoNotSuccessKickPlayerEven!=null)
//				{
//					DoNotSuccessKickPlayerEven(errorCode);
//				}
//				break; 
			case ErrorCode.KickErrorLimit:	
//				Debug.Log("num > you vip level");
//				 if(DoNotSuccessKickPlayerEven!=null)
//				{
//					DoNotSuccessKickPlayerEven(errorCode);
//				}
//				break;
			case ErrorCode.KickErrorOwner:	
//				Debug.Log("can not kick the owner of room");
//				 if(DoNotSuccessKickPlayerEven!=null)
//				{
//					DoNotSuccessKickPlayerEven(errorCode);
//				}
//				break;
			case ErrorCode.KickErrorPlaying:	
//					Debug.Log("can not kick the playing user");
				if(doNotSuccessKickPlayerEven!=null)
				{
					doNotSuccessKickPlayerEven(errorCode);
				}
				break;
				
				
			//buyItemErroCode
			case ErrorCode.BuyItemChipNotEnough:
				if (buyItemResponseEvent!=null)
					buyItemResponseEvent(false, ItemType.Room);
				break;
			case ErrorCode.BuyItemNotFound:
				if (buyItemResponseEvent!=null)
					buyItemResponseEvent(false, ItemType.Room);
				break;				
				
			default:
				GlobalManager.Log(errorCode);
//			Debug.Log(errorCode.ToString());
				//this.ErrorMessage=LocalizeHelper.Translate("LOGIN_UNKNOWN_ERROR");
				break;
			}
			
			if ((LilyOpCode)operationResponse.OperationCode == LilyOpCode.GuestUpgrade
				|| (LilyOpCode)operationResponse.OperationCode == LilyOpCode.Register)
			{
				ResigerOrUpgradeFinished(false);
			}
		}
		
		private void CreateGameResponse(OperationResponse operationResponse){
			
			
			string InviterServer = operationResponse.Parameters[(byte)LilyOpKey.Address] as string;
            InviterRoom = operationResponse.Parameters[(byte)LilyOpKey.GameId] as string;
			byte nodeid = (byte)operationResponse.Parameters[(byte)LilyOpKey.NodeId];
			
			
			if(peer_toGameServer!=null&&InviterServer==peer_toGameServer.ServerName&&peer_toGameServer.PeerState==PeerStateValue.Connected)
			{
				doJoinRoomOnGameServer ();
				return;
			}
			
			
			gameSeverStatus=GameServerStatus.AskForTheGameServer;
			if(peer_toGameServer!=null&&peer_toGameServer.PeerState==PeerStateValue.Connected)
				peer_toGameServer.Disconnect();
			
			peer_toGameServer = new SelfPeer(this);
			
			peer_toGameServer.ServerName = InviterServer;
			peer_toGameServer.ApplicationName = InviterAppName;
			peer_toGameServer.NodeId = nodeid;	
			if(peer_toGameServer.PeerState==PeerStateValue.Disconnected||peer_toGameServer.PeerState==PeerStateValue.InitializingApplication)
            	peer_toGameServer.Connect(InviterServer, InviterAppName, nodeid);
			else
				peer_toGameServer.Disconnect();
		}
		
	
		
		private void RegisterResponse(OperationResponse operationResponse)
		{
			UserData userData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.UserData] as Hashtable) as UserData;
			if(userData!=null)
			{
//				Debug.Log("RegisterResponse *** ");
				User.Singleton.UserData=userData;
				if (User.Singleton.Avators != null && !User.Singleton.Avators.Exists(r => r.ItemId == (int)User.Singleton.UserData.Avator))
				{
					Props prop = new Props();
					prop.Id = -1;
					prop.ItemId = (int)User.Singleton.UserData.Avator;
					prop.ItemType = ItemType.Avator;
					User.Singleton.Avators.Add(prop);
				}
				Debug.LogWarning(GlobalManager.Singleton.ClientChannelId+"   "+userData.UserId);
//#if UNITY_ANDROID
				BindChannelId(GlobalManager.Singleton.ClientChannelId, userData.UserId);
//#endif
//				
//#if UNTIY_IPHONE
//				string channelID = DeviceInfoHelp.GetChannelId();
//				BindChannelId(channelID, userData.UserId);
//#endif
				ResigerOrUpgradeFinished(true);
				if(Room.Singleton.RoomData == null)
					Room.Singleton.RoomData = new RoomData();
				if(Room.Singleton.RoomData.Users == null)
					Room.Singleton.RoomData.Users = new List<UserData> ();
				if(Room.Singleton.RoomData.Users!=null&&Room.Singleton.RoomData.Users.Count == 0)
				{
					Room.Singleton.RoomData.Users.Add(userData);
				}
				
				GlobalManager.Singleton.Messages.Enqueue(new RegisterMessage(userData));
 				if(registerInSetupSuccessEvent!=null)
				{
					registerInSetupSuccessEvent();
				}
				if ((LilyOpCode)operationResponse.OperationCode == LilyOpCode.Register)
				{
					this.JoinRoom(userData.UserId);
				}
				SavePersonalAccountInfo();
			}
		}
		
		void ResigerOrUpgradeFinished(bool result)
		{
			if (registerOrUpgradeEvent != null)
				registerOrUpgradeEvent(result);
		}
		
		private void LoginReponse(OperationResponse operationResponse)
		{			
			if (gotoLoadingSceneEvent != null){
					gotoLoadingSceneEvent();			
			}
			bool lijidenglu=false; 
			//if(LOADBALANCE)
			//this.AskForTheGameServer();	
			
			
			UserData userData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.UserData] as Hashtable) as UserData;
			if(userData!=null)
			{
				int noSeat=-1;
				if(User.Singleton.UserData!=null){
					noSeat=User.Singleton.UserData.NoSeat;
					lijidenglu= userData.UserId!=User.Singleton.UserData.UserId&&!string.IsNullOrEmpty(User.Singleton.UserData.UserId);
				}
				User.Singleton.UserData=userData;
				User.Singleton.UserData.NoSeat=noSeat;
				if(operationResponse.Parameters.ContainsKey((byte)LilyOpKey.Messages))
				{
					List<UserMessage> messages=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.Messages] as Hashtable) as List<UserMessage>;
					foreach(UserMessage userMessage in messages)
					{
						switch(userMessage.MessageType)
						{
						case MessageType.RequestFriend:
							GlobalManager.Singleton.Messages.Enqueue(new RequestFriendMessage(userMessage.Sender));
							break;
						case MessageType.SendChip:
							GlobalManager.Singleton.Messages.Enqueue(new SendChipMessage(userMessage.Sender,userMessage.Content));
							break;
						case MessageType.AppScore:
							//GlobalManager.Singleton.Messages.Enqueue(new AppScoreMessage());
							User.Singleton.AppScore=true;
							break;
						}
					}
				}
				if(operationResponse.Parameters.ContainsKey((byte)LilyOpKey.HistoryAction))
				{
//					Debug.Log ("History In LoginResponse");
					List<PlayerAction> playerActions=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.HistoryAction] as Hashtable) as List<PlayerAction>;
//					Debug.Log (playerActions.Count);
					Room.Singleton.PokerGame.SetPlayerActions(playerActions);
					this.CurrentAction=JoinGame; // todo: ASK TO lee
					//if(!LOADBALANCE)
					this.JoinRoom(tempObj as string,true);
				}
				else
				{
					Room.Singleton.PokerGame.PlayerActions.Clear();
					if(lijidenglu)
						this.JoinRoom(User.Singleton.UserData.UserId);
					else
						this.JoinRoom(User.Singleton.UserData.UserId,true);
				}
//				GlobalManager.Singleton.AwardChip=(long)operationResponse.Parameters[(byte)LilyOpKey.Chip];
//				Debug.Log("AwardChip:"+GlobalManager.Singleton.AwardChip);
				GlobalManager.Singleton.StartTiming();	
				
				SavePersonalAccountInfo();
			}
			
		}
		
//		private void SavePersonalAccountInfo()
//		{
//			string accountInfo = User.Singleton.UserData.Mail + "|";
//			accountInfo = accountInfo + User.Singleton.UserData.Password + "|";
//			accountInfo = accountInfo + ((int)User.Singleton.UserData.UserType).ToString();
//			FileIOHelper.WriteFile(FileType.Account, accountInfo);
//		}
		
		private void LogoutReponse(OperationResponse operationResponse)
		{
			User.Singleton.GameStatus=GameStatus.Logout;
			Room.Singleton.PokerGame.ClearDropCache();
			if (logoutEvent != null)
				logoutEvent(true);
		}
		
		private void SearchUserResponse(OperationResponse operationResponse)
		{
			List<UserData> searchUsers=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.FriendList] as Hashtable) as List<UserData>;
			GlobalManager.Log (searchUsers.Count.ToString());
			GlobalManager.Singleton.SearchUsers=searchUsers;
			SearchUserDidFinished();
		}
		
		private void RequestFriendResponse(OperationResponse operationResponse)
		{
			GlobalManager.Log ("Request success");
		}
		
		private void AddFriendReponse(OperationResponse operationResponse)
		{
			GlobalManager.Log("Add Friend success");
			UserData userData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.UserData] as Hashtable) as UserData;
			if(User.Singleton.Friends==null)
			{
				User.Singleton.Friends=new List<UserData>();
			}
			User.Singleton.Friends.Add(userData);
		}
		
		private void DeleteFriendResponse(OperationResponse operationResponse)
		{
			string userId=tempObj as string;
			if(User.Singleton.Friends.Exists(rs=>rs.UserId==userId))
			{
				UserData userData=User.Singleton.Friends.Find(rs=>rs.UserId==userId);
				User.Singleton.Friends.Remove(userData);
			}
			tempObj=null;
		}
		
		private void AcceptFriendResponse(OperationResponse operationResponse)
		{
			RoomData roomData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.RoomData] as Hashtable) as RoomData;
			if(roomData!=null)
			{
				GlobalManager.Log("RoomType:"+roomData.Owner.LivingRoomType+",RoomId:"+roomData.Owner.UserId);
				Room.Singleton.RoomData=roomData;
			}
		}
		
		private void InviteFriendResponse(OperationResponse operationResponse)
		{
			GlobalManager.Log ("Invite success");
		}
		
		private void AccessFriendResponse(OperationResponse operationResponse)
		{
			UserData userData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.UserData] as Hashtable) as UserData;
			GlobalManager.Log("NickName:"+userData.NickName+"UserId:"+userData.UserId+"Mail:"+userData.Mail);
		}
		
		private void FriendListResponse(OperationResponse operationResponse)
		{
			GlobalManager.Log ("FriendListResponse in PhotonClient.cs");
			List<UserData> friends=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.FriendList] as Hashtable) as List<UserData>;
			User.Singleton.Friends = friends;
			string strLog="";
//			foreach(UserData friend in friends)
//			{
//				strLog+=friend.Status.ToString()+",";
//			}
			//Debug.Log (strLog);
			OnFriendListFinish(true);
		}
		
		private void OnFriendListFinish(bool result)
		{
			if (friendListEvent != null)
				friendListEvent(result);
		}
		
		private void JoinResponse(OperationResponse operationResponse)
		{			
			
			if (QuickStartAction!=null) {
				QuickStartAction();
				QuickStartAction=null;
				return;
			}
			RoomData roomData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.RoomData] as Hashtable) as RoomData;
			if(roomData!=null)
			{
				GlobalManager.Log("RoomType:"+roomData.Owner.LivingRoomType+",RoomId:"+roomData.Owner.UserId+",Avator:"+roomData.Owner.Avator+",Chips:"+roomData.Owner.Chips);
				Room.Singleton.RoomData=roomData;
				PriRoomUser = Room.Singleton.RoomData.Owner;
				
				if (operationResponse.Parameters.ContainsKey((byte)LilyOpKey.AppScore)) {
					User.Singleton.AppScore=true;
				}
				
				TableInfo tableinfo=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.TableInfo] as Hashtable) as TableInfo;
				Room.Singleton.PokerGame.TableInfo=tableinfo;
				if(this.CurrentAction!=null)
				{
					Debug.Log ("CurrentAction is not null");
					this.CurrentAction();
					this.CurrentAction=null;
				}
				else
				{
					Debug.Log ("CurrentAction is null ");
					User.Singleton.GameStatus=GameStatus.InRoom;
					
					if (gotoBackgroundSceneEvent != null && !isFastStart)
					{
						 Debug.Log ("CurrentAction is null "+isFastStart);
						 gotoBackgroundSceneEvent();
					}
					isFastStart = false;
					
					DidJoinRoomEvent();
				}				
				//MusicManager.Singleton.PlayBgMusic();
				//DidJoinRoomEvent();
			}
			Room.Singleton.PokerGame.ClearDropCache();	
			
		}
		
//		private void LeaveResponse(OperationResponse operationResponse)
//		{
//			if(User.Singleton.GameStatus==GameStatus.JoinRoom)
//			{
//				this.JoinRoom(Room.Singleton.RoomData.RoomId);
//			}
//		}
		
		private void QuickStartResponse(OperationResponse operationResponse)
		{
//			User.Singleton.GameStatus=GameStatus.InGame;
//			RoomData roomData=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.RoomData] as Hashtable) as RoomData;
//			if(roomData!=null)
//			{
//				GlobalManager.Log("RoomType:"+roomData.Owner.LivingRoomType+",RoomId:"+roomData.Owner.UserId);
//				Room.Singleton.RoomData=roomData;
//				RoomController.Singleton.PlayBgMusic();
//			}
			JoinGameResponse(operationResponse);
		}
		
		private void SyncGameDataResponse(OperationResponse operationResponse){			
			if (Room.Singleton.PokerGame.TableInfo!=null) {
				
				if(operationResponse.Parameters.ContainsKey((byte)LilyOpKey.TableInfo))
				{
					TableInfo tableinfo=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.TableInfo] as Hashtable) as TableInfo;
					int[] playingNoSeats=operationResponse.Parameters[(byte)LilyOpKey.PlayingNoSeats] as int[];
					int[] allInNoSeats=operationResponse.Parameters[(byte)LilyOpKey.AllInNoSeats] as int[];
					
					foreach(int playingNoSeat in playingNoSeats)
					{
//						Debug.LogWarning ("############################################### fuck playerinfo:"+playingNoSeat);
						PlayerInfo playinfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(playingNoSeat);
						if(playinfo!=null)
						{
							playinfo.IsPlaying=true;
						}
					}
					foreach(int allInNoSeat in allInNoSeats)
					{
						
						PlayerInfo playinfo=Room.Singleton.PokerGame.TableInfo.GetPlayer(allInNoSeat);
						if(playinfo!=null)
						{
							playinfo.IsAllIn=true;
						}
					}
					
 				 
					 
 
					foreach (PlayerInfo play in tableinfo.Players) {
						if((play as UserData).UserId==User.Singleton.UserData.UserId)
							User.Singleton.UserData.NoSeat=play.NoSeat;
					}
					  DidSyncGameDataTableInfoEvent(true);
					
 
				}else{
						int[] gameCardIds=operationResponse.Parameters[(byte)LilyOpKey.GameCardIds] as int[];
						GameCard[] cards=new GameCard[5];
						for(int i=0;i<5;i++)
						{
							cards[i]=new GameCard(gameCardIds[i]);					
						}
						Room.Singleton.PokerGame.TableInfo.SetCards (cards[0],cards[1],cards[2],cards[3],cards[4]);
						if(operationResponse.Parameters.ContainsKey ((byte)LilyOpKey.PlayerCardId))
						{
							int[] playerCards=operationResponse.Parameters[(byte)LilyOpKey.PlayerCardId] as int[];
							int noseat=User.Singleton.UserData.NoSeat;
							PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayer (noseat);
							GameCard[] gameCards=new GameCard[2];
							for(int i=0;i<2;i++)
							{
								gameCards[i]=new GameCard(playerCards[i]);
							}
							player.Cards=gameCards;	
						}
				}
				
				
				
				
			}
		}		
		
		private void JoinGameResponse(OperationResponse operationResponse)
		{
			Debug.Log("JoinGameResponse");
			InviterId=string.Empty;
			User.Singleton.GameStatus=GameStatus.InGame;
			PhotonClient.Singleton.ErrorMessage = ((ErrorCode)operationResponse.Parameters[(byte)LilyOpKey.ErrorCode]).ToString();
			Room.Singleton.PokerGame.TableInfo=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.TableInfo] as Hashtable) as TableInfo;
			int[] playingNoSeats=operationResponse.Parameters[(byte)LilyOpKey.PlayingNoSeats] as int[];
			int[] allInNoSeats=operationResponse.Parameters[(byte)LilyOpKey.AllInNoSeats] as int[];
			
			if(operationResponse.Parameters.ContainsKey ((byte)LilyOpKey.GameId)){
			int[] gameCardIds=operationResponse.Parameters[(byte)LilyOpKey.GameId] as int[];
				GameCard[] cards=new GameCard[5];
				for(int i=0;i<5;i++)
				{
					cards[i]=new GameCard(gameCardIds[i]);					
				}
				Room.Singleton.PokerGame.TableInfo.SetCards (cards[0],cards[1],cards[2],cards[3],cards[4]);	
				if(cards[0].ToString ()!="--")
					DidBetTurnStartEvent();
				Debug.LogWarning ("tableinfo 5cards:"+cards[0].ToString ()+","+cards[1].ToString ()+","+cards[2].ToString ()+","+cards[3].ToString ()+","+cards[4].ToString ());
			}
			
			
			TypeState gameTypeState=(TypeState)operationResponse.Parameters[(byte)LilyOpKey.GameTypeState];
			
			
			Debug.LogWarning ("tableinfo:"+Room.Singleton.PokerGame.TableInfo.TotalPotAmnt);
			
			foreach(int playingNoSeat in playingNoSeats)
			{
				//Debug.Log ("playerinfo:"+playingNoSeat);
				Room.Singleton.PokerGame.TableInfo.GetPlayer(playingNoSeat).IsPlaying=true;
			}
			foreach(int allInNoSeat in allInNoSeats)
			{
				Room.Singleton.PokerGame.TableInfo.GetPlayer(allInNoSeat).IsAllIn=true;
			}
			int noseat=(int)operationResponse.Parameters[(byte)LilyOpKey.NoSeat];
			User.Singleton.UserData.NoSeat=noseat;			
			
//			if(operationResponse.Parameters.ContainsKey ((byte)LilyOpKey.PlayerCardId)){
//				int[] playerCards=operationResponse.Parameters[(byte)LilyOpKey.PlayerCardId] as int[];
//				PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayer (noseat);
//				GameCard[] gameCards=new GameCard[2];
//					for(int i=0;i<2;i++)
//					{
//						gameCards[i]=new GameCard(playerCards[i]);
//					}
//				player.Cards=gameCards;	
//			}
			
			foreach(PlayerInfo playerInfo in Room.Singleton.PokerGame.TableInfo.Players)
			{
				Room.Singleton.PokerGame.PlayerActionNames[playerInfo.NoSeat]="";						
			}
			if(this.CurrentAction!=null)
			{
				this.CurrentAction();
				this.CurrentAction=null;
			}
			
			if (operationResponse.Parameters.ContainsKey((byte)LilyOpKey.OnlineAwards)) {
				User.Singleton.canGetOnlineAwards=true;
			}
			else
				User.Singleton.canGetOnlineAwards=false;
			
			
						
			JoinGameDidFinished(true,gameTypeState);
			DidJoinGameEventFinished(ErrorCode.Ok,gameTypeState);
//			if(Room.Singleton.PokerGame.TableInfo.Players.Count==1)
//			{
//				Thread thread=new Thread(new ThreadStart(()=>{
//					Thread.Sleep(7000);
//					if(Room.Singleton.PokerGame.TableInfo.Players.Count==1)
//					{
//						this.RequestRobot();
//					}
//				} ));
//				thread.Start();
//			}
		}
		
		private void LeaveGameResponse(OperationResponse operationResponse)
		{
			User.Singleton.GameStatus=GameStatus.InRoom;
			DidLeaveGameFinished();
		}
		
		private void SitResponse(OperationResponse operationResponse)
		{
			User.Singleton.GameStatus=GameStatus.Sit;
			Room.Singleton.PokerGame.TableInfo=SerializeHelper.Deserialize(operationResponse.Parameters[(byte)LilyOpKey.TableInfo] as Hashtable) as TableInfo;
			//GlobalManager.Log (Room.Singleton.PokerGame.TableInfo.NoSeatCurrPlayer);
			//GlobalManager.Log (Room.Singleton.PokerGame.TableInfo.Players.Count);	
			int[] playingNoSeats=operationResponse.Parameters[(byte)LilyOpKey.PlayingNoSeats] as int[];
			int[] allInNoSeats=operationResponse.Parameters[(byte)LilyOpKey.AllInNoSeats] as int[];
			foreach(int playingNoSeat in playingNoSeats)
			{
				//Debug.Log ("playerinfo:"+playingNoSeat);
				Room.Singleton.PokerGame.TableInfo.GetPlayer(playingNoSeat).IsPlaying=true;
			}
			foreach(int allInNoSeat in allInNoSeats)
			{
				Room.Singleton.PokerGame.TableInfo.GetPlayer(allInNoSeat).IsAllIn=true;
			}		
			if (operationResponse.Parameters.ContainsKey((byte)LilyOpKey.OnlineAwards)) {
				User.Singleton.canGetOnlineAwards=true;
			}
			else
				User.Singleton.canGetOnlineAwards=false;
			
			User.Singleton.UserData.NoSeat=(int)operationResponse.Parameters[(byte)LilyOpKey.NoSeat];
			
			TypeState gameTypeState=(TypeState)operationResponse.Parameters[(byte)LilyOpKey.GameTypeState];
			
			
			DidSitDownEvent(gameTypeState);
		}
		
		public void BuyItemResponse(OperationResponse operationResponse)
		{
			Debug.Log ("In BuyItemResponse");
			CloseMaskingTable();
			if (operationResponse.Parameters.ContainsKey((byte)LilyOpKey.Feedback))
			{
				Shop.Singleton.RemoveCurrentIAP();
				return;
			}
			Debug.Log ("In BuyItemResponse Befaore switch");
			switch(Shop.Singleton.TempItemType)
			{
			case ItemType.Room:
				User.Singleton.UserData.OwnRoomTypes|=(RoomType)Shop.Singleton.TempItemId;
				break;
			case ItemType.Chip:
				User.Singleton.UserData.Chips+=Shop.Singleton.TempItemId;
				User.Singleton.UserInfoChanged();
				if(Room.Singleton.PokerGame.TableInfo!=null){
					PlayerInfo player=Room.Singleton.PokerGame.TableInfo.GetPlayer(User.Singleton.UserData.NoSeat);
					if(player!=null)
						player.Chips=User.Singleton.UserData.Chips;
				}
//				Debug.Log("hi, your chips increased!" + User.Singleton.UserData.Chips.ToString());
				break;
			case ItemType.Avator:{
				Props newProps=SerializeHelper.Deserialize (operationResponse.Parameters[(byte)LilyOpKey.UserProps] as Hashtable) as Props;	
				
				if(User.Singleton.Avators.Exists (r=>r.Id==newProps.Id))
				{
					Props oldProp=User.Singleton.Avators.Find(r=>r.Id==newProps.Id);
					oldProp=newProps;
				}else
				{
					User.Singleton.Avators.Add(newProps);
				}
				break;
			}
			case ItemType.Jade:
			case ItemType.Lineage:
			{
				Props newProps=SerializeHelper.Deserialize (operationResponse.Parameters[(byte)LilyOpKey.UserProps] as Hashtable) as Props;
				
				if(User.Singleton.VipProps != null && User.Singleton.VipProps.Exists (r=>r.Id==newProps.Id))
				{
					Props oldProp=User.Singleton.VipProps.Find(r=>r.Id==newProps.Id);
					oldProp=newProps;
				}
				else
				{
					if (User.Singleton.VipProps == null)
						User.Singleton.VipProps = new List<Props>();
					User.Singleton.VipProps.Add(newProps);
				}
				if (buyPropFinishedEvent!=null)
				{
					QueryUserById(User.Singleton.UserData.UserId);
					buyPropFinishedEvent(newProps);
				}
				break;
			}
			}
			
			if(operationResponse.Parameters.ContainsKey((byte)LilyOpKey.Chip)){
				User.Singleton.UserData.Chips=(long)operationResponse.Parameters[(byte)LilyOpKey.Chip];
				User.Singleton.UserInfoChanged();
			}
			
			if (buyItemResponseEvent!=null)
			{
				buyItemResponseEvent(true, Shop.Singleton.TempItemType);
				QueryUserById(User.Singleton.UserData.UserId);
			}
//			Debug.Log("OwnRoomTypes:"+User.Singleton.UserData.OwnRoomTypes);
			Shop.Singleton.RemoveCurrentIAP();
		}
		
		public void SendChipResponse(OperationResponse operationResponse)
		{
		    //User.Singleton.UserData.Chips-=(long)tempObj;
			//User.Singleton.UserInfoChanged();
			DidSendChipMessageFinishEvent();
		}
		
		public void StandupResponse(OperationResponse operationResponse)
		{
			int Noseat=User.Singleton.UserData.NoSeat;
			User.Singleton.UserData.NoSeat=-1;
 			DidStandUpEventFinished(Noseat);
			
		}
		
		public void GetClientVersionResponse(OperationResponse operationResponse)
		{
			string clientVersion=operationResponse.Parameters[(byte)LilyOpKey.ClientVersion] as string;
			//GlobalManager.Singleton.CurrentVersion = clientVersion;
			//User.Singleton.GameStatus = GameStatus.Connected;
			if(this.CurrentAction!=null)
			{
				this.CurrentAction();
			}
			this.CurrentAction=null;
			GlobalManager.Singleton.Notifications.Enqueue(new CheckUpdateNotification(clientVersion));
			GlobalManager.Log(clientVersion);
		}
		#endregion
		#endregion
		
		#region event Function
		void SearchUserDidFinished()
		{
			if (searchUserFinished != null)
			{
				searchUserFinished();
			}
		}
		#endregion
		
		#region event Function
		void JoinGameDidFinished(bool iswork,TypeState gameState)
		{
			if (joinGameFinished != null)
			{
				joinGameFinished(iswork,gameState);
			}
		}
		#endregion
		
		#region
		void DidJoinGameEventFinished(ErrorCode error,TypeState gameState)
		{
			if(joinGameEventFinished!=null)
			{
 				joinGameEventFinished(error,gameState);
			}
			 
		}
		#endregion
 
		#region event Function
		
		void DidBetTurnEndedEvent()
		{
			if(betTurnEndedEvent!=null)
			{
				betTurnEndedEvent();
			}
		}
		
		#endregion
		
		#region event Function
		
		void DidStartGameEvent()
		{
			if(startGameEvent!=null)
			{
				startGameEvent();
			}
		}
		
		#endregion
		
		#region event Function
		
		void DidEndGameEvent(long taxAnimt)
		{
			if(endGameEvent!=null)
			{
				endGameEvent(taxAnimt);
			}
		}
		
		#endregion
		
		#region event Function
		
		void DidBetTurnStartEvent()
		{
			if(betTurnStartEvent!=null)
			{
				betTurnStartEvent();
			}
		}
		
		#endregion
		
		#region event Function
		void DidPlayerWonPotEvent(int potId, long wonmoney,int noseat)
		{
			if(playerWonPotEvent!=null)
				playerWonPotEvent(potId,wonmoney,noseat);
		}
		
		#endregion
		
		#region event Function
		void DidToPlayerHoleCardsChangedEvent(PlayerInfo playinfo)
		{
			if(toPlayerHoleCardsChangedEvent!=null)
			{
				toPlayerHoleCardsChangedEvent(playinfo);
			}
		}
		
		#endregion
		
		#region event Function
		void DidProcessPlayerTurnBeganEvent(int lastNoseat,bool isClient)
		{
			if(playerTurnBeganEvent!=null)
			{
				playerTurnBeganEvent(lastNoseat,isClient);
			}
		}
		
		#endregion  
		
		#region event Function
		void DidProssPlayerTurnEndedEvent(int noseat,bool isClient)
		{
			if(playerTurnEndedEvent!=null)
			{
				playerTurnEndedEvent(noseat,isClient);
			}
		}
		
		#endregion
		
		#region
		void DidPlayerJoinedEvent(int noseat)
		{
			if(playerJoinedEvent!=null)
			{
				playerJoinedEvent(noseat);
			}
		}
		#endregion
		
		#region
		void DidPlayerLeavedEvent(int noseat,bool isKick,PlayerLeaveType leavetype)
		{
			if(playerLeavedEvent!=null)
			{
				playerLeavedEvent(noseat,isKick,leavetype);
			}
		}
		
		#endregion
		
		#region 
		void DidJoinRoomEventFinished(UserData userdata)
		{
			if(joinRoomEventFinished!=null)
			{
				joinRoomEventFinished(userdata);
			}
		}
			#endregion
		
		#region 
		void DidLeaveRoomEventFinished(UserData userdata)
		{
			if(leaveRoomEventFinished!=null)
			{
				leaveRoomEventFinished(userdata);
			}
		}
		
		#endregion
		
		#region
		void DidStandUpEventFinished(int Noseat)
		{
			if(standUpEvent!=null)
			{
				standUpEvent(Noseat);
			}
		}
		
		#endregion
		
		
		#region
		void DidSendGiftEvent(int Sendernoseat,int resivedNoseat,int giftid)
		{
			if(sendGiftEvent!=null)
			{
				sendGiftEvent(Sendernoseat,resivedNoseat,giftid);
			}
		}
		
         #endregion
		
		#region
		void DidBroadcastMessageInTableEvent(int noSeat,string message)
		{
		
			if(broadcastMessageInTableEvent!=null)
			{
				broadcastMessageInTableEvent(noSeat,message);
			}
		}
		#endregion
		
		
		#region
		void DidQueryUserEvent(UserData userdata)
		{
			if(queryUserEvent!=null)
			{
				queryUserEvent(userdata);
			}
		}
		
		#endregion
		
	   #region
	     void DidSitDownEvent(TypeState gamestate)
		{
		     if(sitDownEvent!=null)
			{
				sitDownEvent(gamestate);
			}
		}
		
	   #endregion	
		
	  #region
		
	  void DidJoinRoomEvent()
	  {
			if(joinRoomEvent!=null)
			{
				joinRoomEvent();
			}
		}
		 
	 
		#endregion
	
		#region
		void DidTryJointGameEvent(bool flag)
		{
			if(tryJointGameEvent!=null)
			{
				tryJointGameEvent(flag);
			}
		}
		#endregion
	 
		#region
		void DidInRoomPlayerLeavedEvent()
		{
			if(inRoomPlayerLeavedEvent!=null)
			{
				inRoomPlayerLeavedEvent();
			}
		}
		#endregion	
		
		//DidPlayerWonPotImproveEvent(potId,winner,winamnt,attachedplayer);
		#region
		void DidPlayerWonPotImproveEvent(int potId,int[] winner,long[] winamnt,int[] attachedplayer)
		{
				if(playerWonPotImproveEvent!=null)
				{
					playerWonPotImproveEvent(potId,winner,winamnt,attachedplayer);
				}
		}
		#endregion
		
		#region
		void DidPlayersShowCardsEvent(Dictionary<int,string> players)
		{
				if(didPlayersShowCardsFinished!=null)
				{
					didPlayersShowCardsFinished(players);
				}
		}
		#endregion
		
		#region
		void DidSyncGameDataTableInfoEvent(bool iswork)
		{
			if(syncGameDataTableInfoEvent!=null)
			{
				syncGameDataTableInfoEvent(iswork);
			}
		}
		#endregion
		
		void DidLeaveGameFinished()
		{
			if(leaveGameFinished!=null)
			{
				leaveGameFinished();
			}
		}
		#region
		void DidClickFriendInfoEvent()
		{
			if(clickFriendInfoEvent!=null)
				clickFriendInfoEvent();
		}
		#endregion
		#region
		void DidLoginUserNameOrPasswordErrorEvent()
		{
			if(loginUserNameOrPasswordErrorEvent != null)
			{
				loginUserNameOrPasswordErrorEvent();
				loginUserNameOrPasswordErrorEvent = null;
			}
		}
		#endregion
		#region
		void DidSendChipMessageFinishEvent()
		{
			if(sendChipMessageFinishEvent!=null)
			{
				sendChipMessageFinishEvent();
				sendChipMessageFinishEvent = null;
			}
		}
		#endregion
	}
	 
}
