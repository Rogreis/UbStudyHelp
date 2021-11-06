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
    /// <summary>
    /// Interaction logic for SearchDataEntry.xaml
    /// </summary>
    public partial class SearchDataEntry : UserControl
    {

        private LuceneBookSearch luceneLeft = null;

        private LuceneBookSearch luceneRight = null;

        public event dlShowSearchResults ShowSearchResults = null;

        public SearchDataEntry()
        {
            InitializeComponent();
            this.Loaded += SearchDataEntry_Loaded;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.FontChanged += EventsControl_FontChanged;
            // 
        }

        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
            luceneLeft = new LuceneBookSearch(App.BaseTubFilesPath, Book.LeftTranslation);
            luceneRight = new LuceneBookSearch(App.BaseTubFilesPath, Book.RightTranslation);

            TooglePart1.IsOn= App.ParametersData.SimpleSearchIncludePartI;
            TooglePart2.IsOn = App.ParametersData.SimpleSearchIncludePartII;
            TooglePart3.IsOn = App.ParametersData.SimpleSearchIncludePartIII;
            TooglePart4.IsOn = App.ParametersData.SimpleSearchIncludePartIV;
            TooglePartCurrentPaper.IsOn = App.ParametersData.SimpleSearchCurrentPaperOnly;

            foreach(string searchString in App.ParametersData.SearchStrings)
            {
                ComboWhatToSearch.Items.Add(searchString);
            }
            if (ComboWhatToSearch.Items.Count > 0)
            {
                ComboWhatToSearch.SelectedIndex = 0;
            }
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockWhatToSearch);
            App.Appearance.SetFontSize(TextBlockSearchIn);
            App.Appearance.SetFontSize(ButtonSearchSort);
            App.Appearance.SetFontSize(ButtonSearchLeftText);
            App.Appearance.SetFontSize(ButtonSearchRightText);
            App.Appearance.SetFontSize(TooglePart1);
            App.Appearance.SetFontSize(TooglePart2);
            App.Appearance.SetFontSize(TooglePart3);
            App.Appearance.SetFontSize(TooglePart4);
            App.Appearance.SetFontSize(TooglePartCurrentPaper);
            App.Appearance.SetFontSize(ComboWhatToSearch);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(TextBlockWhatToSearch);
            App.Appearance.SetThemeInfo(TextBlockSearchIn);
            App.Appearance.SetThemeInfo(TextBlockWhatToSearch);
            App.Appearance.SetThemeInfo(ComboWhatToSearch);
        }

        /// <summary>
        /// Get screen data before start querying
        /// </summary>
        /// <returns></returns>
        private SearchData GetData()
        {
            SearchData data = new SearchData();
            data.Part1Included = TooglePart1.IsOn;
            data.Part2Included = TooglePart2.IsOn;
            data.Part3Included = TooglePart3.IsOn;
            data.Part4Included = TooglePart4.IsOn;
            data.CurrentPaperOnly = TooglePartCurrentPaper.IsOn;
            data.CurrentPaper = App.ParametersData.Entry.Paper;
            data.QueryString = ComboWhatToSearch.Text;

            // Store data for next session
            App.ParametersData.SimpleSearchIncludePartI = data.Part1Included;
            App.ParametersData.SimpleSearchIncludePartII = data.Part2Included;
            App.ParametersData.SimpleSearchIncludePartIII = data.Part3Included;
            App.ParametersData.SimpleSearchIncludePartIV = data.Part4Included;
            App.ParametersData.SimpleSearchCurrentPaperOnly = data.CurrentPaperOnly;
            if (App.ParametersData.SearchStrings.Contains(data.QueryString))
            {
                App.ParametersData.SearchStrings.Remove(data.QueryString);
            }
            App.ParametersData.SearchStrings.Add(data.QueryString);
            return data;
        }



        #region events
        private void SearchDataEntry_Loaded(object sender, RoutedEventArgs e)
        {
            SetFontSize();
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void TooglePart1_Toggled(object sender, RoutedEventArgs e)
        {
            if (TooglePart1.IsOn)
            {
                TooglePartCurrentPaper.IsOn = false;
            }
        }

        private void TooglePart2_Toggled(object sender, RoutedEventArgs e)
        {
            if (TooglePart2.IsOn)
            {
                TooglePartCurrentPaper.IsOn = false;
            }
        }

        private void TooglePart3_Toggled(object sender, RoutedEventArgs e)
        {
            if (TooglePart3.IsOn)
            {
                TooglePartCurrentPaper.IsOn = false;
            }
        }

        private void TooglePart4_Toggled(object sender, RoutedEventArgs e)
        {
            if (TooglePart4.IsOn)
            {
                TooglePartCurrentPaper.IsOn = false;
            }
        }

        private void TooglePartCurrentPaper_Toggled(object sender, RoutedEventArgs e)
        {
            if (TooglePartCurrentPaper.IsOn)
            {
                TooglePart1.IsOn = TooglePart2.IsOn = TooglePart3.IsOn = TooglePart4.IsOn = false;
            }

        }

        private SearchData lastData = null;

        private void ButtonSearchSort_Click(object sender, RoutedEventArgs e)
        {
            lastData.SortResults();
            ShowSearchResults?.Invoke(lastData);
        }

        private void ButtonSearchLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ComboWhatToSearch.Text.Trim() == "")
            {
                return;
            }
            SearchData data = GetData();
            luceneLeft.Execute(data);
            lastData = data;
            ShowSearchResults?.Invoke(data);
        }

        private void ButtonSearchRight_Click(object sender, RoutedEventArgs e)
        {
            if (ComboWhatToSearch.Text.Trim() == "")
            {
                return;
            }
            SearchData data = GetData();
            luceneRight.Execute(data);
            lastData = data;
            ShowSearchResults?.Invoke(data);
        }
        #endregion
    }
}
