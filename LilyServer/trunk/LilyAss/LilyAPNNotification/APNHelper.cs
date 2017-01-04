using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JdSoft.Apple.Apns.Notifications;
using LilyAPNNotification.Cache;
using LilyAPNNotification.Model;

namespace LilyAPNNotification
{
    public class APNHelper
    {
        public static void SendNotification(string deviceToken, string msg, APNEvnStatus apnEvnStatus)
        {
            APNHelper apnHelper = new APNHelper(apnEvnStatus);
            apnHelper.DeviceToken = deviceToken;
            apnHelper.AddNotification(msg);

            apnHelper.Service.CleanUp();
        }

        //public static void SendNotification(string deviceToken, string msg)
        //{
        //    Task task = new Task();
        //    task.Params.Add(deviceToken);
        //    task.Params.Add(msg);

        //    NotificationServiceCache.ExcuteNotifyPush(task);
        //}

        public static void SendNotification(APNHelper apnHelper)
        {
            apnHelper.AddNotification();
        }

        public string DeviceToken { get; set; }
        public string Body { get; set; }
        public int Badge { get; set; }
        public string Sound { get; set; }
        public string Action_Loc_Key { get; set; }
        public string Loc_Key { get; set; }
        public List<object> Loc_Args { get; set; }

        public NotificationServiceHelper Service { get; set; }

        public APNHelper(APNEvnStatus apnEvnStatus)
        {
            SetDefault(apnEvnStatus);
        }

        public void SetDefault(APNEvnStatus apnEvnStatus)
        {
            DeviceToken = "f2780c0b3824ea169c3ca5454165e3202c1f331a57265c31af708f58fe79fc4d";
            //Service = NotificationServiceHelper.Instance;
            Service = new NotificationServiceHelper(apnEvnStatus);

            Body = "test...";
            Badge = 1;
            Sound = "default";
        }

        public bool AddNotification()
        {
            return this.AddNotification(Body);
        }

        public bool AddNotification(string msg)
        {
            return this.AddNotification(msg, Sound);
        }

        public bool AddNotification(string msg, string sound)
        {
            return this.AddNotification(msg, sound, Badge);
        }

        public bool AddNotification(string msg, string sound, int badge)
        {
            Body = msg;
            Sound = sound;
            Badge = badge;
            Notification alertNotification = GetNotification();
            bool res = Service.Service.QueueNotification(alertNotification);     
           
            return res;
        }

        private Notification GetNotification()
        {            
            Notification alertNotification = new Notification(DeviceToken);
            alertNotification.Payload.Sound = Sound;
            alertNotification.Payload.Badge = Badge;
            alertNotification.Payload.Alert.Body = Body;
            alertNotification.Payload.Alert.ActionLocalizedKey = Action_Loc_Key;
            alertNotification.Payload.Alert.LocalizedKey = Loc_Key;
            alertNotification.Payload.Alert.LocalizedArgs = Loc_Args;

            return alertNotification;
        }
    }
}
