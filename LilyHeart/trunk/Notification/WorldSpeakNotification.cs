using System;

namespace LilyHeart
{
	public class WorldSpeakNotification:Notification
	{
		public WorldSpeakNotification ()
		{
			this.Target=TargetType.Room;
		}
	}
}

