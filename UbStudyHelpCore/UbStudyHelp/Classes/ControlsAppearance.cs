using ControlzEx.Theming;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UbStandardObjects;

namespace UbStudyHelp.Classes
{

    public static class EnumerationExtension
    {
        public static string Description(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetField(value.ToString());
            var attributes = field.GetCustomAttributes(false);

            // Description is in a hidden Attribute class called DisplayAttribute
            // Not to be confused with DisplayNameAttribute
            dynamic displayAttribute = null;

            if (attributes.Any())
            {
                displayAttribute = attributes.ElementAt(0);
            }

            // return description
            return displayAttribute?.Description ?? "na";
        }
    }

    /// <summary>
    /// Used to change information about controls and web page design information
    /// <see href="https://mahapps.com/docs/themes/thememanager"/>
    /// </summary>
    public class ControlsAppearance
    {
        /// <summary>
        /// Returns font for a block object (family, size and foregroung
        /// </summary>
        public Style ForegroundStyle
        {
            get
            {
                Style style = new Style
                {
                    TargetType = typeof(System.Windows.Documents.Block)
                };

                style.Setters.Add(new Setter(System.Windows.Documents.Block.FontFamilyProperty, new FontFamily(StaticObjects.Parameters.FontFamilyInfo)));
                style.Setters.Add(new Setter(System.Windows.Documents.Block.FontSizeProperty, StaticObjects.Parameters.FontSizeInfo));
                style.Setters.Add(new Setter(System.Windows.Documents.Block.ForegroundProperty, App.Appearance.GetForegroundColorBrush()));
                return style;
            }
        }


        public ControlsAppearance()
        {

        }


        public string GetBackgroundColor()
        {
            return Convert.ToString(Application.Current.FindResource("MahApps.Colors.ThemeBackground"));
        }

        public string GetForegroundColor()
        {
            return Convert.ToString(Application.Current.FindResource("MahApps.Colors.ThemeForeground"));
        }

        public string GetHighlightColor()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return Convert.ToString(Application.Current.FindResource("MahApps.Colors.Highlight"));
        }





        public enum MahColorNames
        {
            Highlight,
            AccentBase,
            Accent,
            Accent2,
            Accent3,
            Accent4,
            ThemeForeground,
            ThemeBackground,
            IdealForeground,
            Gray1,
            Gray2,
            Gray3,
            Gray4,
            Gray5,
            Gray6,
            Gray7,
            Gray8,
            Gray9,
            Gray10,
            Gray,
            [Description("Gray.MouseOver")]
            GrayMouseOver,
            [Description("Gray.SemiTransparent")]
            GraySemiTransparent,
            Flyout,
            ProgressIndeterminate1,
            ProgressIndeterminate2,
            ProgressIndeterminate3,
            ProgressIndeterminate4
        }

        public string GetColor(MahColorNames color)
        {
            string colorStr = color.Description() == "na" ? color.ToString() : color.Description();
            return Convert.ToString(Application.Current.FindResource($"MahApps.Colors.{colorStr}"));
        }


        public string GetGrayColor(int grayColorNumber= 1)
        {
            // https://mahapps.com/docs/themes/thememanager
            return Convert.ToString(Application.Current.FindResource($"MahApps.Colors.Gray{grayColorNumber}"));
        }

        public Brush GetGrayColorBrush(int grayColorNumber = 1)
        {
            return (SolidColorBrush)Application.Current.FindResource($"MahApps.Colors.Gray{grayColorNumber}");
        }


        public Brush GetBackgroundColorBrush()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return (SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.ThemeBackground");
        }

        public Brush GetForegroundColorBrush()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return (SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.ThemeForeground");
        }


        public Brush GetHighlightColorBrush()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return (SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.WindowTitle");
        }


        public Brush GetGrayInativeColorBrush()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return (SolidColorBrush)Application.Current.FindResource("MahApps.Brushes.WindowTitle.NonActive");
        }


        public void SetThemeInfo(Control control, bool reverse= false)
        {
            Style style = new Style
            {
                TargetType = typeof(Control)
            };
            
            style.Setters.Add(new Setter(Control.FontFamilyProperty, new FontFamily(StaticObjects.Parameters.FontFamilyInfo)));
            style.Setters.Add(new Setter(Control.FontSizeProperty, StaticObjects.Parameters.FontSizeInfo));
            if (!(control is ComboBox || control is ListView))
            {
                style.Setters.Add(new Setter(Control.BackgroundProperty, reverse ? GetHighlightColorBrush() : GetBackgroundColorBrush()));
                style.Setters.Add(new Setter(Control.ForegroundProperty, reverse ? GetBackgroundColorBrush() : GetForegroundColorBrush()));
            }
            control.Style = style;
        }

        public void SetThemeInfo(TextBlock control, bool reverse = false)
        {

            Style style = new Style
            {
                TargetType = typeof(TextBlock)
            };

            style.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(StaticObjects.Parameters.FontFamilyInfo)));
            style.Setters.Add(new Setter(TextBlock.FontSizeProperty, StaticObjects.Parameters.FontSizeInfo));

            style.Setters.Add(new Setter(TextBlock.BackgroundProperty, reverse ? GetHighlightColorBrush() : GetBackgroundColorBrush()));
            style.Setters.Add(new Setter(TextBlock.ForegroundProperty, reverse ? GetBackgroundColorBrush() : GetForegroundColorBrush()));
            control.Style = style;
        }


        public void SetFontSize(Control control)
        {
            Style style = new Style
            {
                TargetType = typeof(Control)
            };
            style.Setters.Add(new Setter(Control.FontSizeProperty, StaticObjects.Parameters.FontSizeInfo));
            control.Style = style;
        }

        public void SetFontSize(TextBlock control)
        {
            control.FontSize = StaticObjects.Parameters.FontSizeInfo;
        }

        public void SetHeight(Control control)
        {
            double delta = 10;
            control.Height = StaticObjects.Parameters.FontSizeInfo + delta;
        }

        public string ThemeColor
        {
            get
            {
                Theme theme = ThemeManager.Current.DetectTheme();
                char[] sep = { '.' };
                return theme.Name.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            set
            {
                ((ParametersCore)StaticObjects.Parameters).ThemeColor = value;
                ThemeManager.Current.ChangeTheme(Application.Current, $"{Theme}.{value}");
                EventsControl.FireAppearanceChanged();
            }
        }

        public string Theme
        {
            get
            {
                Theme theme = ThemeManager.Current.DetectTheme();
                char[] sep = { '.' };
                return theme.Name.Split(sep)[0];
            }
            set
            {
                ((ParametersCore)StaticObjects.Parameters).ThemeName = value;
                ThemeManager.Current.ChangeTheme(Application.Current, $"{value}.{((ParametersCore)StaticObjects.Parameters).ThemeColor}");
                EventsControl.FireAppearanceChanged();
            }
        }

    }
}
