using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebNotifications.Models.Gift;

namespace WebNotifications.Helper
{
    public class LilyGiftHelper
    {
        private static Dictionary<int, string> giftDic = null;

        public static string GetGiftNameById(object giftId)
        {
            if (giftDic == null)
            {
                string binaryPath = AppDomain.CurrentDomain.BaseDirectory;
                GiftManager.Singleton.LoadFile(binaryPath + "/Resources/Gifts.xml");
                giftDic = GiftManager.Singleton.GiftNames;
            }

            if (giftId == null) return string.Empty;

            string gid = giftId.ToString();
            int id = -1;
            if (!int.TryParse(gid, out id))
                return string.Empty;

            if (giftDic.ContainsKey(id))
                return giftDic[id];
            return string.Empty;
        }
    }
}