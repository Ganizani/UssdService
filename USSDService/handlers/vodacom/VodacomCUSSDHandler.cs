using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using exactmobile.common.Lookups;
using System.Xml;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.handlers.handlers.vodacom
{
    public class VodacomUSSDHandler : BaseUSSDHandler
    {
        #region Enums
        public enum RequestTypes : int
        {
            Request = 1,
            Response = 2,
            Release = 3,
            Timeout = 4
        }
        #endregion

        #region Private Vars
        #endregion

        #region Properties
        public MobileNetworks.MobileNetwork MobileNetwork
        {
            get { return MobileNetworks.MobileNetwork.Vodacom; }
        }

        public VodacomUSSDHandler.RequestTypes RequestType
        {
            get;
            protected set;
        }

        public String SessionID
        {
            get;
            protected set;
        }

        public String Phase
        {
            get;
            protected set;
        }
        #endregion

        #region Constructor
        public VodacomUSSDHandler(String handlerID)
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
            //return (new VodacomUssdResponse { RequestType = RequestTypes.Response, UssdString = base.ProcessRequest(requestData), sessionId = SessionID, reference = SessionID, responseType = "2" }).AsXml(); 
            //return base.ProcessRequest(requestData); // (new CellCUssdResponse { RequestType = RequestTypes.Response, UssdString = "Test" }).AsXml();
            return base.ProcessRequest(requestData);
        }
        #endregion

        #region Protected Methods
        protected override void DoInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, Dictionary<string, object> values)
        {
            MSISDN = values["msisdn"].ToString();
            SessionID = values["sessionid"].ToString();
            USSDString = values["requestvalue"].ToString();
            String requestType = values["requesttype"].ToString().Replace("\"", String.Empty);
            if (!String.IsNullOrEmpty(requestType))
                RequestType = (RequestTypes)Enum.Parse(typeof(RequestTypes), requestType, true);

            if (RequestType == RequestTypes.Release)
                base.requestType = BaseUSSDHandler.RequestTypes.Release;

            Phase = values["phase"].ToString();
        }
        #endregion
    }
}
