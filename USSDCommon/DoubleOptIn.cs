using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.components;
using System.Collections;
using exactmobile.ussdservice.common.session;
using System.Globalization;
using System.Data.SqlClient;


namespace exactmobile.ussdcommon
{
    public class DoubleOptIn
    {
        private string[] doubleOptKeywords = new string[] { "yes", "ye", "y", "fine", "ok", "ja", "correct", "confirm", "want", "wish", "eys", "es", "sey", "please", "activate", "reply", "join" };
        private string[] doubleOptNoResponseKeywords = new string[] { "no", "nee", "cancel" };

        private enum State
        {
            None,
            RetrieveDOI,
            ConstructerCall
        }

        private enum BillingFrequency : int
        { 
            Day = 1,
            Week = 2,
            Month = 3
        }

        public enum DoubleOptMessageType : int
        {
            Reply = 1,
            Invalid_Reply = 2,
            Reminder_First = 3,
            Reminder_Second = 4
        }

        public enum DoubleOptStatus : int
        {
            Initial_Request = 850,
            Already_subscribed = 851,
            Message_request_sent = 852,
            Exception = 853,
            Reply_received = 854,
            Invalid_reply_keyword = 855,
            Valid_keyword_received = 856,
            Completed = 857,
            First_Reminder_Sent = 858,
            Second_Reminder_Sent = 859
        }

        private string mobileNumber;
        private int subscriptionServiceID;
        private int extraDigits;
        public DoubleOptTransaction Transaction { get; private set; }
        public string ConfirmationMessage { get; private set; }
        private USSDSession session;
                         
        public DoubleOptIn(USSDSession session, int subscriptionServiceID)
        {
            this.session = session;
            this.mobileNumber = session.MSISDN;
            this.subscriptionServiceID = subscriptionServiceID;
            this.session[State.ConstructerCall.ToString()] = State.ConstructerCall;
            this.ConfirmationMessage = null;
        }
      
        public bool GetDOI()
        {            
            if (this.session[State.RetrieveDOI.ToString()] == null)
            {
                this.session[State.RetrieveDOI.ToString()] = State.RetrieveDOI;

                using (var command = DB.ExactDotNet.GetStoredProcCommand("Get_SubscriptionDoubleOptRequest"))
                {
                    DB.ExactUSSD.AddInParameter(command, "@mobileNumber", System.Data.DbType.String, mobileNumber);
                    DB.ExactUSSD.AddInParameter(command, "@subscriptionServiceID", System.Data.DbType.Int32, subscriptionServiceID);

                    using (var reader = DB.ExactDotNet.ExecuteReader(command))
                    {
                        if (reader.Read())
                        {
                            this.extraDigits = reader.GetInt32(1);
                            this.Transaction = GetDoubleOptTransaction(this.mobileNumber, this.extraDigits);
                           
                            return this.Transaction == null ? false : true;
                        }
                    }
                }
            }

            return false;
        }

        private DoubleOptTransaction GetDoubleOptTransaction(string mobileNumber, int extraDigit)
        {
            DoubleOptTransaction transaction = null;

            using (var command = DB.ExactDotNet.GetStoredProcCommand("Get_SubscriptionDoubleOptTransaction"))
            {
                DB.ExactUSSD.AddInParameter(command, "@mobileNumber", System.Data.DbType.String, mobileNumber);
                DB.ExactUSSD.AddInParameter(command, "@extradigit", System.Data.DbType.Int32, extraDigit);

                using (var reader = DB.ExactDotNet.ExecuteReader(command))
                {
                    if (reader.Read())
                    {                       
                        transaction = new DoubleOptTransaction
                        {
                            TransactionID = reader.GetInt32(0),
                            SubscriptionServiceID = reader.GetInt32(1),
                            Reference = reader.GetInt32(2),
                            InitialBillingAmount = reader.GetDecimal(3),
                            InputMechanismID = reader.GetInt32(4),
                            OverrideMessage = reader.IsDBNull(5) ? null : reader.GetString(5),
                            ExpiryDateTime = reader.IsDBNull(6) ? DateTime.Now : reader.GetDateTime(6),
                            TRTransactionID = reader.GetInt32(7),
                            ShortCode = reader.IsDBNull(8) ? null : reader.GetString(8),
                            Keyword = reader.IsDBNull(9) ? null : reader.GetString(9),
                        };
                    }
                }
            }
                       
            return transaction;
        }

        public SubscriptionResponse SendSubscribeRequest()
        {
            return Subscribe();
        }

        private SubscriptionResponse Subscribe()
        {                        
            return ValidateAndSubscribeRequest(this.Transaction);
        }

        private SubscriptionResponse ValidateAndSubscribeRequest(DoubleOptTransaction transaction)
        {
            var response = new SubscriptionResponse();
            SubscriptionHandler handler = new SubscriptionHandler(this.subscriptionServiceID);

            if (transaction != null)
            {
                try
                {                    
                    InsertDoubleOptTRTransaction(transaction.TRTransactionID, mobileNumber, transaction.SubscriptionServiceID, DoubleOptStatus.Reply_received,InputMechanisms.USSD);
                    
                    if (transaction != null)
                    {
                        InsertDoubleOptTRTransaction(transaction.TRTransactionID, mobileNumber, transaction.SubscriptionServiceID, DoubleOptStatus.Valid_keyword_received, InputMechanisms.USSD);
                                              
                        response = handler.Subscribe(mobileNumber, InputMechanisms.USSD, this.Transaction.Keyword, this.Transaction.ShortCode, this.Transaction.Reference);

                        if (response.Status == ServiceMessages.SubscriptionActivated)
                        {
                            InsertDoubleOptTRTransaction(transaction.TRTransactionID, mobileNumber, transaction.SubscriptionServiceID, DoubleOptStatus.Completed, null, response.SubscriptionID,InputMechanisms.USSD);
                            DeleteDoubleoptTransaction(transaction.TransactionID);
                        }
                        else
                        {
                            InsertDoubleOptTRTransaction(transaction.TRTransactionID, mobileNumber, transaction.SubscriptionServiceID, DoubleOptStatus.Exception,string.Format("{0}",response.Exception),response.SubscriptionID,InputMechanisms.USSD);
                        }
                    }
                                                   
                }
                catch (Exception ex)
                {
                    InsertDoubleOptTRTransaction(transaction.TRTransactionID, mobileNumber, transaction.SubscriptionServiceID, DoubleOptStatus.Exception, ex.Message, null,InputMechanisms.USSD);
                }
            }

            return response;       
        }

        public int InsertDoubleOptTRTransaction(int parentTransactionID, string mobileNumber, int subscriptionServiceID, DoubleOptStatus status, InputMechanisms inputMechanism)
        {
            return InsertDoubleOptTRTransaction(parentTransactionID, mobileNumber, subscriptionServiceID, status, null, null, inputMechanism);
        }

        public int InsertDoubleOptTRTransaction(int parentTransactionID, string mobileNumber, int subscriptionServiceID, DoubleOptStatus status, string exception, int? subscriptionID, InputMechanisms inputMechanism)
        {
            int transactionID = 0;

            if (!string.IsNullOrEmpty(exception))
                exception = "'" + exception.Replace("'", "''") + "'";
            else
                exception = "NULL";


            using (CDB cdb = new CDB("Insert double OPT TR Transaction"))
            {
                string queryString = string.Format("Insert_trSubscriptionDoubleOpt {0},'{1}',{2},{3},{4},{5},{6}", parentTransactionID, mobileNumber, subscriptionServiceID, (int)status, exception, subscriptionID == null ? "NULL" : subscriptionID.ToString(), (int)inputMechanism);

                transactionID = int.Parse(cdb.Execute(queryString, CDB.exmReturnTypes.RETURN_SCALAR).ToString());
            }

            return transactionID;
        }

        private bool ValidateDoubleOptKeyword(string shortMessage, string[] source)
        {
            foreach (string keyword in source)
            {
                if (keyword.ToLower() == shortMessage)
                {
                    return true;
                }
            }

            return false;
        }

        private void DeleteDoubleoptTransaction(int transactionID)
        {
            using (CDB cdb = new CDB("Delete double OPT transaction"))
            {
                cdb.Execute(string.Format("Delete_SubscriptionDoubleOptTransaction {0}", transactionID), CDB.exmReturnTypes.RETURN_NON_QUERY);
            }
        }

        
     
        public class DoubleOptTransaction
        {
            public int TransactionID { get; set; }
            public int SubscriptionServiceID { get; set; }
            public int Reference { get; set; }
            public decimal InitialBillingAmount { get; set; }
            public int InputMechanismID { get; set; }
            public string OverrideMessage { get; set; }
            public DateTime ExpiryDateTime { get; set; }
            public int TRTransactionID { get; set; }
            public string ShortCode { get; set; }
            public string Keyword { get; set; }
        }

    }
}
