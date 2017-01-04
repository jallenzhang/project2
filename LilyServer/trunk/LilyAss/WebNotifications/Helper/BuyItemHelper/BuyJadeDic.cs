using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Helper
{
    public class BuyJadeDic
    {
        public static Dictionary<string, int> myDic = new Dictionary<string, int>
        {
            { "康熙御玺包月", 1},
            { "康熙御玺 (90天)", 2},
            { "康熙御玺 (180天)", 3},
            { "康熙御玺包年", 4},
            { "康熙御玺 (永久)", 5},
        }; 
    }
}