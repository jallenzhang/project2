using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebNotifications.Common
{
    public class PayConfigFile
    {
        #region AliPay

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
        public class AliPayConfig
        {
            static AliPayConfig()
            {
                Partner = "2088801081317382";
                Seller = "2088801081317382";
                Subject = "";
                body = "";
                privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBANRTRHQElnRC9tA0Yg90MLkP58HBSxPUkRrEKgJkiWeSCHdAx3NBhtJ7wwB9Bpwp2cZcVCl1gkTOTIPyvkE3eBSOlueWZo0fInxSCcQilfVUUMBHtnBGIvuXUJ43FpkVMLbXMoUuu+nja3mZ00TGWXGaqSkPECi6IT6/DWoTwSrrAgMBAAECgYBzVggJPbS80uNflhTucB9HZ+xsw6MU0pNABu8K/bHmIN2YagfuA9pI3BHBYikFC3bQ2baeDscEuUu3IVVX99mx0wqDOgn2YaOSDTxlkI2g2wEir/yVxK0fqhpXzzd8EAgBJvvOVsGJG08UNtTNyLOSL3n7n7BHHvdDHlEL3aPcAQJBAPrZ08bFk3QbnJ9UdLC7ytNgfPBTnYIbvg49kLueK9UC3vzPTcZt6XRS2t8zHxe0ipQsRJ76+haUZYUVmDsy62ECQQDYrv7WjWmmPOx2qPqrlmNHgtx5GkvpK66VbbQD0StE+3Cnq272DggM5PaW5K//Z2JX+aYCAOIhx0g3LT9ji6XLAkAnIjRXnhsS0fvtH0/VAnbx4uua0nCQC6PqtNAPdO2BnaEL74tCmYMCEqryhxciq5ey5fUOmDjLrPrpeCT3l0oBAkEAwqF1cFy3YEXrFK618rhz1sEZroHLW9cLy1ct0hvpJN5Bk6nuUn/KBWMIiANEf0Jq2KD612PZuwOYv08aWB0QvwJAOTKkqLlgArQ55SBlGdmSA7VzW7tkGlrSfeoso0qrpGUs24UFGNxKIQqwhVBMELtFpE+OaRbnOL0StcfypUV9PA==";
                notify_url = "";
                Input_charset_UTF8 = "utf-8";//必须为utf-8格式
                //Alipaypublick = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDUU0R0BJZ0QvbQNGIPdDC5D+fBwUsT1JEaxCoCZIlnkgh3QMdzQYbSe8MAfQacKdnGXFQpdYJEzkyD8r5BN3gUjpbnlmaNHyJ8UgnEIpX1VFDAR7ZwRiL7l1CeNxaZFTC21zKFLrvp42t5mdNExllxmqkpDxAouiE+vw1qE8Eq6wIDAQAB";
                Alipaypublick = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCQWW2vVuNCuejYeVE3ZWEAdRMKY0+RlhK53I+e zycmeuXJI2PZRkcNgWSVlZJ8mkFfD6QJKmt1PBYh8pkSBbb2thc7ZLkebwSj0QxwhizxnDfAAln5 gotjB80+erP2+kZGJ9RN9abMa1w4FCxqyCkWyKq0Tx2eyZtTzOctePv9EwIDAQAB";
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

        #endregion

        #region 91 Pay

        public static readonly string NinthOne_APP_ID = "103667";
        public static readonly string NinthOne_APP_KEY = "384cb2d0a5401ff9ceca8134c9043f69e1582124cc0b60d4";

        #endregion

        #region For Yeepay

        // 商户编号
        public static string YEEPAY_MERCHANT_ID = "10011845935";
        // 商户密钥
        public static string YEEPAY_KEY_VALUE = "VT5ZD9w7D1F2c1Ao1D3Ax317U38Ncs2509K2B83KWwL410oCU5IgdP1u0736";

        #endregion        

    }
}