using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class ChipsNotEnoughDialog : DialogInfo {

	private GameObject target;
	private string strMethodName;
	public ChipsNotEnoughDialog(string title,GameObject callBackTarget,string MethodName)
	{
		this.Title = title;
		this.Description = LocalizeHelper.Translate("TIPS_JOIN_GAME_CHIP_NOT_ENOUGH_DESCRIPTION");
		this.Buttons = 1;
		this.target = callBackTarget;
		this.strMethodName =  MethodName;
	}
	public override void Process ()
	{
		if(target!=null&&!string.IsNullOrEmpty(strMethodName))
			target.SendMessage(strMethodName,SendMessageOptions.DontRequireReceiver);
	}
}
