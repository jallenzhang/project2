using System;
using UnityEngine;

#if UNITY_ANDROID
namespace AssemblyCSharp
{
	public static class BaiduAndroidBinding
	{
		private const string BINDING_CLASS="com.toufe.lilypoker.BaiduBinding";
		
		public static void startSession(string appId,string channelId)
		{
#if UNITY_ANDROID
			using(AndroidJavaClass cls=new AndroidJavaClass(BINDING_CLASS))
			{
				cls.CallStatic("startSession",appId,channelId);
			}
#endif
		}
		
		public static void logEventWithParameters(string eventId,string eventLabel)
		{
#if UNITY_ANDROID
			using(AndroidJavaClass cls=new AndroidJavaClass(BINDING_CLASS))
			{
				cls.CallStatic("logEventWithParameters",eventId,eventLabel);
			}
#endif
		}
	}
}
#endif

