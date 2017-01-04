using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using DataPersist;
using LilyHeart;

public class RegisterDialog : MonoBehaviour {
	
	public GameObject mailObject;
	public GameObject pswObject;
	public GameObject nickNameObject;
	
	private string oldNickName;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(NetworkUpdate());
	}
	
	IEnumerator NetworkUpdate()
	{
		PhotonClient.Singleton.Update();
		yield return null;
	}
	
	void SavePersonInfo()
	{
		User.Singleton.UserData.UserType = DataPersist.UserType.Normal;
		string accountInfo = User.Singleton.UserData.Mail + "|";
		accountInfo = accountInfo + User.Singleton.UserData.Password + "|";
		accountInfo = accountInfo + ((int)User.Singleton.UserData.UserType).ToString();
		FileIOHelper.WriteFile(FileType.Account, accountInfo);
	}
	
	void UpgradeFinished(bool result)
	{
		PhotonClient.RegisterOrUpgradeEvent -= UpgradeFinished;
		if (result)
		{
			SavePersonInfo();
			Debug.LogWarning("Resigter success");
			closeDialog();
			
		}
		else
		{
			User.Singleton.UserData.NickName = oldNickName;
			User.Singleton.UserData.UserType = UserType.Guest;
			User.Singleton.UserData.Mail = string.Empty;
			User.Singleton.UserData.Password = string.Empty;
			Debug.Log("@@@@@ "+ PhotonClient.Singleton.ErrorMessage);
#if UNITY_IPHONE
			//EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
#endif
			
#if UNITY_ANDROID
			//EtceteraAndroid.showAlert(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
#endif
		}
	}
	
	void onRegister(GameObject btn)
	{
		Debug.Log("onRegister");
		 
		UILabel mailLabel = mailObject.GetComponent<UILabel>();
		UILabel pswLabel = pswObject.GetComponent<UILabel>();
		UILabel nickLabel = nickNameObject.GetComponent<UILabel>();
		
		Debug.Log(mailLabel.text);
		Debug.Log(pswLabel.text);
		Debug.Log(nickLabel.text);
		if (!UtilityHelper.checkEmain(mailLabel.text))
		{
			return;
		}
		
		if (!UtilityHelper.checkPwd(pswLabel.text))
			return;
		
		string nickName = nickLabel.text;
		if(nickName.Length == 0||string.IsNullOrEmpty(nickName))
		{
			UtilityHelper.PopUpErrorDialog(LocalizeHelper.Translate("LOGIN_NICKNAME_LENGTH_ERROR"));
			return;
		}
		nickLabel.text = UtilityHelper.checkNickName(nickName,14);
		
		PhotonClient.RegisterOrUpgradeEvent -= UpgradeFinished;
		PhotonClient.RegisterOrUpgradeEvent += UpgradeFinished;
		oldNickName = User.Singleton.UserData.NickName;
		//User.Singleton.GuestUpgrade(nickLabel.text, mailLabel.text, pswLabel.text.getMD5(), DeviceTokenHelper.myDeviceToken,);
	}
	
	void closeDialog()
	{
		fadePanel fpanel = gameObject.GetComponent<fadePanel>();
		fpanel.fadeOut(null);
	}
	
	void OnApplicationQuit()
	{
		SavePersonInfo();
	}
}
