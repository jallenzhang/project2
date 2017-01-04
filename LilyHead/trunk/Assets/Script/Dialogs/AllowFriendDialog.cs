using System;
using AssemblyCSharp.Helper;

namespace AssemblyCSharp
{
	public class AllowFriendDialog:DialogInfo
	{
		public AllowFriendDialog ()
		{
			this.Title = LocalizeHelper.Translate("TIPS_ALLOW_FRIENDS_TITLE");
			this.Description = LocalizeHelper.Translate("TIPS_ALLOW_FRIENDS_DESCRIPTION");
			this.Buttons = 2;
		}
		
		public override void Process ()
		{
			roombtnAction.Singleton.PopupFriendListDialog();
		}
		
		public override void CancelProcess()
		{
			base.CancelProcess();
		}
	}
}

