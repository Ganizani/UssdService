using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using exactmobile.common.Lookups;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.handlers.handlers.vodacom
{
    public class VodacomUSSDPostHandler : VodacomUSSDHandler, IUSSDHandler
    {
        #region Constructor
        public VodacomUSSDPostHandler(String handlerID)
            : base(handlerID)
        {
        }
        #endregion

        #region Public Methods
        public override Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return (base.IsValidRequest(requestData, inputRequestType) && requestData.ToUpper().Contains("<MSG") && inputRequestType == USSDHandlerRequestType.RequestTypes.Post);
        }
        #endregion

        #region Protected Methods
        protected override Dictionary<string, object> DoBeforeInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            exactmobile.components.logging.LogManager.LogStatus("Message string: {0}", requestData);
            Dictionary<string, object> result = base.DoBeforeInitialize(requestData, inputRequestType);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestData);

            result.Add("msisdn", doc.SelectSingleNode("msg/msisdn").InnerText);
            result.Add("sessionid", doc.SelectSingleNode("msg/sessionid").InnerText);
            XmlNode phaseNode = doc.SelectSingleNode("msg/phase");
            String phaseData = String.Empty;
            if (phaseNode != null)
                phaseData = phaseNode.InnerText;
            result.Add("phase", phaseData);

            XmlNode requestNode = doc.SelectSingleNode("msg/request");
            String requestType = String.Empty;
            if (requestNode.Attributes.Count != 0)
                requestType = requestNode.Attributes["type"].Value;
            result.Add("requesttype", requestType);
            result.Add("requestvalue", requestNode.InnerText);

            //XmlElement msgElement = doc.GetElementById("msg");
            //foreach (XmlElement xmlElement in msgElement.ChildNodes)
            //{
            //    result.Add(xmlAttribute.Name.ToLower(), xmlAttribute.Value);
            //}
            return result;
        }
        #endregion
    }
}
