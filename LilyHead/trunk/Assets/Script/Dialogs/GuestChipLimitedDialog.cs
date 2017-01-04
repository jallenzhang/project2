using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestChipLimitedDialog : DialogInfo {
	public GuestChipLimitedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_GUEST_CHIPS_LIMITED_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_GUEST_CHIPS_LIMITED_DESCRIPTION");
		this.Buttons = 1;
	}
}
