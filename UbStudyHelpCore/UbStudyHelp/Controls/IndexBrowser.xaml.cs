using MdXaml;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for UbStudyBrowser.xaml
    /// </summary>
    public partial class IndexBrowser : UserControl
    {

        // Object to manipulate the index
        private UbStudyHelp.Classes.Index Index = new UbStudyHelp.Classes.Index(App.BaseTubFilesPath);


        public IndexBrowser()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            DataEntry.Index = Index;
            DataEntry.ShowIndexHelp += DataEntry_ShowIndexHelp;
            DataEntry.ShowIndexDetails += DataEntry_ShowIndexDetails;
        }

        /// <summary>
        /// Show the details for some index entry
        /// </summary>
        /// <param name="indexEntry"></param>
        private void CreateWebIndexPage(string indexEntry)
        {
            if (!Index.Load())
                return;
            IndexResults.Visibility = Visibility.Visible;
            Index.ShowResults(indexEntry, IndexResults);
        }



        private void UpdateFont()
        {
            App.Appearance.SetFontSize(IndexResults);
        }


        #region Events

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            UpdateFont();
        }

        private void DataEntry_ShowIndexDetails(string indexEntry)
        {
            CreateWebIndexPage(indexEntry);
        }

        /// <summary>
        /// Make the search help visible
        /// </summary>
        private void DataEntry_ShowIndexHelp()
        {
            // TO DO: show the help
        }

        #endregion





    }
}
