using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Xml.Linq;
using UbStudyHelp.Classes;
using System.Runtime.InteropServices.ComTypes;
using UbStandardObjects.Objects;
using UbStandardObjects;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Get data files specific for the WPF edition
    /// Starting on version 2.1, files are embbedded into the exe and unziped in the start
    /// </summary>
    public class GetDataFilesCore : GetDataFiles
    {
        private string SourceFolder = "";

        public GetDataFilesCore()
        {
            SourceFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TUB_Files");
        }

        /// <summary>
        /// Get all papers from the zipped file
        /// </summary>
        /// <param name="translatioId"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        private string GetFile(short translatioId, bool isZip = true)
        {
            try
            {
                string json = "";
                string translationStartupPath = Path.Combine(SourceFolder, $"TR{translatioId:000}.gz");
                if (File.Exists(translationStartupPath))
                {
                    StaticObjects.Logger.Info("File exists: " + translationStartupPath);
                    byte[] bytes = File.ReadAllBytes(translationStartupPath);
                    json = BytesToString(bytes, isZip);
                    return json;
                }
                else
                {
                    StaticObjects.Logger.Error($"Translation not found {translatioId}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("GetFile", ex);
                return null;
            }
        }


        /// <summary>
        /// Get the translations list from a local file
        /// </summary>
        /// <returns></returns>
        public override List<Translation> GetTranslations()
        {
            string path = Path.Combine(SourceFolder, ControlFileName);
            string json = File.ReadAllText(path);
            return Translations.DeserializeJson(json);
        }

        /// <summary>
        /// Get a translation from a local file
        /// </summary>
        /// <param name="translatioId"></param>
        /// <returns></returns>
        public override Translation GetTranslation(short translatioId)
        {
            Translation translation = StaticObjects.Book.GetTranslation(translatioId);
            if (translation == null)
            {
                translation = new Translation();
            }
            if (translation.Papers.Count > 0)
            {
                return translation;
            }
            string json = GetFile(translatioId, true);
            translation.GetData(json);
            return translation;
        }

    }

}
