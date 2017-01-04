using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.Text;
using System.Web.Security;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;
using WebNotifications.Models.ChannelUser;
using WebNotifications.Helper;
using WebNotifications.Models;

namespace WebNotifications.Controllers
{
    [Description("渠道用户查询后台(底下权限可以继承当前权限)")]
    public class ChannelUserController : BaseController
    {
        DataJackModelDataContext _Assctx = new DataJackModelDataContext();
        DataLilyModelDataContext _Lilyctx = new DataLilyModelDataContext();
        private const string SESSION_KEY = "ChannelUserController_Page";
        private const int EXPORT_TIME = 365;

        [Description("渠道用户查询后台--显示界面")]
        public ActionResult Index(int? page)
        {
            if (page == null)
                page = 1;
            else            
                page = page + (int)Session[SESSION_KEY];
            Session[SESSION_KEY] = page;
            if (ViewData["tbFilterTime"] == null)
                ViewData["tbFilterTime"] = DateHelper.GetNow().ToShortDateString();
            var res = GetDataReportList(GetChannelId(), (page ?? 1), DateHelper.GetNow());

            return View(res);
        }

        [Description("渠道用户查询后台--日期查询")]
        public ActionResult DataSearchByDay(string tbFilterTime)
        {
            var list = GetDataReportList(GetChannelId(), DateHelper.ToMyDateTime(tbFilterTime));
            return View("Index", list);
        }

        [Description("渠道用户查询后台--数据导出")]
        // 导出最近1年的
        public ActionResult DataReportExportAction()
        { 
            DateTime upTime = DateHelper.GetNow();
            DateTime downTime = upTime.AddDays(-EXPORT_TIME);
            var list = GetFilterDataReportList(GetChannelId(), upTime, downTime);
            string conent = GenerateDataExcel(list);

            ExportToFile.DownLoadAsExcel("注册人数", conent);

            return RedirectToAction("Index"); 
        }

        #region Private Method

        private List<DataReport> GetDataReportList(string myChannelId, DateTime myFilterTime)
        {
            DateTime upTime = myFilterTime;
            DateTime dowmTime = myFilterTime.AddDays(-1);

            return GetFilterDataReportList(myChannelId, upTime, dowmTime);
        }

        private List<DataReport> GetDataReportList(string myChannelId, int page, DateTime myFilterTime)
        {
            DateTime upTime = myFilterTime.AddDays(-SiteConfigHelper.DefaultPageSize * (page - 1));
            DateTime dowmTime = myFilterTime.AddDays(-SiteConfigHelper.DefaultPageSize * page);

            return GetFilterDataReportList(myChannelId, upTime, dowmTime);
        }

        private List<DataReport> GetFilterDataReportList(string myChannelId, DateTime upTime, DateTime dowmTime)
        {
            var list = GetCreatedList(upTime, dowmTime);

            // active 
            var activeNum = (from m in _Assctx.ChannelResults
                             where m.datetime.CompareTo(upTime) <= 0
                             && m.datetime.CompareTo(dowmTime) > 0
                             && m.channelId == myChannelId
                             group m by m.datetime.Date into g
                             select new DataReportColumn
                             {
                                 MyDateTime = g.Key.ToShortDateString(),
                                 Num = g.Count()
                             });
            FillDataReportList(activeNum, list, DataReportColumnType.Active);

            // start 0
            var startNum = (from m in _Assctx.ChannelResultALLs
                             where m.datetime.CompareTo(upTime) <= 0
                             && m.datetime.CompareTo(dowmTime) > 0
                             && m.channelId == myChannelId
                             group m by m.datetime.Date into g
                             select new DataReportColumn
                             {
                                 MyDateTime = g.Key.ToShortDateString(),
                                 Num = g.Select(l => l.devicetoken).Distinct().Count()
                             });
            FillDataReportList(startNum, list, DataReportColumnType.Start);

            // pay 
            var payNum = (from m in _Lilyctx.userpayments
                          where m.time.Value != null
                          && m.time.Value.CompareTo(upTime) <= 0
                          && m.time.Value.CompareTo(dowmTime) > 0
                          && m.channelId == myChannelId
                          group m by m.time.Value.Date into g
                          select new DataReportColumn
                          {
                              MyDateTime = g.Key.ToShortDateString(),
                              Num = g.Count()
                          });
            FillDataReportList(payNum, list, DataReportColumnType.Pay);

            // income            
            /*var proportion = (from m in _Assctx.Channels
                              where m.channelId == myChannelId
                              select m.proportion).FirstOrDefault() ?? 0; */
            var incomeNum = (from m in _Lilyctx.userpayments
                             where m.time.Value != null
                                && m.time.Value.CompareTo(upTime) <= 0
                                && m.time.Value.CompareTo(dowmTime) > 0
                                && m.channelId == myChannelId
                             group m by m.time.Value.Date into g
                             select new DataReportColumn
                             {
                                 MyDateTime = g.Key.ToShortDateString(),
                                 //DNum = (g.Sum(m => m.money) ?? 0) * (decimal)proportion
                                 DNum = (g.Sum(m => m.money) ?? 0)
                             });
            FillDataReportList(incomeNum, list, DataReportColumnType.Income);

            return list;
        }

        private List<DataReport> GetCreatedList(DateTime upTime, DateTime dowmTime)
        {
            var list = new List<DataReport>();
            for (DateTime t = upTime; t > dowmTime; t = t.AddDays(-1))
            {
                list.Add(new DataReport
                {
                    MyDateTime = t.ToShortDateString(),
                    ActiveNum = 0,
                    StartNum = 0,
                    PayNum = 0,
                    IncomeNum = 0.00m
                });
            }

            return list;
        }

        private List<DataReport> FillDataReportList(IQueryable<DataReportColumn> queryRes, List<DataReport> list, DataReportColumnType myType)
        {
            foreach (var i in queryRes)
            {
                foreach (var j in list)
                {
                    if (i.MyDateTime == j.MyDateTime)
                    {
                        if (myType == DataReportColumnType.Active)
                            j.ActiveNum = i.Num;
                        else if (myType == DataReportColumnType.Start)
                            j.StartNum = i.Num;
                        else if (myType == DataReportColumnType.Pay)
                            j.PayNum = i.Num;
                        else if (myType == DataReportColumnType.Income)
                            j.IncomeNum = i.DNum;
                        break;
                    }
                }
            }

            return list;
        }

        private string GenerateDataExcel(List<DataReport> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>时间</th><th>当日激活</th><th>当日启动</th><th>当日付费人数</th><th>当日收入</th></tr>");
            foreach (var item in list)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.MyDateTime + "</td>");
                sb.Append("<td>" + item.ActiveNum + "</td>");
                sb.Append("<td>" + item.StartNum + "</td>");
                sb.Append("<td>" + item.PayNum + "</td>");
                sb.Append("<td>" + item.IncomeNum + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GetChannelId()
        {
            var myUser = Membership.GetUser(User.Identity.Name);
            Guid gUserId = new Guid();
            if (null != myUser) {
                gUserId = (Guid)myUser.ProviderUserKey;
            }
            var myChannelId = (from m in _Assctx.UsersInChannels
                               where m.UserId == gUserId
                               select m.ChannelId).FirstOrDefault();

            SetViewDataChannelName(myChannelId);            

            return myChannelId;
        }

        private void SetViewDataChannelName(string myChannelId)
        {
            ViewData["tbChannelName"] = string.Empty;
            if (!string.IsNullOrEmpty(myChannelId))
            {
                var myChannelObj = (from m in _Assctx.Channels
                                     where m.channelId == myChannelId
                                     select m).FirstOrDefault();
                //ViewData["tbChannelName"] = myChannelObj.channelName + "(" + myChannelObj.channelId + ")";
                ViewData["tbChannelName"] = myChannelObj.channelName;
            }
        }

        #endregion
    }
}
