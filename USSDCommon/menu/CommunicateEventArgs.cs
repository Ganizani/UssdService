using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class CommunicateMenuEventArgs:EventArgs
    {
        public CommunicateMenuEventArgs(MenuItem menuItem)
            : base()
        {
            Menu = menuItem;
        }
        public MenuItem Menu
        {
            get;
            set;
        }
        public bool CanCommunicate
        {
            get
            {
                if (Menu != null)
                {
                    if (Menu.CommunicationTypeID >= 1 && (!string.IsNullOrEmpty(Menu.CommunicationText)))
                        return true;                                       
                }
                return false;
            }
        }
    }
}
