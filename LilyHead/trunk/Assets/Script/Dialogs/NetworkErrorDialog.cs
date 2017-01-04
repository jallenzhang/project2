using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;

public class NetworkErrorDialog : DialogInfo {

	public NetworkErrorDialog()
	{
		this.Title = LocalizeHelper.Translate("NETWORK_CONNECTION_ERROR_TITLE");
		this.Description = LocalizeHelper.Translate("NETWORK_CONNECTION_ERROR_DESCRIPTION");
		this.Buttons = 2;	
	}
	
	public override void Process()
	{
		UtilityHelper.MaskTableTryAgain();
	}
	
	public override void CancelProcess()
	{
//		if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
//			UtilityHelper.CloseMaskingTable();
//	    
//		UtilityHelper.ResetFlags();
//		string levelName = string.Empty;
//		if (Application.loadedLevelName == "GamblingInterface_Title")
//			levelName = "BackGround";//if network error, then go to background anyway, if it is login already before.
//		else
//			levelName = Application.loadedLevelName;
//		Application.LoadLevelAsync(levelName);
		Application.Quit();
	}
}
