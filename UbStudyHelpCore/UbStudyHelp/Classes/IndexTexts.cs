using System;
using System.Collections.Generic;
using System.Text;

namespace UbStudyHelp.Classes
{

    public class IndexDetails
    {
        public string Text { get; set; }

        public string Details { get; set; }

        public List<string> References { get; set; } = new List<string>();

        public override string ToString()
        {
            return Text;
        }
    }



    public class IndexTexts
    {
        public List<IndexDetails> Details { get; set; } = new List<IndexDetails>();

        public static IndexTexts operator +(IndexTexts index1, IndexTexts index2)
        {
            index1.Details.AddRange(index2.Details);
            return index1;
        }

    }

}
