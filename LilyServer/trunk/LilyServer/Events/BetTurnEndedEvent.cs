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
    class BetTurnEndedEvent : LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.TypeRound)]
        public TypeRound TypeRound { get; set; }

        [DataMember(Code=(byte)LilyEventKey.Amounts)]
        public Hashtable Amounts {get;set;}

        public BetTurnEndedEvent(TypeRound typeRound,Hashtable amounts)
            : base(-1)
        {
            TypeRound = typeRound;
            Amounts = amounts;
            this.Code = (byte)LilyEventCode.BetTurnEnded;
        }
    }
}
