using UnityEngine;
using System.Collections;
using AssemblyCSharp.Helper;

public class CloseDialog : DialogInfo {
	public CloseDialog()
	{
		this.Title=LocalizeHelper.Translate("TIPS_EXIT_TITLE");
		this.Description=LocalizeHelper.Translate("TIPS_EXIT_DESCRIPTION");
		this.Buttons=2;
	}
	
	public override void Process ()
	{
		Application.Quit();
	}
	
	public override void CancelProcess ()
	{
		try
		{
			base.CancelProcess ();
		}
		finally
		{
			KeyboardListener.EscapePressed=false;
		}
	}
}
