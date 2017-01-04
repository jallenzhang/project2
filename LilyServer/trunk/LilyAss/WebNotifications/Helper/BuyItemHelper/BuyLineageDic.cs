using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataPersist;

namespace WebNotifications.Helper
{
    public class BuyLineageDic
    {
        public static Dictionary<string, int> myDic = new Dictionary<string, int>
        {
            { "皇室血统包月", 1},
            { "皇室血统 (90天)", 2},
            { "皇室血统 (180天)", 3},
            { "皇室血统包年", 4},
            { "皇室血统 (永久)", 5},
        };
    }
}