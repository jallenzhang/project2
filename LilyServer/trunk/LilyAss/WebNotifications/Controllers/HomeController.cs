using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Security;

namespace WebNotifications.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("/Account/Login");
            else
                Response.Redirect("/DataManager");
            return View();
        }
    }
}
