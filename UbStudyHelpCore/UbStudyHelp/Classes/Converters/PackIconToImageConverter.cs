using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace UbStudyHelp.Classes.Converters
{
    public class PackIconToImageConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the thickness to draw the icon with.
        /// </summary>
        public double Thickness { get; set; } = 0.25;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            PackIconFontAwesome icon = new PackIconFontAwesome();
            icon.Kind = PackIconFontAwesomeKind.AdjustSolid;

            Geometry geo = Geometry.Parse(icon.Data);
            GeometryDrawing geoDrawing = new GeometryDrawing();
            geoDrawing.Geometry = geo;
            geoDrawing.Brush = icon.BorderBrush;
            geoDrawing.Pen = new Pen(Brushes.White, 100);
            //geoDrawing.Brush = parameter as Brush ?? Brushes.Black;
            //geoDrawing.Pen = new Pen(geoDrawing.Brush, Thickness);


            //if (value is PackIconFontAwesome)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconBootstrapIcons>).Data);
            //else if (value is PackIconMaterial)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconMaterialKind>).Data);
            //else if (value is PackIconMaterialLight)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconEntypoKind>).Data);
            //else if (value is PackIconModern)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconMaterialLightKind>).Data);
            //else if (value is PackIconEntypo)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconEntypoKind>).Data);
            //else if (value is PackIconOcticons)
            //    geoDrawing.Geometry = Geometry.Parse((value as PackIcon<PackIconOcticonsKind>).Data);

            var drawingGroup = new DrawingGroup { Children = { geoDrawing }, Transform = new ScaleTransform(1, -1) };

            return new DrawingImage { Drawing = drawingGroup };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
