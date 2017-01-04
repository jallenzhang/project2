using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AssemblyCSharp.Helper;
using LilyHeart;

public class RegLogic : loginBase {

	// Use this for initialization
	public UIInput email;
	public UIInput regName;
	public UIInput pwd;
	public GameObject parentItem;
	
	public GameObject mail;
	public GameObject password;
	public GameObject nickName;
	
	void Start () {
		this.loginOK=false;
		LoadLanguage();
	}
	
	private static void LoadLanguage()
	{
		Debug.Log("In function LoadLanguage");
		string languageFile = Application.systemLanguage.ToString();// + ".xml";
		TextAsset languageTextAsset = null;
		string filePath = "Languages/"+languageFile;
		
		languageTextAsset = (TextAsset)Resources.Load(filePath);
		
		if (languageTextAsset == null)
		{
			languageTextAsset = (TextAsset)Resources.Load("Languages/Chinese.xml");
			
			if (languageTextAsset == null)
				return;
		}
			
		LocalizeHelper.CurrentLanguage = languageTextAsset;
		LocalizeHelper.LoadTranslation(ref languageFile, languageTextAsset);
	}
	
	// Update is called once per frame
	void Update () 
	{
		checkLogin();
	}
	
	void OnRegister(string values)
	{
		Debug.Log(values);
		string[] opParams=values.Split(',');
		GlobalManager.Singleton.ParamEmail=opParams[0];
		GlobalManager.Singleton.ParamPassword=opParams[1];
		
		User.Singleton.CheckEmail(GlobalManager.Singleton.ParamEmail);
//		UILabel ulMail = (UILabel)mail.GetComponent("UILabel");
//		UILabel ulPassword = (UILabel)password.GetComponent("UILabel");
//		UILabel ulNickName = (UILabel)nickName.GetComponent("UILabel");
//		
//		if (!checkEmain(ulMail.text))
//		{
//			return;
//		}
//		
//		if (!checkPwd(ulPassword.text))
//			return;
//		
//		if (!checkNickName(ulNickName.text))
//			return;
//		
//		Debug.Log("nickname: " + ulNickName.text);
//		Debug.Log("mail: " + ulMail.text);
//		
//		//todo: popup webview
//		
//		//GameObject.Find("UI Root (2D)/Camera/Anchor").AddComponent<ShowLoadingTable>();
//		User.Singleton.Register(ulNickName.text, ulMail.text, ulPassword.text.getMD5(), DeviceTokenHelper.myDeviceToken, (byte)PlayerAvator.Guest);
//		//UtilityHelper.PreConditionFinished(true); //should move to somewhere to judge the webview reuslt
	}
	
	 void OnShowRegisterView()
	{
		Debug.Log ("In OnShowRegisterView()");
		//WebBindingHelper.ShowRegisterWebView(gameObject.name,"OnRegister");
		PlatformHelper.Register(gameObject.name,"OnRegister");
	}
}
