using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using exactmobile.components.logging;
using exactmobile.common;
using System.IO;
using exactmobile.ussdservice.exceptions;
using exactmobile.ussdservice.handlers;
using exactmobile.ussdservice.configuration.http;
using System.Reflection;

namespace exactmobile.ussdservice.http
{
    public class USSDHttpListenerManager
    {
        #region Private Vars
        private Thread httpListenersThread;
        private AutoResetEvent httpListenersWaitHandle;
        private List<USSDHttpListener> listeners = new List<USSDHttpListener>();
        #endregion

        #region Public Methods
        public void Start()
        {
            StartListenerThread();
        }

        public void Stop() 
        {
            StopListenerThread();
        }
        #endregion

        #region Protected Methods
        protected void StartListenerThread()
        {
            LogManager.LogStatus("Starting USSD Http Listener thread");

            httpListenersThread = new Thread(new ThreadStart(StartHttpListeners));
            httpListenersThread.Start();
        }

        protected void StopListenerThread()
        {
            StopHttpListeners();
            if (httpListenersThread != null)
            {
                try
                {
                    httpListenersWaitHandle.Set();
                    httpListenersThread.Join(30000);
                }
                catch (Exception)
                {
                }
                LogManager.LogStatus("USSD Http Listener thread stopped");
            }
        }

        protected void StartHttpListeners()
        {
            httpListenersWaitHandle = new AutoResetEvent(false);
            try
            {
                USSDHttpListenerConfigurationCollection ussdHttpListenerConfigurations = USSDHttpListenerConfiguration.GetConfiguration().USSDHttpListenerConfigurations;
                foreach (USSDHttpListenerConfigurationSection ussdHttpListenerConfiguration in ussdHttpListenerConfigurations)
                {
                    if (ussdHttpListenerConfiguration.Enabled)
                    {
                        USSDHttpListener httpListener = USSDHttpListener.Start(ussdHttpListenerConfiguration);
                        listeners.Add(httpListener);
                    }
                }
                httpListenersWaitHandle.WaitOne();
            }
            catch (Exception)
            {
                StopHttpListeners();
                throw;
            }
        }

        protected void StopHttpListeners()
        {
            LogManager.LogStatus("USSD Http Listeners stopped");
            while (listeners.Count != 0)
            {
                try
                {
                    listeners[0].Stop();
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex);
                }
                listeners.RemoveAt(0);
            }
        }

        #endregion
    }
}
