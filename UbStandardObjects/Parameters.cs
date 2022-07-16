using System.Collections.Generic;
using System.Drawing;
using UbStandardObjects.Objects;

namespace UbStandardObjects
{
	public struct ColorSerial
    {
		public byte A { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }

		public ColorSerial(byte a, byte r, byte g, byte b)
        {
			A = a;
			R = r;
			G = g;	
			B = b;	
        }
	}

	public abstract class Parameters
	{

		/// <summary>
		/// Last position in the text, default for first paragraph
		/// </summary>
		public TOC_Entry Entry { get; set; } = new TOC_Entry(0, 0, 1, 0, 0, 0);

		public short LanguageIDLeftTranslation { get; set; } = 0;

		public short LanguageIDRightTranslation { get; set; } = 34;

		public int SearchPageSize { get; set; } = 20;

		public bool ShowParagraphIdentification { get; set; } = true;

		public bool ShowBilingual { get; set; } = true;

		/// <summary>
		/// Max items stored for  search and index text
		/// </summary>
		public int MaxExpressionsStored { get; set; } = 50;

		public List<string> SearchStrings { get; set; } = new List<string>();

		public List<string> IndexLetters { get; set; } = new List<string>();

		public bool SimpleSearchIncludePartI { get; set; } = true;

		public bool SimpleSearchIncludePartII { get; set; } = true;

		public bool SimpleSearchIncludePartIII { get; set; } = true;

		public bool SimpleSearchIncludePartIV { get; set; } = true;

		public bool SimpleSearchCurrentPaperOnly { get; set; } = false;

		public double SpliterDistance { get; set; } = 550;  // BUG: Default value needs to be proportional to user screen resolution

		public List<string> SearchIndexEntries { get; set; } = new List<string>();

		public List<TOC_Entry> TrackEntries { get; set; } = new List<TOC_Entry>();

		public string LastTrackFileSaved { get; set; } = "";


		/// <summary>
		/// Git associated repository folder
		/// </summary>
		public string RepositoryOutputFolder { get; set; } = "";

		public string RepositoryOutputPTAlternativeFolder { get; set; } = "";

		public string InputHtmlFilesPath { get; set; } = "";

		public string IndexDownloadedFiles { get; set; } = "";

		public string IndexOutputFilesPath { get; set; } = "";

		public string SqlServerConnectionString { get; set; }

		public string FontFamilyInfo { get; set; } = "Verdana";

		public virtual double FontSizeInfo { get; set; } = 10;

		public virtual ColorSerial HighlightColor { get; set; } = new ColorSerial(0, 0, 102, 255); // rgb(0, 102, 255)

		// Quick search
		public string SearchFor { get; set; } = "";

		public string SimilarSearchFor { get; set; } = "";

		public string CloseSearchDistance { get; set; } = "5";

		public string CloseSearchFirstWord { get; set; } = "";

		public string CloseSearchSecondWord { get; set; } = "";

		public List<string> CloseSearchWords { get; set; } = new List<string>();

		public short CurrentTranslation { get; set; } = 0;

		public double AnnotationWindowWidth { get; set; } = 800;

		public double AnnotationWindowHeight { get; set; } = 450;

	}
}
