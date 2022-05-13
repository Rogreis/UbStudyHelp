using System.Collections.Generic;
using System.Text;

namespace UbStandardObjects.Objects
{

    public enum UbAnnotationType
    {
        Paper,
        Paragraph
    }

    /// <summary>
    /// Base class to store annotations for a paper or paragraph
    /// </summary>
    public class UbAnnotationsStoreData
    {
        public TOC_Entry Entry { get; set; }

        public UbAnnotationType AnnotationType { get; set; }

        public List<string> AnnotationsStrings { get; set; } = new List<string>();

        protected void StoreAnnotation(byte[] bytes)
        {
            var bytesAsString = Encoding.UTF8.GetString(bytes);
            AnnotationsStrings.Add(bytesAsString);
        }
    }

    /// <summary>
    /// Used to dynamic store the annotations for both shown papers
    /// </summary>
    public class UbAnnotationsStoreSet
    {
        public UbAnnotationsStoreData PaperLeftAnnotations { get; set; }
        public UbAnnotationsStoreData PaperRightAnnotations { get; set; }
        public UbAnnotationsStoreData ParagraphAnnotations { get; set; }
    }

}
