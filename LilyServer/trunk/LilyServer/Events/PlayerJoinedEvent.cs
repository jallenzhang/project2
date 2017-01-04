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
    class PlayerJoinedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.PlayerInfo)]
        public Hashtable PlayerInfo { get; set; }

        public PlayerJoinedEvent(Hashtable playerInfo)
            : base(-1)
        {
            this.PlayerInfo = playerInfo;
            this.Code = (byte)LilyEventCode.PlayerJoined;
        }
    }
}
