﻿using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using UbStudyHelp.Classes;
using UbStudyHelp.Pages;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for LeftPainel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {
        UbTocPage ubTocPage = new UbTocPage();
        UbIndexPage ubIndexPage = new UbIndexPage();
        UbSearchPage ubSearchPage = new UbSearchPage();
        UbTrackPage ubTrackPage = new UbTrackPage();
        SearchHelpPage searchHelpPage = new SearchHelpPage();
        SettingsPage optionsPage = new SettingsPage();

        public LeftPanel()
        {
            InitializeComponent();
            this.Loaded += LeftPainel_Loaded;
            ubTocPage.Initialize();
            ubIndexPage.Initialize();
            ubSearchPage.Initialize();
            ubTrackPage.Initialize();
            searchHelpPage.Initialize();
            optionsPage.Initialize();
        }

        private void LeftPainel_Loaded(object sender, RoutedEventArgs e)
        {
            // Default page
            FrameControl.Navigate(ubTocPage);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            HamburgerMenuGlyphItem menuItem = (HamburgerMenuGlyphItem)args.InvokedItem;
            string targetView = menuItem.Tag.ToString();

            switch(targetView)
            {
                case "IncreaseFontSize":
                    App.ParametersData.FontSizeInfo++;
                    EventsControl.FireFontChanged();
                    args.Handled = true;
                    break;
                case "DecreaseFontSize":
                    App.ParametersData.FontSizeInfo--;
                    EventsControl.FireFontChanged();
                    args.Handled = true;
                    break;
                case "UbTocPage":
                    FrameControl.Navigate(ubTocPage);
                    break;
                case "UbIndexPage":
                    FrameControl.Navigate(ubIndexPage);
                    break;
                case "UbSearchPage":
                    FrameControl.Navigate(ubSearchPage);
                    break;
                case "UbTrackPage":
                    FrameControl.Navigate(ubTrackPage);
                    break;
                case "SearchHelpPage":
                    FrameControl.Navigate(searchHelpPage);
                    break;
                case "OptionsPage":
                    FrameControl.Navigate(optionsPage);
                    break;
            }

        }

    }
}
