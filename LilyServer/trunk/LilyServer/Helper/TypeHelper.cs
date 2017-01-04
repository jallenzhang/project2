using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPersist.HelperLib;
using DataPersist;
using System.Collections;

namespace LilyServer.Helper
{
    public static class TypeHelper
    {
        public static byte getByte(this Int32 i)
        {
            return (byte)i;
        }

        public static int getInt32(this string str)
        {
            return Convert.ToInt32(str);
        }       

        public static string ToStringValue(this byte[] bts)
        {
            //string str = SerializeHelper.Deserialize(bts) as string;

            return string.Empty;
        }

        #region UserStatus

        public static UserStatus getStatus(this Int32? status)
        {
            return (UserStatus)Enum.ToObject(typeof(UserStatus), status.Value);
        }

        public static int getInt32Value(this UserStatus value)
        {
            return (int)value;
        }

        #endregion

        #region UserType

        public static UserType getUserType(this Int32? type)
        {
            return (UserType)Enum.ToObject(typeof(UserType), type.Value);
        }

        public static int getInt32Value(this UserType value)
        {
            return (int)value;
        }

        #endregion

        #region user

        public static Hashtable Tobyte(this UserData data)
        {
            Hashtable ht = SerializeHelper.Serialize(data);

            return ht;
        }

        public static UserData ToUserData(this Hashtable ht)
        {
            return SerializeHelper.Deserialize(ht) as UserData;
        }

        public static Hashtable Tobyte(this List<UserData> datas)
        {
            Hashtable ht = SerializeHelper.Serialize(datas);

            return ht;
        }     

        #endregion

        #region friend

        public static Hashtable Tobyte(this FriendData data)
        {
            Hashtable ht = SerializeHelper.Serialize(data);

            return ht;
        }

        public static FriendData ToFriendData(this Hashtable ht)
        {
            return SerializeHelper.Deserialize(ht) as FriendData;
        }

        #endregion

        #region feedback

        public static Hashtable ToHashtable(this FeedbackData data)
        {
            Hashtable ht = SerializeHelper.Serialize(data);

            return ht;
        }

        public static FeedbackData ToFeedbackData(this Hashtable ht)
        {
            return SerializeHelper.Deserialize(ht) as FeedbackData;
        }

        #endregion

        #region tableinfo

        public static Hashtable ToHashtable(this TableInfo table)
        {
            Hashtable ht = SerializeHelper.Serialize(table);

            return ht;
        }

        public static TableInfo ToTableInfo(this Hashtable ht)
        {
            return SerializeHelper.Deserialize(ht) as TableInfo;
        }

        #endregion

        #region PlayerInfo
        public static Hashtable ToHashtable(this PlayerInfo playerInfo)
        {
            return SerializeHelper.Serialize(playerInfo);
        }

        public static PlayerInfo ToPlayerInfo(this Hashtable hashtable)
        {
            return SerializeHelper.Deserialize(hashtable) as PlayerInfo;
        }
        #endregion

        #region History

        public static Hashtable Tobyte(this List<PlayerAction> pa) {
            Hashtable ht = SerializeHelper.Serialize(pa);
            return ht;
        }
        public static Hashtable Tobyte(this PlayerAction data)
        {
            Hashtable ht = SerializeHelper.Serialize(data);
            return ht;
        }


        #endregion
    }
}
