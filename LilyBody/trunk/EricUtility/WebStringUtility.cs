//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;
//
//namespace EricUtility
//{
//    public class WebStringUtility
//    {
//        private static string HtmlEncode(string text)
//        {
//            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
//            StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));
//
//            foreach (char c in chars)
//            {
//                int value = Convert.ToInt32(c);
//                if (value > 127 || value == 32)
//                    result.AppendFormat("&#{0};", value);
//                else
//                    result.Append(c);
//            }
//
//            return result.ToString();
//        }
//        public static string EncodeString(string text)
//        {
//            return HtmlEncode(text);
//        }
//        public static string EncodeUrl(string text)
//        {
//            return HttpUtility.UrlEncode(text);
//        }
//        public static string DecodeString(string text)
//        {
//            return HttpUtility.HtmlDecode(text);
//        }
//        public static string DecodeURL(string text)
//        {
//            return HttpUtility.UrlDecode(text);
//        }
//    }
//}
