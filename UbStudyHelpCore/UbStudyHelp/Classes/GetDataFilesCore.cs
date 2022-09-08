using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes.ContextMenuCode;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Get data files specific for the WPF edition
    /// Starting on version 2.1, files are embbedded into the exe and unziped in the start
    /// </summary>
    public class GetDataFilesCore : GetDataFiles
    {

        public GetDataFilesCore(ParametersCore param) : base(param)
        {
        }

        #region File path creation
        /// <summary>
        /// Generates the control file full path
        /// </summary>
        /// <returns></returns>
        protected override string ControlFilePath()
        {
            return Path.Combine(ApplicationFolderTubFiles, ControlFileName);

        }

        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected override string TranslationFilePath(short translationId)
        {
            return Path.Combine(ApplicationFolderTubFiles, $"TR{translationId:000}.gz");
        }


        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected override string TranslationJsonFilePath(short translationId)
        {
            return Path.Combine(StaticObjects.Parameters.TUB_Files_RepositoryFolder, $"TR{translationId:000}.json");
        }

        /// <summary>
        /// Generates the translation full path
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        protected override string TranslationAnnotationsJsonFilePath(short translationId)
        {
            return Path.Combine(StaticObjects.Parameters.TUB_Files_RepositoryFolder, $"{translationAnnotationsFileName}_{translationId:000}.json");
        }

        protected override string ParagraphAnnotationsJsonFilePath(short translationId)
        {
            return Path.Combine(StaticObjects.Parameters.TUB_Files_RepositoryFolder, $"{paragraphAnnotationsFileName}_{translationId:000}.json");
        }

        #endregion


        #region Load & Store Annotations
        /// <summary>
        /// Create a json string from the annotations list and store
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="annotations"></param>
        public void StoreAnnotations(TOC_Entry entry, List<UbAnnotationsStoreData> annotations)
        {
            string jsonAnnotations = annotations.Count == 0 ? null : StaticObjects.Serialize<List<UbAnnotationsStoreData>>(annotations);
            StoreJsonAnnotations(entry, jsonAnnotations);
        }

        /// <summary>
        /// Loads a list of all TOC/Annotation done for a paper
        /// </summary>
        /// <param name="translationId"></param>
        /// <returns></returns>
        public List<UbAnnotationsStoreData> LoadAnnotations(short translationId)
        {
            string jsonAnnotations = LoadJsonAnnotations(translationId);
            if (!string.IsNullOrWhiteSpace(jsonAnnotations))
            {
                return (StaticObjects.DeserializeObject<List<UbAnnotationsStoreDataCore>>(jsonAnnotations)).ToList<UbAnnotationsStoreData>();
            }
            // Return empty list
            return new List<UbAnnotationsStoreDataCore>().ToList<UbAnnotationsStoreData>();
        }
        #endregion

        /// <summary>
        /// Get a translation from a local file
        /// Here also the annotations are got
        /// </summary>
        /// <param name="translatioId"></param>
        /// <returns></returns>
        public override Translation GetTranslation(short translatioId)
        {
            Translation translation = base.GetTranslation(translatioId);
            // Loading annotations
            translation.Annotations = LoadAnnotations(translatioId);
            return translation;
        }



    }

}
