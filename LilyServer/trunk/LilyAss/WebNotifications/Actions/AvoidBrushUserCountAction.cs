using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebNotifications.Models;
using WebNotifications.Helper;

namespace WebNotifications.Actions
{
    public class AvoidBrushUserCountAction: FilterAttribute, IActionFilter
    {
        DataJackModelDataContext _ctx = new DataJackModelDataContext();

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var parameters = filterContext.ActionDescriptor.GetParameters();
            string serizeid = filterContext.ActionParameters[parameters[0].ParameterName] as string;
            ChangeParterCount(filterContext, serizeid);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        private void ChangeParterCount(ActionExecutingContext filterContext, string serizeid)
        {
            var parter = AvoidBrushCountHelper.GetParter(serizeid);
            if (parter != null)
            {
                if (ChangeParterCountCondition(filterContext))
                {
                    parter.count++;
                    _ctx.SubmitChanges();
                }
            }
        }

        private bool ChangeParterCountCondition(ActionExecutingContext filterContext)
        {
            string ip = filterContext.HttpContext.Request.UserHostAddress;
            DateTime? date = AvoidBrushCountHelper.GetLashSubmitTime(ip);
            if (date != null)
            {
                DateTime current = DateTime.Now;
                if (date.Value.AddSeconds(AvoidBrushCountHelper.Seconds) > current)
                    return false;
            }

            AvoidBrushCountHelper.RereshIP(ip);

            return true;
        }
    }
}