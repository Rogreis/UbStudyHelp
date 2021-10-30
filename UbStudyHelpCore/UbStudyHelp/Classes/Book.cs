using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UbStudyHelp;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Bilingual book main organization
    /// </summary>
    public static class Book
    {

        public static string FilesPath { get; set; }

        public static Translation LeftTranslation { get; private set; }

        public static Translation RightTranslation { get; private set; }

        public static List<Translation> Translations { get; private set; }

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

                if (Translations == null || Translations.Count < 2)
                {
                    throw new Exception("There is no translation list. May be you do not have the correct data to use this tool.");
                }

                LeftTranslation = Translations.Find(o => o.LanguageID == App.ParametersData.LanguageIDLeftTranslation);
                if (LeftTranslation == null)
                {
                    throw new Exception($"Missing translation number {App.ParametersData.LanguageIDLeftTranslation}. May be you do not have the correct data to use this tool.");
                }
                if (!LeftTranslation.Inicialize(baseDataPath))
                {
                    throw new Exception($"Translation {App.ParametersData.LanguageIDLeftTranslation} not initialized. May be you do not have the correct data to use this tool.");
                }

                RightTranslation = Translations.Find(o => o.LanguageID == App.ParametersData.LanguageIDRightTranslation);
                if (RightTranslation == null)
                {
                    throw new Exception($"Missing translation number {App.ParametersData.LanguageIDRightTranslation}. May be you do not have the correct data to use this tool.");
                }
                if (!RightTranslation.Inicialize(baseDataPath))
                {
                    throw new Exception($"Translation {App.ParametersData.LanguageIDRightTranslation} not initialized. May be you do not have the correct data to use this tool.");
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
