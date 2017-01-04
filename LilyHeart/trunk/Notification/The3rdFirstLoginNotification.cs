using System;

namespace LilyHeart
{
	public class The3rdFirstLoginNotification:Notification
	{
		public The3rdFirstLoginNotification ()
		{
			this.Target=TargetType.LauchTable|TargetType.Room|TargetType.Empty;
		}
	}
}

