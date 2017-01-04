using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.ComponentModel;
using System.Collections;

using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Actions;
using WebNotifications.Models.Custom;
using WebNotifications.Permission;
using OpenFlashChart;
using DataPersist;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles = "Administrator, MarketRole")]
    [Description("市场数据分析(底下权限可以继承当前权限)")]
    public class MarketDataController : BaseController
    {        
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();
        private static string CHART_START_SESSION_KEY = "CHART_START_SESSION_KEY_MARKET";
        private static string CHART_END_SESSION_KEY = "CHART_END_SESSION_KEY_MARKET";

        #region View

        [Description("市场数据分析--列表页面")]
        public ActionResult Index()
        {
            return View();
        }

        [Description("市场数据分析--注册人数页面")]
        /// <summary>
        /// 累计注册人数
        /// </summary>
        public ActionResult V1RegisterNum()
        {
            DateTime current = DateHelper.GetNow();
            ViewData["tbStartDate"] = current.AddDays(-1).ToShortDateString();
            ViewData["tbEndDate"] = current.ToShortDateString();
            return View();
        }

        #endregion

        #region Button Action

        [Description("市场数据分析--注册人数查询操作")]
        /// <summary>
        /// 累计注册人数
        /// </summary>
        public ActionResult V1RegisterNumAction(string tbStartDate, string tbEndDate)
        {
            ViewData.Model = GenerateV1RegisterNumData(tbStartDate, tbEndDate);
            SetSessionValue(tbStartDate, tbEndDate);

            return View("V1RegisterNum");
        }

        [Description("市场数据分析--注册人数数据下载")]
        /// <summary>
        /// 累计注册人数
        /// </summary>
        public ActionResult DownloadV1RegisterNumAction(string tbStartDate, string tbEndDate)
        {
            string conent = GenerateV1RegisterNumExcel(tbStartDate, tbEndDate);

            ExportToFile.DownLoadAsExcel("注册人数", conent);

            return RedirectToAction("V1RegisterNum");
        }

        #endregion

        #region Generate Excel Report

        /// <summary>
        /// 累计注册人数 
        /// </summary>
        /// <returns></returns>
        private string GenerateV1RegisterNumExcel(string stardDate, string endData)
        {
            var resList = GenerateV1RegisterNumData(stardDate, endData);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>日期</th><th>累计注册人数</th><th>累计游客人数</th></tr>");
            foreach (var item in resList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.Name + "</td>");
                sb.Append("<td>" + item.Count + "</td>");
                sb.Append("<td>" + item.Count2 + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        #endregion

        #region Generate Data

        /// <summary>
        /// 累计注册人数
        /// </summary>
        /// <param name="stardDate"></param>
        /// <param name="endData"></param>
        /// <returns></returns>
        private List<CustomObject> GenerateV1RegisterNumData(string stardDate, string endData)
        {
            int type_R = (int)BankActionType.Register; // register
            int type_G = (int)UserType.Guest;
            DateTime sdate = DateHelper.GetNow();
            DateTime edate = DateHelper.GetNow();
            if (!DateTime.TryParse(stardDate.Trim(), out sdate))
                sdate = DateHelper.GetNow();
            if (!DateTime.TryParse(endData.Trim(), out edate))
                edate = DateHelper.GetNow();
            int d_day = edate.Subtract(sdate).Days;
            var resList = new List<CustomObject>();
            for (int i = 0; i <= d_day; i++)
            {
                DateTime date = sdate.AddDays(i);           
                // register
                var q_res = (from u in dataContext.users
                             from b in dataContext.banks
                             where u.usertype != type_G
                             && b.optype == type_R
                             && u.userid == b.userid
                             && b.createtime.Value < date.AddDays(1)
                             select u.userid).Distinct().Count();
                // guest
                var g_res = (from u in dataContext.users
                             from b in dataContext.banks
                             where u.usertype == type_G
                             && b.optype == type_R
                             && u.userid == b.userid
                             && b.createtime.Value < date.AddDays(1)
                             select u.userid).Distinct().Count();
                var res = new CustomObject
                {
                    Count = q_res,
                    Count2 = g_res
                };
                res.Name = date.ToShortDateString();
                resList.Add(res);
            }

            return resList;
        }

        #endregion

        #region For Chart

        [Description("市场数据分析--注册人数图表")]
        public string GetRegisterNumChart()
        {
            string start = Session[CHART_START_SESSION_KEY] as string;
            string end = Session[CHART_END_SESSION_KEY] as string;
            var myList = GenerateV1RegisterNumData(start, end);

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();
                        
            chart.Bgcolor = "#303030";
            chart.Title = new Title("累计注册人数");
            chart.Tooltip = new ToolTip("全局提示：#val#");
            chart.Tooltip.Shadow = true;
            chart.Tooltip.Colour = "#e43456";
            chart.Tooltip.MouseStyle = ToolTipStyle.CLOSEST;
            chart.X_Legend = new Legend("time");
            string myGlobalStyle = "color:#ffffff;font-size:20px";
            chart.Title.Style = myGlobalStyle;
            chart.X_Legend.Style = myGlobalStyle;
            chart.X_Axis.Labels.Color = "#ffffff";
            chart.Y_Axis.Labels.Color = "#ffffff";

            ArrayList data1 = new ArrayList();
            ArrayList data2 = new ArrayList();
            double max_limit = 0;
            string tipFormat1 = "时间:{0} 累计注册人数:{1}";
            string tipFormat2 = "时间:{0} 累计游客:{1}";
            if (myList != null)
            {
                for (int i = 0; i < myList.Count; i++)
                {
                    double tmp_value = double.Parse(myList[i].Count.ToString());
                    double tmp_value2 = double.Parse(myList[i].Count2.ToString());
                    var myLineValue1 = new LineDotValue(tmp_value, "#FFB900");
                    var myLineValue2 = new LineDotValue(tmp_value2, "#28A0DC");

                    myLineValue1.DotSize = 2;
                    myLineValue1.DotType = DotType.HOLLOW_DOT;
                    myLineValue1.Tip = string.Format(tipFormat1, myList[i].Name, myList[i].Count);
                    data1.Add(myLineValue1);

                    myLineValue2.DotSize = 2;
                    myLineValue2.DotType = DotType.HOLLOW_DOT;
                    myLineValue2.Tip = string.Format(tipFormat2, myList[i].Name, myList[i].Count2);
                    data2.Add(myLineValue2);
                    if (max_limit < tmp_value) max_limit = tmp_value;
                    if (max_limit < tmp_value2) max_limit = tmp_value2;
                }
            }

            OpenFlashChart.LineHollow line1 = new LineHollow();
            line1.Values = data1;
            line1.Width = 2;
            line1.Colour = "#FFB900";
            line1.Tooltip = "提示：#val#";

            OpenFlashChart.LineHollow line2 = new LineHollow();
            line2.Values = data2;
            line2.Width = 2;
            line2.Colour = "#28A0DC";
            line2.Tooltip = "提示：#val#";

            chart.AddElement(line1);
            chart.AddElement(line2);

            chart.X_Axis.SetRange(0, myList.Count);
            int myStep = 8;
            chart.X_Axis.Steps = (int)myList.Count / myStep;
            if (myList.Count % myStep != 0)
            {
                chart.X_Axis.Steps++;
            }            
            for (int i = 0; i < myList.Count; i++)
            {
                if (i % chart.X_Axis.Steps == 0)
                {
                    chart.X_Axis.Labels.Add(new AxisLabel(myList[i].Name));
                }
                else {
                    chart.X_Axis.Labels.Add(new AxisLabel(string.Empty));
                }
            }

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }

        #endregion

        #region Private Method

        private void SetSessionValue(string tbStartTime, string tbEndTime)
        {
            Session[CHART_START_SESSION_KEY] = tbStartTime;
            Session[CHART_END_SESSION_KEY] = tbEndTime;
        }

        #endregion
    }
}
