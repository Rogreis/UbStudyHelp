using System.Diagnostics;
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
        public static string PathParameters;

        public static ControlsAppearance Appearance = new ControlsAppearance();

        protected override void OnStartup(StartupEventArgs e)
        {
            ControlzEx.Theming.ThemeManager.Current.ThemeSyncMode = ControlzEx.Theming.ThemeSyncMode.SyncAll;
            ControlzEx.Theming.ThemeManager.Current.SyncTheme();
            base.OnStartup(e);
        }


        protected override void OnExit(ExitEventArgs e)
        {
            // Serialize parameters
            ParametersCore.Serialize((ParametersCore)StaticObjects.Parameters, PathParameters);
            base.OnExit(e);
        }

    }



}
