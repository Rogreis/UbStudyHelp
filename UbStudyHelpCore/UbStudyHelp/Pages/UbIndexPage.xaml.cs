using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbIndexPage.xaml
    /// </summary>
    public partial class UbIndexPage : Page
    {

        // Object to manipulate the index
        private UbStudyHelp.Classes.Index Index = new UbStudyHelp.Classes.Index(App.BaseTubFilesPath);

        public UbIndexPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            DataEntry.Index = Index;
            DataEntry.ShowIndexDetails += DataEntry_ShowIndexDetails;
            if (!Index.Load())
                return;
        }

        private void SetFontSize()
        {
        }

        private void SetAppearence()
        {
        }

        private void CreateWebIndexPage(string indexEntry)
        {
            TubIndex index = Index.GetIndexEntry(indexEntry);
            IndexBrowserInstance.CreateWebIndexPage(index);
        }


        #region events
        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void DataEntry_ShowIndexDetails(string indexEntry)
        {
            CreateWebIndexPage(indexEntry);
        }

        #endregion

        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
            IndexBrowserInstance.HeaderHeight = LeftMenuTopControl.ActualHeight + DataEntry.ActualHeight;
            IndexBrowserInstance.HeaderWidth = LeftMenuTopControl.ActualWidth + DataEntry.ActualWidth;


        }




    }
}
