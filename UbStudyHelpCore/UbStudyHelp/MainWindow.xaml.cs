using System.Windows;
using System.Windows.Controls;
using UbStandardObjects;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// For styling documentation <see href="https://mahapps.com/docs/guides/quick-start"/>
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            EventsControl.SendMessage += EventsControl_SendMessage;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.NewPaperShown += EventsControl_NewPaperShown;
            GridSplitterLeft.DragCompleted += GridSplitterLeft_DragCompleted;
        }


        private void FontChanged()
        {
            App.Appearance.SetFontSize(StatusBarVersion);
            App.Appearance.SetFontSize(StatusBarPaper);
            App.Appearance.SetFontSize(StatusBarMessages);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            FontChanged();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GridTexts.ColumnDefinitions[0].Width = new GridLength(StaticObjects.Parameters.SpliterDistance);
            FontChanged();
        }

        private void GridSplitterLeft_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            StaticObjects.Parameters.SpliterDistance = GridTexts.ColumnDefinitions[0].ActualWidth;
            EventsControl.FireGridSplitter(GridTexts.ColumnDefinitions[0].ActualWidth);
        }

        private void EventsControl_SendMessage(string message)
        {
            ShowMessage(message);
        }

        private void ShowMessage(string message, bool fatalError= false)
        {
            StatusBarMessages.Text = message;
            if (fatalError)
            {
                StaticObjects.Logger.Error("Fatal error: " + message);
                MessageBox.Show(message);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                StaticObjects.Logger.Warn(message);
            }
        }


        private void formText_Loaded(object sender, RoutedEventArgs e)
        {
            StatusBarVersion.Text = "v 2.0";
        }


        private void LaunchUFSite(object sender, RoutedEventArgs e)
        {
            //Process.Start("http://www.urantia.org");
            TestWindow cw = new TestWindow();
            cw.ShowInTaskbar = false;
            cw.Owner = Application.Current.MainWindow;
            cw.Show();
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double width = this.ActualWidth;
                double height = gridMain.ActualHeight - StatusBarMainWindow.ActualHeight - SystemParameters.CaptionHeight;
                
                //Debug.WriteLine($"{this.ActualWidth}   {this.ActualHeight} Splitter= {GridTexts.ColumnDefinitions[0].ActualWidth}");
                //Debug.WriteLine($"{gridMain.ActualWidth}   {gridMain.ActualHeight}  {StatusBarMainWindow.ActualWidth}  {StatusBarMainWindow.ActualHeight}");

                EventsControl.FireMainWindowSizeChanged(width, height);
            }
            catch // Ignore errors
            {
                //Debug.WriteLine("Error");
            }


        }
    }
}
