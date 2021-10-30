using MdXaml;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    /// <summary>
    /// Interaction logic for UbStudyBrowser.xaml
    /// </summary>
    public partial class IndexBrowser : UserControl
    {

        // Object to manipulate the index
        private UbStudyHelp.Classes.Index Index = new UbStudyHelp.Classes.Index(App.BaseTubFilesPath);


        public IndexBrowser()
        {
            InitializeComponent();

            EventsControl.FontChanged += EventsControl_FontChanged;
            TextBoxIndexLetters.KeyDown += TexTextBoxIndexLettersoxIndexLetters_KeyDown;
            ComboBoxIndexSearch.DropDownClosed += ComboBoxIndexSearch_DropDownClosed;

            MarkDownDisplay.Markdown = Properties.Resources.SearchHelp;

            //MarkDownDisplay.Style. .Setters[0].

        }

        #region Events

        private void TexTextBoxIndexLettersoxIndexLetters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = TextBoxIndexLetters.Text.Trim();
                if (text.Length > 1)
                {
                    FillComboBoxIndexEntry(text);
                }
            }
        }


        private void ComboBoxIndexSearch_DropDownClosed(object sender, EventArgs e)
        {
            string text = ComboBoxIndexSearch.Text.Trim();
            if (text.Length > 1)
            {
                CreateWebIndexPage(text);
            }
            
        }

        /*
         * 
    <Style TargetType="FlowDocument" x:Key="DocumentStyleGithubLike">
        <Setter Property="FontFamily"    Value="Calibri" />
        <Setter Property="TextAlignment" Value="Left" />
        <Setter Property="PagePadding"   Value="0"/>
        <Setter Property="FontSize"      Value="14"/>

        <Style.Resources>
            <Style TargetType="Section">
                <Setter Property="Padding"         Value="10, 5"/>
                <Setter Property="BorderBrush"     Value="#DEDEDE"/>
                <Setter Property="BorderThickness" Value="5,0,0,0"/>
            </Style>

            <Style TargetType="avalonEdit:TextEditor" xmlns:avalonEdit="http://icsharpcode.net/sharpdevelop/avalonedit">
                <Setter Property="Background"                    Value="#EEEEEE"/>
                <Setter Property="HorizontalScrollBarVisibility" Value="Auto"/>
                <Setter Property="VerticalScrollBarVisibility"   Value="Auto"/>
                <Setter Property="Margin"                        Value="2,0,2,0"/>
                <Setter Property="Padding"                       Value="3"/>
            </Style>

         * */

        public void SetMakrdownStyle(FlowDocument doc)
        {
            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-create-apply-style?view=netdesktop-6.0
            Style style = new Style();
            style.BasedOn = MarkDownDisplay.Style;
            style.Resources["Paragraph"];


            style.Setters.Add(new Setter(FlowDocument.FontSizeProperty, FontSizeInfo));
            style.Setters.Add(new Setter(FlowDocument.BackgroundProperty, new SolidColorBrush(BackgroundColor)));
            style.Setters.Add(new Setter(FlowDocument.ForegroundProperty, new SolidColorBrush(ForegroundColor)));

            style.Resources.Add

            doc.Style = style;
        }


        public void UpdateFont()
        {
            App.Appearance.SetFontSize(TextBoxIndexLetters);
            App.Appearance.SetFontSize(ComboBoxIndexSearch);
            App.Appearance.SetFontSize(IndexResults);
            App.Appearance.SetFontSize(MarkDownDisplay);
            MarkdownStyle.GithubLike. .Sasabune.Setters[0].

            MarkDownDisplay.S
            App.Appearance.SetHeight(TextBoxIndexLetters);
            App.Appearance.SetHeight(ComboBoxIndexSearch);
        }


        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            UpdateFont();
        }

        private void CreateWebIndexPage(string indexEntry)
        {
            if (!Index.Load())
                return;
            Index.ShowResults(indexEntry, IndexResults);
        }



        #endregion


        private void FillComboBoxIndexEntry(string indexEntry)
        {
            if (!Index.Load())
                return;
            List<string> list = Index.Search(indexEntry);
            if (list == null || list.Count == 0)
            {
                ComboBoxIndexSearch.IsEnabled = false;
                return;
            }
            int maxItems = Math.Min(list.Count, App.ParametersData.MaxExpressionsStored);
            ComboBoxIndexSearch.Items.Clear();
            for (int i = 0; i < maxItems; i++)
            {
                ComboBoxIndexSearch.Items.Add(list[i]);
            }
            ComboBoxIndexSearch.SelectedIndex = 0;
            ComboBoxIndexSearch.IsEnabled = true;
            App.ParametersData.IndexLetters = indexEntry;
        }



    }
}
