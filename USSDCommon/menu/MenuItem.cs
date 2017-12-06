using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class MenuItem
    {
        public enum CommunicationType : int
        {
            SMS = 1,
            MMS = 2,
            SM2HTTP = 3
        };

        public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string Name { get; set; }
        public int ReturnMenuId { get; set; }
        public string ReturnValue { get; set; }
        public string DisplayData { get; set; }
        public bool ShowBack { get; set; }
        public int OrderNumber { get; set; }
        public int CommunicationTypeID { get; set; }
        public string CommunicationText { get; set; }
        public bool WaitForHTTPResponse { get; set; }
        public Nullable<bool> IsSMS { get; set; }
        public string ThirdPartyCommunicationRecipients { get; set; }
        public string ThirdPartyCommunicationText { get; set; }
        public bool IsBillable { get; set; }
        public decimal BillAmount { get; set; }
        public bool SendHTTPResponseBySMS { get; set; }
        public int RangeValueFrom { get; set; }
        public int RangeValueTo { get; set; }
        public string AttachmentBase64 { get; set; }
        public string Subject { get; set; }
        public int AttachmentTypeIDRef { get; set; }
        public string RangeText { get; set; }
        public string NotificationKey { get; set; }
    }
}
