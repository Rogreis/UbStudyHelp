using System;
using System.Collections.Generic;
using System.Xml.Linq;
using YamlDotNet.Serialization;

namespace UbStudyHelp.Classes
{
    public class TOC_Entry : BaseClass
    {
        private const int MaxSampleTextSize = 80;

        public short Paper { get; set; } = 0;
        public short Section { get; set; } = 1;
        public short ParagraphNo { get; set; } = 1;
        public string Text { get; set; } = "";
        public bool IsExpanded { get; set; } = false;

        [YamlIgnore]
        public string ParagraphID
        {
            get
            {
                return $"{Paper}:{Section}-{ParagraphNo}";
            }
        }

        [YamlIgnore]
        public string Anchor
        {
            get
            {
                return $"U{Paper}_{Section}_{ParagraphNo}";
            }
        }

        [YamlIgnore]
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

        [YamlIgnore]
        public string Href
        {
            get
            {
                return $"{Paper};{Section};{ParagraphNo}";
            }
        }


        [YamlIgnore]
        public string TextSample
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    return "";
                }
                if (Text.Length < MaxSampleTextSize)
                {
                    return Text;
                }
                // Return MaxSampleTextSize or until first not letter/number character
                int position = Math.Min(MaxSampleTextSize, Text.Length);
                position = Text.IndexOf(' ', position);
                if (position >= 0)
                {
                    return Text.Substring(0, position);
                }
                return Text.Substring(0, MaxSampleTextSize);
            }
        }


        public TOC_Entry()
        {
        }

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



        private bool SamePaperSection(TOC_Entry index)
        {
            return index.Paper == Paper && index.Section == Section && index.IsExpanded;
        }

        public override string ToString()
        {
            return $"{ParagraphID} {TextSample}";
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

        public static bool operator <(TOC_Entry e1, TOC_Entry e2)
        {
            if (System.Object.ReferenceEquals(e1, e2)) return false;
            if ((object)e1 == null || (object)e2 == null) return false;
            if (e1.Paper >= e2.Paper) return false;
            if (e1.Section >= e2.Section) return false;
            if (e1.ParagraphNo >= e2.ParagraphNo) return false;
            return true;
        }

        public static bool operator >(TOC_Entry e1, TOC_Entry e2)
        {
            if (System.Object.ReferenceEquals(e1, e2)) return false;
            if ((object)e1 == null || (object)e2 == null) return false;
            if (e1.Paper <= e2.Paper) return false;
            if (e1.Section <= e2.Section) return false;
            if (e1.ParagraphNo <= e2.ParagraphNo) return false;
            return true;
        }


        public override int GetHashCode()
        {
            return Paper * 100000 + Section * 1000 + ParagraphNo;
        }

    }
}
