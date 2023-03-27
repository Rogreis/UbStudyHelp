using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;

namespace UbStandardObjects.Objects
{

    public class Translation
    {
        protected const string TocTableFileName = "TOC_Table.json";
        public const short NoTranslation = -1;

        public short LanguageID { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public string TIN { get; set; }
        public string TUB { get; set; }
        public string TextButton { get; set; }
        public int CultureID { get; set; }
        public bool UseBold { get; set; }
        public bool RightToLeft { get; set; }
        public int StartingYear { get; set; }
        public int EndingYear { get; set; }
        public string PaperTranslation { get; set; }
        public bool IsEditingTranslation { get; set; } = false;

        public List<Paper> Papers { get; set; } = new List<Paper>();

        #region Non Serializable properties
        /// <summary>
        /// List of available anootations for this translation
        /// </summary>
        [JsonIgnore]
        public List<UbAnnotationsStoreData> Annotations { get; set; } = new List<UbAnnotationsStoreData>();


        [JsonIgnore]
        public List<TOC_Entry> TableOfContents
        {
            get
            {
                List<TOC_Entry> toc = new List<TOC_Entry>();
                foreach (Paper paper in Papers)
                {
                    var paragraphEntries = from p in paper.Paragraphs
                                           where p.ParagraphNo == 0 && !p.IsDivider
                                           orderby p.PK_Seq ascending
                                           select p.Entry;
                    toc.AddRange(paragraphEntries);
                }

                return toc;
            }
        }


        [JsonIgnore]
        public TOC_Table TOC
        {
            get
            {
                TOC_Table toc = new TOC_Table();

                toc.Title = "Lista ded Documentos parea " + Description;

                TOC_Entry intro = GetFirstPartParagraph(0, "Introdução");
                GetPartPapersSections(intro, 0, 0);
                TOC_Entry partI = GetFirstPartParagraph(1, "Parte I");
                GetPartPapersSections(partI, 1, 31);
                TOC_Entry partII = GetFirstPartParagraph(32, "Parte II");
                GetPartPapersSections(partII, 32, 56);
                TOC_Entry partIII = GetFirstPartParagraph(56, "Parte III");
                GetPartPapersSections(partIII, 57, 119);
                TOC_Entry partIV = GetFirstPartParagraph(120, "Parte IV");
                GetPartPapersSections(partIV, 119, 196);

                toc.Parts = new List<TOC_Entry>()
                {
                    intro,
                    partI,
                    partII,
                    partIII,
                    partIV
                };

                return toc;
            }
        }


        [JsonIgnore]
        public string Copyright
        {
            get
            {
                string year = (StartingYear == EndingYear) ? EndingYear.ToString() : StartingYear.ToString() + "," + EndingYear.ToString();
                return "Copyright ©  " + year + " Urantia Foundation. All rights reserved.";
            }
        }

        [JsonIgnore]
        public string Identification
        {
            get
            {
                return LanguageID.ToString() + " - " + Description;
            }
        }

        #endregion

        /// <summary>
        /// Parameter less constructor
        /// </summary>
        public Translation()
        {
        }



        private TOC_Entry GetFirstPartParagraph(short paperNo, string text)
        {
            Paragraph p = (from paper in Papers
                           from par in paper.Paragraphs
                           where par.Paper == paperNo && par.ParagraphNo == 0
                           select par).First();
            return new TOC_Entry(p, text);
        }

        private void GetPartPapersSections(TOC_Entry entry, short startPaperNo, short endPaperNo)
        {
            entry.Papers = (from paper in Papers
                            where paper.PaperNo >= startPaperNo && paper.PaperNo <= endPaperNo
                            orderby paper.PaperNo ascending
                            select paper.Entry).ToList();
            foreach (TOC_Entry entryPaper in entry.Papers)
            {
                entryPaper.Sections = (from paper in Papers
                                       from p in paper.Paragraphs
                                       where p.Paper == entryPaper.Paper && p.ParagraphNo == 0 && p.Section > 0
                                       orderby p.Section ascending
                                       select p.Entry).ToList();
            }
        }

        /// <summary>
        /// Get a paper data for an editing translations
        /// </summary>
        /// <param name="paperNo"></param>
        /// <returns></returns>
        public PaperEdit GetPaperEdit(short paperNo)
        {
            PaperEdit paper = new PaperEdit(paperNo, StaticObjects.Parameters.EditParagraphsRepositoryFolder);
            paper.Paragraphs = new List<Paragraph>();
            Translation EnglishTranslation = StaticObjects.Book.GetTranslation(0);
            string folderPaper = Path.Combine(StaticObjects.Parameters.EditParagraphsRepositoryFolder, $"Doc{paperNo:000}");
            foreach (string filePath in Directory.GetFiles(folderPaper, "*.md"))
            {
                ParagraphMarkDown paragraph = new ParagraphMarkDown(filePath);
                paragraph.FormatInt = EnglishTranslation.GetFormat(paragraph);
                paper.Paragraphs.Add(paragraph);
            }
            return paper;
        }


        /// <summary>
        /// Get all papers data for a non editing translation
        /// </summary>
        /// <param name="jsonString"></param>
        public void GetPapersData(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
            };
            var root = JsonSerializer.Deserialize<JsonRootobject>(jsonString, options);

            if (root.Papers != null)
            {
                Papers = new List<Paper>();
                foreach (JsonPaper jsonPaper in root.Papers)
                {
                    this.Papers.Add(new Paper()
                    {
                        Paragraphs = new List<Paragraph>(jsonPaper.Paragraphs)
                    });
                    // Fix the translation number not set in json file for each paragraph
                    foreach (Paragraph p in jsonPaper.Paragraphs)
                    {
                        p.Entry.TranslationId = LanguageID;
                    }
                }
            }

        }


        /// <summary>
        /// Verify is all mandatory data is ok
        /// </summary>
        /// <returns></returns>
        public bool CheckData()
        {
            try
            {
                StaticObjects.Logger.InInterval(LanguageID, 0, 196, $"Invalid translation number: {LanguageID}");
                StaticObjects.Logger.IsNull(PaperTranslation, $"Paper name is missing for translation {LanguageID}");
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.FatalError($"Fatal error in translation data {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Return a Paper with data
        /// For editing translation the data is got for a paper only direct from repository
        /// </summary>
        /// <param name="paperNo"></param>
        /// <returns></returns>
        public virtual Paper Paper(short paperNo)
        {
            if (IsEditingTranslation)
                 return GetPaperEdit(paperNo);
            else return Papers.Find(p => p.PaperNo == paperNo);
        }

        /// <summary>
        /// Get an annotation object for this entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public UbAnnotationsStoreData GetAnnotation(TOC_Entry entry)
        {
            return Annotations.FirstOrDefault(a => a.Entry == entry);
        }

        /// <summary>
        /// Retuirn the format for an specific paragraph
        /// Get data from English translation
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        public int GetFormat(Paragraph par)
        {
            Paper paper = Paper(par.Paper);
            Paragraph parFound = paper.Paragraphs.Find(p => p.Section == par.Section && p.ParagraphNo == par.ParagraphNo);
            return parFound.FormatInt;
        }


        /// <summary>
        /// Store a created or modified annotation
        /// </summary>
        /// <param name="annotation"></param>
        public void StoreAnnotation(UbAnnotationsStoreData annotation)
        {
            if (Annotations.Exists(a => a.Entry == annotation.Entry))
            {
                Annotations.Remove(Annotations.Find(a => a.Entry == annotation.Entry));
            }
            Annotations.Add(annotation);
        }


        public override string ToString()
        {
            return LanguageID.ToString() + " - " + Description;
        }


    }

    #region Classes to import json file
    internal class JsonRootobject
    {
        public short LanguageID { get; set; }
        public string Description { get; set; }
        public string TIN { get; set; }
        public string TUB { get; set; }
        public int Version { get; set; }
        public string TextButton { get; set; }
        public int CultureID { get; set; }
        public bool UseBold { get; set; }
        public bool RightToLeft { get; set; }
        public short StartingYear { get; set; }
        public short EndingYear { get; set; }
        public string PaperTranslation { get; set; }
        public JsonPaper[] Papers { get; set; }
    }

    internal class JsonPaper
    {
        public Paragraph[] Paragraphs { get; set; }
    }

    internal class TranslationsRoot
    {
        public Translation[] AvailableTranslations { get; set; }
    }


    /// <summary>
    /// Classe used to deserialize json string
    /// </summary>
    public class Translations
    {
        public static List<Translation> DeserializeJson(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            var translationsRoot = JsonSerializer.Deserialize<TranslationsRoot>(jsonString, options);
            List<Translation> list = new List<Translation>();
            list.AddRange(translationsRoot.AvailableTranslations);
            return list;
        }
    }

    #endregion


}
