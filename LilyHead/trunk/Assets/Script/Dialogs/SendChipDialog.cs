using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class SendChipDialog : DialogInfo {
	
	public SendChipDialog(string name, string chipsValue)
	{
		this.Title = chipsValue;
		this.Description = string.Format(LocalizeHelper.Translate("TIPS_CHIP_SEND_TO_ME_DESCRIPTION"),"[ffffff]"+name+"[-]");
 		this.Buttons = 1;
	}
}
