using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace UbStudyHelp.Classes
{
    public static class Log
    {
        private static bool _logIniciado = false;

        public static string PathLog { get; set; }

        public static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Start(string pathLog, bool append = false, bool reStart = false)
        {
            if (!append)
            {
                if (File.Exists(pathLog))
                {
                    File.Delete(pathLog);
                }
            }
            PathLog = pathLog;
            SetupLof4Net(append, reStart);
            Enable();
        }

        private static void SetupLof4Net(bool append, bool reStart= false)
        {
            if (_logIniciado)
                return;

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            //patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ConversionPattern = "%date [%thread] %-5level %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = append;
            roller.File = PathLog;
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "1GB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            //MemoryAppender memory = new MemoryAppender();
            //memory.ActivateOptions();
            //hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

#if DEBUG
            if (!_logIniciado)
            {
                LogManager.GetRepository().Threshold = Level.All;
                _logIniciado = true;
                if (reStart)
                {
                    Logger.Info("UbStudyHelp log re-started");
                }
                else
                {
                    Logger.Info("UbStudyHelp log Started");
                }
            }
#else
            LogManager.GetRepository().Threshold = Level.Off;
#endif

        }

        public static void Close()
        {
            LogManager.GetRepository().Shutdown();
            _logIniciado = false;
        }

        private static void ShowLog()
        {
            Close();
            Process.Start("notepad.exe", PathLog);

            //using (var fs = new FileStream(PathLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //using (var sr = new StreamReader(fs, Encoding.Default))
            //{
            //    string text = sr.ReadToEnd();
            //}

        }


        public static void Enable()
        {
            //SetupLof4Net(true);
            LogManager.GetRepository().Threshold = Level.All;
            //((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.All;
            //((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
        }

        public static void Disable()
        {
            LogManager.GetRepository().Threshold = Level.Off;
            //((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.Off;
            //((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
        }


        public static void NonFatalError(string message)
        {
            Log.Logger.Error(message);
            if (MessageBox.Show(message + ".\n\nOpen log file?", "Ub Study Help", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ShowLog();
            }
        }

        public static void FatalError(string message)
        {
            Log.Logger.Error(message);
            if (MessageBox.Show(message + ".\n\nWe are sorry, but Ub Study Help needs to be closed.\n\nOpen log file?", "Ub Study Help",
                MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                ShowLog();
            }
            Environment.Exit(1);
        }

        public static void Show()
        {
            ShowLog();
            Start(PathLog, true, true);
        }



    }
}
