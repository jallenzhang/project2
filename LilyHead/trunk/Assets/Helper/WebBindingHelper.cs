using System;
using UnityEngine;
using LilyHeart;

namespace AssemblyCSharp
{
	public class WebBindingHelper
	{
		private static bool visible;
		public WebBindingHelper ()
		{
		}
		
		public static void CloseWebView()
		{
			if(visible)
			{
#if UNITY_IPHONE
			EtceteraBinding.dismissWrappedController();
#endif
#if UNITY_ANDROID
			EtceteraAndroid.CloseWebView();
#endif
				visible=false;
			}
		}
		
		public static void ShowRegisterWebView(string gameObjectName,string methodName)
		{
#if UNITY_IPHONE
		EtceteraBinding.ShowWebView("file:///"+Application.streamingAssetsPath+"/Register_ios.html",gameObjectName,methodName);
#endif
#if UNITY_ANDROID
		EtceteraAndroid.showWebView("file:///android_asset/Register_android.html",gameObjectName,methodName);
#endif
			visible=true;
		}
		
		public static void ShowLoginWebView(string gameObjectName,string methodName)
		{
#if UNITY_IPHONE
			EtceteraBinding.ShowWebView("file:///"+Application.streamingAssetsPath+"/Login_ios.html",gameObjectName,methodName);
#endif
#if UNITY_ANDROID
			EtceteraAndroid.showWebView("file:///android_asset/Login_android.html",gameObjectName,methodName);
#endif
			visible=true;
		}
		
		public static void ShowPaySwitchWebView(string gameObjectName, string methodName)
		{	
			string myNickName = User.Singleton.UserData.NickName;
#if UNITY_IPHONE
			// do nothing
#endif
#if UNITY_ANDROID
			EtceteraAndroid.showWebView("file:///android_asset/PaySwitch_android.html",gameObjectName,methodName, myNickName, true);
#endif
			visible=true;
		}
		
		public static void ShowForgetPwdWebView(string gameObjectName, string methodName)
		{	
#if UNITY_IPHONE  
			EtceteraBinding.ShowWebView("file:///"+Application.streamingAssetsPath+"/ForgetPwd_ios.html",gameObjectName,methodName);
#endif
#if UNITY_ANDROID
			EtceteraAndroid.showWebView("file:///android_asset/ForgetPwd_android.html",gameObjectName,methodName);
#endif
			visible=true;			
		}
		
		public static void ExcuteJS(string foo)
		{
			Debug.Log("In ExcuteJS");
			if(visible)
			{
#if UNITY_IPHONE
				EtceteraBinding.excuteJS(foo);
#endif
#if UNITY_ANDROID
				EtceteraAndroid.excuteJS(foo);
#endif
			}
		}
	}
}

