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

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Interaction logic for AnnotationsWindow.xaml
    /// </summary>
    public partial class AnnotationsWindow : Window
    {

        private TOC_Entry Entry = null;
        private FlowDocumentFormat format = new FlowDocumentFormat();
        private UbAnnotations Annotations = new UbAnnotations(UbAnnotationType.Paragraph);


        public AnnotationsWindow(TOC_Entry entry)
        {
            InitializeComponent();

            this.Loaded += AnnotationsWindow_Loaded;
            this.Unloaded += AnnotationsWindow_Unloaded;
            Annotations.Entry = entry;

            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;

            this.Title = $"Paragraph Annotations for {entry.ParagraphID}";

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

            Annotations.StartAnnotations(FlowDocument);

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
            FlowDocument.ContextMenu = new ContextMenu();
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));
            //FlowDocument.ContextMenu.Items.Add(new Separator());
            //FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Print Preview", ApplicationCommands.PrintPreview));
            //FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Print", ApplicationCommands.Print));
            FlowDocument.ContextMenu.Items.Add(new Separator());
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Add Highlight", AnnotationService.CreateHighlightCommand));
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Add Text Note", AnnotationService.CreateTextStickyNoteCommand));
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Add Ink Note", AnnotationService.CreateInkStickyNoteCommand));
            FlowDocument.ContextMenu.Items.Add(new Separator());
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Remove Highlights", AnnotationService.ClearHighlightsCommand));
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Remove Notes", AnnotationService.DeleteStickyNotesCommand));
            FlowDocument.ContextMenu.Items.Add(CreateMenuItem("Remove Highlights & Notes", AnnotationService.DeleteAnnotationsCommand));


            RichTextBoxNote.ContextMenu = new ContextMenu();
            RichTextBoxNote.ContextMenu.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            RichTextBoxNote.ContextMenu.Items.Add(CreateMenuItem("Cut", ApplicationCommands.Cut));
            RichTextBoxNote.ContextMenu.Items.Add(CreateMenuItem("Paste", ApplicationCommands.Paste));
            RichTextBoxNote.ContextMenu.Items.Add(new Separator());
            RichTextBoxNote.ContextMenu.Items.Add(CreateMenuItem("Select all", ApplicationCommands.SelectAll));


            //ToolBarAnnotations.Items.Add(new Separator());
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Copy", ApplicationCommands.Copy));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Select All", ApplicationCommands.SelectAll));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Print", ApplicationCommands.Print));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Find", ApplicationCommands.Find));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Open", ApplicationCommands.Open));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Save", ApplicationCommands.Save));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Save As", ApplicationCommands.SaveAs));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Close", ApplicationCommands.Close));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Undo", ApplicationCommands.Undo));
            //ToolBarAnnotations.Items.Add(CreateMenuItem("Redo", ApplicationCommands.Redo));
        }

        private void AnnotationsWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Annotations.StopAnnotations();
        }


        protected MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem()
            {
                Header = header,
                Command = command,
                Tag = Entry,
            };

            item.Click += Item_Click;
            return item;
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.Source as MenuItem;
            object xx= e.Source;
        }
    }
}
