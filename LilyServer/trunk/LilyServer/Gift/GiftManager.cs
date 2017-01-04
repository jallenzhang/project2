using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace LilyServer
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

        public Dictionary<int,int> GiftPrices { get; private set; }

        private GiftManager()
        {
            GiftPrices = new Dictionary<int, int>();
        }

        public void LoadFile(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gift[]), new XmlRootAttribute("Gifts"));
            StreamReader streamReader = new StreamReader(path);
            gift[] gifts = xmlSerializer.Deserialize(streamReader) as gift[];
            streamReader.Close();

            foreach (gift gift in gifts)
            {
                this.GiftPrices[gift.id] = gift.price;
            }
        }
    }
}
