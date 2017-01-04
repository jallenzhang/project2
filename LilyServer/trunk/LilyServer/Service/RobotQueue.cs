using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using DataPersist;

namespace LilyServer
{
    public class RobotQueue:ConcurrentQueue<RobotInfo>
    {
        public int RobotCapacity { get; set; }
        public bool CanDealloc { get { return this.Count == this.RobotCapacity; } }
    }
}
