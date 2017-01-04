using System;

namespace LilyHeart
{
	public class PlayerTurnBeganNotification:Notification
	{
		public int NoSeat {get;private set;}
        public bool IsClient { get; private set; }

	    public PlayerTurnBeganNotification (int noSeat,bool isclient)
		{
			this.NoSeat=noSeat;
	        this.IsClient = isclient;
		}
	}
}

