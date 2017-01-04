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
    class LilyInviteFriendEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.UserData)]
        public Hashtable UserData { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MessageContent)]
        public string Content { get; set; }

        [DataMember(Code = (byte)LilyEventKey.GameServerAddress)]
        public string GameServerAddress { get; set; }

        public LilyInviteFriendEvent(Hashtable userData,string content)
            : base(-1)
        {
            UserData = userData;
            Content = content;
            this.Code = (byte)LilyEventCode.InviteFriend;
        }
    }
}
