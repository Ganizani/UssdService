using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
 
namespace exactmobile.ussdservice.configuration
{
    public class USSDListenerConfigurationCollection<T> : ConfigurationElementCollection
        where T : USSDListenerConfigurationSection, new() 
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((T)(element)).Name;
        }

        public USSDListenerConfigurationSection this[int index]
        {
            get
            {
                return (T)BaseGet(index);
            }
        }
    }
}
