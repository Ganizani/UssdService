using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.exceptions 
{
    public class USSDUnknownCampaignForNumberException: Exception
    {
        public USSDUnknownCampaignForNumberException(String message) : base(message) { }

        public USSDUnknownCampaignForNumberException(String message, params Object[] args) : base(String.Format(message, args)) { }
    }
}
