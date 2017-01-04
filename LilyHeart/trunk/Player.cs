using System;
using DataPersist;
using UnityEngine;
using System.Collections.Generic;
using System.Timers;

namespace LilyHeart
{
	public class Player:User
	{	
//		public static User Instance
//		{
//			get
//			{
//				if (user == null)
//				{
//					user = new Player();
//					user.MessageOperating = false;
//				}
//				
//				return user;
//			}
//		}
//		
		private Player ()
		{
		}
		
		#region Action
		public static void Init ()
		{
			user=new Player();
			user.MessageOperating = false;
		}
		
		public override void Login (string mail, string password, string deviceToken)
		{		
			GlobalManager.Log("Login In Player");
			
			DeviceTokenAssign(deviceToken);
			
			UserData temp = new UserData ();
			temp.Mail = mail;
			temp.Password = password;
			temp.DeviceType = this.UserData.DeviceType;
			temp.DeviceToken = deviceToken;
			temp.UserType = UserType.Normal;
			
			
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
		
		public override void GuestUpgrade(string nickname,string mail,string password, string deviceToken, byte avator)
		{
			GuestUpgrade(nickname,mail,password,deviceToken,avator,UserType.Normal);
		}
		
		public override void Register(string nickname,string mail,string password, string deviceToken, byte avator)
		{
//			this.UserData.NickName=nickname;
//			this.UserData.Mail=mail;
//			this.UserData.Password=password;
//			this.UserData.UserType=UserType.Normal;
//			this.UserData.Avator = avator;
//			
//			DeviceTokenAssign(deviceToken);
//			
//			if(PhotonClient.Singleton.IsConnected)
//			{
//				PhotonClient.Singleton.Register();
//			}
//			else
//			{
//				PhotonClient.Singleton.CurrentAction=PhotonClient.Singleton.Register;
//				PhotonClient.Singleton.Connect();
//			}
			Register(nickname,mail,password,deviceToken,avator,UserType.Normal);
		}
		#endregion
	}
}

