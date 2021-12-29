using ControlzEx.Theming;
using Microsoft.Windows.Themes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        List<string> ThemmeColors = new List<string>();

        public SettingsPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            ToggleSwitchThemme.Toggled += ToggleSwitchThemme_Toggled;
            ToggleSwitchShowParIdent.Toggled += ToggleSwitchShowParIdent_Toggled;
            ToggleSwitchBilingual.Toggled += ToggleSwitchBilingual_Toggled;

            ThemmeColors.Add("Green");
            ThemmeColors.Add("Blue");
            ThemmeColors.Add("Purple");
            ThemmeColors.Add("Orange");
            ThemmeColors.Add("Lime");
            ThemmeColors.Add("Emerald");
            ThemmeColors.Add("Teal");
            ThemmeColors.Add("Cyan");
            ThemmeColors.Add("Cobalt");
            ThemmeColors.Add("Indigo");
            ThemmeColors.Add("Violet");
            ThemmeColors.Add("Pink");
            ThemmeColors.Add("Red");
            ThemmeColors.Add("Magenta");
            ThemmeColors.Add("Crimson");
            ThemmeColors.Add("Amber");
            ThemmeColors.Add("Yellow");
            ThemmeColors.Add("Brown");
            ThemmeColors.Add("Olive");
            ThemmeColors.Add("Steel");
            ThemmeColors.Add("Mauve");
            ThemmeColors.Add("Taupe");
            ThemmeColors.Add("Sienna");
            ThemmeColors.Sort();
            ComboTheme.ItemsSource = ThemmeColors;
            ComboTheme.SelectedItem = App.ParametersData.ThemeColor;
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(LabelLeftTranslations);
            App.Appearance.SetFontSize(LabelRightTranslations);
            App.Appearance.SetFontSize(LabelColorTheme);
            App.Appearance.SetFontSize(ComboLeftTranslations);
            App.Appearance.SetFontSize(ComboRightTranslation);
            App.Appearance.SetFontSize(ComboTheme);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(LabelLeftTranslations);
            App.Appearance.SetThemeInfo(LabelRightTranslations);
            App.Appearance.SetThemeInfo(LabelColorTheme);
            App.Appearance.SetThemeInfo(ComboLeftTranslations);
            App.Appearance.SetThemeInfo(ComboRightTranslation);
            App.Appearance.SetThemeInfo(ComboTheme);
        }


        private void SetTheme()
        {
            string theme = App.ParametersData.ThemeName + "." + App.ParametersData.ThemeColor;
            ThemeManager.Current.ChangeTheme(Application.Current, theme);
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

        private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBoxItem item = ComboTheme.SelectedItem as ComboBoxItem;
            string item = ComboTheme.SelectedItem as string;
            if (item == null)
            {
                return;
            }
            //App.ParametersData.ThemeColor = (string)item.Content;
            App.ParametersData.ThemeColor = item;
            SetTheme();
            EventsControl.FireAppearanceChanged();
        }

        private void ComboLeftTranslations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translation trans = (sender as ComboBox).SelectedItem as Translation;
            App.ParametersData.LanguageIDLeftTranslation = trans.LanguageID;
            EventsControl.FireTranslationsChanged();
        }

        private void ComboRightTranslation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translation trans = (sender as ComboBox).SelectedItem as Translation;
            App.ParametersData.LanguageIDRightTranslation = trans.LanguageID;
            EventsControl.FireTranslationsChanged();
        }

        private void ToggleSwitchThemme_Toggled(object sender, RoutedEventArgs e)
        {
            App.Appearance.Theme = ToggleSwitchThemme.IsOn ? "Dark" : "Light";
        }

        private void ToggleSwitchShowParIdent_Toggled(object sender, RoutedEventArgs e)
        {
            App.ParametersData.ShowParagraphIdentification = ToggleSwitchShowParIdent.IsOn;
            EventsControl.FireRefreshText();
        }

        private void ToggleSwitchBilingual_Toggled(object sender, RoutedEventArgs e)
        {
            App.ParametersData.ShowBilingual = ToggleSwitchBilingual.IsOn;
            EventsControl.FireBilingualChanged(ToggleSwitchBilingual.IsOn);
        }

        #endregion

        private void SelectComboCurrentTranslation(ComboBox comboBox, short id)
        {
            Translation trans = Book.Translations.Find(t => t.LanguageID == id);
            List<Translation> list = comboBox.ItemsSource as List<Translation>;
            comboBox.SelectedIndex = list.FindIndex(t => t.LanguageID == id);
        }

        public void Initialize()
        {
            SetFontSize();
            SetAppearence();

            //< iconPacks:PackIconIonicons Width = "22"
            //                                       Height = "22"
            //                                       HorizontalAlignment = "Center"
            //                                       VerticalAlignment = "Center"
            //                                       Kind = "SettingsMD" />


            ComboLeftTranslations.ItemsSource = Book.ObservableTranslations;
            ComboRightTranslation.ItemsSource = Book.ObservableTranslations;
            SelectComboCurrentTranslation(ComboLeftTranslations, App.ParametersData.LanguageIDLeftTranslation);
            SelectComboCurrentTranslation(ComboRightTranslation, App.ParametersData.LanguageIDRightTranslation);

            ComboTheme.Text = App.ParametersData.ThemeColor;
            ToggleSwitchThemme.IsOn = App.Appearance.Theme == "Dark";
            ToggleSwitchShowParIdent.IsOn = App.ParametersData.ShowParagraphIdentification;
            ToggleSwitchBilingual.IsOn = App.ParametersData.ShowBilingual;
        }

    }
}
