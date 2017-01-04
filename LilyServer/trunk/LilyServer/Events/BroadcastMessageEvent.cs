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
    class BroadcastMessageEvent:LiteEventBase
    {
        [DataMember(Code=(byte)LilyEventKey.Message)]
        public string Message { get; set; }

        [DataMember(Code = (byte)LilyEventKey.NickName)]
        public string NickName { get; set; }

        public BroadcastMessageEvent(string message)
            : base(-1)
        {
            Message = WordsFilter.getInstance().Filter(message);
            this.Code = (byte)LilyEventCode.BroadcastMessage;
        }

        public BroadcastMessageEvent(string message, string nickname) 
            :this(message)
        {
            NickName = nickname;
        }
    }
}
