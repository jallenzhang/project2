using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LilyServer.Store
{
    public class usermessage
    {
        public Int64 id { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string content { get; set; }
        public DateTime time { get; set; }
    }
}

