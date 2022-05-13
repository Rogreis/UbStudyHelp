using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{
    internal class UbAnnotationsStoreDataCore : UbAnnotationsStoreData
    {
        public List<Annotation> Annotations { get; set; } = new List<Annotation>();

        public UbAnnotationsStoreDataCore(TOC_Entry entry, UbAnnotationType annotationType)
        {
            Entry = entry;
            AnnotationType = annotationType;
        }


        // UbAnnotationsStoreData data
        public UbAnnotationsStoreDataCore(UbAnnotationsStoreData data, XmlStreamStore annotStore)
        {
            Entry = data.Entry;
            AnnotationType = data.AnnotationType;
            foreach (string annotationString in data.AnnotationsStrings)
            {
                try
                {
                    Annotation annotation = DeSerializeAnnotation(annotationString);
                    Annotations.Add(annotation);
                    annotStore.AddAnnotation(annotation);
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


        public void Serialize()
        {
            AnnotationsStrings = new List<string>();
            foreach (Annotation annotation in Annotations)
            {
                StoreAnnotation(SerializeAnnotation(annotation));
            }
        }

    }
}
