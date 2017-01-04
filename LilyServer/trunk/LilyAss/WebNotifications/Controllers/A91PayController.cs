using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebNotifications.Models.A91Pay;
using WebNotifications.Helper;
using log4net;
using WebNotifications.Models;
using DataPersist;
using System.Text;
using System.Net;
using System.IO;
using WebNotifications.Common;

namespace WebNotifications.Controllers
{
    public class A91PayController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(A91PayController));
        private ForAllPayHelper myPayHelper = new ForAllPayHelper();
        //
        // GET: /91Pay/

        public string Notify()
        {
            Add91PayLog("91 center call my notify function.");
            string myResult = RecvFrom91Center();
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();

            return myResult;
        }

        /// <summary>
        /// 接收来自支付中心的订单结果
        /// </summary>
        private string RecvFrom91Center() {
            int result = 0;
            string errorDesc = "未知错误";

            string act = Request["Act"];
            switch (act)
            {
                case "1":
                    result = Process_Act1(out errorDesc);
                    break;
                case "2":
                    result = Process_Act2(out errorDesc);
                    break;
                default:
                    result = 3; // Act 无效
                    break;
            }

            Add91PayLog(act);

            // 返回结果给通用平台服务器
            Response.Clear();
            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            resultDict.Add("ErrorCode", result.ToString());
            resultDict.Add("ErrorDesc", errorDesc);

            return resultDict.ToJson();
        }

        /// <summary>
        /// 接收通用平台的支付成功通知
        /// </summary>
        /// <param name="errorDesc"></param>
        /// <returns></returns>
        private int Process_Act1(out string errorDesc)
        {
            int result = 0;
            errorDesc = "接收失败";
            try
            {
                NdBuyItemObj myItem = new NdBuyItemObj();
                myItem.productId = int.Parse(Request["AppId"]); // 应用ID
                myItem.productName = Request["ProductName"]; // 应用名称
                myItem.consumeStreamId = Request["ConsumeStreamId"]; // 消费流水号
                myItem.cooOrderSerial = Request["CooOrderSerial"]; // 商户订单流水号
                myItem.uin = long.Parse(Request["Uin"]); // 91账号ID
                myItem.goodsID = Request["GoodsID"]; // 商品ID
                myItem.goodsInfo = Request["GoodsInfo"]; // 商品名称
                myItem.goodsCount = int.Parse(Request["GoodsCount"]); // 商品数量
                myItem.originalMoney = decimal.Parse(Request["OriginalMoney"]); // 原价
                myItem.orderMoney = decimal.Parse(Request["OrderMoney"]); // 实际价格
                myItem.note = Request["Note"]; // 备注信息
                myItem.payStatus = int.Parse(Request["PayStatus"]); // 支付状态：0=失败，1=成功
                myItem.createTime = Request["CreateTime"]; // 订单流水创建时间
                myItem.Sign = Request["Sign"];

                // 检查校验码
                string mySign = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9:0.00}{10:0.00}{11}{12}{13:yyyy-MM-dd HH:mm:ss}{14}",
                    myItem.productId, 1, myItem.productName, myItem.consumeStreamId, myItem.cooOrderSerial, myItem.uin, myItem.goodsID, myItem.goodsInfo,
                    myItem.goodsCount, myItem.originalMoney, myItem.orderMoney, myItem.note, myItem.payStatus, myItem.createTime, PayConfigFile.NinthOne_APP_KEY).HashToMD5Hex();

                Add91PayLog(myItem.ToString());

                if (mySign == myItem.Sign)
                {
                    if (myItem.payStatus == 1) // 通用平台支付成功
                    {
                        // TODO: 支付成功，给Uin对应的用户分发所购买的物品，这步由各个开发商自行完成。
                        ToDealWithDB(myItem);
                    }
                    result = 1;
                    errorDesc = "接收成功";
                }
                else
                {
                    result = 5;
                    errorDesc = "Sign无效";
                }
            }
            catch(Exception ex)
            {
                Add91PayLog(ex.ToString());
            }
            return result;
        }
        
        /// <summary>
        /// 接收通用平台的虚拟币直充成功通知
        /// </summary>
        /// <param name="errorDesc"></param>
        /// <returns></returns>
        private int Process_Act2(out string errorDesc)
        {
            int result = 0;
            errorDesc = "接收失败";
            try
            {
                int productId = int.Parse(Request["ProductId"]); // 应用ID
                string productName = Request["ProductName"]; // 应用名称
                string creditOrderNo = Request["CreditOrderNo"]; // 充值订单流水号
                long uin = long.Parse(Request["Uin"]); // 91账号ID
                decimal creditMoney = decimal.Parse(Request["CreditMoney"]); // 充值金额
                string note = Request["Note"]; // 备注信息
                string creditTime = Request["CreditTime"]; // 充值时间
                int creditStatus = int.Parse(Request["CreditStatus"]); // 支付状态：0=失败，1=成功
                string Sign = Request["Sign"];

                // 检查校验码
                string mySign = String.Format("{0}{1}{2}{3}{4}{5:0.00}{6}{7:yyyy-MM-dd HH:mm:ss}{8}{9}",
                    productId, 2, productName, creditOrderNo, uin, creditMoney,
                    "", creditTime, creditStatus, PayConfigFile.NinthOne_APP_KEY).HashToMD5Hex();

                if (mySign == Sign)
                {
                    if (creditStatus == 1) // 通用平台虚拟币直充成功
                    {
                        // TODO: 支付成功，给Uin对应的用户分发所购买的物品，这步由各个开发商自行完成。
                    }
                    result = 1;
                    errorDesc = "接收成功";
                }
                else
                {
                    result = 5;
                    errorDesc = "Sign无效";
                }
            }
            catch
            {
            }
            return result;
        }

        #region Log For 91Pay

        private void Add91PayLog(string info)
        {
            string result = "91flag: " + info;
            log.Info(result);            
        }

        #endregion

        #region Todo With the DB if all is correct

        private void ToDealWithDB(NdBuyItemObj ndItem)
        {
            string out_trade_no = getOutTradeNo(ndItem);
            var myres = ForAllPayHelper.GetUserIdAndChannelId(out_trade_no);
            string myUserId = myres[0];
            if (myPayHelper.IsOrderAlreadyExistInDB(out_trade_no, myUserId))
            {
                var res_tuple = InsertNewPayment(ndItem);
                user user = myPayHelper.AddUserChips(myUserId, ndItem.orderMoney.ToString(), res_tuple.Item1, res_tuple.Item2);
                myPayHelper.InsertUserMsg(user, res_tuple.Item1);
                myPayHelper.InsertUserProps(res_tuple.Item1, res_tuple.Item2, myUserId);
            }
        }

        // 插入到userpayment
        private Tuple<int, long?> InsertNewPayment(NdBuyItemObj ndItem)
        {
            string subject = ndItem.goodsInfo;
            Tuple<int, long?> result = myPayHelper.GetRoomAndChips(subject);
            int mytype = result.Item1;
            long? myitemid = result.Item2;
            decimal mymoney = ndItem.orderMoney;
            var myres = ForAllPayHelper.GetUserIdAndChannelId(getOutTradeNo(ndItem));
            string myuserid = myres[0];
            string mychannelId = string.Empty;
            if(myres.Length > 1){
                mychannelId = myres[1];
            }
            string out_trade_no = getOutTradeNo(ndItem);
            string myalipaynote = ndItem.Sign; ;
            DateTime mytime = DateHelper.GetNow();

            userpayment payment = new userpayment
            {
                time = mytime,
                type = mytype,
                itemid = myitemid,
                money = mymoney,
                note = myalipaynote,
                userid = myuserid,
                payway = (int)PayWay.NineOnePay,
                alipaynote = out_trade_no,
                channelId = mychannelId,
            };

            myPayHelper.InsertToPayment(payment);

            return result;
        }

        private string getOutTradeNo(NdBuyItemObj ndItem)
        {
            return ndItem.goodsID;
        }

        #endregion

        private class NdBuyItemObj
        {
            public int productId; // 应用ID
            public string productName; // 应用名称
            public string consumeStreamId; // 消费流水号
            public string cooOrderSerial; // 商户订单流水号
            public long uin; // 91账号ID
            public string goodsID; // 商品ID
            public string goodsInfo; // 商品名称
            public int goodsCount; // 商品数量
            public decimal originalMoney; // 原价
            public decimal orderMoney; // 实际价格
            public string note; // 备注信息
            public int payStatus; // 支付状态：0=失败，1=成功
            public string createTime; // 订单流水创建时间
            public string Sign;

            public override string ToString()
            {
                string res = string.Format("productId:{0};consumeStreamId:{1};cooOrderSerial:{2};uin:{3};goodsID:{4};goodsInfo:{5};note:{6}", productId, consumeStreamId, cooOrderSerial, uin, goodsID, goodsInfo, note);
                return res;
            }
        }

        #region Test

        public ActionResult NotifyResult()
        {
            PostDataToOurServer();
            return View();
        }

        private void PostDataToOurServer()
        {
            string url = "http://localhost:20135/A91Pay/Notify";
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            string data = get91Data();
            byte[] arrB = encode.GetBytes(data);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            myReq.Method = "POST";
            myReq.ContentType = "application/x-www-form-urlencoded";
            myReq.ContentLength = arrB.Length;
            Stream outStream = myReq.GetRequestStream();
            outStream.Write(arrB, 0, arrB.Length);
            outStream.Close();
        }

        private string get91Data()
        {
            string result = @"AppId=103667&Act=1&ProductName=康熙御玺包月&ConsumeStreamId=3-24962-20120920140232-10-4595&CooOrderSerial=4ecc055c86ed410f9fe951f950cbccbb&Uin=146779639&GoodsId=N9FKYR45LNGQESYN9FKYR45LNGQESY_1_1283_hz_91&GoodsInfo=康熙御玺包月&GoodsCount=1&OriginalMoney=0.01&OrderMoney=0.01&Note=购买60K筹码&PayStatus=1&CreateTime=2010-12-14 23:34:21&Sign=2667575c8582b52a5fe5aa09141b076b";

            return result;
        }

        #endregion
    }
}
