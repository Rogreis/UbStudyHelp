using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UbStandardObjects;

namespace UbStudyHelp.Classes
{
    public class LuceneIndexSearch
    {
        private const string IndexFieldName = "Entry";
        private const string IndexFieldData = "Data";

        public string ErrorMessage { get; private set; } = "";

        public string IndexPath { get; private set; } = "";

        public LuceneIndexSearch(string basePathForFiles, short translationID)
        {
            string pathFile = System.IO.Path.Combine(basePathForFiles, "L" + translationID.ToString("000"));
            IndexPath = System.IO.Path.Combine(pathFile, "in");
            if (!System.IO.Directory.Exists(IndexPath))
            {
                System.IO.Directory.CreateDirectory(IndexPath);
            }
        }

        public void Dispose()
        {
        }


        public bool CreateLuceneIndexForUBIndex(List<TubIndex> Indexes)
        {
            try
            {
                if (System.IO.Directory.GetFiles(IndexPath, "*.*").Count() > 0)
                {
                    return true;
                }
                
                Lucene.Net.Store.Directory luceneIndexDirectory= FSDirectory.Open(IndexPath);
                Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                IndexWriter writer = new IndexWriter(luceneIndexDirectory, config);

                foreach (TubIndex index in Indexes)
                {
                    Document doc = new Document();
                    // StringField indexes but doesn't tokenize
                    doc.Add(new StringField(IndexFieldName, index.Title, Field.Store.YES));
                    doc.Add(new TextField(IndexFieldData, index.Title, Field.Store.YES));
                    writer.AddDocument(doc);
                }
                writer.Flush(triggerMerge: false, applyAllDeletes: false);
                //writer.Flush(true, true);
                //writer.PrepareCommit();
                //writer.Commit();
                writer.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("Error creating Search Index for UB Index.", ex);
                EventsControl.FireSendMessage("Creating Search Index for UB Index", ex);
                return false;
            }
        }


        public List<string> Execute(string query)
        {
            try
            {
                Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

                // How to query
                // http://lucenenet.apache.org/
                // https://lucene.apache.org/core/2_9_4/queryparsersyntax.html

                var reader = DirectoryReader.Open(FSDirectory.Open(IndexPath));
                var parser = new QueryParser(LuceneVersion.LUCENE_48, IndexFieldData, analyzer);
                Query searchQuery = parser.Parse(query);

                List<string> resultsList = new List<string>();


                IndexSearcher searcher = new IndexSearcher(reader);
                TopDocs hits = searcher.Search(searchQuery, 20);

                // Nothing found? Try to expand (emulate starting with)
                if (hits.ScoreDocs.Length == 0)
                {
                    query = query.Trim() + "*";
                    searchQuery = parser.Parse(query);
                    hits = searcher.Search(searchQuery, 20);
                }

                int results = hits.ScoreDocs.Length;
                for (int i = 0; i < results; i++)
                {
                    Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                    resultsList.Add(doc.GetField(IndexFieldData).GetStringValue());
                }
                return resultsList;
            }
            catch (Exception ex)
            {
                StaticObjects.Logger.Error("Error executing Index Search.", ex);
                EventsControl.FireSendMessage("Executing Index Search", ex);
                ErrorMessage = ex.Message;
                return null;
            }
        }

    }
}
