using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebNotifications.Permission
{
    [AuthorizeFilter]
    public class BaseController : Controller
    {
    }
}