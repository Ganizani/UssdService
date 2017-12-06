using System;
using MBDInternal.Data;
namespace USSD.Entities
{
    [DataClass(
        InsertProcedure = "InsertMenuTransaction",
        FetchProcedure = "FetchMenuTransaction",
        ListProcedure = "ListMenuTransactions",
        UpdateProcedure = "UpdateMenuTransaction"
        )
     ]
    public class MenuTransaction
    {

        [DataField]
        public Int64 Id { get; set; }
        [DataField]
        public Int32 USSDMenuId { get; set; }
        [DataField]
        public Int32 USSDCampaignId { get; set; }
        [DataField]
        public Int64 USSDTransactionId { get; set; }
        [DataField]
        public Int32 MobileNetworkID { get; set; }
        [DataField]
        public string MobileNumber { get; set; }
        [DataField]
        public string EnteredSelectionValue { get; set; }
        [DataField]
        public Int32 MenuSection { get; set; }
        [DataField]
        public DateTime CreateDate { get; set; }


        #region Filters
        [DataField]
        public Int64? FilterUSSDMenuId { get; set; }
        [DataField]
        public Int32? FilterUSSDCampaignId { get; set; }
        [DataField]
        public Int64? FilterUSSDTransactionId { get; set; }
        #endregion
    }
}
