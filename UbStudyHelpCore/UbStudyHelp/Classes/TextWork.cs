using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace UbStudyHelp.Classes
{

    public enum TextTag
    {
        Normal,
        Bold,
        Italic,
        Superscript
    }

    public class UbTextTag
    {
        public TextTag Tag { get; set; } = TextTag.Normal;
        public string Text { get; set; } = "";
        public UbTextTag(TextTag tag, string text)
        {
            Tag = tag;
            Text = text;
        }
    }


    public class TextWork
    {
        private const int maxTextSixe = 80;

        private StringBuilder sbText;


        private Dictionary<string, string> EncodedCharacters = new Dictionary<string, string>()
        {
            {"&amp;",  "&"},
            {"&lt;",   "<"},
            {"&gt;",   ">"},
            {"&quot;", "\""},
            {"&apos;", "'"}
        };


        private class HtmlTag
        {
            public const string markWithoutTag = "|~S~|";
            public const string indicatorTag = "|~I~|";

            public static string[] Separators = { markWithoutTag };

            public string Start { get; set; }
            public string End { get; set; }
            public TextTag Tag { get; set; }

            public string MarkStart
            {
                get
                {
                    return markWithoutTag + indicatorTag + ((int)Tag).ToString("00");
                }
            }

            public string MarkEnd
            {
                get
                {
                    return markWithoutTag;
                }
            }

            public HtmlTag(string start, string end, TextTag tag)
            {
                Start = start;
                End = end;
                Tag = tag;
            }

        }

        private List<HtmlTag> HtmlTags = new List<HtmlTag>()
        {
            new HtmlTag("<b>", "</b>",  TextTag.Bold),
            new HtmlTag("<em>", "</em>",  TextTag.Italic),
            new HtmlTag("<sup>", "</sup>",  TextTag.Superscript)
        };


        public string DecodedText
        {
            get
            {
                return sbText.ToString();
            }
        }

        public int MaxSize
        {
            get
            {
                return maxTextSixe;
            }
        }

        public TextWork(string text)
        {
            sbText = new StringBuilder(text);
            foreach (KeyValuePair<string, string> pair in EncodedCharacters)
            {
                sbText.Replace(pair.Key, pair.Value);
            }
        }


        private string RemoveAll(string input, string toReplace, string newValue)
        {
            //Regex rx = new Regex("/" + toReplace + "/gi");
            return Regex.Replace(input, toReplace, newValue);
        }

        private string GetPlainText()
        {
            string text = SecurityElement.Escape(sbText.ToString());
            text = RemoveAll(sbText.ToString(), "&lt;", "<");
            text = RemoveAll(text, "&gt;", "<");
            text = RemoveAll(text, "<span class=\"Colored\">", "");
            text = RemoveAll(text, "<span class=\"SCaps\">", "");
            foreach(HtmlTag tag in HtmlTags)
            {
                text = RemoveAll(text, tag.Start, "");
                text = RemoveAll(text, tag.End, "");
            }
            return RemoveAll(text, "</span>", "");

        }

        public string GetReducedText()
        {
            string text = GetPlainText();
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

        public string GetHtml()
        {
            /*
             Encoded chars
                  &lt;    <
                  &gt;    >

             Possible Html tags found in the text
                  "<span class=\"SCaps\">", "</span>"
                  "<b>", "</b>"
                  "<em>", "</em>"
                  "<br />"
                  "<sup>", "</sup>"
                  <span class="Colored">
                  </span>
            */

            string text = SecurityElement.Escape(sbText.ToString());
            text= RemoveAll(sbText.ToString(), "&lt;", "<");
            text = RemoveAll(text, "&gt;", "<");
            text = RemoveAll(text, "<span class=\"Colored\">", "");
            text = RemoveAll(text, "<span class=\"SCaps\">", "");
            return RemoveAll(text, "</span>", "");
        }


        /// <summary>
        /// Returns the text split in identifies parts to create a WPF Inline
        /// </summary>
        /// <returns></returns>
        public List<UbTextTag> Tags(bool useReduced= false)
        {
 
            List<UbTextTag> list = new List<UbTextTag>();
            string text = useReduced? GetReducedText(): GetHtml();
            foreach (HtmlTag tag in HtmlTags)
            {
                //text = Regex.Replace(text, "\\b" + string.Join("\\b|\\b", tag) + "\\b", highStart + tag + highEnd);
                text = Regex.Replace(text, tag.Start, tag.MarkStart);
                text = Regex.Replace(text, tag.End, tag.MarkEnd);
            }
            string[] parts= text.Split(HtmlTag.Separators, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(string part in parts)
            {
                text = part;
                if (text.StartsWith(HtmlTag.indicatorTag))
                {
                    text = text.Replace(HtmlTag.indicatorTag, "");
                    TextTag textTag = (TextTag)Convert.ToInt32(text.Substring(0, 2));
                    list.Add(new UbTextTag(textTag, text.Remove(0, 2)));
                }
                else
                {
                    list.Add(new UbTextTag(TextTag.Normal, text));
                }
            }
            return list;
        }


    }
}
