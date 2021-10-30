using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Stores a search result
    /// </summary>
    public class SearchResult
    {
        public TOC_Entry Entry { get; set; }

        public string Text { get; set; }

        public SearchResult(Document doc)
        {
            Entry = new TOC_Entry(Convert.ToInt16(doc.GetField("Paper").GetSingleValue()), Convert.ToInt16(doc.GetField("Section").GetSingleValue()), Convert.ToInt16(doc.GetField("ParagraphNo").GetSingleValue()));
            Text = doc.GetField("Text").GetStringValue();
        }

         public string HtmlText
        {
            get
            {
                const int maxCount = 80;
                int maxCharCount = Text.Length < maxCount ? Text.Length : maxCount;
                int size = Text.IndexOf(' ', maxCharCount);
                if (size < 0)
                {
                    size = maxCharCount;
                }
                string textToShow= Text.Length < maxCharCount ? Text : Text.Substring(0, size);
                return "<p><a id=\"" + Entry.Href + "\" target=\"_blank\" href=\"about:blank\">" + Entry.ToString() + "  " + textToShow + "</a></p>";
            }
        }
    }

    public static class SearchResults
    {
        // Fields for Lucene Index Searc
        public const string FieldPaper = "Paper";
        public const string FieldSection = "Section";
        public const string FieldParagraph = "ParagraphNo";
        public const string FieldText = "Text";

        public static List<string> Words { get; set; } = new List<string>();

        public static List<SearchResult> Findings { get; set; } = new List<SearchResult>();


        private static List<int> AllIndexesOf(string str, string value)
        {
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.CurrentCultureIgnoreCase);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static void Clear()
        {
            Findings.Clear();
            Words.Clear();
        }


        public static bool WasFound(TOC_Entry entry)
        {
            return Findings.Find(f => f.Entry.Paper == entry.Paper && f.Entry.Section == entry.Section && f.Entry.ParagraphNo == entry.ParagraphNo) != null;
        }


        public static void SetSearchString(string searchString)
        {
            string textPrefix = SearchResults.FieldText + ":";
            Words = new List<string>();
            bool continues = true;
            int lastStartPos = 0;

            searchString = searchString.Replace('~', ' ');
            searchString = searchString.Replace('^', ' ');

            while (continues)
            {
                int pos = searchString.IndexOf(textPrefix, lastStartPos);
                continues = pos >= 0 && lastStartPos < searchString.Length;
                if (continues)
                {
                    int startPos= pos + textPrefix.Length;
                    char divisor = searchString.ToCharArray()[startPos] == '"' ? '"' : ' ';
                    if (divisor == '"')
                    {
                        startPos += 1;
                    }
                    int endPos = searchString.IndexOf(divisor, startPos);
                    endPos = (endPos >= 0 ? endPos : searchString.Length);
                    int size = endPos - startPos;
                    Words.Add(searchString.Substring(startPos, size));
                    lastStartPos = endPos + 1;
                }
                else
                {
                    break;
                }
                continues = lastStartPos < searchString.Length;
            }
        }


        public static string HighlightWords(Paragraph par, Color color)
        {
            string newText = par.Text;
            if (Words.Count == 0)
            {
                return newText;
            }

            SearchResult result = Findings.Find(l => l.Entry.Paper == par.Paper && l.Entry.Section == par.Section && l.Entry.ParagraphNo == par.ParagraphNo);
            if (result == null)
            {
                return newText;
            }


            foreach (var replace in Words)
            {
                int ind = newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase);
                if (ind >= 0)
                {
                    string wordInText = newText.Substring(newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase), replace.Length);
                    string replacement = $"<span style=\"color: {System.Drawing.ColorTranslator.ToHtml(color).Trim()}\">{wordInText}</span>";
                    newText = Regex.Replace(newText, wordInText, replacement, RegexOptions.IgnoreCase);
                }
            }

            return newText;

        }



    }


}
