﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.exceptions 
{
    public class USSDHttpListenerException: Exception
    {
        public USSDHttpListenerException(String message) : base(message) { }
    }
}
