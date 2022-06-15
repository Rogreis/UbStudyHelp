using System;
using System.Windows;
using System.Windows.Annotations;
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


    public class UbParagraphContextMenu : ContextMenu
    {
        protected FlowDocumentScrollViewer FlowDocument = null;
        protected TOC_Entry Entry = null;
        protected MenuItem menuItemSearch = null;
        protected MenuItem menuItemOpenAnnotations = null;
        private const int MaxWordsForSearch = 20;
        private bool ShowAnnotations = true;
        private bool ShowSearch = true;

  
        public UbParagraphContextMenu(FlowDocumentScrollViewer flowDocument, TOC_Entry entry, bool showAnnotations, bool showSearch)
        {
            ShowAnnotations = showAnnotations;
            ShowSearch = showSearch;
            //this.ContextMenuOpening += UbParagraphContextMenu_ContextMenuOpening;
            Entry = entry;
            FlowDocument = flowDocument;
            FlowDocument.ContextMenuOpening += UbParagraphContextMenu_ContextMenuOpening;
            CreateContextMenu();
        }


        private void UbParagraphContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (menuItemSearch == null)
            {
                return;
            }

            try
            {
                Run r = e.OriginalSource as Run;
                System.Windows.Documents.Paragraph p = r.Parent as System.Windows.Documents.Paragraph;
                ParagraphSearchData paragraphSearchData= p.Tag as ParagraphSearchData;
                if (paragraphSearchData != null)
                {
                    FlowDocument.Tag = paragraphSearchData.Entry;
                    menuItemOpenAnnotations.Tag = paragraphSearchData.Entry;
                }
            }
            catch 
            {
                FlowDocument.Tag = null;
            }


            string[] parts = SplitSelectedText();
            if (parts != null && parts.Length > 0)
            {
                menuItemSearch.Header = "Search for " + parts[0];
                menuItemSearch.Tag = parts[0];
                if (!Items.Contains(menuItemSearch))
                {
                    Items.Add(menuItemSearch);
                }
            }
            else
            {
                try
                {
                    Items.Remove(menuItemSearch);
                }
                catch 
                {
                }
            }
        }


        protected MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Command = command;
            item.VerticalAlignment = VerticalAlignment.Top;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            return item;
        }

        protected MenuItem CreateMenuItem(string header, RoutedEventHandler command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Click += command;
            item.VerticalAlignment = VerticalAlignment.Top;
            item.HorizontalAlignment = HorizontalAlignment.Left;
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
                return text.Split(sep, MaxWordsForSearch, StringSplitOptions.RemoveEmptyEntries);
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

        protected void ItemOpenAnnotations_Click(object sender, RoutedEventArgs e)
        {
            TOC_Entry entry = menuItemOpenAnnotations.Tag as TOC_Entry;
            if (entry != null)
            {
                AnnotationsWindow annotationsWindow = new AnnotationsWindow(entry);
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

        protected void CreateContextMenu()
        {
            if (ShowAnnotations)
            {
                Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
                Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
                Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
                Items.Add(CreateMenuItem("Clear Highlights", AnnotationService.ClearHighlightsCommand));
                Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
                Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));
                Items.Add(new Separator());
            }

            Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            Items.Add(CreateMenuItem("Select All", ApplicationCommands.SelectAll));

            if (ShowSearch)
            {
                Items.Add(new Separator());
                Items.Add(CreateMenuItem("Quick Search", ItemQuickSearch_Click));
                menuItemSearch = CreateMenuItem("Search", ItemSearch_Click);
                //Items.Add(menuItemSearch);
            }

            Items.Add(new Separator());
            menuItemOpenAnnotations = CreateMenuItem("Open Annotations", ItemOpenAnnotations_Click);
            Items.Add(menuItemOpenAnnotations);
        }




    }
}
