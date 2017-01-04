using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class GuestRequestFriendForbiddenDialog : DialogInfo {
	
	public GuestRequestFriendForbiddenDialog()
	{
		this.Title = LocalizeHelper.Translate("TIPS_GUEST_FRIEND_ADD_FAILED_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_GUEST_FRIEND_ADD_FAILED_DESCRIPTION");
		this.Buttons = 1;
	}
}
