using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    public class SendGiftEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.SendNoSeat)]
        public int SenderNoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.ReceiverNoSeat)]
        public int ReceiverNoSeat { get; set; }

        [DataMember(Code=(byte)LilyEventKey.GiftId)]
        public int Gift {get;set;}

        [DataMember(Code = (byte)LilyEventKey.MoneySafeAmnt)]
        public long MoneySafeAmnt { get; set; }

        [DataMember(Code = (byte)LilyEventKey.Chip)]
        public long Chips { get; set; }

        public SendGiftEvent(int senderNoSeat, int gift,long moneySafeAmnt,long chips)
            : this(senderNoSeat,-1, gift,moneySafeAmnt,chips)
        {
        }

        public SendGiftEvent(int senderNoSeat, int receiverNoSeat, int gift,long moneySafeAmnt,long chips)
            : base(-1)
        {
            this.Code = (byte)LilyEventCode.SendGift;
            this.SenderNoSeat = senderNoSeat;
            this.ReceiverNoSeat = receiverNoSeat;
            this.Gift = gift;
            this.MoneySafeAmnt = moneySafeAmnt;
            this.Chips = chips;
        }
    }
}
