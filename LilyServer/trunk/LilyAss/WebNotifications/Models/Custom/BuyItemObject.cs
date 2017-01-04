using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Models.Custom
{
    public class BuyItemObject
    {
        public string name { get; set; }
        public int? typeId { get; set; }
        public long? itemId { get; set; }
        public string buyTimes { get; set; }
        public string peopleCount { get; set; }
        public DateTime? dateTime { get; set; }
        public string userid { get; set; }
    }

    public class BuyItemObjectCompare : IEqualityComparer<BuyItemObject>
    {
        public bool Equals(BuyItemObject x, BuyItemObject y)
        {
            return (x.userid == y.userid);
        }

        public int GetHashCode(BuyItemObject obj)
        {
            return obj.ToString().ToLower().GetHashCode();
        }
    }
}