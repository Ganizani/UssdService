using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using exactmobile.ussdservice.exceptions;
using exactmobile.common.Lookups;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.handlers
{
    public class USSDHandlerFactory
    {
        #region Private Vars
        private static USSDHandlerFactory ussdHandlerFactory;

        private Dictionary<String, Type> ussdhandlers = new Dictionary<String, Type>(); 
        #endregion

        #region Public Methods
        public static void RegisterUSSDHandler(String handlerID, Type ussdHandlerType)
        {
            if (Instance.ussdhandlers.ContainsKey(handlerID))
                throw new USSDHandlerFactoryRegistrationException("Handler {0} has already been registered", handlerID);
            Instance.ussdhandlers.Add(handlerID, ussdHandlerType);
        }

        public static IUSSDHandler GetHandler(MobileNetworks.MobileNetwork mobileNetwork, String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            foreach(KeyValuePair<String, Type> ussdHandlerDefinition in ussdHandlerFactory.ussdhandlers)
            {
                Type ussdHandlerType = ussdHandlerDefinition.Value;
                ConstructorInfo ussdHandlerConstructor = ussdHandlerType.GetConstructor(new Type[] { typeof(String) });
                if (ussdHandlerConstructor != null)
                {
                    IUSSDHandler ussdHandler = ussdHandlerConstructor.Invoke(new Object[] { ussdHandlerDefinition.Key }) as IUSSDHandler;
                    if (ussdHandler != null && ussdHandler.MobileNetwork == mobileNetwork && ussdHandler.IsValidRequest(requestData, inputRequestType))
                        return ussdHandler;
                }
            }
            throw new USSDHandlerFactoryUnknownHandlerException("Unable to find USSDHandler for request data");
        }
        #endregion

        #region Private Methods
        private static USSDHandlerFactory Instance
        {
            get
            {
                if (ussdHandlerFactory == null)
                    ussdHandlerFactory = new USSDHandlerFactory();
                return ussdHandlerFactory;
            }
        }
        #endregion
    }
}
