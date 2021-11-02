using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<TOC_Entry> LocalTrachEntries = new ObservableCollection<TOC_Entry>();

        public UbTrackPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.SeachClicked += EventsControl_SeachClicked;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.TOCClicked += EventsControl_TOCClicked;
            TrackList.SelectionChanged += TrackList_SelectionChanged;
        }

        private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TOC_Entry entry = e.AddedItems[0] as TOC_Entry;
            EventsControl.FireTrackSelected(entry);
        }

        public void Initialize()
        {
            ChangeFont();
            TrackList.Items.Clear();
            TrackList.ItemsSource = LocalTrachEntries;
            foreach (TOC_Entry entry in App.ParametersData.TrackEntries)
            {
                LocalTrachEntries.Add(entry);
            }
        }


        private void AddEntry(TOC_Entry entry)
        {
            if (LocalTrachEntries.Count == App.ParametersData.MaxTrackItems)
            {
                LocalTrachEntries.RemoveAt(LocalTrachEntries.Count - 1);
                App.ParametersData.TrackEntries.RemoveAt(LocalTrachEntries.Count - 1);
            }
            LocalTrachEntries.Insert(0, entry);
            App.ParametersData.TrackEntries.Insert(0, entry);
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
