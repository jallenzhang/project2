using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    public class StringTokenizer
    {
        private string m_TotalStr;
        private string[] m_SplittedStr;
        private int m_Index;
        private char m_Delimitter;

        public StringTokenizer(string str, char delimitter)
        {
            m_TotalStr = str;
            m_Delimitter = delimitter;
            if (str == null)
                m_SplittedStr = new string[0];
            else
                m_SplittedStr = str.Split(delimitter);
            m_Index = -1;
        }

        public void Reset()
        {
            m_Index = -1;
        }

        public bool HasMoreTokens()
        {
            return CountTokens() > m_Index;
        }

        public int CountTokens()
        {
            return m_SplittedStr.Length;
        }

        public string NextToken()
        {
            m_Index++;
            if (HasMoreTokens())
            {
                string s = m_SplittedStr[m_Index];
                if (s == "")
                    return NextToken();
                return s;
            }
            else
                return null;
        }
    }
}
