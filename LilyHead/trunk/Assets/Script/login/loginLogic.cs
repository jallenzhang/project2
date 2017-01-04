using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using DataPersist;
using AssemblyCSharp.Helper;
using LilyHeart;

public class loginLogic : loginBase {

	// Use this for initialization
	
	
	public UIInput LoginName;
	public UIInput pwd;
	public GameObject loginMail;
	public GameObject password;
	public GameObject GuestLoginButton;
	public GameObject parentItem;
	private bool bIsAddMasktable;
	
	void Start () {
		this.loginOK=false;
		bIsAddMasktable = false;
	}
	
	// Update is called once per frame
	void Update () {
		checkLogin();
	}
	
	void OnLogin(string values)
	{
		Debug.Log (values);
		if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			if(bIsAddMasktable)
			{
				UtilityHelper.CloseMaskingTable();
				bIsAddMasktable = false;
			}
		}
		PlatformHelper.OnLogin(values);
		
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
//			GlobalManager.Singleton.ParamEmail=email;
//			break;
//		}
//		//GameObject.Find("UI Root (2D)/Camera/Anchor").AddComponent<ShowLoadingTable>();
//		User.Singleton.Login(email, password.getMD5(), DeviceTokenHelper.myDeviceToken);
//		//Playtomic.Log.Play();
	}
	
	void OnLoginAsGuest()
	{
		GuestLoginButton.collider.enabled=false;
		switch(GlobalManager.Singleton.ApplicationType)
		{
		case AppType.Normal:
			LoginAsGuest();
			break;
		case AppType.NinetyOne:
			if(parentItem!=null)
			{
				parentItem.AddComponent<MaskingTable>();
				bIsAddMasktable = true;
			}
			NdComHelper.InitCancelMetodInLogin(gameObject.name,"CancelLogin");
			PlatformHelper.LoginAsGuest(gameObject.name,"LoginAsGuest","OnLogin");
			break;
		}
	}
	
	void LoginAsGuest()
	{
		GlobalManager.Singleton.InitGuestSignIn = true;
		GlobalManager.Singleton.ParamEmail =  string.Empty;
		GlobalManager.Singleton.ParamPassword = string.Empty;
		PopUpAvatorDialog();
	}
	
	void OnShowLoginView()
	{
		Debug.Log ("In OnShowLoginView()");
		if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			if(parentItem!=null)
			{
				parentItem.AddComponent<MaskingTable>();
				bIsAddMasktable = true;
			}
			NdComHelper.InitCancelMetodInLogin(gameObject.name,"CancelLogin");
		}
		
		ShowLoginViewMethod(gameObject.name);
	}
	
	public static void ShowLoginViewMethod(string gameObjectName){		
		PlatformHelper.Login(gameObjectName,"OnLogin");
	}
	
	void CancelLogin()
	{
		Debug.Log("loginLogic-CancelLogin");
		if(GuestLoginButton!=null)
			GuestLoginButton.collider.enabled=true;
		
		if(bIsAddMasktable)
		{
			UtilityHelper.CloseMaskingTable();
			bIsAddMasktable = false;
		}
		NdComHelper.InitCancelMetodInLogin(string.Empty,string.Empty);
	}
	void PopUpAvatorDialog()
	{
		if(parentItem == null)
			return;
		if(GuestLoginButton!=null)
				GuestLoginButton.collider.enabled=true;
		if(GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
		{
			if(bIsAddMasktable)
			{
				UtilityHelper.CloseMaskingTable();
				bIsAddMasktable = false;
			}
		}
		GameObject prefab = null;
		if (GlobalManager.Singleton.ApplicationType == AppType.NinetyOne)
			prefab=Resources.Load("prefab/chooseAvatar_91") as GameObject;
		else
			prefab=Resources.Load("prefab/chooseAvatar") as GameObject;
		GameObject item=Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		
		item.transform.parent = parentItem.transform;
		AvatorGridScript avatorGridScript = item.GetComponent<AvatorGridScript>();
		if (avatorGridScript != null)
		{
			avatorGridScript.mail = GlobalManager.Singleton.ParamEmail;
			avatorGridScript.password = GlobalManager.Singleton.ParamPassword;
		}
		item.transform.localPosition=new Vector3(0,0,-1);
		item.transform.localScale =new Vector3(1,1,1); 
	}
	void OnDestroy()
	{
		NdComHelper.InitCancelMetodInLogin(string.Empty,string.Empty);
	}
}
