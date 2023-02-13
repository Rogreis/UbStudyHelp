using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;

namespace UbStudyHelp.ChildWindows
{
    /// <summary>
    /// Interaction logic for EditText.xaml
    /// </summary>
    public partial class EditText : Window
    {
        public EditText()
        {
            InitializeComponent();

            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            Loaded += EditText_Loaded;

        }

        private void EditText_Loaded(object sender, RoutedEventArgs e)
        {
            App.Appearance.SetFontSize(richTextBoxEdit);
            App.Appearance.SetFontSize(commitMessage);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);
        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetThemeInfo(richTextBoxEdit);
            App.Appearance.SetThemeInfo(commitMessage);
            App.Appearance.SetThemeInfo(buttonWorking);
            App.Appearance.SetThemeInfo(buttonOk);
            App.Appearance.SetThemeInfo(buttonDoubt);
            App.Appearance.SetThemeInfo(buttonClosed);
            //richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
            //richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetFontSize(richTextBoxEdit);
            App.Appearance.SetFontSize(commitMessage);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);
        }

        public void SetText(TOC_Entry entry)
        {
            richTextBoxEdit.Document.Blocks.Clear();
            Paper paper= StaticObjects.Book.RightTranslation.Paper(entry.Paper);
            UbStandardObjects.Objects.Paragraph par = paper.GetParagraph(entry);
            richTextBoxEdit.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(par.Text)));
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonWorking_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonDoubt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonClosed_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
