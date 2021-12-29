using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace UbStudyHelp.Classes
{
    public enum GeometryImagesTypes
    {
        FontIncrease,
        FontDecrease,
        Clear,
        Sort,
        Load,
        Save,
        Search
    }


    /// <summary>
    /// Converts a geometry data to image source
    /// </summary>
    public class GeometryImages
    {
        public string DataIncreaseFont = "M5.12,14L7.5,7.67L9.87,14M6.5,5L1,19H3.25L4.37,16H10.62L11.75,19H14L8.5,5H6.5M18,7L13,12.07L14.41,13.5L17,10.9V17H19V10.9L21.59,13.5L23,12.07L18,7Z";
        public string DataDecreaseFont = "M5.12,14L7.5,7.67L9.87,14M6.5,5L1,19H3.25L4.37,16H10.62L11.75,19H14L8.5,5H6.5M18,17L23,11.93L21.59,10.5L19,13.1V7H17V13.1L14.41,10.5L13,11.93L18,17Z";
        public string DataSort = "M19 17H22L18 21L14 17H17V3H19M11 13V15L7.67 19H11V21H5V19L8.33 15H5V13M9 3H7C5.9 3 5 3.9 5 5V11H7V9H9V11H11V5C11 3.9 10.11 3 9 3M9 7H7V5H9Z";
        public string DataClear = "M405 375l-119 -119l119 -119l-30 -30l-119 119l-119 -119l-30 30l119 119l-119 119l30 30l119 -119l119 119z";
        private string DataLoad = "M224 136V0H24C10.7 0 0 10.7 0 24v464c0 13.3 10.7 24 24 24h336c13.3 0 24-10.7 24-24V160H248c-13.2 0-24-10.8-24-24zm76.45 211.36l-96.42 95.7c-6.65 6.61-17.39 6.61-24.04 0l-96.42-95.7C73.42 337.29 80.54 320 94.82 320H160v-80c0-8.84 7.16-16 16-16h32c8.84 0 16 7.16 16 16v80h65.18c14.28 0 21.4 17.29 11.27 27.36zM377 105L279.1 7c-4.5-4.5-10.6-7-17-7H256v128h128v-6.1c0-6.3-2.5-12.4-7-16.9z";
        private string DataSave = "M224 136V0H24C10.7 0 0 10.7 0 24v464c0 13.3 10.7 24 24 24h336c13.3 0 24-10.7 24-24V160H248c-13.2 0-24-10.8-24-24zm65.18 216.01H224v80c0 8.84-7.16 16-16 16h-32c-8.84 0-16-7.16-16-16v-80H94.82c-14.28 0-21.41-17.29-11.27-27.36l96.42-95.7c6.65-6.61 17.39-6.61 24.04 0l96.42 95.7c10.15 10.07 3.03 27.36-11.25 27.36zM377 105L279.1 7c-4.5-4.5-10.6-7-17-7H256v128h128v-6.1c0-6.3-2.5-12.4-7-16.9z";
        //private string DataSearch = "M811.648 235.648c-20.046 19.222-39.063 38.469-57.552 58.216l-0.774 0.835c-15.872 16.128-25.429 27.861-25.429 27.861l-119.467 57.045c46.082 52.071 74.225 120.945 74.24 196.392v0.003c0 164.651-133.973 298.667-298.667 298.667s-298.667-134.016-298.667-298.667 133.973-298.667 298.667-298.667c75.221 0 143.787 28.16 196.395 74.197l57.045-119.467s11.733-9.557 27.861-25.429c16.512-15.488 38.229-36.437 59.051-58.325l57.941-59.392 25.771-27.563 90.496 90.496-27.563 25.771c-16.171 15.872-37.76 36.949-59.349 58.027zM384 362.667c-117.632 0-213.333 95.701-213.333 213.333s95.701 213.333 213.333 213.333 213.333-95.701 213.333-213.333-95.701-213.333-213.333-213.333z";
        private string DataSearch = "M796.885 102.102l-243.84 243.883c-108.476-77.12-257.945-58.257-343.857 43.396-85.912 101.649-79.604 252.172 14.512 346.278 94.091 94.146 244.63 100.482 346.301 14.574 101.666-85.907 120.542-235.393 43.418-343.875l243.84-243.883-60.373-60.373zM404.692 725.333c-80.909 0.018-150.712-56.776-167.148-135.997s25.013-159.098 99.252-191.268c74.238-32.166 160.864-7.791 207.426 58.377 46.566 66.168 40.269 155.936-15.070 214.958l25.813-25.6-29.611 29.525c-31.923 32.12-75.375 50.128-120.662 50.006z";

        // 
        /// <summary>
        /// Converts a geometry data to image source
        /// </summary>
        /// <param name="imagesTypes"></param>
        /// <returns></returns>
        public ImageSource GetImage(GeometryImagesTypes imagesTypes)
        {

            string data = "";
            switch(imagesTypes)
            {
                case GeometryImagesTypes.FontIncrease:
                    data = DataIncreaseFont;
                    break;
                case GeometryImagesTypes.FontDecrease:
                    data = DataDecreaseFont;
                    break;
                case GeometryImagesTypes.Clear:
                    data = DataClear;
                    break;
                case GeometryImagesTypes.Sort:
                    data = DataSort;
                    break;
                case GeometryImagesTypes.Load:
                    data = DataLoad;
                    break;
                case GeometryImagesTypes.Save:
                    data = DataSave;
                    break;
                case GeometryImagesTypes.Search:
                    data = DataSearch;
                    break;
            }

            var aGeometryDrawing = new GeometryDrawing
            {
                Geometry = Geometry.Parse(data),
                // Paint the drawing with the current backgroud color
                Brush = App.Appearance.GetHighlightColorBrush()
            };


            //aGeometryDrawing.Brush = App.Appearance.GetBackgroundColorBrush();

            // Outline the drawing with a solid color.
            //aGeometryDrawing.Pen = new Pen(App.Appearance.GetBackgroundColorBrush(), 10);

            //
            // Use a DrawingImage and an Image control
            // to display the drawing.
            //
            DrawingImage geometryImage = new DrawingImage(aGeometryDrawing);

            // Freeze the DrawingImage for performance benefits.
            geometryImage.Freeze();

            return geometryImage;

        }


    }
}
