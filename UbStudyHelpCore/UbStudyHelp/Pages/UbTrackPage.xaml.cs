using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UbStudyHelp.Classes;
using Newtonsoft.Json;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbTrackPage.xaml
    /// </summary>
    public partial class UbTrackPage : Page
    {

        private ObservableCollection<TOC_Entry> LocalTrackEntries = new ObservableCollection<TOC_Entry>();

        private enum TrachSortOrder
        {
            None,
            Ascending,
            Descending
        }
        TrachSortOrder sortOrder = TrachSortOrder.None;

        public UbTrackPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            EventsControl.SearchClicked += EventsControl_SeachClicked;
            EventsControl.IndexClicked += EventsControl_IndexClicked;
            EventsControl.TOCClicked += EventsControl_TOCClicked;
            TrackList.SelectionChanged += TrackList_SelectionChanged;
        }

        public void Initialize()
        {
            SetFontSize();
            SetAppearence();
            TrackList.Items.Clear();
            TrackList.ItemsSource = LocalTrackEntries;
            foreach (TOC_Entry entry in App.ParametersData.TrackEntries)
            {
                LocalTrackEntries.Add(entry);
            }
        }


        private void SetFontSize()
        {
            //App.Appearance.SetFontSize(LabelTrackList);
            App.Appearance.SetFontSize(TrackList);
            App.Appearance.SetFontSize(ButtonTrackSort);
            App.Appearance.SetFontSize(ButtonTrackClear);
            App.Appearance.SetFontSize(ButtonTrackSave);
            App.Appearance.SetFontSize(ButtonTrackLoad);
        }

        private void SetAppearence()
        {
            //App.Appearance.SetThemeInfo(LabelTrackList);
            App.Appearance.SetThemeInfo(TrackList);
            App.Appearance.SetThemeInfo(ButtonTrackSort);
            App.Appearance.SetThemeInfo(ButtonTrackClear);
            App.Appearance.SetThemeInfo(ButtonTrackSave);
            App.Appearance.SetThemeInfo(ButtonTrackLoad);
        }

        private void AddEntry(TOC_Entry entry)
        {
            if (LocalTrackEntries.Count == App.ParametersData.MaxExpressionsStored)
            {
                LocalTrackEntries.RemoveAt(LocalTrackEntries.Count - 1);
                App.ParametersData.TrackEntries.RemoveAt(LocalTrackEntries.Count - 1);
            }

            if (string.IsNullOrEmpty(entry.Text))
            {
                Paper paperLeft = Book.LeftTranslation.Paper(entry.Paper);
                Paragraph par = paperLeft.GetParagraph(entry);
                entry.Text = par.ReducedText;
            }

            LocalTrackEntries.Insert(0, entry);
            App.ParametersData.TrackEntries.Insert(0, entry);
        }

        #region events
        private void EventsControl_TOCClicked(TOC_Entry entry)
        {
            AddEntry(entry);
        }

        private void EventsControl_IndexClicked(TOC_Entry entry)
        {
            AddEntry(entry);
        }

        private void EventsControl_SeachClicked(TOC_Entry entry, List<string> Words = null)
        {
            AddEntry(entry);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                TOC_Entry entry = e.AddedItems[0] as TOC_Entry;
                EventsControl.FireTrackSelected(entry);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }


        #endregion


        private void ButtonTrackSort_Click(object sender, RoutedEventArgs e)
        {
            switch(sortOrder)
            {
                case TrachSortOrder.None:
                    LocalTrackEntries.Sort<TOC_Entry>((a, b) => a.CompareTo(b));
                    sortOrder = TrachSortOrder.Ascending;
                    break;
                case TrachSortOrder.Ascending:
                    LocalTrackEntries.Sort<TOC_Entry>((a, b) => a.InverseCompareTo(b));
                    sortOrder = TrachSortOrder.Descending;
                    break;
                case TrachSortOrder.Descending:
                    LocalTrackEntries.Sort<TOC_Entry>((a, b) => a.CompareTo(b));
                    sortOrder = TrachSortOrder.Ascending;
                    break;
            }
        }

        private void ButtonTrackClear_Click(object sender, RoutedEventArgs e)
        {
            LocalTrackEntries.Clear();
            App.ParametersData.TrackEntries.Clear();
        }

        private void ButtonTrackSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            if (string.IsNullOrEmpty(App.ParametersData.LastTrackFileSaved))
            {
                saveFileDlg.FileName = "";
                saveFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                saveFileDlg.FileName = App.ParametersData.LastTrackFileSaved;
            }
            saveFileDlg.DefaultExt = ".yaml";
            saveFileDlg.Filter = "Yaml documents (.yaml)|*.yaml";
            Nullable<bool> result = saveFileDlg.ShowDialog();
            try
            {
                if (result == true)
                {
                    var jsonString = JsonConvert.SerializeObject(App.ParametersData.TrackEntries, Formatting.Indented);
                    File.WriteAllText(saveFileDlg.FileName, jsonString);
                }
            }
            catch { }

        }

        private void ButtonTrackLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrEmpty(App.ParametersData.LastTrackFileSaved))
            {
                openFileDlg.FileName = "";
                openFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                openFileDlg.FileName = App.ParametersData.LastTrackFileSaved;
            }

            openFileDlg.DefaultExt = ".yaml";
            openFileDlg.Filter = "Yaml documents (.yaml)|*.yaml";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    var jsonString = File.ReadAllText(openFileDlg.FileName);
                    App.ParametersData.TrackEntries = JsonConvert.DeserializeObject<List<TOC_Entry>>(jsonString);
                }
                catch
                {
                    App.ParametersData.TrackEntries = new List<TOC_Entry>();
                }

                // Make local copy
                foreach (TOC_Entry entry in App.ParametersData.TrackEntries)
                {
                    LocalTrackEntries.Add(entry);
                }
            }

        }
    }
}
