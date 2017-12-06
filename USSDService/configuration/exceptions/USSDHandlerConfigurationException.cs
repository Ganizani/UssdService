using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace exactmobile.ussdservice.exceptions
{
    public class USSDHandlerConfigurationException: Exception
    {
        public USSDHandlerConfigurationException(String message) : base(message) { }
    }
}
