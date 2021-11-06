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


        //public static string HighlightWords(Paragraph par, Color color)
        //{
        //    string newText = par.Text;
        //    if (Words.Count == 0)
        //    {
        //        return newText;
        //    }

        //    SearchResult result = Findings.Find(l => l.Entry.Paper == par.Paper && l.Entry.Section == par.Section && l.Entry.ParagraphNo == par.ParagraphNo);
        //    if (result == null)
        //    {
        //        return newText;
        //    }


        //    foreach (var replace in Words)
        //    {
        //        int ind = newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase);
        //        if (ind >= 0)
        //        {
        //            string wordInText = newText.Substring(newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase), replace.Length);
        //            string replacement = $"<span style=\"color: {System.Drawing.ColorTranslator.ToHtml(color).Trim()}\">{wordInText}</span>";
        //            newText = Regex.Replace(newText, wordInText, replacement, RegexOptions.IgnoreCase);
        //        }
        //    }

        //    return newText;

        //}



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
                    //    TextLeft = SearchResults.HighlightWords(parLeft, GetFontColor(FontColorType.Highlight));
                    //    TextRight = SearchResults.HighlightWords(parRight, GetFontColor(FontColorType.Highlight));
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
