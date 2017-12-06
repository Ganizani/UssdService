using System;
using MBDInternal.Data;
namespace USSD.Entities
{
    [DataClass(
        ListProcedure = "ListUSSDCampaigns",
        FetchProcedure = "FetchUSSDCompaign",
        UpdateProcedure = "UpdateUSSDCompaign",
        InsertProcedure = "InsertUSSDCompaign",
        DeleteProcedure = "DeleteUSSDCompaign"
        )
     ]

    public class USSDCampaign
    {
        [DataField]
        public Int32 Id { get; set; }

        [DataField]
        public string Name { get; set; }
        [DataField]
        public string Description { get; set; }
        [DataField]
        public string ProcessorType { get; set; }
        [DataField]
        public DateTime? CampaignStartDate { get; set; }
        [DataField]
        public DateTime? CampaignEndDate { get; set; }
        [DataField]
        public DateTime CreatedDate { get; set; }
        [DataField]
        public Int32 CreatedByUserId { get; set; }
        [DataField]
        public Int32 USSDNumberId { get; set; }
        [DataField]
        public string BackButton { get; set; }
        [DataField]
        public Int32 CommunicationTypeId { get; set; }
        [DataField]
        public bool WaitForHTTPResponse { get; set; }
        [DataField]
        public bool IsBillable { get; set; }
        [DataField]
        public decimal? BillAmount { get; set; }
        [DataField]
        public bool SendHTTPResponseBySMS { get; set; }
        [DataField]
        public string Number { get; set; }

        #region Filters
        [DataField]
        public string FilterUSSDNumber { get; set; }
[DataField]
        public int? FilterUSSDNumberId { get; set; }

        #endregion
    }
}
