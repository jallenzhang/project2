using System;
using UnityEngine;

namespace AssemblyCSharp
{
#if UNITY_ANDROID
	public static class FlurryAndroidBinding
	{	
		private const string FLURRY_BINDING_CLASS="com.toufe.lilypoker.FlurryBinding";
		static FlurryAndroidBinding()
		{
		}
		
		public static void startSession(string apiKey)
		{
			using(AndroidJavaClass cls=new AndroidJavaClass(FLURRY_BINDING_CLASS))
			{
				cls.CallStatic("startSession",apiKey);
			}
		}
		
		public static void logEvent(string eventName)
		{
			using(AndroidJavaClass cls=new AndroidJavaClass(FLURRY_BINDING_CLASS))
			{
				cls.CallStatic("logEvent",eventName);
			}
		}
		
		public static void logEventWithParameters(string eventName,string[] keys,string[] values)
		{
			using(AndroidJavaClass cls=new AndroidJavaClass(FLURRY_BINDING_CLASS))
			{
				cls.CallStatic("logEventWithParameters",eventName,keys,values);
			}
		}
		
		public static void logError(string logString,string stackTrace)
		{
			using(AndroidJavaClass cls=new AndroidJavaClass(FLURRY_BINDING_CLASS))
			{
				cls.CallStatic("logError",logString,stackTrace);
			}
		}
		
		public static void endSession()
		{
			using(AndroidJavaClass cls=new AndroidJavaClass(FLURRY_BINDING_CLASS))
			{
				cls.CallStatic("endSession");
			}
		}
	}
#endif
}

