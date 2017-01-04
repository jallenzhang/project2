using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class PlayerMoneyChangedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MoneySafeAmnt)]
        public long MoneySafeAmnt { get; set; }

        [DataMember(Code = (byte)LilyEventKey.Chip)]
        public long Chips { get; set; }

        public PlayerMoneyChangedEvent(int noSeat, long moneySafeAmnt, long chips)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.MoneySafeAmnt = moneySafeAmnt;
            this.Chips = chips;
            this.Code = (byte)LilyEventCode.PlayerMoneyChanged;
        }
    }

    //class PlayerTakenMoneyChangedEvent : LiteEventBase
    //{
    //    [DataMember(Code = (byte)LilyEventKey.NoSeat)]
    //    public int NoSeat { get; set; }

    //    [DataMember(Code = (byte)LilyEventKey.MoneySafeAmnt)]
    //    public long MoneySafeAmnt { get; set; }

    //    [DataMember(Code = (byte)LilyEventKey.Chip)]
    //    public long Chips { get; set; }

    //    public PlayerTakenMoneyChangedEvent(int noSeat, long moneySafeAmnt, long chips)
    //        : base(-1)
    //    {
    //        this.NoSeat = noSeat;
    //        this.MoneySafeAmnt = moneySafeAmnt;
    //        this.Chips = chips;
    //        this.Code = (byte)LilyEventCode.TakenMoneyChanged;
    //    }
    //}


}
