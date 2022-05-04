using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{


    internal class UbAnnotations
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.annotations.annotationservice.createtextstickynotecommand?view=windowsdesktop-6.0

        private AnnotationService _annotService = null;
        private XmlStreamStore _annotStore = null;
        private MemoryStream MemoryStreamAnnotations = null;
        private List<Annotation> Annotations = new List<Annotation>();

        public UbAnnotationType AnnotationType { get; private set; }
        public TOC_Entry Entry { get; set; }

        private byte[] serializeAnnotation(Annotation annotation)
        {
            MemoryStream stream = new MemoryStream();
            XmlStreamStore annotationStore = new XmlStreamStore(stream);
            annotationStore.AddAnnotation(annotation);
            annotationStore.Flush();
            byte[] serializedAnnotation = stream.ToArray();
            return serializedAnnotation;
        }

        private Annotation deSerializeAnnotation(byte[] data)
        {
            MemoryStream mStream = new MemoryStream(data);
            XmlStreamStore store = new XmlStreamStore(mStream);
            return store.GetAnnotations().First();
        }


        private void _annotStore_StoreContentChanged(object sender, StoreContentChangedEventArgs e)
        {
            switch (e.Action)
            {
                case StoreContentAction.Added:
                    AnnotationResource resource = new AnnotationResource();
                    resource.Contents.Add(Entry.Xml);
                    e.Annotation.Cargos.Add(resource);
                    Annotations.Add(e.Annotation);
                    break;

                case StoreContentAction.Deleted:
                    Annotations.Remove(e.Annotation);
                    break;
            }
        }

        public UbAnnotations(UbAnnotationType annotationType)
        {
            AnnotationType = annotationType;
        }


        /// <summary>
        /// Enables annotations and displays all that are viewable.</summary>
        /// </summary>
        /// <param name="docViewer"></param>
        /// <param name="annotationsBytes"></param>
        /// <returns></returns>
        public AnnotationService StartAnnotations(FlowDocumentScrollViewer docViewer)
        {
            AnnotationService annotService = new AnnotationService(docViewer);
            _annotService = annotService;

            // If the AnnotationService is currently enabled, disable it.
            if (annotService.IsEnabled == true)
                annotService.Disable();

            // Create an AnnotationStore using the file stream.
            MemoryStreamAnnotations = new MemoryStream();
            _annotStore = new XmlStreamStore(MemoryStreamAnnotations);
            _annotStore.StoreContentChanged += _annotStore_StoreContentChanged;

            UbAnnotationsStoreData data = StaticObjects.Book.GetUbAnnotationsStoreData(Entry, AnnotationType);

            if (data != null && data.AnnotationsStrings.Count > 0)
            {
                foreach (string annotationString in data.AnnotationsStrings)
                {
                    Annotation annotation = deSerializeAnnotation(Encoding.UTF8.GetBytes(annotationString));
                    _annotStore.AddAnnotation(annotation);
                }
            }
            // Enable the AnnotationService using the new store.
            annotService.Enable(_annotStore);
            return annotService;
        }// end:StartAnnotations()


        public void Store()
        {
            UbAnnotationsStoreData data = new UbAnnotationsStoreData()
            {
                AnnotationType = this.AnnotationType
            };
            foreach(Annotation annotation in Annotations)
            {
                data.StoreAnnotation(serializeAnnotation(annotation));
            }
            data.Entry = Entry;
            EventsControl.FireAnnotationChanged(data);
        }

        // ------------------------ StopAnnotations ---------------------------
        /// <summary>
        ///   Disables annotation processing and hides all annotations.</summary>
        public void StopAnnotations()
        {
            // If the AnnotationStore is active, flush and close it.
            if (_annotStore != null)
            {
                _annotStore.Flush();
                //_annotStream.Flush();
                //_annotStream.Close();
                _annotStore = null;
            }

            // If the AnnotationService is active, shut it down.
            if (_annotService != null)
            {
                if (_annotService.IsEnabled)
                {
                    _annotService.Disable();  // Disable the AnnotationService.
                    _annotService = null;
                }
            }
        }// end:StopAnnotations()
    }
}
