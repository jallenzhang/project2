using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestHandStrengthFailedDialog : DialogInfo {
	
	private GameObject scriptObj = null;
	public GuestHandStrengthFailedDialog(GameObject spriteObject)
	{
		this.scriptObj = spriteObject;
		this.Title = LocalizeHelper.Translate("GUEST_HAND_STRENGTH_FAILED_TITLE");
		this.Description = LocalizeHelper.Translate("GUEST_HAND_STRENGTH_FAILED_DESCRIPTION");
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		roombtnAction btnAction = scriptObj.GetComponent<roombtnAction>();
		btnAction.onBtnChange();
	}
	public override void CancelProcess()
	{
		SetupDialog dialog = scriptObj.GetComponent<SetupDialog>();
		if(dialog!=null)
			dialog.SetButtons();
	}
}
