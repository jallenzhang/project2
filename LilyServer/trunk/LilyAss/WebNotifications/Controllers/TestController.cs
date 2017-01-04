using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

using WebNotifications.Helper;
using OpenFlashChart;

namespace WebNotifications.Controllers
{
    [HandleError]
    public class TestController : Controller
    {
        //
        // GET: /Test/
        public ActionResult Index()
        {
            return View();
        }
    }
}
