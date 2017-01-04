using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using LilyHeart;

public class NinetyOneGuestBuyTipDialog : DialogInfo {
	private string gameObjectName;
	private string MethodName;
	private bool bIsNotInGambling = false;
	public NinetyOneGuestBuyTipDialog(string strGameObjectName,string strMethodName,bool IsNotInGambling)
	{
		this.gameObjectName = strGameObjectName;
		this.MethodName = strMethodName;
		this.bIsNotInGambling = IsNotInGambling;
		this.Title = LocalizeHelper.Translate("NINETYONE_TIP_MESSAGE_TITLE");
		this.Description = string.Format(LocalizeHelper.Translate("NINETYONE_TIP_MESSAGE"), User.Singleton.UserData.NickName);
		this.Buttons = 1;
		if(bIsNotInGambling)
			this.Buttons = 2;
	}
	public override void Process ()
	{
		if(bIsNotInGambling)
			PlatformHelper.Upgrade(gameObjectName,MethodName);
	}
}
