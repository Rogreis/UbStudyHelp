using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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

            public override string ToString()
            {
                return Text;
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


        //private string CalculateFontSize(int AddToSize)
        //{
        //    return (Convert.ToInt16(App.ParametersData.Appearance.FontSizeInfo) + 4 + AddToSize).ToString() + "px";
        //}


        //private string FonteFamilyInfo
        //{
        //    get
        //    {
        //        return App.ParametersData.Appearance.FontFamilyInfo;
        //    }
        //}

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


        private void linkForReference(InlineCollection Inlines, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            int ind = line.IndexOf("###");
            line= line.Remove(ind, 3);
            string reference = line.Replace(':', ';');
            reference = reference.Replace('.', ';');

            Run run = new Run(line)
            {
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Blue
            };

            Hyperlink hyperlink = new Hyperlink(run)
            {
                NavigateUri = new Uri("about:blank"),
                TextDecorations = TextDecorations.Underline
            };
            hyperlink.Tag = reference;

            hyperlink.Click += Hyperlink_Click;
            //hyperlink.RequestNavigate += Hyperlink_RequestNavigate;

            Inlines.Add(hyperlink);


        }

        private void PrepareInLine(IndexDetails detail, TextBlock tb)
        {
            // First all references are changed to a know mark
            string details = detail.Details;

            foreach (string reference in detail.References.Distinct().ToList())
            {
                details= details.Replace(reference, $"»»»###{reference}»»»");
            }


            char[] separators = { '\r', '\n' };
            string[] lines = details.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] separatorsForLine = { "»»»" };
                string[] lineParts = line.Split(separatorsForLine, StringSplitOptions.RemoveEmptyEntries);

                foreach (string linePart in lineParts)
                {
                    if (linePart.IndexOf("###") >= 0)
                    {
                        linkForReference(tb.Inlines, linePart);
                    }
                    else
                    {
                        Run run = new Run(linePart)
                        {
                            FontWeight = FontWeights.Bold
                        };
                        tb.Inlines.Add(run);
                    }
                }
                tb.Inlines.Add(new LineBreak());
            }
        }



        public void ShowResults(string indexEntry, TextBlock tb)
        {
            IndexDetails detail = Indexes.Details.Find(d => string.Compare(d.Text, indexEntry) == 0);

            tb.Inlines.Clear();
            if (detail == null)
            {
                tb.Inlines.Add($"Index entry not found: {indexEntry}");
            }
            else
            {
                PrepareInLine(detail, tb);
                tb.Inlines.Add(new LineBreak());
                tb.Inlines.Add(new LineBreak());
            }
        }



        //private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        //{
        //}

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            string reference = (string)hyperlink.Tag;
            char[] separators = { ';' };
            string[] parts = reference.Split(separators);
            short Paper = -1;
            short.TryParse(parts[0], out Paper);
            short Section = -1;
            short.TryParse(parts[1], out Section);
            short Paragraph = -1;
            short.TryParse(parts[2], out Paragraph);

            TOC_Entry entry = new TOC_Entry(Paper, Section, Paragraph);
            EventsControl.FireIndexClicked(entry);
            hyperlink.Foreground = Brushes.Red; 
        }
    }
}
