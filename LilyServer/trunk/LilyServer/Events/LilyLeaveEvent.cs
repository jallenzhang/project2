using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class LilyLeaveEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.UserId)]
        public string UserId { get; set; }
        [DataMember(Code = (byte)LilyEventKey.RoomID)]
        public string RoomID { get; set; }

        public LilyLeaveEvent(string userId, int actorNr,string roomid)
            : base(actorNr)
        {
            UserId = userId;
            RoomID = roomid;
            this.Code = (byte)LilyEventCode.Leave;
        }
    }
}
