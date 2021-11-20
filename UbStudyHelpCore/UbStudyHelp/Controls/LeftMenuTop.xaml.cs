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
            this.Resources["ImageSourcePath"] = ConvertBitmapToBitmapImage.Convert(UbStudyHelp.Properties.Resources.SearchEngine);
            this.Resources["TextToDisplay"] = "No Text";
        }


        private string _imageSourceName = "Track.jpg";
        public string ImageSourceName
        {
            get
            {
                return _imageSourceName;
            }
            set
            {
                _imageSourceName = value;
                Bitmap bitmap = null;
                string toolTip = "";
                switch (_imageSourceName)
                {
                    case "TrackLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Track;
                        toolTip = "Visited paragraphs history";
                        break;
                    case "SettingsLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Settings;
                        toolTip = "Change the settings of this tool";
                        break;
                    case "HelpLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Help;
                        toolTip = "Help information";
                        break;
                    case "IndexLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Index;
                        toolTip = "Index of The Urantia Book";
                        break;
                    case "SearchEngineLogo":
                        bitmap = UbStudyHelp.Properties.Resources.SearchEngine;
                        toolTip = "Search engine - see help about how to use";
                        break;
                    case "TocLogo":
                        bitmap = UbStudyHelp.Properties.Resources.TOC;
                        toolTip = "Table of contents";
                        break;
                }
                this.Resources["ImageSourcePath"] = ConvertBitmapToBitmapImage.Convert(bitmap);
                this.Resources["ToolTip"] = toolTip;
                
            }
        }

        private string _text = "Track.jpg";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                this.Resources["TextToDisplay"] = value;
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
