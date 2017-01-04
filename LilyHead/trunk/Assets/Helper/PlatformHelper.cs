using System;
using AssemblyCSharp.Helper;
using UnityEngine;
using LilyHeart;
using DataPersist;

namespace AssemblyCSharp
{
	public static class PlatformHelper
	{
		private const int NINETYONE_APPID=103667;
		private const string NINETYONE_APPKEY="384cb2d0a5401ff9ceca8134c9043f69e1582124cc0b60d4";
		
		public static void StartSession()
		{
			if(GlobalManager.Singleton.ApplicationType==AppType.NinetyOne)
			{
				NdComHelper.StartSession(NINETYONE_APPID,NINETYONE_APPKEY);
			}
		}
		
		public static void Login(string gameObjectName,string methodName)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				WebBindingHelper.ShowLoginWebView(gameObjectName,methodName);
				break;
			case AppType.NinetyOne:
				NdComHelper.Login(gameObjectName,methodName);
				break;
			}
		}
		
		public static void OnLogin(string values)
		{
			string[] arr;
			string email=string.Empty;
			string password=string.Empty;
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				arr=values.Split(',');
				email=arr[0];
				password=arr[1];
				break;
			case AppType.NinetyOne:
				NdComHelper.InitCancelMetodInLogin(GlobalScript.Singleton.name,"EmptyOnLogin");
				email=values;
				GlobalManager.Singleton.ParamEmail=email;
				break;
			}
			//GameObject.Find("UI Root (2D)/Camera/Anchor").AddComponent<ShowLoadingTable>();
			User.Singleton.Login(email, password.getMD5(), DeviceTokenHelper.myDeviceToken);
			//Playtomic.Log.Play();
		}
		
		public static void Register(string gameObjectName,string methodName)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				WebBindingHelper.ShowRegisterWebView(gameObjectName,methodName);
				break;
			}
		}
		
		public static void Upgrade(string gameObejctName,string methodName)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				WebBindingHelper.ShowRegisterWebView(gameObejctName,methodName);
				break;
			case AppType.NinetyOne:
				NdComHelper.GuestRegist(gameObejctName,methodName);
				break;
			}
		}
		
		public static void OnUpgrade(string values)
		{
			string[] param;
		
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				param=values.Split(',');
				GlobalManager.Singleton.ParamEmail=param[0];
				GlobalManager.Singleton.ParamPassword=param[1];
				User.Singleton.CheckEmail(GlobalManager.Singleton.ParamEmail);
				break;
			case AppType.NinetyOne:
				User.Singleton.GuestUpgrade(User.Singleton.UserData.NickName, 
					values, 
					string.Empty, 
					DeviceTokenHelper.myDeviceToken, 
					User.Singleton.UserData.Avator);
				GlobalManager.Singleton.ParamEmail=string.Empty;
				GlobalManager.Singleton.ParamPassword=string.Empty;
				break;
			}
		}
		
		public static void Logout()
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.NinetyOne:
				NdComHelper.Logout();
				break;
			}
		}
		
		public static void LoginAsGuest(string gameObejctName,string methodName,string methodName2)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.NinetyOne:
				NdComHelper.LoginEx(gameObejctName,methodName,methodName2);
				break;
			}
		}
		public static bool CanAutoLogin()
		{
			bool result=true;
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				result=UtilityHelper.CanAutoLogin();
				break;
			case AppType.NinetyOne:
				if(User.Singleton!=null&&User.Singleton.UserData.UserType==UserType.Guest)
				{
					result=true;
				}
				else
				{
					result=NdComHelper.CanAutoLogin();
				}
				break;
			}
			return result;
		}
		
		public static void AutoLogin()
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				UtilityHelper.AutoLogin();
				break;
			case AppType.NinetyOne:
				NdComHelper.InitCancelMetodInLogin(GlobalScript.Singleton.name,"OnCancelLogin");
				NdComHelper.LoginEx(GlobalScript.Singleton.name,"OnLoginAsGuest","OnLogin");
				break;
			}
		}
		public static void ChangePassword(GameObject gameObj)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				SetupDialog setup = gameObj.GetComponent<SetupDialog>();
				setup.onChangePassword();
				break;
			case AppType.NinetyOne:
				NdComHelper.ChangePassword();
				break;
			}
		}
		
		public static void CheckUpdate(string gameObjectName,string passedMethodName,string failedMethodName)
		{
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				GlobalManager.Singleton.GetClientVersion();
				break;
			case AppType.NinetyOne:
				NdComHelper.CheckUpdate(gameObjectName,passedMethodName,failedMethodName);
				break;
			}
		}
	}
}

