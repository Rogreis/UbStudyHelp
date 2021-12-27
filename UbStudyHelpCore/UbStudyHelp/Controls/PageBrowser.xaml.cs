using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbStudyHelp.Classes;
using static Lucene.Net.Documents.Field;
using static Lucene.Net.Util.Fst.Util;
using Paragraph = UbStudyHelp.Classes.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for PageBrowser.xaml
    /// </summary>
    public partial class PageBrowser : UserControl
    {

        private Html_BaseClass commands = null;

        private TOC_Entry lastEntry = new TOC_Entry(0, 1, 0);

        private bool lastShouldHighlightText = false;


        public PageBrowser()
        {
            InitializeComponent();
            this.Loaded += PageBrowser_Loaded;

            EventsControl.TOCClicked += EventsControl_TOCClicked;
            EventsControl.TrackSelected += EventsControl_TrackSelected;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.SearchClicked += EventsControl_SeachClicked;
            EventsControl.RefreshText += EventsControl_RefreshText;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.TranslationsChanged += EventsControl_TranslationsChanged;
            EventsControl.BilingualChanged += EventsControl_BilingualChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }



        /// <summary>
        /// Create and show html for a full paper
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="addToTrack"></param>
        private void Show(TOC_Entry entry, bool shouldHighlightText = true, List<string> Words = null)
        {
            if (App.ParametersData.ShowBilingual)
            {
                commands = new HtmlBilingual();
            }
            else
            {
                commands = new HtmlSingle();
            }

            ShowShowBilingualFlowDocument(entry, shouldHighlightText, Words);


            //// Keep latest pragraph shown for next program section
            //App.ParametersData.Entry= new TOC_Entry(entry);
            //lastEntry = new TOC_Entry(entry);
            //lastShouldHighlightText = shouldHighlightText;

            //EventsControl.FireSendMessage(entry.ToString());
            //string htmlPage = commands.Html(entry, shouldHighlightText, Words);
            //BrowserText.NavigateToString(htmlPage);
        }

        private System.Windows.Documents.Paragraph CreateParagraph(bool highlighted)
        {
            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph()
            {
                Padding = new Thickness(5)
            };

            if (highlighted)
            {
                paragraph.BorderThickness = new Thickness(1);
                paragraph.BorderBrush = App.Appearance.GetHighlightColorBrush();
            };


            paragraph.Style = App.Appearance.ForegroundStyle;
            paragraph.Margin = new Thickness(20, 20, 20, 0);
            paragraph.LineHeight = 32;
            paragraph.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            return paragraph;
        }

        private void ShowParagraphIdentification(System.Windows.Documents.Paragraph paragraph, TOC_Entry entry)
        {
            if (entry != null)
            {
                Brush accentBrush = App.Appearance.GetHighlightColorBrush();
                Run runIdent = new Run(entry.ParagraphID + " ")
                {
                    //FontWeight = FontWeights.Bold,
                    FontSize = App.ParametersData.FontSizeInfo,
                    Foreground = accentBrush
                };
                paragraph.Inlines.Add(runIdent);
            }
        }


        private void FormatParagraph(TableCell cell,
                                     TOC_Entry entry, string text,
                                     bool highlighted = false,
                                     List<string> Words = null)
        {
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(highlighted);
            cell.Blocks.Add(paragraph);
            ShowParagraphIdentification(paragraph, entry);
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }

        private void FormatIdent(TableCell cell,
                                     TOC_Entry entry, string text,
                                     bool highlighted = false,
                                     List<string> Words = null)
        {
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(highlighted);
            cell.Blocks.Add(paragraph);
            paragraph.Margin = new Thickness(50, 20, 20, 0);
            ShowParagraphIdentification(paragraph, entry);
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }


        private void FormatTitle(TableCell cell,
                                 string text,
                                 bool highlighted = false,
                                 List<string> Words = null)
        {
            Brush accentBrush = App.Appearance.GetHighlightColorBrush();
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(highlighted);
            paragraph.FontWeight = FontWeights.Bold;
            paragraph.Foreground = accentBrush;
            cell.Blocks.Add(paragraph);
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }


        private void HtmlSingleBilingualLine(Table table, TOC_Entry entry, string LeftText, string RightText,
                                             enHtmlType enHtmlType = enHtmlType.NormalParagraph,
                                             bool highlighted = false,
                                             List<string> words = null)
        {
            TableRow row = new TableRow();
            TableRowGroup tableRowGroup = new TableRowGroup();
            tableRowGroup.Rows.Add(row);
            table.RowGroups.Add(tableRowGroup);
            TableCell cellLeft = new TableCell();
            TableCell cellRight = new TableCell();
            row.Cells.Add(cellLeft);
            row.Cells.Add(cellRight);

            switch (enHtmlType)
            {
                case enHtmlType.BookTitle:
                    FormatTitle(cellLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, RightText, highlighted, words);
                    break;
                case enHtmlType.PaperTitle:
                    FormatTitle(cellLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, RightText, highlighted, words);
                    break;
                case enHtmlType.SectionTitle:
                    FormatTitle(cellLeft, LeftText, highlighted, words);
                    FormatTitle(cellRight, RightText, highlighted, words);
                    break;
                case enHtmlType.NormalParagraph:
                    FormatParagraph(cellLeft, entry, LeftText, highlighted, words);
                    FormatParagraph(cellRight, entry, RightText, highlighted, words);
                    break;
                case enHtmlType.IdentedParagraph:
                    FormatIdent(cellLeft, entry, LeftText, highlighted, words);
                    FormatIdent(cellRight, entry, RightText, highlighted, words);
                    break;
            }
        }


        private void ShowShowBilingualFlowDocument(TOC_Entry entry, bool shouldHighlightText = true, List<string> Words = null)
        {
            FlowDocument document = new FlowDocument();
            Brush accentBrush = App.Appearance.GetHighlightColorBrush();

            Table table = new Table();
            document.Blocks.Add(table);

            Paper paperLeft = Book.LeftTranslation.Paper(entry.Paper);
            Paper paperRight = Book.RightTranslation.Paper(entry.Paper);

            HtmlSingleBilingualLine(table, null, Book.LeftTranslation.PaperTranslation + " " + entry.Paper.ToString(),
                                           Book.RightTranslation.PaperTranslation + " " + entry.Paper.ToString(),
                                           enHtmlType.PaperTitle);

            int indParagraph = 0;
            foreach (Paragraph parLeft in paperLeft.Paragraphs)
            {
                Paragraph parRight = paperRight.Paragraphs[indParagraph];
                indParagraph++;
                bool highlighted = shouldHighlightText && (parLeft.Entry == entry);
                HtmlSingleBilingualLine(table, parLeft.Entry, parLeft.Text, parRight.Text, parLeft.Format, highlighted, Words);
            }
            TextFlowDocument.Document = document;
            table.RowGroups[20].Rows[0].Cells[0].Blocks.FirstBlock.BringIntoView();
        }


        private void Refresh()
        {
            Show(lastEntry, lastShouldHighlightText);
        }


        private void PageBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Show(App.ParametersData.Entry);
        }


        private void EventsControl_TOCClicked(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_TrackSelected(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_SeachClicked(TOC_Entry entry, List<string> Words)
        {
            Show(entry, true, Words);
        }

        private void EventsControl_IndexClicked(TOC_Entry entry)
        {
            Show(entry, true, null);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            Show(App.ParametersData.Entry);
        }


        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            Show(App.ParametersData.Entry);
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



    }
}
