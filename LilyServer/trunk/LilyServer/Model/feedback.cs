using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer.Store
{
    public class feedback
    {
        public Int64 id { get; set; }
        public string userid { get; set; }
        public string content { get; set; }
        public DateTime time { get; set; }
    }
}
