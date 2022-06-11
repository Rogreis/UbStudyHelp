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

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Interaction logic for AnnotationsWindow.xaml
    /// </summary>
    public partial class AnnotationsWindow : Window
    {

        private TOC_Entry Entry = null;
        private FlowDocumentFormat format = new FlowDocumentFormat();
        private readonly UbAnnotations UbAnnotationsObject = new UbAnnotations(UbAnnotationType.Paragraph);


        public AnnotationsWindow(TOC_Entry entry)
        {
            InitializeComponent();

            Loaded += AnnotationsWindow_Loaded;
            Unloaded += AnnotationsWindow_Unloaded;

            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;

            Title = $"Paragraph Annotations for {entry.Description}";

            Entry= entry;
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
            //App.Appearance.SetFontSize(ToolBarAnnotations);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(RichTextBoxNote);
            App.Appearance.SetThemeInfo(FlowDocument);
            //App.Appearance.SetThemeInfo(ToolBarAnnotations);
        }

        #region Context Menu

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/057cf5c5-26f9-49b6-8e5b-d88f37d8ebb1/wpf-c-insert-image-form-code-to-wpf-i-am-having-difficulty-transfering-the-c-code-to-wpf?forum=wpf

        public void InsertImageTool(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "RTE - Insert Image File";
            openFile.DefaultExt = "rtf";
            openFile.Filter = "Bitmap Files|*.bmp|JPEG Files|*.jpg|GIF Files|*.gif|PNG Files|*.png";
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
                MessageBox.Show("Unable to insert the image format selected.", "RTE - Paste");
            }
        }



        //public void InsertImageTool_Click(object sender, System.EventArgs e)
        //{
        //    OpenFileDialog openFile = new OpenFileDialog();
        //    openFile.Title = "RTE - Insert Image File";
        //    openFile.DefaultExt = "rtf";
        //    openFile.Filter = "Bitmap Files|*.bmp|JPEG Files|*.jpg|GIF Files|*.gif|PNG Files|*.png";
        //    openFile.FilterIndex = 1;
        //    openFile.ShowDialog();

        //    if (openFile.FileName == "")
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        string strImagePath = openFile.FileName;
        //        Image img;
        //        img = Image.FromFile(strImagePath);
        //        Clipboard.SetDataObject(img);
        //        DataFormats.Format df;
        //        df = DataFormats.GetFormat(DataFormats.Bitmap);
        //        if (this.RichTextBoxNote.CanPaste(df))
        //        {
        //            this.RichTextBoxNote.Paste(df);
        //        }
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Unable to insert the image format selected.", "RTE - Paste");
        //    }
        //}

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

        private void Item_InsertImage(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.Source as MenuItem;
            object xx = e.Source;
        }


        private ContextMenu CreateUbParagraphContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            contextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));
            //contextMenu.Items.Add(new Separator());
            //contextMenu.Items.Add(CreateMenuItem("Print Preview", ApplicationCommands.PrintPreview));
            contextMenu.Items.Add(CreateMenuItem("Print", PrintCommand));
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
            contextMenu.Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
            contextMenu.Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
            contextMenu.Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
            contextMenu.Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));
            return contextMenu;
        }

        private ContextMenu CreateRichTextBoxContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            contextMenu.Items.Add(CreateMenuItem("Cut", ApplicationCommands.Cut));
            contextMenu.Items.Add(CreateMenuItem("Paste", ApplicationCommands.Paste));
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(CreateMenuItem("Print", PrintCommand));
            contextMenu.Items.Add(CreateMenuItem("Insert Image", Item_InsertImage));
            contextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Find", ApplicationCommands.Find));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Undo", ApplicationCommands.Undo));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Redo", ApplicationCommands.Redo));
            return contextMenu;
        }


        #endregion

        #region Work with images

        private string GetRTB_XamlPackage()
        {
            TextRange range = new TextRange(RichTextBoxNote.Document.ContentStart, RichTextBoxNote.Document.ContentEnd);
            MemoryStream memoryStream = new MemoryStream();
            range.Save(memoryStream, DataFormats.XamlPackage);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region Open - Save

        // Load XAML into RichTextBox from a file specified by _fileName
        void LoadRTB_XamlPackage(string richTextBoxContext)
        {
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(richTextBoxContext));
            TextRange range = new TextRange(RichTextBoxNote.Document.ContentStart, RichTextBoxNote.Document.ContentEnd);
            range.Load(mStream, DataFormats.XamlPackage);
            mStream.Close();

            //UbAnnotationsObject.
        }



        #endregion

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

        private void AnnotationsWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            UbAnnotationsObject.StopAnnotations();
        }


        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
