using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
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
using UbStudyHelp.Properties;

namespace UbStudyHelp.Controls
{

    partial class UbResourceDictionary : ResourceDictionary
    {


        public UbResourceDictionary()
        {
            //Resources.SearchEngine
        }

    }

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
        ResourceManager temp = new ResourceManager("UbStudyHelp.Controls.UbResourceDictionary", typeof(UbResourceDictionary).Assembly);
        ImageSourceConverter imageSourceConverter = new ImageSourceConverter();

        public LeftMenuTop()
        {
            InitializeComponent();
            //this.DataContext = new MainWindowViewModel();

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
                switch(_imageSourceName)
                {
                    case "TrackLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Track;
                        break;
                    case "SettingsLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Settings;
                        break;
                    case "HelpLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Help;
                        break;
                    case "IndexLogo":
                        bitmap = UbStudyHelp.Properties.Resources.Index;
                        break;
                    case "SearchEngineLogo":
                        bitmap = UbStudyHelp.Properties.Resources.SearchEngine;
                        break;
                    case "TocLogo":
                        bitmap = UbStudyHelp.Properties.Resources.TOC;
                        break;
                }
                this.Resources["ImageSourcePath"] = ConvertBitmapToBitmapImage.Convert(bitmap);
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

        public void SetFontSize()
        {
            App.Appearance.SetFontSize(TextBlockCaption);
        }


    }
}
