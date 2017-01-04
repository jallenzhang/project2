using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebNotifications.Helper;
using WebNotifications.Common;
using WebNotifications.Utils.AliSecuriy;

namespace WebNotifications.Helper.PayHelper
{
    public class AlipayHelper
    {
        /// <summary>
        /// 1: not pass; 2:success
        /// </summary>
        /// <returns></returns>
        public static string CallBack(string psign, string pcontent) {
            //得到签名
            string sign = HttpUtility.HtmlDecode(psign.ToString());

            //得到待签名字符串
            string content = HttpUtility.HtmlDecode(pcontent.ToString());

            //验签数据
            bool isVerify = Function.Verify(content, sign, PayConfigFile.AliPayConfig.Alipaypublick, PayConfigFile.AliPayConfig.Input_charset_UTF8);

            //请结合客户端，本示例返回1表示验签不通过，返回2表示验签通过
            if (!isVerify)
            {
                return "1";
            }
            else
            {
                return "2";
            }
        }

        /// <summary>
        /// Should output the result "success" or "fail" on the page
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Nofity(Dictionary<string, string> request)
        {
            //获取notify_data数据，需要添加notify_data=
            //不需要解密，直接是明文xml格式
            string notify_data = "notify_data=" + request["notify_data"];

            //获取签名
            string sign = request["sign"];

            //验证签名
            bool vailSign = Function.Verify(notify_data, sign, PayConfigFile.AliPayConfig.Alipaypublick, PayConfigFile.AliPayConfig.Input_charset_UTF8);
            if (!vailSign)
            {
                return null;
            }

            //获取交易状态
            string trade_status = Function.GetStrForXmlDoc(request["notify_data"].ToString(), "notify/trade_status");
            if (!trade_status.Equals("TRADE_FINISHED"))
            {
                return null;
            }
            else
            {
                //成功必须在页面上输出success，支付宝才不会再发送通知
                var alixPayObj = GetAlixObject(request["notify_data"]);

                return alixPayObj;
            }
        }

        private static Dictionary<string, string> GetAlixObject(string notify_data)
        {
            ///////////////////////////////处理数据/////////////////////////////////
            // 用户这里可以写自己的商业逻辑
            // 例如：修改数据库订单状态
            // 以下数据仅仅进行演示如何调取
            // 参数对照请详细查阅开发文档
            // 里面有详细说明
            string partner = Function.GetStrForXmlDoc(notify_data, "notify/partner");
            string discount = Function.GetStrForXmlDoc(notify_data, "notify/discount");
            string payment_type =  Function.GetStrForXmlDoc(notify_data, "notify/payment_type");
            string subject = Function.GetStrForXmlDoc(notify_data, "notify/subject");
            string trade_no = Function.GetStrForXmlDoc(notify_data, "notify/trade_no");
            string buyer_email = Function.GetStrForXmlDoc(notify_data, "notify/buyer_email");
            string gmt_create = Function.GetStrForXmlDoc(notify_data, "notify/gmt_create");
            string quantity = Function.GetStrForXmlDoc(notify_data, "notify/quantity");
            string out_trade_no = Function.GetStrForXmlDoc(notify_data, "notify/out_trade_no");
            string seller_id = Function.GetStrForXmlDoc(notify_data, "notify/seller_id");
            string is_total_fee_adjust = Function.GetStrForXmlDoc(notify_data, "notify/trade_status");
            string total_fee = Function.GetStrForXmlDoc(notify_data, "notify/total_fee");
            string gmt_payment = Function.GetStrForXmlDoc(notify_data, "notify/gmt_payment");
            string seller_email = Function.GetStrForXmlDoc(notify_data, "notify/seller_email");
            string gmt_close = Function.GetStrForXmlDoc(notify_data, "notify/gmt_close");
            string price = Function.GetStrForXmlDoc(notify_data, "notify/price");
            string buyer_id = Function.GetStrForXmlDoc(notify_data, "notify/buyer_id");
            string use_coupon = Function.GetStrForXmlDoc(notify_data, "notify/use_coupon");
            ////////////////////////////////////////////////////////////////////////////

            Dictionary<string, string> alixObj = new Dictionary<string, string>();
            alixObj.Add("partner", partner);
            alixObj.Add("discount", discount);
            alixObj.Add("payment_type", payment_type);
            alixObj.Add("subject", subject);
            alixObj.Add("trade_no", trade_no);
            alixObj.Add("buyer_email", buyer_email);
            alixObj.Add("gmt_create", gmt_create);
            alixObj.Add("quantity", quantity);
            alixObj.Add("out_trade_no", out_trade_no);
            alixObj.Add("seller_id", seller_id);
            alixObj.Add("is_total_fee_adjust", is_total_fee_adjust);
            alixObj.Add("total_fee", total_fee);
            alixObj.Add("gmt_payment", gmt_payment);
            alixObj.Add("seller_email", seller_email);
            alixObj.Add("gmt_close", gmt_close);
            alixObj.Add("price", price);
            alixObj.Add("buyer_id", buyer_id);
            alixObj.Add("use_coupon", use_coupon);

            var ourAppendValueStr = ForAllPayHelper.GetUserIdAndChannelId(out_trade_no);
            alixObj.Add("user_id", ourAppendValueStr[0]);
            if (ourAppendValueStr.Length > 1)
            {
                alixObj.Add("channel_id", ourAppendValueStr[1]);
            }
            else {
                alixObj.Add("channel_id", string.Empty);
            }

            return alixObj;
        }
    }
}