using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.common.menu
{
    public class ShowMenuEventArgs:System.EventArgs
    {
        #region Private variables
        readonly MenuItem[] campaignMenus;
        readonly MenuItem currentMenu;
        readonly MenuItem[] currentPage;       
        readonly int requestedMenuID;        
        readonly String displayedText;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="campaignMenuItems">All the current USSD campaign (cached) menus.</param>
        /// <param name="currentMenuItem">Current (parent) menu whose child menu(s) are being displayed.</param>
        /// <param name="currentPageMenus">Current child menu(s) being displayed</param>
        /// <param name="requestedMenuIDValue">Menu ID of the current (parent) menu as requested from the MenuManager.ShowMenu method call.</param>
        /// <param name="displayedMenuText">Text returned from the MenuManager.ShowMenu method call.</param>
        public ShowMenuEventArgs(MenuItem[] campaignMenuItems, MenuItem currentMenuItem, MenuItem[] currentPageMenus, int requestedMenuIDValue,string displayedMenuText)
            : base()
        {
            currentMenu = currentMenuItem;
            campaignMenus = campaignMenuItems;
            requestedMenuID = requestedMenuIDValue;
            currentPage = currentPageMenus;
            displayedText = displayedMenuText;
        }
        
        /// <summary>
        /// All of the current campaign's menu items
        /// </summary>
        public MenuItem[] CampaignMenus
        {
            get
            {
                return campaignMenus;
            }
        }   
  
        /// <summary>
        /// Current menu being displayed. This will be the parent menu of the returned menu (displayed) text
        /// </summary>
        public MenuItem CurrentMenu
        {
            get
            {
                return currentMenu;
            }
        }

        /// <summary>
        /// Current menu items being displayed.
        /// </summary>
        public MenuItem[] CurrentPage
        {
            get
            {
                return currentPage;
            }
        }        
        /// <summary>
        /// Menu ID of the current menu page parent MenuItem as passed in code.
        /// </summary>
        public int RequestedMenuID
        {
            get
            {
                return requestedMenuID;
            }
        }

        /// <summary>
        /// Current displayed menu text
        /// </summary>
        public string DisplayedText
        {
            get
            {
                return displayedText;
            }
        }
    }
}
