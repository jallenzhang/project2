using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using LilyHeart;

public class LogoutConfirmDialog : DialogInfo {
	public LogoutConfirmDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_LOGOUT_CONFIRM_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_LOGOUT_CONFIRM_DESCRIPTION");
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		FileIOHelper.WriteFile(FileType.Account, string.Empty);
		PhotonClient.LogoutEvent += LogoutFinished;
		PlatformHelper.Logout();
		User.Singleton.Logout();
		SettingManager.Singleton.DeleteAll();
	}
	
	void LogoutFinished(bool result)
	{
		PhotonClient.LogoutEvent -= LogoutFinished;
		if (result)
		{
			User.Singleton.UserData = null;
			User.Singleton.VipProps = null;
			User.Singleton.Friends = null;
			User.Singleton.Avators.Clear();
			GotoLaunchScene();
		}
		else
		{
			Debug.Log("Logout Failed!");
			GlobalManager.Log("Logout Failed!");
		}
	}
	
	void GotoLaunchScene()
	{
		if(GlobalManager.Singleton.version == KindOfVersion.Basic)
			Application.LoadLevelAsync("EmptyScene_Login_simple");
		else
		if(GlobalManager.Singleton.version == KindOfVersion.Ultimate)
			Application.LoadLevelAsync("EmptyScene_Login");
	}
}
