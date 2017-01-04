using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using WebNotifications.Models;

namespace WebNotifications.Helper
{
    public class AvoidBrushCountHelper
    {
        public static double Seconds = 5;
        private static Dictionary<string, DateTime> IPTime = new Dictionary<string,DateTime>();

        public static DateTime? GetLashSubmitTime(string ip)
        {
            if (!IPTime.ContainsKey(ip)) return null;

            return IPTime[ip];
        }

        public static void RereshIP(string ip)
        {
            DateTime current = DateTime.Now;
            if (!IPTime.ContainsKey(ip))
                IPTime.Add(ip, current);
            else
                IPTime[ip] = current;
        }

        public static void RemoveIP(string ip)
        {
            if (!IPTime.ContainsKey(ip)) return;

            IPTime.Remove(ip);
        }

        public static Partner GetParter(string serizeid)
        {
            Partner parter = null;
            if (!string.IsNullOrEmpty(serizeid))
            {                int pos = serizeid.LastIndexOf('i');
                var parteId = serizeid.Substring(0, pos);
                string sid = serizeid.Substring(pos + 1);
                int iid = 0;
                int.TryParse(sid, out iid);                
                using (DataJackModelDataContext _ctx = new DataJackModelDataContext())
                {
                    parter = _ctx.Partners.FirstOrDefault(m => m.id == iid);
                }
            }

            return parter;
        }
    }
}