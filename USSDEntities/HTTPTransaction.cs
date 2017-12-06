using System;
using MBDInternal.Data;
namespace USSD.Entities
{
    [DataClass(
        ListProcedure = "ListHTTPTransactions",
        FetchProcedure = "FetchHTTPTransaction",
        UpdateProcedure = "UpdateHTTPTransaction",
        InsertProcedure = "InsertHTTPTransaction"
        )
     ]
    public class HTTPTransaction
    {
        [DataField]
        public Int64 Id { get; set; }
        [DataField]
        public int USSDCampaignId { get; set; }
        [DataField]
        public Int64 USSDTransactionId { get; set; }
        [DataField]
        public int MenuId { get; set; }
        [DataField]
        public string MobileNumber { get; set; }
        [DataField]
        public string ResolveURL { get; set; }
        [DataField]
        public string ClientReference { get; set; }
        [DataField]
        public string ClientMessage { get; set; }
        [DataField]
        public string ClientWebResponse { get; set; }
        [DataField]
        public string Errors { get; set; }
        [DataField]
        public DateTime CreatedDate { get; set; }
        [DataField]
        public int CreatedByUserId { get; set; }

    }
}
