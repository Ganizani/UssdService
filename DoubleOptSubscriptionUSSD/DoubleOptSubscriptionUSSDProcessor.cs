using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using exactmobile.ussdcommon.utility;
using exactmobile.ussdservice.common.menu;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;

namespace exactmobile.ussdservice.processors.DoubleOptSubscriptionUSSD
{
    public class DoubleOptSubscriptionUSSDProcessor: BaseUSSDProcessor, IUSSDProcessor
    {
        class DynamicMenu
        {
            public int Number { get; set; }
            public string Text { get; set; }
            public SubscriptionServiceInfo ServiceInfo { get; set; }
            public string USSDTextKey { get; set; }
        }

        #region Fields
        
        int rootMenuID;
        List<DynamicMenu> dynamicMenuList = new List<DynamicMenu>();
        const string DynamicMenuKey = "DynamicMenu";
        const string TermsAndConditionsKey="terms";
        const string AdminKey = "admin";
        const string CurrentDynamicMenuKey = "currentmenu";
        const string SubscribeRequestSent = "sentrequest";
        const string ClubNamePlaceholder = "[clubname]";
        const string PricePlaceholder = "[price]";
        const string BillingFrequencyPlaceholder = "[frequency]";
        const int FirstOptInMenuID = 44;
        const int DoubleOptInMenuID = 49;
        
        #endregion

        #region Constructor
        public DoubleOptSubscriptionUSSDProcessor(USSDSession session, IUSSDHandler handler)
            : base(session, handler)
        {           
            
        }
        #endregion

        #region BaseUSSDProcessor Overriden Members
        public override string OnConnect()
        {            
            return GetHomeScreen();            
        }

        public override string ManualOverRide()
        {
            if (Helper.Message.Equals("#"))
            {
                Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] = null;
                return GetHomeScreen();
            }
            if (Session.LastMenuAccessed != null)
            {
                if (Session.LastMenuAccessed.MenuId == Session.MainMenuId)
                {
                    int userSelection;
                    if (int.TryParse(Helper.Message.Trim(), out userSelection))
                    {
                        var selectedItem = dynamicMenuList.FirstOrDefault(menu => menu.Number.Equals(userSelection));
                        if (selectedItem != null)
                        {
                            if (selectedItem.ServiceInfo != null)//selected one of the services from the menu
                            {
                                Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] = selectedItem;
                                
                                MenuItem currentMenu = new MenuItem();
                                string menuData = Menu.ShowMenu(0, DoubleOptSubscriptionUSSDProcessor.FirstOptInMenuID, ref currentMenu);
                                Session.LastMenuAccessed = currentMenu;

                                StringBuilder retVal = new StringBuilder(menuData);

                                //placeholders: [clubname],[price],[frequency]
                                retVal.Replace(DoubleOptSubscriptionUSSDProcessor.ClubNamePlaceholder, selectedItem.Text);
                                retVal.Replace(DoubleOptSubscriptionUSSDProcessor.PricePlaceholder, String.Format(new System.Globalization.CultureInfo("en-ZA"), "{0:C}", new object[] { selectedItem.ServiceInfo.Amount }));
                                retVal.Replace(DoubleOptSubscriptionUSSDProcessor.BillingFrequencyPlaceholder, selectedItem.ServiceInfo.BillingFrequency);
                                return retVal.ToString();
                            }
                            else
                            {
                                Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] = null;

                                if (String.Equals(selectedItem.USSDTextKey, DoubleOptSubscriptionUSSDProcessor.TermsAndConditionsKey))
                                {
                                    //viewing terms and conditions info

                                }
                                else if (String.Equals(selectedItem.USSDTextKey, DoubleOptSubscriptionUSSDProcessor.AdminKey))
                                {
                                    //viewing admin info
                                }
                            }
                        }
                    }
                }
                else if (Session.LastMenuAccessed.MenuId == DoubleOptSubscriptionUSSDProcessor.FirstOptInMenuID)
                {
                    int userSelection;
                    if (int.TryParse(Helper.Message.Trim(), out userSelection))
                    {
                        if (userSelection != 1)
                            return GetHomeScreen();
                        else
                        {
                            if (Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] is DynamicMenu)
                            {
                                var selectedItem = (Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] as DynamicMenu);
                                var doubleOptIsActive = ReturnDoubleOptActiveStatus(selectedItem.ServiceInfo.Id, Session.MobileNetworkID);
                                if (doubleOptIsActive)//get the double opt screen
                                {
                                    StringBuilder retVal = new StringBuilder();
                                    MenuItem currentMenu = new MenuItem();
                                    string menuData = Menu.ShowMenu(0, DoubleOptSubscriptionUSSDProcessor.DoubleOptInMenuID, ref currentMenu);
                                    Session.LastMenuAccessed = currentMenu;

                                    retVal = new StringBuilder(menuData);



                                    //placeholders: [clubname],[price],[frequency]
                                    retVal.Replace(DoubleOptSubscriptionUSSDProcessor.ClubNamePlaceholder, selectedItem.Text);
                                    retVal.Replace(DoubleOptSubscriptionUSSDProcessor.PricePlaceholder, String.Format(new System.Globalization.CultureInfo("en-ZA"), "{0:C}", new object[] { selectedItem.ServiceInfo.Amount }));
                                    retVal.Replace(DoubleOptSubscriptionUSSDProcessor.BillingFrequencyPlaceholder, selectedItem.ServiceInfo.BillingFrequency);
                                    return retVal.ToString();
                                }
                                else//send subscribe request here
                                {
                                    bool submitted;
                                    new Subscription().Subscribe(Session.MSISDN, selectedItem.ServiceInfo.Id, false, out submitted,null);
                                    if (submitted)
                                    {
                                        string text;
                                        string delimiter;
                                        if (CommonUtility.ReturnUSSDCampaignText(Session.Campaign.CampaignID, DoubleOptSubscriptionUSSDProcessor.SubscribeRequestSent, out text, out delimiter))
                                        {
                                            return text.Replace(DoubleOptSubscriptionUSSDProcessor.ClubNamePlaceholder, selectedItem.ServiceInfo.DisplayName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return null;
                }
                else if (Session.LastMenuAccessed.MenuId == DoubleOptSubscriptionUSSDProcessor.DoubleOptInMenuID)
                {
                    int userSelection;
                    if (int.TryParse(Helper.Message.Trim(), out userSelection))
                    {
                        if (userSelection == 1)
                        {
                            if (Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] is DynamicMenu)
                            {
                                var selectedItem = (Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] as DynamicMenu);
                                //process subscription here.
                                bool submitted;
                                new Subscription().Subscribe(Session.MSISDN, selectedItem.ServiceInfo.Id, false, out submitted,null);

                                if (submitted)
                                {
                                    string text;
                                    string delimiter;
                                    if (CommonUtility.ReturnUSSDCampaignText(Session.Campaign.CampaignID, DoubleOptSubscriptionUSSDProcessor.SubscribeRequestSent, out text, out delimiter))
                                    {
                                        return text.Replace(DoubleOptSubscriptionUSSDProcessor.ClubNamePlaceholder, selectedItem.Text);
                                    }
                                }
                            }
                        }
                        else
                            return GetHomeScreen();
                    }
                }
            }
            return null;
        }
        #endregion

        #region Private Members
        public string GetHomeScreen()
        {
            string header;
            string delimiter;
            
            //clear the cached current dynamic menu selection value
            Session[DoubleOptSubscriptionUSSDProcessor.CurrentDynamicMenuKey] = null;

            CommonUtility.ReturnUSSDCampaignText(Session.Campaign.CampaignID, "rootheader", out header, out delimiter);

            MenuItem currentMenu = new MenuItem();
            string menuData = Menu.ShowMenu(0, Session.MainMenuId, ref currentMenu);
            CurrentMenu = currentMenu;
            Session.LastMenuAccessed = CurrentMenu;

            var services = new SubscriptionServiceInfo().ReturnDoubleOptSubscriptionServices();            

            int number = 0;
            StringBuilder menuBuilder = new StringBuilder(string.Concat(Convert.ToString(menuData),Convert.ToString(header)));
            foreach (var serviceInfo in services)
            {
                number++;
                var dynamicMenu = new DynamicMenu() { Number = number, Text = (String.IsNullOrEmpty(serviceInfo.DisplayName) ? serviceInfo.Name : serviceInfo.DisplayName), ServiceInfo = serviceInfo };

                menuBuilder.Append(string.Format("{0}: {1}\n", number, dynamicMenu.Text));
                
                dynamicMenuList.Add(dynamicMenu);
            }           

            string termsMenuTitle;
            number++;

            CommonUtility.ReturnUSSDCampaignText(Session.Campaign.CampaignID, "termstitle", out termsMenuTitle, out delimiter);
            menuBuilder.Append(string.Format("{0}{1}", number, termsMenuTitle));
            dynamicMenuList.Add(new DynamicMenu() { Number = number, Text = termsMenuTitle, ServiceInfo = null });

            string adminMenuTitle;
            number++;

            CommonUtility.ReturnUSSDCampaignText(Session.Campaign.CampaignID, "admintitle", out adminMenuTitle, out delimiter);
            menuBuilder.Append(string.Format("{0}{1}", number, adminMenuTitle));
            dynamicMenuList.Add(new DynamicMenu() { Number = number, Text = termsMenuTitle, ServiceInfo = null });

            Session[DoubleOptSubscriptionUSSDProcessor.DynamicMenuKey] = dynamicMenuList;
            
            return menuBuilder.ToString();            
        }
        public bool ReturnDoubleOptActiveStatus(int subscriptionServiceID, int mobileNetworkID)
        {            
            /*returned columns
             * DoubleOptID,
             * NetworkIDRef,
             * SubscriptionServiceIDRef,
             * Active*/

            bool retVal = false;
            using (var command = DB.ExactDotNet.GetStoredProcCommand("Get_SubscriptionDoubleOptManager"))
            {
                DB.ExactDotNet.AddInParameter(command, "@networkID", DbType.Int32, mobileNetworkID);
                DB.ExactDotNet.AddInParameter(command, "@subscriptionServiceID", DbType.Int32, subscriptionServiceID);

                using (var reader = DB.ExactDotNet.ExecuteReader(command))
                {
                    if (reader.Read())
                        retVal = Convert.ToBoolean(reader["Active"]);                                        
                }
            }
            return retVal;
        }
        #endregion
    }
}
