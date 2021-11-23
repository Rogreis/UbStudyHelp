using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class ConvertBitmapToBitmapImage
        {
            /// <summary>
            /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
            /// </summary>
            /// <param name="src">A bitmap image</param>
            /// <returns>The image as a BitmapImage for WPF</returns>
            //public BitmapImage Convert(Bitmap src)
            //{
            //    //MemoryStream ms = new MemoryStream();
            //    //((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //    //BitmapImage image = new BitmapImage();
            //    //image.BeginInit();
            //    //ms.Seek(0, SeekOrigin.Begin);
            //    //image.StreamSource = ms;
            //    //image.EndInit();
            //    //return image;

            //    BitmapImage bitmapImage = new BitmapImage(;
            //    using (MemoryStream outStream = new MemoryStream())
            //    {
            //        BitmapEncoder enc = new BmpBitmapEncoder();
            //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            //        enc.Save(outStream);
            //        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

            //        return new Bitmap(bitmap);
            //    }


            //}
        }



        private void InsertInconPackIconInPage()
        {
            var packIconIonicons = new PackIconIonicons()
            {
                Kind = PackIconIoniconsKind.SettingsMD,
                Margin = new Thickness(4, 4, 2, 4),
                Width = 24,
                Height = 24,
                VerticalAlignment = VerticalAlignment.Center
            };

            var x = new MahApps.Metro.IconPacks.PackIconIonicons()
            {
                Width = 48,
                Height = 48,
                Kind = PackIconIoniconsKind.SettingsMD
            };

            // mahapps.metro.logo2.ico

            //ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            //            packIconIonicons.h,
            //            Int32Rect.Empty,
            //            BitmapSizeOptions.FromEmptyOptions());

            //image.Source = packIconIonicons.Kind.h;

            //using (Icon ico = Icon.ExtractAssociatedIcon(packIconIonicons))
            //{
            //    image.Source = Imaging.CreateBitmapSourceFromHIcon(packIconIonicons. .Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //}

        }


    }
}
