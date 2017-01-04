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
using DataPersist;
using WebNotifications.Models.Custom;
using WebNotifications.Permission;
using OpenFlashChart;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles = "Administrator, ProductRole, OperationRole, MarketRole")]
    [Description("产品数据分析(底下权限可以继承当前权限)")]
    public class ProductDataController : BaseController
    {        
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();

        [Description("产品数据分析--列表页面")]
        public ActionResult Index()
        {
            return View();
        }

        #region View

        [Description("产品数据分析--注册奖励筹码数页面")]
        /// <summary>
        /// 注册奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V1RegistChips()
        {
            return View();
        }

        [Description("产品数据分析--每日奖励筹码数页面")]
        /// <summary>
        /// 每日奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V2EveryChips()
        {
            return View();
        }

        [Description("产品数据分析--用户购买的筹码数量页面")]
        /// <summary>
        /// 用户购买的筹码数量
        /// </summary>
        /// <returns></returns>
        public ActionResult V3UserBuyChips()
        {
            return View();
        }

        [Description("产品数据分析--活动赠送的筹码量页面")]
        /// <summary>
        /// 活动赠送的筹码量
        /// </summary>
        /// <returns></returns>
        public ActionResult V4ActiveSendChips()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            ViewData["tbActiveList"] = items;            
            return View();
        }

        [Description("产品数据分析--用户建房筹码数页面")]
        /// <summary>
        /// 用户建房筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V5CreateRoomChips()
        {
            return View();
        }

        [Description("产品数据分析--牌桌税收页面")]
        /// <summary>
        /// 牌桌税收
        /// </summary>
        /// <returns></returns>
        public ActionResult V6TableTax()
        {
            return View();
        }

        [Description("产品数据分析--玩家购买筹码花费页面")]
        /// <summary>
        /// 玩家购买筹码花费
        /// </summary>
        /// <returns></returns>
        public ActionResult V7CostOfPalyerBuyChips()
        {
            return View();
        }

        [Description("产品数据分析--各类礼物被购买次数页面")]
        /// <summary>
        /// 各类礼物被购买次数
        /// </summary>
        /// <returns></returns>
        public ActionResult V8GiftsBuyTimes()
        {
            return View();
        }

        [Description("产品数据分析--场景使用人数页面")]
        /// <summary>
        /// 场景使用人数
        /// </summary>
        /// <returns></returns>
        public ActionResult V9SceneUsedTimes()
        {
            return View();
        }

        [Description("产品数据分析--角色使用人数页面")]
        /// <summary>
        /// 角色使用人数
        /// </summary>
        /// <returns></returns>
        public ActionResult V10RoleUsedTimes()
        {
            return View();
        }

        //[Description("产品数据分析--牌局默认语句使用次数统计页面")]
        ///// <summary>
        ///// 牌局默认语句使用次数统计
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult V11LanguageUsedTimes()
        //{
        //    return View();
        //}

        [Description("产品数据分析--世界喊话功能使用次数页面")]
        /// <summary>
        /// 世界喊话功能使用次数
        /// </summary>
        /// <returns></returns>
        public ActionResult V12WorldCallUsedTimes()
        {
            return View();
        }

        [Description("产品数据分析--今日对局数和对局时间页面")]
        /// <summary>
        /// 今日对局数和对局时间
        /// </summary>
        /// <returns></returns>
        public ActionResult V23TodayRoundTimesAndDate()
        {
            ViewData["tbDate"] = DateHelper.GetNow().ToShortDateString();
            return View();
        }  

        #endregion

        #region Button Action

        [Description("产品数据分析--注册奖励筹码数查询")]
        /// <summary>
        /// 注册奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V1RegistChipsAction()
        {
            var gustType = (int)UserType.Guest;
            var registerType = (int)UserType.GameCenter;
            // 登陆总人数: 就是这个数据库存在的用户数量
            var loginCount = (from m in dataContext.users
                              //where m.status != 1   // 1 = offline
                              select m.id).Count();
            ViewData["tbLoginCount"] = loginCount;

            // Guest总人数
            var gusetCount = (from m in dataContext.users
                              where m.usertype == gustType // guset = 7
                              select m.id).Count();
            ViewData["tbGusetCount"] = gusetCount;

            // 注册总人数
            var registCount = (from m in dataContext.users
                               where m.usertype != gustType // guset = 7
                               select m.id).Count();
            ViewData["tbRegistCount"] = registCount;

            // Guest用户奖励总筹码数
            var guestChips = (from m in dataContext.banks
                              from o in dataContext.users
                              where
                               m.userid == o.userid
                               && o.usertype == gustType
                               select m.bankout).Sum();
            var t_guestChips = CodeHelper.GetNumberOrZero(guestChips);
            ViewData["tbGuestChips"] = t_guestChips;

            // 注册用户奖励总筹码数
            var registChips = (from m in dataContext.banks
                               where m.optype == registerType  // Register
                               select m.bankout).Sum();
            var t_registChips = CodeHelper.GetNumberOrZero(registChips);
            ViewData["tbRegistChips"] = t_registChips;


            // 总计奖励筹码数
            ViewData["tbTotalChips"] = t_guestChips + t_registChips;        

            return View("V1RegistChips");
        }

        [Description("产品数据分析--注册奖励筹码数下载")]
        /// <summary>
        /// 注册奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV1RegistChipsAction()
        {
            string conent = GenerateV1RegistChipsExcel();

            ExportToFile.DownLoadAsExcel("注册奖励筹码数", conent);

            return RedirectToAction("V1RegistChips");
        }

        [Description("产品数据分析--每日奖励筹码数查询")]
        /// <summary>
        /// 每日奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V2EveryChipsAction()
        {
            var today = DateHelper.GetNow();
            var gustType = (int)UserType.Guest;
            var normalType = (int)UserType.Normal;
            var bankAwardType = (int)BankActionType.Award;
            //今日登陆总人数
            var loginCount = (from m in dataContext.users
                              where m.logintime.Value.Year == today.Year
                              && m.logintime.Value.Month == today.Month
                              && m.logintime.Value.Day == today.Day
                              select m.id).Count();
            ViewData["tbTodayLoginNum"] = loginCount;

            //今日登陆Guest总人数
            var gusetCount = (from m in dataContext.users
                              where m.usertype == gustType
                              && m.logintime.Value.Year == today.Year
                              && m.logintime.Value.Month == today.Month
                              && m.logintime.Value.Day == today.Day
                              select m.id).Count();
            ViewData["tbTodayLoginGuestNum"] = gusetCount;

            // 今日登陆注册用户数
            var registCount = (from m in dataContext.users
                               where m.usertype == normalType
                               && m.logintime.Value.Year == today.Year
                               && m.logintime.Value.Month == today.Month
                               && m.logintime.Value.Day == today.Day
                               select m.id).Count();
            ViewData["tbTodayLoginRegistNum"] = registCount;

            // 今日登陆付费用户数
            var payCount = (from m in dataContext.userpayments
                            where m.time.Value.Year == today.Year
                            && m.time.Value.Month == today.Month
                            && m.time.Value.Day == today.Day
                            select m.id).Count();
            ViewData["tbTodayLoginPayNum"] = payCount;

            // Guest用户今日奖励总筹码数
            var rewardChips = (from m in dataContext.banks
                              from o in dataContext.users
                              where
                               m.userid == o.userid
                               && o.usertype == gustType
                               && m.createtime.Value.Year == today.Year
                               && m.createtime.Value.Month == today.Month
                               && m.createtime.Value.Day == today.Day
                               && m.optype == bankAwardType
                               select m.bankout).Sum();
            ViewData["tbTodayGuestChips"] = CodeHelper.GetNumberOrZero(rewardChips);

            // 注册用户今日奖励总筹码数
            var registChips = (from m in dataContext.banks
                               from u in dataContext.users
                               where
                               m.userid == u.userid
                               && u.usertype == normalType
                               && m.optype == bankAwardType  // Register
                               && m.createtime.Value.Year == today.Year
                               && m.createtime.Value.Month == today.Month
                               && m.createtime.Value.Day == today.Day
                               select m.bankout).Sum();
            ViewData["tbTodayRegistChips"] = CodeHelper.GetNumberOrZero(registChips);

            // 付费用户今日奖励筹码总数
            var payTotalChips = (from m in dataContext.banks
                                 from o in dataContext.userpayments
                               where m.userid == o.userid
                               && m.optype == bankAwardType
                               && m.createtime.Value.Year == today.Year
                               && m.createtime.Value.Month == today.Month
                               && m.createtime.Value.Day == today.Day
                               select m.bankout).Sum();
            ViewData["tbTodayPayChips"] = CodeHelper.GetNumberOrZero(payTotalChips);

            // 总计今日奖励筹码数            
            ViewData["tbTodayTotalChips"] = CodeHelper.GetNumberOrZero(rewardChips) + CodeHelper.GetNumberOrZero(registChips) + CodeHelper.GetNumberOrZero(payTotalChips);

            return View("V2EveryChips");
        }

        [Description("产品数据分析--每日奖励筹码数下载")]
        /// <summary>
        /// 每日奖励筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV2EveryChipsAction()
        {
            string conent = GenerateV2EveryChipsExcel();

            ExportToFile.DownLoadAsExcel("每日奖励筹码数", conent);

            return RedirectToAction("V2EveryChips");
        }

        [Description("产品数据分析--用户购买的筹码数量查询")]
        /// <summary>
        /// 用户购买的筹码数量
        /// </summary>
        /// <returns></returns>
        public ActionResult V3UserBuyChipsAction()
        {
            var today = DateHelper.GetNow();
            int buyChipsType = (int)ItemType.Chip;
            // 今日购买筹码人数
            var buyUserChipsNum = (from m in dataContext.userpayments
                                   where m.type == buyChipsType  // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                               select m.userid).Distinct().Count();
            ViewData["tbBuyChipsUserNum"] = buyUserChipsNum;

            // 今日用户购买的筹码数量
            var buyChipsNum = (from m in dataContext.userpayments
                               where m.type == buyChipsType   // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                               select m.itemid).Sum();
            ViewData["tbBuyChipsNum"] = CodeHelper.GetNumberOrZero(buyChipsNum);

            return View("V3UserBuyChips");
        }

        [Description("产品数据分析--用户购买的筹码数量下载")]
        /// <summary>
        /// 用户购买的筹码数量
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV3UserBuyChipsAction()
        {
            string conent = GenerateV3UserBuyChipsExcel();

            ExportToFile.DownLoadAsExcel("用户购买的筹码数量", conent);

            return RedirectToAction("V3UserBuyChips");
        }

        [Description("产品数据分析--活动赠送的筹码量查询")]
        /// <summary>
        /// 活动赠送的筹码量
        /// </summary>
        /// <returns></returns>
        public ActionResult V4ActiveSendChipsAction()
        {
            // can not get

            return RedirectToAction("V4ActiveSendChips");
        }

        [Description("产品数据分析--活动赠送的筹码量下载")]
        /// <summary>
        /// 活动赠送的筹码量
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV4ActiveSendChipsAction()
        {
            //string conent = GenerateV1RegistChipsExcel();

            //ExportToFile.DownLoadAsExcel("活动赠送的筹码量", conent);

            return RedirectToAction("V4ActiveSendChips");
        }

        [Description("产品数据分析--用户建房筹码数查询")]
        /// <summary>
        /// 用户建房筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult V5CreateRoomChipsAction()
        {
            var today = DateHelper.GetNow();
            int createGameType = (int)BankActionType.CreatGame;
            // 今日建房次数            
            var createRoomTimes = (from m in dataContext.banks
                                   where m.optype == createGameType   // 102 = CreateGame
                                   && m.createtime.Value.Year == today.Year
                                   && m.createtime.Value.Month == today.Month
                                   && m.createtime.Value.Day == today.Day
                                   select m.bankid).Count();
            ViewData["tbCreateRoomTimes"] = createRoomTimes;

            // 今日建房扣除的总费用
            var createRoomTotalMoney = (from m in dataContext.banks
                                        where m.optype == createGameType   // 102 = CreateGame
                                        && m.createtime.Value.Year == today.Year
                                        && m.createtime.Value.Month == today.Month
                                        && m.createtime.Value.Day == today.Day
                                        select m.bankin).Sum();
            ViewData["tbCreateRoomTotalMoney"] = CodeHelper.GetNumberOrZero(createRoomTotalMoney);

            return View("V5CreateRoomChips");
        }

        [Description("产品数据分析--用户建房筹码数下载")]
        /// <summary>
        /// 用户建房筹码数
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadV5CreateRoomChipsAction()
        {
            string conent = GenerateV5CreateRoomChipsExcel();

            ExportToFile.DownLoadAsExcel("用户建房筹码数", conent);

            return RedirectToAction("V5CreateRoomChips");
        }

        [Description("产品数据分析--牌桌税收查询")]
        /// <summary>
        /// 牌桌税收
        /// </summary>        
        /// <returns></returns>
        public ActionResult V6TableTaxAction()
        {
            // can not get now
            var today = DateHelper.GetNow();
            int gameTaxType = (int)BankActionType.GameTax;
            // 今日对局次数    
            var roundTimes = (from m in dataContext.banks
                                   where m.optype == gameTaxType
                                   && m.createtime.Value.Year == today.Year
                                   && m.createtime.Value.Month == today.Month
                                   && m.createtime.Value.Day == today.Day
                                   select m.bankid).Count();
            ViewData["tbRoundTimes"] = roundTimes;

            // 今日牌桌回收的筹码数
            var recvTableChips = (from m in dataContext.banks
                                        where m.optype == gameTaxType  
                                        && m.createtime.Value.Year == today.Year
                                        && m.createtime.Value.Month == today.Month
                                        && m.createtime.Value.Day == today.Day
                                        select m.bankin).Sum();
            ViewData["tbRecvTableChips"] = CodeHelper.GetNumberOrZero(recvTableChips);

            return View("V6TableTax");
        }

        [Description("产品数据分析--牌桌税收下载")]
        /// <summary>
        /// 牌桌税收
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV6TableTaxAction()
        {
            string conent = GenerateV6TableTaxExcel();

            ExportToFile.DownLoadAsExcel("牌桌税收", conent);

            return RedirectToAction("V6TableTax");
        }

        [Description("产品数据分析--玩家购买筹码花费查询")]
        /// <summary>
        /// 玩家购买筹码花费
        /// </summary>        
        /// <returns></returns>
        public ActionResult V7CostOfPalyerBuyChipsAction()
        {
            var today = DateHelper.GetNow();
            int buyChipsType = (int)ItemType.Chip;
            // 今日购买筹码人数
            var buyUserChipsNum = (from m in dataContext.userpayments
                                   where m.type == buyChipsType  // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                                   select m.userid).Distinct().Count();
            ViewData["tbUserNum"] = buyUserChipsNum;

            // 今日用户购买的筹码数量
            var buyChipsNum = (from m in dataContext.userpayments
                               where m.type == buyChipsType   // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                               select m.itemid).Sum();
            ViewData["tbTotalChips"] = CodeHelper.GetNumberOrZero(buyChipsNum);

            return View("V7CostOfPalyerBuyChips");
        }

        [Description("产品数据分析--玩家购买筹码花费下载")]
        /// <summary>
        /// 玩家购买筹码花费
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV7CostOfPalyerBuyChipsAction()
        {
            string conent = GenerateV7CostOfPalyerBuyChipsExcel();

            ExportToFile.DownLoadAsExcel("玩家购买筹码花费", conent);

            return RedirectToAction("V7CostOfPalyerBuyChips");
        }

        [Description("产品数据分析--各类礼物被购买次数查询")]
        /// <summary>
        /// 各类礼物被购买次数
        /// </summary>        
        /// <returns></returns>
        public ActionResult V8GiftsBuyTimesAction()
        {
            int buyGiftType = (int)BankActionType.BuyGift;

            // 各类礼物被购买次数
            var giftBuyTimes = (from m in dataContext.banks
                             where m.optype == buyGiftType
                             group m by m.itemid into tgroup
                             orderby tgroup.Key
                             select new CustomObject
                             {
                                Count = tgroup.Count(),
                                Name = LilyGiftHelper.GetGiftNameById(tgroup.Key)    
                             });
            ViewData.Model = giftBuyTimes;

            return View("V8GiftsBuyTimes");
        }

        [Description("产品数据分析--各类礼物被购买次数下载")]
        /// <summary>
        /// 各类礼物被购买次数
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV8GiftsBuyTimesAction()
        {
            string conent = GenerateV8GiftsBuyTimesExcel();

            ExportToFile.DownLoadAsExcel("各类礼物被购买次数", conent);

            return RedirectToAction("V8GiftsBuyTimes");
        }

        [Description("产品数据分析--场景使用人数查询")]
        /// <summary>
        /// 场景使用人数
        /// </summary>        
        /// <returns></returns>
        public ActionResult V9SceneUsedTimesAction()
        {
            var room = (from m in dataContext.users
                         group m by m.livingroomtype into roomgroup
                         select new
                         {
                             count = roomgroup.Count(),
                             roomtype = roomgroup.Key
                         });
            // int[] types = {0, 1, 2, 4, 8, 16, 32};
            int[] types = { 1, 2, 4, 8, 16, 32 };
            foreach (var item in types)
            {
                ViewData["tbRoomType" + item] = 0;
            }
            int cnt = 0;
            foreach (var item in room)
            {
                string key = "tbRoomType" + item.roomtype;
                if (item.roomtype <= 1) cnt += item.count;  
                if (item.roomtype == 1)
                    ViewData[key] = cnt;
                else if (item.roomtype > 1)
                    ViewData[key] = item.count;
            }

            return View("V9SceneUsedTimes");
        }

        [Description("产品数据分析--场景使用人数下载")]
        /// <summary>
        /// 场景使用人数
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV9SceneUsedTimesAction()
        {
            string conent = GenerateV9SceneUsedTimesExcel();

            ExportToFile.DownLoadAsExcel("场景使用人数", conent);

            return RedirectToAction("V9SceneUsedTimes");
        }

        [Description("产品数据分析--角色使用人数查询")]
        /// <summary>
        /// 角色使用人数
        /// </summary>        
        /// <returns></returns>
        public ActionResult V10RoleUsedTimesAction()
        {
            var roles = (from m in dataContext.users
                         group m by m.avator into rolesgroup
                        select new
                        {
                            count = rolesgroup.Count(),
                            roletype = rolesgroup.Key
                        });
            for (int i = 0; i <= 8; i++)
            {
                ViewData["tbRoleType" + i] = 0;
            }
            foreach (var item in roles)
            {
                ViewData["tbRoleType" + item.roletype] = item.count;
            }

            return View("V10RoleUsedTimes");
        }

        [Description("产品数据分析--角色使用人数下载")]
        /// <summary>
        /// 角色使用人数
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV10RoleUsedTimesAction()
        {
            string conent = GenerateV10RoleUsedTimesExcel();

            ExportToFile.DownLoadAsExcel("角色使用人数", conent);

            return RedirectToAction("V10RoleUsedTimes");
        }

        //[Description("产品数据分析--牌局默认语句使用次数统计查询")]
        ///// <summary>
        ///// 牌局默认语句使用次数统计
        ///// </summary>        
        ///// <returns></returns>
        //public ActionResult V11LanguageUsedTimesAction()
        //{

        //    return RedirectToAction("V11LanguageUsedTimes");
        //}

        //[Description("产品数据分析--牌局默认语句使用次数统计下载")]
        ///// <summary>
        ///// 牌局默认语句使用次数统计
        ///// </summary>        
        ///// <returns></returns>
        //public ActionResult DownloadV11LanguageUsedTimesAction()
        //{
        //    string conent = GenerateV1RegistChipsExcel();

        //    ExportToFile.DownLoadAsExcel("牌局默认语句使用次数统计", conent);

        //    return RedirectToAction("V11LanguageUsedTimes");
        //}

        [Description("产品数据分析--世界喊话功能使用次数查询")]
        /// <summary>
        /// 世界喊话功能使用次数
        /// </summary>        
        /// <returns></returns>
        public ActionResult V12WorldCallUsedTimesAction()
        {
            int d_day = 7;
            
            int broadType = (int)BankActionType.Broadcast;
            DateTime current = DateHelper.GetNow();
            var resList = new List<CustomObject>();
            for (int i = 0; i < d_day; i++)
            {
                DateTime date = current.AddDays(-i);           
                IEnumerable<CustomObject> query = dataContext.ExecuteQuery<CustomObject>(
                        @"
                            SELECT count(userid) as Count, 
                                    COUNT(distinct userid) as Count2
                            FROM [bank]
                            where optype = " + broadType + @"
                            and DAY(createtime) = "+ date.Day + @"
                            and Month(createtime) = " + date.Month + @"
                            and Year(createtime) = " + date.Year + @"
                        "
                    );
                var res = query.FirstOrDefault();
                res.Name = date.ToShortDateString();
                resList.Add(res);
            }

            ViewData.Model = resList;

            return View("V12WorldCallUsedTimes");
        }

        [Description("产品数据分析--世界喊话功能使用次数下载")]
        /// <summary>
        /// 世界喊话功能使用次数
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV12WorldCallUsedTimesAction()
        {
            string conent = GenerateV12WorldCallUsedTimesExcel();

            ExportToFile.DownLoadAsExcel("世界喊话功能使用次数 ", conent);

            return RedirectToAction("V12WorldCallUsedTimes");
        }

        [Description("产品数据分析--今日对局数和对局时间查询")]
          /// <summary>
        /// 今日对局数和对局时间
        /// </summary>        
        /// <returns></returns>
        public ActionResult V23TodayRoundTimesAndDateAction(string tbDate)
        {
            DateTime current = DateHelper.GetNow();
            if (!DateTime.TryParse(tbDate, out current))
            {
                current = DateHelper.GetNow();
            }
            DateTime pdate = current;
            int gameTaxType = (int)BankActionType.GameTax;
            int sysTaxType = (int)BankActionType.GameTaxSystem;
            var playerCreates = (from m in dataContext.banks
                              where m.optype == gameTaxType
                              && m.createtime.Value.Year == pdate.Year
                              && m.createtime.Value.Month == pdate.Month
                              && m.createtime.Value.Day == pdate.Day
                              select m.bankid).Count();
            var systemCreates = (from m in dataContext.banks
                                 where m.optype == sysTaxType
                                 && m.createtime.Value.Year == pdate.Year
                                 && m.createtime.Value.Month == pdate.Month
                                 && m.createtime.Value.Day == pdate.Day
                                 select m.bankid).Count();
            var playerTime = (from m in dataContext.banks
                              where m.optype == gameTaxType
                              && m.createtime.Value.Year == pdate.Year
                              && m.createtime.Value.Month == pdate.Month
                              && m.createtime.Value.Day == pdate.Day
                              select m.duration).Sum();
            var systemTime = (from m in dataContext.banks
                              where m.optype == sysTaxType
                              && m.createtime.Value.Year == pdate.Year
                              && m.createtime.Value.Month == pdate.Month
                              && m.createtime.Value.Day == pdate.Day
                              select m.duration).Sum();

            ViewData["tbPlayerCreates"] = playerCreates;
            ViewData["tbSystemCreates"] = systemCreates;
            if (playerCreates == 0)
                ViewData["tbPlayerTime"] = 0;
            else
                ViewData["tbPlayerTime"] = Math.Round((double)(playerTime / playerCreates), 2);
            if (systemCreates == 0)
                ViewData["tbSystemTime"] = 0;
            else
                ViewData["tbSystemTime"] = Math.Round((double)(systemTime / systemCreates), 2);

            return View("V23TodayRoundTimesAndDate");
        }

        [Description("产品数据分析--今日对局数和对局时间下载")]
        /// <summary>
        /// 今日对局数和对局时间
        /// </summary>        
        /// <returns></returns>
        public ActionResult DownloadV23TodayRoundTimesAndDateAction(string tbDate)
        {
            string conent = GenerateV23TodayRoundTimesAndDateExcel(tbDate);

            ExportToFile.DownLoadAsExcel("今日对局数和对局时间 ", conent);

            return RedirectToAction("V23TodayRoundTimesAndDate");
        }

        #endregion

        #region Generate Excel Report

        private string GenerateV1RegistChipsExcel()
        {
            int regType = (int)BankActionType.Register;
            var userList = (from m in dataContext.users
                           from b in dataContext.banks
                           where m.userid == b.userid
                           && b.optype == regType                           
                           select new
                           {
                               m.userid,
                               m.mail,
                               m.nickname,                               
                               b.createtime,
                               m.devicetype,
                               m.usertype,
                               b.bankout,                                                            
                           });
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>user-id</th><th>首次登陆时间</th><th>账户注册时间</th><th>第一次登陆的设备类型</th><th>目前账户类型</th><th>登陆奖励的筹码数</th></tr>");
            foreach (var item in userList)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.mail + "</td>");
                //sb.Append("<td>" + item.nickname ?? string.Empty + "</td>");
                sb.Append("<td>" + item.createtime ?? string.Empty + "</td>");
                sb.Append("<td>" + item.createtime ?? string.Empty + "</td>");
                sb.Append("<td>" + Enum.GetName(typeof(DeviceType), item.devicetype ?? 0) + "</td>");
                sb.Append("<td>" +  Enum.GetName(typeof(UserType), item.usertype ?? 0) + "</td>");
                sb.Append("<td>" + item.bankout ?? string.Empty + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV2EveryChipsExcel()
        {
            int regType = (int)BankActionType.Register;
            int awardType = (int)BankActionType.Award;

            IEnumerable<V2EveryChipsObject> query = dataContext.ExecuteQuery<V2EveryChipsObject>(
                    @"
                        select
                        users.userid,
                        users.mail, 
                        users.nickname, 
                        users.devicetype,
                        users.usertype, 
                        t2.everyDayAward,
                        t3.createtime
                        from [user] users
                        left join
	                        (SELECT SUM(bankout) everyDayAward, userid
	                          FROM [bank]
	                          where optype = " + awardType + @"
	                          and userid is not null
	                          group by userid) t2
	                         on users.userid = t2.userid
                        left join 
	                        (SELECT createtime, userid
		                          FROM [bank]
		                          where optype = " + regType + @" 
		                          and userid is not null ) t3
                            on users.userid = t3.userid
                    "
                );

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>user-id</th><th>首次登陆时间</th><th>账户注册时间</th><th>第一次登陆的设备类型</th><th>目前账户类型</th><th>收到的每日赠送筹码总量</th></tr>");
            foreach (var item in query)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.mail + "</td>");
                //sb.Append("<td>" + item.nickname ?? string.Empty + "</td>");
                sb.Append("<td>" + item.createtime ?? string.Empty + "</td>");
                sb.Append("<td>" + item.createtime ?? string.Empty + "</td>");
                sb.Append("<td>" + Enum.GetName(typeof(DeviceType), item.devicetype ?? 0) + "</td>");
                sb.Append("<td>" + Enum.GetName(typeof(UserType), item.usertype ?? 0) + "</td>");
                sb.Append("<td>" + item.everyDayAward ?? string.Empty + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 用户购买的筹码数量
        /// </summary>
        /// <returns></returns>
        private string GenerateV3UserBuyChipsExcel()
        {
            var today = DateHelper.GetNow();
            int buyChipsType = (int)ItemType.Chip;
            // 今日购买筹码人数
            var buyUserChipsNum = (from m in dataContext.userpayments
                                   where m.type == buyChipsType  // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                                   select m.userid).Distinct().Count();
            ViewData["tbBuyChipsUserNum"] = buyUserChipsNum;

            // 今日用户购买的筹码数量
            var buyChipsNum = (from m in dataContext.userpayments
                               where m.type == buyChipsType   // 3 = BuyChips
                               && m.time.Value.Year == today.Year
                               && m.time.Value.Month == today.Month
                               && m.time.Value.Day == today.Day
                               select m.itemid).Sum();
            ViewData["tbBuyChipsNum"] = CodeHelper.GetNumberOrZero(buyChipsNum);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>时间</th><th>今日购买筹码人数</th><th>今日用户购买的筹码数量</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + DateTime.Today.ToShortDateString() + "</td>");
            sb.Append("<td>" + CodeHelper.GetNumberOrZero(buyUserChipsNum) + "</td>");
            sb.Append("<td>" + CodeHelper.GetNumberOrZero(buyChipsNum) + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 用户建房筹码数 
        /// </summary>
        /// <returns></returns>
        private string GenerateV5CreateRoomChipsExcel()
        {
            var today = DateHelper.GetNow();
            int createGameType = (int)BankActionType.CreatGame;
            // 今日建房次数            
            var createRoomTimes = (from m in dataContext.banks
                                   from u in dataContext.users
                                   where m.optype == createGameType   // 102 = CreateGame
                                   && m.createtime.Value.Year == today.Year
                                   && m.createtime.Value.Month == today.Month
                                   && m.createtime.Value.Day == today.Day
                                   select new { 
                                    u.userid,
                                    u.mail,
                                    u.nickname,
                                    m.createtime,
                                    m.bankin
                                   });

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>建房人user-id</th><th>建房时间</th><th>建房扣除的费用</th></tr>");
            foreach (var item in createRoomTimes)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                sb.Append("<td>" + item.createtime.Value.ToShortDateString() + "</td>");
                sb.Append("<td>" + CodeHelper.GetNumberOrZero(item.bankin) + "</td>");
                sb.Append("</tr>");
            }            
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 牌桌税收 
        /// </summary>
        /// <returns></returns>
        private string GenerateV6TableTaxExcel()
        {
            // can not get now
            var today = DateHelper.GetNow();
            int gameTaxType = (int)BankActionType.GameTax;
            // 今日对局次数    
            var roundTimes = (from m in dataContext.banks
                              where m.optype == gameTaxType
                              && m.createtime.Value.Year == today.Year
                              && m.createtime.Value.Month == today.Month
                              && m.createtime.Value.Day == today.Day
                              select m.bankid).Count();

            // 今日牌桌回收的筹码数
            var recvTableChips = (from m in dataContext.banks
                                  where m.optype == gameTaxType
                                  && m.createtime.Value.Year == today.Year
                                  && m.createtime.Value.Month == today.Month
                                  && m.createtime.Value.Day == today.Day
                                  select m.bankin).Sum();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>日期</th><th>对局次数</th><th>牌桌回收的筹码数</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + DateHelper.GetNow().ToShortDateString() + "</td>");
            sb.Append("<td>" + roundTimes + "</td>");
            sb.Append("<td>" + CodeHelper.GetNumberOrZero(recvTableChips) + "</td>");
            sb.Append("</tr>");
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 玩家购买筹码花费 
        /// </summary>
        /// <returns></returns>
        private string GenerateV7CostOfPalyerBuyChipsExcel()
        {
            // can not get now
            var today = DateHelper.GetNow();
            int buyChipsType = (int)ItemType.Chip;
            DateTime current = DateHelper.GetNow();

            // 今日购买筹码的人数
            var userNumer = (from u in dataContext.users
                             from p in dataContext.userpayments
                             where u.userid == p.userid // BuyChips
                             && p.type == buyChipsType                 
                             select new { 
                                u.userid,
                                u.mail,
                                u.nickname,
                                p.time,
                                p.money
                             });
            ViewData["tbUserNum"] = userNumer;

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>玩家user-id</th><th>玩家购买筹码的时间</th><th>玩家购买筹码的花费(RMB)</th></tr>");
            foreach (var item in userNumer)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.userid + "</td>");
                //sb.Append("<td>" + item.mail + "</td>");
                //sb.Append("<td>" + item.nickname + "</td>");
                sb.Append("<td>" + item.time.Value.ToShortDateString() + "</td>");
                sb.Append("<td>" + CodeHelper.GetNumberOrZero(item.money) + "</td>");
                sb.Append("</tr>");
            }           
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 各类礼物被购买次数 
        /// </summary>
        /// <returns></returns>
        private string GenerateV8GiftsBuyTimesExcel()
        {
            int buyGiftType = (int)BankActionType.BuyGift;

            // 各类礼物被购买次数
            var giftBuyTimes = (from m in dataContext.banks
                                where m.optype == buyGiftType
                                group m by m.itemid into tgroup
                                orderby tgroup.Key
                                select new CustomObject
                                {
                                    Count = tgroup.Count(),
                                    Name = LilyGiftHelper.GetGiftNameById(tgroup.Key)
                                });

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>礼物名称</th><th>出售数量</th></tr>");
            foreach (var item in giftBuyTimes)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + item.Name + "</td>");
                sb.Append("<td>" + item.Count + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV9SceneUsedTimesExcel()
        {
            var room = (from m in dataContext.users
                        group m by m.livingroomtype into roomgroup
                        select new
                        {
                            roomtype = roomgroup.Key,
                            count = roomgroup.Count()                            
                        }).Distinct();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>场景名称</th><th>使用人数</th></tr>");
            int cnt = 0;
            int[] types = { 0, 1, 2, 4, 8, 16, 32 };
            List<int> use_type = new List<int>();
            foreach (var item in room)
            {
                bool isContinue = true;
                for (int i = 0; i < types.Length; i++) {
                    if (item.roomtype == types[i]) {
                        isContinue = false;
                        break;
                    }
                }

                if (isContinue) continue;
                
                use_type.Add(item.roomtype); // means already use
                string name = LilyRoomHelper.GetRoomTypeName(item.roomtype);
                int count = item.count;                
                if (item.roomtype <= 1) cnt += item.count;
                if (item.roomtype == 1) count = cnt;
                if (item.roomtype > 0)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + name + "</td>");
                    sb.Append("<td>" + count + "</td>");
                    sb.Append("</tr>");
                }
            }
            int tmp_type = 1;
            for (int i = 1; i < types.Length; i++){
                // not used yet                
                if (!use_type.Contains(types[i]))
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + LilyRoomHelper.GetRoomTypeName(tmp_type) + "</td>");
                    sb.Append("<td>0</td>");
                    sb.Append("</tr>");
                }
                tmp_type = tmp_type * 2;
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        private string GenerateV10RoleUsedTimesExcel()
        {
            var roles = (from m in dataContext.users
                         group m by m.avator into rolesgroup
                         select new
                         {
                             count = rolesgroup.Count(),
                             roletype = rolesgroup.Key
                         }).Distinct();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>角色</th><th>使用人数</th></tr>");
            foreach (var item in roles)
            {
                sb.Append("<tr>");
                sb.Append("<td>" + LilyAvatorHelper.GetAvatorName(item.roletype) + "</td>");
                sb.Append("<td>" + item.count + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        /// <summary>
        /// 世界喊话功能使用次数 
        /// </summary>
        /// <returns></returns>
        private string GenerateV12WorldCallUsedTimesExcel()
        {
            int d_day = 7;

            int broadType = (int)BankActionType.Broadcast;
            DateTime current = DateHelper.GetNow();
            var resList = new List<CustomObject>();
            for (int i = 0; i < d_day; i++)
            {
                DateTime date = current.AddDays(-i);
                IEnumerable<CustomObject> query = dataContext.ExecuteQuery<CustomObject>(
                        @"
                            SELECT count(userid) as Count, 
                                    COUNT(distinct userid) as Count2
                            FROM [Lily].[dbo].[bank]
                            where optype = " + broadType + @"
                            and DAY(createtime) = " + date.Day + @"
                            and Month(createtime) = " + date.Month + @"
                            and Year(createtime) = " + date.Year + @"
                        "
                    );
                var res = query.FirstOrDefault();
                res.Name = date.ToShortDateString();
                resList.Add(res);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>时间</th><th>喊话次数</th><th>使用人数</th></tr>");
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

        /// <summary>
        /// 今日对局数和对局时间 
        /// </summary>
        /// <returns></returns>
        private string GenerateV23TodayRoundTimesAndDateExcel(string tbDate)
        {
            int d_day = 7;
            DateTime current = DateHelper.GetNow();
            if (!DateTime.TryParse(tbDate, out current))
            {
                current = DateHelper.GetNow();
            }
            int gameTaxType = (int)BankActionType.GameTax;
            int sysTaxType = (int)BankActionType.GameTaxSystem;
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table>
                <tr><th>牌桌ID</th><th>牌局创建时间</th><th>牌局类型</th><th>牌局创建人</th><th>对局开始时间</th><th>对局结束时间</th><th>对局持续时间</th></tr>");
            sb.Append("<tr>");
            for (int i = 0; i < d_day; i++)
            {
                DateTime pdate = current.AddDays(-i);
                var everyDayResult = (from m in dataContext.banks
                                     where (m.optype == gameTaxType
                                     || m.optype == sysTaxType)
                                     && m.createtime.Value.Year == pdate.Year
                                     && m.createtime.Value.Month == pdate.Month
                                     && m.createtime.Value.Day == pdate.Day
                                     select m);
                             
                foreach (var item in everyDayResult)
                {
                    sb.Append("<td>" + item.bankid + "</td>");
                    sb.Append("<td>" + item.createtime.Value.ToShortDateString() + "</td>");
                    if (item.optype == gameTaxType)
                    {
                        sb.Append("<td>用户</td>");
                        sb.Append("<td>" + item.userid + "</td>");
                    }
                    else
                    {
                        sb.Append("<td>系统</td>");
                        sb.Append("<td>系统</td>");
                    }
                    sb.Append("<td>" + item.createtime.Value.ToLongTimeString() + "</td>");
                    double tduration = 0;
                    if (item.duration != null) tduration = (double)item.duration;
                    sb.Append("<td>" + item.createtime.Value.AddSeconds(tduration).ToLongTimeString() + "</td>");
                    sb.Append("<td>" + item.duration + "</td>");
                    sb.Append("</tr>");                
                }               
            }
            sb.Append("</table>");

            return sb.ToString();
        }

        #endregion

        #region For Chart

        [Description("产品数据分析--各类礼物被购买次数图表")]
        public string GetV8GiftsBuyTimesActionChart()
        {
            int buyGiftType = (int)BankActionType.BuyGift;

            // 各类礼物被购买次数
            var myList = (from m in dataContext.banks
                                where m.optype == buyGiftType
                                group m by m.itemid into tgroup
                                orderby tgroup.Key
                                select new CustomObject
                                {
                                    Count = tgroup.Count(),
                                    Name = LilyGiftHelper.GetGiftNameById(tgroup.Key)
                                });

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();
            chart.Title = new Title("各类礼物被购买次数");
            chart.X_Legend = new Legend("礼物名称");
            chart.Y_Legend = new Legend("购买次数");

            Bar bar = new OpenFlashChart.Bar();
            bar.Colour = "#344565";
            //bar.Alpha = 0.8;
            bar.Text = "礼物名称";
            bar.FontSize = 10;

            List<object> values = new List<object>();
            XAxis xaxis = new XAxis();
            int max_limit = 0;
            foreach (var item in myList)
            {
                values.Add(item.Count);
                AxisLabel myLabel = new AxisLabel(item.Name);
                xaxis.Labels.Add(myLabel);               
                if (item.Count > max_limit) max_limit = item.Count;
            }
            bar.Values = values;

            chart.X_Axis = xaxis;
            chart.X_Axis.Labels.Vertical = true;
            chart.AddElement(bar);
            bar.Tooltip = "购买次数：#val#<br>礼物名称: #x_label#";
            string s = chart.ToPrettyString();

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }

        [Description("产品数据分析--角色使用人数图表")]
        public string GetV10RoleUsedTimesActionChart()
        {
            var roles = (from m in dataContext.users
                         group m by m.avator into rolesgroup
                         select new
                         {
                             count = rolesgroup.Count(),
                             roletype = rolesgroup.Key
                         });
            List<string> roleName = new List<string>();
            roleName.Add("默认");
            roleName.Add("石油大亨");
            roleName.Add("饶舌歌星");
            roleName.Add("海贼王");
            roleName.Add("欧洲王储");
            roleName.Add("意大利黑手党千金");
            roleName.Add("俄罗斯富商大老婆");
            roleName.Add("宋朝官员的姨太太");
            roleName.Add("有钱的小萝莉");           

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();
            chart.Title = new Title("角色使用人数");
            chart.X_Legend = new Legend("角色");
            chart.Y_Legend = new Legend("使用人数");

            Bar bar = new OpenFlashChart.Bar();
            bar.Colour = "#344565";
            //bar.Alpha = 0.8;
            bar.Text = "角色名称";
            bar.FontSize = 10;

            List<int> values = new List<int>();
            XAxis xaxis = new XAxis();
            foreach (var item in roleName)
            {
                AxisLabel myLabel = new AxisLabel(item);
                xaxis.Labels.Add(myLabel);
                values.Add(0);
            }            
            
            int max_limit = 0;
            foreach (var item in roles)
            {
                if (item.roletype != null && item.roletype >= roleName.Count) continue;

                values[item.roletype ?? 0] = item.count;
                if (item.count > max_limit) max_limit = item.count;
            }
            bar.Values = values;

            chart.X_Axis = xaxis;
            chart.X_Axis.Labels.Vertical = true;
            chart.AddElement(bar);
            bar.Tooltip = "使用人数：#val#<br>角色名称: #x_label#";
            string s = chart.ToPrettyString();

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }

        [Description("产品数据分析--场景使用人数图表")]
        public string GetV9SceneUsedTimesActionChart()
        {
            var room = (from m in dataContext.users
                        group m by m.livingroomtype into roomgroup
                        select new
                        {
                            count = roomgroup.Count(),
                            roomtype = roomgroup.Key
                        });
            Dictionary<int, string> roomName = new Dictionary<int, string>();
            roomName.Add(1, "高尚富人小区(默认)");
            roomName.Add(2, "埃及法老墓室");
            roomName.Add(4, "夏威夷风情");
            roomName.Add(8, "日本情调桃源");
            roomName.Add(16, "西部牛仔酒馆");
            roomName.Add(32, "古典浪漫庭院");

            Dictionary<int, int> roomNumber = new Dictionary<int, int>();
            roomNumber.Add(1, 0);
            roomNumber.Add(2, 0);
            roomNumber.Add(4, 0);
            roomNumber.Add(8, 0);
            roomNumber.Add(16, 0);
            roomNumber.Add(32, 0);
         
            foreach (var item in room)
            {
                if (item.roomtype <= 1) roomNumber[1] += item.count;
                else
                    roomNumber[item.roomtype] = item.count;
            }

            OpenFlashChart.OpenFlashChart chart = new OpenFlashChart.OpenFlashChart();
            chart.Title = new Title("场景使用人数 ");
            chart.X_Legend = new Legend("场景");
            chart.Y_Legend = new Legend("使用人数");

            Bar bar = new OpenFlashChart.Bar();
            bar.Colour = "#344565";
            //bar.Alpha = 0.8;
            bar.Text = "场景名称";
            bar.FontSize = 10;

            List<int> values = new List<int>();
            XAxis xaxis = new XAxis();
            int max_limit = 0;
            foreach (var item in roomName)
            {
                AxisLabel myLabel = new AxisLabel(item.Value);
                xaxis.Labels.Add(myLabel);
                values.Add(roomNumber[item.Key]);
                if (roomNumber[item.Key] > max_limit) max_limit = roomNumber[item.Key];
            }
            bar.Values = values;

            chart.X_Axis = xaxis;
            chart.X_Axis.Labels.Vertical = true;
            chart.AddElement(bar);
            bar.Tooltip = "使用人数：#val#<br>场景名称: #x_label#";
            string s = chart.ToPrettyString();

            chart.Y_Axis.SetRange(0, max_limit + 1, (int)(max_limit / 5));

            return chart.ToPrettyString();
        }

        #endregion
    }
}
