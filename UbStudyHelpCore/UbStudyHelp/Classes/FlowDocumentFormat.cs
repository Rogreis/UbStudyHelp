using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{

    public class ParagraphSearchData
    {
        public bool IsRightTranslation { get; set; } = false;
        public string QueryString = "";
        public TOC_Entry Entry { get; set; }
        public System.Windows.Documents.Paragraph DocParagraph;
        public ParagraphMarkDown EditedParagraph = null;
        public bool Highlighted { get; set; } = false;
        public List<string> Words = null;
    }

    public enum ParagraphFormatType
    {
        Normal,
        Idented,
        Center,
        Track,
        Search,
        Title,
        SectionTitle
    }


    public class FormatData
    {
        public TOC_Entry Entry { get; set; }
        public bool Highlighted { get; set; } = false;
        public bool IncludePage { get; set; } = false;
        public bool IsEditing { get; set; } = false;
        public ParagraphFormatType FormatType { get; set; } = ParagraphFormatType.Normal;
        public string Text { get; set; }
        public ParagraphStatus Status { get; set; }
        public List<string> Words = null;

        // Output data
        public System.Windows.Documents.Paragraph DocParagraph { get; set; } = null;
        public Hyperlink Link { get; set; } = null;
    }


    internal class FlowDocumentFormat
    {

        private Brush TextBrush(FormatData data)
        {
            if (data.IsEditing)
            {
                switch (data.Status)
                {
                    case ParagraphStatus.Started:
                        return Brushes.Black;
                    case ParagraphStatus.Working:
                        return Brushes.Black;
                    case ParagraphStatus.Ok:
                        return Brushes.Black;
                }
            }
            return Brushes.White;
        }

        private void SetColors(FormatData data)
        {
            switch (data.Status)
            {
                case ParagraphStatus.Started:
                    data.DocParagraph.Background = Brushes.FloralWhite;
                    break;
                case ParagraphStatus.Working:
                    data.DocParagraph.Background = Brushes.LemonChiffon;
                    break;
                case ParagraphStatus.Doubt:
                    data.DocParagraph.Background = Brushes.Firebrick;
                    break;
                case ParagraphStatus.Ok:
                    data.DocParagraph.Background = Brushes.Aquamarine;
                    break;
                case ParagraphStatus.Closed:
                    data.DocParagraph.Background = App.Appearance.GetBackgroundColorBrush();
                    break;
            }
            data.DocParagraph.Foreground = TextBrush(data);
        }

   
        public Run ParagraphIdentification(FormatData data)
        {
            if (data.Entry != null)
            {
                Brush accentBrush = App.Appearance.GetHighlightColorBrush();
                string id = data.IncludePage ? $"{data.Entry.ParagraphID}" : $"{data.Entry.ParagraphIDNoPage}";
                Run runIdent = new Run(id)
                {
                    FontSize = StaticObjects.Parameters.FontSize - 4,
                    Foreground = data.IsEditing ? TextBrush(data) : accentBrush
                };
                return runIdent;
            }
            return new Run("");
        }

        /// <summary>
        /// Make the paragraph referece an hyperlink
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="includePage"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Hyperlink HyperlinkReference(FormatData data)
        {
            Run runIdent = ParagraphIdentification(data);
            data.Link= new Hyperlink(runIdent)
            {
                NavigateUri = new Uri("about:blank"),
                TextDecorations = null,
                Tag = data.Entry
            };

            data.Link.Inlines.Add(runIdent);
            data.Link.FontWeight = FontWeights.Bold;
            data.Link.Foreground = TextBrush(data);
            return data.Link;
        }


        /// <summary>
        /// Make the whole paragraph an hiperlink
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="includePage"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public void HyperlinkFullParagraph(FormatData data)
        {
            Run runIdent = ParagraphIdentification(data);

            data.Link = new Hyperlink(runIdent)
            {
                NavigateUri = new Uri("about:blank"),
                TextDecorations = null,
                //TextDecorations = TextDecorations.Underline,
                Tag = data.Entry
            };

            data.Link.Inlines.Add(runIdent);
            data.Link.Inlines.Add(new Run("  "));
            TextWork textWork = new TextWork(data.Text);
            textWork.GetInlinesText(data.Link.Inlines, data.Words);
        }


        public System.Windows.Documents.Paragraph CreateDocParagraph(TOC_Entry entry, bool highlighted, ParagraphStatus status)
        {
            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph()
            {
                Padding = new Thickness(5),
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


        public void FormatParagraph(FormatData data)
        {
            if (data.DocParagraph == null)
            {
                data.DocParagraph = CreateDocParagraph(data.Entry, data.Highlighted, data.Status);
            }
            data.DocParagraph.Inlines.Clear();
            TextWork textWork = new(data.Text);

            switch (data.FormatType)
            {
                case ParagraphFormatType.Normal:
                    data.DocParagraph.Inlines.Add(HyperlinkReference(data));
                    data.DocParagraph.Inlines.Add(new Run("  "));
                    textWork.GetInlinesText(data.DocParagraph.Inlines, data.Words);
                    data.DocParagraph.Foreground = TextBrush(data);
                    break;
                case ParagraphFormatType.Idented:
                    data.DocParagraph.Inlines.Add(HyperlinkReference(data));
                    data.DocParagraph.Inlines.Add(new Run("  "));
                    textWork.GetInlinesText(data.DocParagraph.Inlines, data.Words);
                    data.DocParagraph.Margin = new Thickness(50, 20, 20, 0);
                    data.DocParagraph.Foreground = TextBrush(data);
                    break;
                case ParagraphFormatType.Center:
                    data.DocParagraph.Inlines.Add(HyperlinkReference(data));
                    data.DocParagraph.Inlines.Add(new Run("  "));
                    textWork.GetInlinesText(data.DocParagraph.Inlines, data.Words);
                    data.DocParagraph.TextAlignment = TextAlignment.Center;
                    data.DocParagraph.Foreground = TextBrush(data);
                    break;
                case ParagraphFormatType.SectionTitle:
                case ParagraphFormatType.Title:
                    HyperlinkFullParagraph(data);
                    data.DocParagraph.Inlines.Add(data.Link);
                    data.DocParagraph.Inlines.Add(new Run("  "));
                    data.DocParagraph.FontWeight = FontWeights.Bold;
                    data.DocParagraph.Foreground = TextBrush(data);
                    data.Link.Foreground = TextBrush(data);
                    break;
                case ParagraphFormatType.Search:
                case ParagraphFormatType.Track:
                    HyperlinkFullParagraph(data);
                    data.DocParagraph.Inlines.Add(data.Link);
                    data.DocParagraph.Inlines.Add(new Run("  "));
                    data.DocParagraph.Foreground = TextBrush(data);
                    break;
            }
            SetColors(data);
        }

    }
}
