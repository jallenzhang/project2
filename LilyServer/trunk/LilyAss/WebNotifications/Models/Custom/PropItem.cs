using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebNotifications.Models.Custom
{
    public class PropItem
    {
        #region public Properties
        [XmlAttribute]
        public int ItemType { get; set; }
        [XmlAttribute]
        public int ItemId { get; set; }
        [XmlAttribute]
        public int Duration { get; set; }
        [XmlAttribute]
        public string ItemName { get; set; }
        [XmlAttribute]
        public int Price { get; set; }
        [XmlAttribute]
        public int Chip { get; set; }
        #endregion
    }
}