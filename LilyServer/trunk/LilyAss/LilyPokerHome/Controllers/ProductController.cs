using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Objects;
using LilyPokerHome.Models;

namespace LilyPokerHome.Controllers
{
    public class ProductController : Controller
    {
        private LilyModelDataContext myContext = new LilyModelDataContext();

        private static int iosId = 10;
        private static int apkId = 11;
        //
        // GET: /Product/

        public ActionResult Poker()
        {
            SetDownloadUrl();
            
            return View();
        }

        private void SetDownloadUrl() { 
            var res = from m in myContext.configSystems
                      where m.id == iosId || m.id == apkId
                      select m;            
            foreach(var item in res){
                if (item.id == iosId) {
                    ViewData["tbAppStoreUrl"] = item.valuestr;                    
                }
                else if (item.id == apkId) {
                    ViewData["tbAndroidUrl"] = item.valuestr;                
                }
            }
        }
    }
}
