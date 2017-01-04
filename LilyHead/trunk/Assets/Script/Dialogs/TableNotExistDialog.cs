using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class TableNotExistDialog : DialogInfo {

	public TableNotExistDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_JOIN_GAME_NOTEXIST_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_JOIN_GAME_NOTEXIST_DESCRIPTION");
		this.Buttons = 1;
	}
}
