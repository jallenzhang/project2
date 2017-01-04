using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Actions;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles = "Administrator, ProductRole, OperationRole, MarketRole")]
    
    [Description("数据分析(底下权限可以继承当前权限)")]
    public class DataManagerController : BaseController
    {
        [Description("数据分析--首页")]
        public ActionResult Index(string parterid)
        {
            return View();
        }
    }
}
