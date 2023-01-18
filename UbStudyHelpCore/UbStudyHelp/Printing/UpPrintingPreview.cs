using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using UbStandardObjects;

namespace UbStudyHelp.Printing
{



    // https://stackoverflow.com/questions/584551/how-to-print-preview-when-using-a-documentpaginator-to-print
    // https://stackoverflow.com/questions/2322064/how-can-i-produce-a-print-preview-of-a-flowdocument-in-a-wpf-application
    // http://www.nullskull.com/a/1378/wpf-printing-and-print-preview.aspx

    //internal class RowPaginator : DocumentPaginator
    //{
    //    private int rows;
    //    private Size pageSize;
    //    private int rowsPerPage;

    //    public RowPaginator(int rows)
    //    {
    //        this.rows = rows;
    //    }

    //    public override DocumentPage GetPage(int pageNumber)
    //    {
    //        int currentRow = rowsPerPage * pageNumber;
    //        int rowsToPrint = Math.Min(rowsPerPage, rows - (rowsPerPage * pageNumber - 1));
    //        var page = new PageElementRenderer(pageNumber + 1, PageCount, currentRow, rowsToPrint)
    //        {
    //            Width = PageSize.Width,
    //            Height = PageSize.Height
    //        };
    //        page.Measure(PageSize);
    //        page.Arrange(new Rect(new Point(0, 0), PageSize));
    //        return new DocumentPage(page);
    //    }

    //    public override bool IsPageCountValid { get { return true; } }

    //    public override int PageCount { get { return (int)Math.Ceiling(this.rows / (double)this.rowsPerPage); } }

    //    public override Size PageSize
    //    {
    //        get { return this.pageSize; }
    //        set
    //        {
    //            this.pageSize = value;
    //            this.rowsPerPage = PageElementRenderer.RowsPerPage(this.pageSize.Height);
    //            if (rowsPerPage <= 0)
    //                throw new InvalidOperationException("Page can't fit any rows!");
    //        }
    //    }

    //    public override IDocumentPaginatorSource Source { get { return null; } }
    //}


    // Converting XAML to HTML
    // https://github.com/microsoft/wpf-samples/tree/main/Sample%20Applications/HtmlToXamlDemo


    internal class UpPrintingPreview
    {
        private string _previewWindowXaml =
            @"<Window
                xmlns                 ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
                xmlns:x               ='http://schemas.microsoft.com/winfx/2006/xaml'
                Title                 ='UB Print Preview - @@TITLE'
                Height                ='400'
                Width                 ='500'
                WindowStartupLocation ='CenterOwner'>
                <DocumentViewer Name='dv1'/>
             </Window>";


        private void SetThemeInfo(Control control)
        {
            Style style = new Style
            {
                TargetType = typeof(Control)
            };

            style.Setters.Add(new Setter(Control.FontFamilyProperty, new FontFamily(StaticObjects.Parameters.FontFamily)));
            style.Setters.Add(new Setter(Control.FontSizeProperty, StaticObjects.Parameters.FontSize));
            if (!(control is ComboBox || control is ListView))
            {
                style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Colors.White)));
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Black)));
            }
            control.Style = style;
        }

        //void xxx()
        //{
        //    PrintDialog dialog = new PrintDialog();
        //    var paginator = new RowPaginator(rowsToPrint) { PageSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight) };

        //    string tempFileName = System.IO.Path.GetTempFileName();

        //    //GetTempFileName creates a file, the XpsDocument throws an exception if the file already
        //    //exists, so delete it. Possible race condition if someone else calls GetTempFileName
        //    File.Delete(tempFileName);
        //    using (XpsDocument xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite))
        //    {
        //        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
        //        writer.Write(paginator);

        //        PrintPreview previewWindow = new PrintPreview
        //        {
        //            Owner = this,
        //            Document = xpsDocument.GetFixedDocumentSequence()
        //        };
        //        previewWindow.ShowDialog();
        //    }
        //}


        internal void DoPreview(string title, FlowDocumentScrollViewer flowDocument, RichTextBox rtb)
        {
            string fileName = System.IO.Path.GetRandomFileName();
            try
            {

                flowDocument.Background = new SolidColorBrush(Colors.White);
                flowDocument.Foreground = new SolidColorBrush(Colors.Black);

                var separator = new Rectangle();
                separator.Stroke = new SolidColorBrush(Colors.Blue);
                separator.StrokeThickness = 3;
                separator.Height = 3;
                separator.Width = double.NaN;
                separator.Margin = new Thickness(10);

                BlockUIContainer lineBlock = new BlockUIContainer(separator);
                SetThemeInfo(flowDocument);
                flowDocument.Document.Blocks.Add(lineBlock);
                flowDocument.Document.Blocks.AddRange(rtb.Document.Blocks.ToList());
                // write the XPS document
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.ReadWrite))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(flowDocument);
                }

                // Read the XPS document into a dynamically generated
                // preview Window 
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.Read))
                {
                    FixedDocumentSequence fds = doc.GetFixedDocumentSequence();

                    string s = _previewWindowXaml;
                    s = s.Replace("@@TITLE", title.Replace("'", "&apos;"));

                    using (System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(new StringReader(s)))
                    {
                        Window preview = System.Windows.Markup.XamlReader.Load(reader) as Window;

                        DocumentViewer dv1 = LogicalTreeHelper.FindLogicalNode(preview, "dv1") as DocumentViewer;
                        dv1.Document = fds as IDocumentPaginatorSource;


                        preview.ShowDialog();
                    }
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
