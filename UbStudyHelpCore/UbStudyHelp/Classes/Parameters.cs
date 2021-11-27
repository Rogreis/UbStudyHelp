using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UbStudyHelp;
using UbStudyHelp.Classes;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace UbStudyHelp.Classes
{

    [Serializable]
    public class Parameters
    {

        /// <summary>
        /// Last position in the text, default for first paragraph
        /// </summary>
        public TOC_Entry Entry { get; set; } = new TOC_Entry(0, 1, 0);

        public short LanguageIDLeftTranslation { get; set; } = 0;

        public short LanguageIDRightTranslation { get; set; } = 34;

        public int MaxTrackItems { get; set; } = 60;

        public int SearchPageSize { get; set; } = 20;

        public string ThemeName { get; set; } = "Light";

        public string ThemeColor { get; set; } = "Blue";

        public bool ShowParagraphIdentification { get; set; } = true;

        public bool ShowBilingual { get; set; } = true;

        // Simple Search parameters
        public int MaxExpressionsStored { get; set; } = 50;

        public List<string> SearchStrings { get; set; } = new List<string>();

        public bool SimpleSearchIncludePartI { get; set; } = true;

        public bool SimpleSearchIncludePartII { get; set; } = true;

        public bool SimpleSearchIncludePartIII { get; set; } = true;

        public bool SimpleSearchIncludePartIV { get; set; } = true;

        public bool SimpleSearchCurrentPaperOnly { get; set; } = false;

        public string FontFamilyInfo { get; set; } = "Verdana";

        public double FontSizeInfo { get; set; } = 20.0;    // BUG: Default size needs to be proportional to user screen resolution

        public double SpliterDistance { get; set; } = 550;  // BUG: Default value needs to be proportional to user screen resolution

        public List<string> SearchIndexEntries = new List<string>();

        public List<TOC_Entry> TrackEntries = new List<TOC_Entry>();

        public string IndexLetters = "";

        public string LastTrackFileSaved = "";

        /// <summary>
        /// Serialize the parameters instance
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pathParameters"></param>
        public static void Serialize(Parameters p, string pathParameters)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                File.WriteAllText(pathParameters, jsonString);
            }
            catch  {    }
        }

        /// <summary>
        /// Deserialize the parameters instance
        /// </summary>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        public static Parameters Deserialize(string pathParameters)
        {
            try
            {
                var jsonString = File.ReadAllText(pathParameters);
                return JsonConvert.DeserializeObject<Parameters>(jsonString);
            }
            catch 
            {
                return new Parameters();
            }
        }



    }
}
