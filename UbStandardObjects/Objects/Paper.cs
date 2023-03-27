using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UbStandardObjects.Objects
{

    /// <summary>
    /// Used only to convert from json
    /// </summary>
    internal class FullPaper
    {
        public Paragraph[] Paragraphs { get; set; }
    }

    public class Paper
    {
        [JsonIgnore]
        public short PaperNo
        {
            get
            {
                if (Paragraphs.Count > 0)
                {
                    return Paragraphs[0].Paper;
                }
                else
                {
                    return -1;
                }
            }
        }

        [JsonIgnore]
        public string Title
        {
            get
            {
                if (Paragraphs.Count > 0)
                {
                    return Paragraphs[0].Text;
                }
                else
                {
                    return "";
                }
            }
        }


        public List<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();

        public TOC_Entry Entry
        {
            get
            {
                return new TOC_Entry(Paragraphs[0]);
            }
        }

        /// <summary>
        /// Constructor parameterless
        /// </summary>
        public Paper()
        {

        }

        /// <summary>
        /// Constructor from a json string 
        /// </summary>
        /// <param name="jsonString"></param>
        public Paper(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            var fullPaper = JsonSerializer.Deserialize<FullPaper>(jsonString, options);
            Paragraphs.AddRange(fullPaper.Paragraphs);
            fullPaper = null;
        }


        public virtual Paragraph GetParagraph(TOC_Entry entry)
        {
            return Paragraphs.Find(p => p.Section == entry.Section && p.ParagraphNo == entry.ParagraphNo);
        }

        public void GetNotes()
        {

        }

        public override string ToString()
        {
            return $"Paper {PaperNo}";
        }

    }
}
