using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer.Store
{
    public class item
    {
        public Int64 id { get; set; }
        public string userid { get; set; }
        public int type { get; set; }
        public bool used { get; set; }
    }
}
