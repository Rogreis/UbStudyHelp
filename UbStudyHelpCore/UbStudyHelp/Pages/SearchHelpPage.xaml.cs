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
            this.Loaded += SearchHelpPage_Loaded;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetAll(MarkDownDisplay);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetFontSize(MarkDownDisplay);
        }

        private void SearchHelpPage_Loaded(object sender, RoutedEventArgs e)
        {
            MarkDownDisplay.Markdown = Properties.Resources.SearchHelp;
            App.Appearance.SetAll(MarkDownDisplay);
        }
    }
}
