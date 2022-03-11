using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UbStandardObjects;

namespace UbStudyHelp.Classes
{
    public class Index
    {
        private string basePath;
        private LuceneIndexSearch lucene = null;

        public List<TubIndex> TubIndex { get; private set; } = new List<TubIndex>();

        public Index(string basePathForFiles)
        {
            basePath = basePathForFiles;
            lucene = new LuceneIndexSearch(basePath, 0); // Only English for now
        }

        public bool Load()
        {
            try
            {
                string pathFile = Path.Combine(basePath, "tubIndex_000.json");
                string json = File.ReadAllText(pathFile);
                TubIndex = JsonConvert.DeserializeObject<List<TubIndex>>(json);
                return lucene.CreateLuceneIndexForUBIndex(TubIndex);
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("Error loading index.", ex);
                return false;
            }
        }

        public List<string> Search(string startString)
        {
            List<string> wordsFound = lucene.Execute(startString);
            if (wordsFound == null)
            {
                StaticObjects.Logger.Warn("Index search returned null for searching: " + startString);
                return null;
            }

            List<TubIndex> details = new List<TubIndex>();
            foreach (string word in wordsFound)
            {
                List<TubIndex> detailsForWord = TubIndex.FindAll(d => d.Title.StartsWith(word, StringComparison.CurrentCultureIgnoreCase));
                if (detailsForWord != null)
                {
                    details.AddRange(detailsForWord);
                }
            }
            if (details.Count == 0)
                return null;
            details.Sort((d1, d2) => string.Compare(d1.Title, d2.Title, true));
            return details.Select(d => d.Title).Distinct().ToList();
        }

        public TubIndex GetIndexEntry(string indexEntry)
        {
            return TubIndex.Find(d => string.Compare(d.Title, indexEntry) == 0);
        }
    }
}
