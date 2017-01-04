using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Text;
using DataPersist;
using AssemblyCSharp.Helper;
using System;
using LilyHeart;

public class RoomBackground : MonoBehaviour {

	// Use this for initialization
	public GameObject RoleObject;
	public GameObject JoinGameObject;
	public GameObject SpeakWorldDlg;
	public GameObject SpeakWorldTextList;
	public GameObject BeforeClickBackGround;
	public GameObject AferClickBoxAnimation;
	public GameObject BtnDragon;
	public GameObject BtnHat;
	public GameObject BtnVIP;
	public GameObject BtnNormalSetup;
	public GameObject Btn91Setup;
	
	public GameObject Label_lv;
	private AudioSource addChipAudioSource;
	public GameObject HelpIconBtn;
	public UISlicedSprite HelpIconBtnBackGroup;
	public GameObject SetupIconBtn;
	public UISlicedSprite SetupIconBtnBackGroup;
	public GameObject GetAwardAnimaBtn;
	public GameObject BtnGameObjectLogo_91;
	
	private bool hasUpdated = false;
	private bool bIsAlreadyGetSystemNotice = false;
	private int nGetTime = 0;
	private bool bIsCanGetAwardInRoom = false;
	
	public enum AvatorType:byte
	{
		Guest,
		DaHeng = 1,
		Songer,
		Captain,
		European,
		Qianjing,
		Dalaopo,
		Yitaitai,
		Luoli,
		AGe,
		General,//10
		Princess,
		Queen
	}
	 
	void Awake()
	{
		User.Singleton.FriendList();
		MusicManager.Singleton.ForeAudio=gameObject.AddComponent<AudioSource>();
		addChipAudioSource=gameObject.AddComponent<AudioSource>();
	}
	
	void Start () {

		if (User.Singleton == null)
		{
			Debug.Log("User is null!");
			return;
		}
		//PhotonClient.RegisterOrUpgradeEvent += UpgradeFinished;
		//Init Room.Singleton.RoomData.Owner
		if(Room.Singleton.RoomData.Owner ==null)
			Room.Singleton.RoomData.Owner = User.Singleton.UserData;
		
		if(Room.Singleton.RoomData.Owner.UserId != User.Singleton.UserData.UserId)
		{
			if(HelpIconBtn!=null)
			{
				if(HelpIconBtnBackGroup!=null)
					HelpIconBtnBackGroup.color = Color.gray;
				BoxCollider box = HelpIconBtn.GetComponent<BoxCollider>();
				if(box!=null)
					box.enabled = false;
			}
			if(SetupIconBtn!=null)
			{
				if(SetupIconBtnBackGroup!=null)
					SetupIconBtnBackGroup.color = Color.gray;
				BoxCollider box = SetupIconBtn.GetComponent<BoxCollider>();
				if(box!=null)
					box.enabled = false;
			}
		}
		InitSpeakLable();
		
		//if (User.Singleton.UserData.Avator != 0)
		{
			UpdateBackground();
			UpdateButtons();
		}
		
		//User.UserDataChangedEvent -= UpdateButtons;
		User.UserDataChangedEvent += UpdateButtons;
		PhotonClient.QueryUserEvent += UpdateInfo;
		PhotonClient.SameAccountLoginEvent += ReceiveSameAccountLoginEvent;
		PhotonClient.GotoBackgroundSceneEvent += GotoBackgroundSceneEvent;
		PhotonClient.Singleton.QueryUserById(User.Singleton.UserData.UserId);
		PhotonClient.BuyItemResponseEvent += BuyItemResponseEvent;
		PhotonClient.RegisterInSetupSuccessEvent += UpdateWalkAnimationAfterRegister;
		
		if(GlobalManager.Singleton.version == KindOfVersion.Ultimate)
			gameObject.AddComponent<HelpPromtScript>();
		
		if( MusicManager.Singleton.BgAudio!=null)
		{
			StartCoroutine(MusicManager.Singleton.BgFadeIn(1f));
		}
		
		MusicManager.Singleton.PlayBgMusic();
//		string iTunesUrl = "http://www.toufe.com";
//		if(User.Singleton.UserData.UserId == Room.Singleton.RoomData.Owner.UserId)
//			CheckNeedAlertRateApp(iTunesUrl);
#if UNITY_IPHONE
		EtceteraBinding.etceteraPurchaseInfos();
#endif
	}
	
	UIAtlas getAtlas(string tile)
	{
		string path=string.Format("Game Lobby/{0}Atlas",tile);
 		GameObject loadaltalsObject=Resources.Load(path) as GameObject;
		return loadaltalsObject.GetComponent<UIAtlas>();
	}
	private void InitThreeButtons()
	{
		bool bIsOpenKangXi = false;
		bool bIsLineage = false;
		bool bIsVip = false;
		bIsOpenKangXi = User.Singleton.UserData.Jade;
		bIsLineage = User.Singleton.UserData.LineAge;
		if(User.Singleton.UserData.VIP >0) bIsVip = true;
	    
		DoUpdateBtnBackgroup(BtnDragon,"TableBtn_dragon",bIsOpenKangXi);
		DoUpdateBtnBackgroup(BtnHat,"TableBtn_hat",bIsLineage);
		DoUpdateBtnBackgroup(BtnVIP,"TableBtn_Vip",bIsVip);
	}
	private void Init91LogoBtn()
	{
		if(BtnGameObjectLogo_91!=null)
		{
			BtnGameObjectLogo_91.SetActiveRecursively(false);
			if(GlobalScript.ScriptSingleton.ApplicationType == AppType.NinetyOne)
			{	
				BtnGameObjectLogo_91.SetActiveRecursively(true);
			}
		}
	}
	void Enter91Platform()
	{
		Debug.Log("Enter91Platform");
		NdComHelper.Enter91Platform();
	}
	private  void  DoUpdateBtnBackgroup(GameObject Btn, string backgroupName,bool isOpen)
	{
		if(Btn!=null)
		{
			UISprite backgroup =  Btn.GetComponent<UISprite>();
			if(backgroup!=null&&!string.IsNullOrEmpty(backgroupName))
			{
				if(backgroup.atlas==null)
					backgroup.atlas = getAtlas("GameLobbv");
				if(!isOpen)
					backgroup.spriteName = backgroupName + "_dark";
				else
					backgroup.spriteName = backgroupName;
			}
		}
	}
	private  void RequestSystemNotice()
	{
		if (GlobalManager.Singleton.CurrentBoardMessage == null)
		{
			PhotonClient.Singleton.SystemNotice();
		}
	}
	private  void InitSpeakLable()
	{
		if(SpeakWorldTextList!=null)
		{
			UITextList textList = SpeakWorldTextList.GetComponent<UITextList>();
			if(textList == null)
				return;
		    textList.Clear();
		}
		bool bIsShow = SettingManager.Singleton.CallInRoom;
		ShowSystemNotice();
		
		if (SpeakWorldDlg != null && SpeakWorldTextList != null)
		{
			SpeakWorldDlg.SetActiveRecursively(bIsShow);
			SpeakWorldTextList.SetActiveRecursively(bIsShow);
	   }
		if(GlobalManager.Singleton.CurrentBoardMessage == null)
		{
			RequestSystemNotice();
			ShowSystemNotice();
		}
		Invoke("ReSendRequestSystemNotice",2.6f);
	}
	private void ShowSystemNotice()
	{
		if(GlobalManager.Singleton.CurrentBoardMessage!=null)
			bIsAlreadyGetSystemNotice = true;
		string messageTitle = string.Empty;
		string lastSystemMessage = string.Empty;
		
		if(bIsAlreadyGetSystemNotice)
		{
			UITextList textList = null;
			if(SpeakWorldTextList!=null)
			{
				textList = SpeakWorldTextList.GetComponent<UITextList>();
				if(textList == null)
					return;
			}
			foreach(LilyBoardMessages message in GlobalManager.Singleton.BoardMessages)
			{
				if(message.kmMessagesType == WorldMessageType.SystemNotice)
				{
					//judge if system notice
					messageTitle = message.Messages.Substring(9,2);
					if(!string.IsNullOrEmpty(messageTitle)&&messageTitle == LocalizeHelper.Translate("WORLD_SPEAK_SYSTEM_NOTICE_TITLE"))
					{
						if(!string.IsNullOrEmpty(lastSystemMessage))
						{
							if(lastSystemMessage == message.Messages)
								continue;
						}
						lastSystemMessage = message.Messages;
					}
				}
				Debug.Log("ShowSystemNotice:"+message.Messages+"message.type"+message.kmMessagesType.ToString());
				textList.Add(message.Messages);
			}
		}
	}
	private  void CheckNeedAlertRateApp(string iTunesUrl)
	{
		bool bIsNeedRateAlert = User.Singleton.AppScore;
		
		if(!string.IsNullOrEmpty(iTunesUrl))
		{
			if(bIsNeedRateAlert){
#if UNITY_IPHONE
				EtceteraBinding.askForReview(LocalizeHelper.Translate("RATE_ALERT_TITLE"),LocalizeHelper.Translate("RATE_ALERT_MESSAGE"),iTunesUrl);
#endif
			}
		}
		User.Singleton.AppScore=false;
	}

	void BuyItemResponseEvent (bool success, ItemType itemType)
	{
		roombtnAction rba = gameObject.GetComponent<roombtnAction>();
		rba.onBuyItemResponse(success, itemType);
	}
	
	void UpdateInfo(UserData userData)
	{
		UpdateBackground();
		UpdateButtons();
	}
	void UpdateWalkAnimationAfterRegister()
	{
		UtilityHelper.RoomDataChanged();
	}
	/// <summary>
	/// Disableds all btns.
	/// </summary>
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = gameObject.GetComponentsInChildren<BoxCollider>();
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=false;
//		}
//	}
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
	public void ConditionBeforePop()
	{
		if(BeforeClickBackGround == null|| AferClickBoxAnimation == null)
			return;
		InitAwardChest();
		UtilityHelper.PreConditionFinished(true);
	}
	private void InitAwardChest()
	{
		bool bIsCanGetAward = false;
		if(!string.IsNullOrEmpty(User.Singleton.UserData.Awards))
		{
			bIsCanGetAward = IsCanGetAward(User.Singleton.UserData.Awards);
			bIsCanGetAwardInRoom = bIsCanGetAward;
		}
		SetDailySignBoxSatus(bIsCanGetAward);
	}
	private void SetDailySignBoxSatus(bool bIsCanGetAward)
	{
		if (AferClickBoxAnimation != null && BeforeClickBackGround != null)
		{
			if(GlobalManager.Singleton.version == KindOfVersion.Basic)
			{
				if(!bIsCanGetAward)
					BeforeClickBackGround.SetActiveRecursively(bIsCanGetAward);
			}
			else
			if(GlobalManager.Singleton.version == KindOfVersion.Ultimate)
			{
				AferClickBoxAnimation.SetActiveRecursively(bIsCanGetAward);
				BeforeClickBackGround.SetActiveRecursively(!bIsCanGetAward);
			}
		}
	}
	public void CloseBoxAfterPop()
	{
		SetDailySignBoxSatus(false);
		bIsCanGetAwardInRoom = true;
		bool bIsAStyle = true;
		if(bIsCanGetAwardInRoom)
		{
			if(GetAwardAnimaBtn!=null)
			{
				GetAwardAnimaBtn.SetActiveRecursively(true);
				// show a style 
				bIsAStyle = false;
				if(bIsAStyle)
					Util.FadeWidtsOutWithTime(GetAwardAnimaBtn.transform,2.5f);
				else//show by b style
				{
					Invoke("ShowAwardBStyle",1.5f);
				}
			}
			bIsCanGetAwardInRoom =false;
		}
	}
	private  void ShowAwardBStyle()
	{
		TweenPosition.Begin(GetAwardAnimaBtn,0.5f,new Vector3(GetAwardAnimaBtn.transform.localPosition.x,GetAwardAnimaBtn.transform.localPosition.y+Screen.height/4,0f));
		Util.FadeWidtsOutWithTime(GetAwardAnimaBtn.transform,0.5f);
	}
	private bool IsCanGetAward(string str)
	{
		bool bIsCanGetReward = false;
		if(str == null &&str.Length == 0)
			return bIsCanGetReward;
		if(str.IndexOf('|') == -1)
		{
			if(str.IndexOf(',') == -1) return bIsCanGetReward;
			string [] sStringTmpAry = str.Split(new char[]{','});
			if(sStringTmpAry.Length!=2) return bIsCanGetReward;
			if(Convert.ToInt32(sStringTmpAry[0])<4)
			{
				bIsCanGetReward = true;
			}
		}
		else
		{
			string [] myDailyStringStatus = str.Split(new char[]{'|'});
			string s = string.Empty;
			for(int i = 0;i<myDailyStringStatus.Length&&i<7;i++)
			{
				s = myDailyStringStatus[i];
				if(s.IndexOf(',') == -1) return bIsCanGetReward;
				string [] sStringTmpAry = s.Split(new char[]{','});
				if(sStringTmpAry.Length!=2) return bIsCanGetReward;
				if(Convert.ToInt32(sStringTmpAry[0])<4)
				{
					bIsCanGetReward = true;
					break;
				}
			}
		}
		return bIsCanGetReward;
	}
//	void PopUpTips()
//	{
//		disabledAllBtns();
//		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
//		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
//		
//		item.transform.parent=transform;
//		item.transform.localPosition=new Vector3(0,200,-1);
//		item.transform.localScale =new Vector3(1,1,1);
//	}
//	
//	void PopupMessage()
//	{
//		disabledAllBtns();
//		GameObject prefab=Resources.Load("prefab/PromptMessageTable") as GameObject;
//		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
//		
//		item.transform.parent=transform;
//		item.transform.localPosition=new Vector3(0,0,-20);
//		item.transform.localScale =new Vector3(1,1,1);
//	}
	
	// Update is called once per frame
	void Update () {
		SetProgress();
		if(User.Singleton == null)
			Debug.Log("User.Singleton == null");
		if (User.Singleton.UserData == null)
			Debug.Log("User.Singleton.UserData !!!!!!");
		if (!hasUpdated && User.Singleton.UserData.Avator != 0)
		{
			UpdateButtons();
			hasUpdated = true;
		}
		
		if(User.Singleton.GameStatus==GameStatus.Error)
		{
			User.Singleton.GameStatus = GameStatus.NoStatus;
			GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
			//EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		}
		
		if(GlobalManager.Singleton.Notifications.Count>0)
		{
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.Room)==TargetType.Room)
			{
				if(notification is GetAwardNotification)
				{
					GlobalManager.Singleton.Notifications.Dequeue();
					SoundHelper.PlaySound("Music/Other/DailySendChip",addChipAudioSource);
				}
				else if(notification is  SystemNoticeNotification)
				{
					SystemNoticeNotification sysNotifacation =  notification as SystemNoticeNotification;
					DealSystemNoticeNotification(sysNotifacation.m_notice,sysNotifacation.m_nickName,
						sysNotifacation.m_mytype,sysNotifacation.m_myparams,sysNotifacation.m_kmMessageType);
					GlobalManager.Singleton.Notifications.Dequeue();
					ShowSystemNotice();
				}
				else if(notification is  GetGameGradesNotification)
				{
					GlobalManager.Singleton.Notifications.Dequeue();
					GameObject ChampionObject = GameObject.Find("Champion(Clone)");
					if(ChampionObject!=null)
					{
						Champion champion = ChampionObject.GetComponent<Champion>();
						if(champion!=null)
						{
							champion.DealNotificationGameGrades();
						}
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
		StartCoroutine(NetworkUpdate());
	}
	void DealSystemNoticeNotification(string notice,string nickName,StatusTipsType mytype,object [] myparams,WorldMessageType kmMessageType)
	{
		Debug.Log("DealSystemNoticeNotification");
		switch(kmMessageType)
		{
			case WorldMessageType.SystemNotice:
			{
				string strNotice = string.Empty;
				strNotice = string.Format(LocalizeHelper.Translate("SPEAK_SYSTEM"), notice);
				AddWorldBoardMessage(strNotice,kmMessageType);
			}
			break;
			case WorldMessageType.WorldSpeak:
			{
				string boardMessage = string.Format(LocalizeHelper.Translate("SPEAK_WORLD"), nickName, notice);
				AddWorldBoardMessage(boardMessage,WorldMessageType.WorldSpeak);
				if(nickName!=User.Singleton.UserData.NickName)
				{
					notifySpeakWorldTextList();
				}
			}
			break;
			case WorldMessageType.StatusTip:
			{
				string myres = StatusTipsMsgHelper.GetStatusTipsMsg(mytype, myparams);
				string strnotice = string.Format(LocalizeHelper.Translate("SPEAK_SYSTEM"), myres);
				AddWorldBoardMessage(strnotice,WorldMessageType.StatusTip);
				notifySpeakWorldTextList();
			}
			break;
		default:
			break;
		}
	}
	void notifySpeakWorldTextList()
	{
		Debug.Log("notifySpeakWorldTextList");
		GameObject speakWorldObject = GameObject.Find("SpeakWorld(Clone)");
		if(speakWorldObject!=null)
		{
			Debug.Log("SpeakWorldTextList!=null");
			SpeakWorld spWorld = speakWorldObject.GetComponent<SpeakWorld>();
			if(spWorld!=null)
			{
				Debug.Log("spWorld.BroadcastMessageFromWorld();");
				spWorld.BroadcastMessageFromWorld();
			}
		}
	}
	void AddWorldBoardMessage(string message,WorldMessageType type)
	{
		LilyBoardMessages lilyBoardMessage = new LilyBoardMessages ();
		lilyBoardMessage.Messages = message;
		lilyBoardMessage.kmMessagesType = type;
		GlobalManager.Singleton.PushInBoardMessage(lilyBoardMessage);
		GlobalManager.Singleton.CurrentBoardMessage = lilyBoardMessage;
	}
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	private void ReSendRequestSystemNotice()
	{
		if(nGetTime<5&&!bIsAlreadyGetSystemNotice&&GlobalManager.Singleton.CurrentBoardMessage==null)
			{
				Invoke("RequestSystemNotice",3f*nGetTime+0.5f);
				nGetTime++;
			}
	}
	private static string GetAvatorPicString(byte avator)
	{
		string result = string.Empty;
		result = ((AvatorType)avator).ToString();
		
		return result;
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
	
	AsyncOperation async = null;
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	void GotoGame()
	{
		transform.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("GamblingInterface_Title");
		//StartCoroutine(internalLoadLevelAsync(async));
	}
	
	long GetExp(int level, long exp)
	{
		long result = exp;
		
		if (level - 1 > 0)
		{
			long nextValue = (long)(((level-1) * (level-1)) * ((level-1) + 4)/3);
			result += GetExp(level-1, nextValue);
		}
		
		return result;
	}
	
	void ReceiveSameAccountLoginEvent()
	{
		PhotonClient.SameAccountLoginEvent -= ReceiveSameAccountLoginEvent;
		roombtnAction rba = gameObject.GetComponent<roombtnAction>();
		rba.ReceiveSameAccountLoginEvent();
	}
	
	void GotoBackgroundSceneEvent()
	{
		PhotonClient.GotoBackgroundSceneEvent -= GotoBackgroundSceneEvent;
		roombtnAction rba = gameObject.GetComponent<roombtnAction>();
		rba.GotoHome();
	}
	
	void UpdateBackground()
	{
		if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
		{
			UserData userData = Room.Singleton.RoomData.Owner;
			
			ChangeBackGroundSence cbgs = gameObject.GetComponent<ChangeBackGroundSence>();
			cbgs.changeRoomBg(userData.LivingRoomType.ToString());
		}
	}
	
	void UpdateSetupButtons()
	{
		if (GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			BtnNormalSetup.SetActiveRecursively(false);
			Btn91Setup.SetActiveRecursively(true);
		}
		else
		{
			BtnNormalSetup.SetActiveRecursively(true);
			Btn91Setup.SetActiveRecursively(false);
		}
	}
	
	void UpdateButtons()
	{
		Debug.Log("UpdateButtons ***** ");
		if (gameObject == null)
			return;
		
		if (User.Singleton.UserData == null)
			return;

		InitThreeButtons();
		Init91LogoBtn();
		InitAwardChest();
		
		UpdateSetupButtons();
		
		Transform[] alltrans=gameObject.GetComponentsInChildren<Transform>();
		
		int level = User.Singleton.UserData.Level == 0 ? 1 : User.Singleton.UserData.Level;
		foreach(Transform trs in alltrans)
		{
			UILabel label = (UILabel)trs.gameObject.GetComponent<UILabel>();
			UISlicedSprite slicedSprite = (UISlicedSprite)trs.gameObject.GetComponent<UISlicedSprite>();
			UISprite sprite = (UISprite)trs.gameObject.GetComponent<UISprite>();
			switch(trs.gameObject.name)
			{
			case "Label_lv":
				StringBuilder builder = new StringBuilder("Lv.");
				builder.Append(level.ToString());
				UILabel label_v = (UILabel)Label_lv.GetComponent<UILabel>();
				label_v.text = builder.ToString();
				break;
			case "Label_name_info":
				label.text = User.Singleton.UserData.NickName;
				break;
			case "SlicedSprite (GrandDuke)":
				HonorType honor = HonorHelper.GetHonorRecuise(User.Singleton.UserData, HonorType.Citizen);
				slicedSprite.spriteName = HonorHelper.GetHonorPicString(honor, User.Singleton.UserData.Avator);
				break;
			case "Sprite (Missy)":
				sprite.spriteName = GetAvatorPicString(User.Singleton.UserData.Avator);
				break;
			case "PBForeground":
				
				long expNeeded = (long)((level * level) * (level + 4)/3);
				
				UIFilledSprite fillSprite = trs.gameObject.GetComponent<UIFilledSprite>();
				fillSprite.fillAmount = (float)((float)User.Singleton.UserData.LevelExp/(float)expNeeded);
				break;
			case "Button_VFHBackBtn":
				UpdateBackButton(trs.gameObject);
				break;
			case "Button_VFHFriendBtn":
				UpdateFriendConner(trs.gameObject);
				break;
			case "Button_joinGamb":
				if (Room.Singleton.RoomData.Owner.UserId == User.Singleton.UserData.UserId)
					trs.gameObject.transform.localPosition = new Vector3(1000, 137.2316f, 0);
				else
					trs.gameObject.transform.localPosition = new Vector3(305, 103, 0);
					
				break;
			case "Button_Check":
				if (Room.Singleton.RoomData.Owner.UserId != User.Singleton.UserData.UserId)
				{
					Transform trs1 = trs.gameObject.transform.FindChild("Sprite (GLCheck)");
					if (trs1 != null)
					{
						UISprite ss = trs1.gameObject.GetComponent<UISprite>();
						ss.color = Color.gray;
					}
					Transform trs2 = trs.gameObject.transform.FindChild("Background");
					if (trs2 != null)
					{
						UISlicedSprite sliceds = trs2.gameObject.GetComponent<UISlicedSprite>();
						sliceds.color = Color.gray;
					}
					Transform trs3 = trs.gameObject.transform.FindChild("icon");
					if (trs3 != null)
					{
						UISlicedSprite sliceds2 = trs3.gameObject.GetComponent<UISlicedSprite>();
						sliceds2.color = Color.gray;
					}
					BoxCollider box = trs.gameObject.GetComponent<BoxCollider>();
					box.enabled = false;
				}
				break;
			case "Button_GotChips":
				GameObject chipsObj = trs.gameObject.transform.FindChild("Label_chips").gameObject;
				UILabel chipslabel = chipsObj.GetComponent<UILabel>();
				chipslabel.text = User.Singleton.UserData.Chips >= 100 ? string.Format("{0:0,00}", User.Singleton.UserData.Chips) : User.Singleton.UserData.Chips.ToString();
				break;
			}
		}
	}
	
	void UpdateFriendConner(GameObject friendObject)
	{
		//bool bInMyRoom = GetUserData(ref userData);
		if (Room.Singleton.RoomData.Owner.UserId != User.Singleton.UserData.UserId)
		{
			friendObject.transform.localPosition = new Vector3(316, 182, 0);
			Transform[] alltrans=gameObject.GetComponentsInChildren<Transform>();
			
			//UserData friendInfo = userData;
			UserData friendInfo = Room.Singleton.RoomData.Owner;
			foreach(Transform trs in alltrans)
			{
				UISprite sprite = (UISprite)trs.gameObject.GetComponent<UISprite>();
				UILabel label = (UILabel)trs.gameObject.GetComponent<UILabel>();
				switch(trs.gameObject.name)
				{
				case "avatar":
					sprite.spriteName = GetAvatorPicString(friendInfo.Avator);
					//trs.gameObject.transform.localScale = new Vector3(61f, 81f, 1);
					break;
				case "chips":
					int level = HonorHelper.GetChipLevel(friendInfo.Chips, 1);
					sprite.spriteName = HonorHelper.GetChipString(level);
					break;
				case "Label_chip":
					label.text = friendInfo.Chips >= 100 ? string.Format("{0:0,00}", friendInfo.Chips) : friendInfo.Chips.ToString();
					break;
				case "Label_Lv":
					StringBuilder builder = new StringBuilder("Lv.");
					label.text = builder.Append(friendInfo.Level.ToString()).ToString();
					break;
				case "Label_name":
					label.text = friendInfo.NickName;
					break;
				case "title":
					HonorType type = HonorHelper.GetHonorRecuise(friendInfo, HonorType.Citizen);
					sprite.spriteName = HonorHelper.GetHonorPicString(type, friendInfo.Avator);
					break;
				case "Label_Title":
					label.text = UtilityHelper.GetHonorName(friendInfo);
					break;
				}
			}
		}
		else
		{
			friendObject.transform.localPosition = new Vector3(1000, 248.3766f, 0);
		}
	}
	
	void UpdateBackButton(GameObject backObject)
	{
		//UserData userData = null;
		//bool bInMyRoom = GetUserData(ref userData);
		if (Room.Singleton.RoomData.Owner.UserId != User.Singleton.UserData.UserId)
		{
			backObject.transform.localPosition = new Vector3(-437, 208, 0);
			Transform[] alltrans=gameObject.GetComponentsInChildren<Transform>();
			
			foreach(Transform trs in alltrans)
			{
				UISprite sprite = (UISprite)trs.gameObject.GetComponent<UISprite>();
				switch(trs.gameObject.name)
				{
				case "avatar_back":
					sprite.spriteName = GetAvatorPicString(User.Singleton.UserData.Avator);
					//trs.gameObject.transform.localScale = new Vector3(61f, 81f, 1);
					break;
				}
			}
		}
		else
		{
			backObject.transform.localPosition = new Vector3(1000, 248.3766f, 0);
		}
	}
	
	void BroadcastMessageFromWorld(string boardMessage)
	{
		UITextList textList = transform.FindChild("TextList").GetComponent<UITextList>();
		textList.Add(boardMessage);
		if (SettingManager.Singleton.CallInRoom)
		{
			if (SpeakWorldDlg != null && SpeakWorldTextList != null)
			{
				SpeakWorldDlg.SetActiveRecursively(true);
				SpeakWorldTextList.SetActiveRecursively(true);
			}
		}
		else
		{
			if (SpeakWorldDlg != null && SpeakWorldTextList != null)
			{
				SpeakWorldDlg.SetActiveRecursively(false);
				SpeakWorldTextList.SetActiveRecursively(false);
			}
		}
	}
	
	void UpgradeFinished(bool result)
	{
		if (result)
		{
			PhotonClient.RegisterOrUpgradeEvent -= UpgradeFinished;
			roombtnAction rba = gameObject.GetComponent<roombtnAction>();
			
			//rba.PopupChooseRoleDialog();
		}
	}
	
	void OnDestroy()
	{
		User.UserDataChangedEvent -= UpdateButtons;
		PhotonClient.QueryUserEvent -= UpdateInfo;
		//PhotonClient.BroadcastMessageEvent -= BroadcastMessageFromWorld;
		PhotonClient.RegisterOrUpgradeEvent -= UpgradeFinished;
		PhotonClient.SameAccountLoginEvent -= ReceiveSameAccountLoginEvent;
		PhotonClient.GotoBackgroundSceneEvent -= GotoBackgroundSceneEvent;
		PhotonClient.BuyItemResponseEvent -= BuyItemResponseEvent;
		PhotonClient.RegisterInSetupSuccessEvent -= UpdateWalkAnimationAfterRegister;
		User.Singleton.MessageOperating = false;
	}
	
//	void OnApplicationQuit()
//	{
//		GlobalManager.Singleton.EndTiming();
//	}
	
	void ActivedToIdle()
	{
		User.Singleton.ActivedToIdle();
	}
	
	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Debug.Log("OnApplicationPause Suspend");
			UtilityHelper.CloseMaskingTable();
			User.Singleton.Suspend();
			StartCoroutine(NetworkUpdate());
		}
		else
		{
			Debug.Log("OnApplicationPause Actived");
			if(User.Singleton!=null)
				User.Singleton.ActivedToIdle();
			StartCoroutine(NetworkUpdate());
			if (User.bBuyThingsInIos)
			{
				User.bBuyThingsInIos = false;
			}
			else
			{
				MaskingTable table = gameObject.AddComponent<MaskingTable>();//
				table.SetCallback(gameObject, "ActivedToIdle");
			}
			
		}
	}
}
