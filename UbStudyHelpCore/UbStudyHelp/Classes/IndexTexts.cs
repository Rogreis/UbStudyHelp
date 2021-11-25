using System;
using System.Collections.Generic;
using System.Text;

namespace UbStudyHelp.Classes
{

    public class IndexDetails
    {
        public int DetailType { get; set; }
        public string Text { get; set; } = "";
        public List<string> Links { get; set; } = new List<string>();

    }

    public class TubIndex
    {
        public string Title { get; set; }
        public List<IndexDetails> Details { get; set; } = new List<IndexDetails>();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Title);
            foreach (IndexDetails detail in Details)
            {
                sb.AppendLine($"   {detail.DetailType}  {detail.Text}");
                foreach (string link in detail.Links)
                {
                    sb.AppendLine("      " + link);
                }
            }
            sb.AppendLine("");
            return sb.ToString();
        }
    }


}
