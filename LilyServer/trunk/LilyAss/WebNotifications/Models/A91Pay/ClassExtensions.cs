using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;

namespace WebNotifications.Models.A91Pay
{
    public static class ClassExtensions
    {

        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>Json格式的字符串</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        #region HashToMD5Hex UTF8编码字符串计算MD5值(十六进制编码字符串)
        /// <summary>
        /// UTF8编码字符串计算MD5值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>MD5(十六进制编码字符串)</returns>
        public static string HashToMD5Hex(this string sourceStr)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Bytes);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                    sBuilder.Append(result[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
        #endregion

        #region UrlEncode 编码文本中的特殊字符为Url转义字符
        /// <summary>
        /// 编码文本中的特殊字符为Url转义字符
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>编码过的文本</returns>
        public static string UrlEncode(this string sourceStr)
        {
            return HttpUtility.UrlEncode(sourceStr);
        }
        #endregion

    }
}