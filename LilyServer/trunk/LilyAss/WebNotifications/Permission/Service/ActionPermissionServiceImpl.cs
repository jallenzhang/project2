using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using WebNotifications.Models;
using WebNotifications.Permission.Service;

namespace WebNotifications.Permission
{
    public class ActionPermissionServiceImpl : IActionPermissionService
    {
        #region IActionPermissionService 成员

        public IEnumerable<ActionPermission> GetAllDistinctActionByAssembly()
        {
            var result = GetAllActionByAssembly().Distinct(
                new Comparint<ActionPermission>(
                        delegate(ActionPermission x, ActionPermission y)
                        {
                            if (x.controllerName == y.controllerName
                                && x.actionName == y.actionName)
                                return true;
                            return false;
                        }
                    )
                );

            return result;
        }

        public IList<ActionPermission> GetAllActionByAssembly()
        {
            var result = new List<ActionPermission>();

            var types = Assembly.Load("WebNotifications").GetTypes();

            foreach (var type in types)
            {
                if (type.BaseType != null && type.BaseType.Name == "BaseController")//如果是Controller
                {
                    var class_ap = new ActionPermission();
                    class_ap.actionName = string.Empty;
                    class_ap.controllerName = type.Name.Substring(0, type.Name.Length - 10); // 去掉“Controller”后缀

                    object[] attrs = type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                    if (attrs.Length > 0)
                        class_ap.description = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;
                    result.Add(class_ap);

                    var members = type.GetMethods();
                    foreach (var member in members)
                    {
                        if (member.ReturnType.Name == "ActionResult")//如果是Action
                        {
                            var ap = new ActionPermission();

                            ap.actionName = member.Name;
                            ap.controllerName = member.DeclaringType.Name.Substring(0, member.DeclaringType.Name.Length - 10); // 去掉“Controller”后缀

                            attrs = member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                            if (attrs.Length > 0)
                                ap.description = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;

                            result.Add(ap);
                        }

                    }
                }
            }
            return result;
        }

        public IList<ActionPermission> QueryActionPlist(string query, int start, int limit, out long total)
        {
            IList<ActionPermission> allActions = GetAllActionByAssembly();

            total = (from a in allActions
                     where a.actionName.ToLower().Contains(query.ToLower())
                     select a).Count();

            var result = (from a in allActions
                          where a.actionName.ToLower().Contains(query.ToLower())
                          select a).Skip(start).Take(limit);

            return new List<ActionPermission>(result);
        }

        #endregion
    }
}