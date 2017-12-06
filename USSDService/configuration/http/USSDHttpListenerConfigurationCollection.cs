using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
 
namespace exactmobile.ussdservice.configuration.http
{
    [ConfigurationCollection(typeof(USSDHttpListenerConfigurationSection))]
    public sealed class USSDHttpListenerConfigurationCollection : USSDListenerConfigurationCollection<USSDHttpListenerConfigurationSection>
    {
    }
}
