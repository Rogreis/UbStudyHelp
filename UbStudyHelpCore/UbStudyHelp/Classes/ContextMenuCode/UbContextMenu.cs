using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes.ContextMenuCode
{
    internal class UbContextMenu : ContextMenu
    {
        protected const int MaxWordsForSearch = 20;
        protected FlowDocumentScrollViewer FlowDocument = null;
        protected TOC_Entry Entry = null;

        protected MenuItem CreateMenuItem(string header, RoutedUICommand command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Command = command;
            item.VerticalAlignment = VerticalAlignment.Top;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            return item;
        }

        protected MenuItem CreateMenuItem(string header, RoutedEventHandler command)
        {
            MenuItem item = new MenuItem();
            item.Header = header;
            item.Click += command;
            item.VerticalAlignment = VerticalAlignment.Top;
            item.HorizontalAlignment = HorizontalAlignment.Left;
            return item;
        }


        //protected void SelectAll()
        //{
        //    TextPointer pointerStart = FlowDocument.Document.ContentStart;
        //    TextPointer pointerEnd = FlowDocument.Document.ContentEnd;
        //    FlowDocument.Selection.Select(pointerStart, pointerEnd);
        //}



        //protected string GetAllText()
        //{
        //    TextRange range = new TextRange(Document.Document.ContentStart, Document.Document.ContentEnd);
        //    using (Stream stream = new MemoryStream())
        //    {
        //        range.Save(stream, DataFormats.Text);
        //        return Encoding.UTF8.GetString((stream as MemoryStream).ToArray());
        //    }
        //}


        //protected System.Windows.Documents.Paragraph GetParagraphNoSelection()
        //{
        //    TextPointerContext tpc = FlowDocument.Document.ContentStart.GetPointerContext(LogicalDirection.Forward);
        //    System.Windows.Documents.Paragraph p= new System.Windows.Documents.Paragraph();
        //    return p;
        //}


        protected ParagraphSearchData GetCurrentParagraph()
        {
            try
            {
                TextPointer pointer = FlowDocument.Selection.Start;
                System.Windows.Documents.Paragraph p = pointer.Paragraph;
                if (p == null) return null;
                ParagraphSearchData data = p.Tag as ParagraphSearchData;
                return data;
            }
            catch
            {
                return null;
            }
        }

        protected string GetSelectedText()
        {
            try
            {
                TextPointer potStart = FlowDocument.Selection.Start;
                TextPointer potEnd = FlowDocument.Selection.End;
                TextRange range = new TextRange(potStart, potEnd);
                return range.Text;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Split the currente selected text in words
        /// </summary>
        /// <returns></returns>
        protected string[] SplitSelectedText()
        {
            // Some day check regex:  new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*")  https://stackoverflow.com/questions/10239518/how-do-i-split-a-phrase-into-words-using-regex-in-c-sharp
            string text = GetSelectedText();
            if (text != null && !string.IsNullOrWhiteSpace(text))
            {
                char[] sep = { ' ', '.', ',', ';', ':', '!', '?', '\t' };
                return text.Split(sep, MaxWordsForSearch, StringSplitOptions.RemoveEmptyEntries);
            }
            return null;
        }
    }
}
