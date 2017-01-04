using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace AssemblyCSharp
{
	public static class FlurryHelper
	{
		
		public static void StartSession()
		{
#if UNITY_IPHONE
			if(Application.platform==RuntimePlatform.IPhonePlayer)
			{
				FlurryIOSBinding.startSession(StatisticsHelper.FLURRY_IOS_APIKEY);
			}
#endif
#if UNITY_ANDROID
		if(Application.platform==RuntimePlatform.Android)
			{
				FlurryAndroidBinding.startSession(StatisticsHelper.FLURRY_ANDROID_APIKEY);
			}
#endif
		}
		
		public static void LogEvent(string eventName)
		{
#if UNITY_IPHONE
			if(Application.platform==RuntimePlatform.IPhonePlayer)
			{
				FlurryIOSBinding.logEvent(eventName);
			}
#endif
#if UNITY_ANDROID
				FlurryAndroidBinding.logEvent(eventName);
#endif
		}
		
		public static void LogEvent(string eventName,Dictionary<string,string> parameters)
		{
			if(Application.platform==RuntimePlatform.IPhonePlayer||Application.platform==RuntimePlatform.Android)
			{
				string[] keys=new string[parameters.Count];
				string[] values=new string[parameters.Count];
				parameters.Keys.CopyTo(keys,0);
				parameters.Values.CopyTo(values,0);
#if UNITY_IPHONE
				if(Application.platform==RuntimePlatform.IPhonePlayer)
				{
					FlurryIOSBinding.logEventWithParameters(eventName,keys,values,parameters.Count);
				}
#endif
#if UNITY_ANDROID
					FlurryAndroidBinding.logEventWithParameters(eventName,keys,values);
#endif
			}
		}
		
		public static void LogCallInWorldEvent()
		{
			Dictionary<string,string> parameters=new Dictionary<string, string>();
			parameters[StatisticsHelper.PARAMETER_KEY_TIME]=DateTime.Now.ToString();
			LogEvent(StatisticsHelper.CALL_IN_WORLD,parameters);
		}
		
		public static void LogCallInTableEvent()
		{
			LogCallInTableEvent(StatisticsHelper.PARAMETER_VALUE_CUSTOM);
		}
		
		public static void LogCallInTableEvent(string message)
		{
			Dictionary<string,string> parameters=new Dictionary<string, string>();
			parameters[StatisticsHelper.PARAMETER_KEY_TIME]=DateTime.Now.ToString();
			parameters[StatisticsHelper.PARAMETER_KEY_WORD]=message;
			LogEvent(StatisticsHelper.CALL_IN_TABLE,parameters);
		}

		public static void logError(string logString,string stackTrace)
		{
#if UNITY_IPHONE
			if(Application.platform==RuntimePlatform.IPhonePlayer)
			{
				FlurryIOSBinding.logError(logString,stackTrace);
			}
#endif
#if UNITY_ANDROID
				FlurryAndroidBinding.logError(logString,stackTrace);
#endif
		}
		
		public static void endSession()
		{
#if UNITY_ANDROID
			FlurryAndroidBinding.endSession();
#endif
		}
	}
}

