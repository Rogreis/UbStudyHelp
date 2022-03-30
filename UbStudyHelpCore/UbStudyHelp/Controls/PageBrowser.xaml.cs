﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using Paragraph = UbStandardObjects.Objects.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for PageBrowser.xaml
    /// </summary>
    public partial class PageBrowser : UserControl
    {

        private Html_BaseClass commands = null;

        private bool lastShouldHighlightText = false;

        private FlowDocument MainDocument = new FlowDocument();

        private FlowDocumentFormat format = new FlowDocumentFormat();


        private PaperContextMenu PaperContext = null;


        public PageBrowser()
        {
            InitializeComponent();
            this.Loaded += PageBrowser_Loaded;
            PaperContext = new PaperContextMenu(TextFlowDocument);

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
            if (StaticObjects.Parameters.ShowBilingual)
            {
                commands = new HtmlBilingual();
            }
            else
            {
                commands = new HtmlSingle();
            }
            StaticObjects.Parameters.Entry = entry;
            ShowShowBilingualFlowDocument(entry, shouldHighlightText, Words);
            EventsControl.FireNewPaperShown();
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
            paragraph.Margin = new Thickness(20, 10, 10, 0);
            paragraph.LineHeight = 26;
            paragraph.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            return paragraph;
        }



        private void FormatParagraph(TableCell cell,
                                     TOC_Entry entry, string text,
                                     bool highlighted = false,
                                     List<string> Words = null)
        {
            System.Windows.Documents.Paragraph paragraph = CreateParagraph(highlighted);
            cell.Blocks.Add(paragraph);
            paragraph.Tag = cell.Tag;
            paragraph.Inlines.Add(format.ParagraphIdentification(entry, true));
            paragraph.Inlines.Add(new Run(" "));
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
            paragraph.Tag = cell.Tag;
            paragraph.Inlines.Add(format.ParagraphIdentification(entry, true));
            paragraph.Inlines.Add(new Run(" "));
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
            paragraph.Tag = cell.Tag;
            cell.Blocks.Add(paragraph);
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines, Words);
        }



        private void HtmlSingleBilingualLine(TableRowGroup tableRowGroup, TOC_Entry entry, string LeftText, string RightText,
                                             enHtmlType htmlType = enHtmlType.NormalParagraph,
                                             bool highlighted = false,
                                             List<string> words = null)
        {
            TableRow row = new TableRow();
            tableRowGroup.Rows.Add(row);
            row.Tag = entry;
            TableCell cellLeft = new TableCell();
            cellLeft.Tag = new ParagraphSearchData() { IsRightTranslation = false, Entry = entry };
            TableCell cellRight = new TableCell();
            cellRight.Tag = new ParagraphSearchData() { IsRightTranslation = true, Entry = entry };
            row.Cells.Add(cellLeft);
            row.Cells.Add(cellRight);


            switch (htmlType)
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
            
            Brush accentBrush = App.Appearance.GetHighlightColorBrush();

            Table table = new Table();
            MainDocument.Blocks.Add(table);
            TableRowGroup tableRowGroup = new TableRowGroup();
            table.RowGroups.Add(tableRowGroup);

            Paper paperLeft = StaticObjects.Book.LeftTranslation.Paper(entry.Paper);
            Paper paperRight = StaticObjects.Book.RightTranslation.Paper(entry.Paper);

            string titleLeft = StaticObjects.Book.LeftTranslation.PaperTranslation.Replace("1", paperLeft.PaperNo.ToString());
            string titleRight = StaticObjects.Book.RightTranslation.PaperTranslation.Replace("1", paperRight.PaperNo.ToString());

            HtmlSingleBilingualLine(tableRowGroup, null, titleLeft, titleRight, enHtmlType.PaperTitle);

            int indParagraph = 0;
            foreach (Paragraph parLeft in paperLeft.Paragraphs)
            {
                Paragraph parRight = paperRight.Paragraphs[indParagraph];
                indParagraph++;
                bool highlighted = shouldHighlightText && (parLeft.Entry == entry);
                HtmlSingleBilingualLine(tableRowGroup, parLeft.Entry, parLeft.Text, parRight.Text, parLeft.Format, highlighted, Words);
            }
            TextFlowDocument.Tag = entry;
            TextFlowDocument.Document = MainDocument;

            if (entry != null && tableRowGroup != null)
            {
                //Paragraph pragraph1 = Annotation / Textrange / Paragraph, etc...  
                //paragraph1.BringIntoView();
                TableRow row = tableRowGroup.Rows.ToList().Find(r => (r.Tag as TOC_Entry) == entry);
                
                if (row != null)
                {
                    if (row.IsLoaded)
                    {
                        row.BringIntoView();
                    }
                    else
                    {
                        row.Loaded += Row_Loaded;
                    }
                }
            }


        }

        private void Row_Loaded(object sender, RoutedEventArgs e)
        {
            TableRow row = (sender as TableRow);
            System.Windows.Documents.Paragraph paragraph = row.Cells[0].Blocks.FirstBlock as System.Windows.Documents.Paragraph;
            if (paragraph != null)
            {
                paragraph.BringIntoView();
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

