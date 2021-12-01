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
using UbStudyHelp.Controls;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbSearchResults.xaml
    /// </summary>
    public partial class UbSearchResults : Page
    {
        public UbSearchResults()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockSearchResults);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TextBlockSearchResults);
        }

        public void ShowSearchResults(SearchData data, int nrPage, int pageSize, int totalPages)
        {
            TextBlockSearchResults.Inlines.Clear();
            data.GetInlinesText(TextBlockSearchResults.Inlines, nrPage, pageSize, totalPages);
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
        #endregion


        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
        }


    }
}
