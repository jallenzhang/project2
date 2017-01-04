using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using System;
using LilyHeart;

public class roombtnAction : MonoBehaviour {
	private static roombtnAction _roombtnAction;
	public static roombtnAction Singleton 
	{
		get {return _roombtnAction;}
	}
	
	public GameObject item;
	
	public GameObject parentItem;
	private AsyncOperation async = null;
	private UserData roomDataBeforeInvite = null;
	
	public static event Action KickByVip;
	public static event Action DidRegisterInSetupSuccessEvent;
	
	public roombtnAction(){
		PMAchive.ResetAchivement();
	}
	
	void Awake()
	{
		_roombtnAction=this;
	}
	
	// Use this for initialization
	void Start () 
	{	
		if (GlobalManager.Singleton.AwardChip > 0)
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new DailyChipGiftDialog(GlobalManager.Singleton.AwardChip.ToString()));
			PopupChipGift();
			GlobalManager.Singleton.AwardChip = 0;
		}
		if (User.Singleton.UserData.Avator == 0)
		{
			PopUpAvatorDialog();
		}
	}
	
	void SetProgress()
	{
		if (async != null)
		{
			Debug.Log("Progress is: " + async.progress);
			LoadingPercentHelper.Progress = async.progress;
		}
	}
	
	// Update is called once per frame
	void Update () {
		SetProgress();
		if(GlobalManager.Singleton.Messages.Count>0 && !User.Singleton.MessageOperating)
		{
			Debug.Log("Message In");
			User.Singleton.CurrentMessage = GlobalManager.Singleton.Messages.Dequeue();
			
			if (User.Singleton.CurrentMessage.GetType() ==  typeof(RequestFriendMessage))
			{
				User.Singleton.MessageOperating = true;
				PopupMessage();
			}
			else if (User.Singleton.CurrentMessage.GetType() ==  typeof(InviteFriendMessage))
			{
				InviteFriendMessage inviteFriendMessage=(InviteFriendMessage)User.Singleton.CurrentMessage;
				
				if(inviteFriendMessage.DestinationId==Room.Singleton.RoomData.Owner.UserId&&
					inviteFriendMessage.DestinationType==InviteFriendMessage.DESTINATION_ROOM)
				{
					return;
				}
				
				roomDataBeforeInvite = Room.Singleton.RoomData.Owner;

				User.Singleton.MessageOperating = true;
				if (inviteFriendMessage.DestinationType == InviteFriendMessage.DESTINATION_GAME)
				{
					PhotonClient.JoinGameFinished -= JoinGameFinished;
					PhotonClient.JoinGameFinished += JoinGameFinished;
				}
				Debug.Log("to PopupMessage");
				PopupMessage();
			}
			else if (User.Singleton.CurrentMessage.GetType() ==  typeof(AddFriendMessage))
			{
				User.Singleton.MessageOperating = true;
				Debug.Log("AddFriendMessage " + ((PlayerMessage)User.Singleton.CurrentMessage).NickName);
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new FriendAddedDialog(((PlayerMessage)User.Singleton.CurrentMessage).NickName));
				PopUpTips();
			}
			else if (User.Singleton.CurrentMessage.GetType() ==  typeof(SendChipMessage))
			{
				User.Singleton.MessageOperating = true;
				Debug.Log("SendChipMessage");
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SendChipDialog(((PlayerMessage)User.Singleton.CurrentMessage).NickName, ((PlayerMessage)User.Singleton.CurrentMessage).Title));
				PopupChipGift();
			}
            else if (User.Singleton.CurrentMessage.GetType() == typeof(AchievementMessage)) {
                PopUpAchivement();
            }
			else if (User.Singleton.CurrentMessage.GetType() == typeof(KickedByVIPMessage)) {
				User.Singleton.MessageOperating = true;
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new KickedByVIPDialog(User.Singleton.CurrentMessage.Content, gameObject, "PopupPropDialog"));
				PopUpTips();
				
				if(KickByVip!=null)
				{
					KickByVip();
				}
				////
			}
			else if (User.Singleton.CurrentMessage.GetType() == typeof(RegisterMessage)) {
				
				if(DidRegisterInSetupSuccessEvent!=null)
				{
					DidRegisterInSetupSuccessEvent();
				}
             } 
		}
		
		
		if(GlobalManager.Singleton.Notifications.Count>0)
		{			
			Notification notification=GlobalManager.Singleton.Notifications.Peek();			
			if((notification.Target&TargetType.Room)==TargetType.Room)
			{
				if(notification is CheckEmailNotification)
				{
					CheckEmailNotification checkEmailNotification=GlobalManager.Singleton.Notifications.Dequeue() as CheckEmailNotification;
					if(checkEmailNotification.IsSuccess)
					{
						if(User.Singleton.UserData.Avator<=0)
							PopUpAvatorDialog();
						else
						{
							WebBindingHelper.CloseWebView();
							PhotonClient.RegisterOrUpgradeEvent += UtilityHelper.OnUpgradeFinish;
							User.Singleton.GuestUpgrade(User.Singleton.UserData.NickName, GlobalManager.Singleton.ParamEmail, GlobalManager.Singleton.ParamPassword.getMD5(), DeviceTokenHelper.myDeviceToken, User.Singleton.UserData.Avator);
						}
					}
					else
					{
						WebBindingHelper.ExcuteJS("hideloading();");
						//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
					}
				}
				else if(notification is The3rdFirstLoginNotification)
				{
					PopUpAvatorDialog2();
					GlobalManager.Singleton.Notifications.Dequeue();
				}
			}			
		}
		
		ForgotLogic.SendEmailMsgToWebView();
		
		StartCoroutine(NetworkUpdate());
	}
	
	void PopUpAvatorDialog()
	{
		WebBindingHelper.CloseWebView();
		GameObject prefab=Resources.Load("prefab/chooseAvatar2") as GameObject;
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
	void PopUpAvatorDialog2()
	{
		WebBindingHelper.CloseWebView();
		GameObject prefab=Resources.Load("prefab/chooseAvatar") as GameObject;
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
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	public void ReceiveSameAccountLoginEvent()
	{
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new SameAccountLoginDialog());
		PopUpTips();
	}
	
	public void ReJionRoomWhenChipNotEnough()
	{
//		if(roomDataBeforeInvite!=null)
//		{
//			 User.Singleton.JoinRoom(roomDataBeforeInvite.UserId);
//			 Debug.Log("roomDataBeforeInvite.UserId"+roomDataBeforeInvite.UserId);
//			 roomDataBeforeInvite = null;
//		}
//		else
//		{
//			if(PhotonClient.Singleton.PriRoomUser!=null)
//			{
//				User.Singleton.JoinRoom(PhotonClient.Singleton.PriRoomUser.UserId);
//				Debug.Log("PriRoomOwerUser.UserId"+PhotonClient.Singleton.PriRoomUser.UserId);
//			}
//			else
//				User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
//		}
		User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
	}
	#region POPUP DIALOGS  
//	public void PopupChooseRoleDialog()
//	{
//		//disabledAllBtns();
//		GameObject prefab=Resources.Load("prefab/chooseRolePrefab") as GameObject;
//		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
//		
//		item.transform.parent=transform;
//		item.transform.localPosition=new Vector3(0,0,-17);
//		item.transform.localScale =new Vector3(1,1,1); 
//	}
	
	void PopUpPromtRegTips()
	{
		GameObject prefab=Resources.Load("prefab/PromtRegTips") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(-300,-160,-20);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void PopupChipGift()
	{
		GameObject prefab=Resources.Load("prefab/DailyGiftTable") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,10,-20);
		item.transform.localScale =new Vector3(1,1,1);
		
		AudioSource audioSource=item.AddComponent<AudioSource>();
		SoundHelper.PlaySound("Music/Other/DailySendChip",audioSource);
	}
	
	void PopUpTipsEx()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=parentItem.transform;
		item.transform.localPosition=new Vector3(0,200,-20);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void PopupMessage()
	{
		GameObject prefab=Resources.Load("prefab/PromptMessageTable") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		InviteFriendMessage inviteFriendMessage=User.Singleton.CurrentMessage as InviteFriendMessage;
		
		if (inviteFriendMessage != null)
		{
			if (GlobalManager.Singleton.version == KindOfVersion.Basic 
				&& inviteFriendMessage.DestinationId!=Room.Singleton.RoomData.Owner.UserId
				&& inviteFriendMessage.DestinationType==InviteFriendMessage.DESTINATION_ROOM)
			{
				friendDialog dialog = item.GetComponent<friendDialog>();
				dialog.acceptBtnActived = false;
			}
		}
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,-20);
		item.transform.localScale =new Vector3(1,1,1);
		
	}
	
	void PopupPlayInfoDialog()
	{	
		Transform btnPlayerInfor=transform.FindChild("Player Prefab");
		if(btnPlayerInfor==null)
		{
			
			GameObject prefab=Resources.Load("prefab/Player Prefab") as GameObject;
			item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
			item.transform.parent=transform;
			item.name="Player Prefab";
			item.transform.localPosition=new Vector3(0,0,-2);
			item.transform.localScale =new Vector3(1,1,1);
		}
	}
	
	void PopupBuyChipsFromBackground()
	{
		GameObject prefab=Resources.Load("prefab/BuyChipsTable") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		 
		item.transform.localPosition=new Vector3(0,0,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void PopupBuyChipsFromInfoDialog()
	{
		GameObject prefab=Resources.Load("prefab/BuyChipsTable") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform.FindChild("ButtonName");
		item.transform.localPosition=new Vector3(207,-85,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	public void PopupFriendListDialog()
	{
		Transform friendList= transform.FindChild("friendList");
		if(friendList==null)
		{
 			GameObject prefab=Resources.Load("prefab/cardFriend/FriendList") as GameObject;
			item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 			item.transform.parent=transform;
			item.name="friendList";
			item.transform.localPosition=new Vector3(0,0,-13);
			item.transform.localScale =new Vector3(1,1,1);
		}
	}
	
	
	
	void PopupBuySceneDialog()
	{
		GameObject prefab=Resources.Load("prefab/buySence") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		//item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,0);
		item.transform.localScale =new Vector3(0.003169572f,0.003169572f,0.003169572f);
		item.name="BuySence";
	}
	
	void PopupSetupDialog()
	{
		GameObject prefab = null;
		if (GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
			prefab=Resources.Load("prefab/setup2_91") as GameObject;
		else
			prefab=Resources.Load("prefab/setup2") as GameObject;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void DidTryJointGameEvent(bool flag)
	{
		Debug.Log("DidTryJointGameEvent:"+flag);
		User.bShowGameSetting_Flag = false;
		UtilityHelper.CloseMaskingTable();
		PhotonClient.TryJointGameEvent-=DidTryJointGameEvent;
		
		UtilityHelper.PreConditionFinished(!flag);
		twoEvents = false;
	 

	}
	
	void tryJoinGame()
	{
		PhotonClient.Singleton.TryJoinGame();
	}
	
	private bool twoEvents = false;
	void PopupGameSettingDialog()
	{
		lock(lockObj)
		{
			if (!User.bShowGameSetting_Flag)
			{
				User.bShowGameSetting_Flag = true;
				PhotonClient.TryJointGameEvent+=DidTryJointGameEvent;
				PhotonClient.JoinGameFinished+=JoinGameFinished;
				twoEvents = true;
				MaskingTable table = gameObject.AddComponent<MaskingTable>();
				table.SetCallback(gameObject, "tryJoinGame");
				PhotonClient.Singleton.TryJoinGame();
			}
		}
 	}
	
	void PopupHonorDialog()
	{
		GameObject prefab=Resources.Load("prefab/TitlePrefab") as GameObject;
		
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
						
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,-1000,-10);
		item.transform.localScale =new Vector3(1,1,1);
		
		//“time”:3, “loopType”:”pingPong”, “delay”:1
		iTween.MoveTo(item,iTween.Hash("y",0,"loopType","none","time",0.2f,"easetype","easeInOutQuad"));
		
	}
	
	void PopupAchivementDialog()
	{
		UserData userData;
		if(User.Singleton.CurrentPlayInfo == 2)
			userData = Room.Singleton.RoomData.Owner;
		else
			userData = User.Singleton.UserData;		GameObject prefab=Resources.Load("prefab/Achivement") as GameObject;
 		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
 		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,-600,-10);
		item.transform.localScale =new Vector3(1,1,1);
 		iTween.MoveTo(item,iTween.Hash("y",0,"loopType","none","time",0.2f,"easetype","easeInOutQuad"));
	}

    public void PopUpAchivement()
    {
        GameObject prefab = Resources.Load("prefab/Achivementbar") as GameObject;
        GameObject item = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		
		// set layer
		string myLayerStr = "Ignore Raycast";		
		var child = GameObject.Find("buySence(Clone)");
		if(null != child){
			myLayerStr = "Default";
		} 
		item.layer = LayerMask.NameToLayer(myLayerStr);

        PMAchive chaScript = item.GetComponent<PMAchive>();
        chaScript.item = item;
		chaScript.localPosition = new Vector3(0, 0, -450);
		
        item.transform.parent = transform;
    }
	
	void PopupRegTable()
	{
		GameObject prefab=Resources.Load("prefab/RegTable_2") as GameObject;
 		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void PopupSpeakWorldTable()
	{
		GameObject prefab=Resources.Load("prefab/SpeakWorld") as GameObject;
 		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,-12);
		item.transform.localScale =new Vector3(1,1,1);
	}
	
	void PopupPropDialog()
	{
		GameObject prefab=Resources.Load("prefab/props") as GameObject;
 		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
			
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,0,-2);
		item.transform.localScale =new Vector3(1,1,1);
	}

	#endregion
	
	/// <summary>
	/// Buttons the card firend.
	///  show cardFriends  
	/// </summary>
	/// 
	///
	void btnSpeak()
	{
		PopupSpeakWorldTable();
	}
	
	void btnPlayerInfor()
	{
		User.Singleton.CurrentPlayInfo = 1;
		UtilityHelper.PreConditionFinished(true);
	}
	
	void btnFriendInfo()
	{
		User.Singleton.CurrentPlayInfo = 2;
		MaskingTable table = gameObject.AddComponent<MaskingTable>();
		if(Room.Singleton.RoomData.Owner.UserId!=null)
		{
			PhotonClient.Singleton.QueryUserById(Room.Singleton.RoomData.Owner.UserId);
			PhotonClient.ClickFriendInfoEvent+=UpdateFriendInfo;
		}
	}
	void UpdateFriendInfo()
	{
		UtilityHelper.PreConditionFinished(true);
		PhotonClient.ClickFriendInfoEvent-=UpdateFriendInfo;
		UtilityHelper.CloseMaskingTable();
	}
	void btnBuyChipsFromBackground()
	{
		PopupBuyChipsFromBackground();
	}
	
	void btnBuyChipsFromInfo()
	{
		PopupBuyChipsFromInfoDialog();
	}
	
//	void OnEnable()
//	{
//		EtceteraManager.purcharseFinished += purcharseFinished;
//	}
//	
//	void OnDisable()
//	{
//		EtceteraManager.purcharseFinished -= purcharseFinished;
//	}
	
	void startBuyChips(int index, int price)
	{
#if UNITY_IPHONE
		if( Application.platform == RuntimePlatform.IPhonePlayer && MyVersion.CurrentPlatform == DevicePlatform.Normal)
		{
			EtceteraBinding.etceteraPurchaseWithIndex(index);
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
				EtceteraBinding.etceteraPurchaseWithUnityCall91Pay(index.ToString(), price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
			else
				EtceteraBinding.etceteraPurchaseWithUnityCallAlixPay(index.ToString(), price, User.Singleton.UserData.UserId, UtilityHelper.GetChannelId());
		}
#endif
	}
	void btnBuyChips(GameObject btn)
	{
		//UserType.Guest-Buy remove btnBuyChips limit
//		if (User.Singleton.UserData.UserType == UserType.Guest)
//		{
//			Player.Instance.MessageOperating = true;
//			((Player)Player.Instance).CurrentInfos.Enqueue(new GuestBuyChipsLimitedDialog());
//			parentItem = gameObject.transform.parent.gameObject;//here get front button game object
//			PopUpTips();
//		}
		if (User.Singleton.UserData.UserType == UserType.Guest&&GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NinetyOneGuestBuyTipDialog(gameObject.name,"OnRegister",true));
			PopUpTips();
		}
		else
		{
			switch(btn.name)
			{
			case "Button_1":
				startBuyChips(6800000, 648);
				break;
			case "Button_2":
				startBuyChips(3400000, 328);
				break;
			case "Button_3":
				startBuyChips(700000, 68);
				break;
			case "Button_4":
				startBuyChips(300000, 30);
				break;
			case "Button_5":
				startBuyChips(250000, 25);
				break;
			case "Button_6":
				startBuyChips(120000, 12);
				break;
			case "Button_7":
				startBuyChips(60000, 6);
				break;
			}
		}
	}
	
	public void onBuyItemResponse(bool success, ItemType itemType)
	{
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new BuyItemResponseDialog(success));
		PopUpTips();
		
		if (itemType == ItemType.Chip)
		{
			AudioSource audioSource = gameObject.GetComponent<AudioSource>();
			if (audioSource == null)
			{
				audioSource = gameObject.AddComponent<AudioSource>();
			}
			SoundHelper.PlaySound("Music/Other/DailySendChip",audioSource);
		}
	}
	
	//setup dialog logout button
	void onLogout()
	{
		if (User.Singleton.UserData.UserType == UserType.Guest)
		{
//			switch(GlobalManager.Singleton.ApplicationType)
//			{
//			case AppType.Normal:
//				WebBindingHelper.ShowLoginWebView(gameObject.name,"OnLogin");
//				break;
//			case AppType.NinetyOne:
//				NdComHelper.Login(gameObject.name,"OnLogin");
//				break;
//			}
			PlatformHelper.Login(gameObject.name,"OnLogin");
		}
		else
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new LogoutConfirmDialog());
			PopUpTips();
		}
	}
	
	//setup dialog register or changepassowrd button
	public void onBtnChange()
	{
		Debug.Log("onBtnChange " + User.Singleton.UserData.UserType.ToString());
		if (User.Singleton.UserData.UserType == DataPersist.UserType.Guest)
		{
//			switch(GlobalManager.Singleton.ApplicationType)
//			{
//			case AppType.Normal:
//				WebBindingHelper.ShowRegisterWebView(gameObject.name,"OnRegister");
//				break;
//			case AppType.NinetyOne:
//				NdComHelper.GuestRegist(gameObject.name,"OnRegister");
//				break;
//			}
			Debug.Log(gameObject.name);
			PlatformHelper.Upgrade(gameObject.name,"OnRegister");
		}
		else
		{
			//change password
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChangePasswordConfirmDialog(gameObject));
			PopUpTips();
		}
	}
	
	void OnLogin(string values)
	{
		Debug.Log (values);
//		string[] arr;
//		string email=string.Empty;
//		string password=string.Empty;
//		switch(GlobalManager.Singleton.ApplicationType)
//		{
//		case AppType.Normal:
//			arr=values.Split(',');
//			email=arr[0];
//			password=arr[1];
//			break;
//		case AppType.NinetyOne:
//			email=values;
//			password=string.Empty;
//			GlobalManager.Singleton.ParamEmail=email;
//			break;
//		}
//		User.Singleton.Login(email, password.getMD5(), DeviceTokenHelper.myDeviceToken);
		PlatformHelper.OnLogin(values);
		WebBindingHelper.CloseWebView();
	}
	
	void OnRegister(string values)
	{
//		string[] opParams;
//		switch(GlobalManager.Singleton.ApplicationType)
//		{
//		case AppType.Normal:
//			opParams=values.Split(',');
//			GlobalManager.Singleton.ParamEmail=opParams[0];
//			GlobalManager.Singleton.ParamPassword=opParams[1];
//			User.Singleton.CheckEmail(GlobalManager.Singleton.ParamEmail);
//			break;
//		case AppType.NinetyOne:
//			User.Singleton.GuestUpgrade(User.Singleton.UserData.NickName, 
//				values, 
//				string.Empty, 
//				DeviceTokenHelper.myDeviceToken, 
//				User.Singleton.UserData.Avator);
//			GlobalManager.Singleton.ParamEmail=string.Empty;
//			GlobalManager.Singleton.ParamPassword=string.Empty;
//			break;
//		}
		Debug.Log("KLLLLLLLLLLLLLLLL");
		PlatformHelper.OnUpgrade(values);
	}
	
	//setup dialog change system notification status
	void setSystemNotification(bool bValue)
	{
		if (!bValue)
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NotificationCloseConfirmDialog("System", bValue, gameObject));
			PopUpTips();
		}
		else
		{
			SetupDialog dialog = gameObject.GetComponent<SetupDialog>();
			dialog.realSetSystemNotification(bValue);
		}
	}
	//setup dialog change frirend notification
	void setFriendActivityNotification(bool bValue)
	{
		if (!bValue)
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new NotificationCloseConfirmDialog("Friend", bValue, gameObject));
			PopUpTips();
		}
		else
		{
			SetupDialog dialog = gameObject.GetComponent<SetupDialog>();
			dialog.realSetFriendActivityNotification(bValue);
		}
	}
	//setup dialog change handstrength
	void setHandStrength(bool bValue)
	{
		bool toSet = bValue;
		 
		//remove UserType.Guest HandStrength Limit
//		if (toSet && User.Singleton.UserData.UserType == UserType.Guest)
//		{
//			Player.Instance.MessageOperating = true;
//			((Player)Player.Instance).CurrentInfos.Enqueue(new GuestHandStrengthFailedDialog(gameObject));
//			PopUpTips();
//		}
//		else
//		{
			SetupDialog dialog = gameObject.GetComponent<SetupDialog>();
			dialog.realSetHandStrength(bValue);
//		}
	}
	void FriendListFinished(bool result)
	{
		Debug.Log("FriendListFinished aaaa");
		User.bShowFriend_Flag = false;
		PhotonClient.FriendListEvent -= FriendListFinished;
		UtilityHelper.CloseMaskingTable();
		UtilityHelper.PreConditionFinished(result);
	}
	
	/// <summary>
	/// Buttons the carf firend.
	/// </summary>
	///
	
	private  object lockObj = new object();
	
	void showFriendList()
	{
		PhotonClient.Singleton.FriendList();
	}
	
	void btnCarfFirend()
	{
		if (!User.bShowFriend_Flag)
		{
			lock(lockObj)
			{
				if (!User.bShowFriend_Flag)
				{
					User.bShowFriend_Flag = true;
					
					MaskingTable table = gameObject.AddComponent<MaskingTable>();
					table.SetCallback(gameObject, "showFriendList");
					PhotonClient.FriendListEvent += FriendListFinished;
					PhotonClient.Singleton.FriendList();
				}
			}
		}
	}
	
	//friend list panel: friend request 
	void btnRequestFriend()
	{
//		if (User.Singleton.UserData.UserType == UserType.Guest)
//		{
//			((Player)Player.Instance).CurrentInfos.Enqueue(new GuestRequestFriendForbiddenDialog());
//			parentItem = gameObject.transform.parent.parent.parent.parent.parent.gameObject;//here get front button game object
//			PopUpTipsEx();
//		}
//		else
//		{
		//remove UserType.Guest add friend limit
			cardFirend cf = gameObject.GetComponent<cardFirend>();
			cf.realRequestFriend();
//		}
	}
	
	void btnGotoHome()
	{
		PhotonClient.Singleton.LeaveRoom();
		PhotonClient.Singleton.JoinRoom(User.Singleton.UserData.UserId);
		Debug.Log("!!!!!!! btnGotoHome at roombtnAction");
	}
	
	void btnSences()
	{
		PopupBuySceneDialog();
	}
	
	void unRegsiterEvent()
	{
		GameObject chactorManager = GameObject.Find("ChactorsManage");
		if (chactorManager != null)
			chactorManager.GetComponent<RoomChactorManage>().UnRegseterEvent();
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
	 
	void match()
	{
		unRegsiterEvent();
		transform.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("GamblingInterface_game");
	}
 	void GotoGame()
	{
		unRegsiterEvent();
		transform.parent.gameObject.AddComponent<ShowLoadingTable>();
		async = Application.LoadLevelAsync("GamblingInterface_Title");
	}
	
	public void GotoHome()
	{
		unRegsiterEvent();
		transform.parent.gameObject.AddComponent<ShowLoadingTable>();
		if (GlobalManager.Singleton.version == KindOfVersion.Ultimate)
			async = Application.LoadLevelAsync("Background");
		else
			async = Application.LoadLevelAsync("Background_simple");
	}
	
	void onSetup()
	{
		
		if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
		{
			UtilityHelper.PreConditionFinished(true);
		}
		else
		{
			UtilityHelper.PreConditionFinished(false);
		}
	}
	
	void btnStartGame()
	{
		if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
		{
			if (User.Singleton.UserData.Level < 3)
			{
				User.Singleton.MessageOperating = true;
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new LevelLimitedDialog());
				PopUpTips();
				UtilityHelper.PreConditionFinished(false);
				return;
			}
			
			if (User.Singleton.UserData.Chips < 200)
			{
				User.Singleton.MessageOperating = true;
				GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("TIPS_START_GAME_CHIP_NOT_ENOUGH_TITLE"),null,string.Empty));
				PopUpTips();
				UtilityHelper.PreConditionFinished(false);
				return;
			}
			
			if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
				PopupGameSettingDialog();
		}
		else
		{
			UtilityHelper.PreConditionFinished(false);
		}
	}
	
	void btnFastStartGame(GameObject btn)
	{
		btn.GetComponent<BoxCollider>().enabled=false;
		
		if (User.Singleton.UserData.Chips < 20)
		{
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("TIPS_START_GAME_CHIP_NOT_ENOUGH_TITLE"),null,string.Empty));
			PopUpTips();
			btn.GetComponent<BoxCollider>().enabled=true;
			return;
		}
		LeeTestFadeOut();
	}
	
	void MyFastGotoGame(){		
		JoinGamblingSettingInfor.Singleton.isFastStart=true;
		PhotonClient.Singleton.isFastStart = true;
		PhotonClient.Singleton.LeaveRoom();
		GotoGame();
	}
	
	void JoinGameFinished(bool iswork,TypeState gamesate)
	{
		User.bShowGameSetting_Flag = false;
		UtilityHelper.CloseMaskingTable();
		Debug.LogWarning("JoinGameFinished");
		PhotonClient.JoinGameFinished-=JoinGameFinished;
		 // set to false, can click join game again
		Debug.Log("Error Code is: " + PhotonClient.Singleton.ErrorMessage);
		if (PhotonClient.Singleton.ErrorMessage == ErrorCode.Sucess.ToString())
		{
 
			JoinGamblingSettingInfor.Singleton.haveDone=true;
			GotoGame();
		}
		else if ((PhotonClient.Singleton.ErrorMessage == ErrorCode.TableNotExist.ToString()
			|| PhotonClient.Singleton.ErrorMessage == ErrorCode.GameIdNotExists.ToString()) && !twoEvents)
		{
 
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new TableNotExistDialog());
			PopUpTips();
		}
		else if ((PhotonClient.Singleton.ErrorMessage == ErrorCode.ChipsNotEnough.ToString()) && !twoEvents)
		{
			Debug.Log("PhotonClient.Singleton.ErrorMessage == ErrorCode.ChipsNotEnough.ToString()");
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChipsNotEnoughDialog(LocalizeHelper.Translate("TIPS_JOIN_GAME_CHIP_NOT_ENOUGH_TITLE"),gameObject,"ReJionRoomWhenChipNotEnough"));
			PopUpTips();
		}
		else if ((PhotonClient.Singleton.ErrorMessage == ErrorCode.GameFull.ToString()) && !twoEvents)
		{
 
			User.Singleton.MessageOperating = true;
			GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new TableFullErrorDialog());
			PopUpTips();
		}
		bJoiningRoom = false;
	}
	
	private bool bJoiningRoom = false;
	void onJoinGame(GameObject btn)
	{
 		if (bJoiningRoom)
			return;
		
		bJoiningRoom = true;
		Debug.Log("aaa onJoinGame");
		PhotonClient.JoinGameFinished+=JoinGameFinished;
		PhotonClient.Singleton.TryJoinGame();
	}
	
	/// <summary>
	/// Disableds all btns.
	/// </summary>
//	void disabledAllBtns()
//	{
//		BoxCollider[] boxcoolides = parentItem.GetComponentsInChildren<BoxCollider>();//parentItem
//		if(boxcoolides!=null)
//		{
//			foreach(BoxCollider one in boxcoolides)
//			{
//				one.enabled=false;
//			}
//		}
//	}
//	/// <summary>
//	/// Enables the allbtns.
//	/// </summary>
//	void enableAllbtns()
//	{
//		BoxCollider[] boxcoolides =item.transform.parent.GetComponentsInChildren<BoxCollider>();//item.transform.parent.
//		foreach(BoxCollider one in boxcoolides)
//		{
//			one.enabled=true;
//		}
//	}
	/// <summary>
	/// Buttons the show title.
	/// </summary>
	void btnShowTitle()
	{
		PopupHonorDialog();
	}
	
	void btnAchivement()
	{
		PopupAchivementDialog();
	}
	
	
	void destroyitem()
	{
 		if(item)
		{
	 	    Destroy(item);
 			
		}
	}
	
	/// <summary>
	/// Buttons the close.
	/// </summary>
	void btnClose()
	{
 		if(item!=null)
		{
			GameObject.DestroyImmediate(item);
 			//iTween.MoveTo(item,iTween.Hash("y",-10,"loopType","none","time",0.5f,"easetype","easeInOutQuad","oncomplete","destroyitem","oncompletetarget",gameObject));
 		}
	}
	
	void OnDestroy()
	{
		bJoiningRoom = false;
		PhotonClient.JoinGameFinished-=JoinGameFinished;
		PhotonClient.ClickFriendInfoEvent-=UpdateFriendInfo;
		_roombtnAction=null;
	}
	
	void LeeTestFadeOut(){		
		GameObject prefab = Resources.Load("prefab/FadeOutEffect") as GameObject;
        GameObject item = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;				
		item.transform.parent = transform;
		
		FadeOutHelper foH = item.GetComponent<FadeOutHelper>();
		foH.FinishBehaviour += MyFastGotoGame;
		
		PlayMakerFSM pm = item.GetComponent<PlayMakerFSM>();
		pm.enabled = true;
		pm.Fsm.Start();
	}
	
	void ShowTutorial()
	{
		//in my room
		if (User.Singleton.Friends == null || !User.Singleton.Friends.Exists(f => f.UserId == Room.Singleton.RoomData.Owner.UserId))
		{
			UtilityHelper.PreConditionFinished(true);

//			GameObject obj=Instantiate(Resources.Load("prefab/study 2"),new Vector3(0,0,0),Quaternion.identity) as GameObject;
//			obj.AddComponent<fadePanel>();
//			obj.layer=gameObject.layer;
//			obj.transform.parent=transform;
//			obj.transform.FindChild("Button_above").gameObject.layer = gameObject.layer;
//			obj.transform.FindChild("Button_back").gameObject.layer = gameObject.layer;
//			UIButtonMessage ubm = obj.transform.FindChild("Button_back").gameObject.AddComponent<UIButtonMessage>();
//			ubm.target = obj;
//			ubm.functionName = "fadeOut";
//			obj.transform.FindChild("Button_down").gameObject.layer = gameObject.layer;
//			obj.transform.localPosition=new Vector3(990,282,-12);
//			obj.transform.localScale =new Vector3(1,1,1);
		}
	}
}