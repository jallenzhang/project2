using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class SameAccountLoginDialog : DialogInfo {
	public SameAccountLoginDialog()
	{
		this.Title = LocalizeHelper.Translate("SAME_ACCOUNT_LOGIN_TITLE");
		this.Description = LocalizeHelper.Translate("SAME_ACCOUNT_LOGIN_DESCRIPTION");
		this.Buttons = 1;
	}
	
	public override void Process()
	{
		Application.Quit();
	}
}
