using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class BuyGiftFailedDialog : DialogInfo {
	
	public BuyGiftFailedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_BUY_GIFT_CHIP_FAILED_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_BUY_GIFT_CHIP_FAILED_DESCRIPTION");
		this.Buttons = 1;
	}
}
