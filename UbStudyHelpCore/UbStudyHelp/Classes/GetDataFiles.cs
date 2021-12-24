using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Xml.Linq;
using UbStudyHelp.Classes;
using System.Runtime.InteropServices.ComTypes;

namespace UbStudyHelp.Classes
{
    public class GetDataFiles
    {
        public const string ControlFileName = "UbHelpTextControl.xml";
        private const string indexFileName = "Index.zip";


        //private bool GetPrivate(string destinationFolder, string fileName, bool isZip = true)
        //{
        //    try
        //    {
        //        var githubToken = "[token]";
        //        var url = "https://github.com/[username]/[repository]/archive/[sha1|tag].zip";
        //        var path = @"[local path]";

        //        using (var client = new System.Net.Http.HttpClient())
        //        {
        //            var credentials = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:", githubToken);
        //            credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
        //            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        //            var contents = client.GetByteArrayAsync(url).Result;
        //            System.IO.File.WriteAllBytes(path, contents);
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = $"Error getting file {fileName}: {ex.Message}. May be you do not have the correct data to use this tool.";
        //        Log.Logger.Error(message, ex);
        //        return false;
        //    }
        //}



        /// <summary>
        /// Get public data from github
        /// <see href="https://raw.githubusercontent.com/Rogreis/TUB_Files/main"/>
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        private bool GetPublic(string destinationFolder, string fileName, bool isZip = true)
        {
            try
            {
                // https://raw.githubusercontent.com/Rogreis/TUB_Files/main/UbHelpTextControl.xml
                Log.Logger.Info("Downloading files from github.");
                string url = isZip ? $"https://github.com/Rogreis/TUB_Files/raw/main/{fileName}" :
                    $"https://raw.githubusercontent.com/Rogreis/TUB_Files/main/{fileName}";
                var path = Path.Combine(destinationFolder, fileName);
                Log.Logger.Info("Url: " + url);
                Log.Logger.Info("Destination path: " + path);

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("a", "a");
                    wc.DownloadFile(url, path);
                }
                if (isZip)
                {
                    Log.Logger.Info("Unzipping to: " + destinationFolder);
                    ZipFile.ExtractToDirectory(path, destinationFolder);
                }


                //using (var client = new System.Net.Http.HttpClient())
                //{
                //    var contents = client.GetByteArrayAsync(url).Result;
                //    System.IO.File.WriteAllBytes(path, contents);
                //}

                //var githubToken = "token";
                //var request = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/$OWNER/$REPO/contents/$PATH");
                //request.Headers.Add(HttpRequestHeader.Authorization, string.Concat("token ", githubToken));
                //request.Accept = "application/vnd.github.v3.raw";
                //request.UserAgent = "test app"; //user agent is required https://developer.github.com/v3/#user-agent-required
                //using (var response = request.GetResponse())
                //{
                //    var encoding = System.Text.ASCIIEncoding.UTF8;
                //    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                //    {
                //        var fileContent = reader.ReadToEnd();
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                string message = $"Error getting file {fileName}: {ex.Message}. May be you do not have the correct data to use this tool.";
                Log.Logger.Error(message, ex);
                return false;
            }
        }

        private bool GetFile(string destinationFolder, string fileName, bool isZip = true)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Log.Logger.Info("Creating data folder: " + destinationFolder);
                Directory.CreateDirectory(destinationFolder);
            }
            string path = Path.Combine(destinationFolder, fileName);
            if (!File.Exists(path))
            {
                Log.Logger.Info("File does not exist: " + path);
                return GetPublic(destinationFolder, fileName, isZip);
            }
            else
            {
                Log.Logger.Info("File exists: " + path);
                return true;
            }
        }


        public bool CheckFiles(string destinationFolder)
        {
            try
            {
                if (!GetFile(destinationFolder, ControlFileName, false))
                {
                    return false;
                }
                if (!GetFile(destinationFolder, indexFileName, true))
                {
                    return false;
                }

                string pathlistTranslations = Path.Combine(destinationFolder, ControlFileName);

                XElement xElem = XElement.Load(pathlistTranslations);
                List<Translation> Translations = (from lang in xElem.Descendants("Translation")
                                                  select new Translation(lang)).ToList();

                foreach (Translation translation in Translations)
                {
                    string fileTranslationName = $"L{translation.LanguageID:000}.zip";
                    if (!GetFile(destinationFolder, fileTranslationName, true))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Data not loaded!", ex);
                return false;
            }
        }

    }

}
