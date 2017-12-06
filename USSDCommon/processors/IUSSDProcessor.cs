using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.session;

namespace exactmobile.ussdservice.common.processors
{
    public interface IUSSDProcessor
    {
        USSDSession Session { get; set;  }
        IUSSDHandler Handler { get; set;  }

        void Initialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType);
        String ProcessRequest(String requestData);
        string ManualOverRide();
        string OnConnect();

    }
}
