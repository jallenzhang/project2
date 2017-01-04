using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;

using DataPersist;
using WebNotifications.Utils.AliSecuriy;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Models.Custom;
using WebNotifications.Helper.PayHelper;
using log4net;

namespace WebNotifications.Controllers
{
    public class AlixPayController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AlixPayController));
        private ForAllPayHelper myPayHelper = new ForAllPayHelper();

        // private static string R_Str = string.Empty;        

        public string Notify()
        {
            return "";
        }        

        [HttpPost]
        [ValidateInput(false)]
        public string Notify(string sign, string sign_type, string notify_data)
        {
            sign = sign.Replace(" ", "+");

            // R_Str += GetNote(sign, sign_type, notify_data) + "; ";
            var aliRequest = new Dictionary<string, string>();
            aliRequest.Add("sign", sign);
            aliRequest.Add("sign_type", sign_type);
            aliRequest.Add("notify_data", notify_data);

            string result = "fail";
            try {                
                var alixPayObj = AlipayHelper.Nofity(aliRequest);
                if (null != alixPayObj)
                {
                    if (CheckAndInsert(alixPayObj, sign, sign_type, notify_data))
                    {
                        result = "success";
                    }
                }                
            }
            catch(Exception ex) {
                log.Info(ex.ToString());                
            }

            if (result == "fail")
            {
                AddAlilyPayLog(sign, sign_type, notify_data);
            }

            return result;
        }

        //public string NotifyResult()
        //{
        //    return R_Str;
        //}
 
        #region To DB

        // 插入到所对应的数据表
        private bool CheckAndInsert(Dictionary<string, string> alixPayObj, string sign, string sign_type, string notify_data)
        {
            string out_trade_no = alixPayObj["out_trade_no"];
            string myuserid = alixPayObj["user_id"];  
            
            if(myPayHelper.IsOrderAlreadyExistInDB(out_trade_no, myuserid))            
            {
                var res_tuple = InsertNewPayment(alixPayObj, sign, sign_type, notify_data);
                user user = myPayHelper.AddUserChips(myuserid, alixPayObj["price"], res_tuple.Item1, res_tuple.Item2);
                myPayHelper.InsertUserMsg(user, res_tuple.Item1);
                myPayHelper.InsertUserProps(res_tuple.Item1, res_tuple.Item2, myuserid);
                return true;
            }

            return false;
        }

        // 插入到userpayment
        private Tuple<int, long?> InsertNewPayment(Dictionary<string, string> alixPayObj, string sign, string sign_type, string notify_data)
        {
            DateTime mytime = DateHelper.GetNow();
            string subject = alixPayObj["subject"];
            Tuple<int, long?> result =  myPayHelper.GetRoomAndChips(subject);
            int mytype = result.Item1;
            long? myitemid = result.Item2;
            decimal mymoney = GetMoneyDecimalType(alixPayObj["total_fee"]);
            string myuserid = alixPayObj["user_id"];
            string out_trade_no = alixPayObj["out_trade_no"];
            string myalipaynote = GetNote(sign, sign_type, notify_data);
            string mychannelId = alixPayObj["channel_id"];

            userpayment payment = new userpayment
            {
                time = mytime,
                type = mytype,
                itemid = myitemid,
                money = mymoney,
                note = myalipaynote,
                userid = myuserid,
                payway = (int)PayWay.Alipay,
                alipaynote = out_trade_no,
                channelId = mychannelId,
            };

            myPayHelper.InsertToPayment(payment);

            return result;
        }

        private decimal GetMoneyDecimalType(string money) {
            decimal result = 0;
            decimal.TryParse(money, out result);

            return result;
        }

        private string GetNote(string sign, string sign_type, string notify_data)
        {
            string note = "sign=" + sign + "&sign_type=" + sign_type + "&notify_data=" + notify_data;
            //note = Url.Encode(note);

            return note;
        }

        #endregion

        #region Log to LilyAss

        private void AddAlilyPayLog(string sign, string sign_type, string notify_data)
        {
            string result = "sign:" + sign
                + ";sign_type:" + sign_type
                + " ;notify_data:" + notify_data;
            log.Info(result);
        }

        #endregion

        public ActionResult NotifyResult()
        {
            PostDataToOurServer();
            return View();
        }

        private void PostDataToOurServer()
        {
            string url = "http://localhost:20135/Alixpay/Notify";
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            string data = getAlixData();
            byte[] arrB = encode.GetBytes(data);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();
        }

        private string getAlixData()
        {
            string result = @"sign=mYVWk/dzdSYNFhXL8iVsEtnyJaOV+BOvv3ScQvoef66mVv6lHT4pgo8nphoHKNunPhJLAhhbuwQm+G6ABRlNOOYC6piP2Om9ljhpA6rtqg/Wgygdu2jwG/3HBfGZbG1ZXGJCBkSx1avhIkBBYpVKDykwx/FoQoEln96kym4MLEc=&sign_type=RSA&notify_data=<notify><partner>2088801081317382</partner><discount>0.00</discount><payment_type>1</payment_type><subject>60K筹码</subject><trade_no>2012091856913875</trade_no><buyer_email>poker@toufe.com</buyer_email><gmt_create>2012-09-18 21:34:35</gmt_create><quantity>1</quantity><out_trade_no>0918213426_1_1637_test</out_trade_no><seller_id>2088801081317382</seller_id><trade_status>TRADE_FINISHED</trade_status><is_total_fee_adjust>N</is_total_fee_adjust><total_fee>6.00</total_fee><gmt_payment>2012-09-18 21:34:35</gmt_payment><seller_email>pay@toufe.com</seller_email><gmt_close>2012-09-18 21:34:35</gmt_close><price>6.00</price><buyer_id>2088802200430755</buyer_id><use_coupon>N</use_coupon></notify>";

            return result;
        }
    }
}
