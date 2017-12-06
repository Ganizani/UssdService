using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using USSD.BLL;

namespace exactmobile.ussdservice.common.menu
{
    public sealed class MenuTransaction
    {
        private MenuTransaction()
        {
        }
        
        public static bool Add(int menuIDRef, int USSDCampaignIDRef, long USSDTransactionIDRef, int mobileNetworkID, string mobileNumber, string enteredSelectionValue,int MenuSection, out long transactionID)
        {          
            
            transactionID = -1;
            var model = new USSD.Entities.MenuTransaction {
                USSDMenuId = menuIDRef,
                USSDCampaignId = USSDCampaignIDRef,
                USSDTransactionId = USSDTransactionIDRef,
                MobileNetworkID = mobileNetworkID ==0? 1:mobileNetworkID,
                MobileNumber = mobileNumber,
                CreateDate = DateTime.Now,
                EnteredSelectionValue = enteredSelectionValue,
                MenuSection = MenuSection
            };
            Logic.Instance.MenuTransactions.Insert(model);
            transactionID = model.Id;
            
            return (transactionID >= 1);
        }
    }
}
