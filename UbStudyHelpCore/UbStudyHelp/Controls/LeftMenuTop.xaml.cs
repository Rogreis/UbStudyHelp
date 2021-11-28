using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{

    partial class UbResourceDictionary : ResourceDictionary
    {


        public UbResourceDictionary()
        {
        }

    }
    //ResourceManager temp = new ResourceManager("UbStudyHelp.Controls.UbResourceDictionary", typeof(UbResourceDictionary).Assembly);

    public static class ConvertBitmapToBitmapImage
    {
        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// </summary>
        /// <param name="src">A bitmap image</param>
        /// <returns>The image as a BitmapImage for WPF</returns>
        public static BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }


    /// <summary>
    /// Interaction logic for LeftMenuTop.xaml
    /// </summary>
    public partial class LeftMenuTop : UserControl
    {
        ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public LeftMenuTop()
        {
            InitializeComponent();
            //this.DataContext = new MainWindowViewModel();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            this.Loaded += LeftMenuTop_Loaded;
            this.Resources["TextToDisplay"] = "No Text";
        }


        private string _imageSourceName = "TocLogo";
        public string ImageSourceName
        {
            get
            {
                return _imageSourceName;
            }
            set
            {

                _imageSourceName = value;
                string toolTip = "";
                string textToDisplay = "";
                switch (_imageSourceName)
                {
                    case "TrackLogo":
                        SvgImage.Source = new Uri("Images/Track.svg", UriKind.Relative);
                        toolTip = "Paragraph's track";
                        textToDisplay = "Visited paragraph's track";
                        break;
                    case "SettingsLogo":
                        SvgImage.Source = new Uri("Images/Settings.svg", UriKind.Relative);
                        toolTip = "Configurations";
                        textToDisplay = "Available configurations";
                        break;
                    case "HelpLogo":
                        SvgImage.Source = new Uri("Images/Help.svg", UriKind.Relative);
                        toolTip = "Help information";
                        textToDisplay = "Help information about some tool's features";
                        break;
                    case "IndexLogo":
                        SvgImage.Source = new Uri("Images/Index.svg", UriKind.Relative);
                        toolTip = "Index of The Urantia Book";
                        textToDisplay = "Subject index for the Urantia Book";
                        break;
                    case "SearchEngineLogo":
                        SvgImage.Source = new Uri("Images/Search.svg", UriKind.Relative);
                        toolTip = "Search engine tool";
                        textToDisplay= "Search Engine tool for the Urantia Book";
                        break;
                    case "TocLogo":
                        SvgImage.Source = new Uri("Images/TOC.svg", UriKind.Relative);
                        toolTip = "Table of contents";
                        textToDisplay = "Table of Contents - papers and sections";
                        break;
                }
                this.Resources["TextToDisplay"] = textToDisplay;
                this.Resources["ToolTip"] = toolTip;
            }
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockCaption);
        }

        private void LeftMenuTop_Loaded(object sender, RoutedEventArgs e)
        {
            SetFontSize();
            App.Appearance.SetThemeInfo(TextBlockCaption);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetThemeInfo(TextBlockCaption);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }



    }
}
