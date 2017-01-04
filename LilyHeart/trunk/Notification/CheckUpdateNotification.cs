using System;

namespace LilyHeart
{
	public class CheckUpdateNotification:Notification
	{
		public string ClientVersion {get;set;}
		public CheckUpdateNotification (string version)
		{
			this.Target=TargetType.Empty;
			this.ClientVersion=version;
		}
	}
}

