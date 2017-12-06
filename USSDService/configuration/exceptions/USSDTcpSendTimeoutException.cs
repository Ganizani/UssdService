using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.exceptions 
{
    public class USSDTcpSendTimeoutException: Exception
    {
        public USSDTcpSendTimeoutException(String message) : base(message) { }

        public USSDTcpSendTimeoutException(String message, params Object[] args) : base(String.Format(message, args)) { }
    }
}
