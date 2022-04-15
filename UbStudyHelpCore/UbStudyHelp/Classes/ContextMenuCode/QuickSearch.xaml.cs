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
using System.Windows.Shapes;
using UbStandardObjects;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class QuickSearch : Window
    {
        public QuickSearch()
        {
            InitializeComponent();

            GeometryImages images = new GeometryImages();

            this.Loaded += QuickSearch_Loaded;

            //        
            ButtonCancelSearchImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchSearchImage.Source = images.GetImage(GeometryImagesTypes.Search);

            ButtonCancelSimilarImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchSimilarImage.Source = images.GetImage(GeometryImagesTypes.Search);

            ButtonCancelCloseImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchCloseImage.Source = images.GetImage(GeometryImagesTypes.Search);

            BorderThickness = new Thickness(2.0);
            BorderBrush = App.Appearance.GetHighlightColorBrush();
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.ToolWindow;
            Owner = Application.Current.MainWindow;

        }

        private void QuickSearch_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDefaultData();
        }

        private string SearchFor { get => TextBoxSearchFor.Text; set => TextBoxSearchFor.Text = value; }

        private string SimilarSearchFor { get => TextBoxSimilarSearchFor.Text; set => TextBoxSimilarSearchFor.Text = value; }

        private string FirstWord { get => ComboFirstWord.Text; set => ComboFirstWord.Text = value; }

        private string SecondWord { get => ComboSecondWord.Text; set => ComboSecondWord.Text = value; }


        private List<string> _closeSearchWords = new List<string>();
        private List<string> CloseSearchWords
        {
            get
            {
                return _closeSearchWords;
            }
            set
            {
                _closeSearchWords = value;
                ComboFirstWord.ItemsSource = _closeSearchWords;
                ComboSecondWord.ItemsSource = _closeSearchWords;
                if (_closeSearchWords.Count > 0)
                {
                    FirstWord = SearchFor = SimilarSearchFor = CloseSearchWords[0];
                }
                if (_closeSearchWords.Count > 1)
                {
                    SecondWord = CloseSearchWords[1];
                }
            }
        }

        private string Distance { get => SliderDistance.Value.ToString(); set => SliderDistance.Value = Convert.ToDouble(value); }

        /// <summary>
        /// Indicates whether it is right tranlation
        /// </summary>
        public bool IsRightTranslation { get; set; }

        private void LoadDefaultData()
        {
            SearchFor = StaticObjects.Parameters.SearchFor;
            SimilarSearchFor = StaticObjects.Parameters.SimilarSearchFor;
            // Stored values recovered only when no current data
            if (CloseSearchWords.Count == 0)
            {
                CloseSearchWords = StaticObjects.Parameters.CloseSearchWords;
                FirstWord = StaticObjects.Parameters.CloseSearchFirstWord;
                SecondWord = StaticObjects.Parameters.CloseSearchSecondWord;
            }
            Distance = StaticObjects.Parameters.CloseSearchDistance;
        }

        private void SaveDefaultData()
        {
            StaticObjects.Parameters.SearchFor = SearchFor;
            StaticObjects.Parameters.SimilarSearchFor = SimilarSearchFor;
            StaticObjects.Parameters.CloseSearchDistance = Distance;
            StaticObjects.Parameters.CloseSearchWords = CloseSearchWords;
            StaticObjects.Parameters.CloseSearchFirstWord = FirstWord;
            StaticObjects.Parameters.CloseSearchSecondWord = SecondWord;
        }


        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Search(QuickSearchType type)
        {
            SaveDefaultData();

            ParagraphSearchData data = new ParagraphSearchData();
            data.IsRightTranslation = IsRightTranslation;
            switch (type)
            {
                case QuickSearchType.Quick:
                    data.QueryString = SearchFor;
                    break;
                case QuickSearchType.Similar:
                    data.QueryString = SimilarSearchFor + "~";
                    break;
                case QuickSearchType.Close:
                    data.QueryString = $"\"{FirstWord} {SecondWord}\"~{Distance}";
                    break;
            }

            EventsControl.FireSendMessage($"Use Right Translation= {data.IsRightTranslation}  QueryString= {data.QueryString}");
            EventsControl.FireDirectSearch(data);

            this.DialogResult = true;
            this.Close();
        }

        private void ButtonSearchSearch_Click(object sender, RoutedEventArgs e)
        {
            Search(QuickSearchType.Quick);
        }

        private void ButtonSearchSimilar_Click(object sender, RoutedEventArgs e)
        {
            Search(QuickSearchType.Similar);
        }

        private void ButtonSearchClose_Click(object sender, RoutedEventArgs e)
        {
            Search(QuickSearchType.Close);
        }

        /// <summary>
        /// Set the internal data
        /// </summary>
        /// <param name="parts"></param>
        public void SetData(string[] parts)
        {
            if (IsRightTranslation)
            {
                ButtonSearchCloseText.Text = ButtonSearchSearchText.Text = ButtonSearchSimilarText.Text = "Search " + StaticObjects.Book.RightTranslation.Description;
            }
            else
            {
                ButtonSearchCloseText.Text = ButtonSearchSearchText.Text = ButtonSearchSimilarText.Text = "Search " + StaticObjects.Book.LeftTranslation.Description;
            }
            if (parts == null || parts.Length == 0)
            {
                return;
            }
            CloseSearchWords = new List<string>(parts);
        }


    }
}
