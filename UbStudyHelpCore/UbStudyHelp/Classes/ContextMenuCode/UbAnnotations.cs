using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;

namespace UbStudyHelp.Classes.ContextMenuCode
{
    internal class UbAnnotations
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.annotations.annotationservice.createtextstickynotecommand?view=windowsdesktop-6.0

        private AnnotationService _annotService = null;
        private XmlStreamStore _annotStore = null;
        private MemoryStream MemoryStreamAnnotations = null;


        public string XmlAnnotations
        {
            private set
            {
                MemoryStreamAnnotations = new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
            }

            get
            {
                return Encoding.UTF8.GetString(MemoryStreamAnnotations.ToArray());
            }
        }


        // ------------------------ StartAnnotations --------------------------
        /// <summary>
        ///   Enables annotations and displays all that are viewable.</summary>
        public AnnotationService StartAnnotations(FlowDocumentScrollViewer doc, string xmlAnnotations = "")
        {
            AnnotationService annotService = new AnnotationService(doc);

            //_annotStorePath = MakeMyDocumentsAppFolder("UbStudyAidAnnotations.txt");

            // If the AnnotationService is currently enabled, disable it.
            if (annotService.IsEnabled == true)
                annotService.Disable();

            // Open a memory stream for restore/storing annotations.
            XmlAnnotations = xmlAnnotations;
            MemoryStreamAnnotations = new MemoryStream();

            // Create an AnnotationStore using the memory string with the annotations already stored
            _annotStore = new XmlStreamStore(MemoryStreamAnnotations);

            // Enable the AnnotationService using the new store.
            annotService.Enable(_annotStore);
            return annotService;
        }// end:StartAnnotations()

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
