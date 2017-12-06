using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace exactmobile.ussdservice.configuration.http
{
    public sealed class USSDHttpListenerConfigurationSection : USSDListenerConfigurationSection
    {
        #region Properties
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = true)]
        public Boolean Enabled
        {
            get
            {
                return (Boolean)base["enabled"];
            }
            set
            {

                base["enabled"] = value;
            }
        }
        
        [ConfigurationProperty("address", DefaultValue = "127.0.0.1", IsRequired = true)]
        public String Address
        {
            get
            {
                return (String)base["address"];
            }
            set
            {

                base["address"] = value;
            }
        }

        [ConfigurationProperty("numberoflisteners", DefaultValue = 1, IsRequired = false)]
        public int NumberOfListeners
        {
            get
            {
                return (int)base["numberoflisteners"];
            }
            set
            {

                base["numberoflisteners"] = value;
            }
        }
        
        [ConfigurationProperty("mobilenetwork", DefaultValue = "", IsRequired = false)]
        public String MobileNetwork
        {
            get
            {
                return (String)base["mobilenetwork"];
            }
            set
            {

                base["mobilenetwork"] = value;
            }
        }
        #endregion
    }
}
