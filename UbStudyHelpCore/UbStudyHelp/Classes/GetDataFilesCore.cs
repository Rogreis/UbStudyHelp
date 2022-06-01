using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        #region Store Annotations
        private void StoreListAnnotations(List<UbAnnotationsStoreData> list, TOC_Entry entry)
        {
            string path = TranslationAnnotationsJsonFilePath(entry.TranslationId);
            string jsonString = App.Serialize<List<UbAnnotationsStoreData>>(list);
            File.WriteAllText(path, jsonString);
        }

        private void StoreParagraphAnnotations(UbAnnotationsStoreData data)
        {
            List<UbAnnotationsStoreData> list = LoadPaperAnnotations(data.Entry.TranslationId);
            UbAnnotationsStoreData existingData = list.Find(a => a.Entry == data.Entry && data.AnnotationType == UbAnnotationType.Paragraph);
            if (existingData != null)
            {
                list.Remove(existingData);
            }
            list.Add(data);
            StoreListAnnotations(list, data.Entry);
        }

        private void StorePaperAnnotations(UbAnnotationsStoreData data)
        {
            List<UbAnnotationsStoreData> list = LoadPaperAnnotations(data.Entry.TranslationId);
            UbAnnotationsStoreData existingData = list.Find(a => a.Entry == data.Entry && data.AnnotationType == UbAnnotationType.Paper);
            if (existingData != null)
            {
                list.Remove(existingData);
            }
            list.Add(data);
            StoreListAnnotations(list, data.Entry);
        }


        public override void StoreAnnotations(UbAnnotationsStoreSet annotationsSet)
        {

            if (annotationsSet.ParagraphAnnotations != null)
            {
                StoreParagraphAnnotations(annotationsSet.ParagraphAnnotations);
            }
            else
            {
                StorePaperAnnotations(annotationsSet.PaperLeftAnnotations);
                StorePaperAnnotations(annotationsSet.PaperRightAnnotations);
            }
        }
        #endregion

        /// <summary>
        /// Loads a list of all TOC/Annotation done for a paper
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
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
