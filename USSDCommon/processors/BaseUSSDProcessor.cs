using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.menu;
using exactmobile.ussdcommon;
using exactmobile.components;
using USSD.BLL;

namespace exactmobile.ussdservice.common.processors
{
    public abstract class BaseUSSDProcessor
    {        
        private IUSSDHandler handler;    

        private readonly string campaignNotStartedMessage;
        private readonly string campaignEndedMessage;
        const string CurrentMenuTransactionIDKey = "CurrentMenuTransactionID";
        
        public MenuItem CurrentMenu
        {
            get;
            set;
        }



        public MenuItem CurrentMenuBack
        {
            get;
            set;
        }
        public USSDSession Session 
        {
            get;
            set;
        }

        public IUSSDHandler Handler 
        {
            get { return this.handler; }
            set { this.handler = value; }
        }

        public USSDStringHelper Helper
        {
            get;
            set;
        }

        public MenuManager Menu
        {
            get;
            set;
        }

        public BaseUSSDProcessor(USSDSession session, IUSSDHandler handler)
        {
       

            this.Session = session;
            this.handler = handler;

            if (!ussdcommon.utility.CommonUtility.TryGetAppSettings("CampaignNotStartedMessage", true, out campaignNotStartedMessage))
                campaignNotStartedMessage = "This campaign starts on [date]";

            if (!ussdcommon.utility.CommonUtility.TryGetAppSettings("CampaignEndedMessage", true, out campaignEndedMessage))
                campaignEndedMessage = "This campaign ended on [date]";            
        }

        public void Initialize(string requestData, USSDHandlerRequestType.RequestTypes inputRequestType)
        {
            Menu = new MenuManager(Session.Campaign.CampaignID, Session.USSDNumber.Value, Session.MainMenuId,Session.Campaign.BackButton);
            Menu.ShowMenuEvent += new MenuManager.ShowMenuEventHandler(Menu_ShowMenuEvent);
            Menu.CommunicateMenuEvent += new MenuManager.CommunicateMenuEventHandler(Menu_CommunicateMenuEvent);
            Menu.ThirdPartyCommunicationEvent += new MenuManager.ThirdPartyCommunicationEventHandler(Menu_ThirdPartyCommunicationEvent);
            Menu.MenuOBSBillingEvent += new MenuManager.OBSBillingEventHandler(Menu_MenuOBSBillingEvent);
        }

        void Menu_MenuOBSBillingEvent(object sender, OBSBillingEventArgs args)
        {
            if (args.IsValid)
            {
                OBSBilling.BillUser(Session.Campaign.CampaignID, args.BillingMenuItem.MenuId, Session.USSDTransactionID, Session.MSISDN, Session.MobileNetworkID, args.BillingMenuItem.BillAmount);
            }
        }

        void Menu_ThirdPartyCommunicationEvent(object sender, MenuItem currentMenuItem)
        {
            if (currentMenuItem.IsSMS.HasValue)
            {
                if (currentMenuItem.IsSMS.Value)
                {
                    string communicationFeedback;
                    var recipients = currentMenuItem.ThirdPartyCommunicationRecipients.Replace(" ","").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    string commsText = String.Format("{0}; Mobile Number: {1}", currentMenuItem.ThirdPartyCommunicationText,Session.MSISDN);
                    foreach (var number in recipients)
                    {
                        exactmobile.ussdcommon.MenuCommunication.SendCommunication(Session.Campaign.CampaignID, currentMenuItem.MenuId, Session.USSDTransactionID, MenuItem.CommunicationType.SMS, ussdcommon.utility.CommonUtility.ReturnNetworkID(number.Trim()), number.Trim(), commsText, (Session.USSDNumber.HasValue ? Convert.ToString(Session.USSDNumber.Value) : ""), currentMenuItem.WaitForHTTPResponse,false, out communicationFeedback);
                        
                    }
                }
                else
                {
                    string smtpServer;
                    if (!ussdcommon.utility.CommonUtility.TryGetAppSettings("SMTPServer", true, out smtpServer))
                        smtpServer = "192.168.1.110";
                    
                    CXMLCapsule capsule = new CXMLCapsule();

                    capsule.SetParameter("Sender", "noreply@exactmobile.com", true);
                    capsule.SetParameter("Recipients", currentMenuItem.ThirdPartyCommunicationRecipients.Replace(" ", ""), true);
                    capsule.SetParameter("Subject", "Exact USSD Notification Campaign #" + Session.Campaign.CampaignID.ToString(), true);
                    capsule.SetParameter("Priority", "2", true);
                    capsule.SetParameter("SMTPServer", smtpServer, true);                    
                    capsule.SetParameter("Body", string.Format("{0}. Mobile Number: {1}",currentMenuItem.ThirdPartyCommunicationText,Session.MSISDN), true);
                    capsule.SetParameter("BodyFormat", "", true);
                    capsule.SetParameter("CarbonCopies", "", true);
                    capsule.SetParameter("BlindCarbonCopies", "", true);

                    CQueueWrapper mailQueue = new CQueueWrapper("EmailOutQ");
                    mailQueue.Send(capsule);                    
                }
            }
        }

        void Menu_CommunicateMenuEvent(object sender, CommunicateMenuEventArgs args)
        {            
            if (args.CanCommunicate)
            {   
                string communicationFeedback = null;            
                //send out communication request here
                var commsType = ((MenuItem.CommunicationType)Enum.Parse(typeof(MenuItem.CommunicationType),args.Menu.CommunicationTypeID.ToString()));
                
                exactmobile.ussdcommon.MenuCommunication.SendCommunication(Session.Campaign.CampaignID, args.Menu.MenuId, Session.USSDTransactionID, commsType, Session.MobileNetworkID, Session.MSISDN, Convert.ToString(args.Menu.CommunicationText), (Session.USSDNumber.HasValue ? Convert.ToString(Session.USSDNumber.Value) : ""),args.Menu.WaitForHTTPResponse,args.Menu.SendHTTPResponseBySMS, out communicationFeedback);

                //set your communicationFeedback value here if the communication request yielded
                //displayed results e.g. SM2HTTP
                if (sender is IResultsOverridable && !string.IsNullOrEmpty(communicationFeedback))
                {     
                    (sender as IResultsOverridable).OverrideValue = communicationFeedback;               
                    (sender as IResultsOverridable).ResultsOverriden = true;                    
                }
            }
        }

        private void Menu_ShowMenuEvent(object sender, ShowMenuEventArgs args)
        {
            //perform all menu transaction logging here
            int MenuSection = 0; 
            long menuTransactionID;
            string enteredSelectionValue;
            if(!string.IsNullOrEmpty(Helper.Message))
                enteredSelectionValue = ((Helper.Message.Length >= 2) ? Helper.Message : Helper.Message);
               // enteredSelectionValue = ((Helper.Message.Length>=2) ? "-1":Helper.Message); // old
            else
                enteredSelectionValue = string.Empty;

            Session["HelperMessage"] = Helper.Message;

            if (Session["MenuSection"] != null && Session["FlagBackMenu"].ToString() == "false")
            {
                try
                {

                    if (Helper.Message != "0")
                        Session["MenuSection"] = int.Parse(Session["MenuSection"].ToString()) + 1;
                    MenuSection = int.Parse(Session["MenuSection"].ToString());
                    Session["FlagBackMenu"] = "false_";
                }
                catch
                {
                    MenuSection = 0;
                }
            }
            else if (Session["MenuSection"] != null && Session["FlagBackMenu"].ToString() == "true")
            {
                try
                {

                    Session["MenuSection"] = int.Parse(Session["MenuSection"].ToString()) - 1;
                    //  MenuSection = int.Parse(Session["MenuSection"].ToString());
                    Session["FlagBackMenu"] = "true_";
                }
                catch
                {
                    MenuSection = 0;
                }
            }
            else 
            {
                MenuSection = 0; //Duplicate 
            }
           




            if (MenuTransaction.Add(args.RequestedMenuID, Session.Campaign.CampaignID, Session.USSDTransactionID, Session.MobileNetworkID, Session.MSISDN, enteredSelectionValue, MenuSection, out menuTransactionID))
                Session[BaseUSSDProcessor.CurrentMenuTransactionIDKey] = menuTransactionID;
            else
                Session[BaseUSSDProcessor.CurrentMenuTransactionIDKey] = -1;

          

        }

        public string ProcessRequest(string requestData)
        {
            string data = requestData;

            Helper = USSDStringHelper.Decode(Handler.USSDString);

           
            

            if (Helper.Message == "0")
                Session["FlagBackMenu"] = "true";
            else
                Session["FlagBackMenu"] = "false";


            
           //check if the campaign is still active
            if (Session.Campaign != null)
            {
                if (Session.Campaign.StartDateTime > DateTime.Now)
                    return campaignNotStartedMessage.Replace("[date]", Session.Campaign.StartDateTime.ToString("dd MMMM yyyy"));
                if (Session.Campaign.EndDateTime < DateTime.Now)
                    return campaignEndedMessage.Replace("[date]", Session.Campaign.EndDateTime.ToString("dd MMMM yyyy"));       
            }

            if (Session.MainMenuId == 0)
            {               

                Session.MainMenuId = MenuManager.GetRootMenuId(Session.Campaign.CampaignID);
                data = OnConnect();

                if (data == null)
                {                    
                    MenuItem currentMenu = new MenuItem();
                    data = Menu.ShowMenu(Session.MainMenuId, Session.MainMenuId, ref currentMenu);
                    Session.LastMenuAccessed = currentMenu;
                    Session["data"] = data;
                }
            }
            else
            {
               
                CurrentMenu = Menu.FindMenuItemByReturnValueNEW(Session.LastMenuAccessed, Helper.Message);
                //exactmobile.components.logging.LogManager.LogStatus("Message string: {0}", Helper.Message ) ;
                if (Helper.Message != Session.Campaign.BackButton)
                {
                    data = ManualOverRide();

                    if (data == null)
                    {// Build Dynamic menu
                        if (CurrentMenu != null)
                        {
                            var menu = new MenuItem();
                            data = Menu.ShowMenu(Session.LastMenuAccessed.MenuId, CurrentMenu.MenuId, ref menu);
                            CurrentMenu = menu;
                            Session.LastMenuAccessed = CurrentMenu;
                        }
                        else
                        {
                            if (Session.LastMenuAccessed!= null && Session.LastMenuAccessed.ReturnMenuId > 0)
                            {
                                CurrentMenu = new MenuItem();
                                var menu = new MenuItem();
                                data = Menu.ShowMenu(Session.LastMenuAccessed.ReturnMenuId, Session.LastMenuAccessed.ReturnMenuId, ref menu);
                                CurrentMenu = menu;
                                Session.LastMenuAccessed = CurrentMenu;
                            }
                            else
                                data += "You have made an invalid selection.\n" + Session.Campaign.BackButton + ". Back";// +Menu.ShowMenu(Session.LastMenuAccessed.MenuId, Session.LastMenuAccessed.MenuId, ref currentMenu);
                        }
                    }
                }
                else if (Helper.Message == "0" && (Session.Campaign.CampaignID == 1))//override for the back button on campaign that use the 0 button for paging and # to go to home screen
                {
                    data = ManualOverRide();

                    if (data == null)
                    {// Build Dynamic menu
                        if (CurrentMenu != null)
                        {
                            var menu = new MenuItem();
                            data = Menu.ShowMenu(Session.LastMenuAccessed.MenuId, CurrentMenu.MenuId, ref menu);
                            CurrentMenu = menu;
                            Session.LastMenuAccessed = CurrentMenu;
                        }
                        else
                        {
                            if (Session.LastMenuAccessed.ReturnMenuId > 0)
                            {
                                CurrentMenu = new MenuItem();
                                var menu = new MenuItem();
                                data = Menu.ShowMenu(Session.LastMenuAccessed.ReturnMenuId, Session.LastMenuAccessed.ReturnMenuId, ref menu);
                                CurrentMenu = menu;
                                Session.LastMenuAccessed = CurrentMenu;
                            }
                            else
                                data += "You have made an invalid selection.\n#. Home";
                        }
                    }
                }
                else
                {
                    // Go to previous menu
                    CurrentMenu = new MenuItem();
                    var menu = new MenuItem();

                   
                    data = Menu.ShowMenu(Session.LastMenuAccessed.ParentMenuId, Session.LastMenuAccessed.ParentMenuId, ref menu,true,false,false);

                    CurrentMenu = menu;
                    if (Session.LastMenuAccessed.MenuId != CurrentMenu.MenuId)
                    {
                        Session.LastMenuAccessed = CurrentMenu;
                        string EnteredValue = Session["EnteredValue"].ToString();
                        if (Helper.Message == "0")
                        {
                           
                            UpdateBackMenu(Session.USSDTransactionID, CurrentMenu.MenuId, Session.Campaign.CampaignID, EnteredValue);
                        }
                    }
                    else
                    {
                        MenuItem tmp1 = Menu.GetParentMenu(Session.LastMenuAccessed.ParentMenuId);

                        if (tmp1 != null)
                        {
                            menu = new MenuItem();
                            CurrentMenu = Menu.GetParentMenu(tmp1.MenuId);
                            string EnteredValue = Session["EnteredValue"].ToString();
                            if (Helper.Message == "0")
                            {

                                UpdateBackMenu(Session.USSDTransactionID, CurrentMenu.MenuId, Session.Campaign.CampaignID, EnteredValue);
                            }
                            data = Menu.ShowMenu(CurrentMenu.ParentMenuId, CurrentMenu.ParentMenuId, ref menu, true, false, false);
                            CurrentMenu = menu;
                            Session.LastMenuAccessed = CurrentMenu;
                        }
                        else
                        {
                            menu = new MenuItem();
                            data = "You have made an invalid selection.\n" + Menu.ShowMenu(Session.MainMenuId, Session.MainMenuId, ref menu, true, false, false);
                            CurrentMenu = menu;
                            Session.LastMenuAccessed = CurrentMenu;
                        }
                    }
                }
            }
            Session["data"] = data;
            return data;
        }
        static bool UpdateBackMenu(long USSDtransactionID, long MenuID, int campaignID, string EnteredSelectionValue)
        {
            var model = Logic.Instance.MenuTransactions.ListFiltered(new USSD.Entities.MenuTransaction { FilterUSSDMenuId = MenuID, FilterUSSDCampaignId = campaignID,FilterUSSDTransactionId = USSDtransactionID }).FirstOrDefault();
            if(model != null)
            {
                model.EnteredSelectionValue = EnteredSelectionValue;
                Logic.Instance.MenuTransactions.Update(model); 
                return true;
            }
            return false;
        }
        public abstract string ManualOverRide();
        public abstract string OnConnect();
                       
    }
}
