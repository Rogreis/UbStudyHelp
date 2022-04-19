using System;
using System.Collections.Generic;
using System.Text;

namespace UbStandardObjects.Objects
{

    public enum UbAnnotationType
    {
        Paper,
        Paragraph
    }

    public enum UbStoreContentAction
    {
        Insert,
        Delete,
        Modify
    }

    public class UbAnnotationsStoreData
    {
        public short TranslationId { get; set; } = 0;
        public short Paper { get; set; } = 0;
        public short Section { get; set; } = 1;
        public short ParagraphNo { get; set; } = 1;

        public string NoteXml { get; set; }
        public UbStoreContentAction Action { get; set; }
        public UbAnnotationType AnnotationType { get; set; }
    }
}
