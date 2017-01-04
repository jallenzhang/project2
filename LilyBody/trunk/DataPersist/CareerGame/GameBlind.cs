using System.Xml.Serialization;

namespace DataPersist
{
   public  class GameBlind
    {
       [XmlAttribute]
        public int ID { get; set; }
       [XmlAttribute]
        public int BigBlind { get; set; }
    }
}
