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

        // Stores the local annotations list

        private UbAnnotationsStoreDataCore UbAnnotationsLeft = null;
        private UbAnnotationsStoreDataCore UbAnnotationsRight = null;
        private UbAnnotationsStoreDataCore UbAnnotationsParagraph = null;

        private FlowDocumentScrollViewer CurrentDocViewer = null;

        // True when a new anootration is created
        private bool NewAnnotation = true;

        public UbAnnotationType AnnotationType { get; private set; }
        private TOC_Entry Entry { get; set; }


        private TOC_Entry GetCurrentEntry()
        {
            try
            {
                TextPointer pointer = CurrentDocViewer.Selection.Start;
                System.Windows.Documents.Paragraph p = pointer.Paragraph;
                if (p == null) return null;
                ParagraphSearchData data = p.Tag as ParagraphSearchData;
                return data.Entry;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Send all annotations to be stored
        /// </summary>
        private void Store()
        {
            if (NewAnnotation)
            {
                UbAnnotationsStoreSet UbAnnotationsStoreSet = new UbAnnotationsStoreSet();
                switch (AnnotationType)
                {
                    case UbAnnotationType.Paper:
                        UbAnnotationsLeft.Serialize();
                        UbAnnotationsRight.Serialize();
                        UbAnnotationsStoreSet.PaperLeftAnnotations = UbAnnotationsLeft;
                        UbAnnotationsStoreSet.PaperRightAnnotations = UbAnnotationsRight;
                        break;
                    case UbAnnotationType.Paragraph:
                        UbAnnotationsParagraph.Serialize();
                        UbAnnotationsStoreSet.ParagraphAnnotations = UbAnnotationsParagraph;
                        break;
                }

                EventsControl.FireAnnotationChanged(UbAnnotationsStoreSet);
            }
        }

        private void _annotStore_StoreContentChanged(object sender, StoreContentChangedEventArgs e)
        {
            switch (e.Action)
            {
                case StoreContentAction.Added:
                    // A single annotation list is kept in this object
                    AnnotationResource resource = new AnnotationResource();
                    TOC_Entry entry = null;
                    switch (AnnotationType)
                    {
                        case UbAnnotationType.Paper:
                            // For a paper we need to get the curret paragraph
                            entry = GetCurrentEntry();
                            break;
                        case UbAnnotationType.Paragraph:
                            // For a paragraph, the entry is informed is the start anootations
                            entry= Entry;
                            break;
                    }
                    if (entry == null)
                    {
                        throw new Exception("entry null in _annotStore_StoreContentChanged");
                    }
                    entry.Text = ""; // Text is not needed
                    resource.Contents.Add(entry.Xml);
                    e.Annotation.Cargos.Add(resource);

                    switch (AnnotationType)
                    {
                        case UbAnnotationType.Paper:
                            if (entry.TranslationId == StaticObjects.Book.LeftTranslation.LanguageID)
                            {
                                UbAnnotationsLeft.Annotations.Add(e.Annotation);
                            }
                            else
                            {
                                UbAnnotationsRight.Annotations.Add(e.Annotation);
                            }
                            break;
                        case UbAnnotationType.Paragraph:
                            UbAnnotationsParagraph.Annotations.Add(e.Annotation);
                            break;
                    }

                    break;

                case StoreContentAction.Deleted:
                    UbAnnotationsLeft.Annotations.Remove(e.Annotation);
                    break;
            }
            Store();
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
            NewAnnotation = false;
            UbAnnotationsStoreSet ubAnnotationsStoreSet = null;
            switch (AnnotationType)
            {
                case UbAnnotationType.Paper:
                    //ubAnnotationsStoreSet = StaticObjects.Book.GetPaperAnnotations(Entry);
                    //UbAnnotationsLeft = new UbAnnotationsStoreDataCore(ubAnnotationsStoreSet.PaperLeftAnnotations, _annotStore);
                    //UbAnnotationsRight = new UbAnnotationsStoreDataCore(ubAnnotationsStoreSet.PaperRightAnnotations, _annotStore);

                    break;
                case UbAnnotationType.Paragraph:
                    ubAnnotationsStoreSet = StaticObjects.Book.GetParagraphAnnotations(Entry);
                    UbAnnotationsParagraph = new UbAnnotationsStoreDataCore(ubAnnotationsStoreSet.ParagraphAnnotations, _annotStore);
                    break;
            }
            NewAnnotation = true;

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
    }
}
