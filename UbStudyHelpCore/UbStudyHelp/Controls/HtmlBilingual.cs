using System.Collections.Generic;
using System.Text;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Controls
{
    public class HtmlBilingual : Html_BaseClass
    {
        #region Html

        protected override void IdentedLine(StringBuilder sb, string LeftText, string RightText)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine(" <td " + Bloquote() + " width= \"50%\" valign=\"top\"><p>" + LeftText + "</p></td>");
            sb.AppendLine(" <td " + Bloquote() + " width= \"50%\" valign=\"top\"><p>" + RightText + "</p></td>");
            sb.AppendLine("</tr>");
        }

        protected override void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine(" <td width= \"50%\" valign=\"top\"><p>" + LeftText + "</p></td>");
            sb.AppendLine(" <td width= \"50%\" valign=\"top\"><p>" + RightText + "</p></td>");
            sb.AppendLine("</tr>");
        }

        public override string Html(TOC_Entry entry, bool shouldHighlightText, List<string> Words = null)
        {
            Paper paperLeft = StaticObjects.Book.LeftTranslation.Paper(entry.Paper);
            Paper paperRight = StaticObjects.Book.RightTranslation.Paper(entry.Paper);
            StringBuilder sb = new StringBuilder();

            PageHeader(sb, entry, paperLeft.Title);

            HtmlSingleLine(sb, "<h1>" + StaticObjects.Book.LeftTranslation.PaperTranslation + " " + entry.Paper.ToString() + "</h1>", "<h1>" + StaticObjects.Book.RightTranslation.PaperTranslation +
                " " + entry.Paper.ToString() + "</h1>");

            int indParagraph = 0;
            foreach (UbStandardObjects.Objects.Paragraph parLeft in paperLeft.Paragraphs)
            {
                UbStandardObjects.Objects.Paragraph parRight = paperRight.Paragraphs[indParagraph];
                indParagraph++;

                string TextLeft = parLeft.Text;
                string TextRight = parRight.Text;
                string cssClass = "";
                if (shouldHighlightText)
                {
                    if (parLeft.Entry == entry)
                    {
                        cssClass = " class=\"border-highlight\"";
                        TextLeft = HighlightWords(TextLeft, Words);
                        TextRight = HighlightWords(TextRight, Words);
                    }
                }

                sb.AppendLine("<tr>");
                switch (parLeft.Format)
                {
                    case enHtmlType.BookTitle:
                        HtmlSingleLine(sb, "<h1>" + TextLeft + "</h1>", "<h1>" + TextRight + "</h1>");
                        break;
                    case enHtmlType.PaperTitle:
                        HtmlSingleLine(sb, "<h2>" + TextLeft + "</h2>", "<h2>" + TextRight + "</h2>");
                        break;
                    case enHtmlType.SectionTitle:
                        sb.AppendLine("<td width= \"50%\" valign=\"top\">");
                        sb.AppendLine("<div class=\"c1\">");
                        sb.AppendLine($"<h3><a name=\"{parLeft.AName}\"></a>{TextLeft}</h3>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</td>");

                        sb.AppendLine("<td width= \"50%\" valign=\"top\">");
                        sb.AppendFormat("<h3>{0}</h3>", "<h3>" + TextRight + "</h3>");
                        sb.AppendLine("</td>");
                        break;
                    case enHtmlType.NormalParagraph:
                        if (StaticObjects.Parameters.ShowParagraphIdentification)
                        {
                            TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                            TextRight = $"<p{cssClass}><sup>{parLeft.Identification}</sup></a> {TextRight}</p>";
                        }
                        HtmlSingleLine(sb, TextLeft, TextRight);
                        break;
                    case enHtmlType.IdentedParagraph:
                        if (StaticObjects.Parameters.ShowParagraphIdentification)
                        {
                            TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                            TextRight = $"<p{cssClass}><sup>{parLeft.Identification}</sup></a> {TextRight}</p>";
                        }
                        IdentedLine(sb, TextLeft, TextRight);
                        break;
                }
                sb.AppendLine("</tr>");

            }

            PageFooter(sb);
            return sb.ToString();

        }

        #endregion
    }
}
