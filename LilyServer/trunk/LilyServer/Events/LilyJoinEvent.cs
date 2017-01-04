using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using System.Collections;
using Lite.Operations;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    public class LilyJoinEvent : LiteEventBase
    {
        public LilyJoinEvent(int actorNr, Hashtable userData)
            : base(actorNr)
        {
            this.Code = (byte)LilyEventCode.Join;
            this.UserData = userData;
        }

        [DataMember(Code = (byte)LilyEventKey.UserData)]
        public Hashtable UserData
        {
            get;
            set;
        }
    }
}
