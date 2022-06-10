using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using UbStandardObjects;
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

        private ObservableCollection<string> LocalSearchStringsEntries = new ObservableCollection<string>();

        public SearchDataEntry()
        {
            InitializeComponent();
            this.Loaded += SearchDataEntry_Loaded;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.BilingualChanged += EventsControl_BilingualChanged;
            EventsControl.TranslationsChanged += EventsControl_TranslationsChanged;
            EventsControl.DirectSearch += EventsControl_DirectSearch;

        }

        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
            luceneLeft = new LuceneBookSearch(App.BaseTubFilesPath, StaticObjects.Book.LeftTranslation);
            luceneRight = new LuceneBookSearch(App.BaseTubFilesPath, StaticObjects.Book.RightTranslation);

            TooglePart1.IsOn= StaticObjects.Parameters.SimpleSearchIncludePartI;
            TooglePart2.IsOn = StaticObjects.Parameters.SimpleSearchIncludePartII;
            TooglePart3.IsOn = StaticObjects.Parameters.SimpleSearchIncludePartIII;
            TooglePart4.IsOn = StaticObjects.Parameters.SimpleSearchIncludePartIV;
            TooglePartCurrentPaper.IsOn = StaticObjects.Parameters.SimpleSearchCurrentPaperOnly;

            foreach (string entry in StaticObjects.Parameters.SearchStrings)
            {
                LocalSearchStringsEntries.Add(entry);
            }
            ComboWhatToSearch.ItemsSource = LocalSearchStringsEntries;
            if (ComboWhatToSearch.Items.Count > 0)
            {
                ComboWhatToSearch.SelectedIndex = 0;
            }
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockWhatToSearch);
            App.Appearance.SetFontSize(TextBlockSearchIn);
            App.Appearance.SetFontSize(ButtonSearchLeftText);
            App.Appearance.SetFontSize(ButtonSearchRightText);
            App.Appearance.SetFontSize(TooglePart1);
            App.Appearance.SetFontSize(TooglePart2);
            App.Appearance.SetFontSize(TooglePart3);
            App.Appearance.SetFontSize(TooglePart4);
            App.Appearance.SetFontSize(TooglePartCurrentPaper);
            App.Appearance.SetFontSize(ComboWhatToSearch);

            GeometryImages images = new GeometryImages();
            SortButtonImageIcon.Source = images.GetImage(GeometryImagesTypes.Sort);
            ButtonSearchLeftImage.Source = images.GetImage(GeometryImagesTypes.Search);
            ButtonSearchRightImage.Source = images.GetImage(GeometryImagesTypes.Search);

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
            data.CurrentPaper = StaticObjects.Parameters.Entry.Paper;
            data.QueryString = ComboWhatToSearch.Text;

            // Store data for next session
            StaticObjects.Parameters.SimpleSearchIncludePartI = data.Part1Included;
            StaticObjects.Parameters.SimpleSearchIncludePartII = data.Part2Included;
            StaticObjects.Parameters.SimpleSearchIncludePartIII = data.Part3Included;
            StaticObjects.Parameters.SimpleSearchIncludePartIV = data.Part4Included;
            StaticObjects.Parameters.SimpleSearchCurrentPaperOnly = data.CurrentPaperOnly;

            ((ParametersCore)StaticObjects.Parameters).AddEntry(StaticObjects.Parameters.SearchStrings, LocalSearchStringsEntries, data.QueryString);


            //if (StaticObjects.Parameters.SearchStrings.Contains(data.QueryString))
            //{
            //    StaticObjects.Parameters.SearchStrings.Remove(data.QueryString);
            //}
            //StaticObjects.Parameters.SearchStrings.Add(data.QueryString);
            //FillComboWhatToSearch();
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

        private void EventsControl_BilingualChanged(bool ShowBilingual)
        {
            ButtonSearchRight.IsEnabled = ShowBilingual;
        }


        private SearchData lastData = null;

        private void ButtonSearchSort_Click(object sender, RoutedEventArgs e)
        {
            if (lastData != null)
            {
                lastData.SortResults();
                ShowSearchResults?.Invoke(lastData);
            }
        }

        private void ButtonSearchLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ComboWhatToSearch.Text.Trim() == "")
            {
                return;
            }
            SearchData data = GetData();
            if (luceneLeft.Translation.LanguageID != StaticObjects.Book.LeftTranslation.LanguageID)
            {
                luceneLeft = new LuceneBookSearch(App.BaseTubFilesPath, StaticObjects.Book.LeftTranslation);
            }
            if (luceneLeft.Execute(data))
            {
                lastData = data;
                ShowSearchResults?.Invoke(data);
            }
            ButtonSearchSort.IsEnabled = data.SearchResults.Count > 0;
        }

        private void ButtonSearchRight_Click(object sender, RoutedEventArgs e)
        {
            if (ComboWhatToSearch.Text.Trim() == "")
            {
                return;
            }
            SearchData data = GetData();
            if (luceneRight.Translation.LanguageID != StaticObjects.Book.RightTranslation.LanguageID)
            {
                luceneRight = new LuceneBookSearch(App.BaseTubFilesPath, StaticObjects.Book.RightTranslation);
            }
            if (luceneRight.Execute(data))
            {
                lastData = data;
                ShowSearchResults?.Invoke(data);
            }
            ButtonSearchSort.IsEnabled = data.SearchResults.Count > 0;
        }

        private void EventsControl_DirectSearch(ParagraphSearchData searchData)
        {
            if (searchData.QueryString.Trim() == "")
            {
                return;
            }
            ComboWhatToSearch.Text = searchData.QueryString;
            SearchData data = GetData();
            data.QueryString = searchData.QueryString;
            if (searchData.IsRightTranslation)
            {
                if (luceneRight.Execute(data))
                {
                    lastData = data;
                    ShowSearchResults?.Invoke(data);
                }
            }
            else
            {
                if (luceneLeft.Execute(data))
                {
                    lastData = data;
                    ShowSearchResults?.Invoke(data);
                }
            }
            ButtonSearchSort.IsEnabled = data.SearchResults.Count > 0;
        }


        private void EventsControl_TranslationsChanged()
        {
            ButtonSearchLeftText.Text= "Search " + StaticObjects.Book.LeftTranslation.Description;
            ButtonSearchRightText.Text = "Search " + StaticObjects.Book.RightTranslation.Description;
        }

        #endregion
    }
}
