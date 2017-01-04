using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
    public enum StatusTipsType : byte
    {
        // 好友上线
        FriendUp = 1,

        // 购买筹码信息
        BuyChips = 2,

        // 购买场景信息
        BuyScence = 3,

        // 购买角色信息
        BuyAvator = 4,

        // VIP开通信息
        BeVipPlayer = 5,

        // 康熙玉玺开通信息
        KangxiJade = 6,

        // 皇室血统开通信息    
        KangxiLineage = 7,

        // 某玩家获得同花顺信息
        StraightFlush = 8,

        // 某玩家获得皇家同花顺信息  
        RoyalStraightFlush = 9
    }
}
