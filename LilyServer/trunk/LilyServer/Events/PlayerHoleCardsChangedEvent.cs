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
    class PlayerHoleCardsChangedEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.IsPlaying)]
        public bool IsPlaying { get; set; }

        [DataMember(Code = (byte)LilyEventKey.GameCardIds)]
        public int[] GameCards { get; set; }

        public PlayerHoleCardsChangedEvent(int noSeat,bool isPlaying,int[] gameCards)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.IsPlaying = isPlaying;
            this.GameCards = gameCards;
            this.Code = (byte)LilyEventCode.PlayerHoleCardsChanged;
        }
    }
}
