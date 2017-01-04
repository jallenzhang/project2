using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.ComponentModel;

using WebNotifications.Models;
using WebNotifications.Helper;
using Webdiyer.WebControls.Mvc;

namespace WebNotifications.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DataAnalyzeController : Controller
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 软件注册量
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult V23SoftRegisterNum(int? page)
        {        
            var userList = 
                from m in dataContext.users
                select m;

            ViewBag.Message = userList.Count();
            var pageList = userList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        /// <summary>
        /// Download File
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV23SoftRegisterNum()
        {
            string conent = GenerateV23SoftRegisterNumExcel();

            ExportToFile.DownLoadAsExcel("软件注册量", conent);

            return RedirectToAction("V23SoftRegisterNum");
        }

        /// <summary>
        /// 游戏人数数据.
        /// </summary>
        /// <returns></returns>
        public ActionResult V31HumanInGameNum()
        {
            double intervalTime = -5;
            DateTime currentDate = DateHelper.GetNow();
            currentDate = currentDate.AddMinutes(intervalTime);
            var userList = from m in dataContext.users
                           where (m.logintime >= currentDate)
                           select m;

            ViewData.Model = userList;
            ViewBag.Message = userList.Count();

            return View();
        }

        /// <summary>
        /// 输赢平率数据
        /// </summary>
        /// <returns></returns>
        public ActionResult V34WinLossData(int? page)
        {
            var userList =
                from m in dataContext.users
                select m;
            ViewBag.Message = userList.Count();
            var pageList = userList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        /// <summary>
        /// 输赢平率数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV34WinLossData(int? page)
        {
            string conent = GenerateV34WinLossDataExcel();

            ExportToFile.DownLoadAsExcel("输赢平率数据", conent);

            return RedirectToAction("V34WinLossData");
        }

        /// <summary>
        /// 筹码总量
        /// </summary>
        /// <returns></returns>
        public ActionResult V41ChipsTotal()
        {
            var totalChips = (from m in dataContext.users
                           select m.chips).Sum();
            var userNum = (from m in dataContext.users
                           select m).Count();
            ViewData["totalChips"] = totalChips;
            ViewData["averageChips"] = totalChips / userNum;

            return View();
        }

        /// <summary>
        /// 用户注册赠送量
        /// </summary>
        /// <returns></returns>
        public ActionResult V43UserRegisterBeSentChipsNum()
        {
            // 2 means register
            int optype = 2;
            var totalChips = (from m in dataContext.banks
                              where m.optype == optype
                              select m.bankout).Sum();
            ViewData["totalChips"] = totalChips;

            return View();
        }

        #region Private Method

        private string GenerateV23SoftRegisterNumExcel()
        {
            var userList = from m in dataContext.users
                           select m;
            StringBuilder sb = new StringBuilder();
            sb.Append("<table><tr><th>id</th><th>user-id</th></tr>");
            foreach(var item in userList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.id + "</td>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.nickname ?? string.Empty + "</td>");
                //sb.Append("<td>" + item.username ?? string.Empty + "</td>");
                //sb.Append("<td>" + item.mail ?? string.Empty + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV34WinLossDataExcel()
        {
            var userList = from m in dataContext.users
                           select m;
            StringBuilder sb = new StringBuilder();
            sb.Append("<table><tr><th>id</th><th>user-id</th><th>totalgame</th><th>wins</th><th>win percent</th></tr>");
            foreach (var item in userList)
            {
                double mtotal = item.totalgame ?? 0;
                double mwins = item.wins ?? 0;
                double res = 0;
                if(mtotal != 0) res = mwins / mtotal;
                res =  Math.Round(res, 2);
                sb.Append("<tr>");
                sb.Append("<td>" + item.id + "</td>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.nickname ?? string.Empty + "</td>");
                //sb.Append("<td>" + item.username ?? string.Empty + "</td>");
                //sb.Append("<td>" + item.mail ?? string.Empty + "</td>");
                sb.Append("<td>" + mtotal + "</td>");
                sb.Append("<td>" + mwins + "</td>");
                sb.Append("<td>" + res + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        #endregion

        //public ActionResult DetailsReport()
        //{
        //    // get data
        //    var userList = from m in dataContext.users
        //                   select m.id;
        //    // ViewData.Model = userList;

        //    // report
        //    LocalReport localReport = new LocalReport();
        //    // ---------
        //    //localReport.LoadReportDefinition(GenerateRdlc());
        //    // ---------
        //    localReport.ReportPath = Server.MapPath("~/Content/Reports/V23SoftRegisterNumReport.rdlc");
        //    ReportDataSource reportDataSource = new ReportDataSource("Customers", CreateDataTable());
        //    //ReportDataSource reportDataSource = new ReportDataSource("Customers", userList);
        //    //localReport.DataSources.Add(reportDataSource);
            
        //    //------------
        //    localReport.Refresh();

        //    // ------------

        //    string reportType = "PDF";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;

        //    string deviceInfo =
        //    "<DeviceInfo>" +
        //    "  <OutputFormat>PDF</OutputFormat>" +
        //    "  <PageWidth>8.5in</PageWidth>" +
        //    "  <PageHeight>11in</PageHeight>" +
        //    "  <MarginTop>0.5in</MarginTop>" +
        //    "  <MarginLeft>1in</MarginLeft>" +
        //    "  <MarginRight>1in</MarginRight>" +
        //    "  <MarginBottom>0.5in</MarginBottom>" +
        //    "</DeviceInfo>";

        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;

        //    //Render the report
        //    renderedBytes = localReport.Render(
        //        reportType,
        //        deviceInfo,
        //        out mimeType,
        //        out encoding,
        //        out fileNameExtension,
        //        out streams,
        //        out warnings);

        //    //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
        //    return File(renderedBytes, mimeType);
        //}

        //public System.Data.DataTable CreateDataTable()
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    dt.Columns.Add("Key", typeof(string));
        //    dt.Columns.Add("Group1", typeof(string));
        //    dt.Columns.Add("Group2", typeof(string));
        //    dt.Columns.Add("Value", typeof(double));
        //    dt.Rows.Add("K1", "G1", "M1", 34);
        //    dt.Rows.Add("K1", "G1", "M2", 22);
        //    dt.Rows.Add("K1", "G2", "M2", 55);
        //    dt.Rows.Add("K2", "G6", "M1", 155);
        //    dt.Rows.Add("K2", "G7", "M1", 535);

        //    return dt;
        //}
        
        //public MemoryStream GenerateRdlc()
        //{
        //    DynamicReport dr = new DynamicReport(Server.MapPath("~/Content/Reports/V23SoftRegisterNumReport.rdlc"));

        //    dr.AddDetailsCell("xyh", "=Fields!fy.Value");
        //    dr.AddTableColumn();
        //    dr.AddTableHeaderCell("xyh", "学院");

        //    MemoryStream ms = new MemoryStream();
        //    XmlSerializer serializer = new XmlSerializer(typeof(XmlDocument));
        //    serializer.Serialize(ms, dr.doc);
        //    ms.Position = 0;

        //    return ms;
        //}
    }
}
