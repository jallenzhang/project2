using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Text;
using System.Collections;

using WebNotifications.Models;
using DataPersist;
using WebNotifications.Permission;
using WebNotifications.Models.Custom;
using Webdiyer.WebControls.Mvc;
using WebNotifications.Helper;
using OpenFlashChart;

namespace WebNotifications.Controllers
{
    [Description("用户数据查询(底下权限可以继承当前权限)")]
    public class UserSearchController : BaseController
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();
        private static string CHART_START_SESSION_KEY = "CHART_START_SESSION_KEY";
        private static string CHART_END_SESSION_KEY = "CHART_END_SESSION_KEY";

        #region ActionResult Method

        [Description("用户数据查询--页面")]
        public ActionResult Index()
        {
            return View();
        }

        [Description("用户数据查询--查询动作")]
        public ActionResult UserSearchAction(string tbnickname, string tbmail)
        {
            var result = (from m in dataContext.users
                          where m.nickname == tbnickname
                              && m.mail == tbmail
                          select m).FirstOrDefault();

            if (result != null)
            {
                ViewData["userid"] = result.userid;
                ViewData["usertype"] = Enum.GetName(typeof(UserType), result.usertype ?? 7);
                ViewData["userexp"] = result.exp;
                ViewData["userchip"] = result.chips;
                ViewData["totalgames"] = result.totalgame;
                ViewData["wingames"] = result.wins;
                ViewData["besthand"] = result.besthandtype;
                ViewData["ownmoney"] = result.money;
                ViewData["devicestype"] = Enum.GetName(typeof(DeviceType), result.devicetype ?? 0);
            }

            return View("Index");
        }

        [Description("用户数据查询--在线人数查询")]
        public ActionResult OnlineNum()
        {
            ViewData["tbStartTime"] = DateHelper.GetNow().ToShortDateString();            
            ViewData["tbEndTime"] = DateHelper.GetNow().ToShortDateString();

            SetSessionValue(ViewData["tbStartTime"].ToString(), ViewData["tbEndTime"].ToString());

            return View();
        }

        [Description("用户数据查询--在线实时人数查询")]
        [HttpPost]
        public ActionResult OnlineNum(string tbStartTime, string tbEndTime)
        {
            DateTime start = ParseStrToDateTime(tbStartTime);
            DateTime end = ParseStrToDateTime(tbEndTime);
            ViewData["tbStartTime"] = start.ToShortDateString();
            ViewData["tbEndTime"] = end.ToShortDateString();
            var resList = GetOnlineUserStr(start, end);
            var myList = GetCustomOnlineUser(resList);

            SetSessionValue(start.ToShortDateString(), end.ToShortDateString());

            return View(myList);
        }

        [Description("用户数据查询--在线实时人数下载")]
        public ActionResult OnlineNumDownload()
        {
            string conent = GenerateExcel();

            ExportToFile.DownLoadAsExcel("在线实时人数 ", conent);

            return RedirectToAction("OnlineNum");
        }

        #endregion

        #region Private Method

        /// <summary>
        /// 在线实时人数下载 
        /// </summary>
        /// <returns></returns>
        private string GenerateExcel()
        {
            var resList = (from p in dataContext.ReportUserOnlines
                           orderby p.reportDate descending
                           select p);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>当前时间</th><th>在线人数</th></tr>");

            foreach (var item in resList)
            {
                var myList = GetCustomOnlineUser(item);
                foreach (var myOnline in myList)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + myOnline.CurrentDate.ToLongDateString() + "</td>");
                    sb.Append("<td>" + myOnline.UserNum + "</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private ReportUserOnline GetOnlineUserStr(DateTime start, DateTime end)
        {
            var resList = (from p in dataContext.ReportUserOnlines
                           where p.reportDate >= start
                           && p.reportDate <= end
                           orderby p.reportDate descending
                           select p).FirstOrDefault();

            return resList;
        }

        private List<OnlineCaculateObject> GetCustomOnlineUser(ReportUserOnline reportUserOnline)
        {
            if (reportUserOnline != null)
            {
                var mylist = new List<OnlineCaculateObject>();

                string[] user_numbers = reportUserOnline.reportContent.Split(new char[] { '|' });
                DateTime mydate = reportUserOnline.reportDate;
                for (int i = 0; i < user_numbers.Length; i++)
                {
                    if (!string.IsNullOrEmpty(user_numbers[i].Trim()))
                    {
                        var ocob = new OnlineCaculateObject();
                        if (DateTime.Parse("2012/9/18").CompareTo(mydate) >= 0)
                            ocob.CurrentDate = mydate.AddMinutes(i * 30);
                        else
                            ocob.CurrentDate = mydate.AddMinutes(i * 10);
                        ocob.UserNum = user_numbers[i];
                        mylist.Add(ocob);
                    }
                }
                return mylist;
            }

            return null;
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

        private void SetSessionValue(string tbStartTime, string tbEndTime)
        {
            Session[CHART_START_SESSION_KEY] = tbStartTime;
            Session[CHART_END_SESSION_KEY] = tbEndTime;
        }

        #endregion        

        #region For Chart

        [Description("用户数据查询--在线实时人数图表")]
        public string GetOnlinePeopleChart()
        {
            DateTime start = DateHelper.ToMyDateTime(Session[CHART_START_SESSION_KEY] as string);
            DateTime end = DateHelper.ToMyDateTime(Session[CHART_END_SESSION_KEY] as string);             
            var resList = GetOnlineUserStr(start, end);
            var myList = GetCustomOnlineUser(resList);

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();

            chart.X_Legend = new Legend("time");
            chart.Y_Legend = new Legend("number");
            chart.Title = new Title("在线实时人数");
            chart.Tooltip = new ToolTip("全局提示：#val#");
            chart.Tooltip.Shadow = true;
            chart.Tooltip.Colour = "#e43456";
            chart.Tooltip.MouseStyle = ToolTipStyle.CLOSEST;
            
            ArrayList data1 = new ArrayList();
            double max_limit = 0;
            string tipFormat = "人数:{0}; 时间:{1}";
            if (myList != null)
            {                
                for (int i = 0; i < myList.Count; i++)
                {
                    double tmp_value = double.Parse(myList[i].UserNum);
                    var myLineValue = new LineDotValue(tmp_value, "#9933CC");
                    myLineValue.DotSize = 2;
                    myLineValue.DotType = DotType.HOLLOW_DOT;
                    myLineValue.Tip = string.Format(tipFormat, myList[i].UserNum, myList[i].CurrentDate.ToShortTimeString());
                    data1.Add(myLineValue);
                    if (max_limit < tmp_value) max_limit = tmp_value;
                }
            }

            OpenFlashChart.LineHollow line1 = new LineHollow();
            line1.Values = data1;
            line1.Width = 2;
            line1.Colour = "#3D5C56";
            line1.Tooltip = "提示：#val#";                        
            chart.AddElement(line1);            

            chart.X_Axis.SetRange(0, 150);

            chart.X_Axis.Steps = 6;
            for (int i = 0; i < 150; i++)
            {
                if (i % 6 == 0)
                {
                    chart.X_Axis.Labels.Add(new AxisLabel((i/6).ToString()));
                }
                else {
                    chart.X_Axis.Labels.Add(new AxisLabel(string.Empty));
                }
            }

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }



        #endregion
    }
}
