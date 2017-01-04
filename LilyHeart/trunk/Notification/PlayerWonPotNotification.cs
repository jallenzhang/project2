using System;

namespace LilyHeart
{
	public class PlayerWonPotNotification:Notification
	{
		public long Amount {get;private set;}
		public int NoSeat {get;private set;}
		public PlayerWonPotNotification (long amount,int noseat)
		{
			this.Amount=amount;
			this.NoSeat=noseat;
		}
	}
}

