using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.components;

namespace exactmobile.ussdcommon
{
    public class SMS
    {
        
        public static void sendSMS(int campaignId, string recipient, string message)
        {
            CQueueWrapper smsSender = new CQueueWrapper("smsSterkinekorQ");
            CSMSCapsule sms = new CSMSCapsule(recipient, "", false, CSMSCapsule.MessageTypes.DEFAULT, 1, 5, 7, 5, CUtility.GetNetworkID(recipient), false, CUtility.ToHex(message), false);

            smsSender.Send(sms);
            smsSender = null;
            sms = null;

            Transaction.InsertSMS(campaignId, recipient, message);
        }
        
    }
}
