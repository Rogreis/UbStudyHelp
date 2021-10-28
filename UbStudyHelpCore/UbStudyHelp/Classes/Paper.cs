using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UbStudyHelp;

namespace UbStudyHelp.Classes
{
    public enum enHtmlType
    {
        BookTitle = 0,
        PaperTitle = 1,
        SectionTitle = 2,
        NormalParagraph = 3,
        IdentedParagraph = 4
    }


    public class Paper
    {
        private short PaperNo = -1;

        public List<Paragraph> Paragraphs = null;

        public string Title { get; private set; }


        public Paper(string pathData, Translation Translation, short PaperNo)
        {
            this.PaperNo = PaperNo;
            string BasePathPaper = Path.Combine(pathData, "L" + Translation.LanguageID.ToString("000"));
            string PathXmlFile = Path.Combine(BasePathPaper, "Paper" + PaperNo.ToString("000") + ".xml");

            XElement xElem = XElement.Load(PathXmlFile);

            Paragraphs = new List<Paragraph>();

            foreach (XElement xElemPar in xElem.Descendants("Paragraph"))
            {
                Paragraphs.Add(new Paragraph(xElemPar));
            }

            Paragraph parTitle = Paragraphs.Find(p => p.Section == 0 && p.ParagraphNo == 0);
            Title = parTitle != null? parTitle.Text : "No Title, check your data";

        }

    }
}
