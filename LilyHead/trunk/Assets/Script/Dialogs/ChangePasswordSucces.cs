using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class ChangePasswordSucces : DialogInfo {

	public ChangePasswordSucces()
	{
		this.Title = LocalizeHelper.Translate("CHANGE_PASSWORD_SUCCESS_TITLE");
		this.Description = LocalizeHelper.Translate("CHANGE_PASSWORD_SUCCESS_DESCRIPTION");
		this.Buttons = 1;
	}
}
