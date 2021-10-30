using System;
using System.Collections.Generic;
using System.Text;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    public class HtmlCommandsPage : HtmlCommands
    {
        #region Html

        protected override string IdentedLine(string text)
        {
            return "<table><tr><td width= \"30px\" >&nbsp;</td><td valign=\"top\"><p>" + text + "</p></td></tr></table>";
        }

        protected override void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText, bool Ident = false)
        {
            sb.AppendLine("<tr>");
            if (Ident)
            {
                sb.AppendLine(" <td width= \"50%\" valign=\"top\">" + IdentedLine(LeftText) + "</td>");
                sb.AppendLine(" <td width= \"50%\" valign=\"top\">" + IdentedLine(RightText) + "</td>");
            }
            else
            {
                sb.AppendLine(" <td width= \"50%\" valign=\"top\"><p>" + LeftText + "</p></td>");
                sb.AppendLine(" <td width= \"50%\" valign=\"top\"><p>" + RightText + "</p></td>");
            }
            sb.AppendLine("</tr>");
        }

        public override string HtmlLine(TOC_Entry entry, bool shouldHighlightText)
        {
            Paper paperLeft = Book.LeftTranslation.Paper(entry.Paper);
            Paper paperRight = Book.RightTranslation.Paper(entry.Paper);

            Encoding enc = Encoding.UTF8;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + enc.WebName + "\">");
            sb.AppendLine("<title>" + paperLeft.Title + "</title>");

            Styles(sb);

            if (entry.Section > 0)
            {
                sb.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                sb.AppendLine(" function JumpToSection()");
                sb.AppendLine(" {");
                sb.AppendLine("   window.location.hash = \"" + entry.Anchor + "\"; ");
                sb.AppendLine(" }");
                sb.AppendLine(" JumpToSection();");
                sb.AppendLine("</script>");
            }
            sb.AppendLine("</head>");

            sb.AppendLine("<body>");
            sb.AppendLine("<table>");

            HtmlSingleLine(sb, "<h1>" + Book.LeftTranslation.PaperTranslation + " " + entry.Paper.ToString() + "</h1>", "<h1>" + Book.RightTranslation.PaperTranslation +
                " " + entry.Paper.ToString() + "</h1>");

            int indParagraph = 0;
            foreach (Paragraph parLeft in paperLeft.Paragraphs)
            {
                Paragraph parRight = paperRight.Paragraphs[indParagraph];
                indParagraph++;

                string TextLeft = parLeft.Text;
                string TextRight = parRight.Text;
                string cssClass = "";
                if (shouldHighlightText)
                {
                    if (parLeft.Entry == entry)
                    {
                        cssClass = " class=\"border-highlight\"";
                        TextLeft = SearchResults.HighlightWords(parLeft, UbFontColor(FontColorType.Highlight, true));
                        TextRight = SearchResults.HighlightWords(parRight, UbFontColor(FontColorType.Highlight, true));
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
                        TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                        TextRight = $"<p{cssClass}><sup>{parLeft.Identification}</sup></a> {TextRight}</p>";
                        HtmlSingleLine(sb, TextLeft, TextRight);
                        break;
                    case enHtmlType.IdentedParagraph:
                        //TextLeft = string.Format("<p><a name=\"{0}\"><sup>{1}</sup></a> {2}</p>", oParagraph.AName, oParagraph.Identification, TextLeft);
                        TextLeft = $"<p{cssClass}><a name=\"{parLeft.AName}\"><sup>{parLeft.Identification}</sup></a> {TextLeft}</p>";
                        TextRight = $"<p{cssClass}><sup>{parLeft.Identification}</sup></a> {TextRight}</p>";
                        HtmlSingleLine(sb, TextLeft, TextRight, true);
                        break;
                }
                sb.AppendLine("</tr>");

            }

            Copyright(sb);
            sb.AppendLine("</table>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();

        }

        #endregion
    }
}
