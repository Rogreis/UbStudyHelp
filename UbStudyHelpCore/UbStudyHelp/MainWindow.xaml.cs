using ControlzEx.Theming;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
            EventsControl.SendMessage += EventsControl_SendMessage;

        }

        private void EventsControl_SendMessage(string message)
        {
            ShowMessage(message);
        }

        private void ShowMessage(string message, bool fatalError= false)
        {
            Debug.WriteLine(message);
            StatusBarMessages.Text = message;
            if (fatalError)
            {
                MessageBox.Show(message);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private bool LoadData()
        {
            GetDataFiles dataFiles = new GetDataFiles();
            try
            {
                if (!dataFiles.CheckFiles(App.BaseTubFilesPath))
                {
                    return false;
                }
                return Book.Inicialize(App.BaseTubFilesPath);
            }
            catch (Exception ex)
            {
                EventsControl.FireSendMessage("Loading TOC data", ex);
                return false;
            }
        }



        private void formText_Loaded(object sender, RoutedEventArgs e)
        {
            StatusBarVersion.Text = "v 2.0";
            // Information about current theme stored for other controls
            if (!LoadData())
            {
                ShowMessage("Data not loaded!", true);
            }
        }


        private void LaunchUFSite(object sender, RoutedEventArgs e)
        {
            //Process.Start("http://www.urantia.org");
            TestWindow cw = new TestWindow();
            cw.ShowInTaskbar = false;
            cw.Owner = Application.Current.MainWindow;
            cw.Show();
        }






    }
}
