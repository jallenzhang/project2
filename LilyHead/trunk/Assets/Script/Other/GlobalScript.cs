using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using LilyHeart;
using AssemblyCSharp.Helper;
using System.Collections.Generic;
using System.Threading;
using System;
using DataPersist;

public class GlobalScript : MonoBehaviour {
	private static GameObject singleton;
	public static float startTime;
	public static GameObject Singleton { get {return singleton;}}
	
	public AppType ApplicationType=AppType.Normal;
	public string ServerName ="10.0.1.8:4530";
	public string ApplicationName="Master";
	public Queue<DialogInfo> CurrentInfos { get; set; }
	public static GlobalScript ScriptSingleton {get; private set;} 
	
	public PayWay UnityPayWay = PayWay.Alipay;
	
	void Awake()
	{
		if(singleton==null)
		{
			CurrentInfos = new Queue<DialogInfo>();
			StatisticsHelper.StartSession();
			GlobalManager.Singleton.ApplicationType=ApplicationType;
#if UNITY_ANDROID
			GlobalManager.Singleton.ClientChannelId=MyVersion.ChannelId;
#endif
#if UNITY_IPHONE
			GlobalManager.Singleton.ClientChannelId=UtilityHelper.GetChannelId();
#endif
			singleton=gameObject;
			ScriptSingleton=this;
			DontDestroyOnLoad(gameObject);
			Application.RegisterLogCallback(HandleError);
#if UNITY_ANDROID
			Application.targetFrameRate=30;
#endif
			PlatformHelper.StartSession();
			PhotonClient.Singleton.ServerName=this.ServerName;
			PhotonClient.Singleton.ApplicaitonName=this.ApplicationName;
			PhotonClient.Singleton.AutoLogin=()=>{if(UtilityHelper.CanAutoLogin()) { UtilityHelper.AutoLogin();}};
			PhotonClient.Singleton.SavePersonalAccountInfo=()=>{
				string accountInfo = User.Singleton.UserData.Mail + "|";
				accountInfo = accountInfo + User.Singleton.UserData.Password + "|";
				accountInfo = accountInfo + ((int)User.Singleton.UserData.UserType).ToString();
				FileIOHelper.WriteFile(FileType.Account, accountInfo);
			};
			PhotonClient.Singleton.CloseMaskingTable=UtilityHelper.CloseMaskingTable;
			PhotonClient.Singleton.RoomDataChanged=UtilityHelper.RoomDataChanged;
		}
		else if(singleton!=gameObject)
		{
			Destroy(gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if(notification is ErrorNotification)
			{
				ErrorNotification errorNotification=GlobalManager.Singleton.Notifications.Dequeue() as ErrorNotification;
				string content=string.Empty;
				switch(errorNotification.ErrorCode)
				{
				case ErrorCode.SystemError:
					content=LocalizeHelper.Translate("LOGIN_SYSTEM_ERROR");
					break;
				case ErrorCode.AuthenticationFail:
					content=LocalizeHelper.Translate("LOGIN_AUTHENTICATION_FAIL");
					break;
				case ErrorCode.MailIsEmpty:
				case ErrorCode.PassWordIsEmpty:
				case ErrorCode.UserExist:			
				case ErrorCode.MailExist:
				case ErrorCode.NickNameExist:
					content=LocalizeHelper.Translate("LOGIN_USER_EXIST");
					break;
				case ErrorCode.UserNotExist:
					content=LocalizeHelper.Translate("FIND_PASSWORD_FAILED_DESCRIPTION");
					break;	
				};
				UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), content, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
			}
		}
	}
	
	void HandleError(string logString,string stackTrace,LogType logType)
	{
		if(logType==LogType.Exception||logType==LogType.Error)
		{
			StatisticsHelper.logError(logString,stackTrace);
		}
	}
	
	void OnLogin(string values)
	{
		PlatformHelper.OnLogin(values);
	}
	void OnLoginAsGuest(string values)
	{
		UtilityHelper.AutoLogin();
	}
	
	void OnApplicationQuit()
	{
		StatisticsHelper.endSession();
		
		GlobalManager.Singleton.EndTiming();
	}
	void OnCancelLogin()
	{
		Debug.Log("OnCancelLogin");
		Application.Quit();
	}
	void EmptyOnLogin()
	{}
}
