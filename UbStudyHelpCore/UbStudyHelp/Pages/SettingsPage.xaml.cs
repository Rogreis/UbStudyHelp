using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Lucene.Net.Documents.Field;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            ToggleSwitchThemme.Toggled += ToggleSwitchThemme_Toggled;
            ToggleSwitchShowParIdent.Toggled += ToggleSwitchShowParIdent_Toggled;
            ToggleSwitchBilingual.Toggled += ToggleSwitchBilingual_Toggled;
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
            ComboBoxItem item = ComboTheme.SelectedItem as ComboBoxItem;
            App.ParametersData.ThemeColor = (string)item.Content;
            SetTheme();
            EventsControl.FireAppearanceChanged();
        }

        private void ToggleSwitchThemme_Toggled(object sender, RoutedEventArgs e)
        {
            App.Appearance.Theme = ToggleSwitchThemme.IsOn ? "Dark" : "Light";
        }

        private void ToggleSwitchShowParIdent_Toggled(object sender, RoutedEventArgs e)
        {
            App.ParametersData.ShowParIdent = ToggleSwitchShowParIdent.IsOn;
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

        private void ToggleSwitchBilingual_Toggled(object sender, RoutedEventArgs e)
        {
            App.ParametersData.ShowBilingual = ToggleSwitchBilingual.IsOn;
            EventsControl.FireTranslationsChanged();
        }

        #endregion

        private void SelectComboCurrentTranslation(ComboBox comboBox, short id)
        {
            Translation trans= Book.Translations.Find(t => t.LanguageID == id);
            List<Translation> list = comboBox.ItemsSource as List<Translation>;
            comboBox.SelectedIndex= list.FindIndex(t => t.LanguageID == id);
        }

        public void Initialize()
        {
            SetFontSize();
            SetAppearence();

            ComboLeftTranslations.ItemsSource = Book.ObservableTranslations;
            ComboRightTranslation.ItemsSource = Book.ObservableTranslations;
            SelectComboCurrentTranslation(ComboLeftTranslations, App.ParametersData.LanguageIDLeftTranslation);
            SelectComboCurrentTranslation(ComboRightTranslation, App.ParametersData.LanguageIDRightTranslation);

            ComboTheme.Text = App.ParametersData.ThemeColor;
            ToggleSwitchThemme.IsOn = App.Appearance.Theme == "Dark";
            ToggleSwitchShowParIdent.IsOn = App.ParametersData.ShowParIdent;
            ToggleSwitchBilingual.IsOn= App.ParametersData.ShowBilingual;
        }

    }
}
