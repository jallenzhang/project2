using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    [Description("管理员界面管理(底下权限可以继承当前权限)")]
    public class AdminManagerController : BaseController
    {
        [Description("管理员界面管理--显示界面")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
