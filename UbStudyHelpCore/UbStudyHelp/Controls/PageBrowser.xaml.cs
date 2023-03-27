using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xaml;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using static Lucene.Net.Search.FieldValueHitQueue;
using static Lucene.Net.Util.Packed.PackedInt32s;
using static UbStudyHelp.Classes.FlowDocumentFormat;
using Paragraph = UbStandardObjects.Objects.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for PageBrowser.xaml
    /// </summary>
    public partial class PageBrowser : UserControl
    {

        private bool lastShouldHighlightText = false;

        // Used for specific formating 
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
            EventsControl.RefreshParagraphText += EventsControl_RefreshParagraphText;

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


        private void SetHyperlink(FormatData data)
        {
            if (data.Link != null) 
            {
                data.Link.RequestNavigate += Hyperlink_RequestNavigate;
                data.Link.MouseEnter += Hyperlink_MouseEnter;
                data.Link.MouseLeave += Hyperlink_MouseLeave;
            }
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


        private void FormatParagraph(TableCell cell, FormatData data)
        {
            data.FormatType = ParagraphFormatType.Normal;
            format.FormatParagraph(data);
            data.DocParagraph.Tag = cell.Tag;
            cell.Blocks.Add(data.DocParagraph);
            SetHyperlink(data);
            ((ParagraphSearchData)cell.Tag).DocParagraph = data.DocParagraph;
        }

        private void FormatIdent(TableCell cell, FormatData data)
        {
            data.FormatType = ParagraphFormatType.Idented;
            format.FormatParagraph(data);
            data.DocParagraph.Tag = cell.Tag;
            cell.Blocks.Add(data.DocParagraph);
            SetHyperlink(data);
            ((ParagraphSearchData)cell.Tag).DocParagraph = data.DocParagraph;
        }

        private void FormatCenter(TableCell cell, FormatData data)
        {
            data.FormatType = ParagraphFormatType.Center;
            format.FormatParagraph(data);
            SetHyperlink(data);
            data.DocParagraph.Tag = cell.Tag;
            cell.Blocks.Add(data.DocParagraph);
            ((ParagraphSearchData)cell.Tag).DocParagraph = data.DocParagraph;
        }

        private void FormatTitle(TableCell cell, FormatData data)
        {
            data.FormatType = ParagraphFormatType.Title;
            format.FormatParagraph(data);
            SetHyperlink(data);
            cell.Blocks.Add(data.DocParagraph);
            data.DocParagraph.Tag = cell.Tag;
            ((ParagraphSearchData)cell.Tag).DocParagraph= data.DocParagraph;
        }

        private TableRow HtmlSingleBilingualLine(TableRowGroup tableRowGroup, TOC_Entry entryLeft, TOC_Entry entryRight,
                                             Paragraph leftParagraph, Paragraph rightParagraph,
                                             ParagraphHtmlType htmlType = ParagraphHtmlType.NormalParagraph,
                                             bool highlighted = false,
                                             List<string> words = null)
        {
            TableRow row = new TableRow();
            tableRowGroup.Rows.Add(row);
            row.Tag = entryLeft;
            TableCell cellLeft = new TableCell();
            cellLeft.Tag = new ParagraphSearchData() { IsRightTranslation = false, Entry = entryLeft, Highlighted= highlighted, Words= words };
            TableCell cellRight = new TableCell();
            cellRight.Tag = new ParagraphSearchData() { IsRightTranslation = true, Entry = entryRight, Highlighted = highlighted, Words = words };
            row.Cells.Add(cellLeft);
            row.Cells.Add(cellRight);

            FormatData dataLeft = new FormatData()
            {
                Entry = entryLeft,
                Words = words,
                Status = leftParagraph.IsEditTranslation ? leftParagraph.Status : ParagraphStatus.Closed,
                Highlighted = highlighted,
                IsEditing= leftParagraph.IsEditTranslation,
                Text = leftParagraph.Text,
            };

            FormatData dataRight = new FormatData()
            {
                Entry = entryRight,
                Words = words,
                Status = rightParagraph.IsEditTranslation ? rightParagraph.Status : ParagraphStatus.Closed,
                IsEditing = rightParagraph.IsEditTranslation,
                Highlighted = highlighted,
                Text = rightParagraph.Text,
            };


            if (dataLeft.Text.StartsWith("* * *") || dataLeft.Text.StartsWith("~ ~ ~") ||
                dataRight.Text.StartsWith("* * *") || dataRight.Text.StartsWith("~ ~ ~"))
            {
                dataLeft.Text = "* * * *";
                dataRight.Text = "* * * *";
                FormatCenter(cellLeft, dataLeft);
                FormatCenter(cellRight, dataRight);
                return row;
            }

            switch (htmlType)
            {
                case ParagraphHtmlType.BookTitle:
                case ParagraphHtmlType.PaperTitle:
                case ParagraphHtmlType.SectionTitle:
                    FormatTitle(cellLeft, dataLeft);
                    FormatTitle(cellRight, dataRight);
                    break;
                case ParagraphHtmlType.NormalParagraph:
                    FormatParagraph(cellLeft, dataLeft);
                    FormatParagraph(cellRight, dataRight);
                    break;
                case ParagraphHtmlType.IdentedParagraph:
                    FormatIdent(cellLeft, dataLeft);
                    FormatIdent(cellRight, dataRight);
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

            //HtmlSingleBilingualLine(tableRowGroup, entryLeft, entryRight, titleLeft, titleRight, ParagraphHtmlType.PaperTitle);

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
                TableRow row = HtmlSingleBilingualLine(tableRowGroup, parLeft.Entry, parRight.Entry, parLeft, parRight, parLeft.Format, highlighted, Words);
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

        private void EventsControl_RefreshParagraphText(ParagraphSearchData searchData)
        {
            FormatData data = new FormatData()
            {
                Entry = searchData.Entry,
                Words = searchData.Words,
                Status = searchData.EditedParagraph.Status,
                Highlighted = searchData.Highlighted,
                Text = searchData.EditedParagraph.Text,
                IsEditing= true,
                DocParagraph= searchData.DocParagraph
            };
            format.FormatParagraph(data);
        }

        private void PageBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            //Show(StaticObjects.Parameters.Entry);
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

