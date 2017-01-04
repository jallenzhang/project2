using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Models
{
    public class DeviceUserObj
    {
        public DateTime DateTime { get; set; }
        public int TotalAmount { get; set; }
        public int RegisterAmount { get; set; }
        public int CommunityAmount { get; set; }
        public int GuestAmount { get; set; }
        public int GuestToRegister { get; set; }
    }
}