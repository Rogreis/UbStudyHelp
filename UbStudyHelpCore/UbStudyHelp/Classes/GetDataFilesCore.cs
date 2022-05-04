using System;
using System.Collections.Generic;
using System.IO;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Get data files specific for the WPF edition
    /// Starting on version 2.1, files are embbedded into the exe and unziped in the start
    /// </summary>
    public class GetDataFilesCore : GetDataFiles
    {

        public GetDataFilesCore()
        {
            SourceFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, TubFilesFolder);
            StoreFolder = App.BaseTubFilesPath;
        }


        /// <summary>
        /// Get all papers from the zipped file
        /// </summary>
        /// <param name="translationId"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        private string GetFile(short translationId, bool isZip = true)
        {
            try
            {
                string json = "";
                string translationStartupPath = TranslationFilePath(translationId);
                if (File.Exists(translationStartupPath))
                {
                    StaticObjects.Logger.Info("File exists: " + translationStartupPath);
                    byte[] bytes = File.ReadAllBytes(translationStartupPath);
                    json = BytesToString(bytes, isZip);
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
        public override List<Translation> GetTranslations()
        {
            string path = ControlFilePath();
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

        public override void StorePaperAnnotations(short translationId, List<UbAnnotationsStoreData> list)
        {
            string path = TranslationAnnotationsJsonFilePath(translationId);
            string jsonString = App.Serialize<List<UbAnnotationsStoreData>>(list);
            File.WriteAllText(path, jsonString);
        }

        public override List<UbAnnotationsStoreData> LoadPaperAnnotations(short translationId)
        {
            string path = TranslationAnnotationsJsonFilePath(translationId);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return App.DeserializeObject<List<UbAnnotationsStoreData>>(json);
            }
            return new List<UbAnnotationsStoreData>();
        }

    }

}
