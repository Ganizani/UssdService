using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.exceptions 
{
    public class USSDTcpReceiveTimeoutException: Exception
    {
        public USSDTcpReceiveTimeoutException(String message) : base(message) { }

        public USSDTcpReceiveTimeoutException(String message, params Object[] args) : base(String.Format(message, args)) { }
    }
}
