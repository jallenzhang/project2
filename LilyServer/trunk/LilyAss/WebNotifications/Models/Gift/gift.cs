using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebNotifications.Models.Gift
{
    public class gift
    {
        [XmlElement]
        public int id { get; set; }
        [XmlElement]
        public string name { get; set; }
    }
}