using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.IO;

using log4net;
using WebNotifications.Helper;
using WebNotifications.Models.YeePay;
using WebNotifications.Utils.Yeepay;
using WebNotifications.Helper.PayHelper;
using WebNotifications.Models;
using DataPersist;

namespace WebNotifications.Controllers
{
    public class YeepayController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(YeepayController));
        private ForAllPayHelper myPayHelper = new ForAllPayHelper();

        public void Notify()
        {
            try
            {
                AddYeePayLog(Request.RawUrl);
                var buyCallbackResult = GetBuyCallbackResult();

                CallBackResult(buyCallbackResult);
            }
            catch (Exception ex)
            {
                AddYeePayLog(ex.ToString());
            }
        }

        #region Private Method

        private BuyCallbackResult GetBuyCallbackResult() {
            BuyCallbackResult result = YeepayHelper.VerifyCallback(Request["p1_MerId"], Request["r0_Cmd"], Request["r1_Code"], Request["r2_TrxId"],
                Request["r3_Amt"], Request["r4_Cur"], DecodeMyStr("r5_Pid"), Request["r6_Order"], Request["r7_Uid"],
                Request["r8_MP"], Request["r9_BType"], Request["rp_PayDate"], Request["hmac"]);

            return result;
        }

        private void CallBackResult(BuyCallbackResult result) {
            AddYeePayLog("Yeepay enter CallBackResult: " + result.ErrMsg);
            if (string.IsNullOrEmpty(result.ErrMsg))
            {
                AddYeePayLog("Yeepay enter R1_Code: " + result.R1_Code);
                //在接收到支付结果通知后，判断是否进行过业务逻辑处理，不要重复进行业务逻辑处理
                if (result.R1_Code == "1")
                {
                    AddYeePayLog(result.R5_Pid);
                    if (result.R9_BType == "1")
                    {
                        //  callback方式:浏览器重定向
                        ToDealWithDB(result);
                        Response.Write("支付成功!" +
                            "<br>接口类型:" + result.R0_Cmd +
                            "<br>返回码:" + result.R1_Code +
                            //"<br>商户号:" + result.P1_MerId +
                            "<br>交易流水号:" + result.R2_TrxId +
                            "<br>商户订单号:" + result.R6_Order +

                            "<br>交易金额:" + result.R3_Amt +
                            "<br>交易币种:" + result.R4_Cur +
                            "<br>订单完成时间:" + result.Rp_PayDate +
                            "<br>回调方式:" + result.R9_BType +
                            "<br>错误信息:" + result.ErrMsg + "<BR>");
                    }
                    else if (result.R9_BType == "2")
                    {
                        // * 如果是服务器返回则需要回应一个特定字符串'SUCCESS',且在'SUCCESS'之前不可以有任何其他字符输出,保证首先输出的是'SUCCESS'字符串
                        ToDealWithDB(result);
                        Response.Write("SUCCESS");
                        Response.Write("支付成功!" +
                             "<br>接口类型:" + result.R0_Cmd +
                             "<br>返回码:" + result.R1_Code +
                            //"<br>商户号:" + result.P1_MerId +
                             "<br>交易流水号:" + result.R2_TrxId +
                             "<br>商户订单号:" + result.R6_Order +

                             "<br>交易金额:" + result.R3_Amt +
                             "<br>交易币种:" + result.R4_Cur +
                             "<br>订单完成时间:" + result.Rp_PayDate +
                             "<br>回调方式:" + result.R9_BType +
                             "<br>错误信息:" + result.ErrMsg + "<BR>");
                    }
                }
                else
                {
                    Response.Write("支付失败!" +
                             "<br>接口类型:" + result.R0_Cmd +
                             "<br>返回码:" + result.R1_Code +
                        //"<br>商户号:" + result.P1_MerId +
                             "<br>交易流水号:" + result.R2_TrxId +
                             "<br>商户订单号:" + result.R6_Order +

                             "<br>交易金额:" + result.R3_Amt +
                             "<br>交易币种:" + result.R4_Cur +
                             "<br>订单完成时间:" + result.Rp_PayDate +
                             "<br>回调方式:" + result.R9_BType +
                             "<br>错误信息:" + result.ErrMsg + "<BR>");
                }
            }
            else
            {
                AddYeePayLog(result.ToString());
                Response.Write("交易签名无效!");               
            }
        }

        #endregion

        #region Todo With the DB if all is correct

        private void ToDealWithDB(BuyCallbackResult yeePayResult)
        {
            string out_trade_no = getOutTradeNo(yeePayResult);
            var myres = ForAllPayHelper.GetUserIdAndChannelId(out_trade_no);
            string myUserId = myres[0];
            if (myPayHelper.IsOrderAlreadyExistInDB(out_trade_no, myUserId))
            {
                var res_tuple = InsertNewPayment(yeePayResult);
                user user = myPayHelper.AddUserChips(myUserId, yeePayResult.R3_Amt.ToString(), res_tuple.Item1, res_tuple.Item2);
                myPayHelper.InsertUserMsg(user, res_tuple.Item1);
                myPayHelper.InsertUserProps(res_tuple.Item1, res_tuple.Item2, myUserId);
            }
        }

        // 插入到userpayment
        private Tuple<int, long?> InsertNewPayment(BuyCallbackResult yeePayResult)
        {
            string subject = yeePayResult.R5_Pid;
            Tuple<int, long?> result = myPayHelper.GetRoomAndChips(subject);
            int mytype = result.Item1;
            long? myitemid = result.Item2;
            decimal mymoney = decimal.Parse(yeePayResult.R3_Amt);
            var myres = ForAllPayHelper.GetUserIdAndChannelId(getOutTradeNo(yeePayResult));
            string myuserid = myres[0];
            string mychannelId = string.Empty;
            if (myres.Length > 1)
            {
                mychannelId = myres[1];
            }
            string out_trade_no = getOutTradeNo(yeePayResult);
            string mynote = yeePayResult.ToString();
            DateTime mytime = DateHelper.GetNow();

            userpayment payment = new userpayment
            {
                time = mytime,
                type = mytype,
                itemid = myitemid,
                money = mymoney,
                note = mynote,
                userid = myuserid,
                payway = (int)PayWay.Yeepay,
                alipaynote = out_trade_no,
                channelId = mychannelId,
            };

            myPayHelper.InsertToPayment(payment);

            return result;
        }

        private string getOutTradeNo(BuyCallbackResult yeePayResult)
        {
            return yeePayResult.R6_Order;
        }

        #endregion

        #region Log For YeePay

        private void AddYeePayLog(string errorStr)
        {
            string result = "YeePay flag: error:" + errorStr;
            log.Warn(result);
        }

        private void AddYeePayLog(string orderid, string str)
        {
            string result = "YeePay flag: orderid:" + orderid+"; str:" + str;
            log.Info(result);
        }

        private string DecodeMyStr(string param) {
            param = param + "=";
            int startPos = Request.RawUrl.IndexOf(param);
            string subStr = Request.RawUrl.Substring(startPos + param.Length);
            int endPos = subStr.IndexOf("&");
            if(endPos == -1){
                endPos = subStr.Length;
            }
            subStr = subStr.Substring(0, endPos);
            string res = HttpUtility.UrlDecode(subStr, Encoding.GetEncoding("gb2312"));

            return res;
        }

        #endregion

        #region Test

        public ActionResult NotifyResult()
        {
            HttpGet("http://localhost:20135/Yeepay/Notify", getYeepayData());
            return View();
        }

        private void PostDataToOurServer()
        {
            string url = "http://localhost:20135/Yeepay/Notify";
            Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
            string data = getYeepayData();
            byte[] arrB = encode.GetBytes(data);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();
        }

        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        private string getYeepayData()
        {
            string result = @"p1_MerId=10011845935&r0_Cmd=Buy&r1_Code=1&r2_TrxId=618102551291924C&r3_Amt=0.01&r4_Cur=RMB&r5_Pid=60K%B3%EF%C2%EB&r6_Order=13488134064956609u_1_1747_test&r7_Uid=&r8_MP=com.toufe.lilypoker&r9_BType=2&ru_Trxtime=20120928142355&ro_BankOrderId=857893905&rb_BankId=1000000-NET&rp_PayDate=20120928142354&rq_CardNo=&rq_SourceFee=0.0&rq_TargetFee=0.0&hmac=d7638119e0064c074305fbabe70df2b4";

            return result;
        }

        #endregion
    }
}
