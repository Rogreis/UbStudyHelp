using ControlzEx.Theming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UbStudyHelp.Classes;
using YamlDotNet.Serialization;

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
            pathParameters = Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "UbStudyHelp.yaml");
            string exePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            BaseTubFilesPath = System.IO.Path.Combine(exePath, "TUB_Files");

            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();



            // When parameters files does not exist, we use the default already created.
            if (File.Exists(pathParameters))
            {
                try
                {
                    Deserializer deserializer = new Deserializer();
                    string yamlData = File.ReadAllText(pathParameters);
                    ParametersData = deserializer.Deserialize<Parameters>(yamlData);
                }
                catch (Exception ex) 
                {
                    // In case of error, use default data, ignoring errors
                    string message = ex.Message;
                }
            }

            if (!LoadData())
            {
                throw new Exception("Data not loaded!");
            }


            base.OnStartup(e);
        }


        protected override void OnExit(ExitEventArgs e)
        {
            // Serialize parameters
            Serializer serializer = new Serializer();
            File.WriteAllText(pathParameters, serializer.Serialize(ParametersData));
            base.OnExit(e);
        }

    }



}
