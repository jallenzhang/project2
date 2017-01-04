using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace WebNotifications.Models.Gift
{
    public class GiftManager
    {
        private static GiftManager giftManager;
        public static GiftManager Singleton
        {
            get
            {
                if (giftManager == null)
                {
                    giftManager = new GiftManager();
                }
                return giftManager;
            }
        }

        public Dictionary<int, string> GiftNames { get; private set; }

        private GiftManager()
        {
            GiftNames = new Dictionary<int, string>();
        }

        public void LoadFile(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gift[]), new XmlRootAttribute("Gifts"));
            StreamReader streamReader = new StreamReader(path);
            gift[] gifts = xmlSerializer.Deserialize(streamReader) as gift[];
            streamReader.Close();

            foreach (gift gift in gifts)
            {
                this.GiftNames[gift.id] = gift.name;
            }
        }
    }
}