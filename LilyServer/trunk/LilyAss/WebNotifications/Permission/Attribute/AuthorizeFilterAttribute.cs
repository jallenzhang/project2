using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using WebNotifications.Models;

namespace WebNotifications.Permission
{
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
            var path = filterContext.HttpContext.Request.Path.ToLower();
            if (path == "/" || path == "/Account/Login".ToLower())
                return;//忽略对Login登录页的权限判定

            if (this.AuthorizeCore(filterContext) == false)//根据验证判断进行处理
            {
                filterContext.Result = new HttpUnauthorizedResult();//直接URL输入的页面地址跳转到登陆页
            }
        }

        //权限判断业务逻辑
        protected virtual bool AuthorizeCore(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return false;//判定用户是否登录
            }

            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();

            if (!CheckRoles(filterContext, actionName, controllerName)) return false;

            return true;
        }

        private bool CheckRoles(ActionExecutingContext filterContext, string actionName, string controllerName)
        {
            bool res = false;
            MembershipUser currentUser = Membership.GetUser(filterContext.HttpContext.User.Identity.Name, userIsOnline: true);
            using(DataJackModelDataContext dataContext = new DataJackModelDataContext())
            {
                var permission = (from m in dataContext.ActionPermissions
                                  where m.actionName == actionName
                                  && m.controllerName == controllerName
                                  select m.permission).FirstOrDefault();
                //若只设置了Control权限, 没有设置Action的权限
                if (string.IsNullOrEmpty(permission))
                {
                    permission = (from m in dataContext.ActionPermissions
                                  where m.controllerName == controllerName
                                  && m.actionName == string.Empty
                                  select m.permission).FirstOrDefault();
                    // 当前页面没有设置权限
                    if (string.IsNullOrEmpty(permission))   res = true;
                }

                if (!string.IsNullOrEmpty(permission))
                {
                    var roles = (from mr in dataContext.UsersInRoles
                                 where mr.User.UserName == currentUser.UserName
                                 select mr.Role.RoleName);
                    foreach (var item in roles)
                    {
                        if (permission.Contains(item))
                        {
                            res = true;
                            break;
                        }
                    }
                }
            }

            return res;
        }
    }
}