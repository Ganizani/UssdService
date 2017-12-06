using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using exactmobile.ussdservice.common.menu;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using exactmobile.components;
using System.Net.Mail;
using USSD.BLL;
using USSD.Entities;

namespace exactmobile.ussdcommon
{
    public static class Transaction
    {
        public static void Add(USSDSession session, string ussdString, out long transactionID)
        {            
            transactionID = -1;

                USSDTransaction model =  new USSDTransaction {
                USSDCampaignId= session.Campaign.CampaignID,
                MobileNumber= session.MSISDN,
                Message= ussdString,
                USSDNumberId =  session.Campaign.USSDNumberID,
                CreatedByUserId = 1,
                CreatedDate = DateTime.Now,
                SessionGuid =  session.SessionID,
                MobileNetworkId= 1 //Vodacom only for now but this need to be dynamic
                };
            Logic.Instance.USSDTransactions.Insert(model);
            transactionID = model.Id;


        }

        public static void InsertSMS(int campaignId, string mobilenumber, string message)
        {
            Database subscriptionsDB = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            DbCommand command = subscriptionsDB.GetStoredProcCommand("sp_InsertSmsOutgoing",new object[]{campaignId,mobilenumber,message});
            
            subscriptionsDB.ExecuteNonQuery(command);

            command.Dispose();
        }

        public static void InsertEmail(string smtpServer, MailMessage message, int campaignID, bool sent)
        {
            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            var command = db.GetStoredProcCommand("[usp_InsertEmailTransaction]");
            StringBuilder recipients = new StringBuilder();
            if (message.To != null && message.To.Count != 0)
            {
                recipients.Append("To: ");
                foreach (var to in message.To)
                    recipients.AppendFormat("{0};",to.Address);                
            }
            if (message.CC != null && message.CC.Count != 0)
            {
                recipients.Append("CC: ");
                foreach (var cc in message.CC)
                    recipients.AppendFormat("{0};", cc.Address);
            }
            if (message.Bcc != null && message.Bcc.Count != 0)
            {
                recipients.Append("Bcc: ");
                foreach (var bcc in message.Bcc)
                    recipients.AppendFormat("{0};", bcc.Address);
            }
            db.AddInParameter(command,"@ussdCampaignID", DbType.Int32,campaignID);
            db.AddInParameter(command, "@recipients", DbType.String, recipients.ToString());
            db.AddInParameter(command, "@subject", DbType.String, Convert.ToString(message.Subject));
            db.AddInParameter(command, "@emailBody", DbType.String, message.Body);
            db.AddInParameter(command, "@smtpServer", DbType.String, smtpServer);
            db.AddInParameter(command,"@sent", DbType.Boolean,sent);

            using(command)
                db.ExecuteNonQuery(command);
        }

        public static void InsertEmail(string smtpServer, MailMessage message, int campaignID, bool sent, string exception)
        {            
            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            var command = db.GetStoredProcCommand("[usp_InsertEmailTransaction]");
            StringBuilder recipients = new StringBuilder();
            if (message.To != null && message.To.Count != 0)
            {
                recipients.Append("To: ");
                foreach (var to in message.To)
                    recipients.AppendFormat("{0};", to.Address);
            }
            if (message.CC != null && message.CC.Count != 0)
            {
                recipients.Append("CC: ");
                foreach (var cc in message.CC)
                    recipients.AppendFormat("{0};", cc.Address);
            }
            if (message.Bcc != null && message.Bcc.Count != 0)
            {
                recipients.Append("Bcc: ");
                foreach (var bcc in message.Bcc)
                    recipients.AppendFormat("{0};", bcc.Address);
            }
            db.AddInParameter(command, "@ussdCampaignID", DbType.Int32, campaignID);
            db.AddInParameter(command, "@recipients", DbType.String, recipients.ToString());
            db.AddInParameter(command, "@subject", DbType.String, Convert.ToString(message.Subject));
            db.AddInParameter(command, "@emailBody", DbType.String, message.Body);
            db.AddInParameter(command, "@smtpServer", DbType.String, smtpServer);
            db.AddInParameter(command, "@sent", DbType.Boolean, sent);
            db.AddInParameter(command, "@exception", DbType.String, exception);
            using(command)
                db.ExecuteNonQuery(command);
        }
    }
}
