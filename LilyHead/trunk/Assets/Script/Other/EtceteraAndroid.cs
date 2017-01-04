using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;

#if UNITY_ANDROID

public class EtceteraAndroid {
	
	private static AndroidJavaObject _plugin;
	
	static EtceteraAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        
		_plugin = jc.GetStatic<AndroidJavaObject>("currentActivity");
	}
	
	public static void showAlert(string title, string message, string positiveButton)
	{
		showAlert(title, message, positiveButton, string.Empty);
	}
	
	public static void showAlert(string title, string message, string positiveButton, string negativeButton)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		_plugin.Call("showAlert", title, message, positiveButton, negativeButton);
	}
	
	public static void StartBuy(string subject, string body, string price)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		string functionNameToCall = "StartBuy";
		string unityFuntionName = "purcharseFinished";
		if(GlobalScript.ScriptSingleton.UnityPayWay == DataPersist.PayWay.Yeepay){
			functionNameToCall = "StartBuyByYeepay";
			unityFuntionName = "purcharseYeepayFinished";
		}
		_plugin.Call(functionNameToCall, subject, body, price, User.Singleton.UserData.UserId + "_" + MyVersion.ChannelId.Replace('.','_'), "UI Root (2D)_FrontButton", unityFuntionName);	
	}
	
	public static void showWebView(string webUrl)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		_plugin.Call("showWebPage", webUrl);
	}
	
	public static void showWebView(string webUrl,string gameObjectName,string methodName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		_plugin.Call("showWebView", webUrl,gameObjectName,methodName);
	}
	
	public static void showWebView(string webUrl,string gameObjectName,string methodName, string pvalue, bool isPortrait)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		_plugin.Call("showWebView", webUrl,gameObjectName,methodName, pvalue, isPortrait);
	}
	
	public static void excuteJS(string foo)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		
		_plugin.Call("ExcuteJS",foo);
	}
	
	public static void finishCurrentActivity()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		_plugin.Call("finish");
	}
	
	public static void CloseWebView()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		_plugin.Call("CloseWebView");
	}
    public static void setReciveDeviceInfo(string gameObjectName,string methodNameDeviceType,string methodNameOSVersion,string methodNameDeviceToken)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		_plugin.Call("setReciveDeviceInfo",gameObjectName,methodNameDeviceType,methodNameOSVersion,methodNameDeviceToken);
	}
}

#endif
