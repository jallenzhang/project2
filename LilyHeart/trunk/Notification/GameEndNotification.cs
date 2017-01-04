using System;

namespace LilyHeart
{
	public class GameEndNotification:Notification
	{
		public long taxAnimt;
		public GameEndNotification (long tax)
		{
			taxAnimt=tax;
		}
	}
}

