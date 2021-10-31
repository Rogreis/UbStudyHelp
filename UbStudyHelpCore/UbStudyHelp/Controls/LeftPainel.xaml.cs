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
    public partial class LeftPainel : UserControl
    {
        private const double colWidthHamburgerMenuClosed = 48;
        private const double colWidthHamburgerMenuOpen = 380;

        public LeftPainel()
        {
            InitializeComponent();
            HamburgerMenuControl.HamburgerButtonClick += HamburgerMenuControl_HamburgerButtonClick;
        }

        private void HamburgerMenuControl_HamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            //EventsControl.FireSendMessage($"{HamburgerMenuControl.HamburgerWidth}");
            ////

            //if (HamburgerMenuControl.IsPaneOpen)
            //{
            //    GridMain.ColumnDefinitions[0].Width = new GridLength(colWidthHamburgerMenuOpen);
            //}
            //else
            //{
            //    GridMain.ColumnDefinitions[0].Width = new GridLength(colWidthHamburgerMenuClosed);
            //}

            //if (GridMain.ColumnDefinitions[0].Width.Value == colWidthHamburgerMenuClosed)
            //    
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            HamburgerMenuIconItem menuItem = (HamburgerMenuIconItem)args.InvokedItem;
            string targetView = menuItem.Tag.ToString();
            FrameControl.Navigate(new Uri(targetView, UriKind.Relative));
        }

    }
}
