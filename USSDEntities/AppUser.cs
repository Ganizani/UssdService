using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MBDInternal.Data;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace USSD.Entities
{
    public class AppIdentity : IIdentity
    {
        public AppIdentity(string name, string authenticationType, bool isAuthenticated, Guid userId)
        {
            Name = name;
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;
            UserId = userId;
        }

        #region IIdentity
        [DataField]
        public string Name { get; private set; }
        [DataField]
        public string AuthenticationType { get; private set; }
        [DataField]
        public bool IsAuthenticated { get; private set; }

        #endregion IIdentity
        [DataField]
        public Guid UserId { get; private set; }
    }

    [Serializable]
    [DataClass(
      ListProcedure = "sp_UserList_List"
    , InsertProcedure = ""
    , UpdateProcedure = ""
    , DeleteProcedure = ""
    , FetchProcedure = ""
)]

    public class AppUser : IPrincipal
    {
        #region Properties
        [DataField]
        public int Id { get; set; }
        [DataField]
        public string UserName { get; set; }
        [DataField]
        public int wUserLevel { get; set; }
        [DataField]
        public int wInteractiveLevel { get; set; }
        [DataField]
        public string szCheetahPassword { get; set; }
        [DataField]
        public string szDepartment { get; set; }
        [DataField]
        public string szManagerName { get; set; }
        [DataField]
        public string szManagerEMail { get; set; }
        [DataField]
        public string szUserEMail { get; set; }
        [DataField]
        public int wReportsTo { get; set; }
        [DataField]
        public string szCompanyName { get; set; }
        [DataField]
        public string szExtension { get; set; }
        [DataField]
        public string szFirstName { get; set; }
        [DataField]
        public string szSurname { get; set; }
        [DataField]
        public string szMobileNumber { get; set; }
        [DataField]
        public int wCompanyCode { get; set; }
        [DataField]
        public string szHierarchy { get; set; }
        [DataField]
        public int wActiveRequests { get; set; }
        [DataField]
        public int wActiveAssignments { get; set; }
        [DataField]
        public int wActiveTasks { get; set; }
        [DataField]
        public int wActiveSignOffs { get; set; }
        [DataField]
        public int wQAApproval { get; set; }
        [DataField]
        public int w911Tickets { get; set; }
        [DataField]
        public int wHelpDeskAdmin { get; set; }
        [DataField]
        public string szIDNumber { get; set; }
  
       
        #endregion

        #region Filters
        [DataField]
        public string FilterFirstName { get; set; }
        [DataField]
        public string FilterSurname { get; set; }
        [DataField]
        public string FilterNickName { get; set; }
        [DataField]
        public string FilterManagerName { get; set; }
        [DataField]
        public string FilterManagerEmail { get; set; }
        [DataField]
        public string FilterUserEmail { get; set; }
        [DataField]
        public string FilterEmployeeNumber { get; set; }
        [DataField]
        public string FilterEmailAddress { get; set; }
        [DataField]
        public string FilterPassword { get; set; }
        [DataField]
        public string FilterClient { get; set; }
        [DataField]
        public string FilterCallCenter { get; set; }
        [DataField]
        public string FilterIDNumber{ get; set; }
        [DataField]
        public string FilterUserName { get; set; }
        [DataField]
        
        public Nullable<DateTime> FilterDateOfBirth { get; set; }

        #endregion
        public AppUser(AppIdentity identity)
        {
            Identity = identity;
        }

        public AppIdentity Identity { get; private set; }

        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; ; }
        }

        bool IPrincipal.IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}