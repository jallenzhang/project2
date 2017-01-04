using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IPHONE
public static class FlurryIOSBinding{
	[DllImport("__Internal")]
	public static extern void startSession(string key);
	
	[DllImport("__Internal")]
	public static extern void logEvent(string eventName);
	
	[DllImport("__Internal")]
	public static extern void logEventWithParameters(string eventName,string[] keys,string[] values,int length);
	
	[DllImport("__Internal")]
	public static extern void logError(string logString,string stackTrace);
}
#endif