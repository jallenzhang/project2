using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Logging;
using LilyServer.Model;
using LilyServer.Helper;
using DataPersist;

namespace LilyServer
{



    internal class BankService
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private LilyEntities lilyEntities = new LilyEntities();
        private object lockObj = new object();

        private static BankService _instance = null;

        private delegate void AsyncAddDelegate(long money, BankActionType bat, string userid, byte? item, int? duration);

        private delegate void UserPaymentDelegate(ItemType it,long itemid,int money,string note,string userid);

        public static BankService getInstance()
        {
            if (_instance == null) _instance = new BankService();
            return _instance;
        }

        public void addRecord(long money,BankActionType bat,string userid) {
                     
            AsyncAddDelegate caller = new AsyncAddDelegate(add);
            caller.BeginInvoke(money, bat, userid, null, null,null,null);
        }

        public void addRecord(long money, BankActionType bat, string userid,byte item)
        {
            AsyncAddDelegate caller = new AsyncAddDelegate(add);
            caller.BeginInvoke(money, bat, userid,item, null, null,null);
        }

        public void addRecord(long money, BankActionType bat, string userid,int duration)
        {
            AsyncAddDelegate caller = new AsyncAddDelegate(add);
            caller.BeginInvoke(money, bat, userid, null, duration, null, null);
        }

        protected void add(long money, BankActionType bat, string userid, byte? item, int? duration)
        {
            bank record = new bank();
            if ((int)bat >= 100)
                record.bankin = money;
            else
                record.bankout = money;
            record.createtime = DateHelper.GetNow();
            record.optype = (int)bat;
            record.userid = userid;
            if (item != null)
                record.itemid = item;
            if (duration != null)
                record.duration = duration;
            using (LilyEntities bankentity = new LilyEntities())
            {
                bankentity.bank.AddObject(record);
                lock (lockObj)
                {
                    bankentity.SaveChanges();
                } 
            }
                       
        }


        public void addUserPayment(ItemType it, long itemid, int money,string note, string userid)
        {
            UserPaymentDelegate caller = new UserPaymentDelegate(addpayment);
            caller.BeginInvoke(it,itemid,money,note,userid,null,null);
        }

        protected void addpayment(ItemType it, long itemid, int money,string note, string userid)
        {
            userpayment up = new userpayment();
            up.time = DateHelper.GetNow();
            up.status = 1;
            up.type = (int)it;
            up.itemid = itemid;
            up.money = money;
            up.userid = userid;
            up.note = note;
            up.payway = (int)PayWay.IAP;
            up.alipaynote = GetTransactionId(note); // getMD5(note); //
            using (LilyEntities bankentity=new LilyEntities())
            {
                bankentity.userpayment.AddObject(up);
                bankentity.SaveChanges();
            }
        }

        private string getMD5(string iapstring) {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(iapstring, "MD5");
        }

        private string GetBidId(string iapstring)
        {
            return GetSpecialValue(iapstring, "bid");
        }

        #region edit by djz
        private string GetTransactionId(string iapstring) {
            return GetSpecialValue(iapstring, "transaction-id");
        }

        private string GetSpecialValue(string iapstring,string key) {
            string result = IAPReceiptHelper.DecodeReceipt(iapstring);
            var obj = IAPReceiptHelper.GetKeyValue(result);
            return obj[key].ToString();
        }
        public bool checkIAPString(string iapstring,string userId)
        {
            //string md5str=getMD5(iapstring);

            string bid = GetBidId(iapstring);
            if (bid != "com.javgame.dzpk")
                return true;

            string md5str = GetTransactionId(iapstring);

            using (LilyEntities bankentity = new LilyEntities())
            {
                //return bankentity.userpayment.Any(r => r.alipaynote == md5str && r.userid == userId);
                return bankentity.userpayment.Any(r => r.alipaynote == md5str);
            }
        }
        #endregion

        public void AddDeviceInfo(string channelId,string deviceToken,string jixin,string osVersion) {
            using (LilyEntities dentity = new LilyEntities())
            {
                dentity.LilyReportDataAdd(deviceToken, jixin, osVersion, channelId);
            }
        }
    }
}
