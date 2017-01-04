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
    class LilyRequestFriendEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.UserData)]
        public Hashtable Sender { get; set; }

        public LilyRequestFriendEvent(Hashtable userData)
            : base(-1)
        {
            Sender = userData;
            this.Code = (byte)LilyEventCode.RequestFriend;
        }
    }
}
