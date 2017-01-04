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
    class PlayerRankChangedEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.PlayRankList)]
        public byte[] RankList { get; set; }

        public PlayerRankChangedEvent(byte[] rankList)
            : base(-1)
        {            
            this.RankList = rankList;
            this.Code = (byte)LilyEventCode.PlayRankChanegd;
        }
    }
}
