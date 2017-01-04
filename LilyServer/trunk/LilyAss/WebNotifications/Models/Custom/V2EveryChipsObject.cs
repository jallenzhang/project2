using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Models.Custom
{
    public class V2EveryChipsObject
    {
        public string userid { get; set; }
        public string mail { get; set; }
        public string nickname { get; set; }
        public int? devicetype { get; set; }
        public int? usertype { get; set; }
        public long? everyDayAward { get; set; }
        public DateTime? createtime { get; set; }
    }
}