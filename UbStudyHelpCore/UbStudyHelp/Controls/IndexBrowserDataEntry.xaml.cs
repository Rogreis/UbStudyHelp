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

namespace UbStudyHelp.Controls
{
    public delegate void dlShowIndexDetails(string indexEntry);

    /// <summary>
    /// Interaction logic for IndexBrowserDataEntry.xaml
    /// </summary>
    public partial class IndexBrowserDataEntry : UserControl
    {

        public event dlShowIndexDetails ShowIndexDetails = null;

        // Object to manipulate the index
        public UbStudyHelp.Classes.Index Index { get; set; }


        public IndexBrowserDataEntry()
        {
            InitializeComponent();

            // Events
            this.KeyDown += IndexBrowserDataEntry_KeyDown;
            this.Loaded += IndexBrowserDataEntry_Loaded;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            TextBoxIndexLetters.KeyDown += TextBoxIndexLetters_KeyDown;
            ComboBoxIndexSearch.DropDownClosed += ComboBoxIndexSearch_DropDownClosed;
        }


        private void FillComboBoxIndexEntry(string indexEntry)
        {
            if (!Index.Load() || indexEntry.Length < 2)
                return;

            List<string> list = Index.Search(indexEntry);
            if (list == null || list.Count == 0)
            {
                ComboBoxIndexSearch.IsEnabled = false;
                return;
            }
            int maxItems = Math.Min(list.Count, App.ParametersData.MaxExpressionsStored);
            ComboBoxIndexSearch.Items.Clear();
            for (int i = 0; i < maxItems; i++)
            {
                ComboBoxIndexSearch.Items.Add(list[i]);
            }
            App.ParametersData.IndexLetters = indexEntry;
            ComboBoxIndexSearch.SelectedIndex = 0;
            ComboBoxIndexSearch.IsEnabled = true;
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetFontSize(TextBoxIndexLetters);
            App.Appearance.SetFontSize(ComboBoxIndexSearch);
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetHeight(TextBoxIndexLetters);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TextBlockLabelIndex);
            App.Appearance.SetThemeInfo(TextBoxIndexLetters);
            App.Appearance.SetThemeInfo(ComboBoxIndexSearch);
            App.Appearance.SetThemeInfo(TextBlockLabelIndex);
            App.Appearance.SetThemeInfo(TextBoxIndexLetters);
        }


        #region events
        private void CheckKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxIndexLetters.Text.Trim();
                FillComboBoxIndexEntry(text);
            }
        }


        private void IndexBrowserDataEntry_Loaded(object sender, RoutedEventArgs e)
        {
            TextBoxIndexLetters.Text = App.ParametersData.IndexLetters;
            string text = TextBoxIndexLetters.Text.Trim();
            if (text.Length > 1)
            {
                FillComboBoxIndexEntry(text);
            }
        }


        private void ComboBoxIndexSearch_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboBoxIndexSearch.Text.Trim();
            ShowIndexDetails?.Invoke(text);
        }

        private void TextBoxIndexLetters_KeyDown(object sender, KeyEventArgs e)
        {
            CheckKeyDown(e);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }


        private void IndexBrowserDataEntry_KeyDown(object sender, KeyEventArgs e)
        {
            CheckKeyDown(e);
        }

        #endregion

    }
}
