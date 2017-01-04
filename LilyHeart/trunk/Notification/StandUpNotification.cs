using System;

namespace LilyHeart
{
	public class StandUpNotification:Notification
	{
		public int Noseat{get; private set;}
		public StandUpNotification (int nose)
		{
			Noseat=nose;
		}
	}
}

