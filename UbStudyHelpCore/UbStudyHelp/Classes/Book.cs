using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UbStudyHelp;
using System.Collections.ObjectModel;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Bilingual book main organization
    /// </summary>
    public static class Book
    {

        private static Translation GetTranslation(short id)
        {
            Translation trans = Translations.Find(o => o.LanguageID == id);
            if (trans == null)
            {
                throw new Exception($"Missing translation number {App.ParametersData.LanguageIDLeftTranslation}. May be you do not have the correct data to use this tool.");
            }
            if (!trans.Inicialize(FilesPath))
            {
                throw new Exception($"Translation {App.ParametersData.LanguageIDLeftTranslation} not initialized. May be you do not have the correct data to use this tool.");
            }
            return trans;
        }

        public static string FilesPath { get; set; }

        public static Translation LeftTranslation
        {
            get
            {
                return GetTranslation(App.ParametersData.LanguageIDLeftTranslation);
            }
        }

        public static Translation RightTranslation
        {
            get
            {
                return GetTranslation(App.ParametersData.LanguageIDRightTranslation);
            }
        }

        public static List<Translation> Translations { get; private set; }

        public static List<Translation> ObservableTranslations
        {
            get
            {
                List<Translation> list = new List<Translation>();
                list.AddRange(Translations);
                return list;
            }
        }

        public static bool Inicialize(string baseDataPath)
        {
            try
            {
                FilesPath = baseDataPath;
                if (Translations == null)
                {
                    string pathlistTranslations = Path.Combine(baseDataPath, "Languages.xml");

                    XElement xElem = XElement.Load(pathlistTranslations);
                    Translations = (from lang in xElem.Descendants("Translation")
                                    select new Translation(lang)).ToList();
                }
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"General error getting translations: {ex.Message}. May be you do not have the correct data to use this tool.");
            }
        }



    }
}
