using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Text;
using System.ComponentModel;

using WebNotifications.Models;
using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;
using WebNotifications.Helper;

namespace WebNotifications.Controllers
{
    [Description("Action权限管理(底下权限可以继承当前权限)")]
    public class ActionManagerController : BaseController
    {
        private DataJackModelDataContext dataContext = new DataJackModelDataContext();        

        #region Action manager

        [Description("Action权限管理--首页")]
        public ActionResult Index(int? page)
        {
            ViewData["tbRolename"] = FillRoles();
            var resList = (from m in dataContext.ActionPermissions
                           orderby m.controllerName, m.actionName
                           select m);

            var pageList = resList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("Action权限管理--删除操作")]
        public ActionResult ActionNameDeleteAction(int id)
        {
            using (DataJackModelDataContext mydataContext = new DataJackModelDataContext()) {
                var obj = (from m in mydataContext.ActionPermissions
                           where m.id == id
                           select m).FirstOrDefault();
                mydataContext.ActionPermissions.DeleteOnSubmit(obj);
                mydataContext.SubmitChanges();
            }
            
            return RedirectToAction("Index");
        }

        [Description("Action权限管理--添加权限名称")]
        public ActionResult AddActionToRolesAction(string[] selectedNames, string tbRolename)
        {
            using (DataJackModelDataContext mydataContext = new DataJackModelDataContext())
            {
                if (selectedNames != null)
                {
                    tbRolename = tbRolename.Trim();
                    var resList = from m in mydataContext.ActionPermissions
                                  where selectedNames.Contains(m.id.ToString())
                                  select m;
                    if (resList.Count() != 0)
                    {
                        foreach (var item in resList)
                        {
                            if (!item.permission.Contains(tbRolename))
                            {
                                item.permission += ";" + tbRolename;
                            }
                        }

                        mydataContext.SubmitChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Description("Action权限管理--添加Action")]
        public ActionResult ActionAdd()
        {
            ViewData["dpActions"] = FillActionNames();
            ViewData["dpRoles"] = FillRoles();
            return View();
        }

        [Description("Action权限管理--添加Action")]
        [HttpPost]
        public ActionResult ActionAdd(string dpActions, string dpRoles)
        {
            using (DataJackModelDataContext mydataContext = new DataJackModelDataContext())
            {
                string[] astr = GetDPActionName(dpActions);
                var res = (from m in mydataContext.ActionPermissions
                           where m.actionName == astr[1].Trim()
                           && m.controllerName == astr[0].Trim()
                           select m).Count();
                if (res == 0)
                {
                    ActionPermission ap = new ActionPermission
                    {
                        controllerName = astr[1].Trim(),
                        actionName = astr[2].Trim(),
                        description = astr[0].Trim(),
                        permission = dpRoles
                    };
                    mydataContext.ActionPermissions.InsertOnSubmit(ap);
                    mydataContext.SubmitChanges();
                }
            }            

            return RedirectToAction("Index");
        }

        [Description("Action权限管理--取消添加Action")]
        public ActionResult ActionAddCancleAction(string id)
        {
            return RedirectToAction("Index");
        }

        [Description("Action权限管理--导入Action配置")]
        public ActionResult ActionConfigImportAction(HttpPostedFileBase uploadActionConfig)
        {
            if (uploadActionConfig == null)
            {
                ViewData["InOut"] = "没有文件";
            }
            else
            {
                ApplyImportAction(uploadActionConfig.InputStream);
            }

            return RedirectToAction("Index");
        }

        [Description("Action权限管理--导出Action配置")]
        public ActionResult ActionConfigExportAction()
        {
            var resList = (from m in dataContext.ActionPermissions
                           select m);
            StringBuilder sb = new StringBuilder();
            foreach (var res in resList)
            {
                sb.Append("actionName:" + res.actionName + "\r\n");
                sb.Append("permission:" + res.permission + "\r\n");
                sb.Append("controllerName:" + res.controllerName + "\r\n");
                sb.Append("description:" + res.description + "\r\n");
                sb.Append("# ########################\r\n");
            }

            ExportToFile.DownLoadAsConfig("action权限管理", sb.ToString());

            return RedirectToAction("Index");
        }

        #endregion

        #region User Method

        [Description("Action权限管理--创建权限页面")]
        public ActionResult AddRoles(int? page)
        {
            var roleList = from m in dataContext.Roles
                           //where m.User.UserName != User.Identity.Name
                           select m;
            var pageList = roleList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("Action权限管理--创建权限操作")]
        public ActionResult CreateRoles(string tbName)
        {
            if(!Roles.RoleExists(tbName)){
                Roles.CreateRole(tbName);
            }
           
            return RedirectToAction("AddRoles");
        }

        [Description("Action权限管理--删除权限操作")]
        public ActionResult DeleteRolesAction(string roleName) {
            if (Roles.RoleExists(roleName))
            {
                Roles.DeleteRole(roleName);
            }

            return RedirectToAction("AddRoles"); 
        }

        [Description("Action权限管理--用户添加权限页面")]
        public ActionResult AddUserToRoles(int? page)
        {
            var roles = Roles.GetAllRoles();
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (var item in roles)
            {
                typeList.Add(
                    new SelectListItem { Text = item, Value = item }
                );
            }
            ViewData["tbRolename"] = new SelectList(typeList, "Value", "Text");

            var userList = (from m in dataContext.Users
                            join n in dataContext.UsersInRoles on m.UserId equals n.UserId into t1
                            from j1 in t1.DefaultIfEmpty()
                            select new WebNotifications.Models.CustomUser.MyUser
                            {
                                UserName = m.UserName,
                                RoleName = j1.Role.RoleName
                            });
            var pageList = userList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("Action权限管理--用户添加权限页面列表")]
        public ActionResult AddUserToRolesAction(string[] selectedNames, string tbRolename)
        {
            if (selectedNames != null)
            {
                foreach (var item in selectedNames)
                {
                    this.AddRoleToUser(item, tbRolename);
                }
            }
            return RedirectToAction("AddUserToRoles");
        }

        [Description("Action权限管理--用户删除")]
        public ActionResult DeleteUserAction(string username)
        {
            Membership.DeleteUser(username);
            return RedirectToAction("AddUserToRoles");
        }

        #endregion

        #region Private Method

        private SelectList FillRoles()
        {
            var roles = Roles.GetAllRoles();
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (var item in roles)
            {
                typeList.Add(
                    new SelectListItem { Text = item, Value = item }
                );
            }

            return new SelectList(typeList, "Value", "Text");
        }

        private SelectList FillActionNames()
        {
            ActionPermissionServiceImpl apsi = new ActionPermissionServiceImpl();
            var roles = apsi.GetAllDistinctActionByAssembly();
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (var item in roles)
            {
                //var str = item.controllerName + ":" + item.actionName + ":" + item.description;
                //var str = item.controllerName + ":" + item.description;
                //var str = item.description;
                var str = item.description + "[" + item.controllerName + ":" + item.actionName+"]";
                typeList.Add(
                    new SelectListItem { Text = str, Value = str }
                );
            }
            return new SelectList(typeList, "Value", "Text");
        }

        private void AddRoleToUser(string username, string rolename)
        {
            username = username.Trim();
            rolename = rolename.Trim();
            string[] roles = Roles.GetRolesForUser(username);
            Roles.RemoveUserFromRoles(username, roles);
            Roles.AddUserToRole(username, rolename);
        }

        private void ApplyImportAction(Stream upstream)
        {
            var importList = AnalyzeActionManagerConfig(upstream);
            if (importList.Count > 0)
            {
                // clear the ActionPermission table
                dataContext.ExecuteCommand("TRUNCATE TABLE ActionPermission");
                dataContext.ActionPermissions.InsertAllOnSubmit(importList);
                dataContext.SubmitChanges();
            }           
        }

        private List<ActionPermission> AnalyzeActionManagerConfig(Stream upstream)
        {
            var objList = new List<ActionPermission>();
            StreamReader fileStream = new StreamReader(upstream);
            ActionPermission obj = new ActionPermission();
            while (true)
            {
                string line = fileStream.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                if (line.StartsWith(@"#"))
                {
                    if(!string.IsNullOrEmpty(obj.controllerName))
                        objList.Add(obj);
                    obj = new ActionPermission();
                    continue;
                }
                var res = AnalyzeLine(line);
                if(res != null)
                    AppendFieldToObj(obj, res.Item1, res.Item2);
            }

            return objList;
        }

        private Tuple<string, string> AnalyzeLine(string line)
        {
            string[] res = line.Split(new char[] { ':' });
            if(res.Length == 2)
                return new Tuple<string, string>(res[0], res[1]);
            return null;
        }

        private void AppendFieldToObj(ActionPermission obj, string field, string value)
        {
            if (field == "actionName")
                obj.actionName = value;
            else if (field == "permission")
                obj.permission = value;
            else if (field == "controllerName")
                obj.controllerName = value;
            else if (field == "description")
                obj.description = value;
        }

        private string[] GetDPActionName(string postAction) {
            string[] astr = new string[3];
            int spos = postAction.IndexOf('[');
            int epos = postAction.LastIndexOf(']');
            astr[0] = postAction.Substring(0, spos);
            spos++;
            string ts = postAction.Substring(spos, epos - spos);
            string[] as2 = ts.Split(new char[] { ':' });
            astr[1] = as2[0];
            astr[2] = as2[1];
            return astr;
        }

        #endregion        
    }
}
