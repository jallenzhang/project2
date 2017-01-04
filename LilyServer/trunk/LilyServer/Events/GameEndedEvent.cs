using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using DataPersist;
using Photon.SocketServer.Rpc;

namespace LilyServer.Events
{
    class GameEndedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.TaxAmnt)]
        public long TaxAmnt { get; set; }

        public GameEndedEvent(long amnt)
            : base(-1)
        {
            this.TaxAmnt = amnt;
            this.Code = (byte)LilyEventCode.GameEnded;
        }
    }
}
