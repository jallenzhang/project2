using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class SearchFailedDialog : DialogInfo {
	public SearchFailedDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_BLANK_IN_SEARCHI_BOX_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_BLANK_IN_SEARCHI_BOX_DESCRIPTION");
		this.Buttons = 1;
	}
}
