using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestLoginInfoDialog : DialogInfo {
	public GuestLoginInfoDialog()
	{
		this.Title = LocalizeHelper.Translate("GUEST_LOGIN_INFO_TITLE");
		this.Description = LocalizeHelper.Translate("GUEST_LOGIN_INFO_DESCRIPTION");
		this.Buttons = 1;
	}
}
