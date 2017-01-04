using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    public class RoomTypeChangedEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.RoomType)]
        public RoomType RoomType {get;set;}

        public RoomTypeChangedEvent(RoomType roomType):base(-1)
        {
            this.RoomType = roomType;
            this.Code = (byte)LilyEventCode.RoomTypeChanged;
        }
    }
}
