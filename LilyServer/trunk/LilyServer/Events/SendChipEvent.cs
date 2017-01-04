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
    public class SendChipEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.UserData)]
        public Hashtable Sender { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MessageContent)]
        public string Content { get; set; }

        public SendChipEvent(Hashtable sender, long chip,long totalChip)
            : base(-1)
        {
            this.Code = (byte)LilyEventCode.SendChip;
            this.Sender = sender;
            this.Content = chip.ToString()+"|"+totalChip.ToString();
        }
    }
}
