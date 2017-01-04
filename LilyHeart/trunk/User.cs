using System;
using DataPersist;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace LilyHeart
{
	public enum GameStatus:byte
	{
		Offline=1,
		Connected,
		JoinRoom,
		InRoom,
		InGame,
		Sit,
		InSetting,
		InHelp,
		Error,
		NoStatus,
		Logout
	}
	
	public enum PlayerAvator:byte
	{
		Guest,
		DaHeng = 1,
		Songer,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		General,//10
		Princess,
		Queen
	}
	
	public abstract class User 
	{
		public static bool bShowFriend_Flag = false;
		public static bool bShowGameSetting_Flag = false;
		public static bool bBuyThingsInIos = false;
		
		protected static User user;
		
		public static Action userDataChangedEvent;
		public static event Action UserDataChangedEvent {add {userDataChangedEvent+=value;} remove { userDataChangedEvent-=value;}}
		
		public static User Singleton {
			get 
			{
				return user;
			}
		}
		
		#region Member Properties
		private UserData userData = null;
		public UserData UserData 
		{
			get
			{
				return userData;
			}
			set
			{
				if (value != null)
				{
					userData = value;
					UserInfoChanged();
				}
			}
		}
		public List<UserData> Friends {get;set;}
		public GameStatus GameStatus {get;set;}
		public Message CurrentMessage{ get; set; }
		
		public bool MessageOperating{get;set;}
		
		public bool PlayInitAnimation{get;set;}
		
		public int CurrentPlayInfo{get;set;}
		
		public bool guestInfoDialog = false;
		
		public bool MaskingTableOpened = false;
		
		public List<Props> Avators{get;set;}
		
		public List<Props> VipProps{get;set;}
		
		
		public bool canGetOnlineAwards{get;set;}
		
		public bool AppScore{get;set;}
		
		#endregion
		
		#region Member Method
		public void UserInfoChanged()
		{
			if (userDataChangedEvent != null)
				userDataChangedEvent();
		}
		
		public User ()
		{
			UserData=new UserData();
			Friends=new List<UserData>();
			Avators=new List<Props>();
			VipProps=new List<Props>();
			GameStatus=GameStatus.Offline;
			PlayInitAnimation = true;
		}
		
		public abstract void Register(string nickname,string mail,string password, string deviceToken, byte avator);
		
		public void Register(string nickname,string mail,string password, string deviceToken, byte avator,UserType type)
		{
			this.UserData.NickName=nickname;
			this.UserData.Mail=mail;
			this.UserData.Password=password;
			this.UserData.UserType=type;
			this.UserData.Avator=avator;
			
			DeviceTokenAssign(deviceToken);
			
			if(PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.Register();
			}
			else
			{
				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.Register;
				PhotonClient.Singleton.Connect();
			}
		}
		
		public abstract void Login(string username,string password, string deviceToken);
		public void Login(string username,string deviceToken)
		{
			Login(username,string.Empty,deviceToken);
		}
		
		public void DeviceTokenAssign(string deviceToken)
		{
			this.UserData.DeviceToken = deviceToken;
			
			if( Application.platform == RuntimePlatform.IPhonePlayer)
			{
				this.UserData.DeviceType = DataPersist.DeviceType.IOS;
			}
		}
		
		public abstract void GuestUpgrade(string nickname,string mail,string password, string deviceToken, byte avator);
		
		public void GuestUpgrade(string nickname,string mail,string password, string deviceToken, byte avator,UserType userType)
		{
			this.UserData.NickName=nickname;
			this.UserData.Mail=mail;
			this.UserData.Password=password;
			this.UserData.UserType=userType;
			this.UserData.Avator = avator;
			
			DeviceTokenAssign(deviceToken);
			
			if(PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.GuestUpgrade();
			}
			else
			{
				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.GuestUpgrade;
				PhotonClient.Singleton.Connect();
			}
		}
		
		public void LoginAsGuest(string userName, string deviceToken)
		{
			this.UserData.NickName=string.Empty;
			this.UserData.Mail = userName;
			this.UserData.Password = string.Empty;
			this.UserData.UserType = UserType.Guest;
			
			DeviceTokenAssign(deviceToken);
			
			if(PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.Login(this.UserData);
			}
			else
			{
				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.Login;
				PhotonClient.Singleton.Connect();
			}
		}
		
		public void GetPassword(string mail)
		{
			PhotonClient.Singleton.FindPassword(mail);
		}
		
		public void JoinRoom(string roomId)
		{
//			this.GameStatus=GameStatus.JoinRoom;
//			Room.Singleton.RoomData=new RoomData{
//				RoomId=roomId
//			};
//			PhotonClient.Singleton.LeaveRoom();
			PhotonClient.Singleton.JoinRoom(roomId);
		}
		
		
//		public void AcceptInvite(string destinationId){
//			PhotonClient.Singleton.AcceptInvite(destinationId);
//		}
		
		public void Logout()
		{
			PhotonClient.Singleton.Logout();
		}
		
		public void RequestFriend(string userId)
		{
			PhotonClient.Singleton.RequestFriend(userId);
		}
		
		public void AcceptFriend(string userId)
		{
			PhotonClient.Singleton.AcceptFriend(userId);
		}
		
		public void AddFriend(string userId)
		{
			PhotonClient.Singleton.AddFriend(userId);
		}
		
		public void DeleteFriend(string userId)
		{
			PhotonClient.Singleton.DeleteFriend(userId);
		}
		
		public void InviteFriendToRoom(string userId)
		{
			PhotonClient.Singleton.InviteFriend(userId,InviteFriendMessage.DESTINATION_ROOM);
		}
		
		public void InviteFriendToGame(string userId)
		{
			PhotonClient.Singleton.InviteFriend(userId,InviteFriendMessage.DESTINATION_GAME);
		}
		
		public void SearchUser(string nickname)
		{
			PhotonClient.Singleton.SearchUser(nickname);
		}
		
		public void AccessFriend(string userId)
		{
			PhotonClient.Singleton.AccessFriend(userId);
		}
		
		public void FriendList()
		{
			PhotonClient.Singleton.FriendList();
		}
		
		public void QuickStart()
		{
			PhotonClient.Singleton.QuickStart();
		}
		
		public void BroadcastMessage(string message)
		{
			PhotonClient.Singleton.BroadcastMessage(message);
		}
		
		public void BroadcastMessageInTable(string message)
		{
			PhotonClient.Singleton.BroadcastMessageInTable(message);
		}
		
		public void Sit(byte noseat,long chips)
		{
			this.UserData.NoSeat=noseat;
			PhotonClient.Singleton.Sit(chips);
		}
		
		public void JoinGame()
		{
			PhotonClient.Singleton.JoinGame();
		}
		
		public void JoinGame(int bigBlind,int maxPlayers,int thinkingTime,bool onlyFriend)
		{
			Room.Singleton.OnlyFriend=onlyFriend;
			PhotonClient.Singleton.JoinGame(bigBlind,maxPlayers,thinkingTime,onlyFriend);
		}
		
		public void LeaveGame()
		{
			PhotonClient.Singleton.LeaveGame();
		}
		
		public void StandUp(){
			
			PhotonClient.Singleton.Standup ();
		}
		
		public void Save(byte avator,string password,RoomType roomType)
		{
			User.Singleton.UserData.Avator=avator;
			User.Singleton.UserData.Password=password;
			User.Singleton.UserData.LivingRoomType=roomType;
			PhotonClient.Singleton.Save();
		}
		
		public void Fold()
		{
			PhotonClient.Singleton.Fold();
		}
		
		public void Call()
		{
			PhotonClient.Singleton.Call();
		}
		public void Check()
		{
			PhotonClient.Singleton.Check();
		}
		
		public void Raise(long raiseMoney)
		{
			PhotonClient.Singleton.Raise(raiseMoney);
		}
		
		public List<UserData> SearchFriends(string nickname)
		{
			List<UserData> friends=new List<UserData>();
			if(this.Friends!=null)
			{
				foreach(UserData friend in this.Friends)
				{
					if(friend.NickName.Contains(nickname))
					{
						friends.Add(friend);
					}
				}
			}
			return friends;
		}
		
		public void SendChip(string userId,long chip)
		{
			PhotonClient.Singleton.SendChip(userId,chip);
		}
		
		public void SendGift(int giftId)
		{
			PhotonClient.Singleton.SendGift(giftId);
		}
		
		public void SendGift(int noseat,int giftId)
		{
			PhotonClient.Singleton.SendGift(noseat,giftId);
		}
		
		public void QueryUserById(string userId)
		{
			PhotonClient.Singleton.QueryUserById(userId);
		}
		
		public void Setting(){
			PhotonClient.Singleton.SystemSetting ();			
		}
		
		public void Suspend(){
			
			PhotonClient.Singleton.ChangeUserStatus (UserStatus.Suspend);
		}
		
		public void ActivedToIdle()
		{
			PhotonClient.Singleton.ChangeUserStatus (UserStatus.Idle);
		}
		
		public void ActivedToPlaying()
		{
			PhotonClient.Singleton.ChangeUserStatus (UserStatus.Playing);
		}
		
		public void GetAward()
		{
			PhotonClient.Singleton.GetAward();
		}
		
		public void CheckEmail(string email)
		{
			if(PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.CheckEmail(email);
			}
			else
			{
				PhotonClient.Singleton.CurrentAction=()=>PhotonClient.Singleton.CheckEmail(email);
			}
		}
		#endregion
	}
}

