using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;
using LilyServer.Helper;

namespace LilyServer.Events
{
    class BroadcastMessageInTableEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.NoSeat)]
        public int NoSeat {get;set;}

        [DataMember(Code=(byte)LilyEventKey.Message)]
        public string Message { get; set; }

        public BroadcastMessageInTableEvent(string message,int noseat)
            : base(-1)
        {
            NoSeat = noseat;
            Message = WordsFilter.getInstance().Filter(message);
            this.Code = (byte)LilyEventCode.BroadcastMessageInTable;
        }
    }
}
