using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class OBSBillingEventArgs:System.EventArgs
    {
        readonly MenuItem billingMenuItem;
        private OBSBillingEventArgs()
            : base()
        {
        }

        public OBSBillingEventArgs(MenuItem billingMenuItem):base()
        {
            this.billingMenuItem = billingMenuItem;
        }

        public MenuItem BillingMenuItem
        {
            get { return billingMenuItem; }
        }

        public bool IsValid
        {
            get {
                if (BillingMenuItem == null)
                    return false;
                else
                    return (BillingMenuItem.BillAmount > 0 && BillingMenuItem.IsBillable);
            }
        }
    }
}
