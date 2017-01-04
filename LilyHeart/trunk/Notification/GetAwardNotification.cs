using System;

namespace LilyHeart
{
	public class GetAwardNotification:Notification
	{
		public GetAwardNotification ()
		{
			this.Target=TargetType.Room;
		}
	}
}

