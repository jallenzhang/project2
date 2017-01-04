using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using System.Collections;
using DataPersist;

namespace LilyServer.Events
{
    class PlayerWonPotEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MoneyPotId)]
        public int MoneyPotId { get; set; }

        [DataMember(Code = (byte)LilyEventKey.AmountWon)]
        public long AmountWon { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MoneySafeAmnt)]
        public long MoneySafeAmnt { get; set; }

        public PlayerWonPotEvent(int noSeat,int moneyPotId,long amountWon,long moneySafeAmnt)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.MoneyPotId=moneyPotId;
            this.AmountWon = amountWon;
            this.MoneySafeAmnt = moneySafeAmnt;
            this.Code = (byte)LilyEventCode.PlayerWonPot;
        }
    }

    class PlayerWonImprovePotEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.MoneyPotId)]
        public int Id { get; set; }

        [DataMember(Code = (byte)LilyEventKey.WinnerSeats)]
        public int[] Winner { get; set; }
        [DataMember(Code = (byte)LilyEventKey.Amounts)]
        public long[] WinAmnt { get; set; }
        [DataMember(Code = (byte)LilyEventKey.AttachedPlayerSeats)]
        public int[] AttachedPlayer { get; set; }

        public PlayerWonImprovePotEvent(int id,int[] winner,long[] winamnt,int[] attachedplayer)
            :base(-1)
        {
            this.Id = id;
            this.Winner = winner;
            this.WinAmnt = winamnt;
            this.AttachedPlayer = attachedplayer;
            this.Code = (byte)LilyEventCode.PlayerWonPotImprove;
        }
    }
}
