using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;

public class DailyChipGiftDialog : DialogInfo {
	
	public DailyChipGiftDialog(string chipsValue)
	{
		this.Title = chipsValue;
		this.Description = LocalizeHelper.Translate("TIPS_DAILY_GIFT_DESCRIPTION");
		this.Buttons = 1;
	}
}
