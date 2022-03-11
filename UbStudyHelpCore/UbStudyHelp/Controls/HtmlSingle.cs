using System.Collections.Generic;
using System.Text;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Shows a single translations
    /// </summary>
    public class HtmlSingle : Html_BaseClass
    {

        protected override void IdentedLine(StringBuilder sb, string LeftText, string RightText)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine(" <td " + Bloquote() + " width= \"50%\" valign=\"top\"><p>" + LeftText + "</p></td>");
            sb.AppendLine("</tr>");
        }

        protected override void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine(" <td width= \"50%\" valign=\"top\"><p>" + LeftText + "</p></td>");
            sb.AppendLine("</tr>");
        }

        public override string Html(TOC_Entry entry, bool shouldHighlightText, List<string> Words = null)
        {
            Paper paperLeft = StaticObjects.Book.LeftTranslation.Paper(entry.Paper);
            StringBuilder sb = new StringBuilder();

            PageHeader(sb, entry, paperLeft.Title);

            HtmlSingleLine(sb, "<h1>" + StaticObjects.Book.LeftTranslation.PaperTranslation + " " + entry.Paper.ToString() + "</h1>", "<h1>" + StaticObjects.Book.RightTranslation.PaperTranslation +
                " " + entry.Paper.ToString() + "</h1>");

            int indParagraph = 0;
            foreach (UbStandardObjects.Objects.Paragraph parLeft in paperLeft.Paragraphs)
            {
                indParagraph++;

                string TextLeft = parLeft.Text;
                string cssClass = "";
                if (shouldHighlightText)
                {
                    if (parLeft.Entry == entry)
                    {
                        cssClass = " class=\"border-highlight\"";
                        TextLeft = HighlightWords(TextLeft, Words);
                    }
                }

                sb.AppendLine("<tr>");
                switch (parLeft.Format)
                {
                    case enHtmlType.BookTitle:
                        HtmlSingleLine(sb, "<h1>" + TextLeft + "</h1>", null);
                        break;
                    case enHtmlType.PaperTitle:
                        HtmlSingleLine(sb, "<h2>" + TextLeft + "</h2>", null);
                        break;
                    case enHtmlType.SectionTitle:
                        sb.AppendLine("<td width= \"50%\" valign=\"top\">");
                        sb.AppendLine("<div class=\"c1\">");
                        sb.AppendLine($"<h3><a name=\"{parLeft.AName}\"></a>{TextLeft}</h3>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</td>");
                        break;
                    case enHtmlType.NormalParagraph:
                        if (StaticObjects.Parameters.ShowParagraphIdentification)
                        {
                            TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                        }
                        HtmlSingleLine(sb, TextLeft, null);
                        break;
                    case enHtmlType.IdentedParagraph:
                        if (StaticObjects.Parameters.ShowParagraphIdentification)
                        {
                            TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                        }
                        IdentedLine(sb, TextLeft, null);
                        break;
                }
                sb.AppendLine("</tr>");

            }

            PageFooter(sb);
            return sb.ToString();
        }


    }
}
