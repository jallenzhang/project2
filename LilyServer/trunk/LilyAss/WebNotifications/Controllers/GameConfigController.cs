using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;
using System.Text;
using System.ComponentModel;

using WebNotifications.Models;
using WebNotifications.Helper;
using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Description("游戏配置(底下权限可以继承当前权限)")]
    public class GameConfigController : BaseController
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();        

        #region Bot Config

        [Description("游戏配置--机器人配置首页")]
        public ActionResult BotConfigIndex(int? page)
        {
            var resList = (from m in dataContext.configRobotStrategies
                           select m);
            var pageList = resList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("游戏配置--机器人配置修改页面")]
        public ActionResult BotConfigModify(int id)
        {
            var obj = (from m in dataContext.configRobotStrategies
                       where m.id == id
                       select m).FirstOrDefault();
            ViewData.Model = obj;

            return View();
        }

        [Description("游戏配置--机器人配置修改页面")]
        [HttpPost]
        public ActionResult BotConfigModify(configRobotStrategy botconfig)
        {
            if (ModelState.IsValid)
            {
                // check
                if (!CheckBotConfigIsValid(botconfig))
                {
                    ModelState.AddModelError("", "填写的参数不符合规范.eg:盖牌率+叫牌率<100， 延迟应该大于0,小于10.");
                }
                else
                {
                    using (DataLilyModelDataContext _ctx = new DataLilyModelDataContext())
                    {
                        _ctx.configRobotStrategies.Attach(botconfig);
                        _ctx.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, botconfig);
                        _ctx.SubmitChanges();
                    }
                    
                    return RedirectToAction("BotConfigIndex");
                }
            }

            return View(botconfig);
        }

        //[Description("游戏配置--机器人配置删除操作")]
        //[HttpPost]
        //public ActionResult BotDeleteModifyAction(configRobotStrategy botconfig)
        //{
        //    int id = botconfig.id;
        //    var obj = (from m in dataContext.configRobotStrategies
        //               where m.id == id
        //               select m).FirstOrDefault();
        //    dataContext.configRobotStrategies.DeleteOnSubmit(obj);
        //    dataContext.SubmitChanges();

        //    return RedirectToAction("BotConfigIndex");
        //}

        //public ActionResult BotConfigAdd(string id)
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult BotConfigAdd(configRobotStrategy botconfig)
        //{
        //    // check
        //    if (!CheckBotConfigIsValid(botconfig))
        //    {
        //        ModelState.AddModelError("", "填写的参数不符合规范.eg:盖牌率+叫牌率<100， 延迟应该大于0,小于10.");
        //        return View(botconfig);
        //    }

        //    dataContext.configRobotStrategies.InsertOnSubmit(botconfig);
        //    dataContext.SubmitChanges();
        //    return RedirectToAction("BotConfigIndex");
        //}

        [Description("游戏配置--机器人配置修改取消操作")]
        public ActionResult BotConfigCancleAction(string id)
        {
            return RedirectToAction("BotConfigIndex");
        }

        [Description("游戏配置--机器人配置导出操作")]
        public ActionResult BotConfigExportAction()
        {
            var resList = (from m in dataContext.configRobotStrategies
                           select m);
            StringBuilder sb = new StringBuilder();
            foreach (var res in resList)
            {
                sb.Append("strategyid:" + res.strategyid + "\r\n");
                sb.Append("strategy:" + res.strategy + "\r\n");
                sb.Append("typeround:" + res.typeround + "\r\n");
                sb.Append("foldratio:" + res.foldratio + "\r\n");
                sb.Append("callratio:" + res.callratio + "\r\n");
                sb.Append("delaymin:" + res.delaymin + "\r\n");
                sb.Append("delaymax:" + res.delaymax + "\r\n");
                sb.Append("rasiea:" + res.rasiea + "\r\n");
                sb.Append("rasieb:" + res.rasieb + "\r\n");
                sb.Append("# ########################\r\n");
            }

            ExportToFile.DownLoadAsConfig("机器人配置", sb.ToString());

            return RedirectToAction("BotConfigIndex");
        }

        //public ActionResult BotConfigImportAction(HttpPostedFileBase uploadBotConfig)
        //{
        //    if (uploadBotConfig == null)
        //    {
        //        ViewData["InOut"] = "没有文件";
        //    }
        //    else
        //    {
        //        ApplyImportAction(uploadBotConfig.InputStream);
        //    }

        //    return RedirectToAction("BotConfigIndex");
        //}

        #endregion

        #region System Config

        [Description("游戏配置--系统配置首页")]
        public ActionResult SystemConfigIndex(int? page)
        {
            var resList = (from m in dataContext.configSystems
                           select m);

            var pageList = resList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("游戏配置--系统配置修改页面")]
        public ActionResult SystemConfigModify(int id)
        {
            var obj = (from m in dataContext.configSystems
                       where m.id == id
                       select m).FirstOrDefault();
            ViewData.Model = obj;

            return View();
        }

        [Description("游戏配置--系统配置修改页面")]
        [HttpPost]
        public ActionResult SystemConfigModify(configSystem sysconfig)
        {
            if (ModelState.IsValid)
            {                
                //sysconfig.updatetime = DateTime.Now;
                //sysconfig.updateby = User.Identity.Name;
                //dataContext.configSystems.Attach(sysconfig);
                //dataContext.Refresh(System.Data.Linq.RefreshMode.KeepCurrentValues, sysconfig);

                //newsysconfig.updatetime = DateTime.Now;

                string executeSql = @"UPDATE [dbo].[configSystem] SET value = {0}, valuestr ={1}, updateby = {2}, description={3}, updatetime={4} WHERE ID = {5}";
                sysconfig.description = sysconfig.description ?? string.Empty;
                sysconfig.valuestr = sysconfig.valuestr ?? string.Empty;                

                dataContext.ExecuteCommand(executeSql, sysconfig.value, sysconfig.valuestr, User.Identity.Name, sysconfig.description, DateHelper.GetNow(), sysconfig.id);

                // dataContext.SubmitChanges();
                
                return RedirectToAction("SystemConfigIndex");
            }

            return View(sysconfig);
        }

        //[Description("游戏配置--系统配置删除页面")]
        //[HttpPost]
        //public ActionResult SystemDeleteModifyAction(configSystem sysconfig)
        //{
        //    int id = sysconfig.id;
        //    var obj = (from m in dataContext.configSystems
        //               where m.id == id
        //               select m).FirstOrDefault();
        //    dataContext.configSystems.DeleteOnSubmit(obj);
        //    dataContext.SubmitChanges();

        //    return RedirectToAction("SystemConfigIndex");
        //}

        [Description("游戏配置--系统配置添加页面")]
        public ActionResult SystemConfigAdd(string id)
        {
            return View();
        }

        [Description("游戏配置--系统配置添加页面")]
        [HttpPost]
        public ActionResult SystemConfigAdd(configSystem sysconfig)
        {
            int id = (from m in dataContext.configSystems
                      select m).Count();
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext()){
                sysconfig.description = sysconfig.description;
                sysconfig.updatetime = DateHelper.GetNow();
                sysconfig.updateby = User.Identity.Name;
                mydataContext.configSystems.InsertOnSubmit(sysconfig);
                mydataContext.SubmitChanges();            
            }
            
            return RedirectToAction("SystemConfigIndex");
        }

        [Description("游戏配置--系统配置取消页面")]
        public ActionResult SystemConfigCancleAction(string id)
        {
            return RedirectToAction("SystemConfigIndex");
        }

        #endregion

        #region Private Method

        //private void ApplyImportAction(Stream upstream)
        //{
        //    var importList = AnalyzeBotConfig(upstream);
        //    if (importList.Count > 0)
        //    {
        //        // clear the ActionPermission table
        //        dataContext.ExecuteCommand("TRUNCATE TABLE ConfigRobotStrategy");
        //        dataContext.configRobotStrategies.InsertAllOnSubmit(importList);
        //        dataContext.SubmitChanges();
        //    }
        //}

        //private List<configRobotStrategy> AnalyzeBotConfig(Stream upstream)
        //{
        //    var objList = new List<configRobotStrategy>();
        //    StreamReader fileStream = new StreamReader(upstream);
        //    configRobotStrategy obj = new configRobotStrategy();
        //    while(true)
        //    {
        //        string line = fileStream.ReadLine();
        //        if(string.IsNullOrEmpty(line)) break;
        //        if (line.StartsWith(@"#")) 
        //        {
        //            if(!string.IsNullOrEmpty(obj.strategy))
        //                objList.Add(obj);
        //            obj = new configRobotStrategy();
        //            continue;
        //        }
        //        var res = AnalyzeLine(line);
        //        if(res != null)
        //            AppendFieldToObj(obj, res.Item1, res.Item2);
        //    }

        //    return objList;
        //}
        
        //private Tuple<string, string> AnalyzeLine(string line)
        //{            
        //    string[] res = line.Split(new char[]{':'});
        //    if(res.Length == 2)
        //        return new Tuple<string,string>(res[0], res[1]);
        //    return null;
        //}

        //private void AppendFieldToObj(configRobotStrategy obj, string field, string value)
        //{
        //    if (field == "strategyid")
        //        obj.strategyid = StringToInt(value);
        //    else if (field == "strategy")
        //        obj.strategy = value;
        //    else if (field == "typeround")
        //        obj.typeround = StringToInt(value);
        //    else if (field == "foldratio")
        //        obj.foldratio = StringToInt(value);
        //    else if (field == "callratio")
        //        obj.callratio = StringToInt(value);
        //    else if (field == "delaymin")
        //        obj.delaymin = StringToInt(value);
        //    else if (field == "delaymax")
        //        obj.delaymax = StringToInt(value);
        //    else if (field == "rasiea")
        //        obj.rasiea = StringToInt(value);
        //    else if (field == "rasieb")
        //        obj.rasieb = StringToInt(value);
        //}

        private int StringToInt(string input)
        {
            int res = 0;
            int.TryParse(input, out res);
            return res;
        }

        private int StringToInt(object p)
        {
            int res = 0;
            int.TryParse(p.ToString(), out res);
            return res;
        }

        private bool CheckBotConfigIsValid(configRobotStrategy obj)
        {
            if (string.IsNullOrEmpty(obj.strategy)) return false;
            if (obj.typeround < 0 || obj.typeround > 4) return false;      
            //if (obj.foldratio == 0) return false;
            //if (obj.callratio == 0) return false;
            if (obj.delaymax == null || obj.delaymax==0) return false;
            if (obj.delaymin == null || obj.delaymin==0) return false;

            if (obj.delaymin <= 0 || obj.delaymin > 10) return false;
            if (obj.delaymax <= 0 || obj.delaymax > 10) return false;
            if (obj.delaymax < obj.delaymin) return false;
            if (obj.foldratio + obj.callratio >= 100) return false;

            return true;
        }

        #endregion
    }
}
