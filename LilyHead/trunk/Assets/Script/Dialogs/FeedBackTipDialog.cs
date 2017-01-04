using AssemblyCSharp.Helper;
using UnityEngine;
public class FeedBackTipDialog:DialogInfo
{
	public FeedBackTipDialog (bool bIsSendSuss,string feedback)
	{
		if(!string.IsNullOrEmpty(feedback.Trim())&&feedback!= LocalizeHelper.Translate("FEEDBACK_TIP_DESCRIPTION"))
		{
			if(bIsSendSuss)
			{
				this.Title = LocalizeHelper.Translate("FEEDBACK_SUCCESS_TITLE");
				this.Description = LocalizeHelper.Translate("FEEDBACK_SUCCESS_DESCRIPTION");
			}
			else
			{
				this.Title = LocalizeHelper.Translate("FEEDBACK_FAILED_TITLE");
				this.Description = LocalizeHelper.Translate("FEEDBACK_FAILED_DESCRIPTION");
			}
		}
		else
		{
			this.Title = LocalizeHelper.Translate("DIALOG_TITLE_ERROR");
			this.Description = LocalizeHelper.Translate("FEEDBACK_EMPTY_DESCRIPTION");
		}
		this.Buttons = 1;
	}
}

