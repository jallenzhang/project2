using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.ComponentModel;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Models.Custom;
using DataPersist;
using WebNotifications.Permission;
using OpenFlashChart;

namespace WebNotifications.Controllers
{
    //[Authorize]
    [Description("运营数据分析(底下权限可以继承当前权限)")]
    public class OperationDataController : BaseController
    {        
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();

        //[Authorize(Roles = "Administrator, OperationRole")]
        [Description("运营数据分析--列表页面")]
        public ActionResult Index()
        {
            return View();
        }

        #region View

        [Description("运营数据分析--用户等级分布页面")]
        /// <summary>
        /// 用户等级分布
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult V5UserLevelDist()
        {
            return View();
        }

        [Description("运营数据分析--活动名称页面")]
        /// <summary>
        /// 活动名称
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult V6ActiveName()
        {
            return View();
        }

        [Description("运营数据分析--充值用户数量页面")]
        /// <summary>
        /// 充值用户数量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V7RechargeNum()
        {
            DateTime current = DateHelper.GetNow();
            ViewData["tbStartDate"] = current.AddDays(-1).ToShortDateString();
            ViewData["tbEndDate"] = current.ToShortDateString();
            return View();
        }

        [Description("运营数据分析--单个用户充值金额页面")]
        /// <summary>
        /// 单个用户充值金额
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V8SingleRechargeValue()
        {
            return View();
        }

        [Description("运营数据分析--付费率页面")]
        /// <summary>
        /// 付费率
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V9PayPercent()
        {
            ViewData["tbName"] = DateHelper.GetNow().ToShortDateString();
            return View();
        }

        [Description("运营数据分析--ARPU值页面")]
        /// <summary>
        /// ARPU值
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V10ARPUValue()
        {
            ViewData["tbName"] = DateHelper.GetNow().ToShortDateString();
            return View();
        }

        [Description("运营数据分析--付费总量页面")]
        /// <summary>
        /// 付费总量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V11PayValueTotal()
        {
            return View();
        }

        [Description("运营数据分析--单个玩家最高充值数页面")]
        /// <summary>
        /// 单个玩家最高充值数
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V12ActiveName()
        {
            return View();
        }

        #endregion

        #region Button Action

        [Description("运营数据分析--用户等级分布查询")]
        /// <summary>
        /// 用户等级分布
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult V5UserLevelDistAction()
        {
            ViewData.Model = GenerateV5UserLevelDistData();

            return View("V5UserLevelDist");
        }

        [Description("运营数据分析--用户等级分布下载")]
        /// <summary>
        /// 用户等级分布
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult DownloadV5UserLevelDistAction()
        {
            string conent = GenerateV5UserLevelDistExcel();

            ExportToFile.DownLoadAsExcel("用户等级分布", conent);

            return RedirectToAction("V5UserLevelDist");
        }

        [Description("运营数据分析--活动名称查询")]
        /// <summary>
        /// 活动名称
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult V6ActiveNameAction()
        {
            // can not get now

            return View("V6ActiveName");
        }

        [Description("运营数据分析--活动名称下载")]
        /// <summary>
        /// 活动名称
        /// </summary>
        //[Authorize(Roles = "Administrator, OperationRole")]
        public ActionResult DownloadV6ActiveNameAction()
        {
            //string conent = GenerateV1RegistChipsExcel();

            //ExportToFile.DownLoadAsExcel("活动名称", conent);

            return RedirectToAction("V6ActiveName");
        }

        [Description("运营数据分析--充值用户数量查询")]
        /// <summary>
        /// 充值用户数量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V7RechargeNumAction(string tbStartDate, string tbEndDate)
        {
            ViewData.Model = this.GenerateV7RechargeNumData(tbStartDate, tbEndDate);

            return View("V7RechargeNum");
        }

        [Description("运营数据分析--充值用户数量下载")]
        /// <summary>
        /// 充值用户数量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV7RechargeNumAction(string tbStartDate, string tbEndDate)
        {
            string conent = GenerateV7RechargeNumExcel(tbStartDate, tbEndDate);

            ExportToFile.DownLoadAsExcel("充值用户数量 ", conent);

            return RedirectToAction("V7RechargeNum");
        }

        [Description("运营数据分析--单个用户充值金额查询")]
        /// <summary>
        /// 单个用户充值金额
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V8SingleRechargeValueAction(string tbName)
        {
            ViewData.Model = GenerateV8SingleRechargeValueData(tbName);                           
            return View("V8SingleRechargeValue");
        }

        [Description("运营数据分析--单个用户充值金额下载")]
        /// <summary>
        /// 单个用户充值金额
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV8SingleRechargeValueAction(string tbName)
        {
            string conent = GenerateV8SingleRechargeValueExcel(tbName);

            ExportToFile.DownLoadAsExcel("单个用户充值金额", conent);

            return RedirectToAction("V8SingleRechargeValue");
        }

        [Description("运营数据分析--付费率查询")]
        /// <summary>
        /// 付费率
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V9PayPercentAction(string tbName)
        {
            var res = this.GenerateV9PayPercentData(tbName);
            
            ViewData["tbLoginNum"] = res.Item1;
            ViewData["tbPayNum"] = res.Item2;
            ViewData["tbPayPercent"] = res.Item3;

            return View("V9PayPercent");
        }

        [Description("运营数据分析--付费率下载")]
        /// <summary>
        /// 付费率
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV9PayPercentAction(string tbName)
        {
            string conent = GenerateV9PayPercentExcel(tbName);

            ExportToFile.DownLoadAsExcel("付费率", conent);

            return RedirectToAction("V9PayPercent");
        }

        [Description("运营数据分析--ARPU值查询")]
        /// <summary>
        /// ARPU值
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V10ARPUValueAction(string tbName)
        {
            var res = this.GenerateV10ARPUValueData(tbName);

            ViewData["tbPayNumF"] = res.Item1 ;
            ViewData["tbPayNumI"] = res.Item2;
            ViewData["tbPayPercent"] = res.Item3;
            ViewData["tbPayPercentF"] = res.Item4;
            ViewData["tbPayPercentI"] = res.Item5;

            return View("V10ARPUValue");
        }

        [Description("运营数据分析--ARPU值下载")]
        /// <summary>
        /// ARPU值
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV10ARPUValueAction(string tbName)
        {
            string conent = GenerateV10ARPUValueExcel(tbName);

            ExportToFile.DownLoadAsExcel("ARPU", conent);

            return RedirectToAction("V10ARPUValue");
        }

        [Description("运营数据分析--付费总量查询")]
        /// <summary>
        /// 付费总量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V11PayValueTotalAction()
        {
            ViewData["tbTotal"] = GenerateV11PayValueTotalData();

            return View("V11PayValueTotal");
        }

        [Description("运营数据分析--付费总量下载")]
        /// <summary>
        /// 付费总量
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV11PayValueTotalAction()
        {
            string conent = GenerateV11PayValueTotalExcel();

            ExportToFile.DownLoadAsExcel("付费总量", conent);

            return RedirectToAction("V11PayValueTotal");
        }

        [Description("运营数据分析--单个玩家最高充值数查询")]
        /// <summary>
        /// 单个玩家最高充值数
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult V12ActiveNameAction()
        {
            ViewData["tbTotal"] = this.GenerateV12ActiveNameData().Name; 
                       
            return View("V12ActiveName");
        }

        [Description("运营数据分析--单个玩家最高充值数下载")]
        /// <summary>
        /// 单个玩家最高充值数
        /// </summary>
        //[Authorize(Roles = "Administrator")]
        public ActionResult DownloadV12ActiveNameAction()
        {
            string conent = GenerateV12ActiveNameExcel();

            ExportToFile.DownLoadAsExcel("单个玩家最高充值数", conent);

            return RedirectToAction("V12ActiveName");
        }

        #endregion

        #region Generate Excel

        private string GenerateV5UserLevelDistExcel()
        {
            var resList = GenerateV5UserLevelDistData();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>等级</th><th>人数</th></tr>");
            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.Name + "</td>");
                sb.Append("<td>" + item.Count + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV7RechargeNumExcel(string sdate, string edate)
        {
            var resList = GenerateV7RechargeNumData(sdate, edate);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>时间</th><th>使用人数</th></tr>");
            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.Name + "</td>");
                sb.Append("<td>" + item.Count2 + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV8SingleRechargeValueExcel(string name)
        {
            var resList = GenerateV8SingleRechargeValueData(name);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>充值时间</th><th>充值金钱</th></tr>");
            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.time.ToString() + "</td>");
                sb.Append("<td>" + item.money + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV9PayPercentExcel(string datetime)
        {
            var res = GenerateV9PayPercentData(datetime);
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>日期</th><th>导入人数</th><th>付费人数</th><th>付费率</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + datetime + "</td>");
            sb.Append("<td>" + res.Item1 + "</td>");
            sb.Append("<td>" + res.Item2 + "</td>");
            sb.Append("<td>" + res.Item3 + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV10ARPUValueExcel(string datetime)
        {
            var res = GenerateV10ARPUValueData(datetime);
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>日期</th><th>导入人数</th><th>付费人数</th><th>付费额度</th><th>海外和港台ARPU值</th><th>大陆ARPU值</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + datetime + "</td>");
            sb.Append("<td>" + res.Item1 + "</td>");
            sb.Append("<td>" + res.Item2 + "</td>");
            sb.Append("<td>" + res.Item3 + "</td>");
            sb.Append("<td>" + res.Item4 + "</td>");
            sb.Append("<td>" + res.Item5 + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 付费总量
        /// </summary>
        /// <returns></returns>
        private string GenerateV11PayValueTotalExcel()
        {
            var res = GenerateV11PayValueTotalData();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>付费总量(RMB)</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + res + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

         /// <summary>
        /// 单个玩家最高充值数
        /// </summary>
        /// <returns></returns>
        private string GenerateV12ActiveNameExcel()
        {
            var res = this.GenerateV12ActiveNameData();
            var res2 = (from u in dataContext.users
                        where u.userid == res.ID
                        select u).FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>充值数</th><th>UserId</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + res.Name + "</td>");
            sb.Append("<td>" + res2.userid + "</td>");
            //sb.Append("<td>" + res2.nickname + "</td>");
            //sb.Append("<td>" + res2.mail + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }
        
        #endregion

        #region Generate Data

        /// <summary>
        /// 用户等级分布
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CustomObject> GenerateV5UserLevelDistData()
        {
            var levelList = (from m in dataContext.users
                             where m.exp != null
                             select new
                             {
                                 mLevel = LevelExpHelper.GetJustLevel((double)m.exp)
                             }).ToList();
            var resList = (from m in levelList
                           group m by m.mLevel into mgroup
                           orderby mgroup.Key
                           select new CustomObject
                           {

                               Name = mgroup.Key.ToString(),
                               Count = mgroup.Count()
                           });

            return resList;
        }

        /// <summary>
        /// 充值用户数量
        /// </summary>
        /// <param name="stardDate"></param>
        /// <param name="endData"></param>
        /// <returns></returns>
        private List<CustomObject> GenerateV7RechargeNumData(string stardDate, string endData)
        {
            int type = (int)ItemType.Chip; // recharge
            DateTime sdate = DateHelper.GetNow();
            DateTime edate = DateHelper.GetNow();
            if(!DateTime.TryParse(stardDate, out sdate))
                sdate = DateHelper.GetNow();
            if(!DateTime.TryParse(endData, out edate))
                edate = DateHelper.GetNow();
            int d_day = edate.Subtract(sdate).Days;
            var resList = new List<CustomObject>();
            for (int i = 0; i <= d_day; i++)
            {
                DateTime date = sdate.AddDays(i);
                IEnumerable<CustomObject> query = dataContext.ExecuteQuery<CustomObject>(
                        @"
                            SELECT count(id) as Count, 
                                    COUNT(distinct userid) as Count2
                            FROM [userpayment]
                            where type = " + type + @"
                            and DAY(time) = " + date.Day + @"
                            and Month(time) = " + date.Month + @"
                            and Year(time) = " + date.Year + @"
                        "
                    );
                var res = query.FirstOrDefault();
                res.Name = date.ToShortDateString();
                resList.Add(res);
            }

            return resList;
        }

        /// <summary>
        /// 单个用户充值金额
        /// </summary>
        /// <param name="name">user mail or nickname</param>
        /// <returns></returns>
        private IEnumerable<userpayment> GenerateV8SingleRechargeValueData(string name)
        {
            int type = (int)ItemType.Chip;
            name = name.Trim();
            var userList = from m in dataContext.userpayments
                           from u in dataContext.users
                           where m.userid == u.userid
                           && m.type == type
                           && (u.nickname == name || u.mail == name)
                           select m;

            return userList;
        }

        /// <summary>
        /// 付费率
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private Tuple<int, int, double> GenerateV9PayPercentData(string dateTime)
        {
            int type = (int)BankActionType.Register;
            DateTime current = DateHelper.GetNow();
            if (!DateTime.TryParse(dateTime, out current))
                current = DateHelper.GetNow();
            // 至查询日的导入(至少有一次登陆)人数
            var loginNum = (from u in dataContext.users
                            from b in dataContext.banks
                            where u.userid == b.userid
                            && b.optype == type
                            && b.createtime <= current
                            select u.userid).Distinct().Count();
            // 至查询日期为止的付费人数
            var payNum = (from u in dataContext.userpayments
                          where u.time <= current
                          select u.userid).Distinct().Count();
            // 至查询日期的付费率
            var parPercent = 0.0;
            if (loginNum != 0) parPercent = payNum / (loginNum * 1.00);

            return new Tuple<int, int, double>(loginNum, payNum, parPercent);
        }

        /// <summary>
        /// ARPU值
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private Tuple<int, int, decimal, double, double> GenerateV10ARPUValueData(string dateTime)
        {
            int type = (int)BankActionType.Register;
            DateTime current = DateHelper.GetNow();
            if (!DateTime.TryParse(dateTime.Trim(), out current))
                current = DateHelper.GetNow();
            // 至查询日的导入(至少有一次登陆)人数
            var loginNum = (from u in dataContext.users
                            from b in dataContext.banks
                            where u.userid == b.userid
                            && b.optype == type
                            && b.createtime <= current
                            select u.userid).Distinct().Count();
            // 至查询日的付费人数
            var payValueNum = (from u in dataContext.userpayments
                               where u.time <= current
                               select u.userid).Distinct().Count();
            // 至查询日期为止的付费总量
            var payNum = (from u in dataContext.userpayments
                          where u.time <= current
                          select u.money).Sum();
            payNum = CodeHelper.GetNumberOrZero(payNum);
            // 海外和港台计算公式
            var parPercentF = 0.0;
            if (loginNum != 0) parPercentF = (double)payNum / (loginNum * 1.00);
            // 大陆计算公式
            var parPercentI = 0.0;
            if (payValueNum != 0) parPercentI = (double)payNum / (payValueNum * 1.00);

            return new Tuple<int, int, decimal, double, double>(loginNum, payValueNum, (decimal)payNum, parPercentF, parPercentI);
        }

        /// <summary>
        /// 付费总量
        /// </summary>
        private decimal GenerateV11PayValueTotalData()
        {
            var res = (from u in dataContext.userpayments
                       select u.money).Sum();

            return  CodeHelper.GetNumberOrZero(res);
        }

         /// <summary>
        /// 单个玩家最高充值数
        /// </summary>
        private CustomObject GenerateV12ActiveNameData()
        {
            var res = (from m in dataContext.userpayments
                       group m by m.userid into g
                       select new CustomObject
                       {
                           ID = g.Key,
                           Name = g.Sum(p => p.money).ToString()
                       }).FirstOrDefault();

            return res;
        }

        #endregion

        #region For Chart

        [Description("运营数据分析--用户等级分布查询")]
        public string GetV5UserLevelDistChart()
        {
            var myList = GenerateV5UserLevelDistData();

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();
            chart.Title = new Title("用户等级分布");
            chart.X_Legend = new Legend("等级");

            Bar bar = new OpenFlashChart.Bar();
            bar.Colour = "#344565";
            //bar.Alpha = 0.8;
            bar.Text = "用户等级";
            bar.FontSize = 10;

            List<object> values = new List<object>();
            XAxis xaxis = new XAxis();
            int max_limit = 0;
            foreach (var item in myList) {
                values.Add(item.Count);
                xaxis.Labels.Add(new AxisLabel(item.Name));
                if(item.Count > max_limit) max_limit = item.Count;
            }
            bar.Values = values;

            chart.X_Axis = xaxis;
            chart.AddElement(bar);
            bar.Tooltip = "人数：#val#<br>等级: #x_label#";
            string s = chart.ToPrettyString();

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }

        #endregion
    }
}
