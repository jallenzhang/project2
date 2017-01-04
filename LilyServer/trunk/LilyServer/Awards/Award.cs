using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LilyServer
{
    public class Award
    {
        [XmlAttribute]
        public byte Id { get; set; }

        public int Guest { get; set; }
        public int Normal { get; set; }
        public int Pay { get; set; }
    }
}
