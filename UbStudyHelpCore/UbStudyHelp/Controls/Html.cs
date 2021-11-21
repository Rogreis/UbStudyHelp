using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using UbStudyHelp.Classes;
using static Lucene.Net.Search.FieldValueHitQueue;
using static UbStudyHelp.Classes.ControlsAppearance;

namespace UbStudyHelp.Controls
{

    public enum FontColorType
    {
        BackGround = 0,
        FontColor = 1,
        Title = 2,
        Gray,
        Highlight
    }


    public abstract class Html_BaseClass
    {

        // Html colors theme
        private string[] colorTableLight = { "#ffffff", "#000000", "#0066cc", "#b3b3b3", "#ff1aff" };

        // https://material.io/design/color/dark-theme.html#ui-application
        private string[] colorTableDark = { "#121212", "#ffffff", "#BB86FC", "#b3b3b3", "#ff1aff" };

        protected string[] ColorTable
        {
            get
            {
                if (App.ParametersData.ThemeName == "Dark")
                {
                    return colorTableDark;
                }
                return colorTableLight;
            }
        }

        protected string FonteFamilyInfo
        {
            get
            {
                return App.ParametersData.FontFamilyInfo;
            }
        }


        protected string FontSize(int AddToSize)
        {
            return (Convert.ToInt16(App.ParametersData.FontSizeInfo) + 4 + AddToSize).ToString() + "px";
        }

        protected string UbFontColor(FontColorType colorType)
        {
            //string color = ColorTable[(int)colorType];
            MahColorNames mahColorNames = MahColorNames.Highlight;
            switch (colorType)
            {
                case FontColorType.BackGround:
                    mahColorNames = MahColorNames.ThemeBackground;
                    break;
                case FontColorType.FontColor:
                    mahColorNames = MahColorNames.ThemeForeground;
                    break;
                case FontColorType.Highlight:
                    mahColorNames = MahColorNames.Highlight;
                    break;
                case FontColorType.Gray:
                    mahColorNames = MahColorNames.Gray;
                    break;
                case FontColorType.Title:
                    mahColorNames = MahColorNames.Highlight;
                    break;
            }

            string color = App.Appearance.GetColor(mahColorNames);
            return color.Remove(1, 2);
        }

        /// <summary>
        /// Genetates some left space for the idented lines
        /// </summary>
        /// <returns></returns>
        protected string Bloquote()
        {
            return "style=\"padding-left:80px\"";
        }

        protected void Styles(StringBuilder sb)
        {
            string lineHeight = "line-height: 1.8;";

            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(4) + "; background-color: " + UbFontColor(FontColorType.BackGround) + ";}");
            sb.AppendLine("h1   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(10) + "; color: " + UbFontColor(FontColorType.Title) + "; text-align: center; font-style: italic; text-transform: none; font-weight: none;}");
            sb.AppendLine("h2   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(8) + "; color: " + UbFontColor(FontColorType.Title) + "; text-align: center; font-style: none;   text-transform: none; font-weight: none;}");
            sb.AppendLine("h3   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(6) + "; color: " + UbFontColor(FontColorType.Title) + "; text-align: center; font-style: none;   text-transform: none; font-weight: none;}");
            sb.AppendLine("h4   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(4) + "; color: " + UbFontColor(FontColorType.Title) + "; text-align: center; font-style: none;   text-transform: none; font-weight: none; margin-top: 26px;}");

            sb.AppendLine("td   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(1) + "; color: " + UbFontColor(FontColorType.FontColor) + "; text-align: left; font-style: none;  padding:5px 15px; text-transform: none; font-weight: none; " + lineHeight + "}");
            sb.AppendLine("sup  {font-size: " + FontSize(0) + ";  color: #666666;}");

            sb.AppendLine("p   {font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(0) + "; color: " + UbFontColor(FontColorType.FontColor) + "; margin-top: 10px; " + lineHeight + "}");
            sb.AppendLine(".idented {text-indent: 20px; font-family: " + FonteFamilyInfo + "; font-size: " + FontSize(0) + "; color: " + UbFontColor(FontColorType.FontColor) + "; margin-top: 10px; " + lineHeight + "}");

            sb.AppendLine(".copyright {font-family: " + FonteFamilyInfo + "; font-size: \"" + FontSize(-3) + "\"; color: " + UbFontColor(FontColorType.Gray) + "; font-style: italic;}");

            sb.AppendLine("h1.title {");
            sb.AppendLine("padding: 6px;");
            sb.AppendLine("font-size: " + FontSize(20) + ";");
            sb.AppendLine("background: ");
            sb.AppendLine("#009DDC;");
            sb.AppendLine("color: ");
            sb.AppendLine("white;");
            sb.AppendLine("}");

            sb.AppendLine(".border-highlight{ ");
            sb.AppendLine("  border:2px dashed " + UbFontColor(FontColorType.Highlight) + ";   ");
            sb.AppendLine("  padding:0.03em 0.25em; ");
            sb.AppendLine("} ");


            sb.AppendLine(".wordHelp {");
            sb.AppendLine("	font-family: Verdana, Geneva, Tahoma, sans-serif;");
            sb.AppendLine("	border-style: none none dotted none;");
            sb.AppendLine("	border-width: thin;");
            sb.AppendLine(" cursor: pointer;");

            sb.AppendLine("</style>");
        }

        protected void PageHeader(StringBuilder sb, TOC_Entry entry, string title)
        {
            Encoding enc = Encoding.UTF8;

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + enc.WebName + "\">");
            sb.AppendLine("<title>" + title + "</title>");

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
        }

        protected void PageFooter(StringBuilder sb)
        {
            Copyright(sb);
            sb.AppendLine("</table>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
        }

        /// <summary>
        /// Highlight every word 
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        protected string HighlightWords(string htmlText, List<string> Words = null)
        {
            if (Words == null || Words.Count == 0)
            {
                return htmlText;
            }

            string forecolor= System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromName(UbFontColor(FontColorType.BackGround))).Trim();
            string backgroundcolor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromName(UbFontColor(FontColorType.FontColor))).Trim();

            string newText = htmlText;
            foreach (string replace in Words)
            {
                int ind = newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase);
                if (ind >= 0)
                {
                    string wordInText = newText.Substring(newText.IndexOf(replace, StringComparison.CurrentCultureIgnoreCase), replace.Length);
                    string replacement = $"<span style=\"color: {forecolor}; background-color: {backgroundcolor}\">{wordInText}</span>";
                    newText = Regex.Replace(newText, wordInText, replacement, RegexOptions.IgnoreCase);
                }
            }
            return newText;
        }



        protected virtual void Copyright(StringBuilder sb)
        {
            HtmlSingleLine(sb, Book.LeftTranslation.Copyright, Book.RightTranslation.Copyright);
        }

        #region Protected Interface

        protected abstract void IdentedLine(StringBuilder sb, string LeftText, string RightText);

        protected abstract void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText);

        #endregion

        #region public interface

        public abstract string Html(TOC_Entry entry, bool shouldHighlightText, List<string> Words = null);

        #endregion



    }
}
