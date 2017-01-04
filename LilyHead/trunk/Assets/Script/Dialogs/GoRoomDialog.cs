using System;
using AssemblyCSharp.Helper;

namespace AssemblyCSharp
{
	public class GoRoomDialog:DialogInfo {
		private string scene;
		public GoRoomDialog(string scene)
		{
			this.Title=LocalizeHelper.Translate("TIPS_GOROOM_TITLE");
			this.Description=LocalizeHelper.Translate("TIPS_GOROOM_DESCRIPTION");
			this.Buttons=2;
			this.scene=scene;
		}
		
		public override void Process ()
		{
			try
			{
				if(this.scene==KeyboardListener.SCENE_GAME)
				{
					if(ActorInforController.Singleton!=null)
					{
						ActorInforController.Singleton.BtnBackToRoom();
					}
				}
				else if(this.scene==KeyboardListener.SCENE_MATCH)
				{
					if(MatchInforController.Singleton!=null)
					{
						MatchInforController.Singleton.BtnBackToRoom();
					}
				}
			}
			finally
			{
				KeyboardListener.EscapePressed=false;
			}
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
}

