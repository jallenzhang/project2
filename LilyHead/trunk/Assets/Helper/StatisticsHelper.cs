using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public static class StatisticsHelper
	{
		public const string FLURRY_IOS_APIKEY="CA7TJ39I6NXBJYHSQLKS";
		public const string FLURRY_ANDROID_APIKEY="4SDJX34VFP44HJ397L5L";
		public const string BAIDU_IOS_APPID="c07f9519d1";
		public const string BAIDU_ANDROID_APPID="869d9632ff";
		
		public const string CALL_IN_WORLD="CallInWorld";
		public const string CALL_IN_TABLE="CallInTable";
		public const string PARAMETER_KEY_TIME="Time";
		public const string PARAMETER_KEY_WORD="Word";
		public const string PARAMETER_VALUE_CUSTOM="Custom";
		
		public static void StartSession()
		{
			FlurryHelper.StartSession();
#if UNITY_IPHONE
			if(Application.platform==RuntimePlatform.IPhonePlayer)
			{
				BaiduIOSBinding.bdStartSession(BAIDU_IOS_APPID,MyVersion.ChannelId);
			}
#endif
#if UNITY_ANDROID
			if(Application.platform==RuntimePlatform.Android)
			{
				BaiduAndroidBinding.startSession(BAIDU_ANDROID_APPID,MyVersion.ChannelId);
			}
#endif
		}
		
		public static void logError(string logString,string stackTrace)
		{
			FlurryHelper.logError(logString,stackTrace);
		}
		
	    public static void LogCallInWorldEvent()
		{
			FlurryHelper.LogCallInWorldEvent();
#if UNITY_IPHONE
			BaiduIOSBinding.bdLogEventWithParameters(CALL_IN_WORLD,DateTime.Now.ToString());
#endif
#if UNITY_ANDROID
			BaiduAndroidBinding.logEventWithParameters(CALL_IN_WORLD,DateTime.Now.ToString());
#endif
		}
		
		public static void LogCallInTableEvent()
		{
			FlurryHelper.LogCallInTableEvent();
#if UNITY_IPHONE
			BaiduIOSBinding.bdLogEventWithParameters(CALL_IN_TABLE,PARAMETER_VALUE_CUSTOM);
#endif
#if UNITY_ANDROID
			BaiduAndroidBinding.logEventWithParameters(CALL_IN_TABLE,PARAMETER_VALUE_CUSTOM);
#endif
		}
		
		public static void LogCallInTableEvent(string message)
		{
			FlurryHelper.LogCallInTableEvent(message);
#if UNITY_IPHONE
			BaiduIOSBinding.bdLogEventWithParameters(CALL_IN_TABLE,message);
#endif
#if UNITY_ANDROID
			BaiduAndroidBinding.logEventWithParameters(CALL_IN_TABLE,message);
#endif
		}
		
		public static void endSession()
		{
			FlurryHelper.endSession();
		}
	}
}

