using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class GameStartedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeatDealer)]
        public int NoSeatDealer { get; set; }

        [DataMember(Code=(byte)LilyEventKey.NoSeatSmallBlind)]
        public int NoSeatSmallBlind {get;set;}

        [DataMember(Code=(byte)LilyEventKey.NoSeatBigBlind)]
        public int NoSeatBigBlind {get;set;}

        [DataMember(Code = (byte)LilyEventKey.TotalRounds)]
        public int TotalRounds { get; set; }

        [DataMember(Code = (byte)LilyEventKey.PlayingNoSeats)]
        public int[] PlayingNoSeats { get; set; }

        [DataMember(Code = (byte)LilyEventKey.BigBlind)]
        public int BigBlind { get; set; }

        public GameStartedEvent(int noSeatDealer, int noSeatSmallBlind, int noSeatBigBlind)
            : base(-1)
        {
            this.NoSeatDealer = noSeatDealer;
            this.NoSeatSmallBlind = noSeatSmallBlind;
            this.NoSeatBigBlind = noSeatBigBlind;
            this.Code = (byte)LilyEventCode.GameStarted;
        }

        public GameStartedEvent(int noSeatDealer, int noSeatSmallBlind, int noSeatBigBlind, int totalRounds, int[] playingNoSeats,int bigblind)
            : this(noSeatDealer, noSeatSmallBlind, noSeatBigBlind)
        {
            this.PlayingNoSeats = playingNoSeats;
            this.TotalRounds = totalRounds;
            this.BigBlind = bigblind;
        }
    }
}
