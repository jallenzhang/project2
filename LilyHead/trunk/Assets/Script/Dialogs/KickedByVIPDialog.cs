using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class KickedByVIPDialog : DialogInfo {
	
	private GameObject callbackTarget;
	private string callbackMethod;
	
	public KickedByVIPDialog(string nickName, GameObject callbackTarget, string callbackMethod)
	{
		this.callbackTarget = callbackTarget;
		this.callbackMethod = callbackMethod;
		this.Title = LocalizeHelper.Translate("KICKBYVIP_TITLE");
		this.Description = string.Format(LocalizeHelper.Translate("KICKBYVIP_DESCRIPTION"), nickName);
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		callbackTarget.SendMessage(callbackMethod, null, SendMessageOptions.DontRequireReceiver);
	}
	
	public override void CancelProcess ()
	{
		
	}
}
