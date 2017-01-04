using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    class ExperienceAddedEvent :LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.NoSeat)]
        public int NoSeat { get; set; }

        [DataMember(Code = (byte)LilyEventKey.Level)]
        public int Level { get; set; }

        [DataMember(Code = (byte)LilyEventKey.LevelExp)]
        public long LevelExp { get; set; }

        public ExperienceAddedEvent(int noSeat, int level,long levelexp)
            : base(-1)
        {
            this.NoSeat = noSeat;
            this.Level = level;
            this.LevelExp = levelexp;
            this.Code = (byte)LilyEventCode.ExperienceAdded;
        }
    }
}
