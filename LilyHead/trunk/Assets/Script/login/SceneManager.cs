using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using DataPersist;
using LilyHeart;

public class SceneManager : MonoBehaviour {
	
	public KindOfVersion version = KindOfVersion.Ultimate;
	bool loginOK = false;
	bool hasCheckUpdate=false;
	private static int timeoutTimes = 0;
	private DeviceInfo deviceInfomation;
	private bool bIsAndroid = false;
	private bool bIsGetDeviceType = false,bIsGetDeviceOSVersion = false,bIsGetDeviceToken = false;
	private delegate void GetDeviceInfoFinishEvent();
	private GetDeviceInfoFinishEvent getDeviceInfoFinishEvent;
	
	public delegate void GetISODeviceTokenFinishEvent();
	public GetISODeviceTokenFinishEvent getISODeviceTokenFinishEvent;
	public GameObject parentItem;
	// Use this for initialization
	
	void Awake()
	{
		GlobalManager.Singleton.version = version;
	}
	
	void Start () {
		if(audio==null)
		{
			gameObject.AddComponent<AudioSource>();
		}
		SoundHelper.PlaySound("Music/Other/UIOpen",audio);
		loginOK=false;
		RegisterRemoteNotification();
		PhotonClient.Singleton.Connect();
		if (User.Singleton == null)
		{	
			switch(GlobalManager.Singleton.ApplicationType)
			{
			case AppType.Normal:
				Player.Init();
				break;
			case AppType.NinetyOne:
				NinetyOnePlayer.Init();
				break;
			}
			StartCoroutine("TimeOut");
			UtilityHelper.LoadResources();
		}
		UILabel label = transform.FindChild("Label").GetComponent<UILabel>();
		label.text = MyVersion.CurrentVersion + MyVersion.BuildId;
		PhotonClient.GotoBackgroundSceneEvent += GotoRoom;
		PhotonClient.JoinGameFinished += GotoGame;
		deviceInfomation = new DeviceInfo();
		deviceInfomation.strChannelId = MyVersion.ChannelId;
		deviceInfomation.strClientVersion = MyVersion.CurrentVersion;
		GetDeiceInfo();
		if(!bIsAndroid&&deviceInfomation!=null)
		{
			getISODeviceTokenFinishEvent += IOSSubmitDeviceInfo;
		}
	}
	void IOSSubmitDeviceInfo()
	{
		Debug.Log("IOSSubmitDeviceInfo");
		bIsGetDeviceToken = true;
		if(deviceInfomation!=null)
			deviceInfomation.strDeviceToken = DeviceTokenHelper.myDeviceToken;
		SubmitDeviceInfo();
		getISODeviceTokenFinishEvent = null;
	}
	void SubmitDeviceInfo()
	{
		Debug.Log("SubmitDeviceInfo");
		if(deviceInfomation!=null&&bIsGetDeviceType&&bIsGetDeviceToken&&bIsGetDeviceOSVersion)
		{
			Debug.Log("strDeviceToken:"+deviceInfomation.strDeviceToken+" strDeviceType:"+deviceInfomation.strDeviceType+" strOSVersion:"+deviceInfomation.strOSVersion);
			PhotonClient.Singleton.DeviceInfoReady(deviceInfomation);
			getDeviceInfoFinishEvent = null;
		}
	}
	void GetDeiceInfo()
	{
		#if UNITY_IPHONE
		if(deviceInfomation != null)
		{
			deviceInfomation.strDeviceType = UtilityHelper.GetDeviceVersion();
			deviceInfomation.strOSVersion = UtilityHelper.GetOSVersion();
			deviceInfomation.strChannelId = UtilityHelper.GetChannelId();
			bIsGetDeviceOSVersion = true;
			bIsGetDeviceType = true;
		}
		#endif
		#if UNITY_ANDROID
		EtceteraAndroid.setReciveDeviceInfo(gameObject.name,"SetDeviceType","SetOSVersion","SetDeviceToken");
		getDeviceInfoFinishEvent +=SubmitDeviceInfo;
		bIsAndroid = true;
		#endif
	}
	void SetDeviceType(string result)
	{
		if(string.IsNullOrEmpty(result))
			return;
		if(deviceInfomation != null)
		{
			deviceInfomation.strDeviceType = result;
			bIsGetDeviceType = true;
			if(getDeviceInfoFinishEvent!=null)
				getDeviceInfoFinishEvent();
			Debug.Log("SetDeviceType:"+result);
		}
	}
	void SetOSVersion(string result)
	{
		if(string.IsNullOrEmpty(result))
			return;
		if(deviceInfomation != null)
		{
			deviceInfomation.strOSVersion = "Android"+result;
			bIsGetDeviceOSVersion = true;
			if(getDeviceInfoFinishEvent!=null)
				getDeviceInfoFinishEvent();
			Debug.Log("SetOSVersion:"+"Android"+result);
		}
	}
	void SetDeviceToken(string result)
	{
		if(string.IsNullOrEmpty(result))
			return;
		if(deviceInfomation != null)
		{
			deviceInfomation.strDeviceToken = result;
			bIsGetDeviceToken = true;
			if(getDeviceInfoFinishEvent!=null)
				getDeviceInfoFinishEvent();
			Debug.Log("SetDeviceToken:"+result);
		}
	}
	void GotoLaunchTable()
	{
		if (version == KindOfVersion.Ultimate)
			Application.LoadLevelAsync("LaunchTable");
		else if (version == KindOfVersion.Basic)
		{
			if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
			{
				Application.LoadLevelAsync("LaunchTable_91");
			}
			else
				Application.LoadLevelAsync("LaunchTable_simple");
		}
	}
	
	IEnumerator TimeOut ()
	{
		yield return new WaitForSeconds(30.0f);
		UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), LocalizeHelper.Translate("CONNECTING_TIME_OUT_ERROR"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		GotoLaunchTable();
	}
	
	// Update is called once per frame
	void Update () {
		if(PhotonClient.Singleton.IsConnected&&!hasCheckUpdate)
		{
			hasCheckUpdate=true;
			PlatformHelper.CheckUpdate(gameObject.name,"UpdatePassed","UpdateFailed");
			StopCoroutine("TimeOut");
		}
//		else if (User.Singleton.GameStatus==GameStatus.Connected)
//		{
//			StopCoroutine("TimeOut");
//			User.Singleton.GameStatus = GameStatus.NoStatus;
//		}
		else if((User.Singleton.GameStatus==GameStatus.InRoom||User.Singleton.GameStatus==GameStatus.InGame) && !loginOK)
		{
			loginOK=true;
			if(User.Singleton.GameStatus==GameStatus.InRoom)
			{
				Debug.Log("!!!!!!! SceneManager");
				//GotoRoom();
			}
			else if(User.Singleton.GameStatus==GameStatus.InGame)
			{
				//GotoGame(true);
			}
		}
		else if(User.Singleton.GameStatus==GameStatus.Error && !loginOK)
		{
			User.Singleton.GameStatus = GameStatus.NoStatus;
			GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
			timeoutTimes++;
			if (timeoutTimes >= 3)
			{
				//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
				StopCoroutine("TimeOut");
			}
				
			GotoLaunchTable();
		}
		else if (User.Singleton.GameStatus == GameStatus.Logout && !loginOK)
		{
			User.Singleton.GameStatus = GameStatus.NoStatus;
			//FileIOHelper.DeleteFile(FileType.Account);
			FileIOHelper.WriteFile(FileType.Account, string.Empty);
			GotoLaunchTable();
		}
		
		StartCoroutine(NetworkUpdate());
		
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.Empty)==TargetType.Empty)
			{
				if(notification is The3rdFirstLoginNotification)
				{
					
					GlobalManager.Singleton.Notifications.Dequeue();
					PopUpAvatorDialog();
				}
				else if(notification is CheckUpdateNotification)
				{
					CheckUpdateNotification checkUpdateNotification=GlobalManager.Singleton.Notifications.Dequeue() as CheckUpdateNotification;
					GlobalManager.Singleton.CurrentVersion=checkUpdateNotification.ClientVersion;
					if (MyVersion.CurrentVersion != GlobalManager.Singleton.CurrentVersion)//need popup dialog to download the latest version  Upgrade
					{
						if(Application.platform==RuntimePlatform.IPhonePlayer)
						{
							string[] localValues=MyVersion.CurrentVersion.Split('.');
							string[] serverValues=GlobalManager.Singleton.CurrentVersion.Split('.');
							if(localValues[0]!=serverValues[0]||localValues[1]!=serverValues[1])
							{
								GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NewVesionDialog(GlobalManager.Singleton.CurrentVersion));
								PopupUpgradeDialog();
							}
							else
							{
								UpdatePassed();
							}
						}
						else
						{
							GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NewVesionDialog(GlobalManager.Singleton.CurrentVersion));
							PopupUpgradeDialog();
						}
					}
					else
					{
						UpdatePassed();
					}
				}
				else if(notification is ErrorNotification)
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
			else
			{
				GlobalManager.Singleton.Notifications.Dequeue();
			}
		}
	}
	
	void UpdatePassed()
	{
		User.Singleton.DeviceTokenAssign(DeviceTokenHelper.myDeviceToken);
						
		if(PlatformHelper.CanAutoLogin())
		{
			PlatformHelper.AutoLogin();
		}
		else
		{
			UserData user=UtilityHelper.ReadAccount();
			if(user!=null&&user.UserType==UserType.Guest)
			{
				switch(GlobalManager.Singleton.ApplicationType)
				{
				case AppType.Normal:
					GotoLaunchTable();
					break;
				case AppType.NinetyOne:
					PlatformHelper.LoginAsGuest(gameObject.name,"OnLoginAsGuest","OnLogin");
					break;
				}
			}
			else
			{
				GotoLaunchTable();
			}
		}
	}
	
	void UpdateFailed()
	{
		Application.Quit();
	}
	
	void PopUpAvatorDialog()
	{
		WebBindingHelper.CloseWebView();
		GameObject prefab = null;
		if (GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
			prefab=Resources.Load("prefab/chooseAvatar_91") as GameObject;
		else
			prefab=Resources.Load("prefab/chooseAvatar") as GameObject;
		
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=parentItem.transform;
		AvatorGridScript avatorGridScript = item.GetComponent<AvatorGridScript>();
		
		if (avatorGridScript != null)
		{
			avatorGridScript.mail = GlobalManager.Singleton.ParamEmail;
			avatorGridScript.password = GlobalManager.Singleton.ParamPassword;
		}
			
		item.transform.localPosition=new Vector3(0,0,-10);
		item.transform.localScale =new Vector3(1,1,1); 
	}
	
	void OnLoginAsGuest()
	{
		UtilityHelper.AutoLogin();
	}
	
	void OnLogin(string values)
	{
		PlatformHelper.OnLogin(values);
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	private IEnumerator internalLoadLevelAsync(AsyncOperation async)
	{
		while(!async.isDone){
        	LoadingPercentHelper.Progress = async.progress;
			Debug.Log("aaaaaaaaa " + LoadingPercentHelper.Progress);
			yield return async.progress;
		}

		LoadingPercentHelper.Progress = 0;
    }
	
	
	void GotoRoom()
	{
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		AsyncOperation async;
		if (version == KindOfVersion.Ultimate)
			async = Application.LoadLevelAsync("BackGround");
		else if (version == KindOfVersion.Basic)
		    async = Application.LoadLevelAsync("BackGround_simple");
		else
			async = Application.LoadLevelAsync("BackGround");
		
		StartCoroutine(internalLoadLevelAsync(async));
	}
	
	void GotoGame(bool bResult,TypeState gamestate)
	{
		if (PhotonClient.Singleton.ErrorMessage == ErrorCode.Sucess.ToString())
		{
			JoinGamblingSettingInfor.Singleton.haveDone=true;
			transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
			AsyncOperation async = Application.LoadLevelAsync("GamblingInterface_Title");
			StartCoroutine(internalLoadLevelAsync(async));
		}
	}
	
	void PopupUpgradeDialog()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,110,-1);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	private void RegisterRemoteNotification()
	{
		#if UNITY_IPHONE
		NotificationServices.RegisterForRemoteNotificationTypes(UnityEngine.RemoteNotificationType.Alert 
			| UnityEngine.RemoteNotificationType.Badge
			| UnityEngine.RemoteNotificationType.Sound);
		#endif
	}
	
	void OnDestroy()
	{
		PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
		PhotonClient.JoinGameFinished -= GotoGame;
		getDeviceInfoFinishEvent = null;
		StopCoroutine("TimeOut");
	}
}
