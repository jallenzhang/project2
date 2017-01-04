using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DataPersist
{
    public class Achievement
    {
		[XmlAttribute]
        public byte Type { get; set; }
		[XmlAttribute]
        public byte Number { get; set; }
		[XmlAttribute]
        public string Name { get; set; }
		[XmlAttribute]
        public string Condition { get; set; }
		[XmlAttribute]
        public int Chip { get; set; }
		[XmlAttribute]
        public int Exp { get; set; }
    }
}
