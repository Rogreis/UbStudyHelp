using System.Collections.Generic;

namespace UbStandardObjects.Objects
{
    /// <summary>
    /// Bilingual book main organization
    /// </summary>
    public abstract class Book
    {

        public string FilesPath { get; set; }

        public Translation LeftTranslation { get; set; }

        public Translation RightTranslation { get; set; }

        public List<Translation> Translations { get; set; }

        public Translation GetTranslation(short id)
        {
            Translation trans = Translations.Find(o => o.LanguageID == id);
            string message = "";
            if (trans == null)
            {
                message = $"Missing translation number {id}. May be you do not have the correct data to use this tool.";
                StaticObjects.Logger.FatalError(message);
            }
            return trans;
        }


        public List<Translation> ObservableTranslations
        {
            get
            {
                List<Translation> list = new List<Translation>();
                list.AddRange(Translations);
                return list;
            }
        }

        public abstract bool Inicialize(string baseDataPath, short leftTranslationId, short rightTranslationID);

        public abstract void StoreAnnotations(TOC_Entry entry, List<UbAnnotationsStoreData> annotations);

        public abstract void DeleteAnnotations(TOC_Entry entry);
    }
}
