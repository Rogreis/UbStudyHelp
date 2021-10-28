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


        protected override void OnStartup(StartupEventArgs e)
        {
            pathParameters = Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "UbStudyHelp.yaml");
            string exePath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            BaseTubFilesPath = System.IO.Path.Combine(exePath, "TUB_Files");

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
