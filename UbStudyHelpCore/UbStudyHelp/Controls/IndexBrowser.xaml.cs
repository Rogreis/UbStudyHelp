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
using UrantiaBook.Classes;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for UbStudyBrowser.xaml
    /// </summary>
    public partial class IndexBrowser : UserControl
    {

        // Object to manipulate the index
        private UrantiaBook.Classes.Index Index = new UrantiaBook.Classes.Index(App.BaseTubFilesPath);


        public IndexBrowser()
        {
            InitializeComponent();

            EventsControl.FontChanged += EventsControl_FontChanged;
            this.Loaded += IndexBrowser_Loaded;
            //BrowserIndex.Navigating += BrowserIndex_Navigating;
            TextBoxIndexLetters.KeyDown += TexTextBoxIndexLettersoxIndexLetters_KeyDown;
            ComboBoxIndexSearch.DropDownClosed += ComboBoxIndexSearch_DropDownClosed;

        }

        #region Events

        private void TexTextBoxIndexLettersoxIndexLetters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxIndexLetters.Text.Trim();
                if (text.Length > 1)
                {
                    FillComboBoxIndexEntry(text);
                }
            }
        }


        private void ComboBoxIndexSearch_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboBoxIndexSearch.Text.Trim();
            if (text.Length > 1)
            {
                CreateWebIndexPage(text);
            }
            
        }


        private void ComboBoxIndexSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void IndexBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            //UpdateFont();
            //TexTextBoxIndexLettersoxIndexLetters.Text= App.ParametersData.IndexLetters;
        }

        public void UpdateFont()
        {
            App.ParametersData.Appearance.SetFontSize(TextBoxIndexLetters);
            App.ParametersData.Appearance.SetFontSize(ComboBoxIndexSearch);
            App.ParametersData.Appearance.SetFontSize(IndexResults);
            App.ParametersData.Appearance.SetHeight(TextBoxIndexLetters);
            App.ParametersData.Appearance.SetHeight(ComboBoxIndexSearch);
        }


        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            UpdateFont();
        }

        private void CreateWebIndexPage(string indexEntry)
        {
            if (!Index.Load())
                return;
            Index.ShowResults(indexEntry, IndexResults);
        }



        #endregion


        private void FillComboBoxIndexEntry(string indexEntry)
        {
            if (!Index.Load())
                return;
            List<string> list = Index.GetAllIndexEntryStartingWith(indexEntry);
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



    }
}
