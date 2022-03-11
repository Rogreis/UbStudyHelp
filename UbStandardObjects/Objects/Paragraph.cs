using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using System.Text.Json.Serialization;

namespace UbStandardObjects.Objects
{
    public class Paragraph
    {
        public short Paper { get; set; }
        public short PK_Seq { get; set; }
        public short Section { get; set; }
        public short ParagraphNo { get; set; }
        public short Page { get; set; }
        public short Line { get; set; }
        public virtual string Text { get; set; } = "";
        public int FormatInt { get; set; }
        public enHtmlType Format
        {
            get
            {
                return (enHtmlType)FormatInt;
            }
        }

        [JsonIgnore]
        public TOC_Entry Entry 
        { 
            get
            {
                TOC_Entry entry= new TOC_Entry();
                entry.Paper = Paper;
                entry.Section= Section;
                entry.ParagraphNo = ParagraphNo;
                entry.Text = Text;
                return entry;
            }
        }


        [JsonIgnore]
        public string Identification
        {
            get
            {
                return string.Format("{0}:{1}-{2} ({3}.{4})", Paper, Section, ParagraphNo, Page, Line); ;
            }
        }

        [JsonIgnore]
        public string AName
        {
            get
            {
                return string.Format("U{0}_{1}_{2}", Paper, Section, ParagraphNo); ;
            }
        }

        public override string ToString()
        {
            string partText = Text;
            if (string.IsNullOrEmpty(Text))
            {
                partText = "";
            }
            return $"{Identification} {partText}";
        }


    }
}
