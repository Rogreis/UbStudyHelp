using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.ContextMenuCode;
using SearchResult = UbStudyHelp.Classes.SearchResult;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbSearchResults.xaml
    /// </summary>
    public partial class UbSearchResults : Page
    {
        private SearchData lastSearchdata = null;
        private int NrPage, PageSize, TotalPages;
        private FlowDocumentFormat format = new FlowDocumentFormat();

        public UbSearchResults()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        private void SetFontSize()
        {
            App.Appearance.SetFontSize(SearchResultsFlowDocument);
            ShowSearchResults(lastSearchdata, NrPage, PageSize, TotalPages);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(SearchResultsFlowDocument);
            ShowSearchResults(lastSearchdata, NrPage, PageSize, TotalPages);
        }


        private System.Windows.Documents.Paragraph ParagraphLineBreak()
        {
            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph();
            paragraph.Inlines.Add(new LineBreak());
            return paragraph;
        }

        public void ShowSearchResults(SearchData data, int nrPage, int pageSize, int totalPages)
        {
            if (data == null)
            {
                return;
            }
            FlowDocument document = new FlowDocument();

            Brush accentBrush = App.Appearance.GetHighlightColorBrush();
            lastSearchdata = data;
            NrPage = nrPage;
            PageSize = pageSize;
            TotalPages = totalPages;

            System.Windows.Documents.Paragraph paragraphTop = new System.Windows.Documents.Paragraph();
            paragraphTop.Style = App.Appearance.ForegroundStyle;
            document.Blocks.Add(paragraphTop);


            if (data.SearchResults.Count == 0)
            {
                paragraphTop.Inlines.Add(new Run("No paragraph found"));
                EventsControl.FireSendMessage("No paragraph found");
                SearchResultsFlowDocument.Document = document;
                App.Appearance.SetFontSize(SearchResultsFlowDocument);
                App.Appearance.SetThemeInfo(SearchResultsFlowDocument);
                return;
            }
            string message = $"Showing page {nrPage} of {totalPages} ({data.SearchResults.Count} paragraph(s) found)";
            EventsControl.FireSendMessage(message);

            Run runTop = new Run($"({data.SearchResults.Count}) paragraph(s) found")
            {
                //FontWeight = FontWeights.Bold,
                FontSize = StaticObjects.Parameters.FontSize,
                Foreground = accentBrush
            };
            paragraphTop.Inlines.Add(runTop);

            paragraphTop.Inlines.Add(new LineBreak());
            Run runPageShown = new Run($"Showing page {nrPage} of {totalPages}")
            {
                FontSize = StaticObjects.Parameters.FontSize,
                Foreground = accentBrush
            };
            paragraphTop.Inlines.Add(runPageShown);


            //document.Blocks.Add(ParagraphLineBreak());
            //document.Blocks.Add(ParagraphLineBreak());


            int fistItem = (nrPage - 1) * pageSize;
            if (fistItem >= data.SearchResults.Count)
            {
                return;
            }
            int lastItem = (nrPage) * pageSize - 1;
            if (lastItem >= data.SearchResults.Count)
            {
                lastItem = data.SearchResults.Count - 1;
            }

            for (int i = fistItem; i <= lastItem; i++)
            {
                SearchResult result = data.SearchResults[i];
                System.Windows.Documents.Paragraph paragraphSearchResult = new System.Windows.Documents.Paragraph
                {
                    Style = App.Appearance.ForegroundStyle,
                    Tag= result.Entry,
                    ContextMenu = new UbParagraphContextMenu(SearchResultsFlowDocument, result.Entry, false, false)
                };
                document.Blocks.Add(paragraphSearchResult);

                Hyperlink hyperlink = format.HyperlinkFullParagraph(result.Entry, false, result.Text, data.Words);
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                hyperlink.MouseEnter += Hyperlink_MouseEnter;
                hyperlink.MouseLeave += Hyperlink_MouseLeave;
                paragraphSearchResult.Inlines.Add(hyperlink);
            }

            SearchResultsFlowDocument.Document = document;
            App.Appearance.SetFontSize(SearchResultsFlowDocument);
            App.Appearance.SetThemeInfo(SearchResultsFlowDocument);

        }


        #region events
        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
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

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
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

            EventsControl.FireSearchClicked(entry, lastSearchdata.Words);

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor(2));
            var run = hyperlink.Inlines.FirstOrDefault() as Run;
            if (run != null)
            {
                run.Foreground = accentBrush;

            }
            hyperlink.Foreground = accentBrush;
        }


        #endregion


        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
        }


    }
}
