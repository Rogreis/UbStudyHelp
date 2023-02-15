using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using MahApps.Metro.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// For styling documentation <see href="https://mahapps.com/docs/guides/quick-start"/>
    /// </summary>
    public partial class MainWindow
    {
        // Controls the app initializantion done in the activated event
        private bool DataInitialized= false;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Activated += MainWindow_Activated;
            EventsControl.SendMessage += EventsControl_SendMessage;
            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.NewPaperShown += EventsControl_NewPaperShown;
            GridSplitterLeft.DragCompleted += GridSplitterLeft_DragCompleted;

            GeometryImages images = new GeometryImages();
            ButtonRecover.Source = images.GetImage(GeometryImagesTypes.Sort);

            // Objects in the status bar do not need theme update
            //EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
        }


        private void EventsControl_NewPaperShown()
        {
            string messagePaper =
            this.Title = $"The Urantia Book Study Help - Paper {StaticObjects.Parameters.Entry.Paper}";
            StatusBarPaper.Text = $"Paper { StaticObjects.Parameters.Entry.Paper}";
        }

        private void FontChanged()
        {
            App.Appearance.SetFontSize(StatusBarVersion);
            App.Appearance.SetFontSize(StatusBarPaper);
            App.Appearance.SetFontSize(StatusBarMessages);
            App.Appearance.SetFontSize(ReferenceInputBox);
        }


        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            FontChanged();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (!DataInitialized)
            {
                if (!DataInitializer.InitLogger())
                {
                    throw new Exception("Could not initialize logger.");
                }

                if (!DataInitializer.InitParameters())
                {
                    throw new Exception("Could not initialize parameters.");
                }


                GridTexts.ColumnDefinitions[0].Width = new GridLength(StaticObjects.Parameters.SpliterDistance);
                FontChanged();
                StatusBarVersion.Text = "v 2.1.2";
                if (!DataInitializer.InitTranslations())
                {
                    throw new Exception("Could not initialize translations 1.");
                }
                DataInitialized = true;
                LeftPanelControl.Init();
                App.Appearance.Theme = "Dark"; // : "Light";
            }
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

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double width = this.ActualWidth;
                double height = gridMain.ActualHeight - StatusBarMainWindow.ActualHeight - SystemParameters.CaptionHeight;
                EventsControl.FireMainWindowSizeChanged(width, height);
            }
            catch // Ignore errors
            {
            }


        }

        private void ReferenceInputBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string paragraphRef = ReferenceInputBox.Text;
                string aMessage = "";
                TOC_Entry entry = TOC_Entry.FromReference(paragraphRef, ref aMessage);
                if (entry != null)
                {
                    EventsControl.FireSendMessage(aMessage);
                    EventsControl.FireTOCClicked(entry);
                }
                else
                {
                    EventsControl.FireSendMessage(aMessage);
                }
            }
        }

        private bool IsFixingTheme = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsFixingTheme) return;
            IsFixingTheme= true;
            App.Appearance.Theme = "Dark";
            App.Appearance.Theme = "Light";
            App.Appearance.Theme = "Dark";
            IsFixingTheme = false;
        }
    }
}
