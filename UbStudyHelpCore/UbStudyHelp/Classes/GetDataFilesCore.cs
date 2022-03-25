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
        private string DestinationFolder = "";

        private string SourceFolder = "";

        public GetDataFilesCore(string destinationFolder)
        {
            SourceFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TUB_Files");
            DestinationFolder = destinationFolder;
        }

        /// <summary>
        /// Checks is a files exists localy
        /// Is not, get it from server
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isZip"></param>
        /// <returns>The uncompressed json string</returns>
        private string GetFile(short translatioId, string fileName, bool isZip = true)
        {
            try
            {
                string json = "";
                if (!Directory.Exists(DestinationFolder))
                {
                    StaticObjects.Logger.Info("Creating data folder: " + DestinationFolder);
                    Directory.CreateDirectory(DestinationFolder);
                }
                string path = Path.Combine(DestinationFolder, fileName);
                if (!File.Exists(path))
                {
                    StaticObjects.Logger.Info("File does not exist: " + path);
                    string translationStartupPath = Path.Combine(SourceFolder, $"TR{translatioId:000}.gz");
                    byte[] bytes = File.ReadAllBytes(translationStartupPath);
                    json= BytesToString(bytes, isZip);
                    File.WriteAllText(path, json);
                }
                else
                {
                    json= File.ReadAllText(path);
                }
                return json;
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
            Translation translation= StaticObjects.Book.GetTranslation(translatioId);
            if (translation == null)
            {
                translation= new Translation();
            }
            if (translation.Papers.Count > 0)
            {
                return translation;
            }
            string translationFileName = $"TR{translatioId:000}.json";
            string json= GetFile(translatioId, translationFileName, true);
            translation.GetData(json);
            return translation;
        }

    }

}
