using System;
using System.Collections.Generic;
using System.IO;

namespace UbStandardObjects.Objects
{
    /// <summary>
    /// Bilingual book main organization
    /// </summary>
    public class Book
    {

        protected GetDataFiles DataFiles = null;


        public Translation LeftTranslation = null;

        public Translation MiddleTranslation = null;

        public Translation RightTranslation = null;

        public List<Translation> Translations = null;

        public FormatTable FormatTableObject { get; set; } = null;

        public List<Translation> ObservableTranslations
        {
            get
            {
                List<Translation> list = new List<Translation>();
                list.AddRange(Translations);
                return list;
            }
        }



        /// <summary>
        /// Get a translation from the list
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        public virtual void StoreAnnotations(TOC_Entry entry, List<UbAnnotationsStoreData> annotations)
        {

        }

        public virtual void DeleteAnnotations(TOC_Entry entry)
        {

        }



}
}
