using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using AnimationOrTween;
using LilyHeart;
using AssemblyCSharp.Helper;
using DataPersist;

public class ForgotLogic : loginBase {
	
	public GameObject forgotMail;
	//public GameObject forgetBtn;
	//public bool isSimple = false;
	
	// Use this for initialization
	void Start () {
		this.loginOK=false;
	}
	
	// Update is called once per frame
	void Update () {		
		checkLogin();				
	}
	
	void OnForgot()
	{
		UILabel ulMail = (UILabel)forgotMail.GetComponent("UILabel");
		if(string.IsNullOrEmpty(ulMail.text))
			UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), LocalizeHelper.Translate("FINDER_MAIL_EMPTY_ERROR"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		else
			User.Singleton.GetPassword(ulMail.text);
	}
	
	void ForgetPwd()
	{
		// WebBindingHelper.CloseWebView();		
		// CallForgetPwdAnimation();
		
		OpenForgetPwdWebPage(this.gameObject.name);		
	}
	
	static void OpenForgetPwdWebPage(string gameObjectName){
		Debug.Log("Lee test method:OpenForgetPwdWebPage");
		//WebBindingHelper.CloseWebView();
		WebBindingHelper.ShowForgetPwdWebView(gameObjectName, "CallBackSendEmailMethod");	
	}
	
	void CallBackSendEmailMethod(string myEmail){
		if(string.IsNullOrEmpty(myEmail))
			UtilityHelper.ShowAlertDialog(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), LocalizeHelper.Translate("FINDER_MAIL_EMPTY_ERROR"), LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
		else
			User.Singleton.GetPassword(myEmail);		
	}
	
	void CallLoginPageMethod(string myParam){
		Debug.Log("Lee test method:CallLoginPageMethod");
		WebBindingHelper.CloseWebView();		
		loginLogic.ShowLoginViewMethod(gameObject.name);
	}
	
 	public static void SendEmailMsgToWebView(){
		if(GlobalManager.Singleton.Notifications.Count>0)
		{	
			Notification notification=GlobalManager.Singleton.Notifications.Peek();
			if((notification.Target&TargetType.LauchTable)==TargetType.LauchTable)
			{				
				if(notification is FindPasswordNotification)
				{					
					FindPasswordNotification findPasswordNotification=GlobalManager.Singleton.Notifications.Dequeue() as FindPasswordNotification;
					string webViewMsg = string.Empty;
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
	}
	
	/*void CallForgetPwdAnimation(){
		Component[] components = forgetBtn.transform.GetComponents(typeof(UIButtonPlayAnimation));		
		foreach (Component item in components) {
			var uiPlayAni = item as UIButtonPlayAnimation;
			Debug.Log(uiPlayAni.name);
			
			bool forward = true;
#if UNITY_IPHONE
			forward = true;
#endif
#if UNITY_ANDROID
			if(isSimple)
				forward = true;
			else
				forward = false;
#endif
			if(uiPlayAni.target != null){
				int pd = -(int)uiPlayAni.playDirection;
				Direction dir = forward ? uiPlayAni.playDirection : ((Direction)pd);
				ActiveAnimation anim = ActiveAnimation.Play(uiPlayAni.target, uiPlayAni.clipName, dir, uiPlayAni.ifDisabledOnPlay, uiPlayAni.disableWhenFinished);
			}
		}
	}*/
}
