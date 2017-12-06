using System;
using System.Xml;

namespace exactmobile.ussdservice.handlers.handlers.cellc
{
    public class CellCUssdResponse
    {
        public CellCUSSDHandler.RequestTypes RequestType;
        public String UssdString;

        public String AsXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement node = doc.CreateElement("ussdresp");
            XmlAttribute attr = doc.CreateAttribute("reqtype");

            doc.AppendChild(node);

            attr.Value = ((int)RequestType).ToString();
            node.Attributes.Append(attr);
            
            attr = doc.CreateAttribute("ussdstring");
            attr.Value = UssdString;
            
            node.Attributes.Append(attr);
            
            return doc.OuterXml;
        }
    }
}
