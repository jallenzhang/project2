using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Helper
{
    public class BuyChipsDic
    {
        public static Dictionary<string, long> myDic = new Dictionary<string, long>
        {
            { "60K筹码", 60000},
            { "120K筹码", 120000},
            { "250K筹码", 250000},
            { "300K筹码", 300000},
            { "700K筹码", 700000},
            { "3.4M筹码", 3400000},
            { "6.8M筹码", 6800000},
            { "36M筹码", 36000000},
            { "72M筹码", 72000000},
        };
    }
}