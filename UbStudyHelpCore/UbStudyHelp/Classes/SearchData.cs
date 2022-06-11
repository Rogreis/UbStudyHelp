using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Used to send/receive data to/from the book search engine
    /// </summary>
    public class SearchData
    {
        private bool OrderedByParagraphs = false;

        public bool Part1Included { get; set; } = false;

        public bool Part2Included { get; set; } = false;

        public bool Part3Included { get; set; } = false;

        public bool Part4Included { get; set; } = false;

        public bool CurrentPaperOnly { get; set; } = false;

        public string QueryString { get; set; } = "";

        public short CurrentPaper { get; set; } = -1;



        // Output data

        public List<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

        public List<string> Words { get; set; } = new List<string>();

        public bool IsPaperIncluded(int PaperNo) => (
                    (Part1Included && PaperNo < 32) ||
                    (Part2Included && PaperNo >= 32 && PaperNo <= 56) ||
                    (Part3Included && PaperNo >= 57 && PaperNo <= 119) ||
                    (Part4Included && PaperNo >= 120));


        ///// <summary>
        ///// Create the word list to be used when highlighting search text found
        ///// </summary>
        ///// <param name="textPrefix"></param>
        ///// <param name="searchString"></param>
        //public void SetSearchString(string textPrefix, string searchString)
        //{
        //    bool continues = true;
        //    int lastStartPos = 0;

        //    searchString = searchString.Replace('~', ' ');
        //    searchString = searchString.Replace('^', ' ');

        //    while (continues)
        //    {
        //        int pos = searchString.IndexOf(textPrefix, lastStartPos);
        //        continues = pos >= 0 && lastStartPos < searchString.Length;
        //        if (continues)
        //        {
        //            int startPos = pos + textPrefix.Length;
        //            char divisor = searchString.ToCharArray()[startPos] == '"' ? '"' : ' ';
        //            if (divisor == '"')
        //            {
        //                startPos += 1;
        //            }
        //            int endPos = searchString.IndexOf(divisor, startPos);
        //            endPos = (endPos >= 0 ? endPos : searchString.Length);
        //            int size = endPos - startPos;
        //            Words.Add(searchString.Substring(startPos, size));
        //            lastStartPos = endPos + 1;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //        continues = lastStartPos < searchString.Length;
        //    }
        //}

        /// <summary>
        /// Create a list of words to highligh in the search
        /// </summary>
        /// <param name="query"></param>
        public void ExtractTerms(Query query)
        {
            var terms = new HashSet<Term>();
            try
            {
                Words.Clear();

                query.ExtractTerms(terms);
                foreach (Term t in terms)
                {
                    Words.Add(t.Text());
                }
            }
            catch 
            {
                // Try to get words when ExtractTerms is not implemented
                string queryString = query.ToString().Replace("?", "").Replace("*", "").Replace("~", "").Replace("(", "").Replace(")", "");
                string[] parts= queryString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Words.AddRange(parts);
            }
        }

        public void SortResults()
        {
            if (SearchResults.Count == 0)
            {
                return;
            }
            if (!OrderedByParagraphs)
            {
                SearchResults.Sort((a, b) => a.Entry.CompareTo(b.Entry));
                OrderedByParagraphs = true;
            }
            else
            {
                SearchResults.Sort((a, b) => a.OriginalPosition.CompareTo(b.OriginalPosition));
                OrderedByParagraphs = false;
            }
        }




    }
}
