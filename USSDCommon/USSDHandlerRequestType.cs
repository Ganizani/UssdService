using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.handlers
{
    public class USSDHandlerRequestType
    {
        #region Enums
        public enum RequestTypes
        {
            Get,
            Post,
            TCP
        }
        #endregion

        #region Static Methods
        public static RequestTypes GetRequestType(String requestType)
        {
            return (RequestTypes)Enum.Parse(typeof(RequestTypes), requestType, true);
        } 
        #endregion
    }
}
