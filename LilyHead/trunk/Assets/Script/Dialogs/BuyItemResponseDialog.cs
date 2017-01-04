using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class BuyItemResponseDialog : DialogInfo {

	public BuyItemResponseDialog(bool successed)
	{
		if (successed)
		{
			this.Title = LocalizeHelper.Translate("BUY_ITEM_RESPONSE_TITLE");
			this.Description = LocalizeHelper.Translate("BUY_ITEM_RESPONSE_DESCRIPTION");
		}
		else
		{
			this.Title = LocalizeHelper.Translate("BUY_ITEM_RESPONSE_FAILED_TITLE");
			this.Description = LocalizeHelper.Translate("BUY_ITEM_RESPONSE_FAILED_DESCRIPTION");
		}
		this.Buttons = 1;
	}
}
