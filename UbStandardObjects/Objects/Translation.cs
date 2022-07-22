using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Linq;

namespace UbStandardObjects.Objects
{

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


    public class Translation
    {
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


        public List<Paper> Papers { get; set; } = new List<Paper>();

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
                                           where p.ParagraphNo == 0
                                           orderby p.PK_Seq ascending
                                           select p.Entry;
                    toc.AddRange(paragraphEntries);
                }
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

        public Translation()
        {
        }

        public void GetData(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
            };
            var root = JsonSerializer.Deserialize<JsonRootobject>(jsonString, options);
            this.LanguageID = root.LanguageID;
            this.Description = root.Description;
            this.TIN = root.TIN;
            this.TUB = root.TUB;
            this.Version = root.Version;
            this.TextButton = root.TextButton;
            this.CultureID = root.CultureID;
            this.UseBold = root.UseBold;
            this.RightToLeft = root.RightToLeft;
            this.StartingYear = root.StartingYear;
            this.EndingYear = root.EndingYear;
            this.PaperTranslation = root.PaperTranslation;

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


        public Paper Paper(short PaperNo)
        {
            return Papers.Find(p => p.PaperNo == PaperNo);
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

    internal class TranslationsRoot
    {
        public Translation[] TranslationsFromSqlServer { get; set; }
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
            list.AddRange(translationsRoot.TranslationsFromSqlServer);
            return list;
        }
    }
    #endregion


}
