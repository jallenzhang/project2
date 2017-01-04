using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestBuyChipsLimitedDialog : DialogInfo {
	public GuestBuyChipsLimitedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_GUEST_BUY_CHIPS_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_GUEST_BUY_CHIPS_DESCRIPTION");
		this.Buttons = 1;
	}
}
