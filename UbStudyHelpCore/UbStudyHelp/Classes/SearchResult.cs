using System.Collections.Generic;
using System.Windows.Documents;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Stores a search result
    /// </summary>
    public class SearchResult
    {
        private TextWork TextWork = new TextWork();

        private string _text = "";

        public TOC_Entry Entry { get; set; }

        public int OriginalPosition { get; set; } = -1;

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


        public SearchResult(TOC_Entry entry, string text)
        {
            Entry = entry;
            Text = text;
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
