using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{
    internal class UbAnnotationsStoreDataCore : UbAnnotationsStoreData
    {
        [JsonIgnore]
        public List<Annotation> Annotations { get; set; } = new List<Annotation>();


        public UbAnnotationsStoreDataCore(TOC_Entry entry, UbAnnotationType annotationType)
        {
            Entry = entry;
            AnnotationType = annotationType;
        }

        // UbAnnotationsStoreData data
        public UbAnnotationsStoreDataCore(UbAnnotationsStoreData data)
        {
            Entry = data.Entry;
            AnnotationType = data.AnnotationType;
            XamlNotes = data.XamlNotes;
            Entry = data.Entry;
            Title = data.Title;
            AnnotationsInfo = data.AnnotationsInfo;
            foreach (UbAnnotationInfo annotationInfo in data.AnnotationsInfo)
            {
                try
                {
                    Annotation annotation = DeSerializeAnnotation(annotationInfo.AnnotationsString);
                    Annotations.Add(annotation);
                }
                catch // Ignore errors
                { }
            }
        }

        private byte[] SerializeAnnotation(Annotation annotation)
        {
            MemoryStream stream = new MemoryStream();
            XmlStreamStore annotationStore = new XmlStreamStore(stream);
            annotationStore.AddAnnotation(annotation);
            annotationStore.Flush();
            byte[] serializedAnnotation = stream.ToArray();
            return serializedAnnotation;
        }

        private Annotation DeSerializeAnnotation(string annotationString)
        {
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(annotationString));
            XmlStreamStore store = new XmlStreamStore(mStream);
            return store.GetAnnotations().First();
        }


        //public void Serialize()
        //{
        //    AnnotationsInfo = new List<string>();
        //    foreach (Annotation annotation in Annotations)
        //    {
        //        StoreAnnotationBytes(SerializeAnnotation(annotation));
        //    }
        //}

        #region Store one annotation
        public void StoreAnnotation(Annotation annotation)
        {
            StoreAnnotationBytes(annotation.Id.ToString(), SerializeAnnotation(annotation));
        }

        public void RemoveAnnotation(Annotation annotation)
        {
            RemoveAnnotation(annotation.Id.ToString());
        }
        #endregion

        #region Store/get all annotations
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-save-load-and-print-richtextbox-content?view=netframeworkdesktop-4.8
        /// </summary>
        /// <param name="streamStore"></param>
        public void GetAnnotationStream(XmlStreamStore streamStore)
        {
            foreach (UbAnnotationInfo annotationInfo in AnnotationsInfo)
            {
                MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(annotationInfo.AnnotationsString));
                XmlStreamStore store = new XmlStreamStore(mStream);
                streamStore.AddAnnotation(store.GetAnnotations().First());
            }
        }

        #endregion


    }
}
