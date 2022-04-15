using System;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{

    public enum QuickSearchType
    {
        Quick,
        Simple,
        Similar,
        Close
    }

    public class ParagraphSearchData
    {
        public bool IsRightTranslation { get; set; } = false;
        public string QueryString = "";
        public TOC_Entry Entry { get; set; }
    }


    public class UbParagraphContextMenu : ContextMenu
    {
        protected FlowDocumentScrollViewer FlowDocument = null;
        protected TOC_Entry Entry = null;
        protected MenuItem menuItemSearch = null;

        public UbParagraphContextMenu()
        {
            this.ContextMenuOpening += UbParagraphContextMenu_ContextMenuOpening; ;
            CreateContextMenu();
        }


        public UbParagraphContextMenu(TOC_Entry entry)
        {
            this.ContextMenuOpening += UbParagraphContextMenu_ContextMenuOpening; ;
            Entry = entry;
            CreateContextMenu();
        }

        public UbParagraphContextMenu(FlowDocumentScrollViewer flowDocument, TOC_Entry entry)
        {
            this.ContextMenuOpening += UbParagraphContextMenu_ContextMenuOpening;
            Entry = entry;
            FlowDocument = flowDocument;
            CreateContextMenu();
        }


        //public UbParagraphContextMenu(FlowDocumentScrollViewer document)
        //{
        //    FlowDocument = document;
        //    //StartAnnotations(FlowDocument);
        //    AnnotationService annotService = new AnnotationService(FlowDocument);
        //    document.ContextMenu = CreateContextMenu();
        //    document.ContextMenuOpening += ContextMenu_ContextMenuOpening;
        //}


        private void UbParagraphContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (menuItemSearch == null)
            {
                return;
            }

            string[] parts = SplitSelectedText();
            if (parts != null && parts.Length > 0)
            {
                menuItemSearch.Header = "Search for " + parts[0];
                menuItemSearch.Tag = parts[0];
                menuItemSearch.IsEnabled = true;
            }
        }


        protected MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Command = command;
            return item;
        }

        protected MenuItem CreateMenuItem(string header, RoutedEventHandler command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Click += command;
            return item;
        }

        protected string GetSelectedText()
        {
            try
            {
                TextPointer potStart = FlowDocument.Selection.Start;
                TextPointer potEnd = FlowDocument.Selection.End;
                TextRange range = new TextRange(potStart, potEnd);
                return range.Text;
            }
            catch 
            {
                return null;
            }
        }

        //protected string GetAllText()
        //{
        //    TextRange range = new TextRange(Document.Document.ContentStart, Document.Document.ContentEnd);
        //    using (Stream stream = new MemoryStream())
        //    {
        //        range.Save(stream, DataFormats.Text);
        //        return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
        //    }
        //}


        //protected System.Windows.Documents.Paragraph GetParagraphNoSelection()
        //{
        //    TextPointerContext tpc = FlowDocument.Document.ContentStart.GetPointerContext(LogicalDirection.Forward);
        //    System.Windows.Documents.Paragraph p= new System.Windows.Documents.Paragraph();
        //    return p;
        //}


        protected ParagraphSearchData GetCurrentParagraph()
        {
            try
            {
                TextPointer pointer = FlowDocument.Selection.Start;
                System.Windows.Documents.Paragraph p = pointer.Paragraph;
                if (p == null) return null;
                ParagraphSearchData data = p.Tag as ParagraphSearchData;
                return data;
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Split the currente selected text in words
        /// </summary>
        /// <returns></returns>
        protected string[] SplitSelectedText()
        {
            // Some day check regex:  new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*")  https://stackoverflow.com/questions/10239518/how-do-i-split-a-phrase-into-words-using-regex-in-c-sharp
            string text = GetSelectedText();
            if (text != null && !string.IsNullOrWhiteSpace(text))
            {
                char[] sep = { ' ', '.', ',', ';', ':', '!', '?', '\t' };
                return text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }



        //protected void SelectAll()
        //{
        //    TextPointer pointerStart = FlowDocument.Document.ContentStart;
        //    TextPointer pointerEnd = FlowDocument.Document.ContentEnd;
        //    FlowDocument.Selection.Select(pointerStart, pointerEnd);
        //}

        #region Menu item functions


        //protected void ItemCopy_Click(object sender, RoutedEventArgs e)
        //{
        //    TextRange range = new TextRange(FlowDocument.Selection.Start, FlowDocument.Selection.End);
        //    using (Stream stream = new MemoryStream())
        //    {
        //        range.Save(stream, DataFormats.Text);
        //        Clipboard.SetData(DataFormats.Text, Encoding.UTF8.GetString((stream as MemoryStream).ToArray()));
        //    }
        //}

        protected void ItemNewWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Entry != null)
            {
                AnnotationsWindow annotationsWindow = new AnnotationsWindow(Entry);
                annotationsWindow.Show();
            }
        }

        protected void ItemQuickSearch_Click(object sender, RoutedEventArgs e)
        {
            ParagraphSearchData data = GetCurrentParagraph();
            QuickSearch quickSearch = new QuickSearch();
            quickSearch.IsRightTranslation = data == null ? true : data.IsRightTranslation;
            string[] parts = SplitSelectedText();
            quickSearch.SetData(parts);
            quickSearch.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            quickSearch.ShowDialog();
        }



        protected void ItemSearch_Click(object sender, RoutedEventArgs e)
        {
            string textToSearch = menuItemSearch.Tag as string;
            if (textToSearch != null)
            {
                ParagraphSearchData data = GetCurrentParagraph();
                if (data != null)
                {
                    // Fire search command
                    data.QueryString = textToSearch;
                    EventsControl.FireDirectSearch(data);
                }

            }
        }

        //protected void SelectAll_Click(object sender, RoutedEventArgs e)
        //{
        //    SelectAll();
        //}
        #endregion


        #region Annotations

        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.annotations.annotationservice.createtextstickynotecommand?view=windowsdesktop-6.0

        //AnnotationService _annotService = null;
        //FileStream _annotStream = null;
        //string _annotStorePath = "";
        //XmlStreamStore _annotStore = null;


        // ------------------------ StartAnnotations --------------------------
        /// <summary>
        ///   Enables annotations and displays all that are viewable.</summary>
        protected AnnotationService StartAnnotations(FlowDocumentScrollViewer doc)
        {
            AnnotationService annotService = new AnnotationService(doc);

            //// If the AnnotationService is currently enabled, disable it.
            //if (annotService.IsEnabled == true)
            //    annotService.Disable();

            //// Open a stream to the file for storing annotations.
            //_annotStream = new FileStream(
            //    _annotStorePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            //// Create an AnnotationStore using the file stream.
            //_annotStore = new XmlStreamStore(_annotStream);

            //// Enable the AnnotationService using the new store.
            //annotService.Enable(_annotStore);
            return annotService;
        }// end:StartAnnotations()

        #endregion


        protected void CreateContextMenu()
        {
            Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
            Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
            Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
            Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
            Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
            Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));

            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            Items.Add(CreateMenuItem("Select All", ApplicationCommands.SelectAll));
            
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Quick Search", ItemQuickSearch_Click));
            menuItemSearch = CreateMenuItem("Search", ItemSearch_Click);
            Items.Add(menuItemSearch);
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("New Window", ItemNewWindow_Click));
        }




    }
}
