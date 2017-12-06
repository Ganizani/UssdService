using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.campaign;
using exactmobile.ussdservice.common.menu;
using System.Reflection;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.common.session
{
    public class USSDSession
    {
        #region Private Vars
        private IUSSDProcessor processor;
        private Dictionary<String, Object> sessionValues = new Dictionary<string, object>();
        #endregion

        #region Properties
        public readonly Guid SessionID;
        public readonly String MSISDN;
        public UssdMenu LastMenu { get; set; }
        public UssdMenuItem LastMenuItem { get; set; }
        public DateTime LastAccess { get; set; }
        public USSDCampaign Campaign { get; set; }
        public int? USSDNumber { get; set; }
        public string Code { get; set; }
        public int MainMenuId { get; set; }
        public MenuItem LastMenuAccessed { get; set; }
        public readonly int MobileNetworkID;
        public long USSDTransactionID {get;set;}
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Company { get; set; }
        #endregion

        #region Properties
        public Object this[String key]
        {
            get 
            {
                if (sessionValues.ContainsKey(key))
                    return sessionValues[key];
                return null;
            }
            set
            {
                sessionValues[key] = value;
            }
        }
        #endregion

        #region Public Methods
        public USSDSession(String msisdn, UssdMenu lastMenu, UssdMenuItem lastMenuItem)
        {
            this.MSISDN = msisdn;
            this.SessionID = Guid.NewGuid();
            this.LastMenuItem = lastMenuItem;
            this.LastAccess = DateTime.Now;
            this.Campaign = null;
            this.USSDNumber = null;
            this.LastMenu = lastMenu;
            this.Code = null;
          //  this.MobileNetworkID = ussdcommon.utility.CommonUtility.ReturnNetworkID(msisdn);
        }

        public IUSSDProcessor GetProcessor(IUSSDHandler handler)
        {
            if (processor == null)
            {
                Type processorType = Type.GetType(Campaign.ProcessorType, false);
                if (processorType != null)
                {
                    ConstructorInfo constructor = processorType.GetConstructor(new Type[] { typeof(USSDSession), typeof(IUSSDHandler) });
                    if (constructor != null)
                        processor = constructor.Invoke(new Object[] { this, handler }) as IUSSDProcessor;
                }
            }
            processor.Session = this;
            processor.Handler = handler;
            return processor;
        }
        #endregion
    }
}
