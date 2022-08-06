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
        Search,
        Update,
        Next,
        Previous,
        Notes
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
        private string DataSearch = "M796.885 102.102l-243.84 243.883c-108.476-77.12-257.945-58.257-343.857 43.396-85.912 101.649-79.604 252.172 14.512 346.278 94.091 94.146 244.63 100.482 346.301 14.574 101.666-85.907 120.542-235.393 43.418-343.875l243.84-243.883-60.373-60.373zM404.692 725.333c-80.909 0.018-150.712-56.776-167.148-135.997s25.013-159.098 99.252-191.268c74.238-32.166 160.864-7.791 207.426 58.377 46.566 66.168 40.269 155.936-15.070 214.958l25.813-25.6-29.611 29.525c-31.923 32.12-75.375 50.128-120.662 50.006z";
        private string DataUpdate = "M38.0642 154.0646C38.0642 93.18 82.2082 51.706 131.7786 43.122C137.451 42.14 141.2532 36.746 140.271 31.072C139.2888 25.4 133.8942 21.598 128.2218 22.58C69.9884 32.664 17.2168 81.746 17.2168 154.0646C17.21678 184.7982 31.1992 208.951 47.5278 227.3246C59.2248 240.4864 72.6068 251.173 83.3692 259.3596L50.641 259.3596C45.1182 259.3596 40.641 263.8368 40.641 269.3596C40.641 274.8824 45.1182 279.3596 50.641 279.3596L110.641 279.3596C116.1638 279.3596 120.641 274.8824 120.641 269.3596L120.641 209.3596C120.641 203.8368 116.1638 199.3596 110.641 199.3596C105.1182 199.3596 100.641 203.8368 100.641 209.3596L100.641 246.271L100.6108 246.2482L100.609 246.2468L100.6088 246.2466L100.6086 246.2466C89.1792 237.6264 75.2118 227.0924 63.1108 213.476C48.8204 197.3958 38.0642 177.9692 38.0642 154.0646zM260.218 145.9358C260.218 206.177 217.01 247.408 168.0768 256.5942C162.4186 257.6564 158.693 263.1042 159.7552 268.7624C160.8174 274.4204 166.2652 278.146 171.9232 277.0838C229.408 266.2918 281.064 217.479 281.064 145.9358C281.064 115.2024 267.082 91.05 250.754 72.676C239.056 59.514 225.674 48.828 214.912 40.64L247.642 40.64C253.164 40.64 257.642 36.164 257.642 30.64C257.642 25.118 253.164 20.64 247.642 20.64L187.641 20.64C182.1182 20.64 177.641 25.118 177.641 30.64L177.641 90.64C177.641 96.164 182.1182 100.6408 187.641 100.6408C193.1638 100.6408 197.641 96.164 197.641 90.64L197.641 53.73L197.6724 53.754C209.102 62.374 223.07 72.908 235.17 86.524C249.462 102.6048 260.218 122.0312 260.218 145.9358z";
        private string DataNext = "M576 640c0 -17 -7 -33 -19 -45l-448 -448c-12 -12 -28 -19 -45 -19c-35 0 -64 29 -64 64v896c0 35 29 64 64 64c17 0 33 -7 45 -19l448 -448c12 -12 19 -28 19 -45z";
        private string DataPrevious = "M576 1088v-896c0 -35 -29 -64 -64 -64c-17 0 -33 7 -45 19l-448 448c-12 12 -19 28 -19 45s7 33 19 45l448 448c12 12 28 19 45 19c35 0 64 -29 64 -64z";
        private string DataNotes = "M5 2h16v20H3V2h2zm14 18V4H5v16h14zM7 6h10v2H7V6zm10 4H7v2h10v-2zM7 14h7v2H7v-2z";

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
                case GeometryImagesTypes.Update:
                    data = DataUpdate;
                    break;
                case GeometryImagesTypes.Next:
                    data = DataNext;
                    break;
                case GeometryImagesTypes.Previous:
                    data = DataPrevious;
                    break;
                case GeometryImagesTypes.Notes:
                    data = DataNotes;
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
