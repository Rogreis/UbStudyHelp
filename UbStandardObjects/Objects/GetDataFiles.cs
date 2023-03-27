using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace UbStandardObjects.Objects
{
    public class GetDataFiles
    {
        protected const string TubFilesFolder = "TUB_Files";

        protected const string ControlFileName = "AvailableTranslations.json";

        protected const string indexFileName = "Index.zip";

        protected const string translationAnnotationsFileName = "TranslationAnnotations";

        protected const string paragraphAnnotationsFileName = "TranslationParagraphAnnotations";

        //private string currentAnnotationsFileNAme = "Annotations";


        protected Parameters Param = null;

        public GetDataFiles(Parameters param)
        {
            Param = param;
        }

        #region File path creation
        /// <summary>
        /// Generates the control file full path
        /// </summary>
        /// <returns></returns>
        protected virtual string ControlFilePath()
        {
            return Path.Combine(Param.TUB_Files_RepositoryFolder, ControlFileName);

        }

        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected virtual string TranslationFilePath(short translationId)
        {
            return Path.Combine(Param.TUB_Files_RepositoryFolder, $"TR{translationId:000}.gz");
        }


        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected virtual string TranslationJsonFilePath(short translationId)
        {
            return Path.Combine(Param.TUB_Files_RepositoryFolder, $"TR{translationId:000}.json");
        }

        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected virtual string TranslationAnnotationsJsonFilePath(short translationId)
        {
            return Path.Combine(Param.TUB_Files_RepositoryFolder, $"{translationAnnotationsFileName}_{translationId:000}.json");
        }

        protected virtual string ParagraphAnnotationsJsonFilePath(short translationId)
        {
            return Path.Combine(Param.TUB_Files_RepositoryFolder, $"{paragraphAnnotationsFileName}_{translationId:000}.json");
        }

        #endregion



        /// <summary>
        /// Get public data from github
        /// <see href="https://raw.githubusercontent.com/Rogreis/TUB_Files/main"/>
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        protected byte[] GetGitHubBinaryFile(string fileName, bool isZip= false)
        {
            try
            {
                // https://raw.githubusercontent.com/Rogreis/TUB_Files/main/UbHelpTextControl.xml
                UbStandardObjects.StaticObjects.Logger.Info("Downloading files from github.");
                string url = isZip ? $"https://github.com/Rogreis/TUB_Files/raw/main/{fileName}" :
                    $"https://raw.githubusercontent.com/Rogreis/TUB_Files/main/{fileName}";
                UbStandardObjects.StaticObjects.Logger.Info("Url: " + url);

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("a", "a");
                    byte[] bytes = wc.DownloadData(url);
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                string message = $"Error getting file {fileName}: {ex.Message}. May be you do not have the correct data to use this tool.";
                UbStandardObjects.StaticObjects.Logger.Error(message, ex);
                return null;
            }
        }

        /// <summary>
        /// Copy streams
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        protected static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Unzip a Gzipped translation file
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string BytesToString(byte[] bytes, bool isZip= true)
        {
            if (!isZip)
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        //gs.CopyTo(mso);
                        CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
        }

        #region Load & Store Annotations
        protected void StoreJsonAnnotations(TOC_Entry entry, string jsonAnnotations)
        {
            string path = TranslationAnnotationsJsonFilePath(entry.TranslationId);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (string.IsNullOrWhiteSpace(jsonAnnotations))
            {
                return;
            }
            File.WriteAllText(path, jsonAnnotations);
        }

        /// <summary>
        /// Loads a list of all TOC/Annotation done for a paper
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected string LoadJsonAnnotations(short translationId)
        {
            string pathAnnotationsFile = TranslationAnnotationsJsonFilePath(translationId);
            if (File.Exists(pathAnnotationsFile))
            {
                return File.ReadAllText(pathAnnotationsFile);
            }
            return null;
        }

        #endregion


        /// <summary>
        /// Get all papers from the zipped file
        /// </summary>
        /// <param name="translationId"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        protected string GetFile(short translationId, bool isZip = true)
        {
            try
            {
                string json = "";

                string translationJsonFilePath = TranslationJsonFilePath(translationId);
                if (File.Exists(translationJsonFilePath))
                {
                    json = File.ReadAllText(translationJsonFilePath);
                    return json;
                }

                string translationStartupPath = TranslationFilePath(translationId);
                if (File.Exists(translationStartupPath))
                {
                    StaticObjects.Logger.Info("File exists: " + translationStartupPath);
                    byte[] bytes = File.ReadAllBytes(translationStartupPath);
                    json = BytesToString(bytes, isZip);
                    File.WriteAllText(translationJsonFilePath, json);
                    return json;
                }
                else
                {
                    StaticObjects.Logger.Error($"Translation not found {translationId}");
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
        public List<Translation> GetTranslations()
        {
            string path = ControlFilePath();
            string json = File.ReadAllText(path);
            return Translations.DeserializeJson(json);
        }

        /// <summary>
        /// Get a translation from a local file
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        public virtual Translation GetTranslation(short translationId, bool initializePapers= true)
        {
            Translation translation = StaticObjects.Book.GetTranslation(translationId);
            if (translation == null)
            {
                translation = new Translation();
            }

            if (!initializePapers) return translation;

            if (translation.Papers.Count > 0)
            {
                if (!translation.CheckData())
                {
                    return null;
                }

                return translation;
            }

            // FIX ME: IsEditingTranslation is hard codded here, but needs to come from repository
            if (!translation.IsEditingTranslation)
            {
                string json = GetFile(translationId, true);
                translation.GetPapersData(json);

                // Loading annotations
                translation.Annotations = null;
            }
            return translation;


        }

        /// <summary>
        /// Get the zipped format table json and unzipp it to return
        /// </summary>
        /// <returns></returns>
        public string GetFormatTable()
        {
            try
            {
                string formatTableZippedPath = Path.Combine(Param.TUB_Files_RepositoryFolder, @"FormatTable.gz");

                if (!File.Exists(formatTableZippedPath))
                {
                    StaticObjects.Logger.Error($"Format Table not found {formatTableZippedPath}");
                    return null;
                }

                StaticObjects.Logger.Info("File exists: " + formatTableZippedPath);
                byte[] bytes = File.ReadAllBytes(formatTableZippedPath);
                return BytesToString(bytes);
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("GetFormatTable", ex);
                return null;
            }
        }


        /// <summary>
        /// Get the json string for a paper notes
        /// </summary>
        /// <returns></returns>
        public string GetNotes(short paperNo)
        {
            try
            {
                string filePath = Path.Combine(Param.EditParagraphsRepositoryFolder, $@"{ParagraphMarkDown.FolderPath(paperNo)}\Notes.json");
                string jsonString= File.ReadAllText(filePath);
                return "";
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("GetFormatTable", ex);
                return null;
            }
        }

    }

}
