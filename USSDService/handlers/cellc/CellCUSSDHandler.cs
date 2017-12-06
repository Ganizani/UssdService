using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using exactmobile.common.Lookups;
using System.Xml;
using exactmobile.ussdservice.common.menu;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.handlers.handlers.cellc
{
    public class CellCUSSDHandler : BaseUSSDHandler
    {
        #region Enums
        public enum RequestTypes : int
        {
            Request = 1,
            Response = 2,
            Release = 3,
            Timeout = 4
        }

        public enum SubscriberTypes : int
        {
            Prepaid = 1,
            Postpaid = 2
        }
        #endregion

        #region Private Vars
        //private MenuManager menuManager;
        #endregion

        #region Properties
        public MobileNetworks.MobileNetwork MobileNetwork
        {
            get { return MobileNetworks.MobileNetwork.CellC; }
        }

        public CellCUSSDHandler.RequestTypes RequestType
        {
            get;
            protected set;
        }

        public SubscriberTypes? SubscriberType
        {
            get;
            protected set;
        }

        public String SessionID
        {
            get;
            protected set;
        }

        public String Imsi
        {
            get;
            protected set;
        }
        #endregion

        #region Constructor
        public CellCUSSDHandler(String handlerID)
            : base(handlerID)
        {
        }
        #endregion

        #region Public Methods
        public virtual Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return requestData.ToUpper().Contains("TID=");
        }

        public override String ProcessRequest(String requestData)
        {
            //int menuID = MenuManager
            //if (Session.LastMenuItemID == null)


            return (new CellCUssdResponse { RequestType = RequestTypes.Response, UssdString = base.ProcessRequest(requestData) }).AsXml(); 
        }
        #endregion

        #region Protected Methods
        protected override void DoInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, Dictionary<string, object> values)
        {
            MSISDN = values["msisdn"].ToString();
            SessionID = values["tid"].ToString();
            USSDString = values["ussdstring"].ToString();
            RequestType = (RequestTypes)Enum.Parse(typeof(RequestTypes), values["reqtype"].ToString().Replace("\"", String.Empty), true);
            SubscriberType = null;
            if (values.ContainsKey("subtype"))
                SubscriberType = (SubscriberTypes)Enum.Parse(typeof(SubscriberTypes), values["subtype"].ToString().Replace("\"", String.Empty), true);
            Imsi = null;
            if (values.ContainsKey("imsi"))
                Imsi = values["imsi"].ToString();

            if (RequestType == RequestTypes.Release)
                base.RequestType = BaseUSSDHandler.RequestTypes.Release;
        }
        #endregion
    }
}
