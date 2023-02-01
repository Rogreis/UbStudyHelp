using System;
using System.Windows.Controls;
using UbStandardObjects;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbIndexPage.xaml
    /// </summary>
    public partial class UbIndexPage : Page
    {

        // Object to manipulate the index
        private UbStudyHelp.Classes.Index Index = new UbStudyHelp.Classes.Index();

        public UbIndexPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.OpenNewIndexEntry += EventsControl_OpenNewIndexEntry;
            DataEntry.Index = Index;
            DataEntry.ShowIndexDetails += DataEntry_ShowIndexDetails;
            if (!Index.Load())
            {
                StaticObjects.Logger.NonFatalError("Index not lodaded");
            }
        }


        private void SetFontSize()
        {
        }

        private void SetAppearence()
        {
        }

        private void CreateWebIndexPage(string indexEntry)
        {
            if (indexEntry == null)
            {
                IndexBrowserInstance.Clear();
                return;
            }
            TubIndex index = Index.GetIndexEntry(indexEntry);
            EventsControl.FireSendMessage($"New index entry shown: {indexEntry}");
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

        private void EventsControl_OpenNewIndexEntry(string indexEntry)
        {
            CreateWebIndexPage(indexEntry);
        }

        #endregion

        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
        }
    }
}
