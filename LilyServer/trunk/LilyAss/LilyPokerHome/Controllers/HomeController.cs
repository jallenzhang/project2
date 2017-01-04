using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LilyPokerHome.Models;

namespace LilyPokerHome.Controllers
{
    public class HomeController : Controller
    {
        private LilyModelDataContext dataContext = new LilyModelDataContext();

        public ActionResult Index()
        {
            ViewData["tbAppStoreUrl"] = GetAppStoreUrl();
            ViewData["tbAndroidUrl"] = GetAndroidUrl();

            return View();
        }

        private string GetAppStoreUrl() {
            int appId = 10;
            var res = (from m in dataContext.configSystems
                      where m.id == appId
                      select m.valuestr).FirstOrDefault();

            return res;
        }

        private string GetAndroidUrl()
        {
            int android = 11;
            var res = (from m in dataContext.configSystems
                       where m.id == android
                       select m.valuestr).FirstOrDefault();

            return res;
        }
    }
}
