using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using exactmobile.common;

namespace exactmobile.ussdservice.common.session
{
    public class USSDSessionManager
    {
        #region Private Vars
        private static USSDSessionManager ussdSessionManager = null;
        private Thread timeoutThread;
        private Boolean running = false;

        private Dictionary<String, USSDSession> cache = new Dictionary<String, USSDSession>();
        private int sessionTimeout;
        #endregion

        #region Public Methods
        public static void Shutdown()
        {
            if (ussdSessionManager != null)
            {
                ussdSessionManager.running = false;
                try
                {
                    if (!ussdSessionManager.timeoutThread.Join(6000))
                        ussdSessionManager.timeoutThread.Abort();
                }
                catch (Exception)
                {
                }
            }
        }

        public static USSDSessionManager Instance
        {
            get
            {
                if (ussdSessionManager == null)
                    ussdSessionManager = new USSDSessionManager();
                return ussdSessionManager;
            }
        }

        public USSDSessionManager()
        {
            running = true;
            sessionTimeout = ConfigurationAppSettingsWrapper<int>.GetValue("Session Timeout", 180000);
            timeoutThread = new Thread(new ThreadStart(CleanupSessions));
            timeoutThread.Start();
        }

        public USSDSession CreateSession(String msisdn)
        {
            if (String.IsNullOrEmpty(msisdn)) return null;
            USSDSession result = new USSDSession(msisdn, null, null);
            cache.Add(msisdn, result);
            return result;
        }

        public USSDSession GetSession(String msisdn)
        {
            if (String.IsNullOrEmpty(msisdn)) return null;

            if (cache.ContainsKey(msisdn))
                return cache[msisdn];
            return null;
        }

        public void EndSession(String msisdn)
        {
            cache.Remove(msisdn);
        }
        #endregion

        #region Protected methods
        protected void CleanupSessions()
        {
            while (running)
            {
                List<KeyValuePair<String, USSDSession>> sessionsToRemove = cache.Where(c => DateTime.Now.Subtract(c.Value.LastAccess).TotalMilliseconds > sessionTimeout).ToList();
                foreach (KeyValuePair<String, USSDSession> session in sessionsToRemove)
                    EndSession(session.Value.MSISDN);
                Thread.Sleep(5000);
            }
        }
        #endregion
    }
}
