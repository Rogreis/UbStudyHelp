using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Lucene.Net.QueryParsers.Flexible.Messages;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Text;
using static System.Environment;

namespace UbStudyHelp.Classes
{
    internal class DataInitializer
    {
        
        private static string DataFolder()
        {

            string processName = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            var commonpath = GetFolderPath(SpecialFolder.CommonApplicationData);
            return Path.Combine(commonpath, processName);
        }


        private static string MakeProgramDataFolder(string fileName = null)
        {
            string folder = DataFolder();
            Directory.CreateDirectory(folder);
            if (fileName != null) 
            {
                folder = Path.Combine(folder, fileName);
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        /// <summary>
        /// Inicialize the list of available translations
        /// </summary>
        /// <param name="dataFiles"></param>
        /// <returns></returns>
        private static bool InicializeTranslations(GetDataFiles dataFiles)
        {
            try
            {
                if (StaticObjects.Book.Translations == null)
                {
                    StaticObjects.Book.Translations = dataFiles.GetTranslations();
                }
                return true;
            }
            catch (Exception ex)
            {
                string message = $"Could not initialize available translations. See log.";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
                return false;
            }
        }

        /// <summary>
        /// Initialize the format table used for editing translations
        /// </summary>
        private static bool GetFormatTable(GetDataFilesCore dataFiles)
        {
            try
            {
                if (StaticObjects.Book.FormatTableObject == null)
                {
                    StaticObjects.Book.FormatTableObject = new FormatTable(dataFiles.GetFormatTable());
                }
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error($"Missing format table. May be you do not have the correct data to use this tool.", ex);
                return false;
            }
        }


        private static bool InitTranslation(GetDataFilesCore dataFiles, short translationId, ref Translation trans)
        {
            EventsControl.FireSendMessage($"Getting translation {translationId}");
            trans = null;
            if (translationId < 0) return true;
            trans = dataFiles.GetTranslation(translationId);
            if (trans == null)
            {
                StaticObjects.Logger.Error($"Non existing translation: {translationId}");
                return false;
            }
            EventsControl.FireSendMessage($"Getting translation {trans.Description}");
            if (trans.IsEditingTranslation)
            {
                EventsControl.FireSendMessage($"Getting editying translation {trans.Description}");
                EventsControl.FireSendMessage("Checking folders for editing translation");
                if (!Directory.Exists(StaticObjects.Parameters.EditParagraphsRepositoryFolder))
                {
                    StaticObjects.Logger.Error("There is no repositoty set for editing translation");
                    return false;
                }
                if (!GitCommands.IsValid(StaticObjects.Parameters.EditParagraphsRepositoryFolder))
                {
                    StaticObjects.Logger.Error($"Folder is not a valid respository: {StaticObjects.Parameters.EditParagraphsRepositoryFolder}");
                    return false;
                }
                EventsControl.FireSendMessage("Found a valid repository");
                // Format table must exist for editing translation
                if (!GetFormatTable(dataFiles))
                {
                    return false;
                }
            }
            return trans.CheckData();
        }


        public static bool InitLogger()
        {
            try
            {
                // Log for errors
                string pathLog = Path.Combine(MakeProgramDataFolder(), "UbStudyHelp.log");
                StaticObjects.Logger = new LogCore();
                StaticObjects.Logger.Initialize(pathLog, false);
                StaticObjects.Logger.Info("»»»» Startup");
                return true;
            }
            catch (Exception ex)
            {
                string message = "Could not initialize logger";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
                return false;
            }
        }

        public static bool InitParameters()
        {
            try
            {
                App.PathParameters = Path.Combine(MakeProgramDataFolder(), "UbStudyHelp.json");
                if (!File.Exists(App.PathParameters))
                {
                    StaticObjects.Logger.Info("Parameters not found, creating a new one: " + App.PathParameters);
                }
                StaticObjects.Parameters = ParametersCore.Deserialize(App.PathParameters);


                // Set folders and URLs used
                // This must be set in the parameters
                StaticObjects.Parameters.ApplicationDataFolder = MakeProgramDataFolder();
                StaticObjects.Parameters.IndexSearchFolders = MakeProgramDataFolder("IndexSearch");
                StaticObjects.Parameters.TubSearchFolders = MakeProgramDataFolder("TubSearch");


                // This application has a differente location for TUB Files
                StaticObjects.Parameters.TUB_Files_RepositoryFolder = MakeProgramDataFolder("TUB_Files");
                StaticObjects.Parameters.EditParagraphsRepositoryFolder = MakeProgramDataFolder("PtAlternative");
                // Paths not used in this program
                //StaticObjects.Parameters.EditBookRepositoryFolder = "C:\\Trabalho\\Github\\Rogerio\\TUB_PT_BR";
                //StaticObjects.Parameters.UrlRepository = "https://github.com/Rogreis/PtAlternative";

                return true;
            }
            catch (Exception ex)
            {
                string message = "Could not initialize parameters";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
                return false;
            }
        }

        public static bool InitTranslations()
        {
            try
            {
                GetDataFilesCore dataFiles = new GetDataFilesCore((ParametersCore)StaticObjects.Parameters);
                StaticObjects.Book = new BookCore(dataFiles);

                // Verify respository existence
                if (!GitCommands.IsValid(StaticObjects.Parameters.TUB_Files_RepositoryFolder))
                {
                    if (!GitCommands.Clone(StaticObjects.Parameters.TUB_Files_Url, StaticObjects.Parameters.TUB_Files_RepositoryFolder))
                    {
                        StaticObjects.Logger.FatalError("Could not clone translations");
                        return false;
                    }
                }
                else
                {
                    if (!GitCommands.Pull(StaticObjects.Parameters.TUB_Files_RepositoryFolder))
                    {
                        StaticObjects.Logger.FatalError("Could not checkout TUB translations");
                        return false;
                    }
                }


                // Verify respository existence
                if (!GitCommands.IsValid(StaticObjects.Parameters.EditParagraphsRepositoryFolder))
                {
                    if (!GitCommands.Clone(StaticObjects.Parameters.EditParagraphsUrl, StaticObjects.Parameters.EditParagraphsRepositoryFolder))
                    {
                        StaticObjects.Logger.FatalError("Could not clone edit translation");
                        return false;
                    }
                }
                else
                {
                    if (!GitCommands.Pull(StaticObjects.Parameters.EditParagraphsRepositoryFolder))
                    {
                        StaticObjects.Logger.FatalError("Could not checkout TUB translations");
                        return false;
                    }
                }

                EventsControl.FireSendMessage("Getting translations list");
                if (!InicializeTranslations(dataFiles))
                {
                    return false;
                }

                if (!InitTranslation(dataFiles, StaticObjects.Parameters.LanguageIDLeftTranslation, ref StaticObjects.Book.LeftTranslation))
                {
                    return false;
                }

                if (!InitTranslation(dataFiles, StaticObjects.Parameters.LanguageIDMiddleTranslation, ref StaticObjects.Book.MiddleTranslation))
                {
                    return false;
                }

                if (!InitTranslation(dataFiles, StaticObjects.Parameters.LanguageIDRightTranslation, ref StaticObjects.Book.RightTranslation))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                string message = "Could not initialize translations 2. See log.";
                StaticObjects.Logger.Error(message, ex);
                StaticObjects.Logger.FatalError(message);
                return false;
            }
        }
    }
}
