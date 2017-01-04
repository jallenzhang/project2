using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class PlayerTurnBeganEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code=(byte)LilyEventKey.LastNoSeat)]
        public int LastNoSeat {get;set;}

        [DataMember(Code = (byte)LilyEventKey.HigherBet)]
        public long HighBetAmnt { get; set; }

        public PlayerTurnBeganEvent(int noSeat,int lastNoSeat,long highBetAmnt)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.LastNoSeat = lastNoSeat;
            this.HighBetAmnt = highBetAmnt;
            this.Code = (byte)LilyEventCode.PlayerTurnBegan;
        }
    }
}
