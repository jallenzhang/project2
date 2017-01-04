using System.Linq;
using System.Xml.Serialization;
using System.IO;
using DataPersist;

namespace LilyServer
{
    public static class XmlResources
    {
        public static readonly PropItem[] AllProps;

        public static readonly GameBlind[] AllGameBlind;
        public static readonly GameGrade[] AllGameGrade;
        private const string PropsFile = "/Resources/Props.xml";

        private const string GameBlindFile = "/Resources/GameBlind.xml";
        private const string GameGradeFile = "/Resources/GameGrade.xml";


        static XmlResources()
        {
            string folderpath = LilyServer.BinaryPathLily;
            AllProps = LoadObjFromXml<PropItem>(folderpath, PropsFile, "Props");
            AllGameBlind = LoadObjFromXml<GameBlind>(folderpath, GameBlindFile, "GameBlinds");
            AllGameGrade = LoadObjFromXml<GameGrade>(folderpath, GameGradeFile, "GameGrades");
        }

        private static T[] LoadObjFromXml<T>(string path,string filename,string root) {            
            var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));
            var streamReader = new StreamReader(path+filename);
            return xmlSerializer.Deserialize(streamReader) as T[];
        }

        public static GameGrade GetGameGrade(int gradeid)
        {
            GameGrade gg = AllGameGrade.FirstOrDefault(r => r.ID == gradeid) ??
                           AllGameGrade.First();
            return gg;
        }
    }


    public class PropItem{
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
