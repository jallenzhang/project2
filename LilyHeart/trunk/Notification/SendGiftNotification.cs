using System;

namespace LilyHeart
{
	public class SendGiftNotification:Notification
	{
		public int SenderNoSeat{get;private set;}
		public int ReceiverNoSeat {get; private set;}
		public int GiftId {get; private set;}
		
		public SendGiftNotification (int senderNoSeat,int receiverNoSeat,int giftId)
		{
			this.SenderNoSeat=senderNoSeat;
			this.ReceiverNoSeat=receiverNoSeat;
			this.GiftId=giftId;
		}
	}
}

