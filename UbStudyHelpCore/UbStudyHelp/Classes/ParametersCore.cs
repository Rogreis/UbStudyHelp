using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UbStandardObjects;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{

    [Serializable]
    public class ParametersCore : Parameters
    {


        public string ThemeName { get; set; } = "Light";

        public string ThemeColor { get; set; } = "Blue";



        /// <summary>
        /// Add a string to list to be saved with parameters
        /// Keep a control local string updated
        /// Remove duplicates
        /// </summary>
        /// <param name="paramStringList"></param>
        /// <param name="controlStringList"></param>
        /// <param name="indexEntry"></param>
        public void AddEntry(List<string> paramStringList, ObservableCollection<string> controlStringList, string indexEntry)
        {
            // Just avoid duplicates
            if (paramStringList.Contains(indexEntry, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }
            if (paramStringList.Count == MaxExpressionsStored)
            {
                controlStringList.RemoveAt(controlStringList.Count - 1);
                paramStringList.RemoveAt(paramStringList.Count - 1);
            }
            controlStringList.Insert(0, indexEntry);
            paramStringList.Insert(0, indexEntry);
        }


        /// <summary>
        /// Serialize the parameters instance
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pathParameters"></param>
        public static void Serialize(ParametersCore p, string pathParameters)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(p, Formatting.Indented);
                File.WriteAllText(pathParameters, jsonString);
            }
            catch  {    }
        }

        /// <summary>
        /// Deserialize the parameters instance
        /// </summary>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        public static ParametersCore Deserialize(string pathParameters)
        {
            try
            {
                var jsonString = File.ReadAllText(pathParameters);
                return JsonConvert.DeserializeObject<ParametersCore>(jsonString);
            }
            catch 
            {
                return new ParametersCore();
            }
        }



    }
}
