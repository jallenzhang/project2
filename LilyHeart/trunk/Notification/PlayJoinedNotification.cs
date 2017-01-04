using System;

namespace LilyHeart
{
	public class PlayJoinedNotification:Notification
	{
		public int NoSeat {get; private set;}
		public PlayJoinedNotification (int noseat)
		{
			this.NoSeat=noseat;
		}
	}
}

