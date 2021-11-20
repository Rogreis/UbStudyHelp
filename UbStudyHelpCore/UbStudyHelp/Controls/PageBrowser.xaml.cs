﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for PageBrowser.xaml
    /// </summary>
    public partial class PageBrowser : UserControl
    {

        private HtmlCommandsPage commands = new HtmlCommandsPage();

        private TOC_Entry lastEntry = new TOC_Entry(0,1,0);

        private bool lastShouldHighlightText = false;


        public PageBrowser()
        {
            InitializeComponent();

            this.Loaded += PageBrowser_Loaded;
            EventsControl.TOCClicked += EventsControl_TOCClicked;
            EventsControl.TrackSelected += EventsControl_TrackSelected;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.SeachClicked += EventsControl_SeachClicked;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.TranslationsChanged += EventsControl_TranslationsChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }




        /// <summary>
        /// Create and show html for a full paper
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="addToTrack"></param>
        private void Show(TOC_Entry entry, bool shouldHighlightText= false)
        {
            // Keep latest pragraph shown for next program section
            App.ParametersData.Entry= new TOC_Entry(entry);
            lastEntry = new TOC_Entry(entry);
            lastShouldHighlightText = shouldHighlightText;

            EventsControl.FireSendMessage("Paper: " + entry.ToString());
            string htmlPage = commands.HtmlLine(entry, shouldHighlightText);
            BrowserText.NavigateToString(htmlPage);
        }

        private void Refresh()
        {
            Show(lastEntry, lastShouldHighlightText);
        }


        private void PageBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Show(App.ParametersData.Entry);
        }


        private void EventsControl_TOCClicked(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_TrackSelected(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_SeachClicked(TOC_Entry entry)
        {
            Show(entry, true);
        }

        private void EventsControl_IndexClicked(TOC_Entry entry)
        {
            Show(entry);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            Show(App.ParametersData.Entry);
        }


        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            Show(App.ParametersData.Entry);
        }

        private void EventsControl_TranslationsChanged()
        {
            Refresh();
        }



    }
}
