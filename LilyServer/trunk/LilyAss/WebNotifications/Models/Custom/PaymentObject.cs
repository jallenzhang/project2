using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Models.Custom
{
    public class PaymentObject
    {
        public string userid { get; set; }
        public string mail { get; set; }
        public string nickname { get; set; }
        public DateTime? time { get; set; }
        public string devicetype { get; set; }
        public string deviceToken { get; set; }
        public string note { get; set; }
        public string buyway { get; set; }
        public string buyItem { get; set; }
        public string ItemName { get; set; }
        public string buyMoney { get; set; }
    }
}