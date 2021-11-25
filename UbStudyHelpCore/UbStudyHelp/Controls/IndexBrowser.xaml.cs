using MdXaml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStudyHelp.Classes;
using static System.Windows.Forms.LinkLabel;
using Paragraph = System.Windows.Documents.Paragraph;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for UbStudyBrowser.xaml
    /// </summary>
    public partial class IndexBrowser : UserControl
    {

        public IndexBrowser()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.GridSplitterChanged += EventsControl_GridSplitterChanged;
        }

        /// <summary>
        /// Show the details for some index entry
        /// </summary>
        /// <param name="indexEntry"></param>
        public void CreateWebIndexPage(TubIndex index)
        {
            List<IndexTextblockEntry> items = new List<IndexTextblockEntry>();
            foreach (IndexDetails detail in index.Details)
            {
                string links = "";
                double margin = (detail.DetailType - 100) * 5;
                foreach (string link in detail.Links)
                {
                    links += link + " ";
                }
                items.Add(new IndexTextblockEntry(detail.Text, links, margin));
            }
            StackPanelIndexResult.ItemsSource = items;
        }



        private void SetFontSize()
        {
            App.Appearance.SetFontSize(StackPanelIndexResult);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(StackPanelIndexResult);
        }



        #region Events

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            SetFontSize();
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ScrollViewer scv = (ScrollViewer)sender;
            //scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            //e.Handled = true;

            //ListView scv = (ListView)sender;
            //scv.Sc .ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            //e.Handled = true;


        }

        private void EventsControl_GridSplitterChanged(double newWidth)
        {
            //IndexResults.Width= newWidth - 20;
        }


        #endregion

    }
}
