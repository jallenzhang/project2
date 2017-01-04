using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class NotificationCloseConfirmDialog : DialogInfo {
	private const string SystemNotification = "System";
	private const string FriendNotification = "Friend";
	
	private string notificationType = string.Empty;
	private bool bValue = false;
	private GameObject scriptObj = null;
	public NotificationCloseConfirmDialog(string type,bool bvalue, GameObject spriteObject)
	{
		notificationType = type;
		bValue = bvalue;
		scriptObj = spriteObject;
		this.Title = LocalizeHelper.Translate("TIPS_NOTIFICATION_CLOSE_TITLE");
		this.Description = LocalizeHelper.Translate("TIPS_NOTIFICATION_CLOSE_DESCRIPTION");
		this.Buttons = 2;
	}
	
	public override void Process ()
	{
		if (scriptObj != null)
		{
			SetupDialog dialog = scriptObj.GetComponent<SetupDialog>();
			if (notificationType == SystemNotification)
			{
				dialog.realSetSystemNotification(bValue);
			}
			else if (notificationType == FriendNotification)
			{
				dialog.realSetFriendActivityNotification(bValue);
			}
		}
	}
	public override void CancelProcess()
	{
		SetupDialog dialog = scriptObj.GetComponent<SetupDialog>();
		if(dialog!=null)
			dialog.SetButtons();
	}
}
