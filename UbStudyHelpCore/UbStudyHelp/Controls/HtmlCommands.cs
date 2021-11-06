using System;
using System.Collections.Generic;
using System.Text;
using UbStudyHelp.Classes;
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


    public abstract class HtmlCommands
    {

        // Html colors theme
        private string[] colorTableLight = { "#ffffff", "#000000", "#0066cc", "#b3b3b3", "#ff1aff" };

        // https://material.io/design/color/dark-theme.html#ui-application
        private string[] colorTableDark = { "#121212", "#ffffff", "#BB86FC", "#b3b3b3", "#ff1aff" };

        public string[] ColorTable
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

        public string UbFontColor(FontColorType colorType)
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
            return color.Remove(1,2);
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

        #region Protected Html
        protected virtual void Copyright(StringBuilder sb)
        {
            HtmlSingleLine(sb, Book.LeftTranslation.Copyright, Book.RightTranslation.Copyright);
        }

        protected abstract string IdentedLine(string text);

        protected abstract void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText, bool Ident = false);

        #endregion

        #region public interface

        public System.Drawing.Color GetFontColor(FontColorType colorType)
        {
            return System.Drawing.Color.FromName(UbFontColor(colorType));
        }

        public abstract string HtmlLine(TOC_Entry entry, bool shouldHighlightText);

        #endregion



    }
}
