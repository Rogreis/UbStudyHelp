using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UbStudyHelp.Classes
{
    public class BaseClass
    {


        #region XmlElement functions


        protected bool IsNull(XElement elem)
        {
            if (elem == null) return true;
            XAttribute atrib = elem.Attribute("{http://www.w3.org/2001/XMLSchema-instance}nil");
            if (atrib != null && atrib.Value == "true") return true;
            return false;
        }


        protected bool GetBool(XElement xElem)
        {
            if (IsNull(xElem)) return false;
            string val = GetString(xElem);
            if (string.IsNullOrEmpty(val)) return false;
            if (val == "1" || val.ToLower() == "true") return true;
            return false;
        }


        protected short GetShort(XElement xElem)
        {
            if (IsNull(xElem)) return 0;
            short aux = 0;
            short.TryParse(xElem.Value, out aux);
            return aux;
        }

        protected string GetString(XElement xElem)
        {
            if (IsNull(xElem)) return "";
            string sReturnedValue = xElem.Value;
            if (xElem.HasElements)
            {
                sReturnedValue = "";
                XNode node = xElem.FirstNode;
                StringBuilder sb = new StringBuilder();
                do
                {
                    switch (node.NodeType)
                    {
                        case XmlNodeType.Text:
                            sb.Append((node as XText).Value);
                            break;
                        case XmlNodeType.Element:
                            switch ((node as XElement).Name.LocalName.ToLower())
                            {
                                case "i":
                                    sb.Append(node.ToString());
                                    break;
                                case "b":
                                    sb.Append(node.ToString());
                                    break;
                                case "high":
                                    sb.Append(node.ToString());
                                    break;
                            }
                            break;
                    }
                }
                while ((node = node.NextNode) != null);
                sReturnedValue = sb.ToString();
            }
            return sReturnedValue;
        }


        private Dictionary<string, string> DictionryInvalidXmlCharacters()
        {
            return new Dictionary<string, string>()
                 {
                     {"&",  "&amp;"},  // must be the first
	                 {"<",  "&lt;"},
                     {">",  "&gt;"},
                     {"\"",  "&quot;"},
                     {"'", "&apos;"}
                 };
        }

        protected string XmlDecodeString(string xmlIn)
        {
            foreach (KeyValuePair<string, string> pair in DictionryInvalidXmlCharacters())
                xmlIn = xmlIn.Replace(pair.Value, pair.Key);
            return xmlIn;
        }

        protected string XmlEncodeString(string xmlIn)
        {
            foreach (KeyValuePair<string, string> pair in DictionryInvalidXmlCharacters())
                xmlIn = xmlIn.Replace(pair.Key, pair.Value);
            return xmlIn;
        }


        protected string XmlToString(XElement xElem)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create(sb, xws);
            xElem.WriteTo(xw);
            xw.Flush();
            return sb.ToString();
        }





        #endregion


     }
}
