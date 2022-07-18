using System;
using System.Collections.Generic;
using System.Windows;
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
        }


        public override bool Inicialize(string baseDataPath, short leftTranslationId, short rightTranslationID)
        {
            try
            {
                FilesPath = baseDataPath;
                dataFiles = new GetDataFilesCore();
                Translations = dataFiles.GetTranslations();
                LeftTranslation = dataFiles.GetTranslation(leftTranslationId);
                if (!LeftTranslation.CheckData()) return false;
                RightTranslation = dataFiles.GetTranslation(rightTranslationID);
                if (!RightTranslation.CheckData()) return false;
                return true;
            }
            catch (Exception ex)
            {
                string message = $"General error getting translations: {ex.Message}. May be you do not have the correct data to use this tool.";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
                return false;
            }
        }

        public void SetNewTranslation(Translation translation, bool isLeft = true)
        {
            try
            {
                if (isLeft)
                {
                    StaticObjects.Parameters.LanguageIDLeftTranslation = translation.LanguageID;
                    LeftTranslation = dataFiles.GetTranslation(translation.LanguageID);
                    StaticObjects.Logger.IsNull(LeftTranslation, $"Invalid of non existing translation: {translation.LanguageID}");
                    if (!LeftTranslation.CheckData())
                    {
                        return;
                    }
                }
                else
                {
                    StaticObjects.Parameters.LanguageIDRightTranslation = translation.LanguageID;
                    RightTranslation = dataFiles.GetTranslation(translation.LanguageID);
                    StaticObjects.Logger.IsNull(RightTranslation, $"Invalid of non existing translation: {translation.LanguageID}");
                    if (!RightTranslation.CheckData())
                    {
                        return;
                    }
                }
                EventsControl.FireTranslationsChanged();
            }
            catch (Exception ex)
            {
                string message = $"General error changing translation: {ex.Message}. May be you do not have the correct data to use this tool.";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
            }
        }

        public override void StoreAnnotations(TOC_Entry entry, List<UbAnnotationsStoreData> annotations)
        {
            dataFiles.StoreAnnotations(entry, annotations);
        }

        public override void DeleteAnnotations(TOC_Entry entry)
        {
            UbAnnotationsStoreData data = LeftTranslation.Annotations.Find(a => a.Entry == entry);
            if (data != null)
            {
                LeftTranslation.Annotations.Remove(data);
                EventsControl.FireAnnotationsChanges();
                return;
            }

            data = RightTranslation.Annotations.Find(a => a.Entry == entry);
            if (data != null)
            {
                RightTranslation.Annotations.Remove(data);
                EventsControl.FireAnnotationsChanges();
                return;
            }
        }


    }
}
