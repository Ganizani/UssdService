using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.exceptions 
{
    public class USSDHandlerFactoryRegistrationException: Exception
    {
        public USSDHandlerFactoryRegistrationException(String message) : base(message) { }
        
        public USSDHandlerFactoryRegistrationException(String message, params Object[] args) : base(String.Format(message, args)) { }
    }
}
