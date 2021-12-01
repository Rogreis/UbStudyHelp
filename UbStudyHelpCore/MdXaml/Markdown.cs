﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if !MIG_FREE
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
#endif

#if MIG_FREE
namespace Markdown.Xaml
#else
namespace MdXaml
#endif
{
    public class Markdown : DependencyObject, IUriContext
    {
        #region const
        /// <summary>
        /// maximum nested depth of [] and () supported by the transform; implementation detail
        /// </summary>
        private const int _nestDepth = 6;

        /// <summary>
        /// Tabs are automatically converted to spaces as part of the transform  
        /// this constant determines how "wide" those tabs become in spaces  
        /// </summary>
        private const int _tabWidth = 4;

        private const string TagHeading1 = "Heading1";
        private const string TagHeading2 = "Heading2";
        private const string TagHeading3 = "Heading3";
        private const string TagHeading4 = "Heading4";
        private const string TagHeading5 = "Heading5";
        private const string TagHeading6 = "Heading6";
        private const string TagCode = "CodeSpan";
        private const string TagCodeBlock = "CodeBlock";
        private const string TagBlockquote = "Blockquote";
        private const string TagNote = "Note";
        private const string TagTableHeader = "TableHeader";
        private const string TagTableBody = "TableBody";
        private const string TagOddTableRow = "OddTableRow";
        private const string TagEvenTableRow = "EvenTableRow";

        private const string TagBoldSpan = "Bold";
        private const string TagItalicSpan = "Italic";
        private const string TagStrikethroughSpan = "Strikethrough";
        private const string TagUnderlineSpan = "Underline";

        private const string TagRuleSingle = "RuleSingle";
        private const string TagRuleDouble = "RuleDouble";
        private const string TagRuleBold = "RuleBold";
        private const string TagRuleBoldWithSingle = "RuleBoldWithSingle";

        #endregion

        /// <summary>
        /// when true, bold and italic require non-word characters on either side  
        /// WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool StrictBoldItalic { get; set; }

        public bool DisabledTag { get; set; }

        public bool DisabledTootip { get; set; }

        public bool DisabledLazyLoad { get; set; }

        public string AssetPathRoot { get; set; }

        public ICommand HyperlinkCommand { get; set; }

        public Uri BaseUri { get; set; }

        #region dependencyobject property

        // Using a DependencyProperty as the backing store for DocumentStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentStyleProperty =
            DependencyProperty.Register(nameof(DocumentStyle), typeof(Style), typeof(Markdown), new PropertyMetadata(null));

        /// <summary>
        /// top-level flow document style
        /// </summary>
        public Style DocumentStyle
        {
            get { return (Style)GetValue(DocumentStyleProperty); }
            set { SetValue(DocumentStyleProperty, value); }
        }

        #endregion


        #region legacy property

        public Style Heading1Style { get; set; }
        public Style Heading2Style { get; set; }
        public Style Heading3Style { get; set; }
        public Style Heading4Style { get; set; }
        public Style Heading5Style { get; set; }
        public Style Heading6Style { get; set; }
        public Style NormalParagraphStyle { get; set; }
        public Style CodeStyle { get; set; }
        public Style CodeBlockStyle { get; set; }
        public Style BlockquoteStyle { get; set; }
        public Style LinkStyle { get; set; }
        public Style ImageStyle { get; set; }
        public Style SeparatorStyle { get; set; }
        public Style TableStyle { get; set; }
        public Style TableHeaderStyle { get; set; }
        public Style TableBodyStyle { get; set; }
        public Style NoteStyle { get; set; }

        #endregion


        #region regex pattern


        #endregion

        public Markdown()
        {
            HyperlinkCommand = NavigationCommands.GoToPage;
            AssetPathRoot = Environment.CurrentDirectory;
        }

        public FlowDocument Transform(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            text = TextUtil.Normalize(text);
            var document = Create<FlowDocument, Block>(RunBlockGamut(text, true));

            document.SetBinding(FlowDocument.StyleProperty, new Binding(DocumentStyleProperty.Name) { Source = this });

            return document;
        }

        /// <summary>
        /// Perform transformations that form block-level tags like paragraphs, headers, and list items.
        /// </summary>
        private IEnumerable<Block> RunBlockGamut(string text, bool supportTextAlignment)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate2(
                text,
                _codeBlockFirst, CodeBlocksWithLangEvaluator,
                _listLevel > 0 ? _listNested : _listTopLevel, ListEvaluator,
                s1 => DoBlockquotes(s1,
                s2 => DoHeaders(s2,
                s3 => DoHorizontalRules(s3,
                s4 => DoTable(s4,
                s5 => DoNote(s5, supportTextAlignment,
                s6 => DoIndentCodeBlock(s6,
                sn => FormParagraphs(sn, supportTextAlignment)))))))
            );
        }

        /// <summary>
        /// Perform transformations that occur *within* block-level tags like paragraphs, headers, and list items.
        /// </summary>
        private IEnumerable<Inline> RunSpanGamut(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return DoCodeSpans(text,
                s0 => DoImagesOrHrefs(s0,
                s1 => DoTextDecorations(s1,
                s2 => DoText(s2))));
        }


        #region grammer - paragraph

        private static readonly Regex _align = new Regex(@"^p([<=>])\.", RegexOptions.Compiled);
        private static readonly Regex _newlinesLeadingTrailing = new Regex(@"^\n+|\n+\z", RegexOptions.Compiled);
        private static readonly Regex _newlinesMultiple = new Regex(@"\n{2,}", RegexOptions.Compiled);

        /// <summary>
        /// splits on two or more newlines, to form "paragraphs";    
        /// </summary>
        private IEnumerable<Block> FormParagraphs(string text, bool supportTextAlignment)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var trimemdText = _newlinesLeadingTrailing.Replace(text, "");

            string[] grafs = trimemdText == "" ?
                new string[0] :
                _newlinesMultiple.Split(trimemdText);

            foreach (var g in grafs)
            {
                var chip = g;

                TextAlignment? indiAlignment = null;

                if (supportTextAlignment)
                {
                    var alignMatch = _align.Match(chip);
                    if (alignMatch.Success)
                    {
                        chip = chip.Substring(alignMatch.Length);
                        switch (alignMatch.Groups[1].Value)
                        {
                            case "<":
                                indiAlignment = TextAlignment.Left;
                                break;
                            case ">":
                                indiAlignment = TextAlignment.Right;
                                break;
                            case "=":
                                indiAlignment = TextAlignment.Center;
                                break;
                        }
                    }
                }

                var block = Create<Paragraph, Inline>(RunSpanGamut(chip));
                if (NormalParagraphStyle != null)
                {
                    block.Style = NormalParagraphStyle;
                }
                if (indiAlignment.HasValue)
                {
                    block.TextAlignment = indiAlignment.Value;
                }

                yield return block;
            }
        }

        #endregion

        #region grammer - image or href

        private static readonly Regex _imageOrHrefInline = new Regex(string.Format(@"
                (                           # wrap whole match in $1
                    (!)?                    # image maker = $2
                    \[
                        ({0})               # link text = $3
                    \]
                    \(                      # literal paren
                        [ ]*
                        ({1})               # href = $4
                        [ ]*
                        (                   # $5
                        (['""])             # quote char = $6
                        (.*?)               # title = $7
                        \6                  # matching quote
                        [ ]*                # ignore any spaces between closing quote and )
                        )?                  # title is optional
                    \)
                )", GetNestedBracketsPattern(), GetNestedParensPattern()),
                  RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


        private IEnumerable<Inline> DoImagesOrHrefs(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate(text, _imageOrHrefInline, ImageOrHrefInlineEvaluator, defaultHandler);
        }

        private Inline ImageOrHrefInlineEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (String.IsNullOrEmpty(match.Groups[2].Value))
            {
                return TreatsAsHref(match);
            }
            else
            {
                return TreatsAsImage(match);
            }
        }

        private Inline TreatsAsHref(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string linkText = match.Groups[3].Value;
            string url = match.Groups[4].Value;
            string title = match.Groups[7].Value;

            var result = Create<Hyperlink, Inline>(RunSpanGamut(linkText));
            result.Command = HyperlinkCommand;
            result.CommandParameter = url;

            if (!DisabledTootip)
            {
                result.ToolTip = string.IsNullOrWhiteSpace(title) ?
                    url :
                    String.Format("\"{0}\"\r\n{1}", title, url);
            }

            if (LinkStyle != null)
            {
                result.Style = LinkStyle;
            }

            return result;
        }

        private Inline TreatsAsImage(Match match)
        {
            string linkText = match.Groups[3].Value;
            string url = match.Groups[4].Value;
            string title = match.Groups[7].Value;

            BitmapImage imgSource = null;

            // check embedded resoruce
            try
            {
                Uri packUri;
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && BaseUri != null)
                {
                    packUri = new Uri(BaseUri, url);
                }
                else
                {
                    packUri = new Uri(url);
                }

                imgSource = MakeImage(packUri);
            }
            catch { }

            // check filesystem
            if (imgSource is null)
            {
                try
                {
                    Uri imgUri;

                    if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) && !System.IO.Path.IsPathRooted(url) && AssetPathRoot != null)
                    {
                        if (Uri.IsWellFormedUriString(AssetPathRoot, UriKind.Absolute))
                        {
                            imgUri = new Uri(new Uri(AssetPathRoot), url);
                        }
                        else
                        {
                            url = System.IO.Path.Combine(AssetPathRoot ?? string.Empty, url);
                            imgUri = new Uri(url, UriKind.RelativeOrAbsolute);
                        }
                    }
                    else imgUri = new Uri(url, UriKind.RelativeOrAbsolute);

                    imgSource = MakeImage(imgUri);
                }
                catch { }
            }

            // error
            if (imgSource is null)
            {
                return new Run("!" + url) { Foreground = Brushes.Red };
            }


            Image image = new Image { Source = imgSource, Tag = linkText };
            if (ImageStyle is null)
            {
                image.Margin = new Thickness(0);
            }
            else
            {
                image.Style = ImageStyle;
            }
            if (!DisabledTootip && !string.IsNullOrWhiteSpace(title))
            {
                image.ToolTip = title;
            }

            // Bind size so document is updated when image is downloaded
            if (imgSource.IsDownloading)
            {
                Binding binding = new Binding(nameof(BitmapImage.Width));
                binding.Source = imgSource;
                binding.Mode = BindingMode.OneWay;

                BindingExpressionBase bindingExpression = BindingOperations.SetBinding(image, Image.WidthProperty, binding);
                EventHandler downloadCompletedHandler = null;
                downloadCompletedHandler = (sender, e) =>
                {
                    imgSource.DownloadCompleted -= downloadCompletedHandler;
                    imgSource.Freeze();
                    bindingExpression.UpdateTarget();
                };
                imgSource.DownloadCompleted += downloadCompletedHandler;
            }
            else
            {
                image.Width = imgSource.Width;
            }

            return new InlineUIContainer(image);
        }

        private BitmapImage MakeImage(Uri url)
        {
            if (DisabledLazyLoad)
            {
                return new BitmapImage(url);
            }
            else
            {
                var imgSource = new BitmapImage();
                imgSource.BeginInit();
                imgSource.CacheOption = BitmapCacheOption.None;
                imgSource.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                imgSource.CacheOption = BitmapCacheOption.OnLoad;
                imgSource.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                imgSource.UriSource = url;
                imgSource.EndInit();

                return imgSource;
            }
        }

        #endregion


        #region grammer - header

        private static readonly Regex _headerSetext = new Regex(@"
                ^(.+?)
                [ ]*
                \n
                (=+|-+)     # $1 = string of ='s or -'s
                [ ]*
                \n+",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _headerAtx = new Regex(@"
                ^(\#{1,6})  # $1 = string of #'s
                [ ]*
                (.+?)       # $2 = Header text
                [ ]*
                \#*         # optional closing #'s (not counted)
                \n+",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown headers into HTML header tags
        /// </summary>
        /// <remarks>
        /// Header 1  
        /// ========  
        /// 
        /// Header 2  
        /// --------  
        /// 
        /// # Header 1  
        /// ## Header 2  
        /// ## Header 2 with closing hashes ##  
        /// ...  
        /// ###### Header 6  
        /// </remarks>
        private IEnumerable<Block> DoHeaders(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate<Block>(text, _headerSetext, m => SetextHeaderEvaluator(m),
                s => Evaluate<Block>(s, _headerAtx, m => AtxHeaderEvaluator(m), defaultHandler));
        }

        private Block SetextHeaderEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string header = match.Groups[1].Value;
            int level = match.Groups[2].Value.StartsWith("=") ? 1 : 2;

            //TODO: Style the paragraph based on the header level
            return CreateHeader(level, RunSpanGamut(header.Trim()));
        }

        private Block AtxHeaderEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string header = match.Groups[2].Value;
            int level = match.Groups[1].Value.Length;
            return CreateHeader(level, RunSpanGamut(header));
        }

        public Block CreateHeader(int level, IEnumerable<Inline> content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var block = Create<Paragraph, Inline>(content);

            switch (level)
            {
                case 1:
                    if (Heading1Style != null)
                    {
                        block.Style = Heading1Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading1;
                    }
                    break;

                case 2:
                    if (Heading2Style != null)
                    {
                        block.Style = Heading2Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading2;
                    }
                    break;

                case 3:
                    if (Heading3Style != null)
                    {
                        block.Style = Heading3Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading3;
                    }
                    break;

                case 4:
                    if (Heading4Style != null)
                    {
                        block.Style = Heading4Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading4;
                    }
                    break;

                case 5:
                    if (Heading5Style != null)
                    {
                        block.Style = Heading5Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading5;
                    }
                    break;

                case 6:
                    if (Heading6Style != null)
                    {
                        block.Style = Heading6Style;
                    }
                    if (!DisabledTag)
                    {
                        block.Tag = TagHeading6;
                    }
                    break;
            }

            return block;
        }
        #endregion

        #region grammer - Note
        private static readonly Regex _note = new Regex(@"
                ^(\<)       # $1 = starting marker <
                [ ]*
                (.+?)       # $2 = Header text
                [ ]*
                \>*         # optional closing >'s (not counted)
                \n+
            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown into HTML paragraphs.
        /// </summary>
        /// <remarks>
        /// < Note
        /// </remarks>
        private IEnumerable<Block> DoNote(string text, bool supportTextAlignment,
                Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate<Block>(text, _note,
                m => NoteEvaluator(m, supportTextAlignment),
                defaultHandler);
        }

        private Block NoteEvaluator(Match match, bool supportTextAlignment)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string text = match.Groups[2].Value;

            TextAlignment? indiAlignment = null;

            if (supportTextAlignment)
            {
                var alignMatch = _align.Match(text);
                if (alignMatch.Success)
                {
                    text = text.Substring(alignMatch.Length);
                    switch (alignMatch.Groups[1].Value)
                    {
                        case "<":
                            indiAlignment = TextAlignment.Left;
                            break;
                        case ">":
                            indiAlignment = TextAlignment.Right;
                            break;
                        case "=":
                            indiAlignment = TextAlignment.Center;
                            break;
                    }
                }
            }

            return NoteComment(RunSpanGamut(text), indiAlignment);
        }

        public Block NoteComment(IEnumerable<Inline> content, TextAlignment? indiAlignment)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var block = Create<Paragraph, Inline>(content);
            if (NoteStyle != null)
            {
                block.Style = NoteStyle;
            }
            if (!DisabledTag)
            {
                block.Tag = TagNote;
            }
            if (indiAlignment.HasValue)
            {
                block.TextAlignment = indiAlignment.Value;
            }

            return block;
        }
        #endregion

        #region grammer - horizontal rules

        private static readonly Regex _horizontalRules = new Regex(@"
                ^[ ]{0,3}                   # Leading space
                    ([-=*_])                # $1: First marker ([markers])
                    (?>                     # Repeated marker group
                        [ ]{0,2}            # Zero, one, or two spaces.
                        \1                  # Marker character
                    ){2,}                   # Group repeated at least twice
                    [ ]*                    # Trailing spaces
                    \n                      # End of line.
                ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown horizontal rules into HTML hr tags
        /// </summary>
        /// <remarks>
        /// ***  
        /// * * *  
        /// ---
        /// - - -
        /// </remarks>
        private IEnumerable<Block> DoHorizontalRules(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate(text, _horizontalRules, RuleEvaluator, defaultHandler);
        }

        /// <summary>
        /// Single line separator.
        /// </summary>
        private Block RuleEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            switch (match.Groups[1].Value)
            {
                default:
                case "-":
                    {
                        var sep = new Separator();
                        if (SeparatorStyle != null)
                            sep.Style = SeparatorStyle;

                        var container = new BlockUIContainer(sep);
                        if (!DisabledTag)
                        {
                            container.Tag = TagRuleSingle;
                        }
                        return container;
                    }

                case "=":
                    {
                        var stackPanel = new StackPanel();
                        for (int i = 0; i < 2; i++)
                        {
                            var sep = new Separator();
                            if (SeparatorStyle != null)
                                sep.Style = SeparatorStyle;

                            stackPanel.Children.Add(sep);
                        }

                        var container = new BlockUIContainer(stackPanel);
                        if (!DisabledTag)
                        {
                            container.Tag = TagRuleDouble;
                        }
                        return container;
                    }

                case "*":
                    {
                        var stackPanel = new StackPanel();
                        for (int i = 0; i < 2; i++)
                        {
                            var sep = new Separator()
                            {
                                Margin = new Thickness(0)
                            };

                            if (SeparatorStyle != null)
                                sep.Style = SeparatorStyle;

                            stackPanel.Children.Add(sep);
                        }

                        var container = new BlockUIContainer(stackPanel);
                        if (!DisabledTag)
                        {
                            container.Tag = TagRuleBold;
                        }
                        return container;
                    }

                case "_":
                    {
                        var stackPanel = new StackPanel();
                        for (int i = 0; i < 2; i++)
                        {
                            var sep = new Separator()
                            {
                                Margin = new Thickness(0)
                            };

                            if (SeparatorStyle != null)
                                sep.Style = SeparatorStyle;

                            stackPanel.Children.Add(sep);
                        }

                        var sepLst = new Separator();
                        if (SeparatorStyle != null)
                            sepLst.Style = SeparatorStyle;

                        stackPanel.Children.Add(sepLst);

                        var container = new BlockUIContainer(stackPanel);
                        if (!DisabledTag)
                        {
                            container.Tag = TagRuleBoldWithSingle;
                        }
                        return container;
                    }
            }
        }

        #endregion


        #region grammer - list

        // `alphabet order` and `roman number` must start 'a.'～'c.' and 'i,'～'iii,'.
        // This restrict is avoid to treat "Yes," as list marker.
        private const string _firstListMaker = @"(?:[*+=-]|\d+[.]|[a-c][.]|[i]{1,3}[,]|[A-C][.]|[I]{1,3}[,])";
        private const string _subseqListMaker = @"(?:[*+=-]|\d+[.]|[a-c][.]|[cdilmvx]+[,]|[A-C][.]|[CDILMVX]+[,])";

        //private const string _markerUL = @"[*+=-]";
        //private const string _markerOL = @"\d+[.]|\p{L}+[.,]";

        // Unordered List
        private const string _markerUL_Disc = @"[*]";
        private const string _markerUL_Box = @"[+]";
        private const string _markerUL_Circle = @"[-]";
        private const string _markerUL_Square = @"[=]";

        private static readonly Regex _startsWith_markerUL_Disc = new Regex("\\A" + _markerUL_Disc, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerUL_Box = new Regex("\\A" + _markerUL_Box, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerUL_Circle = new Regex("\\A" + _markerUL_Circle, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerUL_Square = new Regex("\\A" + _markerUL_Square, RegexOptions.Compiled);

        // Ordered List
        private const string _markerOL_Number = @"\d+[.]";
        private const string _markerOL_LetterLower = @"[a-c][.]";
        private const string _markerOL_LetterUpper = @"[A-C][.]";
        private const string _markerOL_RomanLower = @"[cdilmvx]+[,]";
        private const string _markerOL_RomanUpper = @"[CDILMVX]+[,]";

        private static readonly Regex _startsWith_markerOL_Number = new Regex("\\A" + _markerOL_Number, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerOL_LetterLower = new Regex("\\A" + _markerOL_LetterLower, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerOL_LetterUpper = new Regex("\\A" + _markerOL_LetterUpper, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerOL_RomanLower = new Regex("\\A" + _markerOL_RomanLower, RegexOptions.Compiled);
        private static readonly Regex _startsWith_markerOL_RomanUpper = new Regex("\\A" + _markerOL_RomanUpper, RegexOptions.Compiled);

        private int _listLevel;

        /// <summary>
        /// Maximum number of levels a single list can have.
        /// In other words, _listDepth - 1 is the maximum number of nested lists.
        /// </summary>
        private const int _listDepth = 4;

        private static readonly string _wholeList = string.Format(@"
            (?<whltxt>                      # whole list
              (?<mkr_i>                     # list marker with indent
                (?![ ]{{0,3}}(?<hrm>[-=*_])([ ]{{0,2}}\k<hrm>){{2,}})
                (?<idt>[ ]{{0,{2}}})
                (?<mkr>{0})                 # first list item marker
                [ ]+
              )
              (?s:.+?)
              (                             # $4
                  \z
                |
                  \n{{2,}}
                  (?=\S)
                  (?!                       # Negative lookahead for another list item marker
                    [ ]*
                    {1}[ ]+
                  )
              )
            )", _firstListMaker, _subseqListMaker, _listDepth - 1);

        private static readonly Regex _startNoIndentRule = new Regex(@"\A[ ]{0,2}(?<hrm>[-=*_])([ ]{0,2}\k<hrm>){2,}",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _startNoIndentSublistMarker = new Regex(@"\A" + _subseqListMaker, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _startQuoteOrHeader = new Regex(@"\A(\#{1,6}[ ]|>|```)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _listNested = new Regex(@"^" + _wholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static readonly Regex _listTopLevel = new Regex(@"(?:(?<=\n)|\A\n?)" + _wholeList,
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private IEnumerable<Block> ListEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            // Check text marker style.
            var markerDetect = GetTextMarkerStyle(match.Groups["mkr"].Value);
            TextMarkerStyle textMarker = markerDetect.Item1;
            string markerPattern = markerDetect.Item2;
            Regex markerRegex = markerDetect.Item3;
            int indentAppending = markerDetect.Item4;

            // count indent from first marker with indent
            int countIndent = TextUtil.CountIndent(match.Groups["mkr_i"].Value);

            // whole list
            string[] whileListLins = match.Groups["whltxt"].Value.Split('\n');

            // collect detendentable line
            var listBulder = new StringBuilder();
            var outerListBuildre = new StringBuilder();
            var isInOuterList = false;
            foreach (var line in whileListLins)
            {
                if (!isInOuterList)
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        listBulder.Append("").Append("\n");
                    }
                    else if (TextUtil.TryDetendLine(line, countIndent, out var stripedLine))
                    {
                        // is it horizontal line?
                        if (_startNoIndentRule.IsMatch(stripedLine))
                        {
                            isInOuterList = true;
                        }
                        // is it header or blockquote?
                        else if (_startQuoteOrHeader.IsMatch(stripedLine))
                        {
                            isInOuterList = true;
                        }
                        // is it had list marker?
                        else if (_startNoIndentSublistMarker.IsMatch(stripedLine))
                        {
                            // is it same marker as now processed?
                            if (markerRegex.IsMatch(stripedLine))
                            {
                                listBulder.Append(stripedLine).Append("\n");
                            }
                            else isInOuterList = true;
                        }
                        else
                        {
                            var detentedline = TextUtil.DetentLineBestEffort(stripedLine, indentAppending);
                            listBulder.Append(detentedline).Append("\n");
                        }
                    }
                    else isInOuterList = true;
                }

                if (isInOuterList)
                {
                    outerListBuildre.Append(line).Append("\n");
                }
            }

            string list = listBulder.ToString();

            var resultList = Create<List, ListItem>(ProcessListItems(list, markerPattern));

            resultList.MarkerStyle = textMarker;

            yield return resultList;

            if (outerListBuildre.Length != 0)
            {
                foreach (var ctrl in RunBlockGamut(outerListBuildre.ToString(), true))
                    yield return ctrl;
            }
        }

        /// <summary>
        /// Process the contents of a single ordered or unordered list, splitting it
        /// into individual list items.
        /// </summary>
        private IEnumerable<ListItem> ProcessListItems(string list, string marker)
        {
            // The listLevel global keeps track of when we're inside a list.
            // Each time we enter a list, we increment it; when we leave a list,
            // we decrement. If it's zero, we're not in a list anymore.

            // We do this because when we're not inside a list, we want to treat
            // something like this:

            //    I recommend upgrading to version
            //    8. Oops, now this line is treated
            //    as a sub-list.

            // As a single paragraph, despite the fact that the second line starts
            // with a digit-period-space sequence.

            // Whereas when we're inside a list (or sub-list), that line will be
            // treated as the start of a sub-list. What a kludge, huh? This is
            // an aspect of Markdown's syntax that's hard to parse perfectly
            // without resorting to mind-reading. Perhaps the solution is to
            // change the syntax rules such that sub-lists must start with a
            // starting cardinal number; e.g. "1." or "a.".

            _listLevel++;
            try
            {
                // Trim trailing blank lines:
                list = Regex.Replace(list, @"\n{2,}\z", "\n");

                string pattern = string.Format(
                  @"(\n)?                  # leading line = $1
                (^[ ]*)                    # leading whitespace = $2
                ({0}) [ ]+                 # list marker = $3
                ((?s:.+?)                  # list item text = $4
                (\n{{1,2}}))      
                (?= \n* (\z | \2 ({0}) [ ]+))", marker);

                var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                var matches = regex.Matches(list);
                foreach (Match m in matches)
                {
                    yield return ListItemEvaluator(m);
                }
            }
            finally
            {
                _listLevel--;
            }
        }

        private ListItem ListItemEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string item = match.Groups[4].Value;
            string leadingLine = match.Groups[1].Value;

            if (!String.IsNullOrEmpty(leadingLine) || Regex.IsMatch(item, @"\n{2,}"))
                // we could correct any bad indentation here..
                return Create<ListItem, Block>(RunBlockGamut(item, false));
            else
            {
                // recursion for sub-lists
                return Create<ListItem, Block>(RunBlockGamut(item, false));
            }
        }

        /// <summary>
        /// Get the text marker style based on a specific regex.
        /// </summary>
        /// <param name="markerText">list maker (eg. * + 1. a. </param>
        /// <returns>
        ///     1; return Type. 
        ///     2: match regex pattern
        ///     3: char length of listmaker
        /// </returns>
        private static Tuple<TextMarkerStyle, string, Regex, int> GetTextMarkerStyle(string markerText)
        {
            if (Regex.IsMatch(markerText, _markerUL_Disc))
            {
                return Tuple.Create(TextMarkerStyle.Disc, _markerUL_Disc, _startsWith_markerUL_Disc, 2);
            }
            else if (Regex.IsMatch(markerText, _markerUL_Box))
            {
                return Tuple.Create(TextMarkerStyle.Box, _markerUL_Box, _startsWith_markerUL_Box, 2);
            }
            else if (Regex.IsMatch(markerText, _markerUL_Circle))
            {
                return Tuple.Create(TextMarkerStyle.Circle, _markerUL_Circle, _startsWith_markerUL_Circle, 2);
            }
            else if (Regex.IsMatch(markerText, _markerUL_Square))
            {
                return Tuple.Create(TextMarkerStyle.Square, _markerUL_Square, _startsWith_markerUL_Square, 2);
            }
            else if (Regex.IsMatch(markerText, _markerOL_Number))
            {
                return Tuple.Create(TextMarkerStyle.Decimal, _markerOL_Number, _startsWith_markerOL_Number, 3);
            }
            else if (Regex.IsMatch(markerText, _markerOL_LetterLower))
            {
                return Tuple.Create(TextMarkerStyle.LowerLatin, _markerOL_LetterLower, _startsWith_markerOL_LetterLower, 3);
            }
            else if (Regex.IsMatch(markerText, _markerOL_LetterUpper))
            {
                return Tuple.Create(TextMarkerStyle.UpperLatin, _markerOL_LetterUpper, _startsWith_markerOL_LetterUpper, 3);
            }
            else if (Regex.IsMatch(markerText, _markerOL_RomanLower))
            {
                return Tuple.Create(TextMarkerStyle.LowerRoman, _markerOL_RomanLower, _startsWith_markerOL_RomanLower, 3);
            }
            else if (Regex.IsMatch(markerText, _markerOL_RomanUpper))
            {
                return Tuple.Create(TextMarkerStyle.UpperRoman, _markerOL_RomanUpper, _startsWith_markerOL_RomanUpper, 3);
            }

            throw new InvalidOperationException("sorry library manager forget to modify about listmerker.");
        }

        #endregion


        #region grammer - table

        private static readonly Regex _table = new Regex(@"
            (                               # whole table
                [ \n]*
                (?<hdr>                     # table header
                    ([^\n\|]*\|[^\n]+)
                )
                [ ]*\n[ ]*
                (?<col>                     # column style
                    \|?([ ]*:?-+:?[ ]*(\||$))+
                )
                (?<row>                     # table row
                    (
                        [ ]*\n[ ]*
                        ([^\n\|]*\|[^\n]+)
                    )+
                )
            )",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public IEnumerable<Block> DoTable(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate(text, _table, TableEvalutor, defaultHandler);
        }

        private Block TableEvalutor(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var headerTxt = match.Groups["hdr"].Value.Trim();
            var styleTxt = match.Groups["col"].Value.Trim();
            var rowTxt = match.Groups["row"].Value.Trim();

            string ExtractCoverBar(string txt)
            {
                if (txt[0] == '|')
                    txt = txt.Substring(1);

                if (String.IsNullOrEmpty(txt))
                    return txt;

                if (txt[txt.Length - 1] == '|')
                    txt = txt.Substring(0, txt.Length - 1);

                return txt;
            }

            var mdtable = new MdTable(
                ExtractCoverBar(headerTxt).Split('|'),
                ExtractCoverBar(styleTxt).Split('|').Select(txt => txt.Trim()).ToArray(),
                rowTxt.Split('\n').Select(ritm =>
                {
                    var trimRitm = ritm.Trim();
                    return ExtractCoverBar(trimRitm).Split('|');
                }).ToList());

            // table
            var table = new Table();
            if (TableStyle != null)
            {
                table.Style = TableStyle;
            }

            // table columns
            while (table.Columns.Count < mdtable.ColCount)
                table.Columns.Add(new TableColumn());

            // table header
            var tableHeaderRG = new TableRowGroup();
            if (TableHeaderStyle != null)
            {
                tableHeaderRG.Style = TableHeaderStyle;
            }
            if (!DisabledTag)
            {
                tableHeaderRG.Tag = TagTableHeader;
            }

            var tableHeader = CreateTableRow(mdtable.Header);
            tableHeaderRG.Rows.Add(tableHeader);
            table.RowGroups.Add(tableHeaderRG);

            // row
            var tableBodyRG = new TableRowGroup();
            if (TableBodyStyle != null)
            {
                tableBodyRG.Style = TableBodyStyle;
            }
            if (!DisabledTag)
            {
                tableBodyRG.Tag = TagTableBody;
            }

            foreach (int rowIdx in Enumerable.Range(0, mdtable.Details.Count))
            {
                var tableBody = CreateTableRow(mdtable.Details[rowIdx]);
                if (!DisabledTag)
                {
                    tableBody.Tag = (rowIdx & 1) == 0 ? TagOddTableRow : TagEvenTableRow;
                }

                tableBodyRG.Rows.Add(tableBody);
            }
            table.RowGroups.Add(tableBodyRG);

            return table;
        }

        private TableRow CreateTableRow(IList<MdTableCell> mdcells)
        {
            var tableRow = new TableRow();

            foreach (var mdcell in mdcells)
            {
                TableCell cell = mdcell.Text is null ?
                    new TableCell() :
                    new TableCell(Create<Paragraph, Inline>(RunSpanGamut(mdcell.Text)));

                if (mdcell.Horizontal.HasValue)
                    cell.TextAlignment = mdcell.Horizontal.Value;

                if (mdcell.RowSpan != 1)
                    cell.RowSpan = mdcell.RowSpan;

                if (mdcell.ColSpan != 1)
                    cell.ColumnSpan = mdcell.ColSpan;

                tableRow.Cells.Add(cell);
            }

            return tableRow;
        }

        #endregion


        #region grammer - code block

        private static Regex _codeBlockFirst = new Regex(@"
                    ^          # Character before opening
                    [ ]{0,3}
                    (`{3,})          # $1 = Opening run of `
                    ([^\n`]*)        # $2 = The code lang
                    \n
                    ((.|\n)+?)       # $3 = The code block
                    \n[ ]*
                    \1
                    (?!`)[\n]+", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);

        private static Regex _indentCodeBlock = new Regex(@"
                    (?:\A|^[ ]*\n)
                    (
                    [ ]{4}.+
                    (\n([ ]{4}.+|[ ]*))*
                    \n?
                    )
                    ", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);


        private IEnumerable<Block> DoIndentCodeBlock(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate(text, _indentCodeBlock, CodeBlocksWithoutLangEvaluator, defaultHandler);
        }

        private Block CodeBlocksWithLangEvaluator(Match match)
            => CodeBlocksEvaluator(match.Groups[2].Value, match.Groups[3].Value);


        private Block CodeBlocksWithoutLangEvaluator(Match match)
        {
            var detentTxt = String.Join("\n", match.Groups[1].Value.Split('\n').Select(line => TextUtil.DetentLineBestEffort(line, 4)));
            return CodeBlocksEvaluator(null, _newlinesLeadingTrailing.Replace(detentTxt, ""));
        }

#if MIG_FREE
        private Block CodeBlocksEvaluator(string lang, string code)
        {
            var text = new Run(code);
            var result = new Paragraph(text);
            if (CodeBlockStyle != null)
            {
                result.Style = CodeBlockStyle;
            }
            if (!DisabledTag)
            {
                result.Tag = TagCodeBlock;
            }

            return result;
        }
#else
        private Block CodeBlocksEvaluator(string lang, string code)
        {
            var txtEdit = new TextEditor();

            if (!String.IsNullOrEmpty(lang))
            {
                var highlight = HighlightingManager.Instance.GetDefinitionByExtension("." + lang);
                txtEdit.SetCurrentValue(TextEditor.SyntaxHighlightingProperty, highlight);
                txtEdit.Tag = lang;
            }

            txtEdit.Text = code;
            txtEdit.HorizontalAlignment = HorizontalAlignment.Stretch;
            txtEdit.IsReadOnly = true;
            txtEdit.PreviewMouseWheel += (s, e) =>
            {
                if (e.Handled) return;

                e.Handled = true;

                var isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                if (isShiftDown)
                {
                    // horizontal scroll
                    var offset = txtEdit.HorizontalOffset;
                    offset -= e.Delta;
                    txtEdit.ScrollToHorizontalOffset(offset);
                }
                else
                {
                    // event bubbles
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = s;

                    var parentObj = ((Control)s).Parent;
                    if (parentObj is UIElement uielm)
                    {
                        uielm.RaiseEvent(eventArg);
                    }
                    else if (parentObj is ContentElement celem)
                    {
                        celem.RaiseEvent(eventArg);
                    }
                }
            };


            var result = new BlockUIContainer(txtEdit);
            if (CodeBlockStyle != null)
            {
                result.Style = CodeBlockStyle;
            }
            if (!DisabledTag)
            {
                result.Tag = TagCodeBlock;
            }

            return result;
        }

#endif

        #endregion


        #region grammer - code

        private static Regex _codeSpan = new Regex(@"
                    (?<!\\)   # Character before opening ` can't be a backslash
                    (`+)      # $1 = Opening run of `
                    (.+?)     # $2 = The code block
                    (?<!`)
                    \1
                    (?!`)", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown `code spans` into HTML code tags
        /// </summary>
        private IEnumerable<Inline> DoCodeSpans(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            //    * You can use multiple backticks as the delimiters if you want to
            //        include literal backticks in the code span. So, this input:
            //
            //        Just type ``foo `bar` baz`` at the prompt.
            //
            //        Will translate to:
            //
            //          <p>Just type <code>foo `bar` baz</code> at the prompt.</p>
            //
            //        There's no arbitrary limit to the number of backticks you
            //        can use as delimters. If you need three consecutive backticks
            //        in your code, use four for delimiters, etc.
            //
            //    * You can use spaces to get literal backticks at the edges:
            //
            //          ... type `` `bar` `` ...
            //
            //        Turns to:
            //
            //          ... type <code>`bar`</code> ...         
            //

            return Evaluate(text, _codeSpan, CodeSpanEvaluator, defaultHandler);
        }

        private Inline CodeSpanEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            string span = match.Groups[2].Value;
            span = Regex.Replace(span, @"^[ ]*", ""); // leading whitespace
            span = Regex.Replace(span, @"[ ]*$", ""); // trailing whitespace

            var result = new Run(span);
            if (CodeStyle != null)
            {
                result.Style = CodeStyle;
            }
            if (!DisabledTag)
            {
                result.Tag = TagCode;
            }

            return result;
        }

        #endregion


        #region grammer - textdecorations

        private static readonly Regex _strictBold = new Regex(@"([\W_]|^) (\*\*|__) (?=\S) ([^\r]*?\S[\*_]*) \2 ([\W_]|$)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _strictItalic = new Regex(@"([\W_]|^) (\*|_) (?=\S) ([^\r\*_]*?\S) \2 ([\W_]|$)",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _strikethrough = new Regex(@"(~~) (?=\S) (.+?) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex _underline = new Regex(@"(__) (?=\S) (.+?) (?<=\S) \1",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex _color = new Regex(@"%\{[ \t]*color[ \t]*:([^\}]+)\}", RegexOptions.Compiled);

        /// <summary>
        /// Turn Markdown *italics* and **bold** into HTML strong and em tags
        /// </summary>
        private IEnumerable<Inline> DoTextDecorations(string text, Func<string, IEnumerable<Inline>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            // <strong> must go first, then <em>
            if (StrictBoldItalic)
            {
                return Evaluate<Inline>(text, _strictBold, m => BoldEvaluator(m, 3),
                    s1 => Evaluate<Inline>(s1, _strictItalic, m => ItalicEvaluator(m, 3),
                    s2 => Evaluate<Inline>(s2, _strikethrough, m => StrikethroughEvaluator(m, 2),
                    s3 => Evaluate<Inline>(s3, _underline, m => UnderlineEvaluator(m, 2),
                    s4 => defaultHandler(s4)))));
            }
            else
            {
                var rtn = new List<Inline>();

                var buff = new StringBuilder();

                void HandleBefore()
                {
                    if (buff.Length > 0)
                    {
                        rtn.AddRange(defaultHandler(buff.ToString()));
                        buff.Clear();
                    }
                }

                for (var i = 0; i < text.Length; ++i)
                {
                    var ch = text[i];
                    switch (ch)
                    {
                        default:
                            buff.Append(ch);
                            break;

                        case '\\': // escape
                            if (++i < text.Length)
                            {
                                switch (text[i])
                                {
                                    default:
                                        buff.Append('\\').Append(text[i]);
                                        break;

                                    case '\\': // escape
                                    case ':': // bold? or italic
                                    case '*': // bold? or italic
                                    case '~': // strikethrough?
                                    case '_': // underline?
                                    case '%': // color?
                                        buff.Append(text[i]);
                                        break;
                                }
                            }
                            else
                                buff.Append('\\');

                            break;

                        case ':': // emoji?
                            {
                                var nxtI = text.IndexOf(':', i + 1);
                                if (nxtI != -1 && EmojiTable.TryGet(text.Substring(i + 1, nxtI - i - 1), out var emoji))
                                {
                                    buff.Append(emoji);
                                    i = nxtI;
                                }
                                else buff.Append(':');
                                break;
                            }

                        case '*': // bold? or italic
                            {
                                var oldI = i;
                                var inline = ParseAsBoldOrItalic(text, ref i);
                                if (inline == null)
                                {
                                    buff.Append(text, oldI, i - oldI + 1);
                                }
                                else
                                {
                                    HandleBefore();
                                    rtn.Add(inline);
                                }
                                break;
                            }

                        case '~': // strikethrough?
                            {
                                var oldI = i;
                                var inline = ParseAsStrikethrough(text, ref i);
                                if (inline == null)
                                {
                                    buff.Append(text, oldI, i - oldI + 1);
                                }
                                else
                                {
                                    HandleBefore();
                                    rtn.Add(inline);
                                }
                                break;
                            }

                        case '_': // underline?
                            {
                                var oldI = i;
                                var inline = ParseAsUnderline(text, ref i);
                                if (inline == null)
                                {
                                    buff.Append(text, oldI, i - oldI + 1);
                                }
                                else
                                {
                                    HandleBefore();
                                    rtn.Add(inline);
                                }
                                break;
                            }

                        case '%': // color?
                            {
                                var oldI = i;
                                var inline = ParseAsColor(text, ref i);
                                if (inline == null)
                                {
                                    buff.Append(text, oldI, i - oldI + 1);
                                }
                                else
                                {
                                    HandleBefore();
                                    rtn.Add(inline);
                                }
                                break;
                            }
                    }
                }

                if (buff.Length > 0)
                {
                    rtn.AddRange(defaultHandler(buff.ToString()));
                }

                return rtn;
            }
        }

        private Inline ParseAsUnderline(string text, ref int start)
        {
            var bgnCnt = CountRepeat(text, start, '_');

            int last = EscapedIndexOf(text, start + bgnCnt, '_');

            int endCnt = last >= 0 ? CountRepeat(text, last, '_') : -1;

            if (endCnt >= 2 && bgnCnt >= 2)
            {
                int cnt = 2;
                int bgn = start + cnt;
                int end = last;

                start = end + cnt - 1;
                var span = Create<Underline, Inline>(RunSpanGamut(text.Substring(bgn, end - bgn)));
                if (!DisabledTag)
                {
                    span.Tag = TagUnderlineSpan;
                }
                return span;
            }
            else
            {
                start += bgnCnt - 1;
                return null;
            }
        }

        private Inline ParseAsStrikethrough(string text, ref int start)
        {
            var bgnCnt = CountRepeat(text, start, '~');

            int last = EscapedIndexOf(text, start + bgnCnt, '~');

            int endCnt = last >= 0 ? CountRepeat(text, last, '~') : -1;

            if (endCnt >= 2 && bgnCnt >= 2)
            {
                int cnt = 2;
                int bgn = start + cnt;
                int end = last;

                start = end + cnt - 1;
                var span = Create<Span, Inline>(RunSpanGamut(text.Substring(bgn, end - bgn)));
                span.TextDecorations = TextDecorations.Strikethrough;

                if (!DisabledTag)
                {
                    span.Tag = TagStrikethroughSpan;
                }
                return span;
            }
            else
            {
                start += bgnCnt - 1;
                return null;
            }
        }

        private Inline ParseAsBoldOrItalic(string text, ref int start)
        {
            // count asterisk (bgn)
            var bgnCnt = CountRepeat(text, start, '*');

            int last = EscapedIndexOf(text, start + bgnCnt, '*');

            int endCnt = last >= 0 ? CountRepeat(text, last, '*') : -1;

            if (endCnt >= 1)
            {
                int cnt = Math.Min(bgnCnt, endCnt);
                int bgn = start + cnt;
                int end = last;

                switch (cnt)
                {
                    case 1: // italic
                        {
                            start = end + cnt - 1;

                            var span = Create<Italic, Inline>(RunSpanGamut(text.Substring(bgn, end - bgn)));
                            if (!DisabledTag)
                            {
                                span.Tag = TagItalicSpan;
                            }
                            return span;
                        }
                    case 2: // bold
                        {
                            start = end + cnt - 1;
                            var span = Create<Bold, Inline>(RunSpanGamut(text.Substring(bgn, end - bgn)));
                            if (!DisabledTag)
                            {
                                span.Tag = TagBoldSpan;
                            }
                            return span;
                        }

                    default: // >3; bold-italic
                        {
                            bgn = start + 3;
                            start = end + 3 - 1;

                            var inline = Create<Italic, Inline>(RunSpanGamut(text.Substring(bgn, end - bgn)));
                            if (!DisabledTag)
                            {
                                inline.Tag = TagItalicSpan;
                            }

                            var span = new Bold(inline);
                            if (!DisabledTag)
                            {
                                span.Tag = TagBoldSpan;
                            }
                            return span;
                        }
                }
            }
            else
            {
                start += bgnCnt - 1;
                return null;
            }
        }

        private Inline ParseAsColor(string text, ref int start)
        {
            var mch = _color.Match(text, start);

            if (mch.Success && start == mch.Index)
            {
                int bgnIdx = start + mch.Value.Length;
                int endIdx = EscapedIndexOf(text, bgnIdx, '%');

                Span span;
                if (endIdx == -1)
                {
                    endIdx = text.Length - 1;
                    span = Create<Span, Inline>(
                        RunSpanGamut(text.Substring(bgnIdx)));
                }
                else
                {
                    span = Create<Span, Inline>(
                        RunSpanGamut(text.Substring(bgnIdx, endIdx - bgnIdx)));
                }

                var colorLbl = mch.Groups[1].Value;

                try
                {
                    var color = colorLbl.StartsWith("#") ?
                        (SolidColorBrush)new BrushConverter().ConvertFrom(colorLbl) :
                        (SolidColorBrush)new BrushConverter().ConvertFromString(colorLbl);

                    span.Foreground = color;
                }
                catch { }

                start = endIdx;
                return span;
            }
            else return null;
        }


        private int EscapedIndexOf(string text, int start, char target)
        {
            for (var i = start; i < text.Length; ++i)
            {
                var ch = text[i];
                if (ch == '\\') ++i;
                else if (ch == target) return i;
            }
            return -1;
        }
        private int CountRepeat(string text, int start, char target)
        {
            var count = 0;

            for (var i = start; i < text.Length; ++i)
            {
                if (text[i] == target) ++count;
                else break;
            }

            return count;
        }


        private Inline ItalicEvaluator(Match match, int contentGroup)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var content = match.Groups[contentGroup].Value;
            var span = Create<Italic, Inline>(RunSpanGamut(content));
            if (!DisabledTag)
            {
                span.Tag = TagItalicSpan;
            }
            return span;
        }

        private Inline BoldEvaluator(Match match, int contentGroup)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var content = match.Groups[contentGroup].Value;
            var span = Create<Bold, Inline>(RunSpanGamut(content));
            if (!DisabledTag)
            {
                span.Tag = TagBoldSpan;
            }
            return span;
        }

        private Inline StrikethroughEvaluator(Match match, int contentGroup)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var content = match.Groups[contentGroup].Value;

            var span = Create<Span, Inline>(RunSpanGamut(content));
            span.TextDecorations = TextDecorations.Strikethrough;
            if (!DisabledTag)
            {
                span.Tag = TagStrikethroughSpan;
            }
            return span;
        }

        private Inline UnderlineEvaluator(Match match, int contentGroup)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            var content = match.Groups[contentGroup].Value;
            var span = Create<Underline, Inline>(RunSpanGamut(content));
            if (!DisabledTag)
            {
                span.Tag = TagUnderlineSpan;
            }
            return span;
        }

        #endregion


        #region grammer - text

        private static Regex _eoln = new Regex("\\s+");
        private static Regex _lbrk = new Regex(@"\ {2,}\n");

        public IEnumerable<Inline> DoText(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var lines = _lbrk.Split(text);
            bool first = true;
            foreach (var line in lines)
            {
                if (first)
                    first = false;
                else
                    yield return new LineBreak();
                var t = _eoln.Replace(line, " ");
                yield return new Run(t);
            }
        }

        #endregion

        #region grammer - blockquote

        private static Regex _blockquote = new Regex(@"
            (?<=\n)
            [\n]*
            ([>].*)
            (\n[>].*)*
            [\n]*
            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private static Regex _blockquoteFirst = new Regex(@"
            ^
            ([>].*)
            (\n[>].*)*
            [\n]*
            ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        private IEnumerable<Block> DoBlockquotes(string text, Func<string, IEnumerable<Block>> defaultHandler)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Evaluate(
                text, _blockquoteFirst, BlockquotesEvaluator,
                sn => Evaluate(sn, _blockquote, BlockquotesEvaluator, defaultHandler)
            );
        }

        private Section BlockquotesEvaluator(Match match)
        {
            if (match is null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            // trim '>'
            var trimmedTxt = string.Join(
                    "\n",
                    match.Value.Trim().Split('\n')
                        .Select(txt =>
                        {
                            if (txt.Length <= 1) return string.Empty;
                            var trimmed = txt.Substring(1);
                            if (trimmed.FirstOrDefault() == ' ') trimmed = trimmed.Substring(1);
                            return trimmed;
                        })
                        .ToArray()
            );

            var blocks = RunBlockGamut(TextUtil.Normalize(trimmedTxt), true);
            var result = Create<Section, Block>(blocks);
            if (BlockquoteStyle != null)
            {
                result.Style = BlockquoteStyle;
            }
            if (!DisabledTag)
            {
                result.Tag = TagBlockquote;
            }

            return result;
        }


        #endregion

        #region helper - make regex

        private static string _nestedBracketsPattern;

        /// <summary>
        /// Reusable pattern to match balanced [brackets]. See Friedl's 
        /// "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string GetNestedBracketsPattern()
        {
            // in other words [this] and [this[also]] and [this[also[too]]]
            // up to _nestDepth
            if (_nestedBracketsPattern is null)
                _nestedBracketsPattern =
                    RepeatString(@"
                    (?>              # Atomic matching
                       [^\[\]]+      # Anything other than brackets
                     |
                       \[
                           ", _nestDepth) + RepeatString(
                    @" \]
                    )*"
                    , _nestDepth);
            return _nestedBracketsPattern;
        }

        private static string _nestedParensPattern;

        /// <summary>
        /// Reusable pattern to match balanced (parens). See Friedl's 
        /// "Mastering Regular Expressions", 2nd Ed., pp. 328-331.
        /// </summary>
        private static string GetNestedParensPattern()
        {
            // in other words (this) and (this(also)) and (this(also(too)))
            // up to _nestDepth
            if (_nestedParensPattern is null)
                _nestedParensPattern =
                    RepeatString(@"
                    (?>              # Atomic matching
                       [^()\n\t]+? # Anything other than parens or whitespace
                     |
                       \(
                           ", _nestDepth) + RepeatString(
                    @" \)
                    )*?"
                    , _nestDepth);
            return _nestedParensPattern;
        }

        /// <summary>
        /// this is to emulate what's evailable in PHP
        /// </summary>
        private static string RepeatString(string text, int count)
        {
            if (text is null)
            {
                throw new NullReferenceException(nameof(text));
            }

            var sb = new StringBuilder(text.Length * count);
            for (int i = 0; i < count; i++)
                sb.Append(text);
            return sb.ToString();
        }

        #endregion


        #region helper - parse

        private TResult Create<TResult, TContent>(IEnumerable<TContent> content)
            where TResult : IAddChild, new()
        {
            var result = new TResult();
            foreach (var c in content)
            {
                result.AddChild(c);
            }

            return result;
        }

        private IEnumerable<T> Evaluate2<T>(
                string text,
                Regex expression1, Func<Match, T> build1,
                Regex expression2, Func<Match, IEnumerable<T>> build2,
                Func<string, IEnumerable<T>> rest)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var index = 0;

            var rtn = new List<T>();

            var match1 = expression1.Match(text, index);
            var match2 = expression2.Match(text, index);

            IEnumerable<T> ProcPre(Match m)
            {
                var prefix = text.Substring(index, m.Index - index);
                return rest(prefix);
            }

            void ProcessMatch1()
            {
                if (match1.Index > index)
                {
                    rtn.AddRange(ProcPre(match1));
                }
                rtn.Add(build1(match1));
                index = match1.Index + match1.Length;
            }

            void ProcessMatch2()
            {
                if (match2.Index > index)
                {
                    rtn.AddRange(ProcPre(match2));
                }
                rtn.AddRange(build2(match2));
                index = match2.Index + match2.Length;
            }

            // match1 vs match2
            while (match1.Success && match2.Success)
            {
                if (match1.Index < match2.Index)
                {
                    ProcessMatch1();
                }
                else
                {
                    ProcessMatch2();
                }
                match1 = expression1.Match(text, index);
                match2 = expression2.Match(text, index);
            }

            while (match1.Success)
            {
                ProcessMatch1();
                match1 = expression1.Match(text, index);
            }

            while (match2.Success)
            {
                ProcessMatch2();
                match2 = expression2.Match(text, index);
            }

            if (index < text.Length)
            {
                var suffix = text.Substring(index, text.Length - index);
                rtn.AddRange(rest(suffix));
            }

            return rtn;
        }

        private IEnumerable<T> Evaluate<T>(string text, Regex expression, Func<Match, T> build, Func<string, IEnumerable<T>> rest)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var matches = expression.Matches(text);
            var index = 0;
            foreach (Match m in matches)
            {
                if (m.Index > index)
                {
                    var prefix = text.Substring(index, m.Index - index);
                    foreach (var t in rest(prefix))
                    {
                        yield return t;
                    }
                }

                yield return build(m);

                index = m.Index + m.Length;
            }

            if (index < text.Length)
            {
                var suffix = text.Substring(index, text.Length - index);
                foreach (var t in rest(suffix))
                {
                    yield return t;
                }
            }
        }

        #endregion
    }
}