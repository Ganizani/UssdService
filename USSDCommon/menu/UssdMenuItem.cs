using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class UssdMenuItem
    {
        #region Properties
        public readonly int MenuItemID;
        public readonly String Name;
        public UssdMenu ReturnMenu;
        public readonly int DisplayOrder;
        public readonly String ReturnValue;
        public readonly UssdMenu ParentMenu;
        public readonly String MenuData;
        public readonly int? ParentMenuItemId;
        
        #endregion

        #region Constructor
        public UssdMenuItem(UssdMenu parentMenu, int menuItemID, String name, int displayOrder, String returnValue, UssdMenu returnMenu, String menuData, int parentMenuItemID)
        {
            this.MenuItemID = menuItemID;
            this.Name = name;
            this.ReturnMenu = returnMenu;
            this.DisplayOrder = displayOrder;
            this.ReturnValue = returnValue;
            this.ParentMenu = parentMenu;
            this.ParentMenuItemId = parentMenuItemID;
            
            if (menuData != null)
                this.MenuData = menuData;
            else
                this.MenuData = String.Empty;
        }
        #endregion
    }
}
