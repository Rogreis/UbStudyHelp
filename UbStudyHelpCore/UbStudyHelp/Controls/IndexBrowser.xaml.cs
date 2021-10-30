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
            DataEntry.Index = Index;
            DataEntry.ShowIndexHelp += DataEntry_ShowIndexHelp;
            DataEntry.ShowIndexDetails += DataEntry_ShowIndexDetails;
            MarkDownDisplay.Markdown = Properties.Resources.SearchHelp;
        }

        /// <summary>
        /// Show the details for some index entry
        /// </summary>
        /// <param name="indexEntry"></param>
        private void CreateWebIndexPage(string indexEntry)
        {
            if (!Index.Load())
                return;
            MarkDownDisplay.Visibility = Visibility.Hidden;
            IndexResults.Visibility = Visibility.Visible;
            Index.ShowResults(indexEntry, IndexResults);
        }


        private void SetMakrdownStyle(FlowDocument doc)
        {
                /*
                 *  Information for issue about mark down font size - remove after fixed
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

            // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-create-apply-style?view=netdesktop-6.0
            Style style = new Style();
            style.BasedOn = MarkDownDisplay.Style;
            //style.Resources["Paragraph"];


            //style.Setters.Add(new Setter(FlowDocument.FontSizeProperty, FontSizeInfo));
            //style.Setters.Add(new Setter(FlowDocument.BackgroundProperty, new SolidColorBrush(BackgroundColor)));
            //style.Setters.Add(new Setter(FlowDocument.ForegroundProperty, new SolidColorBrush(ForegroundColor)));

            //style.Resources.Add

            //doc.Style = style;
        }


        private void UpdateFont()
        {
            App.Appearance.SetFontSize(IndexResults);
            App.Appearance.SetFontSize(MarkDownDisplay);
            //MarkdownStyle.GithubLike. .Sasabune.Setters[0].

        }


        #region Events

        private void EventsControl_FontChanged(Classes.ControlsAppearance appearance)
        {
            UpdateFont();
        }

        private void DataEntry_ShowIndexDetails(string indexEntry)
        {
            CreateWebIndexPage(indexEntry);
        }

        /// <summary>
        /// Make the search help visible
        /// </summary>
        private void DataEntry_ShowIndexHelp()
        {
            IndexResults.Visibility = Visibility.Hidden;
            MarkDownDisplay.Visibility = Visibility.Visible;
        }

        #endregion





    }
}
