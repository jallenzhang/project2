using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using System.ComponentModel;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Models.Custom;
using DataPersist;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    [Description("Photon(底下权限可以继承当前权限)")]
    public class PhotonController : BaseController
    {
        [Description("Photon--监视页面")]
        public ActionResult Monitor()
        {
            string phontonUrl = ConfigurationManager.AppSettings["PhotonWebUrl"];
            ViewBag.Message = phontonUrl;
            return View();
        }
    }
}
