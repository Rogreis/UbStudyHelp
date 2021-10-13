using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace UrantiaBook.Classes
{
    /// <summary>
    /// class used to store the index location of some word
    /// </summary>
    public class Location
    {
        public int Paper { get; set; }
        public int Seq { get; set; }
        public short Section { get; set; }
        public short Paragraph { get; set; }

        public Location(short Paper, short Seq, short Section, short Paragraph)
        {
            this.Paper = Paper;
            this.Seq = Seq;
            this.Section = Section;
            this.Paragraph = Paragraph;
        }


        public Location(XElement xElemLocation)
        {
            Paper = Convert.ToInt32(xElemLocation.Element("P").Value);
            Seq = Convert.ToInt32(xElemLocation.Element("S").Value);
            Section = Convert.ToInt16(xElemLocation.Element("Sc").Value);
            Paragraph = Convert.ToInt16(xElemLocation.Element("Pr").Value);
        }

        public Location(BinaryReader aReader)
        {
            Paper = aReader.ReadInt16();
            if (Paper > -1)
            {
                Seq = aReader.ReadInt16();
                Section = aReader.ReadInt16();
                Paragraph = aReader.ReadInt16();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Location oLocation = obj as Location;
            if (oLocation.Paper != Paper) return false;
            if (oLocation.Seq != Seq) return false;
            if (oLocation.Section != Section) return false;
            if (oLocation.Paragraph != Paragraph) return false;
            return true;
        }

        public static bool operator ==(Location l1, Location l2)
        {
            if (System.Object.ReferenceEquals(l1, l2)) return true;
            if ((object)l1 == null || (object)l2 == null) return false;
            return l1.Equals(l2);
        }

        public static bool operator <(Location l1, Location l2)
        {
            if (System.Object.ReferenceEquals(l1, l2)) return false;
            if ((object)l1 == null || (object)l2 == null) return false;
            if (l1.Paper >= l2.Paper) return false;
            if (l1.Seq >= l2.Seq) return false;
            return true;
        }

        public static bool operator >(Location l1, Location l2)
        {
            if (System.Object.ReferenceEquals(l1, l2)) return false;
            if ((object)l1 == null || (object)l2 == null) return false;
            if (l1.Paper <= l2.Paper) return false;
            if (l1.Seq <= l2.Seq) return false;
            return true;
        }



        public static bool operator !=(Location l1, Location l2)
        {
            return !(l1 == l2);
        }


        public override string ToString()
        {
            return Paper.ToString() + ":" + Section.ToString() + "-" + Paragraph.ToString();
        }


        public string Href
        {
            get
            {
                return Paper.ToString() + ";" + Seq.ToString() + ";" + Section.ToString() + ";" + Paragraph.ToString();
            }
        }

        public override int GetHashCode()
        {
            return Paper * 100000 + Section * 1000 + Paragraph;
        }


    }
}
