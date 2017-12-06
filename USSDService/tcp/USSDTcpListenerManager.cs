using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using exactmobile.ussdservice.exceptions;
using exactmobile.ussdservice.configuration;
using exactmobile.ussdservice.configuration.tcp;

namespace exactmobile.ussdservice.tcp
{
    public class USSDTcpListenerManager
    {
        #region Private Vars
        private List<USSDTCPListener> tcpListeners = new List<USSDTCPListener>();
        #endregion

        #region Public Methods
        public void Start()
        {
            USSDTcpListenerConfigurationCollection ussdTcpListenerConfigurations = USSDTcpListenerConfiguration.GetConfiguration().USSDTcpListenerConfigurations;
            tcpListeners.Clear();
            foreach (USSDTcpListenerConfigurationSection ussdTcpListenerConfiguration in ussdTcpListenerConfigurations)
            {
                if (ussdTcpListenerConfiguration.Enabled)
                {
                    Type ussdTcpListenerConfigurationType = Type.GetType(ussdTcpListenerConfiguration.Type);
                    ConstructorInfo constructor = ussdTcpListenerConfigurationType.GetConstructor(new Type[] { typeof(USSDTcpListenerConfigurationSection) });
                    if (constructor != null)
                    {
                        USSDTCPListener ussdTCPListener = constructor.Invoke(BindingFlags.Static, null, new Object[] { ussdTcpListenerConfiguration }, null) as USSDTCPListener;
                        tcpListeners.Add(ussdTCPListener);
                        ussdTCPListener.Start();
                    }
                }
            }
        }

        public void Stop()
        {
            while (tcpListeners.Count != 0)
            {
                try
                {
                    tcpListeners[0].Stop();
                }
                catch (Exception)
                {
                }
                tcpListeners.RemoveAt(0);
            }
        }
        #endregion
    }
}
