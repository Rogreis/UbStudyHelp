using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// TextBlock control in WPF has Inlines property that isn't a DependencyProperty. 
    /// That makes it impossible to bind it directly to the view model. 
    /// To make it possible, an attached property and some value converters can come to the rescue.
    /// </summary>
    public class StringToInlinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var text = value as string;
            if (text == null)
            {
                return null;
            }
            var words = GetWords(text);
            return GetInlines(words);
        }

        private static Hyperlink LinkFromReference(string line)
        {
            string reference = line.Replace(':', ';');
            reference = reference.Replace('.', ';');

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetHighlightColor());

            Run run = new Run(line)
            {
                Foreground = accentBrush
            };

            Hyperlink hyperlink = new Hyperlink(run)
            {
                NavigateUri = new Uri("about:blank"),
                TextDecorations = null
            };
            hyperlink.Tag = reference;
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            hyperlink.MouseEnter += Hyperlink_MouseEnter;
            hyperlink.MouseLeave += Hyperlink_MouseLeave;
            return hyperlink;
        }


        private static List<Inline> GetInlines(List<string> words)
        {
            List<Inline> Inlines = new List<Inline>();
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    Inlines.Add(LinkFromReference(word));
                    if (words.Last() != word)
                    {
                        Inlines.Add(new LineBreak());
                    }
                }
            }
            return Inlines; // words.Select(x => new Run(x));
        }

        private static List<string> GetWords(string text)
        {
            return text.Split(' ').ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void Hyperlink_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = null;
        }

        private static void Hyperlink_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            hyperlink.TextDecorations = TextDecorations.Underline;
        }


        private static void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            e.Handled = true;
            string reference = (string)hyperlink.Tag;
            char[] separators = { ';' };
            string[] parts = reference.Split(separators);

            short Paper = -1;
            short.TryParse(parts[0], out Paper);
            short Section = -1;
            short.TryParse(parts[1], out Section);
            short Paragraph = -1;
            short.TryParse(parts[2], out Paragraph);

            TOC_Entry entry = new TOC_Entry(Paper, Section, Paragraph);
            EventsControl.FireIndexClicked(entry);

            SolidColorBrush accentBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(App.Appearance.GetGrayColor(2));
            var run = hyperlink.Inlines.FirstOrDefault() as Run;
            if (run != null)
            {
                //run.Foreground= System.Windows.Media.Brushes.Red;
                run.Foreground = accentBrush;

            }
            hyperlink.Foreground = accentBrush;
        }


    }
}


