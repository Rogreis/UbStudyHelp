using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using UbStudyHelp;

namespace UrantiaBook.Classes
{
    public class Parameters
    {
        public short lastPaper = 0;
        public bool ShowBilingual = false;
        public short LanguageIDLeftTranslation = 0;
        public short LanguageIDRightTranslation = 34;

        public int MaxTrackItems = 30;

        // Generic search parameters  
        public int SearchPageSize = 20;

        // Simple Search parameters
        public int MaxExpressionsStored = 20;
        public List<string> listSearchString = new List<string>();
        public bool SimpleSearchIncludePartI = true;
        public bool SimpleSearchIncludePartII = true;
        public bool SimpleSearchIncludePartIII = true;
        public bool SimpleSearchIncludePartIV = true;

        public string IndexLetters = "";


        public bool IsPaperIncluded(int PaperNo) => ((App.objParameters.SimpleSearchIncludePartI && PaperNo < 32) ||
                                                    (App.objParameters.SimpleSearchIncludePartII && PaperNo >= 32 && PaperNo <= 56) ||
                                                    (App.objParameters.SimpleSearchIncludePartIII && PaperNo >= 57 && PaperNo <= 119) ||
                                                    (App.objParameters.SimpleSearchIncludePartIV && PaperNo >= 120));



        public int SpliterDistance = 360;

        public struct SerializableColor
        {
            public int A;
            public int R;
            public int G;
            public int B;

            public SerializableColor(Color color)
            {
                this.A = color.A;
                this.R = color.R;
                this.G = color.G;
                this.B = color.B;
            }

            public static SerializableColor FromColor(Color color)
            {
                return new SerializableColor(color);
            }

            public Color ToColor()
            {
                return Color.FromArgb(A, R, G, B);
            }
        }

        // Font for texts
        public string FontFamily = " Verdana";

        public SerializableColor mFontColorProperty;

        [XmlElement("ColorProperty")]
        private SerializableColor XmlColorProperty
        {
            get { return mFontColorProperty; }
            set { mFontColorProperty = value; }
        }

        [XmlIgnore()]
        public Color FontColor
        {
            get { return mFontColorProperty.ToColor(); }
            set { mFontColorProperty = SerializableColor.FromColor(value); }
        }

        public Color FontColorWebTitles { get; set; } = Color.Blue; 


        public bool FontBold = false;
        public bool FontItalic = false;
        public bool FontUnderline = false;
        public float FontSize = 10;

        //public void xx()
        //{
        //    style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, this.FindResource("NameOfResource")));
        //    FontFamily f = new FontFamily("Consolas"); // the Media namespace

        //}

        //public Font fontForWindowsForms
        //{
        //    get
        //    {
        //        FontFamily f = new FontFamily("Consolas"); // the Media namespace
        //        FontStyle fs = new FontStyle();
        //        fs = FontStyle.Regular;
        //        if (Program.objParameters.FontBold) fs = fs ^ FontStyle.Bold;
        //        if (Program.objParameters.FontItalic) fs = fs ^ FontStyle.Italic;
        //        if (Program.objParameters.FontUnderline) fs = fs ^ FontStyle.Underline;
        //        return new Font(Program.objParameters.FontFamily, Program.objParameters.FontSize, fs);
        //    }
        //}


        //public List<cIndex> listIndex = new List<cIndex>(197);

        public string FontColorForWeb
        {
            get
            {
                return System.Drawing.ColorTranslator.ToHtml(FontColor);
            }
        }

        public string FontColorForWebTitles
        {
            get
            {
                // Color fixed  October-2021
                //return System.Drawing.ColorTranslator.ToHtml(FontColorWebTitles);
                return " #0066cc";
            }
        }

    }
}
