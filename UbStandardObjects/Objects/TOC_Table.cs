using System;
using System.Collections.Generic;
using System.Text;

namespace UbStandardObjects.Objects
{
    public class TOC_Table
    {
        public string Title { get; set; } = "";

        public List<TOC_Entry> Parts { get; set; } = new List<TOC_Entry>();

    }
}
