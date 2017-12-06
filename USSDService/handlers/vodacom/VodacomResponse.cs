using System;
using System.Xml;

namespace exactmobile.ussdservice.handlers.handlers.vodacom
{
    public class VodacomUssdResponse
    {
        public VodacomUSSDHandler.RequestTypes RequestType;
        public String UssdString;
        public string sessionId;
        public string responseType;
        public string reference;

        public String AsXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement nodeRoot = doc.CreateElement("msg");
            XmlElement nodeSession = doc.CreateElement("sessionid");
            XmlElement nodeResponse = doc.CreateElement("response");
            XmlAttribute nodeResponseAtrr = doc.CreateAttribute("type");

            nodeSession.InnerText = sessionId;
            nodeResponse.InnerText = UssdString;
            nodeResponseAtrr.Value = responseType;
                        

            doc.AppendChild(nodeRoot);
            nodeRoot.AppendChild(nodeSession);
            nodeResponse.Attributes.Append(nodeResponseAtrr);
            nodeRoot.AppendChild(nodeResponse);
            
            
            return doc.OuterXml;
        }
    }
}
