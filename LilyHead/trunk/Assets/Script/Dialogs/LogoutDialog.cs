using System;
using AssemblyCSharp.Helper;
using UnityEngine;
using LilyHeart;

namespace AssemblyCSharp
{
	public class LogoutDialog: DialogInfo {
		public LogoutDialog()
		{
			this.Title = LocalizeHelper.Translate("TIPS_LOGOUT_TITLE");
			this.Description = LocalizeHelper.Translate("TIPS_LOGOUT_DESCRIPTION");
			this.Buttons = 2;
		}
		
		public override void Process ()
		{
			try
			{
				FileIOHelper.WriteFile(FileType.Account, string.Empty);
				PhotonClient.LogoutEvent += LogoutFinished;
				PlatformHelper.Logout();
				User.Singleton.Logout();
				SettingManager.Singleton.DeleteAll();
			}
			finally
			{
				KeyboardListener.EscapePressed=false;
			}
		}
		
		void LogoutFinished(bool result)
		{
			PhotonClient.LogoutEvent -= LogoutFinished;
			if (result)
			{
				GotoLaunchScene();
			}
			else
			{
				Debug.Log("Logout Failed!");
			}
		}
		
		public override void CancelProcess ()
		{
			try
			{
				base.CancelProcess ();
			}
			finally
			{
				KeyboardListener.EscapePressed=false;
			}
		}
		
		void GotoLaunchScene()
		{
			Application.LoadLevelAsync("EmptyScene_Login");
		}
	}
}