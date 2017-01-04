using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataPersist;

namespace WebNotifications.Helper
{
    public class LilyRoomHelper
    {
        // new scence
        /*
            雍容华贵的皇宫
            古典巴洛克大厅
            未来太空漫游         
         */

        private static Dictionary<int, string> roomTypeDic = new Dictionary<int, string>
        {
            { 0, "默认"},
            { (int)RoomType.Common, "高尚富人小区"},
            { (int)RoomType.Egypt, "埃及法老墓室"},
            { (int)RoomType.Hawaii, "夏威夷风情"},
            { (int)RoomType.Japan, "日本情调桃源"},
            { (int)RoomType.West, "西部牛仔酒馆"},
            { (int)RoomType.China, "古典浪漫庭院"},
        };

        /// <summary>
        /// roomtype所代表的场景
        /// </summary>
        /// <param name="roomType"></param>
        /// <returns></returns>
        public static string GetRoomTypeName(int roomType)
        {
            if (roomTypeDic.ContainsKey(roomType))
                return roomTypeDic[roomType];
            return string.Empty;
        }
    }
}