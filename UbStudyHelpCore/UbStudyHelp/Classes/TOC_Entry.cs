using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace UrantiaBook.Classes
{
    public class TOC_Entry : BaseClass
    {
        public short Paper { get; set; }
        public short Section { get; set; }
        public short ParagraphNo { get; set; }
        public string Text { get; set; }
        public bool IsExpanded { get; set; }

        public TOC_Entry(short paper, short section, short paragraphNo)
        {
            this.Paper = paper;
            this.Section = section;
            this.ParagraphNo = paragraphNo;
            Text = "";
            IsExpanded = false;
        }


        public TOC_Entry(XElement xElemParagraph)
        {
            Paper = GetShort(xElemParagraph.Element("Paper"));
            Section = GetShort(xElemParagraph.Element("Section"));
            this.ParagraphNo = 0;
            Text = GetString(xElemParagraph.Element("Text"));
            IsExpanded = false;
        }

        public TOC_Entry(Location loc)
        {
            Paper = (short)loc.Paper;
            Section = (short)loc.Section;
            ParagraphNo = (short)loc.Paragraph;
            Text = "";
            IsExpanded = false;
        }

        public TOC_Entry(BrowserPosition position)
        {
            Paper = (short)position.Entry.Paper;
            Section = (short)position.Entry.Section;
            this.ParagraphNo = 0;
            Text = "";
            IsExpanded = false;
        }


        private bool SamePaperSection(TOC_Entry index)
        {
            return index.Paper == Paper && index.Section == Section && index.IsExpanded;
        }

        public override string ToString()
        {
            return ParagraphID;
        }

        public string ParagraphID
        {
            get
            {
                return $"{Paper}:{Section}-{ParagraphNo}";
            }
        }

        public string Anchor
        {
            get
            {
                return $"U{Paper}_{Section}_{ParagraphNo}";
            }
        }

        public string Ident
        {
            get
            {
                if (Section == 0)
                    return $"{Paper} - {Text}";
                else if (ParagraphNo == 0)
                    return $"{Text}";
                else
                    return "??";
            }
        }


        public void CheckOldExpanded(List<TOC_Entry> listOldExpanded)
        {
            IsExpanded = listOldExpanded.Exists(SamePaperSection);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TOC_Entry entry = obj as TOC_Entry;
            if (entry.Paper != this.Paper) return false;
            if (entry.Section != Section) return false;
            if (entry.ParagraphNo != ParagraphNo) return false;
            return true;
        }

        public static bool operator ==(TOC_Entry e1, TOC_Entry e2)
        {
            if (System.Object.ReferenceEquals(e1, e2)) return true;
            if ((object)e1 == null || (object)e2 == null) return false;
            return e1.Equals(e2);
        }

        public static bool operator !=(TOC_Entry e1, TOC_Entry e2)
        {
            return !(e1 == e2);
        }

        public override int GetHashCode()
        {
            return Paper * 100000 + Section * 1000 + ParagraphNo;
        }

    }
}
