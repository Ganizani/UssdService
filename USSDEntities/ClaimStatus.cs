using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBDInternal.Data;
namespace USSD.Entities
{
    [DataClass(
ListProcedure = "ListClaimStatuses"
, InsertProcedure = "InsertClaimStatus"
, FetchProcedure = "FetchClaimStatus"
, DeleteProcedure = "DeleteClaimStatus"
, UpdateProcedure = "UpdateClaimStatus"
)]
  
    public class ClaimStatus
    {
        #region Properties
        //[Column(IsPrimaryKey = true)]
        [DataField]
        public int Id { get; set; }

        [DataField]
        public long ClaimNumber { get; set; }

        [DataField]
        public int StatusId { get; set; }

        [DataField]
        public int CreatedByUserID { get; set; }

        [DataField]
        public string Notes { get; set; }

        [DataField]
        public DateTime CreatedDateTime { get; set; }

        [DataField]
        public bool IsNoteRequired { get; set; }

        [DataField]
        public bool? FirstNotification { get; set; }
        [DataField]
        public bool? SecondNotification { get; set; }
        [DataField]
        public bool? ThirdNotification { get; set; }
        [DataField]
        public bool? FourthNotification { get; set; }

        #endregion

        #region Filtering Fields

        [DataField]
        public int? FilterStatusId { get; set; }
        [DataField]
        public long? FilterClaimNumber { get; set; }

        #endregion

        #region Resolved Fields
        [DataField]
        public string StatusDescription { get; set; }
        #endregion


    }
}
