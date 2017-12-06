using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
 
namespace exactmobile.ussdservice.configuration.tcp
{
    [ConfigurationCollection(typeof(USSDTcpListenerConfigurationSection))]
    public sealed class USSDTcpListenerConfigurationCollection : USSDListenerConfigurationCollection<USSDTcpListenerConfigurationSection>
    {
    }
}
