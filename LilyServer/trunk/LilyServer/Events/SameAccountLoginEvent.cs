using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using DataPersist;

namespace LilyServer.Events
{
    public class SameAccountLoginEvent:LiteEventBase
    {
        public SameAccountLoginEvent():base(-1)
        {
            this.Code = (byte)LilyEventCode.SameAccountLogin;
        }
    }
}
