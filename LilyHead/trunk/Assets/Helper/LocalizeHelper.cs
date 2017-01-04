using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using System.IO;

namespace AssemblyCSharp.Helper
{
    public class LocalizeHelper
    {
        private static Dictionary<string, Dictionary<string, string>> stringDictionary = new Dictionary<string, Dictionary<string, string>>();
		public static TextAsset CurrentLanguage;
		
		public static string Translate(string id)
        {
            string retValue = string.Empty;

            string key = id.ToUpper();
            
            //todo get current language
			string filename = Application.systemLanguage.ToString();// + ".xml";
			
            LoadTranslation(ref filename, CurrentLanguage);
			
			Debug.Log("aa " + filename);

            retValue = stringDictionary[filename][key];

            return retValue;
        }

        public static void LoadTranslation(ref string filename, TextAsset languageAsset)
        {
			Debug.Log("In function LoadTranslation");
            if (stringDictionary == null)
                stringDictionary = new Dictionary<string, Dictionary<string, string>>();
			
			Debug.Log("filename is: " + filename);
            // if not loaded before, then load a new one
            if (!stringDictionary.Keys.Contains(filename))
            {
                LoadXMlFile(ref filename, languageAsset);
            }
        }

        private static void LoadXMlFile(ref string filename, TextAsset languageAsset)
        {
            //string filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),filename);
			
//			string filePath = System.IO.Path.Combine(Application.dataPath, "Resoures/Languages");
//			filePath = System.IO.Path.Combine(filePath, filename);
//            if (!System.IO.File.Exists(filePath))
//            {
//                //add some log info here;
//                return;
//            }
			Debug.Log("In function LoadXMlFile:"+filename);
			if(languageAsset==null)
			{
				Debug.Log ("languageAsset is null");
				languageAsset = (TextAsset)Resources.Load("Languages/Chinese.xml");
				filename = "Chinese";
				if (languageAsset == null)
					return;
			}
			byte[] encodedString = Encoding.UTF8.GetBytes(languageAsset.text);

		    // Put the byte array into a stream and rewind it to the beginning
		    MemoryStream ms = new MemoryStream(encodedString);
		    ms.Flush();
		    ms.Position = 0;


            XmlDocument doc = new XmlDocument();
            doc.Load(ms);

            XmlNode root = doc.SelectSingleNode("strings");
            XmlNodeList nodeList = root.ChildNodes;

            foreach(XmlNode node in nodeList)
            {
                XmlElement xe = (XmlElement)node;
                string key = string.Empty;
                string value = string.Empty;
                foreach (XmlNode nd in xe.ChildNodes)
                {
                    XmlElement xce = (XmlElement)nd;
                    if (xce.Name == "key")
                    {
                        key = xce.InnerText;
                    }
                    else if (xce.Name == "value")
                    {
                        value = xce.InnerText;
                    }
					
					if (!key.Equals(string.Empty) && !value.Equals(string.Empty))
	                {
	                    if (!stringDictionary.Keys.Contains(filename))
	                        stringDictionary.Add(filename, new Dictionary<string, string>());
	
	                    if (!stringDictionary[filename].ContainsKey(key))
	                        stringDictionary[filename].Add(key, value);
	
	                }
                }
            }

        }
    }
}
