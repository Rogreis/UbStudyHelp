using System;
using System.Collections.Generic;
using System.Text;

namespace UbStandardObjects.Objects
{
    /// <summary>
    /// Implements the indes information
    /// </summary>
    public class BookIndex
    {
        public short PaperNo { get; set; } = 0;
        public short Section { get; set; } = 0;
        public short ParagraphNo { get; set; } = 0;
        public string Text { get; set; } = "";

        public string Href 
        { 
            get 
            {
                return $"Doc{PaperNo:000}.html";
            }
        }

        public List<BookIndex> SubItems = new List<BookIndex>();

        public BookIndex()
        {

        }

    }

}
