using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UbStudyHelp.Classes;

namespace UbStudyHelpUnitTest
{
    [TestFixture]
    public class LuceneIndexTests
    {


        // Private methods
        private string GetBaseTubPath()
        {
            string exePath = TestContext.CurrentContext.TestDirectory;
            Assert.IsTrue(!string.IsNullOrEmpty(exePath), "Missing current context test directory");
            return System.IO.Path.Combine(exePath, "TUB_Files");
        }





        [SetUp]
        public void Setup()
        {
        }

        [Test, Order(1)]
        public void TestTubFolderExists()
        {
            string baseTubFilesPath = GetBaseTubPath();
            Assert.IsTrue(Directory.Exists(baseTubFilesPath), "Missing TUB files directory");
        }

        [Test, Order(2)]
        public void TestIndexFilesExist()
        {
            string baseTubFilesPath = GetBaseTubPath();
            Index index = new Index(baseTubFilesPath);
            foreach (string indexFileName in index.FileSufixes)
            {
                string indexFilePath = Path.Combine(baseTubFilesPath, indexFileName + ".json");
                Assert.IsTrue(File.Exists(indexFilePath), $"Missing Index file {indexFileName}");
            }
        }

        [Test, Order(3)]
        public void TestIndexLoad()
        {
            string baseTubFilesPath = GetBaseTubPath();
            Index index = new Index(baseTubFilesPath);
            IndexTexts Indexes = new IndexTexts();
            foreach (string indexFileName in index.FileSufixes)
            {
                string indexFilePath = Path.Combine(baseTubFilesPath, indexFileName + ".json");
                string json = File.ReadAllText(indexFilePath);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(json), $"Jason string invalid for {indexFileName}");
                IndexTexts indexGroup = JsonConvert.DeserializeObject<IndexTexts>(json);
                Assert.IsNotNull(indexGroup, $"Index object null for {indexFileName}");
                Indexes += indexGroup;
            }
        }



        [Test, Order(4)]
        public void TestIndexCreation()
        {
            string baseTubFilesPath = GetBaseTubPath();
            Index index = new Index(baseTubFilesPath);
            Assert.IsTrue(index.Load(), $"Index not loaded");
        }

        [Test, Order(5)]
        public void TestIndexSearch()
        {
            LuceneIndexSearch lucene = new LuceneIndexSearch(GetBaseTubPath(), 0);
            string baseTubFilesPath = GetBaseTubPath();
            Index index = new Index(baseTubFilesPath);
            Assert.IsTrue(index.Load(), $"Index not loaded");
            List<string> list= index.Search("Cabinet");
            Assert.IsTrue(list.Count > 0, $"Word not found");
        }


    }
}