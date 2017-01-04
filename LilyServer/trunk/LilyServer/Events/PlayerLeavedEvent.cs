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
    class PlayerLeavedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.NickName)]
        public string NickName { get; set; }

        [DataMember(Code = (byte)LilyEventKey.PlayerLeaveType)]
        public byte PlayerLeaveType { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MessageContent)]
        public string MessageContent { get; set; }

        public PlayerLeavedEvent(int noSeat)
            : base(-1)
        {
            this.NoSeat = noSeat;
            //this.Bystander = false;
            this.Code = (byte)LilyEventCode.PlayerLeaved;
        }
    }
}
