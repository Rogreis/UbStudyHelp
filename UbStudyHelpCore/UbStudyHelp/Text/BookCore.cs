using System;
using System.Collections.Generic;
using System.Text;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Text
{
    public class BookCore : Book
    {
		private GetDataFilesCore dataFiles = null;


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
