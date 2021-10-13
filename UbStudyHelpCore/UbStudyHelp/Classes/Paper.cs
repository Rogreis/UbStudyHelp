using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UbStudyHelp;

namespace UrantiaBook.Classes
{
    public enum enHtmlType
    {
        BookTitle = 0,
        PaperTitle = 1,
        SectionTitle = 2,
        NormalParagraph = 3,
        IdentedParagraph = 4
    }


    public class Paper
    {
        public List<Paragraph> listParagraphs = null;

        private Translation Translation = null;
        private short PaperNo = -1;


        public string Title { get; private set; }



        public Paper(string pathData, Translation Translation, short PaperNo)
        {
            this.Translation = Translation;
            this.PaperNo = PaperNo;
            string BasePathPaper = Path.Combine(pathData, "L" + Translation.LanguageID.ToString("000"));
            string PathXmlFile = Path.Combine(BasePathPaper, "Paper" + PaperNo.ToString("000") + ".xml");

            XElement xElem = XElement.Load(PathXmlFile);

            listParagraphs = new List<Paragraph>();

            foreach (XElement xElemPar in xElem.Descendants("Paragraph"))
            {
                listParagraphs.Add(new Paragraph(xElemPar));
            }

            Paragraph parTitle = listParagraphs.Find(p => p.Section == 0 && p.ParagraphNo == 0);
            Title = parTitle != null? parTitle.Text : "No Title, check your data";

        }


        private string IdentedLine(string text)
        {
            return "<table><tr><td width= \"30px\" >&nbsp;</td><td valign=\"top\"><p>" + text + "</p></td></tr></table>";
        }

        private void HtmlOneLine(StringBuilder sb, string text, bool Ident = false)
        {
            if (Ident)
            {
                //sb.AppendLine("<tr class=\"idented\">");
                sb.AppendLine("<tr><td width= \"100%\" valign=\"top\">" + IdentedLine(text) + "</td></tr>");
            }
            else
            {
                sb.AppendLine("<tr><td width= \"100%\" valign=\"top\"><p>" + text + "</p></td></tr>");
            }
        }

        private void HtmlBilingualLine(StringBuilder sb, string LeftText, string RightText, bool Ident = false)
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

        private short PK_Seq_Searched = -1;

        private bool CheckParagraph(Paragraph par)
        {
            return (par.PK_Seq == PK_Seq_Searched);
        }

        private bool IsStartingWord(string Text, string word, int Position)
        {
            if (Position < 0) return false;
            char[] characters = Text.ToCharArray();
            bool LeftIsOk = true;
            if (Position > 0) LeftIsOk = !char.IsLetterOrDigit(characters[Position - 1]);
            return LeftIsOk;
        }

        private bool IsWord(string Text, string word, int Position)
        {
            if (Position < 0) return false;
            char[] characters = Text.ToCharArray();
            bool LeftIsOk = true, RightIsOk = true;
            if (Position > 0) LeftIsOk = !char.IsLetterOrDigit(characters[Position - 1]);
            if (Position < (Text.Length - word.Length - 1)) RightIsOk = !char.IsLetterOrDigit(characters[Position + word.Length]);
            return LeftIsOk && RightIsOk;
        }

        private void Copyright(StringBuilder sb, Translation oTranslationLeft, Translation oTranslationRight = null)
        {
            ////  short leftLanguageId, short rightLanguageId= -1, int startingYear= 2013, int endingYear= 2013
            //if (oTranslationRight != null)
            //    HtmlBilingualLine(sb, oTranslationLeft.Copyright, oTranslationRight.Copyright);
            //else
            //    HtmlOneLine(sb, oTranslationLeft.Copyright);
        }

        private string CalculateFontSize(int AddToSize)
        {
            return (Convert.ToInt16(App.objParameters.FontSize) + 4 + AddToSize).ToString() + "px";
        }

        private string Styles()
        {
            string lineHeight = "line-height: 1.8;";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(4) + "; color: " + App.objParameters.FontColorForWeb + ";}");
            sb.AppendLine("h1   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(10) + "; color: " + App.objParameters.FontColorForWebTitles + "; text-align: center; font-style: italic; text-transform: none; font-weight: none;}");
            sb.AppendLine("h2   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(8) + "; color: " + App.objParameters.FontColorForWebTitles + "; text-align: center; font-style: none;   text-transform: none; font-weight: none;}");

            // Paper title style:  H3
            sb.AppendLine("h3   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(6) + "; color: " + App.objParameters.FontColorForWebTitles + "; text-align: center; font-style: none;   text-transform: none; font-weight: none;}");

            // Paper numbe and section title style:  H4
            sb.AppendLine("h4   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(4) + "; color: " + App.objParameters.FontColorForWebTitles + "; text-align: center; font-style: none;   text-transform: none; font-weight: none; margin-top: 26px;}");

            sb.AppendLine("td   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(1) + "; color: " + App.objParameters.FontColorForWeb + "; text-align: left; font-style: none;  padding:5px 15px; text-transform: none; font-weight: none; " + lineHeight + "}");
            sb.AppendLine("sup  {font-size: " + CalculateFontSize(0) + ";  color: #666666;}");
            sb.AppendLine(".ColItal {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + App.objParameters.FontColorForWeb + "; font-style: italic;}");
            sb.AppendLine(".Colored {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + App.objParameters.FontColorForWeb + ";}");
            sb.AppendLine(".ppg     {font-family: " + App.objParameters.FontFamily + "; font-size: 9px;  color: " + App.objParameters.FontColorForWeb + ";  vertical-align:top;}");
            sb.AppendLine(".super   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + ";  color: " + App.objParameters.FontColorForWeb + ";  vertical-align:top;}");
            sb.AppendLine(".head4   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + App.objParameters.FontColorForWeb + ";}");
            sb.AppendLine(".head5   {font-family: " + App.objParameters.FontFamily + "; font-size: 18px; color: " + App.objParameters.FontColorForWeb + ";}");
            sb.AppendLine("p   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + App.objParameters.FontColorForWeb + "; margin-top: 10px; " + lineHeight + "}");
            sb.AppendLine(".idented {text-indent: 20px; font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + App.objParameters.FontColorForWeb + "; margin-top: 10px; " + lineHeight + "}");

            sb.AppendLine(".copyright {font-family: " + App.objParameters.FontFamily + "; font-size: \"" + CalculateFontSize(-3) + "\"; color: " + System.Drawing.ColorTranslator.ToHtml(Color.Gray) + "; font-style: italic;}");

            sb.AppendLine("h1.title {");
            sb.AppendLine("padding: 6px;");
            sb.AppendLine("font-size: " + CalculateFontSize(20) + ";");
            sb.AppendLine("background: ");
            sb.AppendLine("#009DDC;");
            sb.AppendLine("color: ");
            sb.AppendLine("white;");
            sb.AppendLine("}");

            sb.AppendLine(".border-highlight{ ");
            sb.AppendLine("  border:2px dashed red;   ");
            sb.AppendLine("  padding:0.03em 0.25em; ");
            sb.AppendLine("} ");


            sb.AppendLine(".wordHelp {");
            sb.AppendLine("	font-family: Verdana, Geneva, Tahoma, sans-serif;");
            sb.AppendLine("	border-style: none none dotted none;");
            sb.AppendLine("	border-width: thin;");
            sb.AppendLine(" cursor: pointer;");



            sb.AppendLine("</style>");
            return sb.ToString();
        }



        public string GetParagraphIdent(BrowserPosition track)
        {
            Paragraph oParagraph = listParagraphs.Find(o => o.Section == track.Entry.Section && o.ParagraphNo == track.Entry.ParagraphNo);
            if (oParagraph != null)
                return oParagraph.ToString();
            return "";
        }



        public string HtmlOnePaper(int paper, int section, int ParagraphNo = 0)
        {
            Encoding enc = Encoding.UTF8;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + enc.WebName + "\" />");
            sb.AppendLine("<title>" + Title + "</title>");
            sb.AppendLine(Styles());


            if (section > 0)
            {
                string Anchor = "U" + paper.ToString() + "_" + section.ToString() + "_" + ParagraphNo.ToString();
                sb.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
                sb.AppendLine(" function JumpToSection()");
                sb.AppendLine(" {");
                sb.AppendLine("   window.location.hash = \"" + Anchor + "\"; ");
                sb.AppendLine(" }");
                sb.AppendLine(" JumpToSection();");
                sb.AppendLine("</script>");
            }

            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<table>");


            HtmlOneLine(sb, "<h1>" + Translation.PaperTranslation + " " + paper.ToString() + "</h1>");


            foreach (Paragraph paragraph in listParagraphs)
            {
                string TextLeft = SearchResults.HighlightWords(paragraph, Color.Fuchsia);


                switch (paragraph.Format)
                {
                    case enHtmlType.BookTitle:
                        HtmlOneLine(sb, "<h1>" + TextLeft + "</h1>");
                        break;
                    case enHtmlType.PaperTitle:
                        HtmlOneLine(sb, "<h1>" + TextLeft + "</h1>");
                        break;
                    case enHtmlType.SectionTitle:
                        TextLeft = "<div class=\"c1\">" + string.Format("<h4><a name=\"U{0}_{1}_{2}\">{3}</a></h4></div>", paragraph.Paper, paragraph.Section, paragraph.ParagraphNo, TextLeft);
                        HtmlOneLine(sb, TextLeft);
                        break;
                    case enHtmlType.NormalParagraph:
                        //               <p><sup>(1.1)    </sup> <a name="U0_0_1">        <sup>0:0.1</sup></a>[3]</p>
                        if (paragraph.ParagraphNo > 0)
                            TextLeft = string.Format("<p><sup>({0}.{1})</sup> <a name=\"U{2}_{3}_{4}\"><sup>{5}:{6}.{7}</sup></a> {8}</p>",
                                  paragraph.Paper, paragraph.Page, paragraph.Paper, paragraph.Section, paragraph.ParagraphNo, paragraph.Paper, paragraph.Section, paragraph.ParagraphNo, TextLeft);
                        HtmlOneLine(sb, TextLeft, false);
                        break;
                    case enHtmlType.IdentedParagraph:
                        TextLeft = string.Format("<sup>({0}.{1})</sup> <a name=\"U{2}_{3}_{4}\"><sup>{5}:{6}.{7}</sup></a> {8}",
                                    paragraph.Paper, paragraph.Page, paragraph.Paper, paragraph.Section, paragraph.ParagraphNo,
                                    paragraph.Paper, paragraph.Section, paragraph.ParagraphNo, TextLeft);
                        HtmlOneLine(sb, TextLeft, true);
                        break;
                }
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");

            }


            // Div used for word glossary
            Copyright(sb, Translation, null);
            sb.AppendLine("</table>");
            sb.AppendLine("<div id=\"div1\" style=\"border-radius: 12px; color: #808080; font-family: verdana, Geneva, Tahoma, sans-serif; font-size: x-small; background-color: #FFFFCC; border: thin dotted #666666; padding: 8px; position: absolute; width: 200px; height: 100px; top: 5px; right: 10px; visibility:hidden\" ></div>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();

        }


        public string HtmlBilingual(Paper rightPaper, TOC_Entry entry, bool shouldHighlightText)
        {
            Encoding enc = Encoding.UTF8;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + enc.WebName + "\">");
            sb.AppendLine("<title>" + Title + "</title>");
            sb.AppendLine(Styles());
            //JavaScriptForGlossary(sb);

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

            HtmlBilingualLine(sb, "<h1>" + Translation.PaperTranslation + " " + entry.Paper.ToString() + "</h1>", "<h1>" + rightPaper.Translation.PaperTranslation + 
                " " + entry.Paper.ToString() + "</h1>");

            foreach (Paragraph paragraph in listParagraphs)
            {
                PK_Seq_Searched = paragraph.PK_Seq;
                Paragraph parLeft = listParagraphs.Find(CheckParagraph);
                Paragraph parRight = rightPaper.listParagraphs.Find(CheckParagraph);
                if (parLeft == null || parRight == null) break;


                string TextLeft = parLeft.Text;
                string TextRight = parRight.Text;
                string cssClass = "";
                if (shouldHighlightText)
                {
                    if (parLeft.Entry == entry)
                    {
                        cssClass = " class=\"border-highlight\"";
                        TextLeft = SearchResults.HighlightWords(parLeft, Color.Fuchsia);
                        TextRight = SearchResults.HighlightWords(parRight, Color.Fuchsia);
                    }
                }

                sb.AppendLine("<tr>");
                switch (paragraph.Format)
                {
                    case enHtmlType.BookTitle:
                        HtmlBilingualLine(sb, "<h1>" + TextLeft + "</h1>", "<h1>" + TextRight + "</h1>");
                        break;
                    case enHtmlType.PaperTitle:
                        HtmlBilingualLine(sb, "<h2>" + TextLeft + "</h2>", "<h2>" + TextRight + "</h2>");
                        break;
                    case enHtmlType.SectionTitle:
                        sb.AppendLine("<td width= \"50%\" valign=\"top\">");
                        sb.AppendLine("<div class=\"c1\">");
                        sb.AppendFormat("<h3><a name=\"U{0}_{1}_{2}\">{3}</a></h3>", paragraph.Paper, paragraph.Section, paragraph.ParagraphNo, "<h3>" + TextLeft + "</h3>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</td>");

                        sb.AppendLine("<td width= \"50%\" valign=\"top\">");
                        sb.AppendFormat("<h3>{0}</h3>", "<h3>" + TextRight + "</h3>");
                        sb.AppendLine("</td>");
                        break;
                    case enHtmlType.NormalParagraph:
                        TextLeft = $"<p{cssClass}><a name=\"{paragraph.AName}\"><sup>{paragraph.Identification}</sup></a> {TextLeft}</p>";
                        TextRight = $"<p{cssClass}><sup>{paragraph.Identification}</sup></a> {TextRight}</p>";
                        HtmlBilingualLine(sb, TextLeft, TextRight);
                        break;
                    case enHtmlType.IdentedParagraph:
                        //TextLeft = string.Format("<p><a name=\"{0}\"><sup>{1}</sup></a> {2}</p>", oParagraph.AName, oParagraph.Identification, TextLeft);
                        TextLeft = $"<p{cssClass}><a name=\"{paragraph.AName}\"><sup>{paragraph.Identification}</sup></a> {TextLeft}</p>";
                        TextRight = $"<p{cssClass}><sup>{paragraph.Identification}</sup></a> {TextRight}</p>";
                        HtmlBilingualLine(sb, TextLeft, TextRight, true);
                        break;
                }
                sb.AppendLine("</tr>");

            }

            Copyright(sb, Translation, rightPaper.Translation);
            sb.AppendLine("</table>");
            // Div used for word glossary
            //sb.AppendLine("<div id=\"div1\" style=\"border-radius: 12px; color: #808080; font-family: verdana, Geneva, Tahoma, sans-serif; font-size: x-small; background-color: #FFFFCC; border: thin dotted #666666; padding: 8px; position: absolute; width: 200px; height: 100px; top: 2000px; right: 10px; visibility:hidden\" ></div>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();

        }







    }
}
