using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using Hyperlink = System.Windows.Documents.Hyperlink;
using Paragraph = System.Windows.Documents.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for UbStudyBrowser.xaml
    /// </summary>
    public partial class IndexBrowser : UserControl
    {

        private TubIndex currentIndexShown = null;

        public double HeaderHeight { get; set; } = 0;

        public double HeaderWidth { get; set; } = 0;

        public IndexBrowser()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        public void Clear()
        {
            FlowDocument document = new FlowDocument();
            IndexDocumentResults.Document = document;
        }


        /// <summary>
        /// Show the details for some index entry
        /// </summary>
        /// <param name="indexEntry"></param>
        public void CreateWebIndexPage(TubIndex index)
        {
            if (index == null)
            {
                return;
            }
            currentIndexShown = index;

            FlowDocument document = new FlowDocument();

            Section section = new Section();
            section.Style= App.Appearance.ForegroundStyle;
            document.Blocks.Add(section);

            Span spanTitle = new Span();
            Bold boldTitle = new Bold();

            Run rTitle = new Run(index.Title);
            boldTitle.Inlines.Add(rTitle);

            Paragraph pTitle = new Paragraph(boldTitle);
            //pTitle.TextAlignment = TextAlignment.Left;
            section.Blocks.Add(pTitle);

            double margin = 0;
            foreach (IndexDetails detail in index.Details)
            {
                bool isSeeAlso = false;
                string linkText = isSeeAlso ? "See also " : detail.Text + " ";
                if (detail.DetailType >= 100)
                {
                    margin = (detail.DetailType - 100) * 5;
                }
                else
                {
                    margin = 40;
                    isSeeAlso = true;
                }
                Paragraph pDetail = new Paragraph(new Run(linkText));
                pDetail.Margin = new Thickness(margin, 0, 0, 0);
                section.Blocks.Add(pDetail);


                Run rComma = new Run("");
                SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

                foreach (string link in detail.Links)
                {
                    Run rHyper = new Run(link);
                    rHyper.Foreground = accentBrush;
                    Hyperlink hyperlink = new Hyperlink(rHyper);
                    hyperlink.TextDecorations = null;
                    hyperlink.Click += Hyperlink_Click;
                    hyperlink.MouseEnter += Hyperlink_MouseEnter;
                    hyperlink.MouseLeave += Hyperlink_MouseLeave;
                    // Tag for Hyperlinks tag are different from seaqrch tags, because of the "see also"
                    string tag = isSeeAlso ? "»" + link : link.Replace(':', ';').Replace('.', ';');
                    hyperlink.Tag = tag;
                    pDetail.Inlines.Add(rComma);
                    pDetail.Inlines.Add(hyperlink);
                    rComma = new Run(", ");
                }


            }
            IndexDocumentResults.Document = document;
            IndexDocumentResults.IsToolBarVisible = false;
            App.Appearance.SetThemeInfo(IndexDocumentResults);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            e.Handled = true;
            if (hyperlink.Tag == null)
            {
                return;
            }

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor(2));
            var run = hyperlink.Inlines.FirstOrDefault() as Run;
            if (run != null)
            {
                run.Foreground = accentBrush;
            }
            hyperlink.Foreground = accentBrush;

            string tag = (string)hyperlink.Tag;
            if (tag.StartsWith("»"))
            {
                tag = tag.Remove(0, 1); 
                EventsControl.FireOpenNewIndexEntry(tag);
            }
            else
            {
                try
                {
                    char[] sep = { ';' };
                    string[] parts = tag.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                    TOC_Entry entry = new TOC_Entry(Convert.ToInt16(parts[0]), Convert.ToInt16(parts[1]), Convert.ToInt16(parts[2]), 0, 0);
                    EventsControl.FireIndexClicked(entry);
                }
                catch
                { // Ignore errors
                }
            }

        }

        private void Hyperlink_MouseLeave(object sender, MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = null;
        }

        private void Hyperlink_MouseEnter(object sender, MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = TextDecorations.Underline;
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(IndexDocumentResults);
            CreateWebIndexPage(currentIndexShown);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(IndexDocumentResults);
            CreateWebIndexPage(currentIndexShown);
        }


        #region Events

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            SetFontSize();
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ScrollViewer scv = (ScrollViewer)sender;
            //scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            //e.Handled = true;

            //ListView scv = (ListView)sender;
            //scv.Sc .ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            //e.Handled = true;


        }

        #endregion

    }
}
