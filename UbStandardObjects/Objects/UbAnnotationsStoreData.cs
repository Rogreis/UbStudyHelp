using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace UbStandardObjects.Objects
{

    public enum UbAnnotationType
    {
        Paper,
        Paragraph
    }


    public class UbAnnotationsStoreData
    {
        public TOC_Entry Entry { get; set; }

        public UbAnnotationType AnnotationType { get; set; }

        public List<string> AnnotationsStrings { get; set; } = new List<string>();

        public List<byte[]> GetAnnotationsBytes()
        {
            List<byte[]> annotationsBytes = new List<byte[]>();
            foreach (var annotation in this.AnnotationsStrings)
            {
                annotationsBytes.Add(Encoding.UTF8.GetBytes(annotation));
            }
            return annotationsBytes;
        }

        public void StoreAnnotation(byte[] bytes)
        {
            var bytesAsString = Encoding.UTF8.GetString(bytes);
            AnnotationsStrings.Add(bytesAsString);
        }
    }
}
