using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Documents;

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







        private void btChange_Click(object sender, RoutedEventArgs e)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/flow-content?view=netframeworkdesktop-4.8
            FlowDocument doc = new FlowDocument();

            FlowDocument document = new FlowDocument();
            string html = "<p> The <b> Markup </b> that is to be converted.</p>";
            TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);
            textRange.Text = html;
            Document.Document = document;

            //for (int i= 0; i<100; i++)
            //{
            //    Paragraph p = new Paragraph(new Run("Hello, world  " + i.ToString()));
            //    p.FontSize = 18;
            //    doc.Blocks.Add(p);
            //    p = new Paragraph(new Run("The ultimate programming greeting!"));
            //    p.FontSize = 14;
            //    p.FontStyle = FontStyles.Italic;
            //    p.TextAlignment = TextAlignment.Left;
            //    p.Foreground = Brushes.Gray;
            //    doc.Blocks.Add(p);
            //}
            //Document.Document = doc;

        }
    }
}

