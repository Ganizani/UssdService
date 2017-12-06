using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace exactmobile.ussdservice.configuration
{
    public class ApplicationSettingSectionConfiguration
    {
        private static string appState;
        public string AppState
        {
            get
            {
                if (String.IsNullOrEmpty(appState))
                {
                    if (!TryGetAppSetting("AppState", out appState, true))
                        appState = "DEV";
                }
                return appState;
            }
        }

        public static bool TryGetAppSetting(string key, out string value, bool isFalseOnStringNullEmpty)
        {
            bool retVal = false;
            try
            {
                value = System.Configuration.ConfigurationManager.AppSettings[key];
                retVal = true;
            }
            catch
            {
                value = string.Empty;
            }
            if (isFalseOnStringNullEmpty)
                retVal = !String.IsNullOrEmpty(value);
            return retVal;
        }        
    }
}
