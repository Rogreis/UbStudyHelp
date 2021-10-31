using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for LeftPainel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {

        public LeftPanel()
        {
            InitializeComponent();
            this.Loaded += LeftPainel_Loaded;
        }

        private void LeftPainel_Loaded(object sender, RoutedEventArgs e)
        {
            // Default page
            FrameControl.Navigate(new Uri(@"Pages\UbTocPage.xaml", UriKind.Relative));
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            HamburgerMenuIconItem menuItem = (HamburgerMenuIconItem)args.InvokedItem;
            string targetView = menuItem.Tag.ToString();

            switch(targetView)
            {
                case "IncreaseFontSize":
                    App.ParametersData.FontSizeInfo++;
                    EventsControl.FireFontChanged();
                    break;
                case "DecreaseFontSize":
                    App.ParametersData.FontSizeInfo--;
                    EventsControl.FireFontChanged();
                    break;
                default:
                    FrameControl.Navigate(new Uri(targetView, UriKind.Relative));
                    break;
            }

        }

    }
}
