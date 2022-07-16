using System.Collections.Generic;
using System.Text;

namespace UbStandardObjects.Objects
{

    public enum UbAnnotationType
    {
        Paper = 0,
        Paragraph = 1
    }

    /// <summary>
    /// Class used to store an anootation with its id in clear text
    /// </summary>
    public class UbAnnotationInfo
    {
        public string Id { get; set; } = "";
        public string AnnotationsString { get; set; } = "";
    }

    /// <summary>
    /// Base class to store annotations for a paper or paragraph
    /// </summary>
    public abstract class UbAnnotationsStoreData
    {

        public string Title { get; set; } = "";

        public TOC_Entry Entry { get; set; }

        public UbAnnotationType AnnotationType { get; set; }

        public List<UbAnnotationInfo> AnnotationsInfo { get; set; } = new List<UbAnnotationInfo>();

        public string XamlNotes { get; set; } = "";

        public UbAnnotationsStoreData()
        {

        }

        protected void StoreAnnotationBytes(string id, byte[] bytes)
        {
            var bytesAsString = Encoding.UTF8.GetString(bytes);
            UbAnnotationInfo info = new UbAnnotationInfo
            {
                Id = id,
                AnnotationsString = bytesAsString
            };
            AnnotationsInfo.Add(info);
        }

        protected void RemoveAnnotation(string id)
        {
            AnnotationsInfo.Remove(AnnotationsInfo.Find(a => a.Id == id));
        }

        public abstract void StoreAnnotation(object annotation);

    }
}

