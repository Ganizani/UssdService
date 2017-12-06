using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExactPaymentWrapper;
using ExactPaymentInterface;

namespace exactmobile.ussdcommon
{
    public static class OBSBilling
    {
        public static void BillUser(int ussdCampaignID, int menuID, long ussdTransactionID, string mobileNumber, int mobileNetworkID, decimal amount)
        {
            int paymentTCPPort = 11757;
            string configTCPPort;
            string paymentTCPAddress;                        
            string resultMessage = string.Empty;
            decimal billAmount;
            if (utility.CommonUtility.TryGetAppSettings("PaymentSystemTCPPort", true, out configTCPPort))
            {
                if (!int.TryParse(configTCPPort, out paymentTCPPort))
                    paymentTCPPort = 11757;
            }

            if (!utility.CommonUtility.TryGetAppSettings("PaymentSystemTCPAddress", true, out paymentTCPAddress))
                paymentTCPAddress = "192.168.1.79";
                        
            PaymentWrapper.WrapperResponse paymentResponse = null;
            PaymentWrapper payment = null;
            try
            {                
                if (!utility.CommonUtility.ReturnNearestChargeCodeAmount(mobileNetworkID, 1, amount, out billAmount))
                    billAmount = amount;

                payment = new PaymentWrapper("ExactUSSD", paymentTCPAddress, paymentTCPPort, "Exactmobile", "ex@ctmobil3", 266, 228);
                paymentResponse = payment.Execute(mobileNumber, mobileNetworkID, "USSD OBSBilling","ussdmenuobsbilling", billAmount);
            }
            finally
            {
                if (payment != null)
                    payment.Close();
            }
            if (paymentResponse != null)
            {                
                if (paymentResponse.statusID != 1)
                {
                    if (!String.IsNullOrEmpty(paymentResponse.Exception))
                        resultMessage = paymentResponse.Exception;

                    resultMessage = resultMessage + String.Format(" StatusID:{0} Transaction ID: {1} Desc: {2}", paymentResponse.statusID, paymentResponse.transactionID, paymentResponse.statusDescription);
                }
                else
                    resultMessage = paymentResponse.statusDescription;

                OBSBilling.AddMenuOBSBillingTransaction(ussdCampaignID, menuID, ussdTransactionID, mobileNumber, paymentResponse.transactionID, billAmount, paymentResponse.statusID, resultMessage);
            }
        }

        private static bool AddMenuOBSBillingTransaction(int ussdCampaignID, int menuID, long ussdTransactionID, string mobileNumber, int exactpaymentTransactionID, decimal amount, int billingStatusID, string billingResponse)
        {

            using (var command = DB.ExactUSSD.GetStoredProcCommand("[usp_MenuOBSBilling_Insert]"))
            {
                DB.ExactUSSD.AddInParameter(command, "@ussdCampaignIDRef", System.Data.DbType.Int32, ussdCampaignID);
                DB.ExactUSSD.AddInParameter(command, "@ussdTransactionIDRef", System.Data.DbType.Int64, ussdTransactionID);
                DB.ExactUSSD.AddInParameter(command, "@menuIDRef", System.Data.DbType.Int32, menuID);
                DB.ExactUSSD.AddInParameter(command, "@exactpaymentTransactionIDRef", System.Data.DbType.Int32, exactpaymentTransactionID);
                DB.ExactUSSD.AddInParameter(command, "@billingStatusID", System.Data.DbType.Int32, billingStatusID);
                DB.ExactUSSD.AddInParameter(command, "@amount", System.Data.DbType.Decimal, amount);
                DB.ExactUSSD.AddInParameter(command, "@mobileNumber", System.Data.DbType.String, mobileNumber);

                if(!String.IsNullOrEmpty(billingResponse))
                    DB.ExactUSSD.AddInParameter(command, "@billingResponse", System.Data.DbType.String, billingResponse);
                
                using (var reader = DB.ExactUSSD.ExecuteReader(command))
                {
                    if (reader.Read())
                        return (Convert.ToInt32(reader["transactionID"]) >= 1);
                }
            }
            return false;
        }
    }
}
