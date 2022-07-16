using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{
    internal class UbNotesContextMenu : UbContextMenu
    {


        public UbNotesContextMenu(TOC_Entry entry)
        {
            Entry = entry;
            CreateContextMenu(entry);
        }


        protected void ItemOpenNotes(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem= e.Source as MenuItem;
            TOC_Entry entry = menuItem.Tag as TOC_Entry;
            if (entry != null)
            {
                AnnotationsWindow annotationsWindow = new AnnotationsWindow(entry, true);
                annotationsWindow.Show();
            }
        }

        protected void ItemTextAndNotes(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            TOC_Entry entry = menuItem.Tag as TOC_Entry;
            if (entry != null)
            {
                EventsControl.FireTrackSelected(entry);
                AnnotationsWindow annotationsWindow = new AnnotationsWindow(entry, true);
                annotationsWindow.Show();
            }
        }

        protected void ItemDeleteNotes(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            TOC_Entry entry = menuItem.Tag as TOC_Entry;
            if (entry != null)
            {
                StaticObjects.Book.DeleteAnnotations(entry);
            }
        }

        private MenuItem CreateMenuItem(string header, RoutedEventHandler command, TOC_Entry entry)
        {
            MenuItem menuItem = CreateMenuItem(header, command);
            menuItem.Tag= entry;
            return menuItem;
        }

        protected void CreateContextMenu(TOC_Entry entry)
        {

            Items.Add(CreateMenuItem("Open Notes", ItemOpenNotes, entry));
            Items.Add(CreateMenuItem("Open Text and Notes", ItemTextAndNotes, entry));
            Items.Add(new Separator());
            Items.Add(CreateMenuItem("Delete", ItemDeleteNotes, entry));
        }


    }
}
