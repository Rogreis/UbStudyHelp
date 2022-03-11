using ControlzEx.Standard;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Text;
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

        public static string BaseTubFilesPath = "";

        public static ControlsAppearance Appearance = new ControlsAppearance();

        private string DataFolder()
        {
            string processName = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
            return Path.Combine(commonpath, processName);
        }


        private string MakeProgramDataFolder(string fileName)
        {
            string folder = DataFolder();
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, fileName);
        }




        protected override void OnStartup(StartupEventArgs e)
        {
            string pathLog = MakeProgramDataFolder("UbStudyHelp.log");

            StaticObjects.Logger = new LogCore();
            StaticObjects.Logger.Initialize(pathLog, false);

            pathParameters = MakeProgramDataFolder("UbStudyHelp.json");
            if (!File.Exists(pathParameters))
            {
                StaticObjects.Logger.Info("Parameters not found, creating a new one: " + pathParameters);
            }
            StaticObjects.Parameters= ParametersCore.Deserialize(pathParameters);

            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();

            BaseTubFilesPath = MakeProgramDataFolder("TUB_Files");

            StaticObjects.Book= new BookCore();

            if (!StaticObjects.Book.Inicialize(BaseTubFilesPath, StaticObjects.Parameters.LanguageIDLeftTranslation, StaticObjects.Parameters.LanguageIDRightTranslation))
            {
                StaticObjects.Logger.Error("Data not loaded!");
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
            ParametersCore.Serialize((ParametersCore)StaticObjects.Parameters, pathParameters);
            base.OnExit(e);
        }

    }



}
