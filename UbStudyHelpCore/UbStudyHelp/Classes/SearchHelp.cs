using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbStudyHelp;

namespace UrantiaBook.Classes
{
    /// <summary>
    /// Read the a md file and convert it to html
    /// Uses nuget CommonMarkConverter
    /// </summary>
    public class SearchHelp
    {

        private string CalculateFontSize(int AddToSize)
        {
            return (Convert.ToInt16(App.ParametersData.Appearance.FontSizeInfo) + 4 + AddToSize).ToString() + "px";
        }


        private void Styles(StringBuilder sb)
        {
            sb.AppendLine("<style> ");
            sb.AppendLine(" ");
            sb.AppendLine("h1  {font-family:Verdana; font-size:20px; color:Blue; margin-top: 10px;line-height: 1.8; }  ");
            sb.AppendLine("h2  {font-family:Verdana; font-size:18px; color:Blue; margin-top: 10px;line-height: 1.8; } ");
            sb.AppendLine("h3  {font-family:Verdana; font-size:16px; color:Orange; margin-top: 10px;line-height: 1.8; } ");
            sb.AppendLine("p   {font-family:Verdana; font-size:14px; color:Black; margin-top: 10px;line-height: 1.8; } ");
            sb.AppendLine("body {font-family:Verdana; font-size:14px; color:Black; margin-top: 10px;line-height: 1.8; } ");
            sb.AppendLine("li   {font-family:Verdana; font-size:14px; color:Black; margin-top: 10px;line-height: 1.8; } ");
            sb.AppendLine(" ");
            sb.AppendLine("</style> ");
        }

        public string Html(string pathHelpFile)
        {
            var cmSettings = CommonMark.CommonMarkSettings.Default.Clone();
            cmSettings.RenderSoftLineBreaksAsLineBreaks = true;
            var result = CommonMark.CommonMarkConverter.Convert(File.ReadAllText(pathHelpFile), cmSettings);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html> ");
            sb.AppendLine("<html> ");
            Styles(sb);
            sb.AppendLine("<body> ");
            sb.AppendLine(result);
            sb.AppendLine("</body> ");
            sb.AppendLine("</html> ");
            return sb.ToString();
        }

    }
}
