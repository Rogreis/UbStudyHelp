using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UbStandardObjects.Objects
{
    public abstract class GetDataFiles
    {
        protected const string ControlFileName = "Translations.json";

        protected const string indexFileName = "Index.zip";


        /// <summary>
        /// Get public data from github
        /// <see href="https://raw.githubusercontent.com/Rogreis/TUB_Files/main"/>
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="isZip"></param>
        /// <returns></returns>
        protected byte[] GetGitHubBinaryFile(string fileName, bool isZip= false)
        {
            try
            {
                // https://raw.githubusercontent.com/Rogreis/TUB_Files/main/UbHelpTextControl.xml
                UbStandardObjects.StaticObjects.Logger.Info("Downloading files from github.");
                string url = isZip ? $"https://github.com/Rogreis/TUB_Files/raw/main/{fileName}" :
                    $"https://raw.githubusercontent.com/Rogreis/TUB_Files/main/{fileName}";
                UbStandardObjects.StaticObjects.Logger.Info("Url: " + url);

                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("a", "a");
                    byte[] bytes = wc.DownloadData(url);
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                string message = $"Error getting file {fileName}: {ex.Message}. May be you do not have the correct data to use this tool.";
                UbStandardObjects.StaticObjects.Logger.Error(message, ex);
                return null;
            }
        }

        /// <summary>
        /// Copy streams
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        protected static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Unzip a Gzipped translation file
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string BytesToString(byte[] bytes, bool isZip)
        {
            if (!isZip)
            {
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                using (var msi = new MemoryStream(bytes))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        //gs.CopyTo(mso);
                        CopyTo(gs, mso);
                    }

                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
        }


        /// <summary>
        /// Get a translation
        /// Verify if alread doanloaded
        /// </summary>
        /// <param name="translatioId"></param>
        public abstract Translation GetTranslation(short translatioId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract List<Translation> GetTranslations();

    }

}
