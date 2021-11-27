using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;

namespace UbStudyHelp.Classes
{
    public class Paragraph : BaseClass
    {

        private TextWork TextWork = new TextWork();

        public short Paper { get; set; }
        public short PK_Seq { get; set; }
        public short Section { get; set; }
        public short ParagraphNo { get; set; }
        public short Page { get; set; }
        public short Line { get; set; }

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
                return TextWork.GetPlainText();
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


        public string ReducedText
        {
            get
            {
                return TextWork.GetReducedText();
            }
        }


        public override string ToString()
        {
            string partText= "";
            if (string.IsNullOrEmpty(Text))
            {
                partText = "";
            }
            else 
            {
                partText = TextWork.GetReducedText();
            }
            return $"{Identification} {partText}";
        }

    }
}
