using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// cFlatPosition links a position in the flat search string with a word object to get the locations list
    /// </summary>
    public class FlatPosition
    {
        public short Paper { get; set; }
        public int Seq { get; set; }
        public int Section { get; set; }
        public int ParagraphNo { get; set; }
        public int Position { get; set; }

        public FlatPosition(BinaryReader aReader)
        {
            Paper = aReader.ReadInt16();
            Seq = aReader.ReadInt32();
            Section = aReader.ReadInt32();
            ParagraphNo = aReader.ReadInt32();
            Position = aReader.ReadInt32();
        }

        public override string ToString()
        {
            return $"{Paper}:{Section}-{ParagraphNo}    {Position}";
        }


    }
}
