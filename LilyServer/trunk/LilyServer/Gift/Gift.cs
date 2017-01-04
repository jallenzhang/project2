using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LilyServer
{
    public class gift
    {
        [XmlElement]
        public int id { get; set; }
        [XmlElement]
        public int price { get; set; }
    }
}
