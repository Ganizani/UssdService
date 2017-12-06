using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.session;

namespace exactmobile.ussdcommon
{
    public interface IDoubleOpt
    {
        DoubleOptIn ConstructDoi(USSDSession session, int subscriptionServiceID);
    }
}
