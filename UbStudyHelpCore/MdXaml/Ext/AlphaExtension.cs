﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

#if MIG_FREE
namespace Markdown.Xaml.Ext
#else
namespace MdXaml.Ext
#endif
{
    class AlphaExtension : MarkupExtension
    {
        private readonly string key;
        private readonly float power;

        public Type TargetType { set; get; }

        public AlphaExtension(string key) : this(key, 1f) { }

        public AlphaExtension(string key, float power)
        {
            this.key = key;
            this.power = power;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding(key)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = TargetType },
                Converter = new AlphaConverter(power)
            };
        }

        class AlphaConverter : IValueConverter
        {
            readonly float Power;

            public AlphaConverter(float power)
            {
                this.Power = power;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Color baseColor;

                if (value is SolidColorBrush cBrush)
                {
                    baseColor = cBrush.Color;
                }
                else
                {
                    baseColor = Colors.Black;
                }

                var srcAlpha = baseColor.A / 255f;
                var newAlpha = srcAlpha * Power;

                var newColor = Color.FromArgb((byte)(255 * newAlpha), baseColor.R, baseColor.G, baseColor.B);

                return new SolidColorBrush(newColor);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
