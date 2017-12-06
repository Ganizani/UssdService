using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using exactmobile.ussdservice.exceptions;
using exactmobile.components.logging;

namespace exactmobile.ussdservice.configuration.tcp
{
    public class USSDTcpListenerConfiguration : USSDListenerConfiguration
    {
        #region Private Vars
        #endregion

        #region Properties
        /// <summary>
        /// The value of the property here "Folders" needs to match that of the config file section
        /// </summary>
        [ConfigurationProperty("USSDTcpListeners")]
        public USSDTcpListenerConfigurationCollection USSDTcpListenerConfigurations
        {
            get { return ((USSDTcpListenerConfigurationCollection)(base["USSDTcpListeners"])); }
        }
        #endregion

        #region Public Methods
        public static USSDTcpListenerConfiguration GetConfiguration()
        {
            return (USSDTcpListenerConfiguration)System.Configuration.ConfigurationManager.GetSection("USSDTcpListenerConfigurations");
        }
        #endregion
    }
}
