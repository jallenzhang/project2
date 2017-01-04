using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestLevelLimitedDialog : DialogInfo {
	public GuestLevelLimitedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_GUEST_LEVEL_LIMITED_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_GUEST_LEVEL_LIMITED_DESCRIPTION");
		this.Buttons = 1;
	}
}
