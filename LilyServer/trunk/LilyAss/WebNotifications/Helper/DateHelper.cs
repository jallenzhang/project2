using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Helper
{
    public class DateHelper
    {
        public static DateTime GetNow()
        {
            return DateTime.UtcNow.AddHours(8);
        }

        public static DateTime ToMyDateTime(string passTime)
        {
            DateTime outRes = GetNow();
            if (string.IsNullOrEmpty(passTime)) {
                return outRes;
            }           
            
            if(DateTime.TryParse(passTime, out outRes)){
               return outRes; 
            }

            return GetNow();
        }

        public static DateTime GetSqlMinDate()
        {
            DateTime mindate = DateTime.Parse("1900/1/1");

            return mindate;
        }

        public static string ToMyDateTimeStr(DateTime datetime)
        {
            string res = datetime.ToString("yyy年M月dd日");
            return res;
        }
    }
}