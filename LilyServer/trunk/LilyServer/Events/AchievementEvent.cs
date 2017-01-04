using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Events;
using Photon.SocketServer.Rpc;
using DataPersist;

namespace LilyServer.Events
{
    public class AchievementEvent:LiteEventBase
    {
        [DataMember(Code = (byte)LilyEventKey.AchievementNumber)]
        public byte AchievementNumber { get; set; }

        [DataMember(Code = (byte)LilyEventKey.MessageContent)]
        public string Content { get; set; }

        public AchievementEvent(byte achievementNumber):this(achievementNumber,string.Empty)
        {
        }

        public AchievementEvent(byte achievementNumber, string content)
            : base(-1)
        {
            this.AchievementNumber = achievementNumber;
            this.Content = content;
            this.Code = (byte)LilyEventCode.Achievement;
        }
    }
}
