using System;
using System.Runtime.InteropServices;

public static class NdComIOSBinding
{
	[DllImport("__Internal")]
	public static extern void ndStartSession(int appId,string appKey);
	
	[DllImport("__Internal")]
	public static extern void ndLogin(int flag,string gameObjectName,string methodName);
	
	[DllImport("__Internal")]
	public static extern void ndLoginEx(int flag,string gameObjectName,string methodName,string methodName2);
	
	[DllImport("__Internal")]
	public static extern void ndGuestRegist(int flag,string gameObjectName,string methodName);

	[DllImport("__Internal")]
	public static extern void ndLogout(int flag);
	
	[DllImport("__Internal")]
	public static extern void ndCanAutoLogin(ref int canAutoLogin);
	
	[DllImport("__Internal")]
	public static extern void ndChangePassword();
	
	[DllImport("__Internal")]
	public static extern void ndInitCancelMetodInLogin(string cancelGameObjectName,string cannelMethodName);
	
	[DllImport("__Internal")]
	public static extern void ndCheckUpdate(string gameObjectName,string passedMethodName,string failedMethodName);
		
	[DllImport("__Internal")]
	public static extern void ndLoginPlatform();
}