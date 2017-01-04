using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace EricUtility
{
    public class StringUtility
    {
        public static string Extract(string text, string before, string after)
        {
            return Extract(text, before, after, 0);
        }
        public static string Extract(string text, string before, string after, int startindex)
        {
            int ideb = text.IndexOf(before, startindex) + before.Length;
            if (ideb < before.Length)
                return null;
            int ifin = text.IndexOf(after, ideb);
            if (ifin < 0)
                return null;
            return text.Substring(ideb, ifin - ideb);
        }
        public static string RemoveHTMLTags(string s)
        {
            // Faster than regex: http://dotnetperls.com/remove-html-tags

            char[] array = new char[s.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < s.Length; i++)
            {
                char let = s[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        public static string RemoveBBCodeTags(string s)
        {
            // Faster than regex: http://dotnetperls.com/remove-html-tags

            char[] array = new char[s.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < s.Length; i++)
            {
                char let = s[i];
                if (let == '[')
                {
                    inside = true;
                    continue;
                }
                if (let == ']')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
