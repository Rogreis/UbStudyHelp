using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Stores a search result
    /// </summary>
    public class SearchResult
    {
        private TextWork TextWork = new TextWork();


        private const string highStart = "|~S~|";

        private const string highEnd = "|~E~|";

        public TOC_Entry Entry { get; set; }

        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                TextWork.LoadText(value);
                _text = TextWork.GetHtml();
            }
        }

        public int OriginalPosition { get; set; } = -1;

        public SearchResult(TOC_Entry entry, string text)
        {
            Entry = entry;
            Text = text;
        }

        /// <summary>
        /// Get text start
        /// </summary>
        private string TextStart
        {
            get
            {
                return TextWork.GetReducedText();
            }
        }



        /// <summary>
        /// Generate inlines collection from result text, highlighting found words
        /// </summary>
        /// <returns></returns>
        public void GetInlinesText(InlineCollection Inlines, List<string> Words, bool useReducedText= false)
        {
            TextWork.GetInlinesText(Inlines, Words);
        }

    }


}
