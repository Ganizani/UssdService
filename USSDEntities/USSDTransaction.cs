using System;
using MBDInternal.Data;
namespace USSD.Entities
{
    [DataClass(
        ListProcedure = "ListUSSDTransactions",
        FetchProcedure = "FetchUSSDTransaction",
        UpdateProcedure = "UpdateUSSDTransaction",
        InsertProcedure = "InsertUSSDTransaction"
        )
     ]
    public class USSDTransaction
    {
        #region Fields
        [DataField]
        public Int64 Id { get; set; }
        [DataField]
        public int USSDCampaignId { get; set; }
        [DataField]
        public string Message { get; set; }
        [DataField]
        public string MobileNumber { get; set; }
        [DataField]
        public DateTime CreatedDate { get; set; }
        [DataField]
        public int CreatedByUserId { get; set; }
        [DataField]
        public int USSDNumberId { get; set; }
        [DataField]
        public Guid SessionGuid { get; set; }
        [DataField]
        public int MobileNetworkId { get; set; }
        #endregion

        #region Filters

        #endregion

        #region Resolved Fields
        [DataField]
        public string CampaignName { get; set; }
        [DataField]
        public string Number { get; set; }
        [DataField]
        public string MobileNetwork { get; set; }

        #endregion
    }
}
