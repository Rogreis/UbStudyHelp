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
            this.Loaded += QuickSearch_Loaded;

            GeometryImages images = new GeometryImages();
            ButtonSearchImage.Source = images.GetImage(GeometryImagesTypes.Search);
            ButtonCancelImage.Source = images.GetImage(GeometryImagesTypes.Clear);
        }

        /// <summary>
        /// Indicates whether it is right tranlation
        /// </summary>
        public bool IsRightTranslation { get; set; }

        private void QuickSearch_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
