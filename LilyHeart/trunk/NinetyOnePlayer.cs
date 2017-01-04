using System;
using DataPersist;

namespace LilyHeart
{
	public class NinetyOnePlayer:User
	{
		private NinetyOnePlayer ()
		{
		}
		
		#region Action
		public static void Init ()
		{
			user=new NinetyOnePlayer();
			user.MessageOperating = false;
		}
		
		public override void Login (string uid, string password, string deviceToken)
		{		
			GlobalManager.Log("Login In Player");
			
			DeviceTokenAssign(deviceToken);
			
			UserData temp = new UserData ();
			temp.Mail = uid;
			temp.Password = string.Empty;
			temp.DeviceType = this.UserData.DeviceType;
			temp.DeviceToken = deviceToken;
			temp.UserType = UserType.NinetyOne;
			
			
			if(PhotonClient.Singleton.IsConnected)
			{
				PhotonClient.Singleton.Login(temp);
			}
			else
			{
				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.Login;
				PhotonClient.Singleton.Connect();
			}
		}
		
		public override void Register(string nickname,string mail,string password, string deviceToken, byte avator)
		{
			Register(nickname,mail,string.Empty,deviceToken,avator,UserType.NinetyOne);
		}
		
		public override void GuestUpgrade(string nickname,string mail,string password, string deviceToken, byte avator)
		{
			GuestUpgrade(nickname,mail,string.Empty,deviceToken,avator,UserType.NinetyOne);
		}
		#endregion
	}
}

