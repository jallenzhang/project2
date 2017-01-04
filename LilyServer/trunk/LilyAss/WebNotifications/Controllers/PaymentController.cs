using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Text;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;
using WebNotifications.Models;
using WebNotifications.Models.Custom;
using WebNotifications.Helper;
using DataPersist;

namespace WebNotifications.Controllers
{
    [Description("付费用户信息(底下权限可以继承当前权限)")]
    public class PaymentController : BaseController
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();        
        private string session_paymentStart_key = "paymentStart";
        private string session_paymentEnd_key = "paymentEnd";

        [Description("付费用户信息")]
        public ActionResult PaymentInfo(int? page)
        {
            var resList = GetPaymentQueryableStr();
            var pageList = resList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            ViewData["tbStartTime"] = DateHelper.GetNow().AddYears(-1).ToShortDateString();
            ViewData["tbEndTime"] = DateHelper.GetNow().AddDays(1).ToShortDateString();

            return View(pageList);
        }

        [Description("付费用户信息查询")]
        public ActionResult PaymentInfoSearch(int? page, string tbStartTime, string tbEndTime)
        {
            DateTime start = DateHelper.GetNow();
            DateTime end = DateHelper.GetNow();
            if (tbStartTime != null || tbEndTime != null)
            {
                start = ParseStrToDateTime(tbStartTime);
                end = ParseStrToDateTime(tbEndTime);
                Session[session_paymentStart_key] = start.ToShortDateString();
                Session[session_paymentEnd_key] = end.ToShortDateString();
            }
            else {
                start = ParseStrToDateTime(Session[session_paymentStart_key] as string);
                end = ParseStrToDateTime(Session[session_paymentEnd_key] as string);
                ViewData["tbStartTime"] = Session[session_paymentStart_key] as string;
                ViewData["tbEndTime"] = Session[session_paymentEnd_key] as string;
            }
            
            var resList = GetPaymentQueryableStr(start, end).ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View("PaymentInfo", resList);
        }

        [Description("付费用户信息下载")]
        public ActionResult PaymentInfoDownload()
        {
            string conent = GeneratePaymentInfoExcel();

            ExportToFile.DownLoadAsExcel("付费用户信息 ", conent);

            return RedirectToAction("PaymentInfo");
        }

        [Description("付费道具购买详情")]
        public ActionResult BuyItemsInfo()
        {
            var resList = this.GetBuyItemList();

            return View(resList);
        }

        
        [Description("付费道具购买详情下载")]
        public ActionResult BuyItemInfoDownload()
        {
            string conent = this.GenerateBuyItemsInfoExcel();

            ExportToFile.DownLoadAsExcel("付费道具购买信息 ", conent);

            return RedirectToAction("BuyItemsInfo");
        }        

        #region Private method

        /// <summary>
        /// 付费用户信息下载 
        /// </summary>
        /// <returns></returns>
        private string GeneratePaymentInfoExcel()
        {
            var resList = GetPaymentQueryableStr();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>user-id</th><th>时间</th><th>设备</th><th>购买渠道</th><th>购买金额</th><th>购买物品</th><th>保存的字符串</th><th>DeviceToken</th></tr>");

            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.mail ?? string.Empty + "</td>");
                //sb.Append("<td>" + item.nickname + "</td>");
                sb.Append("<td>" + item.time + "</td>");
                sb.Append("<td>" + item.devicetype + "</td>");
                sb.Append("<td>" + item.buyway + "</td>");
                sb.Append("<td>" + item.buyMoney + "</td>");
                sb.Append("<td>" + item.buyItem + "</td>");
                sb.Append("<td>" + item.note + "</td>");
                sb.Append("<td>" + item.deviceToken + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private IQueryable<PaymentObject> GetPaymentQueryableStr()
        {
            var resList = (from p in dataContext.userpayments
                           from u in dataContext.users
                           where p.userid == u.userid
                           orderby p.time descending
                           select new PaymentObject
                           {
                               userid = u.userid,
                               mail = u.mail,
                               nickname = Server.HtmlEncode(u.nickname),
                               time = p.time,
                               deviceToken = u.devicetoken,
                               buyItem = Enum.GetName(typeof(ItemType), p.type ?? 0),
                               ItemName = this.GetItemName(p.type, p.itemid),
                               buyway = Enum.GetName(typeof(PayWay), p.payway ?? 0),
                               buyMoney = p.money.ToString(),
                               devicetype = Enum.GetName(typeof(DeviceType), u.devicetype ?? 0),
                               note = DecodeReceipt(p.note, p.payway)
                           });

            return resList;
        }

        private IQueryable<PaymentObject> GetPaymentQueryableStr(DateTime start, DateTime end)
        {
            var resList = (from p in dataContext.userpayments
                           from u in dataContext.users
                           where p.userid == u.userid
                           && p.time.Value >= start
                           && p.time.Value <= end
                           orderby p.time descending
                           select new PaymentObject
                           {
                               userid = u.userid,
                               mail = u.mail,
                               nickname = Server.HtmlEncode(u.nickname),
                               time = p.time,
                               deviceToken = u.devicetoken,
                               buyItem = Enum.GetName(typeof(ItemType), p.type ?? 0),
                               ItemName = this.GetItemName(p.type, p.itemid),
                               buyway = Enum.GetName(typeof(PayWay), p.payway ?? 0),
                               buyMoney = p.money.ToString(),
                               devicetype = Enum.GetName(typeof(DeviceType), u.devicetype ?? 0),
                               note = DecodeReceipt(p.note, p.payway)
                           });

            return resList;
        }

        /// <summary>
        /// 康熙玉玺包月  购买次数  购买人数
        /// 康熙玉玺包年  购买次数  购买人数
        /// 皇室血统包月  购买次数  购买人数
        /// 皇室血统包年  购买次数  购买人数            
        /// </summary>
        /// <returns></returns>
        private List<BuyItemObject> GetBuyItemList() {
            int[] jader_arr = new[] {1,4};
            int[] lineage_arr = new[] {1,4};
            var query = GetBuyItemInfo();
            
            List<BuyItemObject> myList = new List<BuyItemObject>();

            foreach(var item in jader_arr){       
                var times = query.Count(m => m.type == (int)ItemType.Jade && m.itemid == item);
                var count = GetBuyItemPeopleBuyCount((int)ItemType.Jade, item);

                BuyItemObject bio = new BuyItemObject();
                bio.name = this.GetItemName((int)ItemType.Jade, item);
                bio.buyTimes = times.ToString();
                bio.peopleCount = count.ToString();
                myList.Add(bio);
            }

            foreach(var item in lineage_arr){
                var times = query.Count(m => m.type == (int)ItemType.Lineage && m.itemid == item);
                var count = GetBuyItemPeopleBuyCount((int)ItemType.Lineage, item);

                BuyItemObject bio = new BuyItemObject();
                bio.name = this.GetItemName((int)ItemType.Jade, item);
                bio.buyTimes = times.ToString();
                bio.peopleCount = count.ToString();
                myList.Add(bio);
            }

            return myList;
        }

        /// <summary>
        /// 付费道具购买详情下载 
        /// </summary>
        /// <returns></returns>
        private string GenerateBuyItemsInfoExcel()
        {
            var resList = (from p in dataContext.userpayments
                           from u in dataContext.users
                           where p.userid == u.userid
                           && (p.type == (int)ItemType.Lineage
                                || p.type == (int)ItemType.Jade)
                           orderby p.time descending, p.userid ascending, p.type descending
                           select new BuyItemObject { 
                                userid = p.userid,
                                dateTime = p.time,
                                name = this.GetItemName(p.type, p.itemid)
                           });

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>user-id</th><th>名称</th><th>购买时间</th>");
            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                sb.Append("<td>" + item.name + "</td>");
                sb.Append("<td>" + item.dateTime + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private IQueryable<userpayment> GetBuyItemInfo()
        {
            var lineage_type = (int)ItemType.Lineage;
            var jade_type = (int)ItemType.Jade;
            var resList = (from p in dataContext.userpayments
                           from u in dataContext.users                           
                           where p.userid == u.userid
                           && ( p.type == lineage_type 
                                || p.type == jade_type)
                           orderby p.time descending
                           select p);

            return resList;
        }

        private int GetBuyItemPeopleBuyCount(int type, long itemid) {
            var res = (from p in dataContext.userpayments
                           from u in dataContext.users
                           where p.userid == u.userid
                           && p.type == type
                           && p.itemid == itemid                           
                           select p.userid).Distinct().Count();

            return res;
        }

        private DateTime ParseStrToDateTime(string str)
        {
            DateTime start = DateHelper.GetNow();
            if (!DateTime.TryParse(str, out start))
            {
                start = DateTime.Today;
            }

            return start;
        }

        private string DecodeReceipt(string s, int? type)
        {
            string result = Server.HtmlEncode(s);
            if (type != (int)PayWay.Alipay)
            {
                result = Server.HtmlEncode(ReceiptHelper.DecodeReceipt(s));
            }

            return result;
        }

        private string GetItemName(int? itemtype, long? itemid)
        {
            string name = string.Empty;
            int int_itemid = (int)(itemid ?? -1); ;
            long long_itemid = (long)(itemid ?? 0); ;
            switch (itemtype)
            {
                case (int)ItemType.Chip: // 筹码
                    name = BuyManagerHelper.GetNameByTypeId((long)long_itemid, BuyChipsDic.myDic);
                    break;
                case (int)ItemType.Room:   // 房间
                    name = BuyManagerHelper.GetNameByTypeId(int_itemid, BuyScenceDic.myDic);
                    break;
                case (int)ItemType.Avator:  // 头像
                    name = BuyManagerHelper.GetNameByTypeId(int_itemid, BuyAvatorDic.myDic);
                    break;
                case (int)ItemType.Jade:    // 玉
                    name = BuyManagerHelper.GetNameByTypeId(int_itemid, BuyJadeDic.myDic);
                    break;
                case (int)ItemType.Lineage:  // 血统
                    name = BuyManagerHelper.GetNameByTypeId(int_itemid, BuyLineageDic.myDic);
                    break;
                default:
                    break;
            }

            return name;
        }

        #endregion  
    }
}
