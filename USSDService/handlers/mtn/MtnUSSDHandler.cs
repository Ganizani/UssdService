using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.common.Lookups;
using exactmobile.ussdservice.common.handlers;
using System.Xml;

namespace exactmobile.ussdservice.handlers.mtn
{
    public class MtnUSSDHandler : BaseUSSDHandler, IUSSDHandler
    {
        #region Properties
        public MobileNetworks.MobileNetwork MobileNetwork
        {
            get { return MobileNetworks.MobileNetwork.Mtn; }
        }

        public String SessionID
        {
            get;
            protected set;
        }
        #endregion

        #region Constructor
        public MtnUSSDHandler(String handlerID)
            : base(handlerID)
        {
        }
        #endregion

        #region Public Methods
        public virtual Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return true;
        }

        public override String ProcessRequest(String requestData)
        {
            return base.ProcessRequest(requestData);
        }
        #endregion

        #region Protected Methods
        protected override Dictionary<string, object> DoBeforeInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            Dictionary<string, object> result = base.DoBeforeInitialize(requestData, inputRequestType);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestData);

            result.Add("msisdn", doc.SelectSingleNode("ussd").Attributes["MSISDN"].Value);
            result.Add("sessionid", doc.SelectSingleNode("ussd").Attributes["REQID"].Value);
            result.Add("requestvalue", doc.SelectSingleNode("ussd").Attributes["STRING"].Value);

            return result;
        }

        protected override void DoInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, Dictionary<string, object> values)
        {
            MSISDN = values["msisdn"].ToString();
            SessionID = values["sessionid"].ToString();
            USSDString = values["requestvalue"].ToString();
            //String requestType = values["requesttype"].ToString().Replace("\"", String.Empty);
        }
        #endregion
    }
}
