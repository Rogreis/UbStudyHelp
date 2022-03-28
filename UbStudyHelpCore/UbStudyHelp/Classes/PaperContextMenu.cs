using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace UbStudyHelp.Classes
{
    internal class PaperContextMenu
    {

        private MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Command = command;
            return item;
        }

        public void SetClipboard(string value)
        {
            if (value == null)
                throw new ArgumentNullException("Attempt to set clipboard with null");

            Process clipboardExecutable = new Process();
            clipboardExecutable.StartInfo = new ProcessStartInfo // Creates the process
            {
                RedirectStandardInput = true,
                FileName = @"clip",
            };
            clipboardExecutable.Start();

            clipboardExecutable.StandardInput.Write(value); // CLIP uses STDIN as input.
            // When we are done writing all the string, close it so clip doesn't wait and get stuck
            clipboardExecutable.StandardInput.Close();

            return;
        }


        public void CreateCellContextMenu(TableCell cell)
        {
            ContextMenu contextMenu = new ContextMenu();

            contextMenu.Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
            contextMenu.Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
            contextMenu.Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
            contextMenu.Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
            contextMenu.Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
            contextMenu.Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));


            MenuItem itemCopy = new MenuItem();
            MenuItem itemSearch = new MenuItem();

            //I have about 10 items
            //...
            itemCopy.Header = "Copy";
            itemCopy.Click += ItemCopy_Click;
            contextMenu.Items.Add(itemCopy);

            itemSearch.Header = "Search";
            itemSearch.Click += ItemSearch_Click;
            contextMenu.Items.Add(itemSearch);

            contextMenu.ContextMenuOpening += ContextMenu_ContextMenuOpening;
            contextMenu.DataContext = cell;

            cell.ContextMenu = contextMenu;
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/annotations-overview?view=netframeworkdesktop-4.8
            ContextMenu contextMenu = sender as ContextMenu;
            TableCell cell = contextMenu.DataContext as TableCell;
            //Paragraph p= cell.Blocks.FirstBlock. as Paragraph;

        }

        private void ItemCopy_Click(object sender, RoutedEventArgs e)
        {
            EventsControl.FireSendMessage("Copy");
        }

        private void ItemSearch_Click(object sender, RoutedEventArgs e)
        {
            EventsControl.FireSendMessage("Menu 2");
        }



    }
}
