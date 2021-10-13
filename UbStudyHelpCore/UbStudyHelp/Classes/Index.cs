using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UbStudyHelp;

namespace UrantiaBook.Classes
{
    public class Index
    {
        private string basePath;
        private List<string> FileSufixes = new List<string>();

        private class IndexDetails
        {
            public string Text { get; set; }

            public string Details { get; set; }

            public List<string> References { get; set; } = new List<string>();

            private string linkForReference(string reference)
            {
                if (reference != null)
                {
                    string newReference = reference;
                    newReference = newReference.Replace(':', ';');
                    newReference = newReference.Replace('.', ';');
                    reference = $"<a id=\"{newReference}\" target=\"_blank\" href=\"about:blank\">{reference}</a>";
                }
                return reference;
            }

            public string Html()
            {
                StringBuilder sb = new StringBuilder(Details);
                foreach(string reference in References)
                {
                    sb.Replace(reference, linkForReference(reference));
                }
                return sb.ToString();
            }
        }

        private class IndexTexts
        {
            public List<IndexDetails> Details { get; set; } = new List<IndexDetails>();

            public static IndexTexts operator +(IndexTexts index1, IndexTexts index2)
            {
                index1.Details.AddRange(index2.Details);
                return index1;
            }

        }


        private IndexTexts Indexes = new IndexTexts();

        public Index(string basePathForFiles)
        {
            basePath = basePathForFiles;
            FileSufixes.Add("aa-az");
            FileSufixes.Add("ba-bz");
            FileSufixes.Add("ca-cz");
            FileSufixes.Add("da-dz");
            FileSufixes.Add("ea-ez");
            FileSufixes.Add("fa-fz");
            FileSufixes.Add("ga-gz");
            FileSufixes.Add("ha-hz");
            FileSufixes.Add("ia-iz");
            FileSufixes.Add("ja-jz");
            FileSufixes.Add("ka-kz");
            FileSufixes.Add("la-lz");
            FileSufixes.Add("ma-mz");
            FileSufixes.Add("na-nz");
            FileSufixes.Add("oa-oz");
            FileSufixes.Add("pa-pz");
            FileSufixes.Add("qa-qz");
            FileSufixes.Add("ra-rz");
            FileSufixes.Add("sa-sz");
            FileSufixes.Add("ta-tz");
            FileSufixes.Add("ua-uz");
            FileSufixes.Add("va-vz");
            FileSufixes.Add("wa-wz");
            FileSufixes.Add("xa-xz");
            FileSufixes.Add("ya-yz");
            FileSufixes.Add("za-zz");
        }


        private string CalculateFontSize(int AddToSize)
        {
            return (Convert.ToInt16(App.objParameters.FontSize) + 4 + AddToSize).ToString() + "px";
        }


        private string FontColorForWeb
        {
            get
            {
                return System.Drawing.ColorTranslator.ToHtml(Color.Black);
                //"#" + String.Format("{0,2:X}{1,2:X}{2,2:X}", FontColor.R, FontColor.G, FontColor.B);
            }
        }



        private void Styles(StringBuilder sb)
        {
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(4) + "; color: " + FontColorForWeb + ";}");
            sb.AppendLine("p   {font-family: " + App.objParameters.FontFamily + "; font-size: " + CalculateFontSize(0) + "; color: " + FontColorForWeb + "; margin-top: 10px;}");
            sb.AppendLine(".tit   {font-family: Verdana;	font-size: " + CalculateFontSize(-2) + ";	color: #808080;	margin-top: 10px;	font-style: italic;}");
            sb.AppendLine("h1.title {");
            sb.AppendLine("padding: 6px;");
            sb.AppendLine("font-size: " + CalculateFontSize(20) + ";");
            sb.AppendLine("background: ");
            sb.AppendLine("#009DDC;");
            sb.AppendLine("color: ");
            sb.AppendLine("white;");
            sb.AppendLine("}");
            sb.AppendLine("sup   {font-family: " + App.objParameters.FontFamily + "; font-size: 9px;  color: #808080; vertical-align:top;}");

            sb.AppendLine("a          {font-family: " + App.objParameters.FontFamily + "; font-size:" + CalculateFontSize(0) + "; color: " + FontColorForWeb + "; text-decoration: none;}");
            sb.AppendLine("a:visited  {font-family: " + App.objParameters.FontFamily + "; font-size:" + CalculateFontSize(0) + "; color: " + FontColorForWeb + "; text-decoration: none;}");
            sb.AppendLine("a:hover    {font-family: " + App.objParameters.FontFamily + "; font-size:" + CalculateFontSize(0) + "; color: " + FontColorForWeb + "; text-decoration: underline;}");

            sb.AppendLine("</style>");
        }

        public bool Load()
        {
            try
            {
                if (Indexes.Details.Count == 0)
                {
                    foreach(string sufix in FileSufixes)
                    {
                        string pathFile = Path.Combine(basePath, sufix + ".json");
                        string json = File.ReadAllText(pathFile);
                        IndexTexts index = JsonConvert.DeserializeObject<IndexTexts>(json);
                        Indexes += index;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetAllIndexEntryStartingWith(string startString)
        {
            List<IndexDetails> details = Indexes.Details.FindAll(d => d.Text.StartsWith(startString, StringComparison.CurrentCultureIgnoreCase));
            if (details == null)
                return null;
            return details.Select(d => d.Text).ToList();
            //return details.SelectMany<IndexDetails, string>(d => d.Text);
        }

        public string SearchIndex(string indexEntry)
        {
            IndexDetails detail = Indexes.Details.Find(d => string.Compare(d.Text, indexEntry) == 0);

            Encoding enc = Encoding.UTF8;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=" + enc.WebName + "\" />");
            sb.AppendLine("<title></title>");
            Styles(sb);
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            if (detail == null)
            {
                sb.AppendLine($"Index entry not found: {indexEntry}<br />");
            }
            else
            {
                char[] separators = { '\r', '\n' };
                string[] lines = detail.Html().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines)
                {
                    sb.AppendLine($"<p>{line}</p>");
                }
            }
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("<p>&nbsp;</p>");
            sb.AppendLine("");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();

        }


    }
}
