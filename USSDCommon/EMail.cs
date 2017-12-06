using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace exactmobile.ussdcommon
{
    public class EMail
    {
        private EMail()
        { 
        }

        public static bool SendEmail(MailMessage message, int campaignID)
        {
            bool sent = false;
            string smtpServer;
            string sendTimeout;
            if(!utility.CommonUtility.TryGetAppSettings("SMTPServer",true,out smtpServer))
                smtpServer = "192.168.1.110";
            if (!utility.CommonUtility.TryGetAppSettings("SMTPSendTimeout", true, out sendTimeout))
                sendTimeout = string.Empty;
            try
            {
                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Send(message);
                sent = true;

                int timeout;
                if (!String.IsNullOrEmpty(sendTimeout) && int.TryParse(sendTimeout, out timeout))
                    smtpClient.Timeout = timeout;

                Transaction.InsertEmail(smtpServer,message,campaignID,sent);
            }
            catch (Exception ex)
            {
                Transaction.InsertEmail(smtpServer, message, campaignID, sent, ex.ToString());
            }
            return sent;
        }
        
    }
}
