using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Permission;
using DataPersist;

namespace WebNotifications.Controllers
{
    [Description("渠道配置(底下权限可以继承当前权限)")]
    public class ChannelController : BaseController
    {
        DataJackModelDataContext _Assctx = new DataJackModelDataContext();
        DataLilyModelDataContext _Lilyctx = new DataLilyModelDataContext();

        private static int DeviceListShowDays = 7;

        #region Channel Config

        [Description("渠道配置--渠道列表")]
        public ActionResult ChannelIndex(int? page)
        {
            var res = from m in _Assctx.Channels
                      select m;
            var pageList = res.ToPagedList<Channel>(page ?? 1, SiteConfigHelper.DefaultPageSize);
            return View(pageList);
        }

        [Description("渠道配置--渠道新增")]
        public ActionResult ChannelAdd()
        {
            return View();
        }

        [Description("渠道配置--渠道新增")]
        [HttpPost]
        public ActionResult ChannelAdd(Channel channel)
        {
            using (DataJackModelDataContext _ctx = new DataJackModelDataContext())
            {
                channel.channelId = channel.channelId.Trim();
                _ctx.Channels.InsertOnSubmit(channel);
                _ctx.SubmitChanges();
            }

            return RedirectToAction("ChannelIndex");
        }

        [Description("渠道配置--渠道新增取消")]
        public ActionResult ChannelAddCancleAction() {
            return RedirectToAction("ChannelIndex"); 
        }

        [Description("渠道配置--渠道修改")]
        public ActionResult ChannelModify(string id)
        {
            var obj = (from m in _Assctx.Channels
                       where m.channelId == id
                       select m).FirstOrDefault();
            ViewData.Model = obj;
            
            return View();
        }

        [Description("渠道配置--渠道修改")]
        [HttpPost]
        public ActionResult ChannelModify(Channel channel)
        {
            using (DataJackModelDataContext _ctx = new DataJackModelDataContext())
            {
                channel.channelName = channel.channelName.Trim();
                _ctx.Channels.Attach(channel);
                _ctx.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, channel);
                _ctx.SubmitChanges();
            }

            return RedirectToAction("ChannelIndex");
        }

        [Description("渠道配置--渠道修改取消")]
        public ActionResult ChannelModifyCancleAction()
        {
            return RedirectToAction("ChannelIndex");
        }

        [Description("渠道配置--渠道删除操作")]
        [HttpPost]
        public ActionResult ChannelDelete(Channel channel)
        {
            using (DataJackModelDataContext _ctx = new DataJackModelDataContext())
            {
                var oldChannel = (from m in _ctx.Channels
                                  where m.channelId == channel.channelId
                                  select m).FirstOrDefault();
                if (null != oldChannel)
                {
                    _ctx.Channels.DeleteOnSubmit(oldChannel);
                    _ctx.SubmitChanges();
                }
            }

            return RedirectToAction("ChannelIndex");
        }

        #endregion

        #region Analyze from the ChannelResult

        [Description("渠道配置--安装量统计查询")]
        public ActionResult DeviceChannel(int? page)
        {            
            DateTime current = DateHelper.GetNow();
            ViewData["tbFilterTime"] = current.ToShortDateString();
            DeviceChannelModel dcm = GetDeviceChannelModel(page, current);
            
            return View(dcm);
        }

        [Description("渠道配置--安装量统计查询(按照天)")]
        public ActionResult DeciveChannelSearchByDay(string tbFilterTime)
        { 
            DateTime filterTime = DateHelper.ToMyDateTime(tbFilterTime);
            DeviceChannelModel dcm = GetDeviceChannelModel(1, filterTime);

            return View("DeviceChannel", dcm); 
        }

        private DeviceChannelModel GetDeviceChannelModel(int? page, DateTime fitlerTime)
        {
            DeviceChannelModel dcm = new DeviceChannelModel();
            dcm.DateNameList = new List<string>();
            for (int i = 0; i < DeviceListShowDays; i++)
            {
                dcm.DateNameList.Add(fitlerTime.AddDays(-i).ToString("M月d日"));
            }
            var mylist = InitAmountOfDeviceChannel(fitlerTime);
            mylist = CreateAmountOfOfDeviceChannel(mylist, fitlerTime);
            dcm.DChannelObjList = new PagedList<DeviceChannelObj>(mylist, 1, mylist.Count);

            return dcm;
        }

        private List<DeviceChannelObj> InitAmountOfDeviceChannel(DateTime filterDate)
        {
            var res = (from p in _Assctx.Channels
                       select p).OrderBy(m => m.channelId);
            var mylist = new List<DeviceChannelObj>();
            foreach (var item in res)
            {
                DeviceChannelObj obj = new DeviceChannelObj();
                obj.DeviceNumberList = this.GetDeviceChannelObjIntList();
                obj.DateTimeList = this.GetDeviceChannelTimeList(filterDate);

                obj.ChannelId = item.channelId;
                obj.ChannelName = item.channelName;
                obj.Proportion = item.proportion ?? 0;
                mylist.Add(obj);
            }

            return mylist;
        }

        private List<DeviceChannelObj> CreateAmountOfOfDeviceChannel(List<DeviceChannelObj> mylist, DateTime filterTime)
        {
            var smalltime = filterTime.AddDays(-DeviceListShowDays);
            var res = (from m in _Assctx.ChannelResults
                       where m.datetime.CompareTo(smalltime) >= 0
                       group m by new { m.channelId, m.datetime } into g
                       select new DeviceSearchResult
                       {
                           //Cooperation = g.Key.cooperation,
                           ChannelId = g.Key.channelId,
                           DateTime = g.Key.datetime,
                           Amount = g.Count()
                       }).OrderByDescending(m => m.DateTime).OrderBy(m => m.ChannelId);

            foreach (var item in res)
            {
                foreach (var j in mylist)
                {
                    if (item.ChannelId.Trim() == j.ChannelId.Trim())
                    {
                        for (int i = 1; i < j.DeviceNumberList.Count; i++)
                         {
                            DateTime flagTime = j.DateTimeList[i - 1];
                            if (flagTime.Year == item.DateTime.Year
                                && flagTime.DayOfYear == item.DateTime.DayOfYear)
                            {
                                j.DeviceNumberList[i] += item.Amount;
                                break;
                            }
                        }
                    }
                }
            }

            SetAllOfChannelDeviceNumber(mylist);

            return mylist;
        }

        private void SetAllOfChannelDeviceNumber(List<DeviceChannelObj> mylist)
        {
            var res = (from p in _Assctx.ChannelResults                       
                       group p by new { p.channelId } into g
                       select new DeviceSearchResult
                       {
                           ChannelId = g.Key.channelId,
                           Amount = g.Count(),
                       }).OrderBy(m => m.ChannelId);

            foreach (var item in res)
            {
                foreach (var j in mylist)
                {
                    if (item.ChannelId == j.ChannelId)
                    {
                        j.DeviceNumberList[0] = item.Amount;
                    }
                }
            }
        }

        private List<int> GetDeviceChannelObjIntList() {
            var list = new List<int>();
            for (int i = 0; i <= DeviceListShowDays; i++)
            {
                list.Add(0);
            }
            return list;
        }

        private List<DateTime> GetDeviceChannelTimeList(DateTime filterTime)
        {
            var list = new List<DateTime>();
            for (int i = 0; i < DeviceListShowDays; i++)
            {
                list.Add(filterTime.AddDays(-i));
            }
            return list;
        }

        #endregion       

        #region Analyze from the ChannelResult of DeviceType

        [Description("渠道配置--IOS机型列表")]
        public ActionResult DeviceTypeIOS(int? page)
        {
            ViewData["tbFilterTime"] = DateHelper.GetNow().ToShortDateString();
            var res = GetDeviceTypeObjList(DateHelper.GetSqlMinDate(), (int)DeviceCategoryEnum.IOS);
            var pageList = res.ToPagedList<DeviceTypeObj>(page ?? 1, SiteConfigHelper.DefaultPageSize);
            return View(pageList);
        }

        [Description("渠道配置--Android机型列表")]
        public ActionResult DeviceTypeAndroid(int? page)
        {
            ViewData["tbFilterTime"] = DateHelper.GetNow().ToShortDateString();
            var res = GetDeviceTypeObjList(DateHelper.GetSqlMinDate(), (int)DeviceCategoryEnum.Andorid);
            var pageList = res.ToPagedList<DeviceTypeObj>(page ?? 1, SiteConfigHelper.DefaultPageSize);
            return View(pageList);
        }

        [Description("渠道配置--IOS机型列表查询")]
        public ActionResult DeviceTypeIOSSearch(string tbFilterTime)
        {
            var res = GetDeviceTypeObjList(DateHelper.ToMyDateTime(tbFilterTime), (int)DeviceCategoryEnum.IOS);
            var pageList = res.ToPagedList<DeviceTypeObj>(1, SiteConfigHelper.DefaultPageSize);
            return View("DeviceTypeIOS", pageList);  
        }

        [Description("渠道配置--Android机型列表查询")]
        public ActionResult DeviceTypeAndroidSearch(string tbFilterTime)
        {
            var res = GetDeviceTypeObjList(DateHelper.ToMyDateTime(tbFilterTime), (int)DeviceCategoryEnum.Andorid);
            var pageList = res.ToPagedList<DeviceTypeObj>(1, SiteConfigHelper.DefaultPageSize);
            return View("DeviceTypeAndroid", pageList);  
        }

        private IQueryable<DeviceTypeObj> GetDeviceTypeObjList(DateTime filterTime, int category) {
            var res = (from m in _Assctx.Mobiles
                       where m.datetime.CompareTo(filterTime) >= 0
                       && m.mobileCategory == category
                       group m by m.mobileType into gr
                       select new DeviceTypeObj
                       {
                           Name = gr.Key,
                           Amount = gr.Count()
                       });
            return res;
        }

        #endregion

        #region Analyze from the ChannelResult of DeviceSystem

        [Description("渠道配置--IOS系统列表")]
        public ActionResult DeviceSystemIOS(int? page)
        {
            ViewData["tbFilterTime"] = DateHelper.GetNow().ToShortDateString();
            var res = GetDeviceSystemObjList(DateHelper.GetSqlMinDate(), (int)DeviceCategoryEnum.IOS);
            var pageList = res.ToPagedList<DeviceSystemObj>(page ?? 1, SiteConfigHelper.DefaultPageSize);
            return View(pageList);
        }

        [Description("渠道配置--Android系统列表")]
        public ActionResult DeviceSystemAndroid(int? page)
        {
            ViewData["tbFilterTime"] = DateHelper.GetNow().ToShortDateString();
            var res = GetDeviceSystemObjList(DateHelper.GetSqlMinDate(), (int)DeviceCategoryEnum.Andorid);
            var pageList = res.ToPagedList<DeviceSystemObj>(page ?? 1, SiteConfigHelper.DefaultPageSize);
            return View(pageList);
        }

        [Description("渠道配置--IOS系统列表查询")]
        public ActionResult DeviceSystemIOSSearch(string tbFilterTime)
        {
            var res = GetDeviceSystemObjList(DateHelper.ToMyDateTime(tbFilterTime), (int)DeviceCategoryEnum.IOS);
            var pageList = res.ToPagedList<DeviceSystemObj>(1, SiteConfigHelper.DefaultPageSize);
            return View("DeviceSystemIOS", pageList);  
        }

        [Description("渠道配置--Android系统列表查询")]
        public ActionResult DeviceSystemAndroidSearch(string tbFilterTime)
        {
            var res = GetDeviceSystemObjList(DateHelper.ToMyDateTime(tbFilterTime), (int)DeviceCategoryEnum.Andorid);
            var pageList = res.ToPagedList<DeviceSystemObj>(1, SiteConfigHelper.DefaultPageSize);
            return View("DeviceSystemAndroid", pageList);  
        }

        private IQueryable<DeviceSystemObj> GetDeviceSystemObjList(DateTime filterTime, int category)
        {
            var res = (from m in _Assctx.Mobiles
                       where m.datetime.CompareTo(filterTime) >= 0
                       && m.mobileCategory == category
                       group m by m.mobileSystem into gr
                       select new DeviceSystemObj
                       {
                           Name = gr.Key,
                           Amount = gr.Count()
                       });
            return res;
        }

        #endregion

        #region Analyze from the ChannelResult of User

        [Description("渠道配置--用户")]
        public ActionResult UserIndex()
        {
            DateTime current = DateHelper.GetNow();
            ViewData["tbFilterTime"] = current.ToShortDateString();
            var res = GetUserAmountList(current, string.Empty);
            ViewData["dpChannels"] = GetMyChannelsList();        
            return View(res);
        }

        [Description("渠道配置--用户查询列表")]
        public ActionResult UserSearchList(string tbFilterTime, string dpChannels)
        {
            DateTime current = DateHelper.ToMyDateTime(tbFilterTime);
            var res = GetUserAmountList(current, dpChannels);
            ViewData["dpChannels"] = GetMyChannelsList();
            return View("UserIndex", res); 
        }

        private List<DeviceUserObj> GetUserAmountList(DateTime filterTime, string dpChannels)
        {
            var myList = new List<DeviceUserObj>();
            for (int i = 0; i < DeviceListShowDays; i++)
			{
                DateTime date = filterTime.AddDays(-i);   
                var myobj = new DeviceUserObj();
                myList.Add(myobj);
                myobj.DateTime = date;
                // 总用户
			    var res_amount = (from m in _Lilyctx.users
                                  from b in _Lilyctx.banks
                                  where b.userid == m.userid
                                  && b.createtime.Value.Year == date.Year
                                  && b.createtime.Value.DayOfYear == date.DayOfYear
                                  && (dpChannels == string.Empty || dpChannels == m.channelid) 
                                  && (b.optype == (int)BankActionType.Register
                                        || b.optype == (int)BankActionType.GuestUpgrade
                                  )
                                  select m.userid).Distinct().Count();
                myobj.TotalAmount = res_amount;
                // 注册用户
                var register_amount = (from m in _Lilyctx.users
                                  from b in _Lilyctx.banks
                                  where b.userid == m.userid
                                  && b.createtime.Value.Year == date.Year
                                  && b.createtime.Value.DayOfYear == date.DayOfYear
                                  && b.optype == (int)BankActionType.Register
                                  && m.usertype != (int)UserType.Guest
                                  && (dpChannels == string.Empty || dpChannels == m.channelid)
                                  select m.userid).Distinct().Count();
                myobj.RegisterAmount = register_amount;
                // 社区用户
                var community_amount = (from m in _Lilyctx.users
                                       from b in _Lilyctx.banks
                                       where b.userid == m.userid
                                       && b.createtime.Value.Year == date.Year
                                       && b.createtime.Value.DayOfYear == date.DayOfYear
                                       && b.optype == (int)BankActionType.Register
                                       && m.usertype != (int)UserType.Guest
                                       && m.usertype != (int)UserType.Normal
                                       && (dpChannels == string.Empty || dpChannels == m.channelid)
                                       select m.userid).Distinct().Count();
                myobj.CommunityAmount = community_amount;
                // 游客
                var guest_amount = (from m in _Lilyctx.users
                                       from b in _Lilyctx.banks
                                       where b.userid == m.userid
                                       && b.createtime.Value.Year == date.Year
                                       && b.createtime.Value.DayOfYear == date.DayOfYear
                                       && b.optype == (int)BankActionType.Register
                                       && m.usertype == (int)UserType.Guest
                                       && (dpChannels == string.Empty || dpChannels == m.channelid)
                                       select m.userid).Distinct().Count();
                myobj.GuestAmount = guest_amount;
                // 游客转注册
                var guestToReg_amount = (from m in _Lilyctx.users
                                    from b in _Lilyctx.banks
                                    where b.userid == m.userid
                                    && b.createtime.Value.Year == date.Year
                                    && b.createtime.Value.DayOfYear == date.DayOfYear
                                    && b.optype == (int)BankActionType.GuestUpgrade
                                    && m.usertype == (int)UserType.Guest
                                    && (dpChannels == string.Empty || dpChannels == m.channelid)
                                    select m.userid).Distinct().Count();
                myobj.GuestToRegister = guestToReg_amount;
			}

            return myList;
        }

        private SelectList GetMyChannelsList()
        { 
            var res = (from m in _Assctx.Channels
                       select m);
            List<SelectListItem> channelList = new List<SelectListItem>();
            channelList.Add(
                        new SelectListItem { Text = "[所有]", Value = string.Empty }
                    );
            foreach (var item in res)
            {
                channelList.Add(
                        new SelectListItem { Text = item.channelName, Value= item.channelId }
                    );
            }

            return new SelectList(channelList, "Value", "Text");
        }

        #endregion

        #region  Analyze from the ChannelResult of Income

        [Description("渠道配置--发行渠道收入")]
        public ActionResult DeviceIncome() 
        {
            DateTime current = DateHelper.GetNow();
            ViewData["tbFilterTime"] = current.ToShortDateString();
            DeviceChannelModel dcm = GetDeviceChannelIncome(1, current);

            return View(dcm);
        }

        [Description("渠道配置--发行渠道收入查询(按照天)")]
        public ActionResult DeviceIncomeSearchByDay(string tbFilterTime)
        {
            DateTime filterTime = DateHelper.ToMyDateTime(tbFilterTime);
            DeviceChannelModel dcm = GetDeviceChannelIncome(1, filterTime);

            return View("DeviceIncome", dcm);
        }

        private DeviceChannelModel GetDeviceChannelIncome(int? page, DateTime fitlerTime)
        {
            DeviceChannelModel dcm = new DeviceChannelModel();
            dcm.DateNameList = new List<string>();
            for (int i = 0; i < DeviceListShowDays; i++)
            {
                dcm.DateNameList.Add(fitlerTime.AddDays(-i).ToString("M月d日"));
            }
            // 初始化所有数据
            var mylist = InitMoneyOfChannelIncome(fitlerTime);
            // 查询结果并复制数据
            mylist = SeachResultOfChannelIncome(mylist, fitlerTime);
            dcm.DChannelObjList = new PagedList<DeviceChannelObj>(mylist, 1, mylist.Count);

            return dcm;
        }

        private List<DeviceChannelObj> InitMoneyOfChannelIncome(DateTime filterDate)
        {
            var res = (from p in _Assctx.Channels
                       select p).OrderBy(m => m.channelId);
            var mylist = new List<DeviceChannelObj>();
            foreach (var item in res)
            {
                DeviceChannelObj obj = new DeviceChannelObj();
                obj.PayMoneyList = this.GetPayMoneyObjDoubleList();
                obj.DateTimeList = this.GetDeviceChannelTimeList(filterDate);

                obj.ChannelId = item.channelId;
                obj.ChannelName = item.channelName;
                obj.Proportion = item.proportion ?? 0;
                mylist.Add(obj);
            }

            return mylist;
        }

        private List<DeviceChannelObj> SeachResultOfChannelIncome(List<DeviceChannelObj> mylist, DateTime filterTime)
        {
            DateTime smallDate = filterTime.AddDays(-DeviceListShowDays);
            var res = (from p in _Lilyctx.userpayments                                          
                       where p.channelId != null
                       && p.channelId != string.Empty
                       && p.time != null
                       && p.time.Value.CompareTo(smallDate) >= 0
                       group p by new { p.channelId, p.time.Value } into g
                       select new DeviceSearchResult
                       {
                           ChannelId = g.Key.channelId,
                           Money = (double)(g.Sum(t => t.money) ?? 0),
                           DateTime = g.Key.Value
                       }).OrderByDescending(m => m.DateTime).OrderBy(m => m.ChannelId);

            foreach (var item in res)
            {
                foreach (var j in mylist) 
                {
                    if (item.ChannelId == j.ChannelId)
                    {
                        for (int i = 1; i < j.PayMoneyList.Count; i++)
                        {
                            DateTime flagTime = j.DateTimeList[i - 1];
                            if (flagTime.Year == item.DateTime.Year
                                && flagTime.DayOfYear == item.DateTime.DayOfYear)
                            {
                                j.PayMoneyList[i] += item.Money;
                                break;
                            }
                        }
                    }                 
                }
            }

            SetAllOfChannelIncome(mylist);

            return mylist;
        }

        private void SetAllOfChannelIncome(List<DeviceChannelObj> mylist)
        {
            var res = (from p in _Lilyctx.userpayments
                       where p.channelId != null
                       && p.channelId != string.Empty
                       group p by new { p.channelId } into g
                       select new DeviceSearchResult
                       {
                           ChannelId = g.Key.channelId,
                           Money = (double)(g.Sum(t => t.money) ?? 0),                         
                       }).OrderBy(m => m.ChannelId);

            foreach (var item in res)
            {
                foreach (var j in mylist)
                {
                    if (item.ChannelId == j.ChannelId)
                    {
                        //j.PayMoneyList[0] = item.Money * (item.Proportion ?? 0); // 算提成
                        j.PayMoneyList[0] = item.Money;
                        break;
                    }
                }
            }
        }

        private List<double> GetPayMoneyObjDoubleList()
        {
            var list = new List<double>();
            for (int i = 0; i <= DeviceListShowDays; i++)
            {
                list.Add(0);
            }
            return list;
        }

        #endregion
    }
}
