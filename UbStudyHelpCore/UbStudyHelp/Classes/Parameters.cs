﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using UbStudyHelp;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Classes
{
    public class Parameters
    {

        /// <summary>
        /// Last position in the text, default for first paragraph
        /// </summary>
        public TOC_Entry Entry { get; set; } = new TOC_Entry(0, 1, 0);

        public short LanguageIDLeftTranslation { get; set; } = 0;

        public short LanguageIDRightTranslation { get; set; } = 34;

        public int MaxTrackItems { get; set; } = 30;

        public int SearchPageSize { get; set; } = 20;

        public string ThemeName { get; set; } = "Light";

        public string ThemeColor { get; set; } = "Blue";

        // Simple Search parameters
        public int MaxExpressionsStored { get; set; } = 20;

        public List<string> SearchStrings { get; set; } = new List<string>();

        public bool SimpleSearchIncludePartI { get; set; } = true;

        public bool SimpleSearchIncludePartII { get; set; } = true;

        public bool SimpleSearchIncludePartIII { get; set; } = true;

        public bool SimpleSearchIncludePartIV { get; set; } = true;

        public bool IsPaperIncluded(int PaperNo) => ((App.ParametersData.SimpleSearchIncludePartI && PaperNo < 32) ||
                                                    (App.ParametersData.SimpleSearchIncludePartII && PaperNo >= 32 && PaperNo <= 56) ||
                                                    (App.ParametersData.SimpleSearchIncludePartIII && PaperNo >= 57 && PaperNo <= 119) ||
                                                    (App.ParametersData.SimpleSearchIncludePartIV && PaperNo >= 120));


        public int SpliterDistance { get; set; } = 360;


        public List<string> SearchIndexEntries = new List<string>();

        public string IndexLetters = "";

    }
}
