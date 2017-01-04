using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LilyAPNNotification;
using DataPersist;
using ExitGames.Logging;
using LilyAPNNotification.Model;

namespace LilyServer.Helper
{
    public class LilyPN
    {

        /// <summary>
        ///   An <see cref = "ILogger" /> instance used to log messages to the logging framework.
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();


        public static void SendNotification(string receiveUserID, NotificationType nt, string senderUserID)
        {
            try
            {
                if (string.IsNullOrEmpty(receiveUserID))
                    return;
                Model.user myUser = UserService.getInstance().QueryUserByUserId(receiveUserID);
                UserData receiveUser = myUser.ToUserData();
                //用户设备检查
                if (receiveUser.DeviceType != DeviceType.IOS || string.IsNullOrEmpty(receiveUser.DeviceToken))
                    return;
                //是否开启通知
                if (!receiveUser.FriendNotify)
                    return;
                //在线状态检查
                if (receiveUser.Status == UserStatus.Idle || receiveUser.Status == UserStatus.Playing)
                    return;

                string deviceToken = receiveUser.DeviceToken;
                string senderName = string.Empty;
                if (!string.IsNullOrEmpty(senderUserID)) {
                    UserData sender = UserService.getInstance().QueryUserByUserId(senderUserID).ToUserData();
                    senderName = sender.NickName;
                }

                APNEvnStatus apnEvnSatus = GetAPNEvnStatus(myUser);

                switch (nt)
                {
                    case NotificationType.System:
                        //SendNotificationHelp.SendNotificationSystemInfo();
                        break;
                    case NotificationType.ActionNeeded:
                        SendNotificationHelp.SendNotificationShowCars(deviceToken, apnEvnSatus);
                        break;
                    case NotificationType.AddFriend:
                        //SendNotificationHelp.
                        SendNotificationHelp.SendNotificationFriendRequest(deviceToken, senderName, apnEvnSatus);
                        break;
                    case NotificationType.SendChips:
                        SendNotificationHelp.SendNotificationChipsReceived(deviceToken, senderName, apnEvnSatus);
                        break;
                    case NotificationType.InviteFriendGame:
                        SendNotificationHelp.SendNotificationFriendJoinGame(deviceToken, senderName, apnEvnSatus);
                        break;
                    case NotificationType.InviteFriendRoom:
                        SendNotificationHelp.SendNotificationFriendVisit(deviceToken, senderName, apnEvnSatus);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            { 
                //to do...........
                log.Error(ex.Message,ex);
            }
        }

        public static APNEvnStatus GetAPNEvnStatus(Model.user userData)
        {
            if (userData.usertype == (int)UserType.NinetyOne)
            {
                return APNEvnStatus.A91;
            }
            else if (userData.usertype == (int)UserType.Guest && userData.channelid == "fc.91ios")
            {
                return APNEvnStatus.A91; 
            }
            else
            {
                return APNEvnStatus.Toufe;
            }
        }
    }
}
