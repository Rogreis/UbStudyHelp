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
using System.Globalization;
using System.Windows.Markup;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lastIcon = -1;

        public MainWindow()
        {
            InitializeComponent();
            CreateImage();

            CanvasIcons.Load();
            NextIcon();
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


        public DrawingImage Convert()
        {


            PackIconFontAwesome icon = new PackIconFontAwesome();
            icon.Kind = PackIconFontAwesomeKind.AdjustSolid;


             Geometry geo = Geometry.Parse(icon.Data);
            GeometryDrawing geoDrawing = new GeometryDrawing();
            geoDrawing.Geometry = geo;
            geoDrawing.Brush = icon.BorderBrush;
            geoDrawing.Pen = new Pen(Brushes.White, 100);
            //geoDrawing.Brush = parameter as Brush ?? Brushes.Black;
            //geoDrawing.Pen = new Pen(geoDrawing.Brush, Thickness);

            
            //if (value is PackIconFontAwesome)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconBootstrapIcons>).Data);
            //else if (value is PackIconMaterial)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconMaterialKind>).Data);
            //else if (value is PackIconMaterialLight)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconEntypoKind>).Data);
            //else if (value is PackIconModern)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconMaterialLightKind>).Data);
            //else if (value is PackIconEntypo)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconEntypoKind>).Data);
            //else if (value is PackIconOcticons)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconOcticonsKind>).Data);

            var drawingGroup = new DrawingGroup { Children = { geoDrawing }, Transform = new ScaleTransform(1, -1) };

            return new DrawingImage { Drawing = drawingGroup };
        }


        private void CreateImage()
        {
            //IconImage.Source = Convert();
        }

        private string[] IconData =
        {
            "M126.5625 281.25A14.0625 14.0625 0 0 0 140.625 267.1875V150A9.375 9.375 0 0 1 159.375 150V197.49375L160.9875 197.56875C166.93125 197.79375 172.93125 197.71875 176.2875 197.0625C178.8 196.55625 181.8000000000001 195.2625 184.6875 193.65C186.13125 192.8625 187.5 190.89375 187.5 187.7625V150A9.375 9.375 0 1 1 206.25 150V179.34375A91.875 91.875 0 0 0 208.2375000000001 179.53125C214.1625000000001 179.98125 219.1875 179.71875 221.5125000000001 178.78125C223.7250000000001 177.91875 227.1375000000001 174.9 230.6250000000001 170.71875C232.1437500000001 168.91875 233.4375000000001 167.15625 234.3750000000001 165.8625V140.625A9.375 9.375 0 0 1 253.1250000000001 140.625V159.375H259.5375A18.75 18.75 0 0 0 278.19375 138.75L273.1125 87.84375A46.875 46.875 0 0 0 267.16875 69.2625L241.0125 23.475A9.375 9.375 0 0 0 232.8750000000001 18.75H113.15625A9.375 9.375 0 0 0 105.35625 22.93125L78.4875 63.24375A28.125 28.125 0 0 0 73.93125 75.73125L67.4625 133.9500000000001A9.375 9.375 0 0 0 74.94375 144.1875L93.75 147.9375V131.25A9.375 9.375 0 0 1 112.5 131.25V267.1875A14.0625 14.0625 0 0 0 126.5625 281.25zM159.375 216.2625V267.1875A32.8125 32.8125 0 1 1 93.75 267.1875V167.0625L71.25 162.5625A28.125 28.125 0 0 1 48.825 131.8875L55.29375 73.65A46.875 46.875 0 0 1 62.8875 52.8375L89.75625 12.525A28.125 28.125 0 0 1 113.15625 0H232.875A28.125 28.125 0 0 1 257.2875 14.175L283.44375 59.94375A65.625 65.625 0 0 1 291.76875 85.9875L296.85 136.89375A37.5 37.5 0 0 1 259.5375 178.125H248.64375A95.8875 95.8875 0 0 1 244.9875 182.775C241.40625 187.06875 235.425 193.425 228.4875 196.2C221.6625 198.9375 212.625 198.675 206.775 198.225L204.3 198A23.625 23.625 0 0 1 193.8 210.0375A49.35 49.35 0 0 1 179.9625 215.4375C174.15 216.6 166.10625 216.525 160.275 216.3L159.375 216.2625zM198.6375 178.29375z",
            "M3,9H17V7H3V9M3,13H17V11H3V13M3,17H17V15H3V17M19,17H21V15H19V17M19,7V9H21V7H19M19,13H21V11H19V13Z"
        };

        private void NextIcon()
        {
            lastIcon++;
            if (lastIcon >= CanvasIcons.Icons.Count)
            {
                lastIcon = 0;
            }
            SvgImage.Source = new Uri("pack://application:,,,/Resources/Settings.svg");
            //SvgImage.Source = new Uri(@"/ImageResTestLib;component/MyData/SomeStuff/Resources/Img.png", UriKind.RelativeOrAbsolute);
            //this.Resources["Width"] = CanvasIcons.Icons[lastIcon].Width;
            //this.Resources["Height"] = CanvasIcons.Icons[lastIcon].Height;
            ////this.Resources["MatrixTransform"] = CanvasIcons.Icons[lastIcon].MatrixTransform;
            //CanvasPath path = new CanvasPath();
            //path.Data = CanvasIcons.Icons[lastIcon].PathData;
            //this.Resources["PathData"] = path;
        }


        private void btChange_Click(object sender, RoutedEventArgs e)
        {
            NextIcon();
        }
    }
}

