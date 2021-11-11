using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Stores a search result
    /// </summary>
    public class SearchResult
    {
        private const string highStart = "|~S~|";

        private const string highEnd = "|~E~|";

        public TOC_Entry Entry { get; set; }

        public string Text { get; set; }

        public int OriginalPosition { get; set; } = -1;

        public SearchResult(TOC_Entry entry, string text)
        {
            Entry = entry;
            Text = text;
        }

        /// <summary>
        /// Get text start
        /// </summary>
        private string TextStart
        {
            get
            {
                TextWork work = new TextWork(Text);
                string htmlText = work.DecodedText;
                const int maxCount = 80;
                int maxCharCount = htmlText.Length < maxCount ? htmlText.Length : maxCount;
                int size = htmlText.IndexOf(' ', maxCharCount);
                if (size < 0)
                {
                    size = maxCharCount;
                }
                return htmlText.Length < maxCharCount ? htmlText : htmlText.Substring(0, size);
            }
        }




        private void Convert(InlineCollection Inlines, List<string> Words)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

                string cleaned = Text;
                foreach (string word in Words)
                {
                    cleaned = Regex.Replace(cleaned, "\\b" + string.Join("\\b|\\b", word) + "\\b", "");
                }

                string escapedXml = SecurityElement.Escape(Text);

                while (escapedXml.IndexOf(highStart) != -1)
                {
                    //up to highStart is normal
                    Inlines.Add(new Run(escapedXml.Substring(0, escapedXml.IndexOf(highStart))));
                    //between highStart and highEnd is highlighted
                    Inlines.Add(new Run(escapedXml.Substring(escapedXml.IndexOf(highStart) + 5,
                                              escapedXml.IndexOf(highEnd) - (escapedXml.IndexOf(highStart) + 5)))
                    {
                        FontSize = App.ParametersData.FontSizeInfo,
                        FontWeight = FontWeights.Bold,
                        Background = accentBrush
                    });

                    //the rest of the string (after the highEnd)
                    escapedXml = escapedXml.Substring(escapedXml.IndexOf(highEnd) + 5);
                }

                if (escapedXml.Length > 0)
                {
                    Inlines.Add(new Run(escapedXml));
                }
            }
        }

        /// <summary>
        /// Generate inlines collection from result text, highlighting found words
        /// </summary>
        /// <returns></returns>
        public void GetInlinesText(InlineCollection Inlines, List<string> Words)
        {
            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            if (Words == null || Words.Count == 0)
            {
                Run run = new Run(TextStart)
                {
                    FontSize = App.ParametersData.FontSizeInfo,
                    FontWeight = FontWeights.Bold,
                    Foreground = accentBrush
                };
                Inlines.Add(run);
                return;
            }

            Convert(Inlines, Words);
        }

    }

    //public static class SearchResults
    //{


    //    public static List<SearchResult> Findings { get; set; } = new List<SearchResult>();


    //    private static List<int> AllIndexesOf(string str, string value)
    //    {
    //        List<int> indexes = new List<int>();
    //        for (int index = 0; ; index += value.Length)
    //        {
    //            index = str.IndexOf(value, index, StringComparison.CurrentCultureIgnoreCase);
    //            if (index == -1)
    //                return indexes;
    //            indexes.Add(index);
    //        }
    //    }



    //    public static bool WasFound(TOC_Entry entry)
    //    {
    //        return Findings.Find(f => f.Entry.Paper == entry.Paper && f.Entry.Section == entry.Section && f.Entry.ParagraphNo == entry.ParagraphNo) != null;
    //    }






    //}


}
