using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdcommon
{
    public class USSDStringHelper
    {
        public int USSDNumber;
        public String Message;

        public static USSDStringHelper Decode(String ussdString)
        {
            USSDStringHelper result = new USSDStringHelper();
            int messageStartIndex = -1;
            if (!String.IsNullOrEmpty(ussdString.Trim()) && ussdString.Contains("*120*"))
            {
                String ussdNumberString = ussdString.Trim().Replace("*120*", String.Empty);
                int lastStar = ussdNumberString.IndexOf("*");
                if (lastStar >= 0)
                {
                    ussdNumberString = ussdNumberString.Substring(0, lastStar);
                    messageStartIndex = lastStar;
                }
                int lastHash = ussdNumberString.IndexOf("#");
                if (lastStar < 0 && lastHash >= 0)
                {
                    ussdNumberString = ussdNumberString.Substring(0, lastHash);
                    messageStartIndex = lastHash;
                }
                lastHash = ussdNumberString.IndexOf("%23");
                if (lastStar < 0 && lastHash >= 0)
                {
                    ussdNumberString = ussdNumberString.Substring(0, lastHash);
                    messageStartIndex = lastHash;
                }
                // keep bug fix 4 Cell C. # is been posted as %23. 
                ussdNumberString.Replace("%23", "#");
                result.USSDNumber = int.Parse(ussdNumberString);
            }
            if (messageStartIndex >= 0)
            {
                ussdString = ussdString.Replace("*120*", String.Empty).Replace("#", String.Empty).Replace("%23", String.Empty).Replace("*", String.Empty).Trim();
                if (ussdString.Length >= messageStartIndex)
                    result.Message = ussdString.Substring(messageStartIndex);
                else
                    result.Message = ussdString;
            }
            else
            {
                // keep bug fix 4 Cell C. # is been posted as %23. 


                result.Message = ussdString.Replace("%23", "#"); 
        }
            return result;
        }
    }
}
