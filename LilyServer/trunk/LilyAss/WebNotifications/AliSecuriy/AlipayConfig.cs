using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace WebNotifications.AliSecuriy
{
    /// <summary>
    /// 类名：Config
    /// 功能：支付宝配置公共类
    /// 详细：该类是配置所有请求参数，支付宝网关、接口，商户的基本参数等
    /// 版本：1.0
    /// 日期：2011-09-15
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class Config
    {
        static Config()
        {
            Partner = "2088801081317382";
            Seller = "2088801081317382";
            Subject = "";
            body = "";
            privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAMyOMQOb/28zUwnOCDfYnyeB0OmNtt6QDm6kITn8JC+SOjJD7QAKSpdtZGB0CnnrmsHmEdVVBdWuWgmF7zOX2OLx1LUXGBrfI5SES/0sQ+svPeJDG4f07nju+SY28F35t2RAhLDMGGfGL7AYFxeH0hSI5yOg9N1g6R64bo4S8UwzAgMBAAECgYA80FlKFWrPiJa7Z4GR+Nj4SePuS69+Y52mzy0BrDCl4/dhUkh3ppeehWs+McGblawg0WGj5u8hJACorCT9Vbuo1HS3eGkI66B327FSU6HJwdiYwr992kPSg54jMpLa8NwgjLj1zky5eGd5+CYIaFavxU4/3Q4+w+pt6+216INJAQJBAPfVcPjer4dQ3uF7F1vihKaFwrlonAuGNw295t96QraeO1ooeBiI4A6uX4H3fUeZAwGKBfVvFCfMkC6jvYE6ZuECQQDTS67zEbjauMDu9D3HtQ0kJhaqH2OccX84Tt3J2UJ4+vlgy7ZEbJVd1Q7KkwIR/E/3H6mit9JW5SOA3NE5JFmTAkEAlLEijq5Mccs7bd0ELsTBAYfPRJ5WwTNNZJlDI2GfFSHqSjVtmIrGowhLlRZ/u605+HpvCMoUNayt9M2YrSf1AQJAaWNmb3Z3bSCZmpXX+rQjSdR1mYTueilh+wPbO8JRlWYY3F6/GoHOPm72YbPRZIckm23/flmRYCYJ/0wkTwwCYQJAU07SI7aryaE8Vnh4dcg02to9ZGzgcHrTQ20hUaATPwe4k1oedWwa8vje7xt9Zz3A5kdPWq25TAs8HDDfuRwwEg==";
            notify_url = "";
            Input_charset_UTF8 = "utf-8";//必须为utf-8格式
            Alipaypublick = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCdmvQbBrRHvYkmy2hhn0EnAQIrVSs7meWg3O9Lz97/s58yi+HN94juytoJimRCa1TgXDLkzLB2nK5HAFgfI93w0foRwveupWljn9LJvynUc63RBspxXp0GcEkF0PksjA0HU0wntZk5NIuuMVYHM5zl/N9+J1WE6st4VIqcAdxzlwIDAQAB";
        }

        #region 属性

        /// <summary>
        /// 合作商户ID
        /// </summary>
        public static string Partner
        { get; set; }
        /// <summary>
        /// 卖家帐号
        /// </summary>
        public static string Seller
        { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public static string Subject
        { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public static string body
        { get; set; }
        /// <summary>
        /// 商户私钥
        /// </summary>
        public static string privateKey
        { get; set; }
        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public static string Alipaypublick
        { get; set; }
        /// <summary>
        /// 编码格式UTF-8
        /// </summary>
        public static string Input_charset_UTF8
        { get; set; }
        /// <summary>
        /// Notify异步通知URL
        /// </summary>
        public static string notify_url
        { get; set; }

        #endregion
    }
}