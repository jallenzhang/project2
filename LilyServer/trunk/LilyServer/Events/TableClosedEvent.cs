using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using DataPersist;

namespace LilyServer.Events
{
    class TableClosedEvent:LiteEventBase
    {
        public TableClosedEvent():base(-1)
        {
            this.Code = (byte)LilyEventCode.TableClosed;
        }
    }
}
