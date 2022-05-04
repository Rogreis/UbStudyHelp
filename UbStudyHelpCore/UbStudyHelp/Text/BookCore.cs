using System;
using System.Collections.Generic;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Text
{
    public class BookCore : Book
    {

        private GetDataFilesCore dataFiles = null;


        public BookCore()
        {
            EventsControl.AnnotationChanged += EventsControl_AnnotationChanged;
        }

        #region Annotations

        private void EventsControl_AnnotationChanged(UbAnnotationsStoreData data)
        {
            List<UbAnnotationsStoreData> list = dataFiles.LoadPaperAnnotations(data.Entry.TranslationId);
            UbAnnotationsStoreData existingData= list.Find(d => d.Entry == data.Entry && data.AnnotationType == data.AnnotationType);
            if (existingData != null)
            {
                list.Remove(existingData);
            }
            list.Add(data);
            dataFiles.StorePaperAnnotations(data.Entry.TranslationId, list);
        }

        public override UbAnnotationsStoreData GetUbAnnotationsStoreData(TOC_Entry entry, UbAnnotationType annotationType)
        {
            List<UbAnnotationsStoreData> list = dataFiles.LoadPaperAnnotations(entry.TranslationId);
            return list.Find(d => d.Entry == entry && d.AnnotationType == annotationType);
        }

        #endregion


        public override bool Inicialize(string baseDataPath, short leftTranslationId, short rightTranslationID)
        {
            try
            {
                FilesPath = baseDataPath;
                dataFiles = new GetDataFilesCore();
                Translations = dataFiles.GetTranslations();
                LeftTranslation = dataFiles.GetTranslation(leftTranslationId);
                RightTranslation = dataFiles.GetTranslation(rightTranslationID);
                return true;
            }
            catch (Exception ex)
            {
                string message = $"General error getting translations: {ex.Message}. May be you do not have the correct data to use this tool.";
                StaticObjects.Logger.Error(message, ex);
                return false;
            }
        }

        public void SetNewTranslation(Translation translation, bool isLeft = true)
        {
            if (isLeft)
            {
                StaticObjects.Parameters.LanguageIDLeftTranslation = translation.LanguageID;
                LeftTranslation = dataFiles.GetTranslation(translation.LanguageID);
            }
            else
            {
                StaticObjects.Parameters.LanguageIDRightTranslation = translation.LanguageID;
                RightTranslation = dataFiles.GetTranslation(translation.LanguageID);
            }
            EventsControl.FireTranslationsChanged();
        }

    }
}
