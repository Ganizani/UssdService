using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MBDInternal.Data;

namespace USSD.Entities
{
    [DataClass(
        ListProcedure = "sp_Employers_List",
        FetchProcedure = "sp_Employers_Fetch",
        InsertProcedure = "sp_Employers_Insert",
        UpdateProcedure = "sp_Employers_Update"
        )]
    public class Employer
    {
        [DataField]
        public int
            wEmployerID { get; set; }
        [DataField]
        public string
            szEmployerName { get; set; }
        [DataField]
        public string
            szSuburbTown { get; set; }
        [DataField]
        public string
            szPayPAddr1 { get; set; }
        [DataField]
        public string
            szPayPAddr2 { get; set; }
        [DataField]
        public string
            szPayPAddr3 { get; set; }
        [DataField]
        public string
            szPayPAddr4 { get; set; }
        [DataField]
        public string
            szPayPCode { get; set; }
        [DataField]
        public string
            szPostalAddress1 { get; set; }
        [DataField]
        public string
            szPostalAddress2 { get; set; }
        [DataField]
        public string
            szPostalAddress3 { get; set; }
        [DataField]
        public string
            szPostalAddress4 { get; set; }
        [DataField]
        public string
            szPostalCode { get; set; }
        [DataField]
        public string
            szContactPerson { get; set; }
        [DataField]
        public string
            szTelNo { get; set; }
        [DataField]
        public string
            szFaxNo { get; set; }
        [DataField]
        public string
            szEmail { get; set; }
        [DataField]
        public string
            szSalDate { get; set; }
        [DataField]

        public string szIsState { get; set; }
        [DataField]
        public string szCommission { get; set; }
        [DataField]
        public int wID { get; set; }
        [DataField]
        public string szReferenceNumber { get; set; }
        [DataField]
        public int wTracerID { get; set; }
        [DataField]
        public string szEmployer { get; set; }
        [DataField]
        public string szEmployerAddress1 { get; set; }
        [DataField]
        public string szEmployerAddress2 { get; set; }
        [DataField]
        public string szEmployerAddress3 { get; set; }
        [DataField]
        public string szEmployerAddress4 { get; set; }
        [DataField]
        public string szEmployerPostalCode { get; set; }
        [DataField]
        public string szEmployerPayPoint { get; set; }
        [DataField]
        public string szEmployerPayPoint2 { get; set; }
        [DataField]
        public string szEmployerPayPoint3 { get; set; }
        [DataField]
        public string szEmployerPayPoint4 { get; set; }
        [DataField]
        public string szEmployerPayPointCode { get; set; }
        [DataField]
        public string szEmployeeNumber { get; set; }
        [DataField]
        public string szEmployerTel { get; set; }
        [DataField]
        public string szEmployerHRContact { get; set; }
        [DataField]
        public string szEmployerHRTel { get; set; }
        [DataField]
        public DateTime? dtCapture { get; set; }
        [DataField]
        public bool? bValidated { get; set; }
        [DataField]
        public DateTime? dtValidation { get; set; }
        [DataField]
        public string szValidatedBy { get; set; }
        [DataField]
        public bool? bUploaded { get; set; }

        #region  Filters
        [DataField]
        public string FilterEmployerName { get; set; }
        [DataField]
        public int?  FilterTracerID { get; set; }
        [DataField]
        public string FilterReferenceNumber { get; set; }
        
        #endregion
    }
}
