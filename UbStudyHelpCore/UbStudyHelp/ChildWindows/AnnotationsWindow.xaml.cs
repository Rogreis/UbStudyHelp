using System.IO;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using UbStandardObjects.Objects;
using static System.Environment;
using System.Text;
using UbStudyHelp.Classes.ContextMenuCode;
using UbStandardObjects;
using System.Windows.Media;
using UbStudyHelp.Printing;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Text.Json;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Interaction logic for AnnotationsWindow.xaml
    /// </summary>
    public partial class AnnotationsWindow : Window
    {

        private TOC_Entry Entry = null;
        private FlowDocumentFormat format = new FlowDocumentFormat();
        private UbAnnotationsStoreDataCore AnnotationsStoreDataCore = null;
        private UbAnnotations UbAnnotationsObject = null;
        private bool ShowAnnotationsContextMenu = false;

        private Translation CurrentTranslation = null;
        private bool ExistingNote = true;   // indicate the existence of the note before starts editing
        private bool DeletingNote = false;  // indicate that the note is being deleted

        public AnnotationsWindow(TOC_Entry entry, bool showAnnotationsContextMenu = false)
        {
            InitializeComponent();

            // Worked local events
            Loaded += AnnotationsWindow_Loaded;
            Unloaded += AnnotationsWindow_Unloaded;
            SizeChanged += AnnotationsWindow_SizeChanged;

            ShowAnnotationsContextMenu = showAnnotationsContextMenu;

            Width = StaticObjects.Parameters.AnnotationWindowWidth;
            Height = StaticObjects.Parameters.AnnotationWindowHeight;


            CurrentTranslation = StaticObjects.Book.LeftTranslation.LanguageID == entry.TranslationId ? StaticObjects.Book.LeftTranslation : StaticObjects.Book.RightTranslation;
            AnnotationsStoreDataCore = (UbAnnotationsStoreDataCore)CurrentTranslation.GetAnnotation(entry);
            if (AnnotationsStoreDataCore == null)
            {
                AnnotationsStoreDataCore = new UbAnnotationsStoreDataCore();
                ExistingNote = false;
            }

            // Force the annotation store object to be for paragraph in case it was just created
            AnnotationsStoreDataCore.AnnotationType = UbAnnotationType.Paragraph;
            UbAnnotationsObject = new UbAnnotations(AnnotationsStoreDataCore, entry);
            AnnotationsStoreDataCore.FillAnnotations();
            SetRichTextNote(AnnotationsStoreDataCore.XamlNotes);
            TextBoxTitle.Text = AnnotationsStoreDataCore.Title;

            Title = $"Annotations for {entry.ParagraphIDNoPage}";


            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;


            Entry = entry;
            BorderThickness = new Thickness(2.0);
            BorderBrush = App.Appearance.GetHighlightColorBrush();
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.ToolWindow;

            Owner = Application.Current.MainWindow;

            FlowDocument document = new FlowDocument();
            //SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            document.Blocks.Add(format.FullParagraph(entry, false, entry.Text));
            FlowDocument.Document = document;
            App.Appearance.SetFontSize(FlowDocument);
            App.Appearance.SetThemeInfo(FlowDocument);

            //RichTextBoxNote.AppendText(entry.Description);
            SetFontSize();
            SetAppearence();
        }


        private void SetFontSize()
        {
            App.Appearance.SetFontSize(RichTextBoxNote);
            App.Appearance.SetFontSize(FlowDocument);
            App.Appearance.SetFontSize(TextBlockTitle);
            App.Appearance.SetFontSize(TextBoxTitle);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(RichTextBoxNote);
            App.Appearance.SetThemeInfo(FlowDocument);
            App.Appearance.SetThemeInfo(TextBlockTitle);
            App.Appearance.SetThemeInfo(TextBoxTitle);
        }

        #region Context Menu

        /// <summary>
        /// 
        /// <see href="https://stackoverflow.com/questions/2322064/how-can-i-produce-a-print-preview-of-a-flowdocument-in-a-wpf-application"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintCommand(object sender, RoutedEventArgs e)
        {
            UpPrintingPreview upPrintingPreview = new UpPrintingPreview();
            upPrintingPreview.DoPreview(Entry.Description, FlowDocument, RichTextBoxNote);
        }

        private MenuItem CreateMenuItem(string header, RoutedEventHandler command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Click += command;
            return item;
        }


        private MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem()
            {
                Header = header,
                Command = command,
                Tag = Entry,
            };
            return item;
        }

        private ContextMenu CreateUbParagraphContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            contextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));
            //contextMenu.Items.Add(new Separator());
            //contextMenu.Items.Add(CreateMenuItem("Print Preview", ApplicationCommands.PrintPreview));
            //contextMenu.Items.Add(CreateMenuItem("Print", PrintCommand));
            if (ShowAnnotationsContextMenu)
            {
                contextMenu.Items.Add(new Separator());
                contextMenu.Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
                //contextMenu.Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
                //contextMenu.Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
                contextMenu.Items.Add(new Separator());
                contextMenu.Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
                //contextMenu.Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
                //contextMenu.Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));
            }

            return contextMenu;
        }

        private ContextMenu CreateRichTextBoxContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            contextMenu.Items.Add(CreateMenuItem("Cut", ApplicationCommands.Cut));
            contextMenu.Items.Add(CreateMenuItem("Paste", ApplicationCommands.Paste));
            contextMenu.Items.Add(new Separator());
            //contextMenu.Items.Add(CreateMenuItem("Print", PrintCommand));
            //contextMenu.Items.Add(CreateMenuItem("Insert Image", Item_InsertImage));
            contextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));
            //contextMenu.Items.Add(CreateMenuItem("Find", ApplicationCommands.Find));
            contextMenu.Items.Add(CreateMenuItem("Undo", ApplicationCommands.Undo));
            contextMenu.Items.Add(CreateMenuItem("Redo", ApplicationCommands.Redo));
            return contextMenu;
        }


        #endregion

        #region Get/Set richtext 
        private string GetRichTextNote()
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                TextRange range = new TextRange(RichTextBoxNote.Document.ContentStart, RichTextBoxNote.Document.ContentEnd);
                range.Save(outputStream, DataFormats.XamlPackage);
                outputStream.Flush();
                return Convert.ToBase64String(outputStream.ToArray());
            }
        }

        private void SetRichTextNote(string xamlNote)
        {
            if (string.IsNullOrWhiteSpace(xamlNote))
            {
                return;
            }

            using (MemoryStream outputStream = new MemoryStream(Convert.FromBase64String(xamlNote)))
            {
                TextRange range = new TextRange(RichTextBoxNote.Document.ContentStart, RichTextBoxNote.Document.ContentEnd);
                range.Load(outputStream, DataFormats.XamlPackage);
            }
        }

        private bool IsTextEmpty(RichTextBox rtb)
        {
            var start = rtb.Document.ContentStart;
            var end = rtb.Document.ContentEnd;
            int dif = start.GetOffsetToPosition(end);
            return dif <= 4;
        }

        #endregion

        #region Events

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            SetFontSize();
        }

        private void AnnotationsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            FlowDocument.ContextMenu = CreateUbParagraphContextMenu();
            RichTextBoxNote.ContextMenu = CreateRichTextBoxContextMenu();
            UbAnnotationsObject.StartAnnotations(FlowDocument, Entry);

        }

        private void AnnotationsWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StaticObjects.Parameters.AnnotationWindowHeight = e.NewSize.Height;
            StaticObjects.Parameters.AnnotationWindowWidth = e.NewSize.Width;
        }


        private void AnnotationsWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stop annotatios and persist them
            UbAnnotationsObject.StopAnnotations();
            if (DeletingNote)
            {
                return;
            }
            AnnotationsStoreDataCore.XamlNotes = GetRichTextNote();
            AnnotationsStoreDataCore.Entry = Entry;
            AnnotationsStoreDataCore.Title = TextBoxTitle.Text;
            if (ExistingNote || !(AnnotationsStoreDataCore.Annotations.Count == 0 && IsTextEmpty(RichTextBoxNote)))
            {

                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                };
                CurrentTranslation.StoreAnnotation(AnnotationsStoreDataCore);
                StaticObjects.Book.StoreAnnotations(Entry, CurrentTranslation.Annotations);
                EventsControl.FireAnnotationsChanges();
            }
        }


        #endregion

        #region Tool bar buttons

        #region Work with images
        public void InsertImageTool()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "RTE - Insert Image File";
            openFile.DefaultExt = "rtf";
            openFile.Filter = "Bitmap Files|*.bmp|JPEG Files|*.jpg|GIF Files|*.gif|PNG Files|*.png|All files|*.*";
            openFile.FilterIndex = 1;
            openFile.ShowDialog();

            if (openFile.FileName == "")
            {
                return;
            }

            try
            {

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFile.FileName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                Image image = new Image();
                image.Source = bitmap;
                System.Windows.Documents.Paragraph para = new System.Windows.Documents.Paragraph();
                para.Inlines.Add(image);
                RichTextBoxNote.Document.Blocks.Add(para);
            }
            catch
            {
                MessageBox.Show("Unable to insert the image format selected.", "Error");
            }
        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {
            InsertImageTool();
        }


        #endregion

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear text and highlights?", "Ub Study Help", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RichTextBoxNote.Document.Blocks.Clear();
                UbAnnotationsObject.ClearAnnotation();
            }
        }

        private void ButtonClearText_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear all text?", "Ub Study Help", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                RichTextBoxNote.Document.Blocks.Clear();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Delete notes for {Entry.ParagraphID} - {TextBoxTitle.Text} ?", "Ub Study Help", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeletingNote = true;
                StaticObjects.Book.DeleteAnnotations(Entry);
                Close();
            }
        }

        private void ButtonClearHighlights_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear all highlights?", "Ub Study Help", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                UbAnnotationsObject.ClearAnnotation();
            }
        }

        #endregion

    }
}
