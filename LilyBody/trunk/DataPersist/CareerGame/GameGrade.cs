using System.Xml.Serialization;

namespace DataPersist
{
   public class GameGrade
    {
        [XmlAttribute]
        public int ID { get; set; }
       [XmlAttribute]
        public int First { get; set; }
       [XmlAttribute]
        public int Second { get; set; }
       [XmlAttribute]
        public int Tickets { get; set; }
       [XmlAttribute]
        public int Tip { get; set; }
    }
}
