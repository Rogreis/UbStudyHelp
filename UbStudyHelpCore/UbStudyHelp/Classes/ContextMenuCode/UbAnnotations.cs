using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
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

        private FlowDocumentScrollViewer CurrentDocViewer = null;

        private bool OkToRun { get; set; } = true;

        private TOC_Entry Entry { get; set; }

        // Stores the local annotations list
        public UbAnnotationsStoreDataCore AnnotationsStoreDataCore { get; private set; }


        //private TOC_Entry GetCurrentEntry()
        //{
        //    try
        //    {
        //        TextPointer pointer = CurrentDocViewer.Selection.Start;
        //        System.Windows.Documents.Paragraph p = pointer.Paragraph;
        //        if (p == null) return null;
        //        ParagraphSearchData data = p.Tag as ParagraphSearchData;
        //        return data.Entry;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


        public UbAnnotations(UbAnnotationsStoreDataCore annotationsStoreDataCore, TOC_Entry entry)
        {
            if (annotationsStoreDataCore == null)
            {
                StaticObjects.Logger.Warn("UbAnnotationsStoreData null while construction UbAnnotations object");
                OkToRun = false;
                return;
            }
            if (entry == null)
            {
                StaticObjects.Logger.Warn("Entry null while construction UbAnnotations object");
                OkToRun = false;
                return;
            }
            OkToRun = true;
            AnnotationsStoreDataCore = annotationsStoreDataCore;
            Entry = TOC_Entry.CreateEntry(entry, entry.TranslationId);
            Entry.Text = ""; // Text is not needed
        }


        #region Store annotations

        private void _annotStore_StoreContentChanged(object sender, StoreContentChangedEventArgs e)
        {
            if (!OkToRun)
            {
                return;
            }

            switch (e.Action)
            {
                case StoreContentAction.Added:
                    // A single annotation list is kept in this object
                    AnnotationResource resource = new AnnotationResource("Entry");
                    resource.Contents.Add(Entry.Xml);
                    e.Annotation.Cargos.Add(resource);
                    AnnotationsStoreDataCore.StoreAnnotation(e.Annotation);
                    break;

                case StoreContentAction.Deleted:
                    AnnotationsStoreDataCore.Annotations.Remove(e.Annotation);
                    break;
            }
        }

        #endregion



        #region Start - Stop Annotations
        /// <summary>
        /// Enables annotations and displays all that are viewable.</summary>
        /// </summary>
        /// <param name="docViewer"></param>
        /// <param name="annotationsBytes"></param>
        /// <returns></returns>
        public AnnotationService StartAnnotations(FlowDocumentScrollViewer docViewer, TOC_Entry entry)
        {
            this.Entry = entry;
            CurrentDocViewer = docViewer;


            AnnotationService annotService = new AnnotationService(docViewer);
            _annotService = annotService;

            // If the AnnotationService is currently enabled, disable it.
            if (annotService.IsEnabled == true)
                annotService.Disable();

            // Create an AnnotationStore using the file stream.
            MemoryStreamAnnotations = new MemoryStream();
            _annotStore = new XmlStreamStore(MemoryStreamAnnotations);

            // Restore the stored annotations
            AnnotationsStoreDataCore.GetAnnotationStream(_annotStore);

            // This event only can be set after loading the existing annotations
            _annotStore.StoreContentChanged += _annotStore_StoreContentChanged;

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

        #endregion



    }
}
