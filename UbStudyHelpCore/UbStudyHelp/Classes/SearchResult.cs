using System.Collections.Generic;
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
        private TextWork TextWork = new TextWork();


        private const string highStart = "|~S~|";

        private const string highEnd = "|~E~|";

        public TOC_Entry Entry { get; set; }

        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                TextWork.LoadText(value);
                _text = TextWork.GetHtml();
            }
        }

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
                return TextWork.GetReducedText();
            }
        }




        private void Convert(InlineCollection Inlines, List<string> Words)
        {

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());
            foreach (UbTextTag textTag in TextWork.Tags(false))
            {
                switch (textTag.Tag)
                {
                    case TextTag.Normal:
                        Run runNormal = new Run(textTag.Text);
                        runNormal.FontSize = App.ParametersData.FontSizeInfo;
                        Inlines.Add(runNormal);
                        break;
                    case TextTag.Italic:
                        var i = new Italic();
                        i.FontSize = App.ParametersData.FontSizeInfo;
                        i.Inlines.Add(textTag.Text);
                        Inlines.Add(i);
                        break;
                    case TextTag.Bold:
                        var b = new Bold();
                        b.FontSize = App.ParametersData.FontSizeInfo;
                        b.Inlines.Add(textTag.Text);
                        Inlines.Add(b);
                        break;
                    case TextTag.Superscript:
                        Run runSuper = new Run(textTag.Text);
                        runSuper.FontSize = App.ParametersData.FontSizeInfo;
                        runSuper.BaselineAlignment = BaselineAlignment.Superscript;
                        Inlines.Add(runSuper);
                        break;
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
