using System;
using System.Collections.Generic;
using System.Linq;
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
using ControlzEx.Theming;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.ContextMenuCode;
using static Lucene.Net.Search.FieldValueHitQueue;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for NotesPage.xaml
    /// </summary>
    public partial class NotesPage : Page
    {
        // Format the shown text
        private FlowDocumentFormat format = new FlowDocumentFormat();

        public NotesPage()
        {
            InitializeComponent();
            EventsControl.AnnotationsChanges += EventsControl_AnnotationsChanges;
        }


        private void SetAppearence()
        {
            //App.Appearance.SetThemeInfo(LabelLeftTranslations);
            //App.Appearance.SetThemeInfo(LabelRightTranslations);
            //App.Appearance.SetThemeInfo(LabelColorTheme);
            //App.Appearance.SetThemeInfo(ComboLeftTranslations);
            //App.Appearance.SetThemeInfo(ComboRightTranslation);
            //App.Appearance.SetThemeInfo(ComboTheme);
        }



        private void SetFontSize()
        {
            //App.Appearance.SetFontSize(LabelLeftTranslations);
            //App.Appearance.SetFontSize(LabelRightTranslations);
            //App.Appearance.SetFontSize(LabelColorTheme);
            //App.Appearance.SetFontSize(ComboLeftTranslations);
            //App.Appearance.SetFontSize(ComboRightTranslation);
            //App.Appearance.SetFontSize(ComboTheme);
        }

        private class AnnotationIdent
        {
            public string Title { get; set; } = "";

            public TOC_Entry Entry { get; set; }

            public AnnotationIdent(string title, TOC_Entry entry)
            {
                Title = title;
                Entry = entry;
            }
        }

        public void Initialize()
        {
            SetFontSize();
            SetAppearence();

            UpdateData();



            //  .Select(x => new YourClass { OtherID = x.otherID, DayID = x.dayID).ToList();

            //ComboLeftTranslations.ItemsSource = StaticObjects.Book.ObservableTranslations;
            //ComboRightTranslation.ItemsSource = StaticObjects.Book.ObservableTranslations;
            //SelectComboCurrentTranslation(ComboLeftTranslations, StaticObjects.Parameters.LanguageIDLeftTranslation);
            //SelectComboCurrentTranslation(ComboRightTranslation, StaticObjects.Parameters.LanguageIDRightTranslation);

            //ComboTheme.Text = ((ParametersCore)StaticObjects.Parameters).ThemeColor;
            //ToggleSwitchThemme.IsOn = App.Appearance.Theme == "Dark";

            //GeometryImages images = new GeometryImages();
            //ButtonUpdateAvailableImage.Source = images.GetImage(GeometryImagesTypes.Update);
            //ToggleSwitchShowParIdent.IsOn = StaticObjects.Parameters.ShowParagraphIdentification;
            //ToggleSwitchBilingual.IsOn = StaticObjects.Parameters.ShowBilingual;
        }


        /// <summary>
        /// Update the shown list of annotations (a merge between both shown translations)
        /// </summary>
        private void UpdateData()
        {
            List<AnnotationIdent> list = new List<AnnotationIdent>();

            list.AddRange(StaticObjects.Book.LeftTranslation.Annotations.Select(a => new AnnotationIdent(a.Title, a.Entry)));
            list.AddRange(StaticObjects.Book.RightTranslation.Annotations.Select(a => new AnnotationIdent(a.Title, a.Entry)));
            list.Sort((a, b) => a.Entry.CompareTo(b.Entry));

            FlowDocument document = new FlowDocument();
            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            foreach (AnnotationIdent ident in list)
            {
                FormatData data = new FormatData()
                {
                    FormatType= ParagraphFormatType.Normal,
                    Entry = ident.Entry,
                    Status = ParagraphStatus.Closed,
                    Text = ident.Entry.Text,
                };
                format.FormatParagraph(data);
                data.DocParagraph.Padding = new Thickness(5, 5, 5, 0);

                data.Link.Tag = ident.Entry;
                data.Link.RequestNavigate += Hyperlink_RequestNavigate;
                data.Link.MouseEnter += Hyperlink_MouseEnter;
                data.Link.MouseLeave += Hyperlink_MouseLeave;
                data.Link.MouseRightButtonDown += Hyperlink_MouseRightButtonDown;

                document.Blocks.Add(data.DocParagraph);
            }
            NotesFlowDocument.Document = document;
            App.Appearance.SetFontSize(NotesFlowDocument);
            App.Appearance.SetThemeInfo(NotesFlowDocument);
        }


        private void EventsControl_AnnotationsChanges(TOC_Entry entry)
        {
            UpdateData();
        }



        #region Hyperlink events



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
            //NotesFlowDocument.ContextMenu = new UbNotesContextMenu(NotesFlowDocument, null);
            Hyperlink hyperlink = sender as Hyperlink;
            if (hyperlink.Tag == null)
            {
                return;
            }
            TOC_Entry entry = hyperlink.Tag as TOC_Entry;
            if (entry == null)
            {
                return;
            }

            hyperlink.ContextMenu = new UbNotesContextMenu(entry);
            e.Handled = true;
            hyperlink.ContextMenu.IsOpen = true;
        }

        private void Hyperlink_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            if (hyperlink.Tag == null)
            {
                return;
            }
            TOC_Entry entry = hyperlink.Tag as TOC_Entry;
            if (entry == null)
            {
                return;
            }

            hyperlink.ContextMenu = new UbNotesContextMenu(entry);
            e.Handled = true;
            hyperlink.ContextMenu.IsOpen = true;
        }


        #endregion


    }
}
