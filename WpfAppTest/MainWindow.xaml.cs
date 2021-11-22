using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
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
