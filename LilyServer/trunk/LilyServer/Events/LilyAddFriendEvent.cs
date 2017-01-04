using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using System.Collections;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class LilyAddFriendEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.UserData)]
        public Hashtable UserData { get; set; }

        public LilyAddFriendEvent(Hashtable userData)
            : base(-1)
        {
            this.Code = (byte)LilyEventCode.AddFriend;
            this.UserData = userData;
        }

    }
}
