using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.ContextMenuCode;

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

        private List<Annotation> PaperAnnotations= new List<Annotation>();
        private List<Annotation> ParagraphAnnotations = new List<Annotation>();

        private void EventsControl_AnnotationChanged(UbAnnotationData data)
        {
            switch (data.AnnotationType)
            {
                case EbAnnotationType.Paper:
                    PaperAnnotationsWork(data);
                    break;
                case EbAnnotationType.Paragraph:
                    ParagraphAnnotationsWork(data);
                    break;
            }
        }

        private void PaperAnnotationsWork(UbAnnotationData data)
        {
            switch(data.Action)
            {
                case StoreContentAction.Added:
                    PaperAnnotations.Add(data.Note);
                    break;
                case StoreContentAction.Deleted:
                    PaperAnnotations.Remove(data.Note);
                    break;
            }
        }

        private void ParagraphAnnotationsWork(UbAnnotationData data)
        {
            switch (data.Action)
            {
                case StoreContentAction.Added:
                    ParagraphAnnotations.Add(data.Note);
                    break;
                case StoreContentAction.Deleted:
                    ParagraphAnnotations.Remove(data.Note);
                    break;
            }
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
