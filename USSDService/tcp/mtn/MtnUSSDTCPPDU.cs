using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Threading;
using System.Net;
using exactmobile.components.logging;
using exactmobile.common;
using System.IO;
using System.Net.Sockets;
using exactmobile.ussdservice.configuration.tcp;
using System.Xml;
using System.Xml.Serialization;

namespace exactmobile.ussdservice.tcp.mtn
{
    [Serializable]
    [XmlRoot(Namespace = "", ElementName = "ussd", DataType = "string", IsNullable = false)]
    public class MtnUSSDTCPPDU
    {
        #region enums
        public enum PDUTypes
        {
            PSSDR,
            PSSDC,
            PSSRR,
            PSSRC,
            USSRR,
            USSRC,
            USSNR,
            USSNC,
            CLOSE,
            ABORT,
            CTRL,
            TTICK,
            TTACK,
            TTNAK,
            MARS,
            ARES
        }
        #endregion

        #region Properties
        [XmlAttribute(AttributeName = "PDU")]
        public String Pdu { get; set; }
        [XmlAttribute(AttributeName = "MSISDN")]
        public String MSISDN { get; set; }
        [XmlAttribute(AttributeName = "STRING")]
        public String Data { get; set; }
        [XmlAttribute(AttributeName = "TID")]
        public String SessionID { get; set; }
        [XmlAttribute(AttributeName = "REQID")]
        public String RequestID { get; set; }
        [XmlAttribute(AttributeName = "ENCODING")]
        public String Encoding { get; set; }
        [XmlAttribute(AttributeName = "TARIFF")]
        public String Tariff { get; set; }
        [XmlAttribute(AttributeName = "STATUS")]
        public String Status { get; set; }
        [XmlElement(ElementName = "cookie")]
        public String Cookie { get; set; }
        #endregion

        #region Public Methods
        // For serialization
        public MtnUSSDTCPPDU() { }

        public static MtnUSSDTCPPDU FromXML(String xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MtnUSSDTCPPDU));
            MemoryStream memStream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(xmlString));
            try
            {
                memStream.Position = 0;
                return serializer.Deserialize(memStream) as MtnUSSDTCPPDU;
            }
            finally
            {
                memStream.Close();
                memStream.Dispose();
            }
        }

        public String ToXML()
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(MtnUSSDTCPPDU));
            //MemoryStream memStream = new MemoryStream();
            //try
            //{
            //    serializer.Serialize(memStream, this);
            //    Byte[] buffer = new Byte[memStream.Length + 3];
            //    buffer[0] = 255;
            //    buffer[memStream.Length - 1] = 255;
            //    buffer[memStream.Length - 2] = 255;
            //    memStream.Position = 0;
            //    memStream.Read(buffer, 1, (int)memStream.Length);
            //    return Encoding.ASCII.GetString(buffer);
            //}
            //finally
            //{
            //    memStream.Close();
            //    memStream.Dispose();
            //}

            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
            xmlDoc.AppendChild(xmlDeclaration);

            XmlElement ussdElement = xmlDoc.CreateElement("ussd");
            xmlDoc.AppendChild(ussdElement);

            XmlAttribute pduAttribute = xmlDoc.CreateAttribute("PDU");
            pduAttribute.Value = this.Pdu;
            ussdElement.Attributes.Append(pduAttribute);

            XmlAttribute msisdnAttribute = xmlDoc.CreateAttribute("MSISDN");
            msisdnAttribute.Value = (this.MSISDN == null ? "*" : this.MSISDN);
            ussdElement.Attributes.Append(msisdnAttribute);

            XmlAttribute stringAttribute = xmlDoc.CreateAttribute("STRING");
            stringAttribute.Value = this.Data;
            ussdElement.Attributes.Append(stringAttribute);

            XmlAttribute tidAttribute = xmlDoc.CreateAttribute("TID");
            tidAttribute.Value = this.SessionID;
            ussdElement.Attributes.Append(tidAttribute);

            XmlAttribute reqidAttribute = xmlDoc.CreateAttribute("REQID");
            reqidAttribute.Value = (this.RequestID == null ? "0" : this.RequestID);
            ussdElement.Attributes.Append(reqidAttribute);

            XmlAttribute encodingAttribute = xmlDoc.CreateAttribute("ENCODING");
            encodingAttribute.Value = (this.Encoding == null ? "ASCII" : this.Encoding);
            ussdElement.Attributes.Append(encodingAttribute);

            XmlAttribute tariffAttribute = xmlDoc.CreateAttribute("TARIFF");
            tariffAttribute.Value = (this.Tariff == null ? "*" : this.Tariff);
            ussdElement.Attributes.Append(tariffAttribute);

            XmlAttribute statusAttribute = xmlDoc.CreateAttribute("STATUS");
            statusAttribute.Value = (this.Status == null ? "0" : this.Status);
            ussdElement.Attributes.Append(statusAttribute);

            XmlElement cookieElement = xmlDoc.CreateElement("COOKIE");
            ussdElement.AppendChild(cookieElement);

            //XmlAttribute valueAttribute = xmlDoc.CreateAttribute("VALUE");
            //valueAttribute.Value = Cookie;
            //cookieElement.Attributes.Append(valueAttribute);

            return xmlDoc.OuterXml;
        }

        public PDUTypes FindResponseType(PDUTypes pduType, Boolean endResponse)
        {
            if (pduType == PDUTypes.PSSDR)
                return PDUTypes.PSSDC;
            else if (pduType == PDUTypes.PSSRR && endResponse)
                return PDUTypes.PSSRC;
            else if (pduType == PDUTypes.PSSRR && !endResponse)
                return PDUTypes.USSRR;
            else if (pduType == PDUTypes.USSRR)
                return PDUTypes.USSRC;
            else if (pduType == PDUTypes.USSRC)
                return PDUTypes.USSRR;
            return pduType;
        }
        #endregion
    }
}
