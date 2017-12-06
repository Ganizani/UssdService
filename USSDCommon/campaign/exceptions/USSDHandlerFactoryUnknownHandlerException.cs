using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.exceptions 
{
    public class USSDHandlerFactoryUnknownHandlerException: Exception
    {
        public USSDHandlerFactoryUnknownHandlerException(String message) : base(message) { }
    }
}
