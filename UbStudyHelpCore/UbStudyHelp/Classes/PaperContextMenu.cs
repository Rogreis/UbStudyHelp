using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStandardObjects;
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

    internal class PaperContextMenu
    {
        private FlowDocumentScrollViewer FlowDocument = null;
        private MenuItem menuItemSearch = null;

        public PaperContextMenu(FlowDocumentScrollViewer document)
        {
            FlowDocument= document;
            //StartAnnotations(FlowDocument);
            AnnotationService annotService = new AnnotationService(FlowDocument);
            document.ContextMenu = CreateCellContextMenu();
            document.ContextMenuOpening += ContextMenu_ContextMenuOpening;
        }

        #region Context Menu

        private MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Command = command;
            return item;
        }

        private MenuItem CreateMenuItem(string header, RoutedEventHandler command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Click += command;
            return item;
        }
        

        public ContextMenu CreateCellContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            //contextMenu.Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
            //contextMenu.Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
            //contextMenu.Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
            //contextMenu.Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
            //contextMenu.Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
            //contextMenu.Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));


            contextMenu.Items.Add(CreateMenuItem("Copy", ItemCopy_Click));
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateMenuItem("Quick Search", ItemQuickSearch_Click));
            menuItemSearch = CreateMenuItem("Search", ItemSearch_Click);
            contextMenu.Items.Add(menuItemSearch);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateMenuItem("Select All", SelectAll_Click));

            return contextMenu;
        }


        private string GetSelectedText()
        {
            TextPointer potStart = FlowDocument.Selection.Start;
            TextPointer potEnd = FlowDocument.Selection.End;
            TextRange range = new TextRange(potStart, potEnd);
            return range.Text;
        }


        //private string GetAllText()
        //{
        //    TextRange range = new TextRange(Document.Document.ContentStart, Document.Document.ContentEnd);
        //    using (Stream stream = new MemoryStream())
        //    {
        //        range.Save(stream, DataFormats.Text);
        //        return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
        //    }
        //}


        private ParagraphSearchData GetCurrentParagraph()
        {
            TextPointer pointer = FlowDocument.Selection.Start;
            System.Windows.Documents.Paragraph p = pointer.Paragraph;
            if (p == null) return null;
            ParagraphSearchData data= p.Tag as ParagraphSearchData;
            return data;
        }

        /// <summary>
        /// Split the currente selected text in words
        /// </summary>
        /// <returns></returns>
        private string[] SplitSelectedText()
        {
            // SOme day check regex:  new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*")  https://stackoverflow.com/questions/10239518/how-do-i-split-a-phrase-into-words-using-regex-in-c-sharp
            string text = GetSelectedText();
            if (text != null && !string.IsNullOrWhiteSpace(text))
            {
                char[] sep = { ' ', '.', ',', ';', ':', '!', '?', '\t' };
                return text.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }


        private void SelectAll()
        {
            TextPointer pointerStart = FlowDocument.Document.ContentStart;
            TextPointer pointerEnd = FlowDocument.Document.ContentEnd;
            FlowDocument.Selection.Select(pointerStart, pointerEnd);

            // Hightlight all:

            //while (pointer != null)
            //{
            //    TextPointerContext tpc= pointer.GetPointerContext(LogicalDirection.Forward);
            //    if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            //    {
            //        string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
            //        // where textRun is the text in the flowDocument
            //        // and searchText is the text that is being searched for
            //        int indexInRun = textRun.IndexOf(searchText);
            //        if (indexInRun >= 0)
            //        {
            //            TextPointer startPos = pointer.GetPositionAtOffset(indexInRun);
            //            TextPointer endPos = pointer.GetPositionAtOffset(indexInRun + searchText.Length);
            //            docViewer.Selection.Select(startPos, endPos);
            //            break;
            //        }
            //    }
            //    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            //}
        }


        
        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
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

        private void ItemCopy_Click(object sender, RoutedEventArgs e)
        {
            TextRange range = new TextRange(FlowDocument.Selection.Start, FlowDocument.Selection.End);
            using (Stream stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Text);
                Clipboard.SetData(DataFormats.Text, Encoding.UTF8.GetString((stream as MemoryStream).ToArray()));
            }
        }


        private void ItemQuickSearch_Click(object sender, RoutedEventArgs e)
        {
            string[] parts = SplitSelectedText();

            ParagraphSearchData data = GetCurrentParagraph();
            QuickSearch quickSearch = new QuickSearch();
            quickSearch.ShowInTaskbar = false;
            quickSearch.SetData(parts);
            quickSearch.IsRightTranslation = data == null ? true : data.IsRightTranslation;
            quickSearch.WindowStyle = WindowStyle.ToolWindow;
            quickSearch.Owner = Application.Current.MainWindow;

            quickSearch.BorderThickness = new Thickness(2.0);
            quickSearch.BorderBrush = App.Appearance.GetHighlightColorBrush();

            //const double MinWidth = 200;
            //const double MinHeight = 200;
            //double width = Application.Current.MainWindow.Width - 50;
            //double height = Application.Current.MainWindow.Height - 30;
            //quickSearch.Width = width > MinWidth ? width : MinWidth;
            //quickSearch.Height = height > MinHeight ? height : MinHeight;

            quickSearch.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            //var point = Mouse.GetPosition(Application.Current.MainWindow);
            //quickSearch.Left = point.X;
            //quickSearch.Top = point.Y;
            quickSearch.ShowDialog();
        }



        private void ItemSearch_Click(object sender, RoutedEventArgs e)
        {
            string textToSearch= menuItemSearch.Tag as string;
            if (textToSearch != null)
            {
                ParagraphSearchData data= GetCurrentParagraph();
                if (data != null)
                {
                    // Fire search command
                    data.QueryString = textToSearch;
                    EventsControl.FireDirectSearch(data);
                }

            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }


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
        private AnnotationService StartAnnotations(FlowDocumentScrollViewer doc)
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



    }
}
