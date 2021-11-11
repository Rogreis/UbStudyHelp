using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UbStudyHelp.Classes;

namespace UbStudyHelpMSTest
{
    [TestClass]
    public class LuceneUnitTest
    {

        private string baseDataPath = @"..\..\..\..\UbStudyHelp\bin\Debug\netcoreapp3.1\TUB_Files";

        [TestMethod]
        public void TestMethod1()
        {
        }




        // Private methods
        private string GetBaseTubPath()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            //string exePath = TestContext.CurrentContext.TestDirectory;
            Assert.IsTrue(!string.IsNullOrEmpty(exePath), "Missing current context test directory");
            return System.IO.Path.Combine(exePath, "TUB_Files");
        }





        public void Setup()
        {
        }

        [TestMethod]
        public void TestTubFolderExists()
        {
            string baseTubFilesPath = GetBaseTubPath();
            Assert.IsTrue(Directory.Exists(baseTubFilesPath), "Missing TUB files directory");
        }

        [TestMethod]
        public void TestIndexFilesExist()
        {
            string baseTubFilesPath = GetBaseTubPath();
            UbStudyHelp.Classes.Index index = new UbStudyHelp.Classes.Index(baseTubFilesPath);
            foreach (string indexFileName in index.FileSufixes)
            {
                string indexFilePath = Path.Combine(baseTubFilesPath, indexFileName + ".json");
                Assert.IsTrue(File.Exists(indexFilePath), $"Missing Index file {indexFileName}");
            }
        }

        [TestMethod]
        public void TestIndexLoad()
        {
            string baseTubFilesPath = GetBaseTubPath();
            UbStudyHelp.Classes.Index index = new UbStudyHelp.Classes.Index(baseTubFilesPath);
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



        [TestMethod]
        public void TestIndexCreation()
        {
            string baseTubFilesPath = GetBaseTubPath();
            UbStudyHelp.Classes.Index index = new UbStudyHelp.Classes.Index(baseTubFilesPath);
            Assert.IsTrue(index.Load(), $"Index not loaded");
        }

        [TestMethod]
        public void TestIndexSearch()
        {
            LuceneIndexSearch lucene = new LuceneIndexSearch(GetBaseTubPath(), 0);
            string baseTubFilesPath = GetBaseTubPath();
            UbStudyHelp.Classes.Index index = new UbStudyHelp.Classes.Index(baseTubFilesPath);
            Assert.IsTrue(index.Load(), $"Index not loaded");
            List<string> list = index.Search("Cabinet");
            Assert.IsTrue(list.Count > 0, $"Word not found");
        }

        [TestMethod]
        public void TestBookSearch()
        {
            Translation translation034 = new Translation();
            string pathTranslation = Path.Combine(baseDataPath, "L034");
            translation034.Inicialize(pathTranslation);
            LuceneBookSearch lucene = new LuceneBookSearch(GetBaseTubPath(), translation034);
            string baseTubFilesPath = GetBaseTubPath();
            UbStudyHelp.Classes.Index index = new UbStudyHelp.Classes.Index(baseTubFilesPath);
            Assert.IsTrue(index.Load(), $"Index not loaded");
            List<string> list = index.Search("Cabinet");
            Assert.IsTrue(list.Count > 0, $"Word not found");
        }



    }


}
