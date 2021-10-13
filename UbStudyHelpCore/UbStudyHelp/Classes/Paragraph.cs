using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;

namespace UrantiaBook.Classes
{
    public class Paragraph : BaseClass
    {
        public short Paper { get; set; }
        public short PK_Seq { get; set; }
        public short Section { get; set; }
        public short ParagraphNo { get; set; }
        public short Page { get; set; }
        public short Line { get; set; }
        public string Text { get; set; }
        public enHtmlType Format { get; set; }

        public TOC_Entry Entry { get; private set; }


        public Paragraph(XElement xElemParagraph)
        {
            Paper = GetShort(xElemParagraph.Element("Paper"));
            PK_Seq = GetShort(xElemParagraph.Element("PK_Seq"));
            Section = GetShort(xElemParagraph.Element("Section"));
            ParagraphNo = GetShort(xElemParagraph.Element("ParagraphNo"));
            Page = GetShort(xElemParagraph.Element("Page"));
            Line = GetShort(xElemParagraph.Element("Line"));
            Text = GetString(xElemParagraph.Element("Text"));
            Format = (enHtmlType)GetShort(xElemParagraph.Element("Format"));
            Entry = new TOC_Entry(Paper, Section, ParagraphNo);
        }

        public string NoHtml
        {
            get
            {
                return System.Text.RegularExpressions.Regex.Replace(Text, @"<[^>]*>", string.Empty,
                                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
        }


        public string TextForSearchEngine
        {
            get
            {
                string result = System.Text.RegularExpressions.Regex.Replace(Text, @"<[^>]*>", string.Empty,
                                  System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                return ToSimpleCharacters(result);
            }
        }

        public string HtmlTextForSearchEngine
        {
            get
            {
                return ToSimpleCharacters(Text);
            }
        }

        public string Identification
        {
            get
            {
                return string.Format("{0}:{1}-{2} ({3}.{4})", Paper, Section, ParagraphNo, Page, Line); ;
            }
        }

        public string AName
        {
            get
            {
                return string.Format("U{0}_{1}_{2}", Paper, Section, ParagraphNo); ;
            }
        }


        public override string ToString()
        {
            string partText= "";
            if (string.IsNullOrEmpty(Text))
            {
                partText = "";
            }
            else if (ParagraphNo == 0)
            {
                partText = Text;
            }
            else
            {
                int position = Math.Min(40, Text.Length);
                position = Text.IndexOf(' ', position);
                if (position >= 0)
                    partText = Text.Substring(0, position);
            }
            return $"{Identification} {partText}";
        }

    }
}
