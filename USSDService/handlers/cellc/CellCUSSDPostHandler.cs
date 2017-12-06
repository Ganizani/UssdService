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

namespace exactmobile.ussdservice.handlers.handlers.cellc
{
    public class CellCUSSDPostHandler : CellCUSSDHandler, IUSSDHandler
    {
        #region Constructor
        public CellCUSSDPostHandler(String handlerID)
            : base(handlerID)
        {
        }
        #endregion

        #region Public Methods
        public override Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return (base.IsValidRequest(requestData, inputRequestType) && inputRequestType == USSDHandlerRequestType.RequestTypes.Post);
        }
        #endregion

        #region Protected Methods
        protected override Dictionary<string, object> DoBeforeInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            Dictionary<string, object> result = base.DoBeforeInitialize(requestData, inputRequestType);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestData);

            foreach (XmlAttribute xmlAttribute in doc.ChildNodes[0].Attributes)
                result.Add(xmlAttribute.Name.ToLower(), xmlAttribute.Value);
            return result;
        }
        #endregion
    }
}
