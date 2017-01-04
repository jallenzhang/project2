using System;

namespace LilyHeart
{
	public class PlayerTurnEndedNotification:Notification
	{
		public int NoSeat {get;private set;}
        public bool IsClient { get; private set; }
		public PlayerTurnEndedNotification (int noSeat,bool isclient)
		{
			this.NoSeat=noSeat;
		    this.IsClient = isclient;
		}
	}
}

