using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Models.ChannelUser;

namespace WebNotifications.Controllers
{
    [Description("用户渠道管理(底下权限可以继承当前权限)")]
    public class ChannelUserManagerController : BaseController
    {
        private DataJackModelDataContext _AssCtx = new DataJackModelDataContext();

        [Description("用户渠道--列表")]
        public ActionResult Index(int? page)
        {
            var channelList = (from m in _AssCtx.Channels
                               select m);            
            List<SelectListItem> typeList = new List<SelectListItem>();
            foreach (var item in channelList)
            {
                typeList.Add(
                    new SelectListItem { Text = item.channelName, Value = item.channelId }
                );
            }
            ViewData["tbChannelName"] = new SelectList(typeList, "Value", "Text");

            var resList = (from m in _AssCtx.Users
                           join p in _AssCtx.UsersInChannels on m.UserId equals p.UserId into t1
                           from j1 in t1.DefaultIfEmpty()
                           join q in _AssCtx.Channels on j1.ChannelId equals q.channelId into t2
                           from j2 in t2.DefaultIfEmpty()
                           select new UserInChannelExt { 
                               UserId = m.UserId.ToString(),
                               UserName = m.UserName,
                               ChannelId = j2.channelId,
                               ChannelName = j2.channelName
                           });
            var pageList = resList.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        [Description("用户渠道--添加渠道到用户")]
        public ActionResult AddUserToChannelsAction(string[] selectedNames, string tbChannelName)
        {
            if (selectedNames != null)
            {
                foreach (var item in selectedNames)
                {
                    this.AddChannelToUser(item, tbChannelName);
                }
            }
            return RedirectToAction("Index");
        }

        [Description("用户渠道--从用户删除渠道")]
        public ActionResult DeleteUserInChannelAction(string userId)
        {
            Guid guserId = Guid.Parse(userId.Trim());
            using (DataJackModelDataContext myCtx = new DataJackModelDataContext())
            {
                // delete                
                var existObj = (from m in myCtx.UsersInChannels
                                where m.UserId == guserId
                                select m).FirstOrDefault();
                if (existObj != null)
                {
                    myCtx.UsersInChannels.DeleteOnSubmit(existObj);
                    myCtx.SubmitChanges();
                }
            }

            return RedirectToAction("Index");
        }

        private void AddChannelToUser(string userId, string channelId)
        {
            Guid guserId = Guid.Parse(userId.Trim());
            channelId = channelId.Trim();
            using (DataJackModelDataContext myCtx = new DataJackModelDataContext())
            {
                // delete                
                var existObj = (from m in myCtx.UsersInChannels
                                    where m.UserId == guserId
                                    select m).FirstOrDefault();
                if(existObj != null){
                    myCtx.UsersInChannels.DeleteOnSubmit(existObj);
                }

                // insert
                UsersInChannel obj = new UsersInChannel
                {
                    UserId = guserId,
                    ChannelId = channelId
                };
                myCtx.UsersInChannels.InsertOnSubmit(obj);
                myCtx.SubmitChanges();
            }
        }
    }
}
