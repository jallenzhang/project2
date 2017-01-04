using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;
using AssemblyCSharp;
using LilyHeart;

public class FriendAddedDialog : DialogInfo {

	public FriendAddedDialog(string name)
	{
		this.Title = LocalizeHelper.Translate("TIPS_FRIEND_ADDED_TITLE");
		this.Description = string.Format(LocalizeHelper.Translate("TIPS_FRIEND_ADDED_DESCRIPTION"),name);
		this.Buttons = 1;
	}
}


public class TableFullErrorDialog : DialogInfo {

	public TableFullErrorDialog()
	{
		this.Title = LocalizeHelper.Translate("SAME_ACCOUNT_LOGIN_TITLE");
		this.Description = LocalizeHelper.Translate("TABLEFULLERROR");
		this.Buttons = 1;
	}
	public override void Process ()
	{
		User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
	}
}

public class GameIdNotExistsDialog : DialogInfo {

	public GameIdNotExistsDialog()
	{
		this.Title = LocalizeHelper.Translate("SAME_ACCOUNT_LOGIN_TITLE");
		this.Description = LocalizeHelper.Translate("JOINGAMEERROR1");
		this.Buttons = 1;
	}
	public override void Process ()
	{
		User.Singleton.JoinRoom(User.Singleton.UserData.UserId);
	}
}

