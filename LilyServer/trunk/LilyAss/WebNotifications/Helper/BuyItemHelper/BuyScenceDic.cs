using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataPersist;

namespace WebNotifications.Helper
{
    public class BuyScenceDic
    {
        public static Dictionary<string, int> myDic = new Dictionary<string, int>
        {
            { "高尚富人小区", 1},
            { "埃及法老墓室", 2},
            { "夏威夷风情", 4},
            { "日本情调桃源", 8},
            { "西部牛仔酒馆", 16},
            { "古典浪漫庭院", 32},
            // 以上已经买了，所以id不能动 
   
            { "雍容华贵的皇宫", 64},
            { "古典巴洛克大厅", 128},
            { "未来太空漫游", 256},
        };
    }
}