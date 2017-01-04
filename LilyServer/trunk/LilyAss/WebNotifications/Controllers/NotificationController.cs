using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using DataPersist;
using LilyAPNNotification;
using Webdiyer.WebControls.Mvc;
using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Models.Custom;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles="Administrator")]
    [Description("推送(底下权限可以继承当前权限)")]
    public class NotificationController : BaseController
    {
        private DataLilyModelDataContext dataContext = new DataLilyModelDataContext();

        [Description("推送--首页")]
        public ActionResult PushMsg()
        {
            return View("PushMsg");
        }

        //public ActionResult ShowCardsAction()
        //{
        //    SendAllNotifications(SendNotificationHelp.SendNotificationShowCars);
        //    return View("PushMsg");
        //}

        //public ActionResult FriendRequestAction(string tbMsg)
        //{
        //    SendAllNotifications(tbMsg, SendNotificationHelp.SendNotificationFriendRequest);
        //    return View("PushMsg");
        //}

        //public ActionResult ChipsReceivedAction(string tbMsg)
        //{
        //    SendAllNotifications(tbMsg, SendNotificationHelp.SendNotificationChipsReceived);
        //    return View("PushMsg");
        //}

        //public ActionResult FriendVisitAction(string tbMsg)
        //{
        //    SendAllNotifications(tbMsg, SendNotificationHelp.SendNotificationFriendVisit);
        //    return View("PushMsg");
        //}

        //public ActionResult FirendJoinGameAction(string tbMsg)
        //{
        //    SendAllNotifications(tbMsg, SendNotificationHelp.SendNotificationFriendJoinGame);
        //    return View("PushMsg");
        //}

        [Description("推送--系统推送操作")]
        public ActionResult SystemInfoAction(string tbMsg)
        {
            //SendAllNotifications(tbMsg,SendNotificationHelp.SendNotificationSystemInfo);
            return View("PushMsg");
        }

        private void SendAllNotifications(Action<string> fun)
        {
            var list = GetIOSTokenList();
            foreach (var item in list)
            {
                fun.Invoke(item);
            }
        }

        private void SendAllNotifications(string tbMsg, Action<string, string> fun)
        {
            var list = GetIOSTokenList();
            foreach (var item in list)
            {
                fun.Invoke(item, tbMsg);
            }

            //jack_testMulityTimes(tbMsg, fun);
        }

        private List<string> GetIOSTokenList()
        {
            int deviceType = (int)DeviceType.IOS;
            var tokenList = from m in dataContext.users
                             where m.systempn != false // can call the notifaction
                             && m.devicetype == deviceType
                             && m.devicetoken != null
                             && m.devicetoken != string.Empty
                             select m.devicetoken;
            
            return tokenList.ToList<string>();
        }

        private void jack_testMulityTimes(string tbMsg, Action<string, string> fun)
        {
            int cnt = 111;
            var list = jack_test();
            for (int i = 0; i < cnt; i++)
            {
                foreach (var item in list)
                {
                    fun.Invoke(item, tbMsg + " " + i);
                }
            }
        }

        private List<string> jack_test() {
            List<string> list = new List<string>();
            list.Add("7f52d3aae10114c77161b9343a21fcefc3c9b8af1f5f34c4e26b4a0bcb42a7dd");
            list.Add("817e01489784b38e65a4a39bfd7eb9aec67942feb9236508db8cebdb632e1e62");
            list.Add("9ca9262007239e65b34dd5f05655ca07fb218ce7d92daa9970c4fc346502a2eb");

            //iphone4
            list.Add("f2780c0b3824ea169c3ca5454165e3202c1f331a57265c31af708f58fe79fc4d");

            list.Add("55a6cbebe0eeb8d1fb317b7f00e60c0b81432ec0ebec0a07c22d419ac0eefce3");
            list.Add("d332b45ad23df8faee57aa3a272fd78497cdc3ac2a813a8106f640f2273f9300");

            //正式 itouch
            list.Add("f0b344c861ffe68469d81afd7a46b3dd03bf923e9edc07894c9f45639bf8ae2d");

            //越狱 itouch
            list.Add("5976ea81d831cee171bed58e184714b4c809d8ebacf5f2b10aa4f2eb6c899f97");

            list.Add("2d7670f4288e499d168500c21c3c490ea07535a456bd8b642ddc7aa8954cc9d8");

            //ipad
            list.Add("91f29883bcdd7d69554fc5bedce68e881e41cc9190410d93b5d2ff0f21bb2fb2");

            

            return list;
        }
    }
}
