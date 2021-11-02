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

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbTrackPage.xaml
    /// </summary>
    public partial class UbTrackPage : Page
    {

        public UbTrackPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.SeachClicked += EventsControl_SeachClicked;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.TOCClicked += EventsControl_TOCClicked;
        }

        public void Initialize()
        {
            ChangeFont();
            TrackList.ItemsSource = App.ParametersData.TrachEntries;
        }


        private void AddEntry(TOC_Entry entry)
        {
            App.ParametersData.TrachEntries.Add(entry.Ident);
        }


        private void EventsControl_TOCClicked(TOC_Entry entry)
        {
            AddEntry(entry);
        }

        private void EventsControl_IndexClicked(TOC_Entry entry)
        {
            AddEntry(entry);
        }

        private void EventsControl_SeachClicked(TOC_Entry entry)
        {
            AddEntry(entry);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            //throw new NotImplementedException();
        }

        private void ChangeFont()
        {
            App.Appearance.SetFontSize(LabelTrackList);
            App.Appearance.SetFontSize(TrackList);
        }


        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            ChangeFont();
        }
    }
}
