using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LilyServer.Helper
{
    public class WordsFilter
    {
        public static string path=string.Empty;
        private const string filename = "words.txt";
        private Dictionary<char, IList<string>> keyDict;

        private static WordsFilter _instance = null;

        public static WordsFilter getInstance()
        {
            if (_instance == null) _instance = new WordsFilter();
            return _instance;
        }

        static WordsFilter() {
            path = LilyServer.BinaryPathLily;
        }

        public WordsFilter() {
            string words = this.ReadFile().Replace("\r\n", "|");
            string[] strList = words.Split('|');
            keyDict = new Dictionary<char, IList<string>>();
            if (strList.Length>0)
            {
                foreach (string s in strList)
                {
                    if (s == "")
                    {
                        continue;
                    }
                    if (keyDict.ContainsKey(s[0]))
                    {
                        keyDict[s[0]].Add(s);
                    }
                    else
                    {
                        keyDict.Add(s[0], new List<string> { s });
                    }
                }
            }
        }

        private string ReadFile() {

            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }
            //string path = System.Environment.CurrentDirectory+ "\\bin\\" + filename;
            string filpath = path + "\\" + filename;
            if (File.Exists(filpath))
            {
                using (FileStream fs = new FileStream(filpath, FileMode.Open))
                {
                    return new StreamReader(fs).ReadToEnd();
                }
            }
            return string.Empty;           
        }

        public string Filter(string message) {
            if (string.IsNullOrEmpty(message))
                return string.Empty;
            int len = message.Length;
            StringBuilder sb = new StringBuilder(len);
            bool isfilter = true;
            for (int i = 0; i < len; i++)
            {
                if (keyDict.ContainsKey(message[i]))
                {
                    foreach (string s in keyDict[message[i]])
                    {
                        isfilter = true;
                        int j = i;
                        foreach (char c in s)
                        {
                            if (j >= len || c != message[j++])
                            {
                                isfilter = false;
                                break;
                            }
                        }
                        if (isfilter)
                        {
                            i += s.Length - 1;
                            sb.Append('*', s.Length);
                            break;
                        }

                    }
                    if (!isfilter)
                    {
                        sb.Append(message[i]);
                    }
                }
                else
                {
                    sb.Append(message[i]);
                }
            }
            return sb.ToString();
        }
    }
}
