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
    public class GetDataFilesCore : GetDataFiles
    {
        private string DestinationFolder = "";

        public GetDataFilesCore(string destinationFolder)
        {
            DestinationFolder= destinationFolder;
        }

        /// <summary>
        /// Checks is a files exists localy
        /// Is not, get it from server
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isZip"></param>
        /// <returns>The uncompressed json string</returns>
        private string GetFile(string fileName, bool isZip = true)
        {
            try
            {
                if (!Directory.Exists(DestinationFolder))
                {
                    StaticObjects.Logger.Info("Creating data folder: " + DestinationFolder);
                    Directory.CreateDirectory(DestinationFolder);
                }
                string path = Path.Combine(DestinationFolder, fileName);
                if (!File.Exists(path))
                {
                    StaticObjects.Logger.Info("File does not exist: " + path);

                    byte[] bytes = GetGitHubBinaryFile(fileName, true);
                    string json = BytesToString(bytes, isZip);
                    if (isZip)
                    {
                        File.WriteAllBytes(path, bytes);
                    }
                    else
                    {
                        File.WriteAllText(path, json);
                    }
                    return json;
                }
                else
                {
                    StaticObjects.Logger.Info("File exists: " + path);
                    if (isZip)
                    {
                        byte[] bytes= File.ReadAllBytes(path);
                        return BytesToString(bytes, isZip);
                    }
                    else
                    {
                        return File.ReadAllText(path);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("GetFile", ex);
                return null;
            }
        }

        public bool CheckFiles(string destinationFolder)
        {
            //try
            //{
            //    if (!GetFile(destinationFolder, ControlFileName, false))
            //    {
            //        return false;
            //    }
            //    if (!GetFile(destinationFolder, indexFileName, true))
            //    {
            //        return false;
            //    }

            //    string pathlistTranslations = Path.Combine(destinationFolder, ControlFileName);

            //    XElement xElem = XElement.Load(pathlistTranslations);
            //    List<TranslationCore> Translations = (from lang in xElem.Descendants("Translation")
            //                                      select new TranslationCore(lang)).ToList();

            //    foreach (TranslationCore translation in Translations)
            //    {
            //        string fileTranslationName = $"L{translation.LanguageID:000}.zip";
            //        if (!GetFile(destinationFolder, fileTranslationName, true))
            //        {
            //            return false;
            //        }
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Log.Logger.Error("Data not loaded!", ex);
            //    return false;
            //}
            return false;
        }

        public override List<Translation> GetTranslations()
        {
            string json = GetFile(ControlFileName, false);
            return Translations.DeserializeJson(json);
        }

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
            string translationFileName = $"TR{translatioId:000}.gz";
            string json= GetFile(translationFileName, true);
            translation.GetData(json);
            return translation;
        }

    }

}
