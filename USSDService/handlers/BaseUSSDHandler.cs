using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.exceptions;
using exactmobile.ussdservice.common.menu;
using exactmobile.ussdservice.common.campaign;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdservice.common.handlers;
using System.Web;
using exactmobile.components.logging;

namespace exactmobile.ussdservice.handlers
{
    public abstract class BaseUSSDHandler
    {
        #region Private Vars
        private String handlerID = String.Empty;
        private String ussdString;
        protected USSDSession Session;
        protected int USSDNumber = 0;
        protected MenuManager menuManager;
        protected RequestTypes requestType;

        public enum RequestTypes
        {
            Response,
            Release
        }

        #endregion

        #region Properties
        public String HandlerID
        {
            get
            {
                if (this.handlerID.Trim().Length == 0)
                    throw new USSDHandlerConfigurationException("Handler has not been configured. Missing HandlerID");
                return this.handlerID;
            }
        }

        public RequestTypes RequestType
        {
            get;
            protected set;
        }

        public String MSISDN
        {
            get;
            protected set;
        }

        public String USSDString
        {
            get { return ussdString; }
            protected set
            {
                ussdString = value;
            }
        }
        #endregion

        #region Constructor
        public BaseUSSDHandler()
        {
        }

        public BaseUSSDHandler(String handlerID)
        {
            this.handlerID = handlerID;
        }
        #endregion

        #region Public Methods
        public override bool Equals(object obj)
        {
            IUSSDHandler compareUSSDHandler = obj as IUSSDHandler;
            return compareUSSDHandler.HandlerID.Equals(this.handlerID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Initialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, out Boolean isTimeout, out Boolean isInvalid)
        {
            Dictionary<String, Object> values = DoBeforeInitialize(requestData, inputRequestType);
            try
            {
                DoInitialize(requestData, inputRequestType, values);
            }
            finally
            {
                isTimeout = false;
                isInvalid = false;

                if (requestType == RequestTypes.Release)
                {
                    isTimeout = true;
                    USSDSessionManager.Instance.EndSession(MSISDN);
                }
                else
                {
                    DoAfterInitialize(requestData, inputRequestType, values, out isTimeout, out isInvalid);
                    if (!isTimeout && !isInvalid && Session.GetProcessor(this as IUSSDHandler) != null)
                        Session.GetProcessor(this as IUSSDHandler).Initialize(requestData, inputRequestType);
                }
            }
        }

        public virtual String ProcessRequest(String requestData)
        {
            try
            {
                if (Session.GetProcessor(this as IUSSDHandler) != null)
                {
                    Session.GetProcessor(this as IUSSDHandler).Session = Session;
                    return Session.GetProcessor(this as IUSSDHandler).ProcessRequest(requestData);
                }
                else
                {
                    String result = String.Empty;
                    if (Session.LastMenu == null)
                    {
                        Session.LastMenu = menuManager.Menu.First();
                        result = menuManager.BuildMenu(Session.LastMenuItem.MenuItemID);
                    }
                    else
                    {
                        UssdMenu lastMenu;
                        result = menuManager.BuildMenu(Session.LastMenu.MenuID, USSDString, out lastMenu);
                        Session.LastMenu = lastMenu;
                    }
                    return result;
                }
            }
            catch(Exception exp)
            {
                LogManager.LogError(exp);
                throw new Exception("Veiullez essayer plutard");
            }
        }
        #endregion

        #region Protected Methods
        protected virtual Dictionary<string, object> DoBeforeInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            return new Dictionary<string, object>();
        }

        protected abstract void DoInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, Dictionary<string, object> values);

        protected virtual void DoAfterInitialize(String requestData, USSDHandlerRequestType.RequestTypes inputRequestType, Dictionary<string, object> values, out Boolean isTimeout, out Boolean isInvalid)
        {
            bool addTransaction = false;
            isTimeout = false;
            isInvalid = false;
            if (String.IsNullOrEmpty(MSISDN)) throw new Exception("Invalid request. No MSISDN Specified");

            if (String.IsNullOrEmpty(USSDString))
            {
                throw new Exception("notify# votre choix n'est pas correcte! ");
            }

            if (USSDString.ToUpper() == "USER+TIMEOUT" || requestData.ToUpper().Contains("PDU=\"ABORT\""))
            {
                isTimeout = true;
                USSDSessionManager.Instance.EndSession(MSISDN);
                return;
            }

            Session = USSDSessionManager.Instance.GetSession(MSISDN);
            if (Session == null)
            {
                Session = USSDSessionManager.Instance.CreateSession(MSISDN);
                addTransaction = true;
            }

            if(USSDString.ToLower().Contains("continue")||string.IsNullOrEmpty(USSDString))
            {
                throw new Exception("notify# votre choix n'est pas correcte! ");
            }
            if (Session.USSDNumber == null) // || (!String.IsNullOrEmpty(USSDString.Trim()) && USSDString.Contains("*120*")))
            {

                String ussdNumberString = string.Empty;

                try
                {
                    ussdNumberString = System.Net.WebUtility.UrlDecode(USSDString.Trim());
                }
                catch { ussdNumberString = USSDString.Replace("%2A4006%2A", "*4006*").Replace("%23", "#"); }

                //exactmobile.components.logging.LogManager.LogStatus("Message string: {0}", USSDString);
                ussdNumberString = ussdNumberString.Trim().Replace("*4006*", String.Empty);
                int lastStar = ussdNumberString.IndexOf("*");
                if (lastStar >= 0)
                    ussdNumberString = ussdNumberString.Substring(0, lastStar);
                int lastHash = ussdNumberString.IndexOf("#");
                if (lastStar < 0 && lastHash >= 0)
                    ussdNumberString = ussdNumberString.Substring(0, lastHash);
                lastHash = ussdNumberString.IndexOf("%23");
                if (lastStar < 0 && lastHash >= 0)
                {
                    try
                    {
                        ussdNumberString = ussdNumberString.Substring(0, lastHash);
                    }
                    catch { }
                }
                int.TryParse(ussdNumberString, out USSDNumber);
                USSDSessionManager.Instance.EndSession(Session.MSISDN);
                Session = USSDSessionManager.Instance.CreateSession(MSISDN);
                Session.USSDNumber = USSDNumber;
            }
            else
            {
                USSDNumber = Session.USSDNumber.Value;
            }
            if (Session.Campaign == null)
                Session.Campaign = USSDCampaignManager.GetCampaignID(USSDNumber);

            //if (Session.LastMenuId == 0)
            //  Session.LastMenuId = MenuManager.GetRootMenuId(Session.Campaign.CampaignID);

            //if (menuManager == null)
            //    menuManager = new MenuManager(Session.Campaign.CampaignID, USSDNumber);

            if (Session.Campaign == null)
            {
                isInvalid = true;
                USSDSessionManager.Instance.EndSession(MSISDN);
                return;
            }
            else
            {
                if (addTransaction)
                {
                    long transactionID;
                    ussdcommon.Transaction.Add(Session, USSDString, out transactionID);
                    Session.USSDTransactionID = transactionID;
                }
            }
        }
        #endregion
    }
}
