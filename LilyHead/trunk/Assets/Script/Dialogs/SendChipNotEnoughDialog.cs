using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class SendChipNotEnoughDialog : DialogInfo {
	public SendChipNotEnoughDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_NOTIFICATION_CLOSE_TITLE");
		this.Description = LocalizeHelper.Translate("SEND_CHIP_MONEY_NOT_ENOUGH");
		this.Buttons = 1;
	}
}
