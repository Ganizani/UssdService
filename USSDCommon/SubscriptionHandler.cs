using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.components;
using System.Collections;
using ExactPaymentWrapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using exactmobile.ussdcommon;

namespace exactmobile.ussdcommon
{
    public class SubscriptionHandler
    {
        public SubscriptionService service;
        private CQueueWrapper SendQueue = null;
        private int trackId = 3;
        private string billingEngineAddress = "192.168.1.79";
        private int billingEnginePort = 11757;
               
        public SubscriptionHandler(int subscriptionServiceID)
        {            
            this.service = new SubscriptionService(subscriptionServiceID);
        }

        public SubscriptionResponse Subscribe(string mobileNumber, InputMechanisms inputMechanism, string keyword, string shortCode, int reference)
        {
            SubscriptionResponse response = new SubscriptionResponse
            {
                MobileNumber = mobileNumber,
                Service = this.service,
                NetworkID = CUtility.GetNetworkID(mobileNumber)
            };
            
            if (IsSubscribed(mobileNumber, this.service.ID))
            {
                response.Status = ServiceMessages.AlreadySubscribed;
                return response;
            }

            response.SubscriptionID = AddSubscription(service.ID, mobileNumber, response.NetworkID, reference, shortCode, keyword);
            SendSubscribeResponse(service, response.SubscriptionID, mobileNumber, reference, CSubscriptionEngineCapsule.RequestResults.Success);

            AddSubscriptionInteraction(response.SubscriptionID, (int)inputMechanism, InteractionTypes.Subscribe);
            AddSubscriptionInteraction(response.SubscriptionID, (int)inputMechanism, InteractionTypes.Confirmation);
            
            PaymentWrapper.WrapperResponse paymentResponse = null;
            bool result = Execute(service.PaymentSystemClientID, service.ID, response.SubscriptionID, mobileNumber, response.NetworkID, service.BillingAmount, true,reference ,out paymentResponse);

            if (!result)
            {
                response.Exception = string.Format("{0}: {1}", paymentResponse.statusDescription, paymentResponse.Exception);
                response.Status = ServiceMessages.FailedBillings;
                return response;
            }

            response.Status = ServiceMessages.SubscriptionActivated;
            
            return response;
        }

        public bool IsSubscribed(string mobileNumber, int serviceID)
        {
            CDB cdb = new CDB("subscription");
            bool isSub = (bool)cdb.Execute("SUB_IsSubscribed '" + mobileNumber + "'," + serviceID, CDB.exmReturnTypes.RETURN_SCALAR);
            cdb.Close();

            return isSub;
        }
                
        private bool Execute(int paymentSystemClientId, int subscriptionServiceId, int subscriptionId, string mobileNumber, int mobileNetworkId, decimal amount, bool initialBilling, int reference, out PaymentWrapper.WrapperResponse response)
        {        
            int transactionID = 0;
            int statusID = 0;

            bool success = BillUser(paymentSystemClientId, subscriptionId, mobileNumber, mobileNetworkId, amount, initialBilling, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), out transactionID, out statusID, out response);
           
            if (success)            
                ExtendSubscription(subscriptionServiceId, subscriptionId, mobileNumber, mobileNetworkId, 5, reference);                           
            else
            {                                   
                AddBillingRetry(subscriptionId, false, amount);                                        
                SendUpdate(this.service, subscriptionId, DateTime.MinValue, Statuses.DoesNotExist, reference, CSubscriptionEngineCapsule.RequestResults.Success, false);                
            }

            return success;
        }


        private int AddSubscription(int serviceID, string mobileNumber, int mobileNetworkID, int reference, string shortCode, string keyword)
        {
            CDB DB = new CDB("Typheous - ");
            int subscriptionID = (int)DB.Execute(
                "SUB_AddSubscription "
                    + " @SubscriptionServiceID=" + serviceID.ToString()
                    + ",@MobileNumber=" + mobileNumber
                    + ",@MobileNetworkID=" + mobileNetworkID.ToString()
                    + ",@StatusID=" + (int)Statuses.Pending
                    + ",@Reference=" + reference.ToString()
                // @MarketingIDRef  does not get set here, but that is OK, as it gets defaulted to 0 in the stored procedure
                    + ",@ShortCode=" + shortCode
                    + ",@Keyword=" + keyword
                , CDB.exmReturnTypes.RETURN_SCALAR
            );
            DB.Close();
            DB = null;

            return subscriptionID;
        }

        private static void TruncatePendingBillingRequests()
        {
            SqlDatabase database = new SqlDatabase(ussdcommon.DB.ExactDotNet.ConnectionString, "truncate table pending billing request");
            database.Open();
            database.CommandText = "Truncate_SubscriptionPendingBillingRequests";
            database.ExecuteNonQuery();
            database.Close();

        }

        private static void DeletePendingBillingRequest(int subscriptionID)
        {
            SqlDatabase database = new SqlDatabase(ussdcommon.DB.ExactDotNet.ConnectionString, "Delete pending billing request");
            database.Open();
            database.CommandText = "Delete_SubscriptionPendingBillingRequest";
            database.AddParameter("@subscriptionID", SqlDbType.Int, subscriptionID);
            database.ExecuteNonQuery();
            database.Close();
        }

        private void UnsubscribeUser(int subscriptionServiceID, int subscriptionID, string mobileNumber, int networkID, int reference)
        {
            //SubscriptionService subscriptionService = GetSubscriptionService(subscriptionServiceID);

            //UpdateSubscriptionStatus(subscriptionID, Statuses.Inactive);
            AddSubscriptionInteraction(subscriptionID, 56, InteractionTypes.Unsubscribe);
            UpdateSubscriptionStatus(subscriptionID, 345);

            SendServiceMessage(subscriptionServiceID, subscriptionID, mobileNumber, networkID, null, null, ServiceMessages.SubscriptionDeactivated);
            CSubscriptionEngineCapsule.RequestResults requestResult = CSubscriptionEngineCapsule.RequestResults.Success;

            SendUnsubscribeResponse(this.service, subscriptionID, reference, requestResult);
        }


        private bool CheckAndExtendBillingRetry(int billingRetryID)
        {
            CDB DB = new CDB("CheckAndExtendBillingRetry");
            bool cancelled = (bool)DB.Execute("Sub_SubscriptionCheckAndExtend " + billingRetryID.ToString(), CDB.exmReturnTypes.RETURN_SCALAR);
            DB.Close();
            DB = null;

            return cancelled;
        }

        private void AddBillingRetry(int subscriptionID, bool initialBilling, decimal amount)
        {
            CDB DB = new CDB("AddBillingRetry");
            DB.Execute("SUB_AddBillingRetry " + subscriptionID.ToString() + "," + (initialBilling ? "1" : "0") + "," + amount.ToString(), CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

        private void SendUnsubscribeResponse(SubscriptionService service, int subscriptionID, int reference, CSubscriptionEngineCapsule.RequestResults result)
        {
            CXMLCapsule capsule = CSubscriptionEngineCapsule.CreateUnsubscribeConfirmationCapsule(service.ID, subscriptionID, reference, result);
            service.ResponseQueue.Send(capsule);
            capsule = null;
        }

        private void SendSubscribeResponse(SubscriptionService service, int subscriptionID, string mobileNumber, int reference, CSubscriptionEngineCapsule.RequestResults result)
        {
            CXMLCapsule capsule = CSubscriptionEngineCapsule.CreateSubscribeConfirmationCapsule(service.ID, subscriptionID, mobileNumber, reference, result);
            service.ResponseQueue.Send(capsule);
            capsule = null;
        }

        private bool BillUser(int paymentSystemClientID, int subscriptionID, string mobileNumber, int mobileNetworkID, decimal amount, bool initialBilling, string startDate, out int transactionId, out int statusID, out PaymentWrapper.WrapperResponse response)
        {
            response = null;

            statusID = 0;
            bool success = false;
            transactionId = 0;

            //hack for CellC
            if (mobileNetworkID == 3)
            {
                if (amount == 4.99m)
                    amount = 5m;

                if (amount == 3.99m)
                    amount = 4m;
            }

            try
            {

                PaymentWrapper payment = new PaymentWrapper(this.ToString(), this.billingEngineAddress, this.billingEnginePort, "Exactmobile", "ex@ctmobil3", paymentSystemClientID);
                //PaymentWrapper payment = new PaymentWrapper(ConfigurationSettings.AppSettings["billingEngineAddress"],"Exactmobile", "ex@ctmobil3", paymentSystemClientID);
                //payment.EnableRetry = true;
                //payment.MaxRetry = 120; // 120 * 5000 = 600sec (10 minutes)

                response = null;

                try
                {
                    //Common.Log.InfoWrite(string.Format("{0}", "+"), ConsoleColor.Green);
                    if (startDate != null && startDate != "")
                        response = payment.Execute(mobileNumber, mobileNetworkID, "SubscriptionService", "SUB" + subscriptionID, amount, initialBilling, true, DateTime.Parse(startDate));
                    else
                        response = payment.Execute(mobileNumber, mobileNetworkID, "SubscriptionService", "SUB" + subscriptionID, amount);
                    //Common.Log.InfoWrite(string.Format("{0}", "-"), ConsoleColor.Magenta);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR sending billing request: " + ex.Message);
                }
                finally
                {
                    payment.Close();
                }

                transactionId = payment.OldTransactionID;
                statusID = response.statusID;

                switch (response.statusID)
                {
                    case 1:
                        success = true;
                        break;
                    case 37://Invalid MSISDN	
                    //case 172://Subscription not valid
                    //case 115:
                    //    unsubscribe = true;
                    //    break;
                    //case 120://Subscriber locked
                    //    //					case 91://Subscriber barred
                    //    unsubscribe = CanUnsubScribeLockedUser(subscriptionID);
                    //    break;
                    default:
                        success = false;
                        break;
                }

                AddBillingRequest(subscriptionID, payment.OldTransactionID, initialBilling, response.transactionID, response.statusID, amount);

            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }

        


        private void SendServiceMessage(int serviceID, int subscriptionID, string mobilenumber, int mobileNetworkID, string extraDigits, string destinationNumber, ServiceMessages serviceMessage)
        {
            SendServiceMessage(serviceID, subscriptionID, mobilenumber, mobileNetworkID, extraDigits, destinationNumber, serviceMessage, null);
        }

        private void SendServiceMessage(int serviceID, int subscriptionID, string mobilenumber, int mobileNetworkID, string extraDigits, string destinationNumber, ServiceMessages serviceMessage, string overRideMessage)
        {
            string messages = null;


            if (overRideMessage == null)
            {
                CDB DB = new CDB("subscription");
                messages = (string)DB.Execute("SUB_GetSubscriptionServiceMessage " + serviceID.ToString() + "," + ((int)serviceMessage).ToString(), CDB.exmReturnTypes.RETURN_SCALAR);
                DB.Close();
                DB = null;
            }
            else
            {
                messages = overRideMessage;
            }

            if (messages == null)
            {
                return;
            }

            if (messages.Length == 0)
            {
                return;
            }

            messages = messages.Trim();

            ArrayList message = new ArrayList();

            while (messages.Length > 0)
            {
                int index = messages.IndexOf("###");

                if (index > 0)
                {
                    string msg = messages.Substring(0, index).Trim();

                    if (msg.Length > 160)
                    {
                        msg = msg.Substring(0, 160);
                    }

                    message.Add(msg);
                    messages = messages.Substring(index + 3).Trim();
                }
                else
                {
                    messages = messages.Trim();

                    if (messages.Length > 160)
                    {
                        messages = messages.Substring(0, 160);
                    }

                    message.Add(messages);

                    break;
                }
            }

            foreach (string msg in message)
            {
                if (mobileNetworkID == 3 && msg == "Welcome to UNREAL! To access all the specials, go to www.exactmobile.mobi/unreal (on WAP) or dial *120*120#. Cost: R10/week (R7.50/week for Club members).")
                {
                    SendMessage(serviceID, subscriptionID, mobilenumber, mobileNetworkID, extraDigits, destinationNumber, "Welcome to UNREAL! To access all the great specials, go to www.exactmobile.mobi/unreal by WAP on yr phone. Cost: R10/week (R7.50/week for Club members).");
                }
                else
                {
                    SendMessage(serviceID, subscriptionID, mobilenumber, mobileNetworkID, extraDigits, destinationNumber, msg);
                }
            }
        }

        private void SendMessage(int serviceID, int subscriptionID, string mobilenumber, int mobileNetworkID, string extraDigits, string destinationNumber, string message)
        {
            int transactionID = AddMessageTransaction(serviceID, subscriptionID, mobilenumber, mobileNetworkID, extraDigits, destinationNumber, message);

            CSMSCapsule capsule = new CSMSCapsule(mobilenumber, "", true, CSMSCapsule.MessageTypes.DEFAULT, 1, 5, 7, 5, mobileNetworkID, false, CUtility.ToHex(message), false);

            if (destinationNumber != null && destinationNumber != "")
            {
                capsule.SetParameter("DESTINATIONADDRESS", destinationNumber, true);
            }

            if (extraDigits != null && extraDigits != "")
            {
                capsule.SetParameter("EXTRADIGITS", extraDigits, true);
            }

            capsule.SetParameter("SUB_MESSAGETRANSACTIONID", transactionID.ToString(), true);

            SendQueue.Send(capsule);
            capsule = null;
        }

        private int AddMessageTransaction(int serviceID, int subscriptionID, string mobileNumber, int mobileNetworkID, string extraDigits, string destinationNumber, string message)
        {
            CDB DB = new CDB("subscriptions");
            int transactionID = (int)DB.Execute("SUB_AddMessageTransaction " + (serviceID == 0 ? "null" : serviceID.ToString()) + "," + (subscriptionID == 0 ? "null" : subscriptionID.ToString()) + ",'" + mobileNumber + "'," + mobileNetworkID.ToString() + "," + (extraDigits == "" ? "null" : "'" + extraDigits + "'") + "," + (destinationNumber == "" ? "null" : "'" + destinationNumber + "'") + ",'" + message.Replace("'", "''") + "'", CDB.exmReturnTypes.RETURN_SCALAR);
            DB.Close();
            DB = null;

            return transactionID;
        }

        private bool CanUnsubScribeLockedUser(int subscriptionID)
        {
            CDB cdb = new CDB("subscriptions");
            bool unsubscribe = (bool)cdb.Execute("exm_CanUnsubLocked " + subscriptionID, CDB.exmReturnTypes.RETURN_SCALAR);
            cdb.Close();

            return unsubscribe;
        }

        private void AddBillingRequest(int subscriptionID, int paymentSystemTransactionID, bool initialBilling, int exactPaymentTransactionId, int exactPaymentStatusID, decimal exactPaymentAmount)
        {
            CDB DB = new CDB("Add Subscription rebilling Request");
            DB.Execute("SUB_AddBillingRequest " + subscriptionID.ToString() + "," + paymentSystemTransactionID.ToString() + "," + (initialBilling ? "1" : "0") + "," + exactPaymentTransactionId + "," + exactPaymentStatusID + "," + exactPaymentAmount, CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

        private void UpdateRebillingStatus(int billingRetryID, int retryCount, int statusID)
        {

            CDB cdb = new CDB("Update Subscription REbilling Status");

            cdb.Execute("sub_UpdateBillingContinuesStatus " + billingRetryID + "," + statusID + "," + retryCount, CDB.exmReturnTypes.RETURN_NON_QUERY);

            cdb.Close();
        }

        private void ExtendSubscription(int serviceID, int subscriptionID, string mobileNumber, int mobileNetworkID, int inputMechanismID, int reference)
        {
            //SubscriptionService service = GetSubscriptionService(serviceID);

            AddSubscriptionInteraction(subscriptionID, inputMechanismID, InteractionTypes.Extention);

            DateTime expiryDateTime = CalculateAndUpdateSubscriptionDateTimes(subscriptionID, service);

            UpdateSubscriptionStatus(subscriptionID, Statuses.Active);

            SendUpdate(this.service, subscriptionID, expiryDateTime, Statuses.Active, reference, CSubscriptionEngineCapsule.RequestResults.Success, false);

            // SendServiceMessage(serviceID, subscriptionID, mobileNumber, mobileNetworkID, null, null, ServiceMessages.SubscriptionExtended);
        }


        private void ExtendSubscription(int serviceID, int subscriptionID, int reference, DateTime nextBillingDate, int inputMechanismId)
        {
            //SubscriptionService service = GetSubscriptionService(serviceID);

            AddSubscriptionInteraction(subscriptionID, inputMechanismId, InteractionTypes.Extention);

            //DateTime expiryDateTime = CalculateAndUpdateSubscriptionDateTimes(subscriptionID, service);

            UpdateSubscriptionStatus(subscriptionID, Statuses.Active);

            SendUpdate(this.service, subscriptionID, nextBillingDate, Statuses.Active, reference, CSubscriptionEngineCapsule.RequestResults.Success, false);

            // SendServiceMessage(serviceID, subscriptionID, mobileNumber, mobileNetworkID, null, null, ServiceMessages.SubscriptionExtended);
        }

        private void SendUpdate(SubscriptionService service, int subscriptionID, DateTime expiryDateTime, Statuses status, int reference, CSubscriptionEngineCapsule.RequestResults result, bool initialBilling)
        {
            Statuses actualStatus = status;
            DateTime actualExpiryDateTime = expiryDateTime;

            if (actualStatus == Statuses.DoesNotExist)
            {
                actualStatus = GetSubscriptionStatus(subscriptionID);
            }

            if (actualExpiryDateTime == DateTime.MinValue)
            {
                actualExpiryDateTime = GetSubscriptionExpiryDateTime(subscriptionID);
            }

            CXMLCapsule capsule = CSubscriptionEngineCapsule.CreateSubscriptionUpdateCapsule(service.ID, subscriptionID, actualExpiryDateTime, (int)actualStatus, reference, result, initialBilling);
            service.ResponseQueue.Send(capsule);
            capsule = null;
        }

        private Statuses GetSubscriptionStatus(int subscriptionID)
        {
            CDB DB = new CDB("GetSubscriptionStatus");
            Statuses status = (Statuses)(int)DB.Execute("SUB_GetSubscriptionStatus " + subscriptionID.ToString(), CDB.exmReturnTypes.RETURN_SCALAR);
            DB.Close();
            DB = null;

            return status;
        }

        private DateTime GetSubscriptionExpiryDateTime(int subscriptionID)
        {
            CDB DB = new CDB("GetSubscriptionExpiryDateTime");

            DateTime dateTime = DateTime.MinValue;

            object dt = DB.Execute("SUB_GetSubscriptionExpiryDateTime " + subscriptionID.ToString(), CDB.exmReturnTypes.RETURN_SCALAR);

            if (dt != null)
            {
                if (dt != DBNull.Value)
                {
                    dateTime = (DateTime)dt;
                }
            }

            DB.Close();
            DB = null;

            return dateTime;
        }

        private DateTime CalculateAndUpdateSubscriptionDateTimes(int subscriptionID, SubscriptionService service)
        {
            DateTime expiryDateTime = DateTime.MinValue;
            DateTime lastBillingDateTime = DateTime.MinValue;
            DateTime nextBillingDateTime = DateTime.MinValue;

            CDB DB = new CDB("Get Subscription Date - ");
            SqlDataReader reader = (SqlDataReader)DB.Execute("SUB_GetSubscriptionDateTimes " + subscriptionID.ToString(), CDB.exmReturnTypes.RETURN_READER);

            if (reader.Read())
            {
                expiryDateTime = reader.IsDBNull(0) ? DateTime.MinValue : reader.GetDateTime(0);
                lastBillingDateTime = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1);
                nextBillingDateTime = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2);
            }

            reader.Close();
            reader = null;

            DB.Close();
            DB = null;

            if (expiryDateTime < DateTime.Now)
            {
                expiryDateTime = DateTime.Now;
            }

            switch (service.BillingFrequency)
            {
                case BillingFrequencies.Daily:
                    expiryDateTime = expiryDateTime.AddDays(1D);
                    break;
                case BillingFrequencies.Weekly:
                    expiryDateTime = expiryDateTime.AddDays(7D);
                    break;
                case BillingFrequencies.Monthly:
                    expiryDateTime = expiryDateTime.AddMonths(1);
                    break;
            }

            nextBillingDateTime = expiryDateTime;

            switch (service.BillingFrequency)
            {
                case BillingFrequencies.Weekly:
                case BillingFrequencies.Monthly:
                    nextBillingDateTime = expiryDateTime.AddHours(-48D);
                    break;
                default:
                    nextBillingDateTime = expiryDateTime;
                    break;
            }

            //	This has been added to set all future billings @ 1AM
            nextBillingDateTime = new DateTime(nextBillingDateTime.Year, nextBillingDateTime.Month, nextBillingDateTime.Day, 00, 01, 01);

            UpdateSubscriptionDateTimes(subscriptionID, expiryDateTime, nextBillingDateTime, trackId);

            return expiryDateTime;
        }

        private void UpdateSubscriptionDateTimes(int subscriptionID, DateTime expiryDateTime, DateTime nextBillingDateTime, int trackingId)
        {

            CDB DB = new CDB(this.ToString());
            DB.Execute("SUB_UpdateSubscriptionDateTimes " + subscriptionID.ToString() + ",'" + expiryDateTime.ToString("yyyy-MM-dd HH:mm:ss:fff") + "','" + nextBillingDateTime.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'," + trackingId, CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

       
        private void AddSubscriptionInteraction(int subscriptionID, int requestInputMechanismID, InteractionTypes interactionType)
        {
            CDB DB = new CDB("AddSubscriptionInteraction");
            DB.Execute("SUB_AddSubscriptionInteraction " + subscriptionID.ToString() + "," + requestInputMechanismID.ToString() + "," + ((int)interactionType).ToString(), CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

        private void UpdateSubscriptionStatus(int subscriptionID, Statuses status)
        {
            CDB DB = new CDB("UpdateSubscriptionStatus");
            DB.Execute("SUB_UpdateSubscriptionStatus " + subscriptionID.ToString() + "," + ((int)status).ToString(), CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

        private void UpdateSubscriptionStatus(int subscriptionID, int statusID)
        {
            CDB DB = new CDB("UpdateSubscriptionStatus");
            DB.Execute("SUB_UpdateSubscriptionStatus " + subscriptionID.ToString() + "," + statusID.ToString(), CDB.exmReturnTypes.RETURN_NON_QUERY);
            DB.Close();
            DB = null;
        }

       
    }
}


public class SubscriptionResponse
{
    public SubscriptionService Service { get; set; }
    public int SubscriptionID { get;  set; }
    public string MobileNumber { get;  set; }
    public int NetworkID { get;  set; }
    public ServiceMessages Status { get;  set; }
    public string Exception { get;  set; }
}


public enum BillingFrequencies
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3
}

public enum BillingTimes
{
    Exact = 1,
    DayTime = 2,
    Specific = 3
}

public enum InteractionTypes
{
    Subscribe = 1,
    Confirmation = 2,
    Billing = 3,
    Unsubscribe = 4,
    SMSBillingRequest = 5,
    Activation = 6,
    Extention = 7
}

public enum Statuses
{
    DoesNotExist = -1,
    Failed = 1,
    Pending = 2,
    Active = 13,
    Inactive = 14,
    Frozen = 282,
    Dormant = 307,
    Invalid = 322
}

public enum ServiceMessages
{
    ConfirmationRequest = 1,
    SMSBillingRequest = 2,
    SubscriptionActivated = 3,
    SubscriptionDeactivated = 4,
    AlreadySubscribed = 5,
    ErrorMessage = 6,
    MobileNetworkNotSupported = 7,
    SubscriptionExtended = 8,
    FailedBillings = 9,
    Reminder = 10
    
}

public enum InputMechanisms : int
{ 
    IVR = 1,
    XCS = 2,
    WEB = 3,
    PRS = 4,
    SYSTEM_GENERATED = 5,
    WAP = 6,
    USSD = 20
}

