using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Helper
{
    public class LilyAvatorHelper
    {
        // avator
        /*
            康熙大帝（30天）
            乾隆大帝（60天）
            雍正大帝（90天）
            孝庄太后（60天）      
         */

        private static Dictionary<int, string> avatarDic = new Dictionary<int, string>
        {
            { 0, "默认"},
            { 1, "石油大亨"},
            { 2, "饶舌歌星"},
            { 3, "海贼王"},
            { 4, "欧洲王储"},
            { 5, "意大利黑手党千金"},
            { 6, "俄罗斯富商大老婆"},
            { 7, "宋朝官员的姨太太"},
            { 8, "有钱的小萝莉"},
            { 9, "Guest"},
        };

        /// <summary>
        /// 头像所代表的名称
        /// </summary>
        /// <param name="avatorId"></param>
        /// <returns></returns>
        public static string GetAvatorName(int? avatorId)
        {
            int id = avatorId == null ? 0 : (int)avatorId;
            if (avatarDic.ContainsKey(id))
                return avatarDic[id];
            return string.Empty;
        }
    }
}