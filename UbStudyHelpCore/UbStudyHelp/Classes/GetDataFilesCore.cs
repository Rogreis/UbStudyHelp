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

        public GetDataFilesCore(string appFolder, string localStorageFolder) : base(appFolder, localStorageFolder)
        {
        }


        #region Load & Store Annotations
        /// <summary>
        /// Create a json string from the annotations list and store
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="annotations"></param>
        public void StoreAnnotations(TOC_Entry entry, List<UbAnnotationsStoreData> annotations)
        {
            string jsonAnnotations = StaticObjects.Serialize<List<UbAnnotationsStoreData>>(annotations);
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
