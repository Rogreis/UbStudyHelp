using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes.ContextMenuCode;

namespace UbStudyHelp.Classes
{

    internal enum QuickSearchType
    {
        Quick,
        Simple,
        Similar,
        Close
    }

    internal class ParagraphSearchData
    {
        public bool IsRightTranslation { get; set; } = false;
        public string QueryString = "";
        public TOC_Entry Entry { get; set; }
    }


    internal class UbParagraphContextMenu : UbContextMenu
    {
        private MenuItem menuItemSearch = null;
        private MenuItem menuItemOpenAnnotations = null;
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
        }


        private void UbParagraphContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            CreateContextMenu();

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
                AnnotationsWindow annotationsWindow = new AnnotationsWindow(entry, true);
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

        protected void ItemOpenInGitHub_Click(object sender, RoutedEventArgs e)
        {
            ParagraphSearchData data = GetCurrentParagraph();
            if (data != null) 
            {
                string url = $"https://github.com/Rogreis/PtAlternative/blob/correcoes/Doc{data.Entry.Paper:000}/Par_{data.Entry.Paper:000}_{data.Entry.Section:000}_{data.Entry.ParagraphNo:000}.md";
                // Doc016/Par_016_003_16:3-16 (188.4).md
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
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
            Items.Clear();
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
            if (StaticObjects.Book != null && StaticObjects.Book.RightTranslation != null && StaticObjects.Book.RightTranslation.IsEditingTranslation) 
            {
                ParagraphSearchData data = GetCurrentParagraph();
                if (data != null && data.IsRightTranslation)
                    Items.Add(CreateMenuItem("Open in Github", ItemOpenInGitHub_Click));
            }

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
