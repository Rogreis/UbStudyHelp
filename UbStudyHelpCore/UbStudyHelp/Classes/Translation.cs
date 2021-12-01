﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using System.Text;
using System.IO;
using System.Xml.Linq;
using UbStudyHelp;

namespace UbStudyHelp.Classes
{

    public class Translation : BaseClass
    {
        public short LanguageID { get; set; }
        public string Description { get; set; }
        public string TIN { get; set; }
        public string TUB { get; set; }
        public string LanguageCode { get; set; }
        public string TextButton { get; set; }
        public int CultureID { get; set; }
        public bool UseBold { get; set; }
        public bool RightToLeft { get; set; }

        public int StartingYear { get; set; }
        public int EndingYear { get; set; }
        public string ExternalName { get; set; }

        public string PaperTranslation { get; set; }

        public List<TOC_Entry> TableOfContents = null;

        private string BaseFilesPath { get; set; }

        public Translation()
        {
        }


        public Translation(XElement record)
        {
            // LanguageID, Description, TUB, TIN, UseBold, RightToLeft, TextButton, CultureID, StartingYear, EndingYear
            LanguageID = GetShort(record.Element("LanguageID"));
            Description = GetString(record.Element("Description"));
            TUB = GetString(record.Element("TUB"));
            TIN = GetString(record.Element("TIN"));
            UseBold = GetBool(record.Element("UseBold"));
            RightToLeft = GetBool(record.Element("RightToLeft"));
            TextButton = GetString(record.Element("TextButton"));
            CultureID = GetShort(record.Element("CultureID"));
            StartingYear = GetShort(record.Element("StartingYear"));
            EndingYear = GetShort(record.Element("EndingYear"));
            ExternalName = GetString(record.Element("ExternalName"));
            PaperTranslation = GetString(record.Element("PaperTranslation"));
        }


        public string Copyright
        {
            get
            {
                string year = (StartingYear == EndingYear) ? EndingYear.ToString() : StartingYear.ToString() + "," + EndingYear.ToString();
                return "Copyright ©  " + year + " Urantia Foundation. All rights reserved.";
            }
        }


        /// <summary>
        /// 
        /// Initialize data for a given translations
        /// 
        /// We have the following contents for a translation
        /// root
        /// sub folder L999 where 999 is the language ID 
        ///     toc.xml      table of contents file (xml format)
        ///     Paper999.xml each paper
        /// 
        /// 
        /// </summary>
        /// <param name="pathData"></param>
        /// <returns></returns>
        public bool Inicialize(string pathData)
        {
            try
            {
                BaseFilesPath = pathData;
                // Loading TOC: table of contents
                string pathFile = Path.Combine(pathData, "L" + LanguageID.ToString("000"));
                pathFile = Path.Combine(pathFile, "toc.xml");
                XElement xElemIndex = XElement.Load(pathFile);
                TableOfContents = new List<TOC_Entry>();
                foreach (XElement xElemPar in xElemIndex.Descendants("IndexEntry"))
                {
                    TableOfContents.Add(new TOC_Entry(xElemPar));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"General error initializing translation {LanguageID}: {ex.Message}. May be you do not have the correct data to use this tool.");
            }
        }



        public Paper Paper(short PaperNo)
        {
            Paper p = new Paper(BaseFilesPath, this, PaperNo);
            return p;
        }



        public override string ToString()
        {
            return LanguageID.ToString() + " - " + Description;
        }

    }
}
