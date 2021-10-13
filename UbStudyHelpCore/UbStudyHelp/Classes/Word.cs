using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace UrantiaBook.Classes
{
    /// <summary>
    /// class used to store a word that occurs in some translation
    /// </summary>
    public class Word : BaseClass
    {
        public string WordText { get; set; }
        public string Word_NonAccented { get; set; }
        public string Definition { get; set; }
        public List<Location> listLocaltion = new List<Location>();

        public Word(string Word)
        {
            this.WordText = Word;
        }

        public Word(BinaryReader aReader)
        {
            WordText = aReader.ReadString();
            Word_NonAccented = aReader.ReadString();
            bool continues = true;
            while (continues)
            {
                Location oLocaltion = new Location(aReader);
                if (oLocaltion.Paper >= 0)
                    listLocaltion.Add(oLocaltion);
                else continues = false;
            }
        }



        public Word(XElement xElemWord)
        {
            WordText = xElemWord.Element("W").Value;
            foreach (XElement xElemLocation in xElemWord.Descendants("L"))
                listLocaltion.Add(new Location(xElemLocation));
        }
    }
}
