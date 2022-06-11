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

        private void EventsControl_AnnotationChanged(UbAnnotationsStoreSet annotations)
        {
            dataFiles.StoreAnnotations(annotations);
        }

        public override UbAnnotationsStoreSet GetParagraphAnnotations(TOC_Entry entry)
        {
            UbAnnotationsStoreSet annotationsSet = new UbAnnotationsStoreSet();
            annotationsSet.ParagraphAnnotations= dataFiles.LoadPaperAnnotations(entry.TranslationId)
                    //.Find(d => d.Entry.Paper == entry.Paper && d.Entry.TranslationId == LeftTranslation.LanguageID && d.AnnotationType == UbAnnotationType.Paragraph) ?? new UbAnnotationsStoreData();
                    .Find(d => d.Entry.Paper == entry.Paper && d.Entry.TranslationId == LeftTranslation.LanguageID) ?? new UbAnnotationsStoreData();
            annotationsSet.ParagraphAnnotations.Entry = entry;
            annotationsSet.ParagraphAnnotations.Entry.Text = "";
            return annotationsSet;
        }


        /// <summary>
        /// Read stored annotations for this translation-paper
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>Retuns only the annotations for both right and left paper</returns>
        public override UbAnnotationsStoreSet GetPaperAnnotations(TOC_Entry entry)
        {
            UbAnnotationsStoreSet annotationsSet = new UbAnnotationsStoreSet();

            annotationsSet.PaperLeftAnnotations =
                dataFiles.LoadPaperAnnotations(StaticObjects.Book.LeftTranslation.LanguageID)
                    .Find(d => d.Entry.Paper == entry.Paper && d.Entry.TranslationId == LeftTranslation.LanguageID && d.AnnotationType == UbAnnotationType.Paper) ?? new UbAnnotationsStoreData();
            annotationsSet.PaperRightAnnotations =
                dataFiles.LoadPaperAnnotations(StaticObjects.Book.RightTranslation.LanguageID)
                    .Find(d => d.Entry.Paper == entry.Paper && d.Entry.TranslationId == RightTranslation.LanguageID && d.AnnotationType == UbAnnotationType.Paper) ?? new UbAnnotationsStoreData();
            if (annotationsSet.PaperLeftAnnotations == null)
            {
                annotationsSet.PaperLeftAnnotations = new UbAnnotationsStoreData();
            }
            if (annotationsSet.PaperRightAnnotations == null)
            {
                annotationsSet.PaperRightAnnotations = new UbAnnotationsStoreData();
            }
            //annotationsSet.PaperLeftAnnotations.Entry = TOC_Entry.CreateEntry(entry, LeftTranslation.LanguageID);
            //annotationsSet.PaperRightAnnotations.Entry = TOC_Entry.CreateEntry(entry, RightTranslation.LanguageID);
            return annotationsSet;
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
