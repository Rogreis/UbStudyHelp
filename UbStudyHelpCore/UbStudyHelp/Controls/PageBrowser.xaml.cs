using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.ContextMenuCode;
using Paragraph = UbStandardObjects.Objects.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for PageBrowser.xaml
    /// </summary>
    public partial class PageBrowser : UserControl
    {

        private bool lastShouldHighlightText = false;

        private FlowDocumentFormat format = new FlowDocumentFormat();

        // Last highlighted paragraph to force it to ne shown in the screen
        private System.Windows.Documents.Paragraph currentParagraph = null;

        /// <summary>
        /// Annootations is set for the page document scroll object, then must be global to the module
        /// </summary>
        // ANNOTATIONS REMOVAL
        //private UbAnnotations UbAnnotationsObject = new UbAnnotations(UbAnnotationType.Paper);

        public PageBrowser()
        {
            InitializeComponent();
            Loaded += PageBrowser_Loaded;
            TextFlowDocument.LayoutUpdated += TextFlowDocument_LayoutUpdated;

            EventsControl.TOCClicked += EventsControl_TOCClicked;
            EventsControl.TrackSelected += EventsControl_TrackSelected;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.SearchClicked += EventsControl_SearchClicked;
            EventsControl.RefreshText += EventsControl_RefreshText;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.TranslationsChanged += EventsControl_TranslationsChanged;
            EventsControl.BilingualChanged += EventsControl_BilingualChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;

            TextFlowDocument.ContextMenu = new UbParagraphContextMenu(TextFlowDocument, null, false, true);

        }



        /// <summary>
        /// Create and show html for a full paper
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="addToTrack"></param>
        private void Show(TOC_Entry entry, bool shouldHighlightText = true, List<string> Words = null)
        {
            StaticObjects.Parameters.Entry = entry;
            // ANNOTATIONS REMOVAL
            //UbAnnotationsObject.StopAnnotations();
            //UbAnnotationsObject.StartAnnotations(TextFlowDocument, entry);
            ShowShowBilingualFlowDocument(entry, shouldHighlightText, Words);
            EventsControl.FireNewPaperShown();
        }

        /// <summary>
        /// Common paragrph creation
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="highlighted"></param>
        /// <returns></returns>
        private System.Windows.Documents.Paragraph CreateParagraph(TOC_Entry entry, bool highlighted)
        {
            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph()
            {
                Padding = new Thickness(5),
                //ContextMenu = new UbParagraphContextMenu(TextFlowDocument, entry, true, true),
                Tag = entry,
            };

            if (highlighted)
            {
                paragraph.BorderThickness = new Thickness(1);
                paragraph.BorderBrush = App.Appearance.GetHighlightColorBrush();
                currentParagraph = paragraph;
            };

            paragraph.Style = App.Appearance.ForegroundStyle;
            paragraph.Margin = new Thickness(20, 10, 10, 0);
            paragraph.LineHeight = 26;
            paragraph.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            return paragraph;
        }

        private Hyperlink ReferenceAsHyperlink(TOC_Entry entry, bool includePage)
        {
            Hyperlink hyperlink = format.HyperlinkReference(entry, true);
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            hyperlink.MouseEnter += Hyperlink_MouseEnter;
            hyperlink.MouseLeave += Hyperlink_MouseLeave;
            return hyperlink;
        }

        #region events for hyperlink
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
            EventsControl.FireTOCClicked(entry);

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor(2));
            var run = hyperlink.Inlines.FirstOrDefault() as Bold;
            if (run != null)
            {
                run.Foreground = accentBrush;
            }
        }
        #endregion

        private void FormatParagraph(TableCell cell,
                                     TOC_Entry entry, string text,
                                     bool highlighted = false,
                                     List<string> Words = null)
        {
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(entry, highlighted);
            cell.Blocks.Add(paragraph);
            paragraph.Tag = cell.Tag;
            paragraph.Inlines.Add(ReferenceAsHyperlink(entry, true));
            paragraph.Inlines.Add(new Run(" "));
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }

        private void FormatIdent(TableCell cell,
                                     TOC_Entry entry, string text,
                                     bool highlighted = false,
                                     List<string> Words = null)
        {
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(entry, highlighted);
            cell.Blocks.Add(paragraph);
            paragraph.Margin = new Thickness(50, 20, 20, 0);
            paragraph.Tag = cell.Tag;
            paragraph.Inlines.Add(ReferenceAsHyperlink(entry, true));
            paragraph.Inlines.Add(new Run(" "));
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }


        private void FormatTitle(TableCell cell,
                                 TOC_Entry entry, string text,
                                 bool highlighted = false,
                                 List<string> Words = null)
        {
            Brush accentBrush = App.Appearance.GetHighlightColorBrush();
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(entry, highlighted);
            paragraph.FontWeight = FontWeights.Bold;
            paragraph.Foreground = accentBrush;
            paragraph.Tag = cell.Tag;
            cell.Blocks.Add(paragraph);
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }



        private TableRow HtmlSingleBilingualLine(TableRowGroup tableRowGroup, TOC_Entry entryLeft, TOC_Entry entryRight, string LeftText, string RightText,
                                             ParagraphHtmlType htmlType = ParagraphHtmlType.NormalParagraph,
                                             bool highlighted = false,
                                             List<string> words = null)
        {
            TableRow row = new TableRow();
            tableRowGroup.Rows.Add(row);
            row.Tag = entryLeft;
            TableCell cellLeft = new TableCell();
            cellLeft.Tag = new ParagraphSearchData() { IsRightTranslation = false, Entry = entryLeft };
            TableCell cellRight = new TableCell();
            cellRight.Tag = new ParagraphSearchData() { IsRightTranslation = true, Entry = entryRight };
            row.Cells.Add(cellLeft);
            row.Cells.Add(cellRight);


            switch (htmlType)
            {
                case ParagraphHtmlType.BookTitle:
                    FormatTitle(cellLeft, entryLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, entryRight, RightText, highlighted, words);
                    break;
                case ParagraphHtmlType.PaperTitle:
                    FormatTitle(cellLeft, entryLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, entryRight, RightText, highlighted, words);
                    break;
                case ParagraphHtmlType.SectionTitle:
                    FormatTitle(cellLeft, entryLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, entryRight, RightText, highlighted, words);
                    break;
                case ParagraphHtmlType.NormalParagraph:
                    FormatParagraph(cellLeft, entryLeft, LeftText, highlighted, words);
                    FormatParagraph(cellRight, entryRight, RightText, highlighted, words);
                    break;
                case ParagraphHtmlType.IdentedParagraph:
                    FormatIdent(cellLeft, entryLeft, LeftText, highlighted, words);
                    FormatIdent(cellRight, entryRight, RightText, highlighted, words);
                    break;
            }
            return row;
        }


        private void ShowShowBilingualFlowDocument(TOC_Entry entry, bool shouldHighlightText = true, List<string> Words = null)
        {

            Brush accentBrush = App.Appearance.GetHighlightColorBrush();

            Table table = new Table();
            FlowDocument MainDocument = new FlowDocument();
            MainDocument.Blocks.Add(table);
            
            TableRowGroup tableRowGroup = new TableRowGroup();
            table.RowGroups.Add(tableRowGroup);

            Paper paperLeft = StaticObjects.Book.LeftTranslation.Paper(entry.Paper);
            Paper paperRight = StaticObjects.Book.RightTranslation.Paper(entry.Paper);

            string titleLeft = $"{StaticObjects.Book.LeftTranslation.PaperTranslation} {paperLeft.PaperNo}";
            string titleRight = $"{StaticObjects.Book.RightTranslation.PaperTranslation} {paperRight.PaperNo}";

            // Entry informations goes to each paragraph without text
            TOC_Entry entryLeft = TOC_Entry.CreateEntry(entry, StaticObjects.Book.LeftTranslation.LanguageID);
            TOC_Entry entryRight = TOC_Entry.CreateEntry(entry, StaticObjects.Book.RightTranslation.LanguageID);
            entryLeft.Text = entryRight.Text = "";

            HtmlSingleBilingualLine(tableRowGroup, entryLeft, entryRight, titleLeft, titleRight, ParagraphHtmlType.PaperTitle);

            int indParagraph = 0;
            foreach (Paragraph parLeft in paperLeft.Paragraphs)
            {
                Paragraph parRight = null;
                if (indParagraph >= paperRight.Paragraphs.Count)
                {
                    parRight = parLeft.DeepCopy();
                    parRight.Text = "TEXT NOT FOUND";
                }
                else
                {
                    parRight = paperRight.Paragraphs[indParagraph];
                }
                indParagraph++;
                bool highlighted = shouldHighlightText && (parLeft.Entry * entry);
                TableRow row= HtmlSingleBilingualLine(tableRowGroup, parLeft.Entry, parRight.Entry, parLeft.Text, parRight.Text, parLeft.Format, highlighted, Words);
            }
            TextFlowDocument.Tag = entry;
            TextFlowDocument.Document = MainDocument;
        }

        private void TextFlowDocument_LayoutUpdated(object sender, System.EventArgs e)
        {
            if (currentParagraph != null)
            {
                currentParagraph.BringIntoView();
                currentParagraph = null;
            }
        }


        private void Refresh()
        {
            Show(StaticObjects.Parameters.Entry, lastShouldHighlightText);
        }


        private void PageBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Show(StaticObjects.Parameters.Entry);
        }


        private void EventsControl_TOCClicked(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_TrackSelected(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_SearchClicked(TOC_Entry entry, List<string> Words)
        {
            Show(entry, true, Words);
        }

        private void EventsControl_IndexClicked(TOC_Entry entry)
        {
            Show(entry, true, null);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            Show(StaticObjects.Parameters.Entry);
        }


        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            Show(StaticObjects.Parameters.Entry);
        }

        private void EventsControl_TranslationsChanged()
        {
            Refresh();
        }

        private void EventsControl_BilingualChanged(bool ShowBilingual)
        {
            Refresh();
        }

        private void EventsControl_RefreshText()
        {
            Refresh();
        }

        private void TextFlowDocument_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

