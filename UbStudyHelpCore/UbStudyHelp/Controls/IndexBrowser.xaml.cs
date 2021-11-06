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
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            DataEntry.Index = Index;
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



        private void SetFontSize()
        {
            App.Appearance.SetFontSize(IndexResults);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(IndexResults);
        }



        #region Events

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void DataEntry_ShowIndexDetails(string indexEntry)
        {
            CreateWebIndexPage(indexEntry);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion

    }
}
