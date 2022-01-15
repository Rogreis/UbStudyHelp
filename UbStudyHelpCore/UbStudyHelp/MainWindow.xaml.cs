using ControlzEx.Theming;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// For styling documentation <see href="https://mahapps.com/docs/guides/quick-start"/>
    /// </summary>
    public partial class MainWindow
    {

        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            EventsControl.SendMessage += EventsControl_SendMessage;
            EventsControl.FontChanged += EventsControl_FontChanged;
            GridSplitterLeft.DragCompleted += GridSplitterLeft_DragCompleted;

            timer.Interval = TimeSpan.FromMinutes(3);
            timer.Tick += Timer_Tick;
            timer.Start();


        }



        private void Timer_Tick(object sender, EventArgs e)
        {

            System.Windows.Threading.Dispatcher.CurrentDispatcher.Hooks.DispatcherInactive += Hooks_DispatcherInactive;

        }

        private bool InactiveFired = false;
        private void Hooks_DispatcherInactive(object sender, EventArgs e)
        {
            if (InactiveFired)
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Hooks.DispatcherInactive -= Hooks_DispatcherInactive;
                return;
            }
            InactiveFired = true;
            frmNewVersion frm = new frmNewVersion();
            frm.ShowDialog();
        }

        //int count= 0;
        //private void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        //{
        //    count++;
        //    Debug.WriteLine($"{count,8} componentDispatcher_ThreadIdle");
        //    timer.Stop();
        //    timer.Start();
        //}

        private void FontChanged()
        {
            App.Appearance.SetFontSize(StatusBarVersion);
            App.Appearance.SetFontSize(StatusBarMessages);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            FontChanged();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GridTexts.ColumnDefinitions[0].Width = new GridLength(App.ParametersData.SpliterDistance);
            FontChanged();
        }

        private void GridSplitterLeft_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            App.ParametersData.SpliterDistance = GridTexts.ColumnDefinitions[0].ActualWidth;
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
                Log.Logger.Error("Fatal error: " + message);
                MessageBox.Show(message);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                Log.Logger.Warn(message);
            }
        }


        private void formText_Loaded(object sender, RoutedEventArgs e)
        {
            StatusBarVersion.Text = "v 2.0.1";
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
