﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{

    public class UbAnnotationData
    {
        public Annotation Note { get; set; }
        public StoreContentAction Action { get; set; }
        public EbAnnotationType AnnotationType { get; set; }
    }


    public enum EbAnnotationType
    {
        Paper,
        Paragraph
    }


    internal class UbAnnotations
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.annotations.annotationservice.createtextstickynotecommand?view=windowsdesktop-6.0

        private AnnotationService _annotService = null;
        private XmlStreamStore _annotStore = null;
        private MemoryStream MemoryStreamAnnotations = null;

        public EbAnnotationType AnnotationType { get; private set; }
        public TOC_Entry Entry { get; set; }

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


        public UbAnnotations(EbAnnotationType annotationType)
        {
            AnnotationType = annotationType;
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

            _annotStore.StoreContentChanged += _annotStore_StoreContentChanged;

            // Enable the AnnotationService using the new store.
            annotService.Enable(_annotStore);
            return annotService;
        }// end:StartAnnotations()


        private void _annotStore_StoreContentChanged(object sender, StoreContentChangedEventArgs e)
        {
            AnnotationResource resource= new AnnotationResource();
            resource.Contents.Add(Entry.Xml);
            e.Annotation.Cargos.Add(resource);
            UbAnnotationData data = new UbAnnotationData()
            {
                Note = e.Annotation,
                Action = e.Action,
                AnnotationType = this.AnnotationType
            };
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