using System;
using System.Web.Mvc;
using LilyPokerHome.Helper;

/// <summary>
/// 权限拦截
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext == null)
        {
            throw new ArgumentNullException("filterContext");
        }

        if (!LoginHelper.ValidateUserInSession()) {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}