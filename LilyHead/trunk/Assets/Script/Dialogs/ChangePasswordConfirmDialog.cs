using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;

public class ChangePasswordConfirmDialog : DialogInfo {
	private GameObject internalObj;
	public ChangePasswordConfirmDialog(GameObject scriptObj)
	{
		internalObj = scriptObj;
		this.Title = LocalizeHelper.Translate("TIPS_CHANGE_PASSWORD_CONFIRM_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_CHANGE_PASSWORD_CONFIRM_DESCRIPTION");
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		PlatformHelper.ChangePassword(internalObj);
	}
}
