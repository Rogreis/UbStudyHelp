using ControlzEx.Standard;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using UbStudyHelp.Classes;
using static System.Environment;

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
                Log.Logger.Error("Data not loaded!", ex);
                return false;
            }
        }


        private string MakeProgramDataFolder(string fileName)
        {
            string processName = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
            string folder= Path.Combine(commonpath, processName);
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, fileName);
        }




        protected override void OnStartup(StartupEventArgs e)
        {
            //string pathLog = Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "UbStudyHelp.log");
            string pathLog = MakeProgramDataFolder("UbStudyHelp.log");
            Log.Start(pathLog);

            //pathParameters = Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "UbStudyHelp.json");
            pathParameters = MakeProgramDataFolder("UbStudyHelp.json");
            if (!File.Exists(pathParameters))
            {
                Log.Logger.Info("Parameters not found, creating a new one: " + pathParameters);
            }
            ParametersData = Parameters.Deserialize(pathParameters);


            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();


            BaseTubFilesPath = MakeProgramDataFolder("TUB_Files");
            if (!LoadData())
            {
                Log.Logger.Error("Data not loaded!");
                if(MessageBox.Show("Data not loaded!\n\nOpen log file?", "Ub Study Help", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start("notepad.exe", pathLog);
                }
                return;
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
