using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class LilyDeleteFriendEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.UserId)]
        public string UserId { get; set; }

        public LilyDeleteFriendEvent(string userId)
            : base(-1)
        {
            UserId = userId;
            this.Code = (byte)LilyEventCode.DeleteFriend;
        }
    }
}
