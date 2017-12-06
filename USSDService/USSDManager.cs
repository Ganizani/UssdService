using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using exactmobile.components.logging;
using exactmobile.ussdservice.handlers;
using exactmobile.ussdservice.http;
using exactmobile.ussdservice.handlers.handlers;
using exactmobile.ussdservice.tcp;
using exactmobile.ussdservice.handlers.handlers.cellc;
using exactmobile.ussdservice.handlers.handlers.vodacom;
using exactmobile.ussdservice.common.menu;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdservice.handlers.mtn;
using System.Reflection;

namespace exactmobile.ussdservice
{
    public class USSDManager
    {
        #region Private Vars
        private USSDHttpListenerManager ussdHttpListener;
        private USSDTcpListenerManager ussdTcpListenerManager;
        #endregion

        #region Public Methods
        public void Start()
        {
            //USSDHandlerFactory.RegisterUSSDHandler("CellC USSD Post Handler", typeof(CellCUSSDPostHandler));
            //USSDHandlerFactory.RegisterUSSDHandler("CellC USSD Get Handler", typeof(CellCUSSDGetHandler));
            Assembly assembly = Assembly.LoadFrom("SubscriptionUSSDProcessor.dll");
            Type type = assembly.GetType("exactmobile.ussdservice.processors.subscriptions.SubscriptionUSSDProcessor");
            USSDHandlerFactory.RegisterUSSDHandler("Vodacom USSD Post Handler", typeof(VodacomUSSDPostHandler));
            USSDHandlerFactory.RegisterUSSDHandler("Vodacom USSD Get Handler", typeof(VodacomUSSDGetHandler));
            //USSDHandlerFactory.RegisterUSSDHandler("Mtn USSD Tcp Handler", typeof(MtnUSSDHandler));
            //var processor = typeof(SubscriptionUSSDProcessor);
            
            ussdHttpListener = new USSDHttpListenerManager();
            ussdHttpListener.Start();

            ussdTcpListenerManager = new USSDTcpListenerManager();
            try
            {
                ussdTcpListenerManager.Start();
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }
        }

        public void Stop()
        {
            ussdHttpListener.Stop();
            ussdTcpListenerManager.Stop();
            USSDSessionManager.Shutdown();
        } 
        #endregion

        #region Protected Methods
             
        #endregion
    }
}
