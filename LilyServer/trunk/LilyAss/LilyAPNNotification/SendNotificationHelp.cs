using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LilyAPNNotification.Model;

namespace LilyAPNNotification
{
    public class SendNotificationHelp
    {
        public const string NOTIFICATION_SHOW_CARDS = "轮到你出牌了！";
        public const string NOTIFICATION_FRIEND_REQUEST = "在康熙棋牌室中，%@想要将你添加为牌友，快去看看吧！";
        public const string NOTIFICATION_CHIPS_RECEIVED = "在康熙棋牌室中，%@赠送给你一大笔筹码，快去看看吧。";
        public const string NOTIFICATION_FRIEND_VISIT = "在康熙棋牌室中，%@邀请你去TA家串门。";
        public const string NOTIFICATION_FIREND_JOIN_GAME = "在康熙棋牌室中，%@邀请你参加TA的牌局。";
        public const string NOTIFICATION_SYSTEM_INFO = "%@";

        /// <summary>
        /// 轮到你出牌了！
        /// </summary>
        /// <param name="deviceToken"></param>
        public static void SendNotificationShowCars(string deviceToken, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_SHOW_CARDS, string.Empty, apnEvnStatus);
        }

        /// <summary>
        /// 在康熙棋牌室中，%@想要将你添加为牌友，快去看看吧！
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="name">玩家姓名</param>
        public static void SendNotificationFriendRequest(string deviceToken, string name, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_FRIEND_REQUEST, name, apnEvnStatus);
        }

        /// <summary>
        /// 在康熙棋牌室中，%@赠送给你一大笔筹码，快去看看吧。
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="name">玩家姓名</param>
        public static void SendNotificationChipsReceived(string deviceToken, string name, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_CHIPS_RECEIVED, name, apnEvnStatus);
        }

        /// <summary>
        /// 在康熙棋牌室中，%@邀请你去TA家串门。
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="name">玩家姓名</param>
        public static void SendNotificationFriendVisit(string deviceToken, string name, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_FRIEND_VISIT, name, apnEvnStatus);
        }

        /// <summary>
        /// 在康熙棋牌室中，%@邀请你参加TA的牌局。
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="name">玩家姓名</param>
        public static void SendNotificationFriendJoinGame(string deviceToken, string name, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_FIREND_JOIN_GAME, name, apnEvnStatus);
        }

        /// <summary>
        /// 系统消息
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="info">系统消息</param>
        public static void SendNotificationSystemInfo(string deviceToken, string info, APNEvnStatus apnEvnStatus)
        {
            SendNotification(deviceToken, NOTIFICATION_SYSTEM_INFO, info, apnEvnStatus);
        }

        /// <summary>
        /// 自定义消息
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="loc_Key">localize key</param>
        /// <param name="info">argument string</param>
        public static void SendNotification(string deviceToken, string loc_Key, string info, APNEvnStatus apnEvnStatus)
        {
            APNHelper apn = new APNHelper(apnEvnStatus);
            apn.DeviceToken = deviceToken;
            apn.Loc_Key = loc_Key;
            if (!string.IsNullOrEmpty(info))
            {
                apn.Loc_Args = new List<object>();
                apn.Loc_Args.Add(info);
            }

            APNHelper.SendNotification(apn);
        }
    }
}
