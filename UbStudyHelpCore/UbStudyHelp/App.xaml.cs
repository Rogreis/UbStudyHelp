using System;
using System.IO;
using System.Windows;
using UbStudyHelp.Classes;

namespace UbStudyHelp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Styles based on https://mahapps.com/docs/guides/quick-start
        private static string pathParameters;

        public static Parameters ParametersData = new Parameters();

        public static string BaseTubFilesPath = "";

        public static ControlsAppearance Appearance = new ControlsAppearance();

        private static bool LoadData()
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


        protected override void OnStartup(StartupEventArgs e)
        {
            pathParameters = Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "UbStudyHelp.json");
            string exePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            BaseTubFilesPath = System.IO.Path.Combine(exePath, "TUB_Files");

            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();

            ParametersData = Parameters.Deserialize(pathParameters);

            if (!LoadData())
            {
                throw new Exception("Data not loaded!");
            }
            base.OnStartup(e);
        }


        protected override void OnExit(ExitEventArgs e)
        {
            // Serialize parameters
            Parameters.Serialize(ParametersData, pathParameters);
            base.OnExit(e);
        }

    }



}
