using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class BetTurnStartedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.TypeRound)]
        public TypeRound TypeRound { get; set; }

        [DataMember(Code=(byte)LilyEventKey.GameCardIds)]
        public int[] GameCardIds {get;set;}

        public BetTurnStartedEvent(TypeRound typeRound, int[] gameCardIds)
            : base(-1)
        {
            TypeRound = typeRound;
            this.GameCardIds = gameCardIds;
            this.Code = (byte)LilyEventCode.BetTurnStarted;
        }
    }
}
