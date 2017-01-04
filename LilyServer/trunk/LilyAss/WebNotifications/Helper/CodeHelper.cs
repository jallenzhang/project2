using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace WebNotifications.Helper
{
    public class CodeHelper
    {
        public static long GetNumberOrZero(long? num)
        {
            long zero = 0;
            if (null == num) return zero;
            return string.IsNullOrEmpty(num.ToString()) ? zero : (long)num; 
        }

        public static decimal GetNumberOrZero(decimal? num)
        {
            decimal zero = 0;
            if (null == num) return zero;
            return string.IsNullOrEmpty(num.ToString()) ? zero : (long)num; 
        }

        public static bool IsToday(DateTime? datetime)
        {
            if (null == datetime) return false;

            return datetime.Value.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd");
        }

        public static string GetEncodeStr(string str)
        {
            str = ToHexString(str);
            return str;
        }

        private static string ToHexString(string s) 
        { 
            char[] chars = s.ToCharArray(); 
            StringBuilder builder = new StringBuilder(); 
            for (int index = 0; index < chars.Length; index++) 
            { 
                bool needToEncode = NeedToEncode(chars[index]); 
                if (needToEncode) 
                { 
                    string encodedString = ToHexString(chars[index]); 
                    builder.Append(encodedString);
                } 
                else 
                { 
                    builder.Append(chars[index]);
                } 
            }             
            return builder.ToString(); 
        }

        private static string ToHexString(char chr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < encodedBytes.Length; index++)
            {
                builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
            }

            return builder.ToString();
        }

        private static bool NeedToEncode(char chr) 
        { 
            string reservedChars = "$-_.+!*'(),@=&"; 
            if (chr > 127)                
                return true; 
            
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)                
                return false; 
            return true; 
        } 
    }
}