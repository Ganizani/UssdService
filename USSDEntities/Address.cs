using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MBDInternal.Data;

namespace USSD.Entities
{
    [DataClass(ListProcedure = "ListAddresses",
        FetchProcedure = "FetchAddress",
        UpdateProcedure = "UpdateAddress",
        InsertProcedure = "InsertAddress",
        DeleteProcedure = "DeleteAddress"
    )]
    public partial class Address
    {
        #region Properties
        [DataField]
        [DisplayName("Id")]
        [Column(IsPrimaryKey = true)]
        [Required(ErrorMessage = "You must provide a Id")]
        public int Id { get; set; }

        [DataField]
        [DisplayName("IsResidentialAddress")]
        [Required(ErrorMessage = "You must provide a IsResidentialAddress")]
        public bool IsResidentialAddress { get; set; }

        [DataField]
        [DisplayName("IsPostBox")]
        [Required(ErrorMessage = "You must provide a IsPostBox")]
        public bool IsPostBox { get; set; }

        [DataField]
        [DisplayName("Company Name / Complex Name")]
        public string ComplexOrPostbox { get; set; }

        [DataField]
        [DisplayName("StreetAndNumber")]
        public string StreetAndNumber { get; set; }

        [DataField]
        [DisplayName("Suburb")]
        //[Required(ErrorMessage = "You must specify the suburb")]
        public string Suburb { get; set; }

        [DataField]
        [DisplayName("City")]
        //[Required(ErrorMessage = "You must specify the city")]
        public string City { get; set; }

        [DataField]
        //[Required(ErrorMessage = "You must specify the region")]
        public string Region { get; set; }

        [DataField]
        [DisplayName("Province")]
        //[Required(ErrorMessage = "You must specify the province")]
        public string Province { get; set; }

        [DataField]
        //[Required(ErrorMessage = "You must specify the country")]
        public string Country { get; set; }

        [DataField]
        [DisplayName("Code")]
        //[Required(ErrorMessage = "You must specify the code")]
        public string Code { get; set; }

        [DataField]
        [DisplayName("XLong")]
        public Decimal? XLong { get; set; }

        [DataField]
        [DisplayName("YLat")]
        public Decimal? YLat { get; set; }

        [DataField]
        [DisplayName("IsActive")]
        [Required(ErrorMessage = "You must provide a IsActive")]
        public bool IsActive { get; set; }

        [DataField]
        [DisplayName("CreateUserId")]
        [Required(ErrorMessage = "You must provide a CreateUserId")]
        public int CreateUserId { get; set; }

        [DataField]
        [DisplayName("CreateDate")]
        [Required(ErrorMessage = "You must provide a CreateDate")]
        public DateTime CreateDate { get; set; }

        [DataField]
        [DisplayName("ModifyUserId")]
        [Required(ErrorMessage = "You must provide a ModifyUserId")]
        public int ModifyUserId { get; set; }

        [DataField]
        [DisplayName("ModifyDate")]
        [Required(ErrorMessage = "You must provide a ModifyDate")]
        public DateTime ModifyDate { get; set; }

        #endregion

        #region Constructors
        public Address()
        {
            this.IsActive = true;
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
        }
        #endregion
    }
}