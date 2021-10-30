using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Xml.Linq;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Classes
{
    public class GetDataFiles
    {
        private bool GetPrivate()
        {
            try
            {
                var githubToken = "[token]";
                var url = "https://github.com/[username]/[repository]/archive/[sha1|tag].zip";
                var path = @"[local path]";

                using (var client = new System.Net.Http.HttpClient())
                {
                    var credentials = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:", githubToken);
                    credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                    var contents = client.GetByteArrayAsync(url).Result;
                    System.IO.File.WriteAllBytes(path, contents);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        private bool GetPublic(string destinationFolder, string fileName, bool isZip= true)
        {
            try
            {
                // https://raw.githubusercontent.com/Rogreis/TUB_Files/main/Languages.xml

                string url = isZip ? $"https://github.com/Rogreis/TUB_Files/raw/main/{fileName}" :
                    $"https://raw.githubusercontent.com/Rogreis/TUB_Files/main/{fileName}";
                var path = Path.Combine(destinationFolder, fileName);

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("a", "a");
                    wc.DownloadFile(url, path);
                }
                if (isZip)
                {
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
                throw new Exception($"Error getting file {fileName}: {ex.Message}. May be you do not have the correct data to use this tool.");
            }
        }

        private const string translationsFileName = "Languages.xml";
        private const string indexFileName = "Index.zip";


        private void GetFile(string destinationFolder, string fileName, bool isZip = true)
        {
            Directory.CreateDirectory(destinationFolder);
            string path = Path.Combine(destinationFolder, fileName);
            if (!File.Exists(path))
            {
                GetPublic(destinationFolder, fileName, isZip);
            }
        }


        public bool CheckFiles(string destinationFolder)
        {
            try
            {
                GetFile(destinationFolder, translationsFileName, false);
                GetFile(destinationFolder, indexFileName, true);

                string pathlistTranslations = Path.Combine(destinationFolder, "Languages.xml");

                XElement xElem = XElement.Load(pathlistTranslations);
                List<Translation> Translations = (from lang in xElem.Descendants("Translation")
                                                select new Translation(lang)).ToList();

                foreach(Translation translation in Translations)
                {
                    string fileTranslationName = $"L{translation.LanguageID:000}.zip";
                    GetFile(destinationFolder, fileTranslationName, true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
