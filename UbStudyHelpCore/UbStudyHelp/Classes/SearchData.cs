using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Linq;




namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Used to send/receive data to/from the book search engine
    /// </summary>
    public class SearchData
    {
        private bool OrderedByParagraphs = false;

        public bool Part1Included { get; set; } = false;

        public bool Part2Included { get; set; } = false;

        public bool Part3Included { get; set; } = false;

        public bool Part4Included { get; set; } = false;

        public bool CurrentPaperOnly { get; set; } = false;

        public string QueryString { get; set; } = "";

        public short CurrentPaper { get; set; } = -1;



        // Output data

        public List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

        private List<string> Words { get; set; } = new List<string>();

        public bool IsPaperIncluded(int PaperNo) => (
                    (Part1Included && PaperNo < 32) ||
                    (Part2Included && PaperNo >= 32 && PaperNo <= 56) ||
                    (Part3Included && PaperNo >= 57 && PaperNo <= 119) ||
                    (Part4Included && PaperNo >= 120));


        /// <summary>
        /// Create the word list to be used when highlighting search text found
        /// </summary>
        /// <param name="textPrefix"></param>
        /// <param name="searchString"></param>
        public void SetSearchString(string textPrefix, string searchString)
        {
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
                    int startPos = pos + textPrefix.Length;
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

        /// <summary>
        /// Generate inlines collection from result list
        /// </summary>
        /// <returns></returns>
        public void GetInlinesText(InlineCollection Inlines, int nrPage, int pageSize, int totalPages)
        {
            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            Inlines.Clear();

            Run run = new Run($"({SearchResults.Count}) paragraph(s) found")
            {
                FontWeight = FontWeights.Bold,
                FontSize = App.ParametersData.FontSizeInfo,
                Foreground = accentBrush
            };
            Inlines.Add(run);

            if (SearchResults.Count == 0)
            {
                return;
            }

            Inlines.Add(new LineBreak());
            Run run2 = new Run($"Showing page {nrPage} of {totalPages}")
            {
                FontSize = App.ParametersData.FontSizeInfo,
                Foreground = accentBrush
            };
            Inlines.Add(run2);

            if (SearchResults.Count == 0)
            {
                return;
            }
            Inlines.Add(new LineBreak());
            Inlines.Add(new LineBreak());

            int fistItem = (nrPage - 1) * pageSize;
            if (fistItem >= SearchResults.Count)
            {
                return;
            }
            int lastItem = (nrPage) * pageSize - 1;
            if (lastItem >= SearchResults.Count)
            {
                lastItem = SearchResults.Count - 1;
            }

            for (int i = fistItem; i < lastItem; i++)
            {
                SearchResult result = SearchResults[i];

                // Create hyperlink ony with the paragraph identification
                Run runIdent = new Run(result.Entry.ParagraphID + "  ")
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = App.ParametersData.FontSizeInfo,
                    Foreground = accentBrush
                };

                Hyperlink hyperlink = new Hyperlink(runIdent)
                {
                    NavigateUri = new Uri("about:blank"),
                    TextDecorations = null
                };
                hyperlink.Tag = result.Entry;
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                hyperlink.MouseEnter += Hyperlink_MouseEnter;
                hyperlink.MouseLeave += Hyperlink_MouseLeave;
                Inlines.Add(hyperlink);

                // Paragraph text is inserted
                result.GetInlinesText(Inlines, Words);
                Inlines.Add(new LineBreak());
                Inlines.Add(new LineBreak());
            }

        }


        public void SortResults()
        {
            if (SearchResults.Count == 0)
            {
                return;
            }
            if (!OrderedByParagraphs)
            {
                SearchResults.Sort((a, b) => a.Entry.CompareTo(b.Entry));
                OrderedByParagraphs = true;
            }
            else
            {
                SearchResults.Sort((a, b) => a.OriginalPosition.CompareTo(b.OriginalPosition));
                OrderedByParagraphs = false;
            }
        }

        private void Hyperlink_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = null;
        }

        private void Hyperlink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = TextDecorations.Underline;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            e.Handled = true;
            if (hyperlink.Tag == null)
            {
                return;
            }
            TOC_Entry entry = hyperlink.Tag as TOC_Entry;
            if (entry == null)
            {
                return;
            }

            EventsControl.FireSearchClicked(entry, Words);

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor());
            var run = hyperlink.Inlines.FirstOrDefault() as Run;
            if (run != null)
            {
                run.Foreground = accentBrush;

            }
            hyperlink.Foreground = accentBrush;
        }



    }
}
