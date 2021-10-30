using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using UbStudyHelp.Properties;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Used to change information about controls and web page design information
    /// <see href="https://mahapps.com/docs/themes/thememanager"/>
    /// </summary>
    public class ControlsAppearance
    {

        public string FontFamilyInfo { get; set; } = "";

        public double FontSizeInfo { get; set; } = 20.0;

        public Color BackgroundColor { get; set; } = Color.FromRgb(255, 255, 255);

        public Color ForegroundColor { get; set; } = Color.FromRgb(0, 0, 0);


        public ControlsAppearance()
        {

        }


        public string GetAccentColor()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return Convert.ToString(Application.Current.FindResource("MahApps.Colors.AccentBase"));
        }

        public string GetAccent2Color()
        {
            // https://mahapps.com/docs/themes/thememanager
            //Get a resource from the ResourceDictionary in code
            return Convert.ToString(Application.Current.FindResource("MahApps.Colors.Accent2"));
        }


        public ControlsAppearance(Control control)
        {
            // Here we only get the first non empty value
            foreach(XmlLanguage key in control.FontFamily.FamilyNames.Keys)
            {
                string fonteName = "";
                control.FontFamily.FamilyNames.TryGetValue(key, out fonteName);
                if (!string.IsNullOrEmpty(fonteName))
                {
                    FontFamilyInfo = fonteName;
                    break;
                }
            }

            // points = pixels * 72 / 96
            // WPF converts points to pixels with the System.Windows.FontSizeConverter.
            // The FontSizeConverter uses the System.Windows.LengthConverter.
            // The LengthConverter uses the factor 1.333333333333333333 to convert from points (p) to pixels (x): x = p * 1.3333333333333333

            //FontSizeConverter fsc = new FontSizeConverter();
            FontSizeInfo = control.FontSize;
            SolidColorBrush b = control.Background as SolidColorBrush;
            BackgroundColor = b.Color;
            SolidColorBrush f = control.Foreground as SolidColorBrush;
            ForegroundColor = f.Color;
        }

        public void SetAll(Control control)
        {

            Style style = new Style
            {
                TargetType = typeof(Control)
            };

            if (!string.IsNullOrEmpty(FontFamilyInfo))
            {
                style.Setters.Add(new Setter(Control.FontFamilyProperty, new FontFamily(FontFamilyInfo)));
            }

            style.Setters.Add(new Setter(Control.FontSizeProperty, FontSizeInfo));
            style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(BackgroundColor)));
            style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(ForegroundColor)));

            control.Style = style;

        }

        public void SetFontSize(Control control)
        {
            Style style = new Style
            {
                TargetType = typeof(Control)
            };
            style.Setters.Add(new Setter(Control.FontSizeProperty, FontSizeInfo));
            control.Style = style;
        }

        public void SetFontSize(TextBlock control)
        {
            control.FontSize = FontSizeInfo;
        }

        public void SetHeight(Control control)
        {
            double delta = 10;
            control.Height = FontSizeInfo + delta;
        }


    }
}
