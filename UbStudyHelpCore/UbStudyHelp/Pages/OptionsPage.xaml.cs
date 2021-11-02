using ControlzEx.Theming;
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
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        public OptionsPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            ToggleSwitchThemme.Toggled += ToggleSwitchThemme_Toggled;
        }

        public void Initialize()
        {
            ChangeFont();
            ComboTheme.Text = App.ParametersData.ThemeColor;
        }


        private void ChangeFont()
        {
            App.Appearance.SetFontSize(LabelTranslations);
            App.Appearance.SetFontSize(LabelTrack);
            App.Appearance.SetFontSize(LabelThemes);
            App.Appearance.SetFontSize(LabelOptionPage);
            App.Appearance.SetFontSize(LabelColorTheme);

            App.Appearance.SetFontSize(ComboTrack);
            App.Appearance.SetFontSize(ComboTheme);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            ChangeFont();
        }

        private void SetTheme()
        {
            string theme = App.ParametersData.ThemeName + "." + App.ParametersData.ThemeColor;
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
        }


        private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = ComboTheme.SelectedItem as ComboBoxItem;
            App.ParametersData.ThemeColor = (string)item.Content;
            SetTheme();
            EventsControl.FireAppearanceChanged();
        }

        private void ToggleSwitchThemme_Toggled(object sender, RoutedEventArgs e)
        {
            if (App.Appearance.Theme == "Dark")
            {
                App.Appearance.Theme = "Light";
            }
            else
            {
                App.Appearance.Theme = "Dark";
            }
        }


    }
}
