using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using System.Collections.Generic;
using LilyHeart;

public class SetupDialog : MonoBehaviour {
	
	public GameObject buttonsParent;
	public GameObject animationParent;
	public GameObject aboveLabel;
	public GameObject underLabel;
	public GameObject FeedBackLabel;
	public GameObject AccountLabel;
	public UISlicedSprite LogoutObject;
	public UISlicedSprite btnBack;
	public List<GameObject> SetupTFRadioButtons;
	private List<bool> SetupSettingValue;
	
	private const string AboutUrl = "http://www.toufe.com/product/poker";
	private const string MICRO_BLOG="http://weibo.com/kxpoker";
	
	enum Status
	{
		Default,
		Feedback,
		ChangePassword,
		Tutorial
	}
	private Status currentStatus = Status.Default;
	// Use this for initialization
	void Start () {
		User.UserDataChangedEvent += UserDataChanged;
		PhotonClient.LoginUserNameOrPasswordErrorEvent += LoginUserNameOrPassWordError;
		setFirstRow();
		SetButtons();
		if(MusicManager.Singleton.BgAudio!=null)
		{
			StartCoroutine(MusicManager.Singleton.BgFadeOut(0.5f));
		}
	}
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	void UserDataChanged()
	{
		setFirstRow();
		SetButtons();
	}
	void onBack(GameObject btn)
	{
		fadePanel fpanel = gameObject.GetComponent<fadePanel>();
		Animation animation = animationParent.GetComponent<Animation>();
		switch(currentStatus)
		{
		case Status.Default:
			if(MusicManager.Singleton.BgAudio!=null)
			{
				StartCoroutine(MusicManager.Singleton.BgFadeIn(1f));
			}
			
			if (btn)
			{
				BoxCollider box = btn.GetComponent<BoxCollider>();
				box.enabled = false;
				Destroy(btn);
			}
			
			fpanel.fadeOut(null);
			break;
		case Status.Feedback:
			animation.Play("Feedbackanim_back");
			break;
		case Status.ChangePassword:
			PhotonClient.SaveInfoSuccessEvent -= ChangePasswordSuccess;
			animation.Play("setupAnim_back");
			break;
		case Status.Tutorial:
			animation.Play("studyback");
			break;
		}
		currentStatus = Status.Default;
		
	}
	private void setFirstRow()
	{
		if(AccountLabel!=null)
		{
			UILabel label = AccountLabel.GetComponent<UILabel>();
			if(label!=null)
			{
				if (User.Singleton.UserData.UserType == DataPersist.UserType.Guest)
					label.text = User.Singleton.UserData.NickName;
				else 
					label.text = User.Singleton.UserData.Mail;
			}
		}
		if(LogoutObject!=null)
		{
			if (User.Singleton.UserData.UserType == DataPersist.UserType.Guest)
				LogoutObject.spriteName = "STBtn_launch";
			else 
				LogoutObject.spriteName = "STBtn_Esc";
		}
		if(btnBack!=null)
		{
			if (User.Singleton.UserData.UserType == DataPersist.UserType.Guest)
				btnBack.spriteName = GlobalManager.Singleton.ApplicationType==AppType.Normal?"STBtn_mail":"STBtn_Reg";
			else 
				btnBack.spriteName = "STBtn_changepw";
		}
	}
	public void SetButtons()
	{
		InitSetupSettingValueList();
		int nIndex = -1;
		if(SetupTFRadioButtons.Count!=SetupSettingValue.Count)
		{
			return;
		}
		if(SetupTFRadioButtons!=null&&SetupTFRadioButtons.Count>0)
		{
			foreach(GameObject btn in SetupTFRadioButtons)
			{
				nIndex ++;
				TF_RadioButtons MyTFRadioButtons = btn.GetComponent<TF_RadioButtons>();
				if(MyTFRadioButtons!=null&&nIndex>=0&&nIndex<SetupSettingValue.Count)
				{
					MyTFRadioButtons.InitRadioButtonStatus(SetupSettingValue[nIndex]);
				}
			}
		}
	}
	private void InitSetupSettingValueList()
	{
		if(SetupSettingValue == null) SetupSettingValue = new List<bool> ();
		if(SetupSettingValue!=null&&SetupSettingValue.Count>0)
			SetupSettingValue.Clear();
		SetupSettingValue.Add(SettingManager.Singleton.PokerPointer);
		SetupSettingValue.Add(SettingManager.Singleton.Sound);
		SetupSettingValue.Add(SettingManager.Singleton.Music);
		SetupSettingValue.Add(SettingManager.Singleton.ChatBubble);
		SetupSettingValue.Add(SettingManager.Singleton.CallInRoom);
		SetupSettingValue.Add(SettingManager.Singleton.FriendActivityNotification);
		SetupSettingValue.Add(SettingManager.Singleton.SystermNotification);
	}
	public void realSetHandStrength(bool bValue)
	{
		SettingManager.Singleton.PokerPointer = bValue;
		SettingManager.Singleton.SaveFile();
		Debug.Log("SettingManager.Singleton.PokerPointer:"+SettingManager.Singleton.PokerPointer.ToString());
	}
	void setSound(bool bValue)
	{
	    SettingManager.Singleton.Sound = bValue;
		SettingManager.Singleton.SaveFile();
		Debug.Log("SettingManager.Singleton.Sound:"+SettingManager.Singleton.Sound.ToString());
	}
	void setMusic(bool bValue)
	{
		SettingManager.Singleton.Music = bValue;
		SettingManager.Singleton.SaveFile();
		Debug.Log("SettingManager.Singleton.Music:"+SettingManager.Singleton.Music.ToString());
	}
	void setBubble(bool bValue)
	{
		SettingManager.Singleton.ChatBubble = bValue;
		SettingManager.Singleton.SaveFile();
		Debug.Log("SettingManager.Singleton.ChatBubble:"+SettingManager.Singleton.ChatBubble.ToString());
	}
	void setCall(bool bValue)
	{
		GameObject swd = transform.parent.GetComponent<RoomBackground>().SpeakWorldDlg;
		GameObject swtl = transform.parent.GetComponent<RoomBackground>().SpeakWorldTextList;
		SettingManager.Singleton.CallInRoom = bValue;
		if(swd!=null)
			swd.SetActiveRecursively(bValue);
		if(swtl!=null)
			swtl.SetActiveRecursively(bValue);
		SettingManager.Singleton.SaveFile();
		Debug.Log("SettingManager.Singleton.CallInRoom:"+SettingManager.Singleton.CallInRoom.ToString());
	}	
	public void realSetFriendActivityNotification(bool bValue)
	{
		SettingManager.Singleton.FriendActivityNotification = bValue;
		User.Singleton.UserData.FriendNotify = bValue;
		SettingManager.Singleton.SaveFile();
		PhotonClient.Singleton.SystemSetting();
		Debug.Log("SettingManager.Singleton.FriendActivityNotification:"+SettingManager.Singleton.FriendActivityNotification.ToString());
	}
	public void realSetSystemNotification(bool bValue)
	{
		SettingManager.Singleton.SystermNotification = bValue;
		User.Singleton.UserData.SystemNotify = bValue;
		SettingManager.Singleton.SaveFile();
		PhotonClient.Singleton.SystemSetting();
		Debug.Log("SettingManager.Singleton.SystermNotification:"+SettingManager.Singleton.SystermNotification.ToString());
	}
	void onFeedback()
	{
		currentStatus = Status.Feedback;
		Animation animation = animationParent.GetComponent<Animation>();
		animation.Play("Feedbackanim");
	}
	void onAbout()
	{
		UtilityHelper.ShowWebByUrl(AboutUrl);
	}
	void onMicroBlog()
	{
		UtilityHelper.ShowWebByUrl(MICRO_BLOG);
	}
	public void onChangePassword()
	{
		currentStatus = Status.ChangePassword;
		PhotonClient.SaveInfoSuccessEvent += ChangePasswordSuccess;
		Animation animation = animationParent.GetComponent<Animation>();
		animation.Play("setupAnim");
	}
	void onChangePasswordConfirm()
	{
		UILabel aLabel = aboveLabel.GetComponent<UILabel>();
		UILabel uLabel = underLabel.GetComponent<UILabel>();
		if (aLabel.text != uLabel.text)
		{
			Debug.Log("the string is not equal!");
			UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"),  LocalizeHelper.Translate("TWO_PASSWORD_NOT_EQUAL_DESCRIPTION"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		}
		else if (aLabel.text.Length < 6 || aLabel.text.Length > 12)
		{
			Debug.Log("string should between 6 and 12!");
			UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"),  LocalizeHelper.Translate("LOGIN_PASSWORD_LENGTH_ERROR"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		}
		else
		{
			Debug.Log("Success!");
			User.Singleton.Save(User.Singleton.UserData.Avator, aLabel.text.getMD5(), User.Singleton.UserData.LivingRoomType);
		}
	}
	void ChangePasswordSuccess()
	{
		PhotonClient.SaveInfoSuccessEvent -= ChangePasswordSuccess;
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new ChangePasswordSucces());
		PopUpTips();
	}
	void onFeedbackConfirm()
	{
		string feedback = FeedBackLabel.GetComponent<UILabel>().text;
		if(!string.IsNullOrEmpty(feedback.Trim())&&(feedback!=LocalizeHelper.Translate("FEEDBACK_TIP_DESCRIPTION")))
			HelpManager.Feedback(feedback);
		Invoke("AfterFeedBackSend",1f);
	}
	void AfterFeedBackSend()
	{
		string feedback = FeedBackLabel.GetComponent<UILabel>().text;
		User.Singleton.MessageOperating = true;
		GlobalScript.ScriptSingleton.CurrentInfos.Enqueue(new FeedBackTipDialog(PhotonClient.Singleton.Connected,feedback));
		PopUpTips();
	}
	void PopUpTips()
	{
		GameObject prefab=Resources.Load("prefab/tips2") as GameObject;
		GameObject item ;
		item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		item.transform.parent=transform;
		item.transform.localPosition=new Vector3(0,200,-21);
		item.transform.localScale =new Vector3(1,1,1);
	}
	void OnDestroy()
	{
		User.UserDataChangedEvent -= UserDataChanged;
		PhotonClient.LoginUserNameOrPasswordErrorEvent -= LoginUserNameOrPassWordError;
	}
	void LoginUserNameOrPassWordError()
	{
		WebBindingHelper.ExcuteJS("hideloading();");
		//UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
	}
}
