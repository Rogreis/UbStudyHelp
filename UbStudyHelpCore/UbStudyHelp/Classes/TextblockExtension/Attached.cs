using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UbStudyHelp.Classes
{
    public class Attached
    {
        public static IEnumerable<Inline> GetInlines(DependencyObject d)
        {
            return (IEnumerable<Inline>)d.GetValue(InlinesProperty);
        }

        public static void SetInlines(DependencyObject d, IEnumerable<Inline> value)
        {
            d.SetValue(InlinesProperty, value);
        }

        // Using a DependencyProperty as the backing store for Inlines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.RegisterAttached("Inlines", typeof(IEnumerable<Inline>), typeof(Attached),
                new FrameworkPropertyMetadata(OnInlinesPropertyChanged));

        private static void OnInlinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }
            if (e.NewValue == null)
            {
                return;
            }
            var inlinesCollection = textBlock.Inlines;
            inlinesCollection.Clear();
            inlinesCollection.AddRange((IEnumerable<Inline>)e.NewValue);
        }
    }
}
