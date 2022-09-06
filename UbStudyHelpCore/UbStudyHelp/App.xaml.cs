﻿using System.Diagnostics;
using System.IO;
using System.Text.Json;
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
            // Log for errors
            string pathLog = MakeProgramDataFolder("UbStudyHelp.log");

            StaticObjects.Logger = new LogCore();
            StaticObjects.Logger.Initialize(pathLog, false);
            StaticObjects.Logger.Info("»»»» Startup");

            pathParameters = MakeProgramDataFolder("UbStudyHelp.json");
            if (!File.Exists(pathParameters))
            {
                StaticObjects.Logger.Info("Parameters not found, creating a new one: " + pathParameters);
            }
            StaticObjects.Parameters= ParametersCore.Deserialize(pathParameters);


            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();


            // Set folders and URLs used
            // This must be set in the parameters
            StaticObjects.Parameters.ApplicationFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            StaticObjects.Parameters.UbStudyHelpTubFilesSourcePath = MakeProgramDataFolder("TUB_Files");

            // Paths not used in this program
            //StaticObjects.Parameters.TUB_Files_RepositoryFolder = "C:\\Trabalho\\Github\\Rogerio\\TUB_Files";
            //StaticObjects.Parameters.EditParagraphsRepositoryFolder = "C:\\Trabalho\\Github\\Rogerio\\PtAlternative";
            //StaticObjects.Parameters.EditBookRepositoryFolder = "C:\\Trabalho\\Github\\Rogerio\\TUB_PT_BR";
            //StaticObjects.Parameters.UrlRepository = "https://github.com/Rogreis/PtAlternative";

            StaticObjects.Book= new BookCore();

            GetDataFilesCore dataFiles = new GetDataFilesCore((ParametersCore)StaticObjects.Parameters);

            if (!StaticObjects.Book.Inicialize(dataFiles, StaticObjects.Parameters.LanguageIDLeftTranslation, StaticObjects.Parameters.LanguageIDRightTranslation))
            {
                StaticObjects.Logger.FatalError("Data not loaded!");
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
