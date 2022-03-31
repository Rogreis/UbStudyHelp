using Lucene.Net.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using Paragraph = UbStandardObjects.Objects.Paragraph;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for UbTrackPage.xaml
    /// </summary>
    public partial class UbTrackPage : Page
    {

        private FlowDocumentFormat format = new FlowDocumentFormat();

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
        }

        public void Initialize()
        {
            SetFontSize();
            SetAppearence();
            ShowTrackData();
        }


        /// <summary>
        /// Show track data
        /// </summary>
        private void ShowTrackData()
        {
            FlowDocument document = new FlowDocument();
            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            foreach (TOC_Entry entry in StaticObjects.Parameters.TrackEntries)
            {
                System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph()
                {
                    Padding= new Thickness(5),
                    Style = App.Appearance.ForegroundStyle
                };
    

                //Bold link = new Bold();
                //link.FontSize = StaticObjects.Parameters.FontSizeInfo;
                //link.Foreground = accentBrush;
                //link.Inlines.Add(entry.ParagraphID);

                Hyperlink hyperlink = format.HyperlinkFullParagraph(entry, false, entry.Text);
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                hyperlink.MouseEnter += Hyperlink_MouseEnter;
                hyperlink.MouseLeave += Hyperlink_MouseLeave;

                paragraph.Inlines.Add(hyperlink);
                document.Blocks.Add(paragraph);
            }
            TrackDataFlowDocument.Document = document;
            App.Appearance.SetFontSize(TrackDataFlowDocument);
            App.Appearance.SetThemeInfo(TrackDataFlowDocument);
        }


        private void SetFontSize()
        {
            //App.Appearance.SetFontSize(TrackDataFlowDocument);
            ShowTrackData();
            App.Appearance.SetFontSize(ButtonTrackSort);
            App.Appearance.SetFontSize(ButtonTrackClear);
            App.Appearance.SetFontSize(ButtonTrackSave);
            App.Appearance.SetFontSize(ButtonTrackLoad);
            App.Appearance.SetThemeInfo(ButtonTrackSort);
            App.Appearance.SetThemeInfo(ButtonTrackClear);
            App.Appearance.SetThemeInfo(ButtonTrackSave);
            App.Appearance.SetThemeInfo(ButtonTrackLoad);
        }

        private void SetAppearence()
        {
            //App.Appearance.SetThemeInfo(TrackDataFlowDocument);
            ShowTrackData();
            App.Appearance.SetThemeInfo(ButtonTrackSort);
            App.Appearance.SetThemeInfo(ButtonTrackClear);
            App.Appearance.SetThemeInfo(ButtonTrackSave);
            App.Appearance.SetThemeInfo(ButtonTrackLoad);

            GeometryImages images = new GeometryImages();
            ButtonTrackSortImage.Source = images.GetImage(GeometryImagesTypes.Sort);
            ButtonTrackClearImage.Source = images.GetImage(GeometryImagesTypes.Clear);
            ButtonTrackSaveImage.Source = images.GetImage(GeometryImagesTypes.Save);
            ButtonTrackLoadImage.Source = images.GetImage(GeometryImagesTypes.Load);
        }

        private void AddEntry(TOC_Entry entry)
        {
            if (StaticObjects.Parameters.TrackEntries.Count == StaticObjects.Parameters.MaxExpressionsStored)
            {
                StaticObjects.Parameters.TrackEntries.RemoveAt(StaticObjects.Parameters.TrackEntries.Count - 1);
            }

            if (string.IsNullOrEmpty(entry.Text))
            {
                Paper paperLeft = StaticObjects.Book.LeftTranslation.Paper(entry.Paper);
                Paragraph par = paperLeft.GetParagraph(entry);
                entry.Text = par.Text;
            }

            StaticObjects.Parameters.TrackEntries.Insert(0, entry);
            ShowTrackData();
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
        }

        //private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    ScrollViewer scv = (ScrollViewer)sender;
        //    scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        //    e.Handled = true;
        //}

        private void Hyperlink_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = null;
        }

        private void Hyperlink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = TextDecorations.Underline;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            e.Handled = true;
            if (hyperlink.Tag == null)
            {
                return;
            }
            TOC_Entry entry = hyperlink.Tag as TOC_Entry;
            if (entry == null)
            {
                return;
            }
            EventsControl.FireTrackSelected(entry);

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor(2));
            var run = hyperlink.Inlines.FirstOrDefault() as Bold;
            if (run != null)
            {
                run.Foreground = accentBrush;
            }
        }



        #endregion


        private void ButtonTrackSort_Click(object sender, RoutedEventArgs e)
        {
            switch (sortOrder)
            {
                case TrachSortOrder.None:
                    StaticObjects.Parameters.TrackEntries.Sort<TOC_Entry>((a, b) => a.CompareTo(b));
                    sortOrder = TrachSortOrder.Ascending;
                    break;
                case TrachSortOrder.Ascending:
                    StaticObjects.Parameters.TrackEntries.Sort<TOC_Entry>((a, b) => a.InverseCompareTo(b));
                    sortOrder = TrachSortOrder.Descending;
                    break;
                case TrachSortOrder.Descending:
                    StaticObjects.Parameters.TrackEntries.Sort<TOC_Entry>((a, b) => a.CompareTo(b));
                    sortOrder = TrachSortOrder.Ascending;
                    break;
            }
            ShowTrackData();
        }

        private void ButtonTrackClear_Click(object sender, RoutedEventArgs e)
        {
            StaticObjects.Parameters.TrackEntries.Clear();
            ShowTrackData();
        }

        private void ButtonTrackSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
            if (string.IsNullOrEmpty(StaticObjects.Parameters.LastTrackFileSaved))
            {
                saveFileDlg.FileName = "";
                saveFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                saveFileDlg.FileName = StaticObjects.Parameters.LastTrackFileSaved;
            }
            saveFileDlg.DefaultExt = ".ubsht";
            saveFileDlg.Filter = "Ub Study Help Track files (.ubsht)|*.ubsht";
            Nullable<bool> result = saveFileDlg.ShowDialog();
            try
            {
                if (result == true)
                {
                    var jsonString = JsonConvert.SerializeObject(StaticObjects.Parameters.TrackEntries, Formatting.Indented);
                    File.WriteAllText(saveFileDlg.FileName, jsonString);
                }
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error($"Error saving saved track file to {saveFileDlg.FileName}", ex);
            }

        }

        private void ButtonTrackLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            if (string.IsNullOrEmpty(StaticObjects.Parameters.LastTrackFileSaved))
            {
                openFileDlg.FileName = "";
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UbStudyHelp");
                Directory.CreateDirectory(folder);
                openFileDlg.InitialDirectory = folder;
            }
            else
            {
                openFileDlg.FileName = StaticObjects.Parameters.LastTrackFileSaved;
            }

            openFileDlg.DefaultExt = ".ubsht";
            openFileDlg.Filter = "Ub Study Help Track files (.ubsht)|*.ubsht";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    var jsonString = File.ReadAllText(openFileDlg.FileName);
                    StaticObjects.Parameters.TrackEntries = JsonConvert.DeserializeObject<List<TOC_Entry>>(jsonString);
                }
                catch (Exception ex)
                {
                    StaticObjects.Logger.Error($"Error loading saved track file from {openFileDlg.FileName}", ex);
                    StaticObjects.Parameters.TrackEntries = new List<TOC_Entry>();
                }
                ShowTrackData();
            }

        }
    }
}
