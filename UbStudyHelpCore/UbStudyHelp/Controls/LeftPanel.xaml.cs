using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using UbStandardObjects;
using UbStudyHelp.Classes;
using UbStudyHelp.Pages;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for LeftPainel.xaml
    /// </summary>
    public partial class LeftPanel : UserControl
    {
        UbTocPage ubTocPage = null;
        UbIndexPage ubIndexPage = null;
        UbSearchPage ubSearchPage = null;
        UbTrackPage ubTrackPage = null;
        SearchHelpPage searchHelpPage = null;
        SettingsPage optionsPage = null;
        NotesPage notesPage = null;

        public LeftPanel()
        {
            InitializeComponent();
            this.Loaded += LeftPainel_Loaded;
            EventsControl.DirectSearch += EventsControl_DirectSearch;
        }


        /// <summary>
        /// >Fired ehen text context menu search is called
        /// </summary>
        /// <param name="textToSearch"></param>
        /// <param name="useRightTranslation"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void EventsControl_DirectSearch(ParagraphSearchData data)
        {
            FrameControl.Navigate(ubSearchPage);
        }

        private void LeftPainel_Loaded(object sender, RoutedEventArgs e)
        {
            // Default page
            StaticObjects.Logger.Info("»»»» LeftPainel_Loaded");
            FrameControl.Navigate(ubTocPage);
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs args)
        {
            HamburgerMenuGlyphItem menuItem = (HamburgerMenuGlyphItem)args.InvokedItem;
            string targetView = menuItem.Tag.ToString();

            switch (targetView)
            {
                case "IncreaseFontSize":
                    if (StaticObjects.Parameters.FontSizeInfo < 22)
                    {
                        StaticObjects.Parameters.FontSizeInfo++;
                        EventsControl.FireFontChanged();
                    }
                    else
                    {
                        EventsControl.FireSendMessage("Max font size reached.");
                    }
                    args.Handled = true;
                    break;
                case "DecreaseFontSize":
                    if (StaticObjects.Parameters.FontSizeInfo > 10)
                    {
                        StaticObjects.Parameters.FontSizeInfo--;
                        EventsControl.FireFontChanged();
                    }
                    else
                    {
                        EventsControl.FireSendMessage("Min font size reached.");
                    }
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
                case "UbNotes":
                    FrameControl.Navigate(notesPage);
                    break;
            }

        }

        public void Init()
        {
            ubTocPage = new UbTocPage();
            ubIndexPage = new UbIndexPage();
            ubSearchPage = new UbSearchPage();
            ubTrackPage = new UbTrackPage();
            searchHelpPage = new SearchHelpPage();
            optionsPage = new SettingsPage();
            notesPage = new NotesPage();

            ubTocPage.Initialize();
            ubIndexPage.Initialize();
            ubSearchPage.Initialize();
            ubTrackPage.Initialize();
            searchHelpPage.Initialize();
            optionsPage.Initialize();
            notesPage.Initialize();
            StaticObjects.Logger.Info("»»»» LeftPainel created");


        }

    }
}
