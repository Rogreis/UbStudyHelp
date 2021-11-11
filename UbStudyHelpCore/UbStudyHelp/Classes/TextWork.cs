using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UbStudyHelp.Classes
{
    public class TextWork
    {
        private const int maxTextSixe = 80;

        public string DecodedText
        {
            get
            {
                return sbText.ToString();
            }
        }

        private StringBuilder sbText;


        private Dictionary<string, string> EncodedCharacters = new Dictionary<string, string>()
        {
            {"&amp;",  "&"},
            {"&lt;",   "<"},
            {"&gt;",   ">"},
            {"&quot;", "\""},
            {"&apos;", "'"}
        };

        public TextWork(string text)
        {
            sbText = new StringBuilder(text);
            foreach (KeyValuePair<string, string> pair in EncodedCharacters)
            {
                sbText.Replace(pair.Key, pair.Value);
            }
        }

        public string GetReducedText()
        {
            string text = sbText.ToString();
            int maxCharCount = text.Length < maxTextSixe ? text.Length : maxTextSixe;
            int size = maxCharCount;
            // \p{P} matches one of !"#$%&'()*+,-./:;<=>?@[]^_`{|}~
            // [\p{P}-[<>]]   used when < and > should be removed
            // (?![\._])\p{P} check this?? 
            Regex rx = new Regex(@"^.*?\p{P}-[<>]");
            foreach (Match match in rx.Matches(text))
            {
                if (match.Index >= maxCharCount)
                {
                    size = match.Index;
                    break;
                }
            }
            return text.Substring(0, size);
        }
    }
}
