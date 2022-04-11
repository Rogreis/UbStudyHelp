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

            //        
            ButtonCancelSearchImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchSearchImage.Source = images.GetImage(GeometryImagesTypes.Search);

            ButtonCancelSimilarImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchSimilarImage.Source = images.GetImage(GeometryImagesTypes.Search);

            ButtonCancelCloseImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonSearchCloseImage.Source = images.GetImage(GeometryImagesTypes.Search);

            LoadDefaultData();
        }



        private string SearchFor { get => TextBoxSearchFor.Text; set => TextBoxSearchFor.Text = value; }

        private string SimilarSearchFor { get => TextBoxSimilarSearchFor.Text; set => TextBoxSimilarSearchFor.Text = value; }

        private string BoxFirstWord { get => TextBoxFirstWord.Text; set => TextBoxFirstWord.Text = value; }

        private string SecondWord { get => TextBoxSecondWord.Text; set => TextBoxSecondWord.Text = value; }

        private string Distance { get => SliderDistance.Value.ToString(); set => SliderDistance.Value = Convert.ToDouble(value); }

        /// <summary>
        /// Indicates whether it is right tranlation
        /// </summary>
        public bool IsRightTranslation { get; set; }

        private void LoadDefaultData()
        {
            SearchFor = StaticObjects.Parameters.SearchFor;
            SimilarSearchFor = StaticObjects.Parameters.SimilarSearchFor;
            BoxFirstWord = StaticObjects.Parameters.BoxFirstWord;
            SecondWord = StaticObjects.Parameters.SecondWord;
            Distance = StaticObjects.Parameters.Distance;
        }

        private void SaveDefaultData()
        {
            StaticObjects.Parameters.SearchFor = SearchFor;
            StaticObjects.Parameters.SimilarSearchFor = SimilarSearchFor;
            StaticObjects.Parameters.BoxFirstWord = BoxFirstWord;
            StaticObjects.Parameters.SecondWord = SecondWord;
            StaticObjects.Parameters.Distance = Distance;
        }


        /// <summary>
        /// Set the internal data
        /// </summary>
        /// <param name="parts"></param>
        public void SetData(string[] parts)
        {
            if (parts == null || parts.Length == 0)
            {
                return;
            }
            if (parts.Length > 0)
            {
                BoxFirstWord = SearchFor = SimilarSearchFor = parts[0];
            }
            if (parts.Length > 1)
            {
                SecondWord = parts[1];
            }
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
                    data.QueryString = TextBoxSearchFor.Text;
                    break;
                case QuickSearchType.Similar:
                    data.QueryString = TextBoxSimilarSearchFor.Text + "~";
                    break;
                case QuickSearchType.Close:
                    data.QueryString = $"\"{TextBoxFirstWord.Text} {TextBoxSecondWord.Text}\"~{TextBoxDistance.Text}";
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
    }
}
