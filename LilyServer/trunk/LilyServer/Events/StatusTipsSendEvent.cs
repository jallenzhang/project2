using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    public class StatusTipsSendEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.StatusTipsMsgType)]
        public StatusTipsType StatusTipsMsgType { get; set; }

        [DataMember(Code=(byte)LilyEventKey.StatusTipsMsgParams)]
        public object[] StatusTipsMsgParams { get; set; }

        public StatusTipsSendEvent(StatusTipsType tipsType, object[] tipsParams)
            : base(-1)
        {
            this.StatusTipsMsgType = tipsType;
            this.StatusTipsMsgParams = tipsParams;                     
            this.Code = (byte)LilyEventCode.BroadcastStatusTipsMsg;
        }
    }
}
