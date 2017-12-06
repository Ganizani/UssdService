using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using exactmobile.ussdcommon;
using System.Web;
namespace exactmobile.ussdservice.common.menu
{
    public class MenuManager:IResultsOverridable
    {
        #region Events
        public delegate void ShowMenuEventHandler(object sender,ShowMenuEventArgs args);
        public event ShowMenuEventHandler ShowMenuEvent;

        public delegate void CommunicateMenuEventHandler(object sender, CommunicateMenuEventArgs args);
        public event CommunicateMenuEventHandler CommunicateMenuEvent;

        public delegate void ThirdPartyCommunicationEventHandler(object sender,MenuItem currentMenuItem);
        public event ThirdPartyCommunicationEventHandler ThirdPartyCommunicationEvent;

        public delegate void OBSBillingEventHandler(object sender, OBSBillingEventArgs args);
        public event OBSBillingEventHandler MenuOBSBillingEvent;
        #endregion

        #region Private Vars
        private int campaignID;
        private int ussdNumber;
        private List<UssdMenu> menuList = new List<UssdMenu>();
        //private static List<UssdMenuCache> menuListCache = new List<UssdMenuCache>();
        private Dictionary<int, UssdMenu> menus = new Dictionary<int, UssdMenu>();
        private Dictionary<int, UssdMenuItem> menuItems;
        private readonly string backLink; 

        private List<MenuItem> list = new List<MenuItem>();

        #endregion

        #region Properties
        public List<UssdMenu> Menu
        {
            get { return this.menuList; }
        }
        #endregion

        #region Constructor
        public MenuManager(int campaignID, int ussdNumber, int menuId)
        {
            this.campaignID = campaignID;
            this.ussdNumber = ussdNumber;
            backLink = "0. Back";
            LoadMenuList();
            //LoadMenu(campaignID,ussdNumber, menuId);
            //LoadCampaignMenu();
        }
        public MenuManager(int campaignID, int ussdNumber, int menuId,String campaignBackButton)
        {
            this.campaignID = campaignID;
            this.ussdNumber = ussdNumber;
            backLink = campaignBackButton + ". Back";
            LoadMenuList();
            //LoadMenu(campaignID,ussdNumber, menuId);
            //LoadCampaignMenu();
        } 
        #endregion

        #region Public Methods
        public String BuildMenu(int? menuID)
        {
            UssdMenu mainMenu = menus.Where(m => m.Key == menuID).FirstOrDefault().Value;
            
            if (mainMenu == null) return string.Empty;

            StringBuilder result = new StringBuilder();
            foreach (UssdMenuItem menuItem in mainMenu.MenuItems)
            {
                result.AppendLine(menuItem.Name);
            }
            return result.ToString();
        }

        public static int GetRootMenuId(int campaignId)
        {
            int rootMenuId = 0;

            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            DbCommand command = db.GetSqlStringCommand("Select Id from USSDMenus where parentMenuId = 0 and ussdCampaignId = " + campaignId);
                      
            rootMenuId = (int)db.ExecuteScalar(command);

            command.Dispose();
           
            return rootMenuId;
        }

        public String BuildMenu(int menuID, String returnValue, out UssdMenu lastMenu)
        {
            lastMenu = null;
            UssdMenu mainMenu = menus.Where(m => m.Key == menuID).FirstOrDefault().Value;

            if (mainMenu == null) return string.Empty;

            UssdMenuItem menuItem = mainMenu.MenuItems.Where(m => m.ReturnValue.Trim() == returnValue.Trim()).FirstOrDefault();
            if (menuItem != null && menuItem.ReturnMenu != null)
            {
                lastMenu = menuItem.ReturnMenu;
                return BuildMenu(menuItem.ReturnMenu.MenuID);
            }
            return String.Empty;
        }

        public UssdMenu FindMenuByName(String menuName)
        {            
            return menus.Where(m => m.Value.Name.Equals(menuName)).FirstOrDefault().Value;
        }

        public UssdMenuItem FindMenuByParentMenuItemId(int parentMenuId)
        {
            UssdMenuItem i = menuItems.Where(m => m.Value.ParentMenuItemId.Value.Equals(parentMenuId)).FirstOrDefault().Value;
            return i;
        }

        public UssdMenuItem FindMenuItemByReturnValue(UssdMenu parentMenu, string returnValue)
        {
            if (parentMenu == null) return null;

            return parentMenu.MenuItems.Where(mi => mi.ReturnValue.Equals(returnValue)).FirstOrDefault();
        }
        #endregion

        public string LoadMenu(int campaignId,int ussdNumber,int menuId)
        {
            string data = null;

            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            DbCommand command = db.GetStoredProcCommand("sp_GetMenu", new object[] {campaignId,ussdNumber,menuId });

            IDataReader reader = db.ExecuteReader(command);

            while (reader.Read())
            {
                if (data != null)
                    data += "\n";
                data += reader.GetString(2);
            }

            reader.Close();
            command.Dispose();

            return data;
        }

        public MenuItem GetParentMenu(int menuId)
        {
            return list.Where(m => m.MenuId == menuId).FirstOrDefault();
        }
        
        public MenuItem FindMenuItemByReturnValueNEW(MenuItem lastAccessdMenu, string returnValue)
        {
            if (lastAccessdMenu == null) return null;

            try
            {
                return list.Where(m => (m.ReturnValue == returnValue || m.RangeText.IndexOf(returnValue) != -1 ) && m.ParentMenuId == lastAccessdMenu.MenuId).FirstOrDefault();
            }
            catch
            {
                return list.Where(m => m.ReturnValue == returnValue && m.ParentMenuId == lastAccessdMenu.MenuId).FirstOrDefault();           
            
            }

          //  old return
            //return list.Where(m => m.ReturnValue == returnValue  && m.ParentMenuId == lastAccessdMenu.MenuId).FirstOrDefault();           
        }

        public string ShowMenu(int lastMenuId, int menuId, ref MenuItem currentMenu)
        {
            return ShowMenu(lastMenuId, menuId, ref currentMenu, true);            
        }
        public string ShowMenu(int lastMenuId, int menuId, ref MenuItem currentMenu,bool raiseMenuEvent)
        {
            return ShowMenu(lastMenuId, menuId, ref currentMenu, raiseMenuEvent, true);
        }
        public string ShowMenu(int lastMenuId, int menuId, ref MenuItem currentMenu, bool raiseMenuEvent,bool canRaiseCommunicationEvent)
        {
            return ShowMenu(lastMenuId, menuId, ref currentMenu, raiseMenuEvent, true,true);
        }

        public string ShowMenu(int lastMenuId, int menuId, ref MenuItem currentMenu, bool raiseMenuEvent, bool canRaiseCommunicationEvent, bool canRaiseBillEvent)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentMenu.DisplayData))
                    return currentMenu.ShowBack ? currentMenu.DisplayData + "\n" + backLink : currentMenu.DisplayData;
                else
                    currentMenu = new MenuItem();

                currentMenu = list.Where(m => m.MenuId == menuId).FirstOrDefault();

                List<MenuItem> menu = list.Where(m => m.ParentMenuId == menuId).ToList();

                StringBuilder result = new StringBuilder();
                foreach (MenuItem menuItem in menu)
                {
                    result.AppendLine(menuItem.Name);
                }
                //added implementation below for all the compulsory footer menu items
                //as is the '#:Home' menu for MonsterMix
                if (currentMenu != null && currentMenu.ParentMenuId != 0)
                {
                    List<MenuItem> footerMenu = list.Where(m => m.ParentMenuId == -1).ToList();
                    foreach (MenuItem footerMenuItem in footerMenu)
                        result.AppendLine(footerMenuItem.Name);
                }
                if (currentMenu != null && currentMenu.ShowBack)
                    result.Append(backLink);

                if (ShowMenuEvent != null && raiseMenuEvent)
                    ShowMenuEvent(this, new ShowMenuEventArgs(list.ToArray(), currentMenu, menu.ToArray(), menuId, result.ToString().Replace("\\n", "\n").Replace("\\r", "\r")));

                if ( canRaiseCommunicationEvent)
                {
                    if (CommunicateMenuEvent != null)
                    {
                        var args = new CommunicateMenuEventArgs(currentMenu);
                        if (args.CanCommunicate)
                            CommunicateMenuEvent(this, new CommunicateMenuEventArgs(currentMenu));
                    }
                    if (currentMenu.IsSMS != null && currentMenu.IsSMS.HasValue && ThirdPartyCommunicationEvent!= null)
                    {
                        if ((!string.IsNullOrEmpty(currentMenu.ThirdPartyCommunicationText)) 
                            && (!string.IsNullOrEmpty(currentMenu.ThirdPartyCommunicationRecipients)))
                        {
                         
                            if (!string.IsNullOrEmpty(currentMenu.NotificationKey))                                
                            ThirdPartyCommunicationEvent(this, currentMenu);                            
                        }
                    }
                }

                if (canRaiseBillEvent && MenuOBSBillingEvent != null)
                {
                    var args = new OBSBillingEventArgs(currentMenu);
                    if (args.IsValid)
                        MenuOBSBillingEvent(this, args);
                }

                if (menu.Count == 1)
                    currentMenu = menu[0];

                if (ResultsOverriden)
                    return OverrideValue.Replace("\\n", "\n").Replace("\\r", "\r");
                else
                    return result.ToString().Replace("\\n", "\n").Replace("\\r", "\r");
            }
            finally
            {
                ResultsOverriden = false;
                OverrideValue = null;
            }
        }


        public string ShowMenu(int lastMenuId, int menuId, ref MenuItem currentMenu, bool raiseMenuEvent, bool canRaiseCommunicationEvent, bool canRaiseBillEvent, string selectedValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentMenu.DisplayData))
                    return currentMenu.ShowBack ? currentMenu.DisplayData + "\n" + backLink : currentMenu.DisplayData;
                else
                    currentMenu = new MenuItem();

                currentMenu = list.Where(m => m.MenuId == menuId).FirstOrDefault();

                List<MenuItem> menu = list.Where(m => m.ParentMenuId == menuId).ToList();

                StringBuilder result = new StringBuilder();
                foreach (MenuItem menuItem in menu)
                {
                    result.AppendLine(menuItem.Name);
                }
                //added implementation below for all the compulsory footer menu items
                //as is the '#:Home' menu for MonsterMix
                if (currentMenu != null && currentMenu.ParentMenuId != 0)
                {
                    List<MenuItem> footerMenu = list.Where(m => m.ParentMenuId == -1).ToList();
                    foreach (MenuItem footerMenuItem in footerMenu)
                        result.AppendLine(footerMenuItem.Name);
                }
                if (currentMenu != null && currentMenu.ShowBack)
                    result.Append(backLink);

                if (ShowMenuEvent != null && raiseMenuEvent)
                    ShowMenuEvent(this, new ShowMenuEventArgs(list.ToArray(), currentMenu, menu.ToArray(), menuId, result.ToString().Replace("\\n", "\n").Replace("\\r", "\r")));

                if (canRaiseCommunicationEvent)
                {
                    if (CommunicateMenuEvent != null)
                    {
                        var args = new CommunicateMenuEventArgs(currentMenu);
                        if (args.CanCommunicate)
                            CommunicateMenuEvent(this, new CommunicateMenuEventArgs(currentMenu));
                    }
                    if (currentMenu.IsSMS != null && currentMenu.IsSMS.HasValue && ThirdPartyCommunicationEvent != null)
                    {
                        if ((!string.IsNullOrEmpty(currentMenu.ThirdPartyCommunicationText))
                            && (!string.IsNullOrEmpty(currentMenu.ThirdPartyCommunicationRecipients)))
                        {

                            if (!string.IsNullOrEmpty(currentMenu.NotificationKey) && !string.IsNullOrEmpty(selectedValue))
                            {
                                if (currentMenu.NotificationKey == selectedValue)
                                ThirdPartyCommunicationEvent(this, currentMenu);
                            }
                        }
                    }
                }

                if (canRaiseBillEvent && MenuOBSBillingEvent != null)
                {
                    var args = new OBSBillingEventArgs(currentMenu);
                    if (args.IsValid)
                    {
                        if (!string.IsNullOrEmpty(currentMenu.NotificationKey) && !string.IsNullOrEmpty(selectedValue))
                        {
                            if (currentMenu.NotificationKey == selectedValue)
                                MenuOBSBillingEvent(this, args);
                        }
                    }
                }

                if (menu.Count == 1)
                    currentMenu = menu[0];

                if (ResultsOverriden)
                    return OverrideValue.Replace("\\n", "\n").Replace("\\r", "\r");
                else
                    return result.ToString().Replace("\\n", "\n").Replace("\\r", "\r");
            }
            finally
            {
                ResultsOverriden = false;
                OverrideValue = null;
            }
        }

        public void LoadMenuList()
        {           
            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            using (DbCommand command = db.GetStoredProcCommand("usp_GetMenuItems", new object[] { this.campaignID, this.ussdNumber }))
            {
                using (IDataReader reader = db.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        var item = new MenuItem()
                        {
                            MenuId = Convert.ToInt32(reader["MenuId"]),
                            ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]),
                            Name = Convert.ToString(reader["Name"]),
                            ReturnMenuId = reader.IsDBNull(reader.GetOrdinal("ReturnMenuId")) ? 0 : Convert.ToInt32(reader["ReturnMenuId"]),
                            ReturnValue = reader.IsDBNull(reader.GetOrdinal("ReturnValue")) ? null : Convert.ToString(reader["ReturnValue"]),
                            DisplayData = reader.IsDBNull(reader.GetOrdinal("DisplayData")) ? null : Convert.ToString(reader["DisplayData"]),
                            ShowBack = reader.IsDBNull(reader.GetOrdinal("ShowBack")) ? false : Convert.ToBoolean(reader["ShowBack"]),
                            OrderNumber = Convert.ToInt32(reader["OrderNumber"]),
                            CommunicationTypeID = Convert.ToInt32(reader["CommunicationTypeID"]),
                            CommunicationText = Convert.ToString(reader["CommunicationText"]).Trim(),
                            //WaitForHTTPResponse = Convert.ToBoolean(reader["WaitForResponse"]),
                            //ThirdPartyCommunicationRecipients = Convert.ToString(reader["ThirdPartyCommunicationRecipients"]),
                            //ThirdPartyCommunicationText = Convert.ToString(reader["ThirdPartyCommunicationText"]),
                            IsBillable = Convert.ToBoolean(reader["IsBillable"]),
                            BillAmount = reader.IsDBNull(reader.GetOrdinal("BillAmount")) ? 0.00M : Convert.ToDecimal(reader["BillAmount"]),
                            SendHTTPResponseBySMS = Convert.ToBoolean(reader["SendHTTPResponseBySMS"]),
                            //RangeValueFrom =  reader.IsDBNull(18) ? 0 : reader.GetInt32(18),
                            //RangeValueTo =  reader.IsDBNull(19) ? 0 : reader.GetInt32(19)  ,
                            //AttachmentBase64 = Convert.ToString(reader["AttachmentBase64"]).Trim(),
                            //Subject = Convert.ToString(reader["Subject"]).Trim(),
                            //AttachmentTypeIDRef = reader.IsDBNull(22) ? 0 : reader.GetInt32(22),
                            //RangeText = Convert.ToString(reader["RangeText"]).Trim(),
                            //NotificationKey =Convert.ToString(reader["NotificationKey"]).Trim()

                        };

                        //if (reader["IsSMS"] != DBNull.Value)
                        //    item.IsSMS = Convert.ToBoolean(reader["IsSMS"]);

                        list.Add(item);
                    }
                }
            }           
        }

        #region Protected Methods
        protected void LoadCampaignMenu()
        {
            //UssdMenuCache menuCache = menuListCache.Where(mc => mc.CampaignID == this.campaignID && mc.UssdNumber == this.ussdNumber).FirstOrDefault();
            //if (menuCache != null)
            //    menuList = menuCache.MenuList;
            //else
            //{

                Database ussdDatabase = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
                DataSet menuDataSet = ussdDatabase.ExecuteDataSet("SP_GetUSSDMenuForCampaign", new Object[] { this.campaignID, this.ussdNumber });
                try
                {
                    menuList.Clear();
                    menuItems = new Dictionary<int, UssdMenuItem>();
                    Dictionary<UssdMenuItem, int> returnMenus = new Dictionary<UssdMenuItem, int>();
                    foreach (DataRow row in menuDataSet.Tables[0].Rows)
                    {
                        int menuID = (int)row["MenuID"];
                        // Finds or creates the menu object
                        UssdMenu menu = menuList.Where(m => m.MenuID == menuID).FirstOrDefault();
                        if (menu == null)
                        {
                            int? parentMenuIDRef = null;
                            if (!row.IsNull("ParentMenuIDRef"))
                                parentMenuIDRef = (int)row["ParentMenuIDRef"];

                            // Create the parent menu if it needs to exist
                            UssdMenu parentMenu = null;
                            if (parentMenuIDRef != null)
                                parentMenu = menuList.Where(m => m.MenuID == parentMenuIDRef.Value).FirstOrDefault();

                            menu = new UssdMenu(menuID, (string)row["MenuName"], (int)row["MenuTypeID"], parentMenu);
                            menuList.Add(menu);

                            if (!menus.ContainsKey(menuID))
                                menus.Add(menuID, menu);
                            if (parentMenu != null)
                                parentMenu.SubMenus.Add(menu);
                        }

                        int menuItemID = (int)row["MenuItemID"];
                        UssdMenuItem menuItem = null;
                        if (menuItems.ContainsKey(menuItemID))
                            menuItem = menuItems[menuItemID];
                        else
                        {
                            int parentMenuItemID = 0;
                            if(!row.IsNull("ParentMenuItemIDRef"))
                                parentMenuItemID = int.Parse(row["ParentMenuItemIDRef"].ToString());

                            String menuData = null;
                            if (!row.IsNull("MenuData"))
                                menuData = row["MenuData"].ToString();
                            menuItem = new UssdMenuItem(menu, menuItemID, (String)row["MenuItemName"], int.Parse(row["DisplayOrder"].ToString()), row["ReturnValue"].ToString(), null, menuData, parentMenuItemID);
                            menuItems.Add(menuItemID, menuItem);
                        }

                        menu.MenuItems.Add(menuItem);

                        int? returnMenuID = null;
                        if (!row.IsNull("ReturnMenuIDRef"))
                            returnMenuID = (int)row["ReturnMenuIDRef"];
                        UssdMenu returnMenu = null;
                        if (returnMenuID != null)
                            returnMenu = menuList.Where(m => m.MenuID == returnMenuID).FirstOrDefault();

                        if (returnMenuID != null)
                            if (returnMenu == null)
                                returnMenus.Add(menuItem, returnMenuID.Value);
                            else
                                menuItem.ReturnMenu = returnMenu;
                    }
                    // Add the return menus as they might only be loaded after the main menu is loaded
                    foreach (KeyValuePair<UssdMenuItem, int> returnMenuPair in returnMenus)
                    {
                        UssdMenu returnMenu = menus.Where(m => m.Key == returnMenuPair.Value).FirstOrDefault().Value;
                        if (returnMenu != null)
                            returnMenuPair.Key.ReturnMenu = returnMenu;
                    }
                    //menuListCache.Add(new UssdMenuCache { CampaignID = this.campaignID, MenuList = this.menuList, UssdNumber = this.ussdNumber });
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    menuDataSet.Dispose();
                }
            //}
        }
        #endregion

        #region IResultsOverridable Members

        public bool ResultsOverriden
        {
            get;
            set;
        }

        public string OverrideValue
        {
            get;
            set;
        }
        
        #endregion
    }
}
