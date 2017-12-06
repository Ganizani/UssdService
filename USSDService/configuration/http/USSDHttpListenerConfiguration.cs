using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using exactmobile.ussdservice.exceptions;
using exactmobile.components.logging;

namespace exactmobile.ussdservice.configuration.http
{
    public class USSDHttpListenerConfiguration : USSDListenerConfiguration
    {
        #region Private Vars
        #endregion

        #region Properties
        /// <summary>
        /// The value of the property here "Folders" needs to match that of the config file section
        /// </summary>
        [ConfigurationProperty("USSDHttpListeners")]
        public USSDHttpListenerConfigurationCollection USSDHttpListenerConfigurations
        {
            get { return ((USSDHttpListenerConfigurationCollection)(base["USSDHttpListeners"])); }
        }
        #endregion

        #region Public Methods
        public static USSDHttpListenerConfiguration GetConfiguration()
        {
            return (USSDHttpListenerConfiguration)System.Configuration.ConfigurationManager.GetSection("USSDHttpListenerConfigurations");
        }
        #endregion
    }
}
