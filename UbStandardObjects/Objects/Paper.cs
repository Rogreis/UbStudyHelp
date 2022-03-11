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

        internal Paper()
        {
        }

        public Paragraph GetParagraph(TOC_Entry entry)
        {
            return Paragraphs.Find(p => p.Section == entry.Section && p.ParagraphNo == entry.ParagraphNo);
        }

        public override string ToString()
        {
            return $"Paper {PaperNo}";
        }

    }
}
