using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
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

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbSearchPage.xaml
    /// </summary>
    public partial class UbSearchPage : Page
    {

        UbSearchResults searchResults = new UbSearchResults();

        private int pageSize = 20;
        private int noPages = 0;
        private int currentPage = 0;
        private SearchData data = null;

        public UbSearchPage()
        {
            InitializeComponent();
            SearchDataEntry.ShowSearchResults += SearchDataEntry_ShowSearchResults; 
        }

        private void SetButtonsStatus()
        {
            if (data.SearchResults.Count > 0)
            {
                btFirst.Visibility = Visibility.Visible;
                btPrevious.Visibility = Visibility.Visible;
                btNext.Visibility = Visibility.Visible;
                btLast.Visibility = Visibility.Visible;

                btFirst.IsEnabled = currentPage > 1;
                btPrevious.IsEnabled = btFirst.IsEnabled;
                btNext.IsEnabled = currentPage < noPages;
                btLast.IsEnabled = btNext.IsEnabled;
            }
            else
            {
                btFirst.IsEnabled = btPrevious.IsEnabled = btNext.IsEnabled = btLast.IsEnabled = false;
            }
        }

        bool starting = false;
        private void ShowSearchResults(SearchData data, int nextPage)
        {
            // When starting zero results must be shown
            if (!starting)
            {
                if (nextPage < 1 || nextPage > noPages)
                    return;
            }
            starting = false;
            currentPage = nextPage;
            this.data = data;
            searchResults.ShowSearchResults(data, nextPage, pageSize, noPages);
            FrameControl.Navigate(searchResults);
            FrameControl.Tag = currentPage;
            SetButtonsStatus();
        }

        #region events
        private void SearchDataEntry_ShowSearchResults(SearchData data)
        {
            noPages = (int)Math.Ceiling((decimal)data.SearchResults.Count / pageSize);
            starting = true;
            ShowSearchResults(data, 1);
        }

        #endregion


        public void Initialize()
        {
            SearchDataEntry.Initialize();
        }

        private void FrameControl_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //this.NavigationService.FragmentNavigation += NavigationService_FragmentNavigation;
            //currentPage++;
            //searchResults.ShowSearchResults(data, currentPage);
            //FrameControl.Tag = currentPage;
            //FrameControl.Navigate(searchResults);
        }

        private void NavigationService_FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            //Debug.WriteLine(e.Fragment.ToString());
        }

        private void btFirst_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchResults(data, 1);
        }

        private void btPrevious_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchResults(data, currentPage - 1);
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchResults(data, currentPage + 1);
        }

        private void btLast_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchResults(data, noPages);
        }
    }
}
