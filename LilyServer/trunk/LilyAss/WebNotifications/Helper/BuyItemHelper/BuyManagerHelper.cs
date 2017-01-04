using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

using WebNotifications.Models.Custom;
using System.IO;

namespace WebNotifications.Helper
{
    public class BuyManagerHelper
    {
        #region Get type or name
		 
        public static int GetTypeIdByName(string name, Dictionary<string, int> myDic)
        {
            if (myDic.ContainsKey(name))
                return (int)myDic[name];
            return 0;
        }

        public static int GetTypeIdByName(string name, Dictionary<string, long> myDic)
        {
            if (myDic.ContainsKey(name))
                return (int)myDic[name];
            return 0;
        }

        public static string GetNameByTypeId(int typeid, Dictionary<string, int> myDic)
        {
            foreach (var item in myDic)
            {
                if (item.Value == typeid)
                {
                    return item.Key;
                }
            }

            return string.Empty;
        }

        public static string GetNameByTypeId(long typeid, Dictionary<string, long> myDic)
        {
            foreach (var item in myDic)
            {
                if (item.Value == typeid)
                {
                    return item.Key;
                }
            }

            return string.Empty;
        }

	    #endregion

        #region Get Props Object

        private static List<PropItem> myPropList = null;

        public static PropItem GetPropItem(int typeId, int ItemId) 
        {
            if (myPropList == null)
            {
                string binaryPath = AppDomain.CurrentDomain.BaseDirectory;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<PropItem>), new XmlRootAttribute("Props"));
                StreamReader streamReader = new StreamReader(binaryPath + "/Resources/Props.xml");
                myPropList = xmlSerializer.Deserialize(streamReader) as List<PropItem>;
                streamReader.Close();
            }

            if (myPropList == null) return null;

            PropItem myItem = myPropList.Find(rs => rs.ItemType == typeId && rs.ItemId == ItemId);

            return myItem;         
        }

	    #endregion        
    }
}