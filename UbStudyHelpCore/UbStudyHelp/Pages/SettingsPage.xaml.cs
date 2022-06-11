using ControlzEx.Theming;
using log4net.Core;
using log4net;
using Microsoft.Windows.Themes;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Text;
using System.ComponentModel;
using System.Diagnostics;

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

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                EventsControl.FontChanged += EventsControl_FontChanged;
                EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
                EventsControl.UpdateAvailable += EventsControl_UpdateAvailable;

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
                ComboTheme.SelectedItem = ((ParametersCore)StaticObjects.Parameters).ThemeColor;
            }

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
            string theme = ((ParametersCore)StaticObjects.Parameters).ThemeName + "." + ((ParametersCore)StaticObjects.Parameters).ThemeColor;
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
            //StaticObjects.Parameters.ThemeColor = (string)item.Content;
            ((ParametersCore)StaticObjects.Parameters).ThemeColor = item;
            SetTheme();
            EventsControl.FireAppearanceChanged();
        }

        private void ComboLeftTranslations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translation trans = (sender as ComboBox).SelectedItem as Translation;
            ((BookCore)StaticObjects.Book).SetNewTranslation(trans, true);
        }

        private void ComboRightTranslation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translation trans = (sender as ComboBox).SelectedItem as Translation;
            ((BookCore)StaticObjects.Book).SetNewTranslation(trans, false);
        }

        private void ToggleSwitchThemme_Toggled(object sender, RoutedEventArgs e)
        {
            App.Appearance.Theme = ToggleSwitchThemme.IsOn ? "Dark" : "Light";
        }

        private void ToggleSwitchShowParIdent_Toggled(object sender, RoutedEventArgs e)
        {
            StaticObjects.Parameters.ShowParagraphIdentification = ToggleSwitchShowParIdent.IsOn;
            EventsControl.FireRefreshText();
        }

        private void ToggleSwitchBilingual_Toggled(object sender, RoutedEventArgs e)
        {
            StaticObjects.Parameters.ShowBilingual = ToggleSwitchBilingual.IsOn;
            EventsControl.FireBilingualChanged(ToggleSwitchBilingual.IsOn);
        }

        /// <summary>
        /// New updates available
        /// </summary>
        /// <param name="updateList"></param>
        private void EventsControl_UpdateAvailable()
        {
            //ButtonUpdateAvailable.Visibility = Visibility.Visible;
        }


        // https://www.urantia.org/MultiLanguageBook
        // https://github.com/Rogreis/UbStudyHelp

        private void ButtonUpdateAvailable_Click(object sender, RoutedEventArgs e)
        {
            // Not implemented yet
        }

        private void ButtonShowLog_Click(object sender, RoutedEventArgs e)
        {
            StaticObjects.Logger.ShowLog();
        }

        private void ButtonCheckNewVersion_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Rogreis/UbStudyHelp/releases") { UseShellExecute = true });
        }

        private void ButtonSupport_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.facebook.com/groups/3162754254046527") { UseShellExecute = true });
        }

        private void ButtonBugsButtonBugs_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/Rogreis/UbStudyHelp/issues") { UseShellExecute = true });
        }



        #endregion

        private void SelectComboCurrentTranslation(ComboBox comboBox, short id)
        {
            Translation trans = StaticObjects.Book.Translations.Find(t => t.LanguageID == id);
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


            ComboLeftTranslations.ItemsSource = StaticObjects.Book.ObservableTranslations;
            ComboRightTranslation.ItemsSource = StaticObjects.Book.ObservableTranslations;
            SelectComboCurrentTranslation(ComboLeftTranslations, StaticObjects.Parameters.LanguageIDLeftTranslation);
            SelectComboCurrentTranslation(ComboRightTranslation, StaticObjects.Parameters.LanguageIDRightTranslation);

            ComboTheme.Text = ((ParametersCore)StaticObjects.Parameters).ThemeColor;
            ToggleSwitchThemme.IsOn = App.Appearance.Theme == "Dark";

            GeometryImages images = new GeometryImages();
            ButtonUpdateAvailableImage.Source = images.GetImage(GeometryImagesTypes.Update);
            ToggleSwitchShowParIdent.IsOn = StaticObjects.Parameters.ShowParagraphIdentification;
            ToggleSwitchBilingual.IsOn = StaticObjects.Parameters.ShowBilingual;
        }

    }
}
