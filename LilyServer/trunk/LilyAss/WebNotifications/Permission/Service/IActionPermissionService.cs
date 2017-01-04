using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebNotifications.Models;

namespace WebNotifications.Permission.Service
{
    public interface IActionPermissionService
    {
        /// <summary>
        /// 获取当前系统所有的Action
        /// </summary>
        /// <returns></returns>
        IList<ActionPermission> GetAllActionByAssembly();

        IList<ActionPermission> QueryActionPlist(string query, int start, int limit, out long total);
    }
}
