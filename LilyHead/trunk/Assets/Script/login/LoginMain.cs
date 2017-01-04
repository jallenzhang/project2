using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using System.Collections.Generic;
using System;
using LilyHeart;
//using PokerWorld.HandEvaluator;

public class LoginMain : MonoBehaviour 
{
	private User player;
	
	public  UIPanel parentItem;
	
	private const long gameId = 427916;
	private const string gameGuid = "2f33f47e6b654b98";
	private const string apiKey = "ae710c9ada45455ca8f99830944d78";
	
	private static object lockObj = new object();
	private static int timeoutTimes = 0;
	//public List<TextAsset> languages = new List<TextAsset>();
	
	/*
	 *  login user name and pwd
	 * 
	 * 
	 * 
	*/
	
	
	private bool loginOK;
	private bool didAutoLogin = false;
	
	private void RegisterRemoteNotification()
	{
		#if UNITY_IPHONE
		NotificationServices.RegisterForRemoteNotificationTypes(UnityEngine.RemoteNotificationType.Alert 
			| UnityEngine.RemoteNotificationType.Badge
			| UnityEngine.RemoteNotificationType.Sound);
		#endif
	}
	
	void Start()
	{
		//Add-by john wu 2012/08/14
		PhotonClient.LoginUserNameOrPasswordErrorEvent += LoginUserNameOrPassWordError;
		//End-add
		if(audio==null)
		{
			gameObject.AddComponent<AudioSource>();
		}
		SoundHelper.PlaySound("Music/Other/UIOpen",audio);
		
		loginOK=false;
		player = User.Singleton;
		RegisterRemoteNotification();
		if (player == null)
		{
			PhotonClient.Singleton.Connect();
			//Player.InitSingleton();
			
			player=User.Singleton;
			UtilityHelper.LoadResources();
		}
		
		if (User.Singleton.PlayInitAnimation)
		{
			Animation animation = gameObject.GetComponent<Animation>();
			animation.Play("StartinterfaceAnim");
			User.Singleton.PlayInitAnimation = false;
		}
		
		PhotonClient.GotoBackgroundSceneEvent += GotoRoom;
		PhotonClient.GotoLoadingSceneEvent += GotoLoading;
	}
	
	private IEnumerator internalLoadLevelAsync(AsyncOperation async)
	{
		while(!async.isDone){
        	LoadingPercentHelper.Progress = async.progress;
			yield return async.progress;
		}

		LoadingPercentHelper.Progress = 0;
    }
	
	AsyncOperation async = null;
	
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	void GotoLoading(){
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();		
	}
	
	void GotoRoom()
	{
		if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
	    	async = Application.LoadLevelAsync("BackGround");
		else if (GlobalManager.Singleton.version == KindOfVersion.Basic)
			async = Application.LoadLevelAsync("BackGround_simple");
	}
	
	void GotoGame()
	{
		transform.parent.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("GamblingInterface_Title");
	}
	
	/// <summary>
	/// Disableds all btns.
	/// </summary>
	void disabledAllBtns()
	{
		BoxCollider[] boxcoolides = gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=false;
		}
		
	}
	/// <summary>
	/// Enables the allbtns.
	/// </summary>
	void enableAllbtns()
	{
		BoxCollider[] boxcoolides =gameObject.GetComponentsInChildren<BoxCollider>();
		foreach(BoxCollider one in boxcoolides)
		{
			one.enabled=true;
		}
	}
	
	void PopupUpgradeDialog()
	{
		disabledAllBtns();
		GameObject prefab=Resources.Load("prefab/tips") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,110,-1);
		item.transform.localScale =new Vector3(1,1,1);
	}
	private void CheckLoginGameStatus()
	{
		if(User.Singleton.GameStatus==GameStatus.Error&&!loginOK&&PhotonClient.Singleton.IsConnected)
		{
			PhotonClient.LoginUserNameOrPasswordErrorEvent += LoginUserNameOrPassWordError;
			User.Singleton.GameStatus = GameStatus.NoStatus;
		}
	}
	void Update()
	{
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.LauchTable)==TargetType.LauchTable)
			{
				if(notification is CheckEmailNotification)
				{
					CheckEmailNotification checkEmailNotification=notification as CheckEmailNotification;
					if(checkEmailNotification.IsSuccess)
					{
						Debug.Log ("email:"+GlobalManager.Singleton.ParamEmail+",password:"+GlobalManager.Singleton.ParamPassword);
						WebBindingHelper.CloseWebView();
						PopUpAvatorDialog();
					}
					else
					{
						WebBindingHelper.ExcuteJS("hideloading();");
						UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), LocalizeHelper.Translate("LOGIN_USER_EXIST"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
					}
					GlobalManager.Singleton.Notifications.Dequeue();
				}
				else if(notification is The3rdFirstLoginNotification)
				{
					PopUpAvatorDialog();
					GlobalManager.Singleton.ParamPassword=string.Empty;
					GlobalManager.Singleton.Notifications.Dequeue();
				}
				else if(notification is FindPasswordNotification)
				{
					FindPasswordNotification findPasswordNotification=GlobalManager.Singleton.Notifications.Dequeue() as FindPasswordNotification;
					if(findPasswordNotification.IsSuccess)
					{
						UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_TITLE"), 
							LocalizeHelper.Translate("FIND_PASSWORD_SUCCESSED_DESCRIPTION"), 
							LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
					}
					else
					{
						UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("FIND_PASSWORD_FAILED_TITLE"),
							LocalizeHelper.Translate("FIND_PASSWORD_FAILED_DESCRIPTION"),
							LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
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
		
		CheckLoginGameStatus();
		SetProgress();
		lock(lockObj)
		{
			if((User.Singleton.GameStatus==GameStatus.InRoom||User.Singleton.GameStatus==GameStatus.InGame) && !loginOK)
			{
				loginOK=true;
				GlobalManager.Singleton.InitGuestSignIn = false;
				string accountInfo = User.Singleton.UserData.Mail + "|";
				accountInfo = accountInfo + User.Singleton.UserData.Password + "|";
				accountInfo = accountInfo + ((int)User.Singleton.UserData.UserType).ToString();
				FileIOHelper.WriteFile(FileType.Account, accountInfo);
				if(User.Singleton.GameStatus==GameStatus.InRoom)
				{
					//GotoRoom();
				}
				else if(User.Singleton.GameStatus==GameStatus.InGame)
				{
					//GotoGame();
				}
			}
			else if(User.Singleton.GameStatus==GameStatus.Error && !loginOK)
			{
				Debug.Log("In Login Update()");
				User.Singleton.GameStatus = GameStatus.NoStatus;
				//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
				if(!PhotonClient.Singleton.IsConnected)
				{
					timeoutTimes++;
					GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
					if (timeoutTimes > 3)
					{
						//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
					}
				}
			}
			else if (User.Singleton.GameStatus == GameStatus.Logout && !loginOK)
			{
				User.Singleton.GameStatus = GameStatus.NoStatus;
				//FileIOHelper.DeleteFile(FileType.Account);
				FileIOHelper.WriteFile(FileType.Account, string.Empty);
			}
		}
		
		StartCoroutine(NetworkUpdate());
		
	}
	
	void PopUpAvatorDialog()
	{
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
			
		item.transform.localPosition=new Vector3(0,0,-1);
		item.transform.localScale =new Vector3(1,1,1); 
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	//button message 
	// change the scene
	void OnLogin()
	{
		Application.LoadLevel("login");
	}
	
	void OnGuest()
	{
		Application.LoadLevel("login");
	}
	
	void OnRegist()
	{
		Application.LoadLevel("Registe");
	}

//	void OnApplicationQuit()
//	{
//		GlobalManager.Singleton.EndTiming();
//	}
	
	void OnDestroy()
	{
		PhotonClient.GotoBackgroundSceneEvent -= GotoRoom;
		PhotonClient.GotoLoadingSceneEvent-=GotoLoading;
		PhotonClient.LoginUserNameOrPasswordErrorEvent -= LoginUserNameOrPassWordError;
		timeoutTimes = 0;
	}
	
	void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			PhotonClient.Singleton.Connect();
			StartCoroutine(NetworkUpdate());
		}
	}
	void LoginUserNameOrPassWordError()
	{
		WebBindingHelper.ExcuteJS("hideloading();");
		//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
	}
}

