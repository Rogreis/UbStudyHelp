using MdXaml;
using System.Windows;
using System.Windows.Controls;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for SearchHelpPage.xaml
    /// </summary>
    public partial class SearchHelpPage : Page
    {
        public SearchHelpPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        private void SetFontSize()
        {
            App.Appearance.SetFontSize(MarkDownDisplay);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(MarkDownDisplay);
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
            MarkDownDisplay.Markdown = Properties.Resources.SearchHelp;
            App.Appearance.SetThemeInfo(MarkDownDisplay);
        }


    }
}
