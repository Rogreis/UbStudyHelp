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
    /// Interaction logic for UbSearchPage.xaml
    /// </summary>
    public partial class UbSearchPage : Page
    {

        public UbSearchPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            SearchDataEntry.ShowSearchResults += SearchDataEntry_ShowSearchResults; 
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockSearchResults);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TextBlockSearchResults);
        }

        private void ShowSearchResults(SearchData data)
        {
            TextBlockSearchResults.Inlines.Clear();
            data.GetInlinesText(TextBlockSearchResults.Inlines);
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

        private void SearchDataEntry_ShowSearchResults(SearchData data)
        {
            ShowSearchResults(data);
        }

        #endregion


        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
            SearchDataEntry.Initialize();
        }
    }
}
