using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;
using System.Collections;

namespace LilyServer.Events
{
    class PlayerTurnEndedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MoneyBetAmnt)]
        public long MoneyBetAmnt { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MoneySafeAmnt)]
        public long MoneySafeAmnt { get; set; }

        [DataMember(Code = (byte)LilyEventKey.IsPlaying)]
        public bool IsPlaying { get; set; }

        [DataMember(Code = (byte)LilyEventKey.TotalPortAmnt)]
        public long TotalPortAmnt { get; set; }

        [DataMember(Code = (byte)LilyEventKey.AmountPlayed)]
        public long AmountPlayed { get; set; }

        [DataMember(Code = (byte)LilyEventKey.Action)]
        public TypeAction Action { get; set; }

        [DataMember(Code = (byte)LilyEventKey.StepID)]
        public int StepID { get; set; }


        public PlayerTurnEndedEvent(int noSeat,long moneyBetAmnt,long moneySafeAmnt, bool isPlaying, long totalPortAmnt, long AmountPlayed, TypeAction action)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.MoneyBetAmnt = moneyBetAmnt;
            this.MoneySafeAmnt = moneySafeAmnt;
            this.IsPlaying = isPlaying;
            this.TotalPortAmnt = totalPortAmnt;
            this.AmountPlayed = AmountPlayed;
            this.Action = action;
            this.Code = (byte)LilyEventCode.PlayerTurnEnded;
        }

        public PlayerTurnEndedEvent(int noSeat, long moneyBetAmnt, long moneySafeAmnt, bool isPlaying, long totalPortAmnt, long AmountPlayed, TypeAction action,int stepid)
            : this(noSeat,moneyBetAmnt,moneySafeAmnt,isPlaying,totalPortAmnt,AmountPlayed,action)
        {
            this.StepID = stepid;
        }
    }
}
