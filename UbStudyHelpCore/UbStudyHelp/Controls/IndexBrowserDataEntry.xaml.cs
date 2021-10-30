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
    public delegate void dlShowIndexHelp();
    public delegate void dlShowIndexDetails(string indexEntry);

    /// <summary>
    /// Interaction logic for IndexBrowserDataEntry.xaml
    /// </summary>
    public partial class IndexBrowserDataEntry : UserControl
    {

        public event dlShowIndexHelp ShowIndexHelp = null;
        public event dlShowIndexDetails ShowIndexDetails = null;

        // Object to manipulate the index
        public UbStudyHelp.Classes.Index Index { get; set; }


        public IndexBrowserDataEntry()
        {
            InitializeComponent();

            // Events
            this.KeyDown += IndexBrowserDataEntry_KeyDown;
            EventsControl.FontChanged += EventsControl_FontChanged;
            TextBoxIndexLetters.KeyDown += TextBoxIndexLetters_KeyDown;
            ComboBoxIndexSearch.DropDownClosed += ComboBoxIndexSearch_DropDownClosed;
            ButtonHelp.Click += ButtonHelp_Click;
        }


        /// <summary>
        /// Make the search help visible
        /// </summary>
        private void ShowHelp()
        {
            ShowIndexHelp?.Invoke();
        }

        private void FillComboBoxIndexEntry(string indexEntry)
        {
            if (!Index.Load())
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
            ComboBoxIndexSearch.SelectedIndex = 0;
            ComboBoxIndexSearch.IsEnabled = true;
            App.ParametersData.IndexLetters = indexEntry;
        }


        private void UpdateFont()
        {
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetFontSize(TextBoxIndexLetters);
            App.Appearance.SetFontSize(ComboBoxIndexSearch);
            App.Appearance.SetFontSize(TextBlockLabelIndex);
            App.Appearance.SetHeight(TextBoxIndexLetters);
            App.Appearance.SetHeight(ComboBoxIndexSearch);
        }

        private void CheckKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxIndexLetters.Text.Trim();
                if (text.Length > 1)
                {
                    FillComboBoxIndexEntry(text);
                }
            }
            if (e.Key == Key.F1)
            {
                ShowHelp();
            }
        }


        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            ShowHelp();
        }

        private void ComboBoxIndexSearch_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboBoxIndexSearch.Text.Trim();
            if (text.Length > 1)
            {
                ShowIndexDetails?.Invoke(text);
            }
        }

        private void TextBoxIndexLetters_KeyDown(object sender, KeyEventArgs e)
        {
            CheckKeyDown(e);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            UpdateFont();
        }

        private void IndexBrowserDataEntry_KeyDown(object sender, KeyEventArgs e)
        {
            CheckKeyDown(e);
        }

    }
}
