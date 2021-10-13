using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UrantiaBook.Classes;

namespace UbStudyHelp.Classes
{
    public class TreeViewItemUB : TreeViewItem
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.windows.data.bindingoperations.enablecollectionsynchronization?view=windowsdesktop-5.0
        // https://www.codeproject.com/Articles/124644/Basic-Understanding-of-Tree-View-in-WPF

        public TOC_Entry Entry { get; private set; }

        public TreeViewItemUB(TOC_Entry entry)
        {
            Entry = entry;
            Header = entry.Ident;
        }
    }
}
