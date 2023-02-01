using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.ContextMenuCode;

namespace UbStudyHelp.Text
{
    public enum NewTranslation
    {
        Left,
        Middle,
        Right,
        Compare
    }

    public class BookCore : Book
    {

        private GetDataFilesCore DataReader = null;


        public BookCore(GetDataFilesCore dataReader)
        {
            EventsControl.AnnotationsChanges += EventsControl_AnnotationsChanges;
            DataReader = dataReader;
        }

       public void SetNewTranslation(Translation translation, NewTranslation newTranslation)
        {
            try
            {
                switch(newTranslation)
                {
                    case NewTranslation.Left:
                        if (StaticObjects.Parameters.LanguageIDLeftTranslation == translation.LanguageID)
                        {
                            return;
                        }
                        StaticObjects.Parameters.LanguageIDLeftTranslation = translation.LanguageID;
                        LeftTranslation = DataReader.GetTranslation(translation.LanguageID);
                        StaticObjects.Logger.IsNull(LeftTranslation, $"Invalid of non existing translation: {translation.LanguageID}");
                        break;
                    case NewTranslation.Middle:
                        if (StaticObjects.Parameters.LanguageIDMiddleTranslation == translation.LanguageID)
                        {
                            return;
                        }

                        StaticObjects.Parameters.LanguageIDMiddleTranslation = translation.LanguageID;
                        MiddleTranslation = DataReader.GetTranslation(translation.LanguageID);
                        StaticObjects.Logger.IsNull(MiddleTranslation, $"Invalid of non existing translation: {translation.LanguageID}");
                        break;
                    case NewTranslation.Right:
                        StaticObjects.Parameters.LanguageIDRightTranslation = translation.LanguageID;
                        RightTranslation = DataReader.GetTranslation(translation.LanguageID);
                        StaticObjects.Logger.IsNull(RightTranslation, $"Invalid of non existing translation: {translation.LanguageID}");
                        break;
                    case NewTranslation.Compare:
                        break;
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
            ((GetDataFilesCore)DataReader).StoreAnnotations(entry, annotations);
        }

        /// <summary>
        /// Remove annotations from the translation list
        /// Do not need to store; this will be done for another event fired next
        /// </summary>
        /// <param name="entry"></param>
        public override void DeleteAnnotations(TOC_Entry entry)
        {
            UbAnnotationsStoreData data = LeftTranslation.Annotations.Find(a => a.Entry == entry);
            if (data != null)
            {
                LeftTranslation.Annotations.Remove(data);
                EventsControl.FireAnnotationsChanges(entry);
                return;
            }

            data = RightTranslation.Annotations.Find(a => a.Entry == entry);
            if (data != null)
            {
                RightTranslation.Annotations.Remove(data);
                EventsControl.FireAnnotationsChanges(entry);
                return;
            }
        }

        private void EventsControl_AnnotationsChanges(TOC_Entry entry)
        {
            StoreAnnotations(entry, entry.TranslationId == StaticObjects.Parameters.LanguageIDLeftTranslation ? LeftTranslation.Annotations : RightTranslation.Annotations);
        }



    }
}
