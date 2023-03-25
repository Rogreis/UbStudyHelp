using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Controls;
using static Lucene.Net.Search.FieldValueHitQueue;
using Paragraph = System.Windows.Documents.Paragraph;

namespace UbStudyHelp.ChildWindows
{
    /// <summary>
    /// Interaction logic for EditText.xaml
    /// </summary>
    public partial class EditText : Window
    {

        private PaperEdit paper = null;
        // Used for specific formating 
        private FlowDocumentFormat format = new FlowDocumentFormat();
        private ParagraphSearchData paragraphSearchData= null;

        public EditText()
        {
            InitializeComponent();

            EventsControl.FontChanged += EventsControl_FontChanged;
            EventsControl.AppearanceChanged += EventsControl_AppearanceChanged;
            Loaded += EditText_Loaded;

            //  Background="{StaticResource ChildWindowsBackground}"
            // Access the StaticResource
            Brush brush = (Brush)this.Resources["ChildWindowsBackground"];

            // Set the background color of the Grid
            Background = brush;

        }

        private void EditText_Loaded(object sender, RoutedEventArgs e)
        {
            App.Appearance.SetFontSize(richTextBoxEdit);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);

        }

        private void EventsControl_AppearanceChanged(ControlsAppearance appearance)
        {
            //App.Appearance.SetThemeInfo(richTextBoxEdit);
            //App.Appearance.SetThemeInfo(commitMessage);
            //App.Appearance.SetThemeInfo(buttonWorking);
            //App.Appearance.SetThemeInfo(buttonOk);
            //App.Appearance.SetThemeInfo(buttonDoubt);
            //App.Appearance.SetThemeInfo(buttonClosed);
            //richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Blue);
            //richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void EventsControl_FontChanged(ControlsAppearance appearance)
        {
            App.Appearance.SetFontSize(richTextBoxEdit);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);
        }

        private void SetLink(TextBlock textBlock, TOC_Entry entry, string url, string text)
        {
            textBlock.Inlines.Clear();
            Run runIdent = new Run(text);
            Hyperlink link = new Hyperlink(runIdent)
            {
                NavigateUri = new Uri(url),
                TextDecorations = TextDecorations.Underline,
                Tag = entry,
            };
            link.Click += Link_Click;
            textBlock.Inlines.Add(link);
        }


        public void SetText(ParagraphSearchData searchData)
        {
            paragraphSearchData = searchData;
            richTextBoxEdit.Document.Blocks.Clear();
            paper = StaticObjects.Book.RightTranslation.GetPaperEdit(searchData.Entry.Paper);
            searchData.EditedParagraph = (ParagraphMarkDown)paper.GetParagraph(searchData.Entry);
            Note note = Notes.GetNote(paper.NotesList, searchData.EditedParagraph);

            FormatData data = new FormatData()
            {
                Entry = searchData.Entry,
                Words = searchData.Words,
                Status = searchData.EditedParagraph.Status,
                Highlighted = searchData.Highlighted,
                Text = searchData.EditedParagraph.Text,
            };
            format.FormatParagraph(data);

            richTextBoxEdit.Document.Blocks.Add(data.DocParagraph);
            richTextBoxEdit.Background = data.DocParagraph.Background;
            richTextBoxEdit.Foreground = data.DocParagraph.Foreground;

            string url = $"https://github.com/Rogreis/PtAlternative/blob/correcoes/Doc{data.Entry.Paper:000}/Par_{data.Entry.Paper:000}_{data.Entry.Section:000}_{data.Entry.ParagraphNo:000}.md";

            SetLink(LinkToGitHub, data.Entry, url, "Edit in github");
            url = StaticObjects.Parameters.EditParagraphsRepositoryFolder;
            SetLink(LinkToRepository, null, url, "Open Repository");

        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            if (link != null)
            {
                Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri) { UseShellExecute = true });
            }
            else
            {
                Process.Start(link.NavigateUri.AbsoluteUri);
            }
            e.Handled = true;
        }

        #region Data save

        private string RichtextToHtml()
        {
            string markdown = "";
            foreach (Block block in richTextBoxEdit.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run)
                        {
                            Run run = (Run)inline;
                            //if (run.FontWeight == FontWeights.Bold)
                            //{
                            //    markdown += "**" + run.Text + "**";
                            //}
                            if (run.FontStyle == FontStyles.Italic)
                            {
                                markdown += "*" + run.Text + "*";
                            }
                            else
                            {
                                markdown += run.Text;
                            }
                        }
                    }
                }
            }
            return markdown;
        }

        private void SaveData(ParagraphStatus status)
        {
            try
            {
                paragraphSearchData.EditedParagraph.Text = RichtextToHtml();
                paragraphSearchData.EditedParagraph.SaveText(StaticObjects.Parameters.EditParagraphsRepositoryFolder);
                Note note = Notes.GetNote(paper.NotesList, paragraphSearchData.EditedParagraph);
                note.Status = (short)status;
                Notes.SaveNotes(paper.NotesList, paragraphSearchData.EditedParagraph, note);

                List<string> relativeFilesList = new List<string>();
                relativeFilesList.Add(ParagraphMarkDown.RelativeFilePathWindows(paragraphSearchData.EditedParagraph));
                relativeFilesList.Add(Notes.RelativeNotesPath(paragraphSearchData.EditedParagraph.Paper));

                SearchDataEntry.LuceneBookSearchRight.UpdateIndex(paragraphSearchData.EditedParagraph);
                EventsControl.FireRefreshParagraphText(paragraphSearchData);
                Close();
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage("Paragraph save data error", ex);
                MessageBox.Show($"Paragraph save data error: {ex.Message}");
            }

        }


        #endregion

        #region Save Status & Data region buttons
        private void buttonWorking_Click(object sender, RoutedEventArgs e)
        {
            SaveData(ParagraphStatus.Working);
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SaveData(ParagraphStatus.Ok);
        }

        private void buttonDoubt_Click(object sender, RoutedEventArgs e)
        {
            SaveData(ParagraphStatus.Doubt);
        }

        private void buttonClosed_Click(object sender, RoutedEventArgs e)
        {
            SaveData(ParagraphStatus.Closed);
        }

        #endregion

        #region Italic
        private bool IsRichTextBoxSelectionItalic()
        {
            return richTextBoxEdit.Selection.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic);
        }

        private void RichtextBoxSelectionRemoveItalic()
        {
            richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
        }

        private void RichtextboxSetItalic()
        {
            richTextBoxEdit.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void buttonItalic_Click(object sender, RoutedEventArgs e)
        {
            if (IsRichTextBoxSelectionItalic())
            {
                RichtextBoxSelectionRemoveItalic();
                return;
            }
            else
            {
                RichtextboxSetItalic();
            }
        }

        #endregion    }

    }
}
