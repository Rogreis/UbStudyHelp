using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{
    internal class FlowDocumentFormat
    {
        public Run ParagraphIdentification(TOC_Entry entry, bool includePage)
        {
            if (entry != null)
            {
                Brush accentBrush = App.Appearance.GetHighlightColorBrush();
                string id = includePage ? $"{entry.ParagraphID}" : $"{entry.ParagraphIDNoPage}";
                Run runIdent = new Run(id)
                {
                    FontSize = StaticObjects.Parameters.FontSizeInfo - 4,
                    Foreground = accentBrush
                };
                return runIdent;
            }
            return new Run("");
        }


        public Hyperlink HyperlinkFullParagraph(TOC_Entry entry, bool includePage, string text)
        {
            Run runIdent = ParagraphIdentification(entry, includePage);
            
            Hyperlink hyperlink = new Hyperlink(runIdent)
            {
                NavigateUri = new Uri("about:blank"),
                TextDecorations = null,
                Tag = entry
            };

            hyperlink.Inlines.Add(runIdent);
            hyperlink.Inlines.Add(new Run("  "));
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(hyperlink.Inlines);
            return hyperlink;
        }

        public System.Windows.Documents.Paragraph FullParagraph(TOC_Entry entry, bool includePage, string text)
        {
            System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph()
            {
                Padding = new Thickness(5),
                Style = App.Appearance.ForegroundStyle,
            };

            Run runIdent = ParagraphIdentification(entry, includePage);
            paragraph.Inlines.Add(runIdent);
            paragraph.Inlines.Add(new Run("  "));
            TextWork textWork = new TextWork(text);
            textWork.GetInlinesText(paragraph.Inlines);
            return paragraph;
        }


    }
}
