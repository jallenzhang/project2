
using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using AssemblyCSharp.Helper;
using LilyHeart;

namespace AssemblyCSharp
{
	public class loginBase:MonoBehaviour
	{
		public bool loginOK;
		protected User player = null;
		
		// Update is called once per frame
		
		public void checkLogin()
		{
			if(loginOK) return;
			User player=User.Singleton;
			if(player.GameStatus==GameStatus.InRoom||player.GameStatus==GameStatus.InGame)
			{
				loginOK=true;
				GlobalManager.Singleton.InitGuestSignIn = false;
				WebBindingHelper.CloseWebView();
//				string accountInfo = player.UserData.Mail + "|";
//				accountInfo = accountInfo + player.UserData.Password + "|";
//				accountInfo = accountInfo + ((int)player.UserData.UserType).ToString();
//				FileIOHelper.WriteFile(FileType.Account, accountInfo);
//				Application.LoadLevel("BackGround");
				//Application.LoadLevel("Main");
			}
//			else if(player.GameStatus==GameStatus.Error)
//			{
//				player.GameStatus = GameStatus.NoStatus;
//				GlobalManager.Log(PhotonClient.Singleton.ErrorMessage);
//				EtceteraBinding.showAlertWithTitleMessageAndButton(LocalizeHelper.Translate("DIALOG_TITLE_ERROR"), PhotonClient.Singleton.ErrorMessage, LocalizeHelper.Translate("DIALOG_BUTTON_OK"));
//			}
			StartCoroutine(NetworkUpdate());
		}
		
		IEnumerator NetworkUpdate()
		{
			PhotonClient.Singleton.Update();
			yield return null;
		}
		
		//check user emain legality or not
		//emial format xxx@xxx.xxx
		public bool checkEmain(string email)
		{
			return UtilityHelper.checkEmain(email);
		}
		
		//check user password 
		public  bool checkPwd(string pwd)
		{
			return UtilityHelper.checkPwd(pwd);
		}
		// check name uniqueness
		public string checkNickName(string nick)
		{
			return UtilityHelper.checkNickName(nick,0);
		}
		
		// return back loginmain scene
		public void Onback()
		{
			Application.LoadLevel("loginMain");
		}
		
		// 
		public delegate void RequestTimeout();
		//can not connect the server
		public delegate void unEnableConnet();
		
		
	}
}

