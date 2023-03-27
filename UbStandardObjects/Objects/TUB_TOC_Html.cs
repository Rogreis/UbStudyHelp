using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace UbStandardObjects.Objects
{

    /// <summary>
    /// Define a table of contents entry for TUB
    /// </summary>
    public class TUB_TOC_Entry
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";

        public short PaperNo { get; set; } = -1;

        public short SectionNo { get; set; } = -1;

        public short ParagraphNo { get; set; } = -1;

        [JsonPropertyName("expanded")]
        public bool Expanded { get; set; }

        [JsonPropertyName("nodes")]
        public List<TUB_TOC_Entry> Nodes { get; set; } = new List<TUB_TOC_Entry>();

        public override string ToString()
        {
            return Text;
        }
    }

    /// <summary>
    /// Output table of contents using bootstrap 5
    /// <see href="https://www.w3schools.com/howto/howto_js_treeview.asp"/>
    /// </summary>
    public class TUB_TOC_Html
    {
        private string classesForUlElement = "nested";
        private string ExpandableLi = "caret expandable";
        private string NonExpandableLi = "caret";

        private List<TUB_TOC_Entry> TocEntries = null;

        private Parameters Param = null;

        public TUB_TOC_Html(Parameters param, List<TUB_TOC_Entry> toc_entries)
        {
            TocEntries= toc_entries;
            Param = param;
        }

        private string Href(short paperNo)
        {
            return $@"javascript:loadDoc('content/Doc{paperNo:000}.html', null)";
        }

        private string Href(TUB_TOC_Entry entry)
        {
            return $@"javascript:loadDoc('content/Doc{entry.PaperNo:000}.html','p{entry.PaperNo:000}_{entry.SectionNo:000}_000')";
        }

        private void HtmlNodes(StringBuilder sb, List<TUB_TOC_Entry> tocEntries, string ident)
        {
            sb.AppendLine($"{ident}<ul class=\"{classesForUlElement}\"> ");
            foreach (TUB_TOC_Entry entry in tocEntries)
            {
                string classes = entry.SectionNo == 0 ? NonExpandableLi : ExpandableLi; 
                bool hasNodes = entry.Nodes != null && entry.Nodes.Count > 0;
                if (hasNodes)
                {
                    // <li><a class="parClosed" href="javascript:loadDoc('content/Doc000.html','p000_001_000')">I. Deidade e Divindade</a> 
                    sb.AppendLine($"{ident}    <li><span class=\"{classes} \"><a class=\"liIndex\" href=\"{Href(entry.PaperNo)}\">{entry.Text}</a></span> ");
                    HtmlNodes(sb, entry.Nodes, ident + "   ");
                }
                else
                {
                    sb.AppendLine($"{ident}   <li><a class=\"liIndex\" href=\"{Href(entry)}\">{entry.Text}</a> ");
                }
                sb.AppendLine($"{ident}   </li> ");
            }
            sb.AppendLine($"{ident}</ul> ");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        public void JavaScript(StringBuilder sb)
        {

            sb.AppendLine("<script> ");
            sb.AppendLine("  var toggler = document.getElementsByClassName(\"caret\"); ");
            sb.AppendLine("  var expandables = document.getElementsByClassName(\"expandable\"); ");
            sb.AppendLine("  var i; ");
            sb.AppendLine(" ");
            sb.AppendLine("  for (i = 0; i < expandables.length; i++) { ");
            sb.AppendLine("      expandables[i].parentElement.querySelector(\".nested\").classList.toggle(\"active\"); ");
            sb.AppendLine("      expandables[i].classList.toggle(\"caret-down\"); ");
            sb.AppendLine("} ");
            sb.AppendLine("  for (i = 0; i < toggler.length; i++) { ");
            sb.AppendLine("    toggler[i].addEventListener(\"click\", function() { ");
            sb.AppendLine("      this.parentElement.querySelector(\".nested\").classList.toggle(\"active\"); ");
            sb.AppendLine("      this.classList.toggle(\"caret-down\"); ");
            sb.AppendLine("    }); ");
            sb.AppendLine("  } ");
            sb.AppendLine("</script> ");

            //sb.AppendLine("<script> ");
            //sb.AppendLine("  var toggler = document.getElementsByClassName(\"caret\"); ");
            //sb.AppendLine("  var i; ");
            //sb.AppendLine(" ");
            //sb.AppendLine("  for (i = 0; i < toggler.length; i++) { ");
            //sb.AppendLine("      toggler[i].parentElement.querySelector(\".nested\").classList.toggle(\"active\"); ");
            //sb.AppendLine("      toggler[i].classList.toggle(\"caret-down\"); ");
            //sb.AppendLine("} ");
            //sb.AppendLine("  for (i = 0; i < toggler.length; i++) { ");
            //sb.AppendLine("    toggler[i].addEventListener(\"click\", function() { ");
            //sb.AppendLine("      this.parentElement.querySelector(\".nested\").classList.toggle(\"active\"); ");
            //sb.AppendLine("      this.classList.toggle(\"caret-down\"); ");
            //sb.AppendLine("    }); ");
            //sb.AppendLine("  } ");
            //sb.AppendLine("</script> ");


            //sb.AppendLine("<script> ");
            //sb.AppendLine("var toggler = document.getElementsByClassName(\"caret\"); ");
            //sb.AppendLine("var i; ");
            //sb.AppendLine(" ");
            //sb.AppendLine("for (i = 0; i < toggler.length; i++) { ");
            //sb.AppendLine("  toggler[i].addEventListener(\"click\", function() { ");
            //sb.AppendLine("    this.parentElement.querySelector(\".nested\").classList.toggle(\"active\"); ");
            //sb.AppendLine("    this.classList.toggle(\"caret-down\"); ");
            //sb.AppendLine("  }); ");
            //sb.AppendLine("} ");
            //sb.AppendLine("</script> ");
        }

        public void Html(string pathTocTable)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ul id=\"myUL\"> ");
            string ident = "";
            foreach (TUB_TOC_Entry entry in TocEntries)
            {
                // <span class="caret">Beverages</span>
                sb.AppendLine($"{ident}   <li><span class=\"{ExpandableLi} \">{entry.Text}</span> ");
                if (entry.Nodes != null && entry.Nodes.Count > 0)
                {
                    HtmlNodes(sb, entry.Nodes, ident + "   ");
                }
                sb.AppendLine($"{ident}   </li> ");
            }
           
            sb.AppendLine("</ul> ");
            sb.AppendLine(" ");
            string html = sb.ToString();
            File.WriteAllText(pathTocTable, html);
        }
    }

}
