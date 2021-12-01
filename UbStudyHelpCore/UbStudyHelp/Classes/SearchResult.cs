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
            foreach (UbTextTag textTag in TextWork.TagsWithHighlightWords(Words, false))
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
                    case TextTag.Highlighted:
                        Bold bHighlighted = new Bold();
                        bHighlighted.FontSize = App.ParametersData.FontSizeInfo;
                        bHighlighted.Foreground = accentBrush;
                        bHighlighted.Inlines.Add(textTag.Text);
                        Inlines.Add(bHighlighted);
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


}
