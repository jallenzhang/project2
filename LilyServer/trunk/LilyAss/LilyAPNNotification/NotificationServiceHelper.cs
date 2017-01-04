using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JdSoft.Apple.Apns.Notifications;
using LilyAPNNotification.Model;

namespace LilyAPNNotification
{
    public class NotificationServiceHelper
    {
        public enum NotificationStatus
        {
            Nothing = 0,
            Error = 1,
            NotificationTooLong = 2,
            BadDeviceToken = 4,
            NotificationFailed = 8,
            NotificationSuccess = 16,
            Connecting = 32,
            Connected = 64,
            Disconnected = 128,
        }

        public NotificationService Service;
        public bool IsSandBox { get; set; }
        public string P12File { get; set; }
        public string P12FilePwd { get; set; }
        public int Connections { get; set; }
        public int SendRetries { get; set; }
        public int ReconnectDelay { get; set; }
        public NotificationStatus Status { get; set; }
        public int SleepBetweenNotifications { get; set; }

        private static readonly object padlock = new object();
        private static NotificationServiceHelper instance = null;

        //public static NotificationServiceHelper Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (padlock)
        //            {
        //                instance = new NotificationServiceHelper();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        public NotificationServiceHelper(APNEvnStatus apnEvnStatus)
        {
            Status = NotificationStatus.Nothing;
            IsSandBox = NotificationServiceSet.Default.IsSandBox;
            Connections = NotificationServiceSet.Default.Connections;
            SendRetries = NotificationServiceSet.Default.SendRetries;
            ReconnectDelay = NotificationServiceSet.Default.ReconnectDelay;
            SetP12FileConfig(apnEvnStatus);
            SleepBetweenNotifications = NotificationServiceSet.Default.SleepBetweenNotifications;

            this.StartService();
        }

        public void StartService()
        {
            this.StartService(P12File, P12FilePwd);
        }

        public void StartService(string p12File, string p12FilePwd)
        {
            Service = new NotificationService(IsSandBox, p12File, p12FilePwd, Connections);
            Service.SendRetries = SendRetries;
            Service.ReconnectDelay = ReconnectDelay;
            RegisterEvent();
        }

        public void RegisterEvent()
        {
            Service.Error += new NotificationService.OnError(service_Error);
            Service.NotificationTooLong += new NotificationService.OnNotificationTooLong(service_NotificationTooLong);
            Service.BadDeviceToken += new NotificationService.OnBadDeviceToken(service_BadDeviceToken);
            Service.NotificationFailed += new NotificationService.OnNotificationFailed(service_NotificationFailed);
            Service.NotificationSuccess += new NotificationService.OnNotificationSuccess(service_NotificationSuccess);
            Service.Connecting += new NotificationService.OnConnecting(service_Connecting);
            Service.Connected += new NotificationService.OnConnected(service_Connected);
            Service.Disconnected += new NotificationService.OnDisconnected(service_Disconnected);
        }

        public void CleanUp()
        {
            System.Threading.Thread.Sleep(SleepBetweenNotifications);
            Service.Close();
            Service.Dispose();
        }

        #region Registerted Event

        private void service_BadDeviceToken(object sender, BadDeviceTokenException ex)
        {
            Status = NotificationStatus.BadDeviceToken;
        }

        private void service_Disconnected(object sender)
        {
            Status = NotificationStatus.Disconnected;
        }

        private void service_Connected(object sender)
        {
            Status = NotificationStatus.Connected;
        }

        private void service_Connecting(object sender)           
        {
            Status = NotificationStatus.Connecting;
        }

        private void service_NotificationTooLong(object sender, NotificationLengthException ex)
        {
            Status = NotificationStatus.NotificationTooLong;
        }

        private void service_NotificationSuccess(object sender, Notification notification)
        {
            Status = NotificationStatus.NotificationSuccess;
        }

        private void service_NotificationFailed(object sender, Notification notification)
        {
            Status = NotificationStatus.NotificationFailed;
        }

        private void service_Error(object sender, Exception ex)
        {
            Status = NotificationStatus.Error;
        }

        #endregion

        #region Private Method

        private void SetP12FileConfig(APNEvnStatus apnEvnStatus)
        {
            if (apnEvnStatus == APNEvnStatus.Toufe) {
                P12File = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NotificationServiceSet.Default.P12File);
                P12FilePwd = NotificationServiceSet.Default.P12FilePwd;
            } 
            else if(apnEvnStatus == APNEvnStatus.A91){
                P12File = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NotificationServiceSet.Default.A91P12File);
                P12FilePwd = NotificationServiceSet.Default.A91P12FilePwd;
            }
        }

        #endregion
    }
}
