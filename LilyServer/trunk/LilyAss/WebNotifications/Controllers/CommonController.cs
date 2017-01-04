using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using WebNotifications.Models;
using Webdiyer.WebControls.Mvc;

namespace WebNotifications.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        public ActionResult Error404()
        {
            return View();
        }
    }
}
