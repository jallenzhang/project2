using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class BuyItemConfirmDialog : DialogInfo {
	
	private GameObject target;
	private string methodName;
	private const string sceneType = "Scene";
	private const string avatarType = "Avatar";
	
	public BuyItemConfirmDialog(long chips, string itemName, GameObject callbackTarget, string callbackMethodName, string type)
	{
		this.Title = LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_TITLE");
		if (type == sceneType)
			this.Description = string.Format(LocalizeHelper.Translate("BUY_SCENE_CONFIRM_DIALOG_DESCRIPTION"), chips.ToString(), LocalizeHelper.Translate(itemName)); 
		else if (type == avatarType)
			this.Description = string.Format(LocalizeHelper.Translate("BUY_AVATAR_CONFIRM_DIALOG_DESCRIPTION"), chips.ToString(), LocalizeHelper.Translate(itemName));
		else
			this.Description = string.Format(LocalizeHelper.Translate("BUY_ITEM_CONFIRM_DIALOG_DESCRIPTION"), chips.ToString(), LocalizeHelper.Translate(itemName));
		target = callbackTarget;
		methodName = callbackMethodName;
		
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		target.SendMessage(methodName, null, SendMessageOptions.DontRequireReceiver);
	}
}
