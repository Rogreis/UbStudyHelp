using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using UbStandardObjects;
using UbStandardObjects.Objects;
using UbStudyHelp.Classes;
using UbStudyHelp.Controls;
using Paragraph = System.Windows.Documents.Paragraph;

namespace UbStudyHelp.ChildWindows
{
    /// <summary>
    /// Interaction logic for EditText.xaml
    /// </summary>
    public partial class EditText : Window
    {

        private ParagraphMarkDown EditedParagraph = null;

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
            App.Appearance.SetFontSize(commitMessage);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);

            GitHubUser.Text= StaticObjects.Parameters.GitAuthorName;
            GitHubEmail.Text = StaticObjects.Parameters.GitEmail;
            commitMessage.Text = StaticObjects.Parameters.GitCommitMessage;
            GithubPassword.Text = StaticObjects.Parameters.GitPassword;
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
            App.Appearance.SetFontSize(commitMessage);
            App.Appearance.SetFontSize(buttonWorking);
            App.Appearance.SetFontSize(buttonOk);
            App.Appearance.SetFontSize(buttonDoubt);
            App.Appearance.SetFontSize(buttonClosed);
        }

        public void SetText(TOC_Entry entry)
        {
            richTextBoxEdit.Document.Blocks.Clear();
            Paper paper = StaticObjects.Book.RightTranslation.Paper(entry.Paper);
            EditedParagraph = (ParagraphMarkDown)paper.GetParagraph(entry);
            richTextBoxEdit.Document.Blocks.Add(new System.Windows.Documents.Paragraph(new Run(EditedParagraph.Text)));

            string url = $"https://github.com/Rogreis/PtAlternative/blob/correcoes/Doc{entry.Paper:000}/Par_{entry.Paper:000}_{entry.Section:000}_{entry.ParagraphNo:000}.md";
            LinkToGitHub.Inlines.Clear();
            Run runIdent = new Run("Edit in github");
            Hyperlink link = new Hyperlink(runIdent)
            {
                NavigateUri = new Uri(url),
                TextDecorations = TextDecorations.Underline,
                Tag = entry,
            };
            link.Click += Link_Click;
            LinkToGitHub.Inlines.Add(link);
        }

        private void Link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        #region Data save

        private string RichtextToHtml()
        {
            string markdown = "";
            foreach(Block block in richTextBoxEdit.Document.Blocks)
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
                EditedParagraph.Text= RichtextToHtml();
                EditedParagraph.SaveText(StaticObjects.Parameters.EditParagraphsRepositoryFolder);
                Note note = Notes.GetNote(EditedParagraph);
                note.Status = (short)status;
                Notes.SaveNotes(EditedParagraph);

                List<string> relativeFilesList= new List<string>();
                relativeFilesList.Add(ParagraphMarkDown.RelativeFilePathWindows(EditedParagraph));
                relativeFilesList.Add(Notes.RelativeNotesPath(EditedParagraph.Paper));

                string gitHubUser = GitHubUser.Text;
                string email = GitHubEmail.Text;
                string message = commitMessage.Text;
                string password = commitMessage.Text;

                StaticObjects.Parameters.GitAuthorName= gitHubUser;
                StaticObjects.Parameters.GitEmail= email;
                StaticObjects.Parameters.GitCommitMessage= message;
                StaticObjects.Parameters.GitPassword= password;

                if (string.IsNullOrWhiteSpace(gitHubUser) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Please fill author name, email and commit message");
                    return;   
                }

                //if (!GitCommands.CommitFiles(gitHubUser, email, relativeFilesList, StaticObjects.Parameters.EditParagraphsRepositoryFolder, message))
                //{
                //    MessageBox.Show("Paragraph commit data error");
                //    return;
                //}
                ////if (!GitCommands.Push(StaticObjects.Parameters.EditParagraphsRepositoryFolder, gitHubUser, password))
                //{
                //    MessageBox.Show("Paragraph push data error");
                //    return;
                //}
                SearchDataEntry.LuceneBookSearchRight.UpdateIndex(EditedParagraph);
                EventsControl.FireRefreshText();
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
