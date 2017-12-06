using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace exactmobile.ussdservice.configuration
{
    public class USSDListenerConfigurationSection : ConfigurationElement
    {
        #region Properties
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey=true)]
        public String Name
        {
            get
            {
                return (String)base["name"];
            }
            set
            {

                base["name"] = value;
            }
        }
        #endregion
    }
}
