using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using exactmobile.common.Lookups;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.common.handlers
{
    public interface IUSSDHandler
    {
        #region Properties
        String HandlerID { get; }
        MobileNetworks.MobileNetwork MobileNetwork { get; }
        String MSISDN { get; }
        String USSDString { get; }
        #endregion

        #region Methods
        Boolean IsValidRequest(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType);
        void Initialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, out Boolean isTimeout,out Boolean isInvalid);
        String ProcessRequest(String requestData); 
        #endregion
    }
}
