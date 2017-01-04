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
    class PlayersShowCardsEvent:LiteEventBase
    {
        
        [DataMember(Code=(byte)LilyEventKey.PlayerInfo)]
        public Dictionary<int,string> ShowCardPlayers { get; set; }

        public PlayersShowCardsEvent()
            : base(-1)
        {
            this.Code = (byte)LilyEventCode.PlayersShowCards;
        }
    }
}
