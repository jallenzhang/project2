using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class LevelLimitedDialog : DialogInfo {
	
	public LevelLimitedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_START_GAME_LEVEL_LIMITED_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_START_GAME_LEVEL_LIMITED_DESCRIPTION");
		this.Buttons = 1;
	}
}
