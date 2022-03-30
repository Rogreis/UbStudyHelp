using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace UbStandardObjects.Objects
{
	public class TOC_Entry
    {
        private const int MaxSampleTextSize = 80;

        public short Paper { get; set; } = 0;
        public short Section { get; set; } = 1;
        public short ParagraphNo { get; set; } = 1;
        public short Page { get; set; }
        public short Line { get; set; }
        public string Text { get; set; } = "";
        public bool IsExpanded { get; set; } = false;

        [JsonIgnore]
        public string ParagraphID
        {
            get
            {
                return $"{Paper}:{Section}-{ParagraphNo} ({Page}.{Line})";
            }
        }

        [JsonIgnore]
        public string ParagraphIDNoPage
        {
            get
            {
                return $"{Paper}:{Section}-{ParagraphNo}";
            }
        }


        [JsonIgnore]
        public string Anchor
        {
            get
            {
                return $"U{Paper}_{Section}_{ParagraphNo}";
            }
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public string Href
        {
            get
            {
                return $"{Paper};{Section};{ParagraphNo}";
            }
        }


        public TOC_Entry()
        {
        }

        public TOC_Entry(Paragraph par)
        {
            this.Paper = par.Paper;
            this.Section = par.Section;
            this.ParagraphNo = par.ParagraphNo;
            this.Page = par.Page;
            this.Line = par.Line;
            Text = par.Text;
            IsExpanded = false;
        }

        public TOC_Entry(short paper, short section, short paragraphNo, short page, short line)
        {
            this.Paper = paper;
            this.Section = section;
            this.ParagraphNo = paragraphNo;
            this.Page = page;
            this.Line = line;
            Text = "";
            IsExpanded = false;
        }

        public TOC_Entry(TOC_Entry entry)
        {
            this.Paper = entry.Paper;
            this.Section = entry.Section;
            this.ParagraphNo = entry.ParagraphNo;
            this.Page = entry.Page;
            this.Line = entry.Line;
            Text = entry.Text;
            IsExpanded = entry.IsExpanded;
        }



        protected bool SamePaperSection(TOC_Entry index)
        {
            return index.Paper == Paper && index.Section == Section && index.IsExpanded;
        }


        public void CheckOldExpanded(List<TOC_Entry> listOldExpanded)
        {
            IsExpanded = listOldExpanded.Exists(SamePaperSection);
        }


        #region Operators
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TOC_Entry entry = obj as TOC_Entry;
            if (entry == null) return false;
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

        public static bool operator <(TOC_Entry e1, TOC_Entry e2)
        {
            if (System.Object.ReferenceEquals(e1, e2)) return false;
            if ((object)e1 == null || (object)e2 == null) return false;

            if (e1.Paper < e2.Paper) return true;
            if (e1.Paper > e2.Paper) return false;

            if (e1.Section < e2.Section) return true;
            if (e1.Section > e2.Section) return false;

            if (e1.ParagraphNo < e2.ParagraphNo) return true;
            return false;
        }

        public static bool operator >(TOC_Entry e1, TOC_Entry e2)
        {
            if (System.Object.ReferenceEquals(e1, e2)) return false;
            if ((object)e1 == null || (object)e2 == null) return false;

            if (e1.Paper > e2.Paper) return true;
            if (e1.Paper < e2.Paper) return false;

            if (e1.Section > e2.Section) return true;
            if (e1.Section < e2.Section) return false;

            if (e1.ParagraphNo > e2.ParagraphNo) return true;
            return false;
        }

        public int CompareTo(TOC_Entry entry)
        {
            if (this == entry)
            {
                return 0;
            }
            if (this < entry)
            {
                return -1;
            }
            return 1;
        }

        public int InverseCompareTo(TOC_Entry entry)
        {
            if (this == entry)
            {
                return 0;
            }
            if (this < entry)
            {
                return 1;
            }
            return -1;
        }


        public override int GetHashCode()
        {
            return Paper * 100000 + Section * 1000 + ParagraphNo;
        }
        #endregion

        public override string ToString()
        {
            return $"{ParagraphID} {Text}";
        }


    }
}
