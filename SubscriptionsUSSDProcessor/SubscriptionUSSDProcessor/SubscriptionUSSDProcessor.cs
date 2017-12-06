using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using exactmobile.ussdservice.common.menu;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace exactmobile.ussdservice.processors.subscriptions
{
    public class SubscriptionUSSDProcessor : BaseUSSDProcessor, IUSSDProcessor
    {
        private const int menu_SendSMSGreatMobileContent = 1;
        private const int menu_ViewSubscriptions = 3;
        private const int menu_JoinMusicClubSubscription = 4;
        private const int menu_unsubscribeFullTrackSubscription = 50;
        private const int menu_unsubscribeGamesWarehouseSubscription = 37;
        private const int menu_JoinGameWarehouseSusbcription = 36;


        private const int menu_DoubleOPTGameWarehouseSusbcription = 1630;
        private const int menu_ConfirmGameWarehouseSusbcription = 1627;
        private const int menu_CancelGameWarehouseSusbcription = 1628;

        private const int menu_DoubleOPTMusicClubSubscription = 1635;
        private const int menu_ConfirmMusicClubSubscription = 1632;
        private const int menu_CancelMusicClubSubscription = 1633;
       
        
        private const int musicClubSubscriptionServiceId = 80;
        private const int GamewarehouseSubscriptionServiceId = 103;

        private const int menu_ShowToGetWAP  = 7;
        private const int menu_StopBilling = 10;
        private const int menu_ContactUs = 5;
        private const int menu_Back = 4;
        private const int menu_ALREADY_SUBSCRIBED = 137;
        private const int menu_ALREADY_SUBSCRIBED_GameWarehouse = 154;
        private const int menu_Join_Gamewarehouse = 4;


        private const int menu_Join_MusicClubSubscription = 2;

        private const int menu_SMS_Link = 17;
        private const int menu_Help = 5;

        private const int menu_YesToMusicClubSubscribe = 178;
        private const int menu_NoToMusicClubSubscribe = 179;
        private const int menu_MainMenuID = 1;

        public SubscriptionUSSDProcessor(USSDSession session, IUSSDHandler handler) : base(session, handler) { }

        public override string OnConnect()
        {
            return string.Format("Welcome to Mutual-Smart\n\n1)Subscribe\n2)Check your Balalance\n3)Unsubscribe\n4)Help");
        }

        public override string ManualOverRide()
        {


            //if (Helper.Message.Equals("0"))
            //    return GoToRootMenu();

            MenuItem currentMenu_Back = new MenuItem();
            string ret_Val = Menu.ShowMenu(Convert.ToInt32(menu_Back), Convert.ToInt32(menu_Back), ref currentMenu_Back,false);
            string BackButton = currentMenu_Back.Name;
            int home = 1;
            string data = null;
            if (Helper.Message == "0")
            {
                MenuItem currentMenu = new MenuItem();
                string results = "";

                if (Session.LastMenuAccessed.ParentMenuId != 0 )
                    results = Menu.ShowMenu(Session.LastMenuAccessed.ParentMenuId, Session.LastMenuAccessed.ParentMenuId, ref currentMenu, false);
                else
                    results = Menu.ShowMenu(home, home, ref currentMenu, false);

                data = results.Replace("DoubleOPT", "");

                Session.LastMenuAccessed = currentMenu;
                  return data;
              
            }



           
            // Services subscribed to
            if (CurrentMenu != null && (CurrentMenu.MenuId == menu_ViewSubscriptions || CurrentMenu.MenuId == menu_unsubscribeFullTrackSubscription || CurrentMenu.MenuId == menu_unsubscribeGamesWarehouseSubscription))
            {
                Session.LastMenuAccessed = CurrentMenu;

                MenuItem currentMenu = new MenuItem();

                string retVal = Menu.ShowMenu(Convert.ToInt32(menu_Back), Convert.ToInt32(menu_Back), ref currentMenu,false);

                data = DisplayListOfSubscriptions() + "\n\n" + currentMenu.Name;
            }// Show information how to unsubscribe from service
            else if (Session.LastMenuAccessed.MenuId == menu_ViewSubscriptions || Session.LastMenuAccessed.MenuId == menu_unsubscribeFullTrackSubscription || Session.LastMenuAccessed.MenuId == menu_unsubscribeGamesWarehouseSubscription)
            {
                Dictionary<int, int> subscriptionMenuItems = Session["SubscriptionMenuItems"] as Dictionary<int, int>;
                data = getUnsubscribeInformation(subscriptionMenuItems.Where(m => m.Key == int.Parse(Helper.Message)).FirstOrDefault().Value);

                data = data.Replace("STOP", "STOP ") + "\n\n" + BackButton;
            }// Join Music Club subscription
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_YesToMusicClubSubscribe)
            {
                Session.LastMenuAccessed = CurrentMenu;


                MenuItem currentMenuBack1 = new MenuItem();

                string ret = Menu.ShowMenu(Convert.ToInt32(menu_Back), Convert.ToInt32(menu_Back), ref currentMenuBack1);

                data = new Subscription().Subscribe(Session.MSISDN, musicClubSubscriptionServiceId,null) + "\n\n" + currentMenuBack1.Name;

                if (data.ToUpper().IndexOf("ALREADY SUBSCRIBED") > 0)
                {
                    MenuItem currentMenu = new MenuItem();

                    string retVal = Menu.ShowMenu(Convert.ToInt32(menu_ALREADY_SUBSCRIBED), Convert.ToInt32(menu_ALREADY_SUBSCRIBED), ref currentMenu);
                    data = currentMenu.Name + "\n\n" + BackButton;
                }

            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_NoToMusicClubSubscribe)
                return GoToMainMenu();
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_CancelGameWarehouseSusbcription)
                return GoToMainMenu();
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_CancelMusicClubSubscription)
                return GoToMainMenu();           
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_Join_MusicClubSubscription)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string results = "";

                MenuItem currentMenu = new MenuItem();

                results = Menu.ShowMenu(Convert.ToInt32(menu_Join_MusicClubSubscription), Convert.ToInt32(menu_Join_MusicClubSubscription), ref currentMenu);


                data = results + "\n\n" + BackButton;


            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_Join_Gamewarehouse)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string results = "";

                MenuItem currentMenu = new MenuItem();

                results = Menu.ShowMenu(Convert.ToInt32(menu_Join_Gamewarehouse), Convert.ToInt32(menu_Join_Gamewarehouse), ref currentMenu);
                

                data = results + "\n\n" + BackButton;


            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_Help)
            {
                Session.LastMenuAccessed = CurrentMenu;

                MenuItem currentMenu = new MenuItem();

                string results = Menu.ShowMenu(Convert.ToInt32(menu_Help), Convert.ToInt32(menu_Help), ref currentMenu);

                data = results.Replace("0. Back", "") + "\n" + BackButton;


            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_JoinGameWarehouseSusbcription)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string results = "";
                MenuItem currentMenu = new MenuItem();

                SubscriptionServiceInfo currentSubscription = new SubscriptionServiceInfo(GamewarehouseSubscriptionServiceId);

                  var isDoubleOpt = new Subscription().GetDoubleOptActiveStatus(GamewarehouseSubscriptionServiceId, Session.MobileNetworkID);

                  if (isDoubleOpt)
                  {
                      results = Menu.ShowMenu(Convert.ToInt32(menu_DoubleOPTGameWarehouseSusbcription), Convert.ToInt32(menu_DoubleOPTGameWarehouseSusbcription), ref currentMenu);
                      data = results.Replace("[clubName]", currentSubscription.DisplayName).Replace("[price]", currentSubscription.Amount.ToString("#.##")).Replace("[frequency]", currentSubscription.BillingFrequency).Replace("DoubleOPT", ""); 
                      Session.LastMenuAccessed = currentMenu; 
                  }
                  else
                  {

                      data = new Subscription().Subscribe(Session.MSISDN, GamewarehouseSubscriptionServiceId,null) + "\n\n" + BackButton;

                      if (data.ToUpper().IndexOf("ALREADY SUBSCRIBED") > 0)
                      {
                        
                          string retVal = Menu.ShowMenu(Convert.ToInt32(menu_ALREADY_SUBSCRIBED_GameWarehouse), Convert.ToInt32(menu_ALREADY_SUBSCRIBED_GameWarehouse), ref currentMenu);

                          data = currentMenu.Name + "\n\n" + BackButton;
                      }
                  }


            }

            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_JoinMusicClubSubscription)
            {
                //dddd
                Session.LastMenuAccessed = CurrentMenu;
                string results = "";
                MenuItem currentMenu = new MenuItem();

                SubscriptionServiceInfo currentSubscription = new SubscriptionServiceInfo(musicClubSubscriptionServiceId);

                var isDoubleOpt = new Subscription().GetDoubleOptActiveStatus(musicClubSubscriptionServiceId, Session.MobileNetworkID);

                if (isDoubleOpt)
                {
                    results = Menu.ShowMenu(Convert.ToInt32(menu_DoubleOPTMusicClubSubscription), Convert.ToInt32(menu_DoubleOPTMusicClubSubscription), ref currentMenu);
                    data = results.Replace("[clubName]", currentSubscription.DisplayName).Replace("[price]", currentSubscription.Amount.ToString("#.##")).Replace("[frequency]", currentSubscription.BillingFrequency).Replace("DoubleOPT", "");
                    Session.LastMenuAccessed = currentMenu;
                }
                else
                {

                    data = new Subscription().Subscribe(Session.MSISDN, musicClubSubscriptionServiceId,null) + "\n\n" + BackButton;

                    if (data.ToUpper().IndexOf("ALREADY SUBSCRIBED") > 0)
                    {

                        string retVal = Menu.ShowMenu(Convert.ToInt32(menu_ALREADY_SUBSCRIBED), Convert.ToInt32(menu_ALREADY_SUBSCRIBED), ref currentMenu);

                        data = currentMenu.Name + "\n\n" + BackButton;
                    }
                }


            }
            //menu_ConfirmMusicClubSubscription
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_ConfirmMusicClubSubscription)
            {
                Session.LastMenuAccessed = CurrentMenu;

                MenuItem currentMenu = new MenuItem();

                data = new Subscription().Subscribe(Session.MSISDN, musicClubSubscriptionServiceId,null) + "\n\n" + BackButton;

                if (data.ToUpper().IndexOf("ALREADY SUBSCRIBED") > 0)
                {

                    string retVal = Menu.ShowMenu(Convert.ToInt32(menu_ALREADY_SUBSCRIBED), Convert.ToInt32(menu_ALREADY_SUBSCRIBED), ref currentMenu);

                    data = currentMenu.Name + "\n\n" + BackButton;
                }

                
            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_ConfirmGameWarehouseSusbcription)
            {
                Session.LastMenuAccessed = CurrentMenu;
               
                MenuItem currentMenu = new MenuItem();               

                data = new Subscription().Subscribe(Session.MSISDN, GamewarehouseSubscriptionServiceId,null) + "\n\n" + BackButton;

                    if (data.ToUpper().IndexOf("ALREADY SUBSCRIBED") > 0)
                    {

                        string retVal = Menu.ShowMenu(Convert.ToInt32(menu_ALREADY_SUBSCRIBED_GameWarehouse), Convert.ToInt32(menu_ALREADY_SUBSCRIBED_GameWarehouse), ref currentMenu);

                        data = currentMenu.Name + "\n\n" + BackButton;
                    }
                


            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_SendSMSGreatMobileContent)
            {
                if (Session["greatMobileContentSMSSend"] == null)
                {
                    Session["greatMobileContentSMSSend"] = true;

                    SMS.sendSMS(Session.Campaign.CampaignID, Session.MSISDN, ConfigurationManager.AppSettings["callCenter_SMSGreatMobileContent"]);

                    MenuItem currentMenu = new MenuItem();

                    string retVal = Menu.ShowMenu(Convert.ToInt32(menu_SMS_Link), Convert.ToInt32(menu_SMS_Link), ref currentMenu);

                    data = currentMenu.Name + "\n\n" + BackButton;

                }
                else
                {
                    data = ConfigurationManager.AppSettings["callCenter_SMSAlreadySent"] + "\n\n" + BackButton;
                }
            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_ContactUs)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string retVal = null;
                MenuItem currentMenu = new MenuItem();

                retVal = Menu.ShowMenu(Convert.ToInt32(menu_ContactUs), Convert.ToInt32(menu_ContactUs), ref currentMenu);

                data = currentMenu.DisplayData + "\n\n" + BackButton;

            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_ShowToGetWAP)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string retVal = null;
                string results = null;
                string line1 = null;
                string line2 = null;
                MenuItem currentMenu = new MenuItem();



                retVal = Menu.ShowMenu(Convert.ToInt32(menu_ShowToGetWAP), Convert.ToInt32(menu_ShowToGetWAP), ref currentMenu);

                results = currentMenu.DisplayData;


                string[] arr = results.Split('*');

                line1 = arr[0].Trim();
                line2 = arr[1].Trim();

                data = line1 + "\n" + line2 + "\n\n" + BackButton;
                data = data.Replace("*", "");


            }
            else if (CurrentMenu != null && CurrentMenu.MenuId == menu_StopBilling)
            {
                Session.LastMenuAccessed = CurrentMenu;
                string retVal = null;
                string results = null;
                string line1 = null;
                string line2 = null;
                MenuItem currentMenu = new MenuItem();



                retVal = Menu.ShowMenu(Convert.ToInt32(menu_StopBilling), Convert.ToInt32(menu_StopBilling), ref currentMenu);

                results = currentMenu.DisplayData;


                string[] arr = results.Split('*');

                line1 = arr[0].Trim();
                line2 = arr[1].Trim();

                data = line1 + "\n" + line2 + "\n\n" + BackButton;
                data = data.Replace("*", "");


            }

            return data.Replace("DoubleOPT\r\n","");
        }
               

        protected string getUnsubscribeInformation(int subscritptionId)
        {
            string data = null;

            Database database = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = database.GetSqlStringCommand("Select top 1 subscriptionID, [description],unsubkeyword,Number,PRSRate	from Sub_Subscriptions sub inner join Sub_SubscriptionServices [service] on subscriptionServiceID = subscriptionServiceIDRef inner join SUB_br_SubscriptionServices_UnsubNumbers unsub on unsub.subscriptionServiceIdRef = subscriptionServiceId inner join sub_unsubNumbers on unsubNumberId = unsubNumberIDRef where 	sub.mobileNumber = @mobileNumber and sub.statusIDRef = 13 and [service].statusIDRef = 13 and subscriptionID = @subscriptionID order by PRSRate");

            DbParameter paramMobileNumber = command.CreateParameter();
            paramMobileNumber.ParameterName = "@MobileNumber";
            paramMobileNumber.Value = Session.MSISDN;
            command.Parameters.Add(paramMobileNumber);
          

            DbParameter paramSubscriptionId = command.CreateParameter();
            paramSubscriptionId.ParameterName = "@subscriptionId";
            paramSubscriptionId.Value = subscritptionId;
            command.Parameters.Add(paramSubscriptionId);

            IDataReader reader = database.ExecuteReader(command);

            if (reader.Read())
            {
                string subscriptionName = reader.GetString(1);
                string unsubKeyword = reader.GetString(2);
                string unsubNumber = reader[3].ToString();
                decimal cost = (decimal)reader[4];

                data = " To unsubscribe from " + subscriptionName + " sms " + unsubKeyword + " to " + unsubNumber + " (" + cost.ToString("c") + "/SMS)";
            }

            reader.Close();
            command.Dispose();

            return data;
        }

        protected string DisplayListOfSubscriptions()
        {
            Session["SubscriptionMenuItems"] = null;

            Database subscriptionsDB = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = subscriptionsDB.GetSqlStringCommand("Select s.SubscriptionID, ss.Description from Sub_Subscriptions s inner join Sub_SubscriptionServices ss on ss.SubscriptionServiceID = s.SubscriptionServiceIDRef where s.MobileNumber = @MobileNumber and s.StatusIDRef = 13 and ss.StatusIDRef = 13 order by ss.Description");
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@MobileNumber";
            parameter.Value = Session.MSISDN;
            command.Parameters.Add(parameter);
            IDataReader reader = subscriptionsDB.ExecuteReader(command);

            StringBuilder menu = new StringBuilder();
            Dictionary<int, int> subscriptionMenuItems = Session["SubscriptionMenuItems"] as Dictionary<int, int>;
            if (subscriptionMenuItems == null)
                subscriptionMenuItems = new Dictionary<int, int>();
            menu.AppendLine("Your Subscriptions:");
            int menuNo = 1;
            while (reader.Read())
            {
                String menuText = String.Format("{0}. {1}", menuNo, reader.GetString(1));
                menu.AppendLine(menuText);
                subscriptionMenuItems.Add(menuNo, reader.GetInt32(0));
                menuNo++;
            }

            Session["SubscriptionMenuItems"] = subscriptionMenuItems;

            command.Dispose();
            return menu.ToString();
        }
        protected string DisplayListOfProvinces()
        {
            Session["ProvinceMenuItems"] = null;

            
            StringBuilder menu = new StringBuilder();
            Dictionary<int, string> subscriptionMenuItems = Session["ProvinceMenuItems"] as Dictionary<int, string>;
            if (subscriptionMenuItems == null)
                subscriptionMenuItems = new Dictionary<int, string>();
                menu.AppendLine("Your Provinces:");
            menu.AppendLine(String.Format("{0}. {1}", 1 , "Bas - Uele"));
            menu.AppendLine(String.Format("{0}. {1}",2 , "Équateur"));
            menu.AppendLine(String.Format("{0}. {1}",3 , "Haut - Katanga"));
            menu.AppendLine(String.Format("{0}. {1}",4 , "Haut - Lomami"));
            menu.AppendLine(String.Format("{0}. {1}",5 , "Haut - Uele"));
            menu.AppendLine(String.Format("{0}. {1}",6 , "Ituri"));
            menu.AppendLine(String.Format("{0}. {1}",7 , "Kasaï"));
            menu.AppendLine(String.Format("{0}. {1}",8 , "Kasaï - Central"));
            menu.AppendLine(String.Format("{0}. {1}",9 , "Kasaï - Oriental"));
            menu.AppendLine(String.Format("{0}. {1}",10, "Kinshasa"));
            menu.AppendLine(String.Format("{0}. {1}",11, "Kongo - Central"));
            menu.AppendLine(String.Format("{0}. {1}",12, "Kwango"));
            menu.AppendLine(String.Format("{0}. {1}",13, "Kwilu"));
            menu.AppendLine(String.Format("{0}. {1}",14, "Lomami"));
            menu.AppendLine(String.Format("{0}. {1}",15, "Lualaba"));
            menu.AppendLine(String.Format("{0}. {1}",16, "Mai - Ndombe"));
            menu.AppendLine(String.Format("{0}. {1}",17, "Maniema"));
            menu.AppendLine(String.Format("{0}. {1}",18, "Mongala"));
            menu.AppendLine(String.Format("{0}. {1}",19, "Nord - Kivu"));
            menu.AppendLine(String.Format("{0}. {1}",20, "Nord - Ubangi"));
            menu.AppendLine(String.Format("{0}. {1}",21, "Sankuru"));
            menu.AppendLine(String.Format("{0}. {1}",22, "Sud - Kivu"));
            menu.AppendLine(String.Format("{0}. {1}",23, "Sud - Ubangi"));
            menu.AppendLine(String.Format("{0}. {1}",24, "Tanganyika"));
            menu.AppendLine(String.Format("{0}. {1}",25, "Tshopo"));
            menu.AppendLine(String.Format("{0}. {1}",26, "Tshuapa"));

            subscriptionMenuItems.Add( 1, "Bas - Uele");
            subscriptionMenuItems.Add( 2, "Équateur");
            subscriptionMenuItems.Add( 3, "Haut - Katanga");
            subscriptionMenuItems.Add( 4, "Haut - Lomami");
            subscriptionMenuItems.Add( 5, "Haut - Uele");
            subscriptionMenuItems.Add( 6, "Ituri");
            subscriptionMenuItems.Add( 7, "Kasaï");
            subscriptionMenuItems.Add( 8, "Kasaï - Central");
            subscriptionMenuItems.Add( 9, "Kasaï - Oriental");
            subscriptionMenuItems.Add( 10, "Kinshasa");
            subscriptionMenuItems.Add( 11, "Kongo - Central");
            subscriptionMenuItems.Add( 12, "Kwango");
            subscriptionMenuItems.Add( 13, "Kwilu");
            subscriptionMenuItems.Add( 14, "Lomami");
            subscriptionMenuItems.Add( 15, "Lualaba");
            subscriptionMenuItems.Add( 16, "Mai - Ndombe");
            subscriptionMenuItems.Add( 17, "Maniema");
            subscriptionMenuItems.Add( 18, "Mongala");
            subscriptionMenuItems.Add( 19, "Nord - Kivu");
            subscriptionMenuItems.Add( 20, "Nord - Ubangi");
            subscriptionMenuItems.Add( 21, "Sankuru");
            subscriptionMenuItems.Add( 22, "Sud - Kivu");
            subscriptionMenuItems.Add( 23, "Sud - Ubangi");
            subscriptionMenuItems.Add( 24, "Tanganyika");
            subscriptionMenuItems.Add( 25, "Tshopo");
            subscriptionMenuItems.Add(26, "Tshuapa");

               

            Session["ProvinceMenuItems"] = subscriptionMenuItems;

            return menu.ToString();
        }

        private string GoToMainMenu()
        {            
            MenuItem currentMenu = new MenuItem();
            var data = Menu.ShowMenu(menu_MainMenuID, menu_MainMenuID, ref currentMenu);
            Session.LastMenuAccessed = currentMenu;
            return data;
        }
    }

   
}
