using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using exactmobile.common.Lookups;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.handlers.handlers.vodacom
{
    public class VodacomUSSDGetHandler : VodacomUSSDHandler, IUSSDHandler
    {
        #region Constructor
        public VodacomUSSDGetHandler(String handlerID)
            : base(handlerID)
        {
        }
        #endregion

        #region Public Methods
        public override Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return (base.IsValidRequest(requestData, inputRequestType) && inputRequestType == USSDHandlerRequestType.RequestTypes.Get);
        }
        #endregion

        #region Protected Methods
        protected override Dictionary<string, object> DoBeforeInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            Dictionary<string, object> result = base.DoBeforeInitialize(requestData, inputRequestType);

            String tempRequestData = requestData.Trim().ToLower();
            // Remove the querystring start identifier
            int paramStartIndex = requestData.IndexOf("?", 0, Math.Min(requestData.Length, 10));
            if (paramStartIndex >= 0)
                tempRequestData = tempRequestData.Remove(0, paramStartIndex+1);

            String[] requestItems = tempRequestData.Split(new String[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String requestItem in requestItems)
            {
                String[] requestItemInfo = requestItem.Split(new String[] { "=" }, StringSplitOptions.None);
                result.Add(requestItemInfo[0], (requestItemInfo.Length > 1 ? requestItemInfo[1] : null));
            }
            return result;
        }
        #endregion
    }
}
