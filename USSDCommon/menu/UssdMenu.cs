using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class UssdMenu
    {
        #region enum
        public enum MenuTypes : int
        {
            MainMenu = 1,	
            SubMenu = 2
        }
        #endregion

        #region Properties
        public readonly int MenuID;
        public readonly String Name;
        public readonly MenuTypes MenuType;
        public readonly UssdMenu ParentMenu;
        
        public List<UssdMenu> SubMenus = new List<UssdMenu>();
        public List<UssdMenuItem> MenuItems = new List<UssdMenuItem>(); 
        #endregion

        #region Constructor
        public UssdMenu(int menuID, String name, int menuTypeID, UssdMenu parentMenu)
        {
            this.MenuID = menuID;
            this.Name = name;
            this.ParentMenu = parentMenu;
            this.MenuType = (MenuTypes)Enum.Parse(typeof(MenuTypes), menuTypeID.ToString(), true);
        }

        public UssdMenu(int menuID, String name, MenuTypes menuType, UssdMenu parentMenu)
        {
            this.MenuID = menuID;
            this.Name = name;
            this.MenuType = menuType;
            this.ParentMenu = parentMenu;
        } 
        #endregion
    }
}
