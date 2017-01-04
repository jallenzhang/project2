using System;

namespace AssemblyCSharp
{
	public static class NdComHelper
	{	
		public static void StartSession(int appId,string appKey)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndStartSession(appId,appKey);
#endif
		}
		
		public static void Login(string gameObjectName,string methodName)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndLogin(0,gameObjectName,methodName);
#endif
		}
		
		public static void Logout()
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndLogout(1);
#endif
		}
		
		public static void LoginEx(string gameObjectName,string methodName,string methodName2)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndLoginEx(0,gameObjectName,methodName,methodName2);
#endif
		}
		
		public static void GuestRegist(string gameObjectName,string methodName)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndGuestRegist(0,gameObjectName,methodName);
#endif
		}
		
		public static bool CanAutoLogin()
		{
#if UNITY_IPHONE
			int canAutoLogin=0;
			NdComIOSBinding.ndCanAutoLogin(ref canAutoLogin);
			return canAutoLogin!=0;
#endif
			return true;
		}
		
		public static void ChangePassword()
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndChangePassword();
#endif
		}
		public static void  InitCancelMetodInLogin(string cancelGameObjectName,string cancelMethodName)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndInitCancelMetodInLogin(cancelGameObjectName,cancelMethodName);
#endif	
		}
		
		public static void CheckUpdate(string gameObjectName,string passedMethodName,string failedMethodName)
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndCheckUpdate(gameObjectName,passedMethodName,failedMethodName);
#endif
		}
		
		public static void  Enter91Platform()
		{
#if UNITY_IPHONE
			NdComIOSBinding.ndLoginPlatform();
#endif	
		}
	}
}

