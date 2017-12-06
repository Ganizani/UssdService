using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    interface IResultsOverridable
    {
        bool ResultsOverriden { get; set; }
        string OverrideValue { get; set; }
    }
}
