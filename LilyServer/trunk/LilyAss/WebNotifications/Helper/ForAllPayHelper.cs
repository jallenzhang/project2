using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebNotifications.Models;
using DataPersist;
using WebNotifications.Models.Custom;

namespace WebNotifications.Helper
{
    public class ForAllPayHelper
    {
        public bool IsOrderAlreadyExistInDB(string out_trade_no, string userid)
        {
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext())
            {
                var res = (from m in mydataContext.userpayments
                           where m.alipaynote == out_trade_no
                          && m.userid == userid
                           select m).Count();
                if (res == 0)
                    return true;
            }

            return false;
        }

        public void InsertToPayment(userpayment myPayment)
        {
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext())
            {
                mydataContext.userpayments.InsertOnSubmit(myPayment);
                mydataContext.SubmitChanges();
            }
        }

        public user AddUserChips(string userId, string price, int itemType, object itemValue)
        {
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext())
            {
                var myuser = (from m in mydataContext.users
                              where m.userid == userId
                              select m).FirstOrDefault();
                long addmoney = (long)Math.Round(double.Parse(price), 0);
                if (myuser != null)
                {
                    if (myuser.money == null)
                        myuser.money = 0;
                    myuser.money += addmoney;

                    if (itemType == (int)ItemType.Chip)
                    {                        
                        long addchips = itemValue == null ? 0 : (long)itemValue;
                        if (myuser.chips == null)
                            myuser.chips = 0;
                        myuser.chips += addchips;
                    }
                    else if (itemType == (int)ItemType.Room)
                    {
                        int roomTypeName = (int)itemValue;
                        myuser.ownroomtypes |= roomTypeName;
                    }

                    mydataContext.SubmitChanges();
                }

                return myuser;
            }
        }

        //只是用于成就
        public void InsertUserMsg(user myUser, int itemType)
        {
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext())
            {
                if (null == myUser) return;

                int number = 8;
                bool res = ((myUser.achievements ?? 0) & (1L << (number - 1))) == 1L << (number - 1);
                if (res) return;

                if (itemType == (int)ItemType.Chip)
                {
                    usermessage usermsg = new usermessage
                    {
                        sender = myUser.userid,
                        receiver = myUser.userid,
                        messagetype = (byte)MessageType.BuyChips,
                        content = string.Empty,
                        time = DateHelper.GetNow(),
                    };

                    mydataContext.usermessages.InsertOnSubmit(usermsg);
                    mydataContext.SubmitChanges();
                }
            }
        }

        public void InsertUserProps(int itemType, object itemValue, string userid)
        {
            using (DataLilyModelDataContext mydataContext = new DataLilyModelDataContext())
            {
                int tmp_itemid = 0;
                int.TryParse(itemValue.ToString(), out tmp_itemid);

                PropItem buyItem = BuyManagerHelper.GetPropItem(itemType, tmp_itemid);
                if (null == buyItem) return;

                userprop uprops = mydataContext.userprops.FirstOrDefault(r => r.UserId == userid && r.ItemType == itemType && r.ItemId == tmp_itemid && r.Status > 0);

                if (uprops != null)
                {
                    uprops.Duration += buyItem.Duration;
                }
                else
                {
                    userprop newProp = new userprop();
                    newProp.ItemId = buyItem.ItemId;
                    newProp.ItemName = buyItem.ItemName;
                    newProp.ItemType = buyItem.ItemType;
                    newProp.PurchaseDate = DateHelper.GetNow();
                    newProp.Status = 1;
                    newProp.UserId = userid;
                    newProp.Duration = buyItem.Duration;

                    mydataContext.userprops.InsertOnSubmit(newProp);
                }

                mydataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// int: type; string: itemId
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public Tuple<int, long?> GetRoomAndChips(string subject)
        {
            int typeId = 0;

            //筹码
            long itemId = BuyManagerHelper.GetTypeIdByName(subject, BuyChipsDic.myDic);
            if (itemId != 0)
                return new Tuple<int, long?>((int)ItemType.Chip, itemId);

            //房间
            typeId = BuyManagerHelper.GetTypeIdByName(subject, BuyScenceDic.myDic);
            if (typeId != 0)
                return new Tuple<int, long?>((int)ItemType.Room, typeId);

            //头像
            typeId = BuyManagerHelper.GetTypeIdByName(subject, BuyAvatorDic.myDic);
            if (typeId != 0)
                return new Tuple<int, long?>((int)ItemType.Avator, typeId);

            //血统
            typeId = BuyManagerHelper.GetTypeIdByName(subject, BuyLineageDic.myDic);
            if (typeId != 0)
                return new Tuple<int, long?>((int)ItemType.Lineage, typeId);

            //玉
            typeId = BuyManagerHelper.GetTypeIdByName(subject, BuyJadeDic.myDic);
            if (typeId != 0)
                return new Tuple<int, long?>((int)ItemType.Jade, typeId);

            // if reach this code means some error happend
            return new Tuple<int, long?>((int)ItemType.Room, -1);
        }

        public static string[] GetUserIdAndChannelId(string out_trade_no) {            
            int pos = out_trade_no.IndexOf("_") + 1;
            string append_str = out_trade_no.Substring(pos);
            var tmp_array = append_str.Split(new char[] { '_' });

            string[] res = new string[tmp_array.Length - 1];
            res[0] = tmp_array[0] + "_" + tmp_array[1];
            if (tmp_array.Length > 2)
            {
                res[1] = tmp_array[2];
            }
            if (tmp_array.Length > 3)
            {
                res[1] = res[1] + "." + tmp_array[3];
            }

            return res;
        }
    }
}