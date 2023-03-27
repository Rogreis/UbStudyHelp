using System.Collections.Generic;
using System.Drawing;
using UbStandardObjects.Objects;

namespace UbStandardObjects
{

    public enum TextShowOption
    {
        RightOnly = 0,
        LeftRight = 1,
        LeftMiddleRight = 2,
        LeftRightCompare = 3,
        LeftMiddleRightCompare = 4
    }


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
        private string darkTheme { get; set; } = "bg-dark text-white";
        private string lightTheme { get; set; } = "bg-light text-black";

        //// From https://getbootstrap.com/docs/5.0/utilities/background/
        //// https://www.w3schools.com/tags/ref_colornames.asp
        //private const string ClassParagraphStarted = "p-3 mb-2 bg-secondary text-white";
        //private const string ClassParagraphWorking = "p-3 mb-2 bg-warning text-dark";
        //private const string ClassParagraphDoubt = "p-3 mb-2 bg-danger text-white";
        //private const string ClassParagraphOk = "p-3 mb-2 bg-primary text-white";

        //private const string ClassParagraphDarkTheme = "p-3 mb-2 bg-dark text-white";
        //private const string ClassParagraphLightTheme = "p-3 mb-2 bg-transparent text-dark";


        /// <summary>
        /// Last position in the text, default for first paragraph
        /// </summary>
        public TOC_Entry Entry { get; set; } = new TOC_Entry(0, 0, 1, 0, 0, 0);

		public short LanguageIDLeftTranslation { get; set; } = 0;

		public short LanguageIDRightTranslation { get; set; } = 34;

        public short LanguageIDMiddleTranslation { get; set; } = -1;  // -1 indicate not to be shown

		public bool ShowCompare { get; set; } = false;
        
		public int SearchPageSize { get; set; } = 20;

		public bool ShowParagraphIdentification { get; set; } = true;

		public bool ShowBilingual { get; set; } = true;

        public TextShowOption TextShowOption { get; set; } = TextShowOption.LeftRight;

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


		public string InputHtmlFilesPath { get; set; } = "";

		public string IndexDownloadedFiles { get; set; } = "";

		public string IndexOutputFilesPath { get; set; } = "";

		public string SqlServerConnectionString { get; set; }

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

	
		/// <summary>
		/// Current data folder
		/// </summary>
		public string ApplicationDataFolder { get; set; } = "";

        /// <summary>
        /// Folder to store local lucene index search data
        /// </summary>
        public string IndexSearchFolders { get; set; } = "";

        /// <summary>
        /// Folder to store local lucene TUB search data
        /// </summary>
        public string TubSearchFolders { get; set; } = "";

        /// <summary>
        /// Repository for translations
        /// </summary>
        public string TUB_Files_RepositoryFolder { get; set; } = "";

        /// <summary>
        /// Github source for translations
        /// </summary>
        public string TUB_Files_Url { get; set; } = "https://github.com/Rogreis/TUB_Files.git";

        /// <summary>
        /// Repository for editing translation
        /// </summary>
        public string EditParagraphsRepositoryFolder { get; set; } = null;

        /// <summary>
        /// Github source for editing translation
        /// </summary>
        public string EditParagraphsUrl { get; set; } = "https://github.com/Rogreis/PtAlternative.git";

        /// <summary>
        /// Branch used for edit data
        /// </summary>
        public string BranchUsed { get; set; } = "correcoes";
        
        /// <summary>
        /// Full book pages local repository
        /// </summary>
        public string EditBookRepositoryFolder { get; set; } = null;

		/// <summary>
		/// Github paragraphs repository
		/// </summary>
		public string UrlRepository { get; set; } = null;

        public float FontSize { get; set; } = 10;

        public string FontFamily { get; set; } = "Georgia,Verdana,Arial,Helvetica";

        public bool IsDarkTheme { get; set; } = true;

        public string DarkText { get; set; } = "black";

        public string LightText { get; set; } = "white";

        public string DarkTextHighlihted { get; set; } = "yellow";

        public string LightTextHighlihted { get; set; } = "blue";

        public string DarkTextGray { get; set; } = "yellow";

        public string LightTextGray { get; set; } = "bisque";

        public string GitAuthorName { get; set; } = "";

        public string GitEmail { get; set; } = "";

        public string GitCommitMessage { get; set; } = "";

        public string GitPassword { get; set; } = "";

        public string BackTextColor
		{
			get
			{
				return IsDarkTheme ? darkTheme : lightTheme;
			}
		}




        /// <summary>
        /// Returns the classes for a paragraph depending on IsEditTranslation and IsDarkTheme
        /// </summary>
        /// <param name="ParagraphStatus"></param>
        /// <returns></returns>
        public virtual string ParagraphClass(Paragraph p)
        {
            if (p != null && p.IsEditTranslation)
            {
                switch (p.Status)
                {
                    case ParagraphStatus.Started:
                        return "parStarted";
                    case ParagraphStatus.Working:
                        return "parWorking";
                    case ParagraphStatus.Doubt:
                        return "parDoubt";
                    case ParagraphStatus.Ok:
                        return "parOk";
                    case ParagraphStatus.Closed:
                        return "parClosed";
                }
            }
            return "parClosed";
        }


        public virtual string BackgroundParagraphColor(ParagraphStatus ParagraphStatus)
		{
            return IsDarkTheme ? darkTheme : lightTheme;
        }

    }
}
