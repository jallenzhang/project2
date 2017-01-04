using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class HandStrengthNotificationDialog : DialogInfo {
	
	private GameObject scriptObj = null;
	private bool bvalue;
	public HandStrengthNotificationDialog(bool bValue,GameObject spriteObject)
	{
		bvalue =  bValue;
		scriptObj = spriteObject;
		this.Title = LocalizeHelper.Translate("TIPS_HAND_STRENGTH_OPEN_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_HAND_STRENGTH_OPEN_DESCRIPTION");
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		if (scriptObj != null)
		{
			SetupDialog dialog = scriptObj.GetComponent<SetupDialog>();
			dialog.realSetHandStrength(bvalue);
		}
	}
}
