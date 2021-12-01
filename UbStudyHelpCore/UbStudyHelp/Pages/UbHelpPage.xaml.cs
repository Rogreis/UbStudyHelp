using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStudyHelp.Classes;
using UbStudyHelp.Classes.MarkdownClasses;

namespace UbStudyHelp.Pages
{
    /// <summary>
    /// Interaction logic for SearchHelpPage.xaml
    /// </summary>
    public partial class SearchHelpPage : Page
    {
        public SearchHelpPage()
        {
            InitializeComponent();
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }

        private void SetFontSize()
        {
            App.Appearance.SetFontSize(FlowDocumentViewer);
        }

        private void SetAppearence()
        {
            App.Appearance.SetThemeInfo(FlowDocumentViewer);
            ConvertMd(Properties.Resources.SearchHelp);
        }


        #region events
        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            SetAppearence();
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            SetFontSize();
        }
        #endregion


        private Style ForegroundStyle
        {
            get
            {
                Style style = new Style
                {
                    TargetType = typeof(Block)
                };

                style.Setters.Add(new Setter(Block.FontFamilyProperty, new FontFamily(App.ParametersData.FontFamilyInfo)));
                style.Setters.Add(new Setter(Block.FontSizeProperty, App.ParametersData.FontSizeInfo));
                style.Setters.Add(new Setter(Block.ForegroundProperty, App.Appearance.GetForegroundColorBrush()));
                return style;
            }
        }

        private Style HeadingStyle(int size)
        {
            Style style = new Style
            {
                TargetType = typeof(Block)
            };

            style.Setters.Add(new Setter(Block.FontFamilyProperty, new FontFamily(App.ParametersData.FontFamilyInfo)));
            style.Setters.Add(new Setter(Block.FontSizeProperty, App.ParametersData.FontSizeInfo + size));
            style.Setters.Add(new Setter(Block.ForegroundProperty, App.Appearance.GetHighlightColorBrush()));
            return style;
        }

        private void ConvertMd(string mdData)
        {

            UbMarkdown markdown = new UbMarkdown();

            markdown.NormalParagraphStyle = ForegroundStyle;
            markdown.Heading1Style = HeadingStyle(4);
            markdown.Heading2Style = HeadingStyle(3);
            markdown.Heading3Style = HeadingStyle(2);
            markdown.Heading4Style = HeadingStyle(1); 
            

            FlowDocument doc = markdown.Transform(mdData);
            FlowDocumentViewer.Document = doc;
        }

        public void Initialize()
        {
            SetAppearence();
            SetFontSize();
            //MarkDownDisplay.Markdown = Properties.Resources.SearchHelp;
            //App.Appearance.SetThemeInfo(MarkDownDisplay);
            App.Appearance.SetThemeInfo(FlowDocumentViewer);
        }


    }
}
