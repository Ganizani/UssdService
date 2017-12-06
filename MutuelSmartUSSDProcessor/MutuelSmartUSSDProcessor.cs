using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.common.processors;
using exactmobile.ussdservice.common.session;
using exactmobile.ussdcommon;
using exactmobile.ussdservice.common.menu;

using System.Configuration;
using USSD.Entities;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace exactmobile.ussdservice.processors
{
    public class MutuelSmartUSSDProcessor : BaseUSSDProcessor, IUSSDProcessor
    {
        private const string YES_TO_SUBSCRIBE = "notify# Thank you for subscribing to mutuelle smart, you will receive an SMS short with more info.";
        private const string NO_TO_SUBSCRIBE = "notify# You are currently not subscribed to mutuelle smart!";
        private const string CONFIRMATION_SUCCESS = "notify# Bravo! ";
        private const string CONFIRMATION_ERROR = "notify# Oops le code est invalide!";
        private exactmobile.ussdcommon.Subscription subscription = null;
        #region Enums
        public enum CampaignMenu : int
        {
            MutualSmart = 1,
            ProvinceSelection = 2,
            CommuneSelection = 5,
            CitySelection = 3,
            MutuelSelection = 6,
            SubscriptionConfirmation = 7,
            YesToSubscribe = 8,
            NoToSubscribe = 9,
            CancelSubscribeMenu = 10,
            ViewCredits = 377,
            YesToUnsubscribe = 11,
            NoToUnsubscribe = 12,
            Competitions = 1508,
            BackButton = 4
        };

        public enum StatusEnum : int
        {
            New = 1,
            Active =2,
            Expired = 3,
            Inactive = 4,
            Cancelled =5
        }
        #endregion
        public MutuelSmartUSSDProcessor(USSDSession session, IUSSDHandler handler) : base(session, handler) { }


        public override string OnConnect()
        {
            MenuItem currentMenu = new MenuItem();
            var menu = GetMenuScreen(CampaignMenu.MutualSmart);
            subscription = new exactmobile.ussdcommon.Subscription();
            if (subscription.IsSubscribedToService(Session.MSISDN, 1))
            {
                var model = subscription.GetSubscription(Session.MSISDN, 1);
                if (model != null && model.status_id == 1)
                {
                    return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)Confirmer \n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
                }

                else
                {
                    return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)Services\n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
                }

            }
            else
            {
                return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)souscrire\n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
            }

        }
        public override string ManualOverRide()
        {

            //if (Helper.Message.Equals("0"))
            //    return GetHomeScreen();
            try {
                if (string.IsNullOrEmpty(Helper.Message))

                {

                    return string.Format("notify# votre choix n'est pas correcte! ");

                }
                if (subscription.IsSubscribedToService(Session.MSISDN, 1))
                {
                    var model = subscription.GetSubscription(Session.MSISDN, 1);
                    if (model != null && model.status_id == 1 && Helper.Message == "1" && Session.LastMenuAccessed.MenuId == 1)
                    {


                        if (Helper.Message == "1" && Session.LastMenuAccessed.MenuId == 1)

                        {

                            return string.Format("request# Confirmation Menu\n\nVeillez typer le numero de reference");

                        }
                        else
                        {
                            if (model.reference_number == Helper.Message.Trim())
                            {
                                UpdateSubscription(model, 2); //Activate subscription
                                return CONFIRMATION_SUCCESS;
                            }
                            else
                            {
                                return CONFIRMATION_ERROR;
                            }
                        }
                    }

                    else if (model.status_id != 3 && Helper.Message != "3" && Session.LastMenuAccessed.MenuId == 1)
                    {
                        return string.Format("request# Bienvenu a la Mutuelle Smart!\n\n1)Services\n2)Balalance\n3)Se desabonner\n4)Guide");
                    }
                    else
                    {
                        if (Helper.Message == "3" && Session.LastMenuAccessed.MenuId == 1)

                        {
                            return "request# Confirmation de desabonnement\n\n" + GetMenuScreen(CampaignMenu.CancelSubscribeMenu);
                        }
                        if (Helper.Message == "1" && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CancelSubscribeMenu)

                        {
                            UpdateSubscription(model, 3); //Activate subscription
                            return string.Format("nofify# Nous vous avons desabonne avec succes!");
                        }
                        if (Helper.Message == "2" && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CancelSubscribeMenu)

                        {
                            return string.Format("request# Bienvenu a la Mutuelle Smart!\n\n1)Services\n2)Balalance\n3)Se desabonner\n4)Guide");
                        }
                    }

                }

                MenuItem currentMenu_Back = new MenuItem();
                string ret_Val = Menu.ShowMenu(Convert.ToInt32(CampaignMenu.ProvinceSelection), Convert.ToInt32(CampaignMenu.ProvinceSelection), ref currentMenu_Back, false);
                string BackButton = currentMenu_Back.Name;
                string data = null;
                string selection = Helper.Message;

                if (Helper.Message == "1" && Session.LastMenuAccessed.MenuId == 1 || ((Helper.Message == "10" || Helper.Message == "0") && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.ProvinceSelection)
                    )
                {
                    MenuItem currentMenu = new MenuItem();
                    string results = "";

                    if (Session.LastMenuAccessed.ParentMenuId != 1)
                    {
                        results = Menu.ShowMenu(Session.LastMenuAccessed.ParentMenuId, Session.LastMenuAccessed.ParentMenuId, ref currentMenu, false);
                        Session.LastMenuAccessed = currentMenu;
                    }
                    else
                        results = GetMenuScreen(CampaignMenu.ProvinceSelection);
                    //var provinces = DisplayListOfProvinces();
                    data = results;
                    if (data.Length < 180)
                    {
                        data += GetMenuSubScreen(CampaignMenu.ProvinceSelection, data.Length, selection);

                    }
                    return "request# " + data;

                }
                else if (((Helper.Message == "10" || Helper.Message == "0") && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CitySelection) || (Session.LastMenuAccessed.MenuId == (int)CampaignMenu.ProvinceSelection))
                {
                    MenuItem currentMenu = new MenuItem();
                    string results = "";
                    if (Session["SelectedProvince"] == null)
                    {
                        try
                        {
                            Session["SelectedProvince"] = (int.Parse(selection) > 10 && selection != "10") ? selection = (int.Parse(selection) - 1 == 10 ? int.Parse(selection) - 2 : int.Parse(selection) - 1).ToString() : Helper.Message;
                        }
                        catch
                        {
                            results = Menu.ShowMenu(Session.LastMenuAccessed.ParentMenuId, Session.LastMenuAccessed.ParentMenuId, ref currentMenu, false);
                            Session.LastMenuAccessed = currentMenu;
                        }
                    }

                    if (Session.LastMenuAccessed.ParentMenuId != 1)
                    {
                        results = Menu.ShowMenu((int)CampaignMenu.CitySelection, (int)CampaignMenu.CitySelection, ref currentMenu, false);
                        Session.LastMenuAccessed = currentMenu;
                    }
                    else
                        results = GetMenuScreen(CampaignMenu.CitySelection);
                    data = results;
                    if (data.Length < 180)
                    {
                        data += GetMenuSubScreen(CampaignMenu.CitySelection, data.Length, selection);

                    }
                    return "request# " + data;

                }
                else if (((Helper.Message == "10" || Helper.Message == "0") && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CommuneSelection) || (Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CitySelection))
                {
                    MenuItem currentMenu = new MenuItem();
                    string results = "";
                    if (Session["SelectedCity"] == null)

                    {
                        try
                        {
                            Session["SelectedCity"] = (int.Parse(selection) > 10 && selection != "10") ? selection = (int.Parse(selection) - 1 == 10 ? TestData.ListOfCities().cities.Where(n => n.province_id == Convert.ToInt32(Session["SelectedProvince"])).ToList()[int.Parse(selection) - 2].city_id : TestData.ListOfCities().cities.Where(n => n.province_id == Convert.ToInt32(Session["SelectedProvince"])).ToList()[int.Parse(selection) - 1].city_id).ToString() : TestData.ListOfCities().cities.Where(n => n.province_id == Convert.ToInt32(Session["SelectedProvince"])).ToList()[int.Parse(Helper.Message) - 1].city_id.ToString();
                        }
                        catch
                        {
                            results = Menu.ShowMenu((int)CampaignMenu.CitySelection, (int)CampaignMenu.CitySelection, ref currentMenu, false);
                            Session.LastMenuAccessed = currentMenu;
                        }
                    }
                    if (Session.LastMenuAccessed.ParentMenuId != 1)
                    {
                        results = Menu.ShowMenu((int)CampaignMenu.CommuneSelection, (int)CampaignMenu.CommuneSelection, ref currentMenu, false);
                        Session.LastMenuAccessed = currentMenu;
                    }
                    else
                        results = GetMenuScreen(CampaignMenu.CommuneSelection);
                    data = results;
                    if (data.Length < 180)
                    {
                        data += GetMenuSubScreen(CampaignMenu.CommuneSelection, data.Length, selection);

                    }
                    return "request# " + data;

                }
                else if (((Helper.Message == "10" || Helper.Message == "0") && Session.LastMenuAccessed.MenuId == (int)CampaignMenu.MutuelSelection) || (Session.LastMenuAccessed.MenuId == (int)CampaignMenu.CommuneSelection))
                {
                    MenuItem currentMenu = new MenuItem();
                    string results = "";
                    if (Session["SelectedCommune"] == null)
                    {
                        try
                        {
                            Session["SelectedCommune"] = (int.Parse(selection) > 10 && selection != "10") ? selection = (int.Parse(selection) - 1 == 10 ? TestData.ListOfCommunes().communes.Where(n => n.city_id == Convert.ToInt32(Session["SelectedCity"])).ToList()[int.Parse(selection) - 2].commune_id : TestData.ListOfCommunes().communes.Where(n => n.city_id == Convert.ToInt32(Session["SelectedCity"])).ToList()[int.Parse(selection) - 1].commune_id).ToString() : TestData.ListOfCommunes().communes.Where(n => n.city_id == Convert.ToInt32(Session["SelectedCity"])).ToList()[int.Parse(Helper.Message) - 1].commune_id.ToString();
                        }
                        catch
                        {
                            results = Menu.ShowMenu((int)CampaignMenu.CommuneSelection, (int)CampaignMenu.CommuneSelection, ref currentMenu, false);
                            Session.LastMenuAccessed = currentMenu;
                        }
                    }


                    if (Session.LastMenuAccessed.ParentMenuId != 1)
                    {
                        results = Menu.ShowMenu((int)CampaignMenu.MutuelSelection, (int)CampaignMenu.MutuelSelection, ref currentMenu, false);
                        Session.LastMenuAccessed = currentMenu;
                    }
                    else
                        results = GetMenuScreen(CampaignMenu.MutuelSelection);
                    data = results;
                    if (data.Length < 180)
                    {
                        data += GetMenuSubScreen(CampaignMenu.MutuelSelection, data.Length, selection);

                    }
                    return "request# " + data;

                }
                else if (Session.LastMenuAccessed.MenuId == (int)CampaignMenu.MutuelSelection)

                {
                    if (Session["SelectedMutuelle"] == null)

                    {
                        try
                        {
                            Session["SelectedMutuelle"] = (int.Parse(selection) > 10 && selection != "10") ? selection = (int.Parse(selection) - 1 == 10 ? TestData.ListOfMutuels().mutuels.Where(n => n.commune_id == Convert.ToInt32(Session["SelectedCommune"])).ToList()[int.Parse(selection) - 2].mutuel_id : TestData.ListOfMutuels().mutuels.Where(n => n.commune_id == Convert.ToInt32(Session["SelectedCommune"])).ToList()[int.Parse(selection) - 1].mutuel_id).ToString() : TestData.ListOfMutuels().mutuels.Where(n => n.commune_id == Convert.ToInt32(Session["SelectedCommune"])).ToList()[int.Parse(Helper.Message) - 1].mutuel_id.ToString();
                        }
                        catch
                        {
                        }
                    }
                    MenuItem currentMenu = new MenuItem();
                    data = "request# Confirmation d'abonnement\n\n" + GetMenuScreen(CampaignMenu.SubscriptionConfirmation);
                    return data;

                }
                else if (Session.LastMenuAccessed.MenuId == (int)CampaignMenu.SubscriptionConfirmation)

                {
                    if (Convert.ToInt32(Helper.Message) == 1)
                    {
                        //subcribe user to mutuelle smart
                        var sub = new ussdcommon.Subscription();
                        data = sub.Subscribe(Session.MSISDN, 1, int.Parse(Session["SelectedMutuelle"].ToString()));

                        ussdcommon.utility.CommonUtility.SendSMSNotification(Session.MSISDN, "Details of the mutuel will shortly be sent to you");
                        //return data;// -- YES_TO_SUBSCRIBE;
                        return "notify# " + data;
                    }
                    else
                    {
                        return NO_TO_SUBSCRIBE;
                    }


                }

                
            }catch{
                GetHomeScreen();
            }
            return GetHomeScreen();
        }

        private void UpdateSubscription(USSD.Entities.Subscription model, int statusId)
        {
            model.status_id = statusId;
            var client = ussdcommon.utility.CommonUtility.GetHttpConnection("MutuelleSmartAPI");
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var URL = new Uri(client.BaseAddress, "subscriptions" );

            using (client)
            {
                try
                {
                    var result = client.PutAsync(URL, content).Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {

                       // log.info( "subscribed successfully");
                    }

                }
                catch (Exception exp)
                {
                }
            }


        }

        private string GetMenuSubScreen(CampaignMenu modelSelection, int length, string selection)
        {
            try
            {
                string returnValue = string.Empty;
                string backValue = "\n0)back\n10)more";
                //tricky stuff. you need to build a string less the length so that it will fit on the returned string and keep track of where you
                // at the the collection for navigation.
                switch (modelSelection)
                {
                    case CampaignMenu.ProvinceSelection:
                        #region Province
                        DisplayListOfProvinces();
                        Dictionary<int, string> provinceMenuItems = Session["ProvinceMenuItems"] as Dictionary<int, string>;
                        int index = Session["CurrentProvinceSelection"] != null ? (int)Session["CurrentProvinceSelection"] : 0;
                        int displayIndex = index >= 10 ? index + 2 : index + 1;
                        if (selection == "10" || index == 0)
                        {
                            Session["PreviousProvinceSelection"] = index;

                            while (index >= 0 && (index < provinceMenuItems.Count) && (length + provinceMenuItems[index + 1].Length + backValue.Length) < 180)
                            {
                                if (index == 0)
                                {
                                    backValue = "\n10)more";
                                }
                                if (displayIndex == 10)
                                {
                                    displayIndex++;
                                }
                                returnValue += $"\n{displayIndex}){provinceMenuItems[index + 1]}";

                                index++;
                                displayIndex++;

                                length += returnValue.Length;
                                Session["CurrentProvinceSelection"] = index;
                                if (index >= provinceMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }
                            }
                        }
                        else if (selection == "0")
                        {
                            index = Convert.ToInt32(Session["PreviousProvinceSelection"]);
                            displayIndex = index >= 10 ? index + 1 : index;
                            Session["CurrentProvinceSelection"] = index;
                            while (index > 0 && (index <= provinceMenuItems.Count) && (length + provinceMenuItems[index].Length + backValue.Length) < 180)
                            {
                                if (index >= provinceMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }

                                returnValue += $"\n{displayIndex  }){provinceMenuItems[index]}";

                                index--;
                                displayIndex--;
                                if (displayIndex == 10)
                                {

                                    displayIndex--;
                                }
                                length += returnValue.Length;
                                Session["PreviousProvinceSelection"] = index;


                                if (index == 1)
                                {
                                    backValue = "\n10)more";
                                }
                            }
                        }
                        break;
                    #endregion
                    case CampaignMenu.CitySelection:
                        #region Cities
                        List<City> cityMenuItems = TestData.ListOfCities().cities.Where(n => n.province_id == Convert.ToInt32(Session["SelectedProvince"])).ToList();

                        // Dictionary<int, string> cityMenuItems = Session["CityMenuItems"] as Dictionary<int, string>;
                        index = Session["CurrentCitySelection"] != null ? (int)Session["CurrentCitySelection"] : 0;
                        displayIndex = index >= 10 ? index + 2 : index + 1;
                        if (selection == "10" || index == 0)
                        {
                            if (selection == "0" && cityMenuItems.Count == 0)
                                return GetHomeScreen();
                            Session["PreviousCitySelection"] = index;

                            while (index >= 0 && (index < cityMenuItems.Count) && (length + cityMenuItems[index].city_name.Length + backValue.Length) < 180)
                            {
                                if (index == 0)
                                {
                                    backValue = "\n10)more";
                                }
                                if (displayIndex == 10)
                                {
                                    displayIndex++;
                                }
                                returnValue += $"\n{displayIndex}){cityMenuItems[index].city_name}";

                                index++;
                                displayIndex++;

                                length += returnValue.Length;
                                Session["CurrentCitySelection"] = index;
                                if (index >= cityMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }
                            }
                        }
                        else if (selection == "0")
                        {
                            index = Convert.ToInt32(Session["PreviousCitySelection"]);
                            displayIndex = index >= 10 ? index + 1 : index;
                            Session["CurrenCitySelection"] = index;
                            while (index > 0 && (index <= cityMenuItems.Count) && (length + cityMenuItems[index].city_name.Length + backValue.Length) < 180)
                            {
                                if (index >= cityMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }

                                returnValue += $"\n{displayIndex  }){cityMenuItems[index].city_name}";

                                index--;
                                displayIndex--;
                                if (displayIndex == 10)
                                {

                                    displayIndex--;
                                }
                                length += returnValue.Length;
                                Session["PreviousCitySelection"] = index;


                                if (index == 1)
                                {
                                    backValue = "\n10)more";
                                }
                            }
                        }
                        break;
                    #endregion
                    case CampaignMenu.CommuneSelection:
                        #region Commune

                        List<Commune> communeMenuItems = TestData.ListOfCommunes().communes.Where(n => n.city_id == Convert.ToInt32(Session["SelectedCity"])).ToList();

                        //Dictionary<int, string> territoryMenuItems = Session["TerritoryMenuItems"] as Dictionary<int, string>;
                        index = Session["CurrentTerritorySelection"] != null ? (int)Session["CurrentTerritorySelection"] : 0;
                        displayIndex = index >= 10 ? index + 2 : index + 1;
                        if (selection == "10" || index == 0)
                        {
                            if (selection == "0" && communeMenuItems.Count == 0)
                                return GetHomeScreen();

                            Session["PreviousTerritorySelection"] = index;

                            while (index >= 0 && (index < communeMenuItems.Count) && (length + communeMenuItems[index].commune_name.Length + backValue.Length) < 180)
                            {
                                if (index == 0)
                                {
                                    backValue = "\n10)more";
                                }
                                if (displayIndex == 10)
                                {
                                    displayIndex++;
                                }
                                returnValue += $"\n{displayIndex}){communeMenuItems[index].commune_name}";

                                index++;
                                displayIndex++;

                                length += returnValue.Length;
                                Session["CurrentTerritorySelection"] = index;
                                if (index >= communeMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }
                            }
                        }
                        else if (selection == "0")
                        {
                            index = Convert.ToInt32(Session["PreviousTerritorySelection"]);
                            displayIndex = index >= 10 ? index + 1 : index;
                            Session["CurrentTerritorySelection"] = index;
                            while (index > 0 && (index <= communeMenuItems.Count) && (length + communeMenuItems[index].commune_name.Length + backValue.Length) < 180)
                            {
                                if (index >= communeMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }

                                returnValue += $"\n{displayIndex  }){communeMenuItems[index].commune_name}";

                                index--;
                                displayIndex--;
                                if (displayIndex == 10)
                                {

                                    displayIndex--;
                                }
                                length += returnValue.Length;
                                Session["PreviousTerritorySelection"] = index;


                                if (index == 1)
                                {
                                    backValue = "\n10)more";
                                }
                            }
                        }
                        break;
                    #endregion
                    case CampaignMenu.MutuelSelection:
                        #region Mutuel

                        List<Mutuel> mutuelMenuItems = TestData.ListOfMutuels().mutuels.Where(n => n.commune_id == Convert.ToInt32(Session["SelectedCommune"])).ToList();

                        //Dictionary<int, string> territoryMenuItems = Session["TerritoryMenuItems"] as Dictionary<int, string>;
                        index = Session["CurrentMutuelSelection"] != null ? (int)Session["CurrentMutuelSelection"] : 0;
                        displayIndex = index >= 10 ? index + 2 : index + 1;
                        if (selection == "10" || index == 0)
                        {
                            if (selection == "0" && mutuelMenuItems.Count == 0)
                                return GetHomeScreen();

                            Session["PreviousMutuelSelection"] = index;

                            while (index >= 0 && (index < mutuelMenuItems.Count) && (length + mutuelMenuItems[index].mutuel_name.Length + backValue.Length) < 180)
                            {
                                if (index == 0)
                                {
                                    backValue = "\n10)more";
                                }
                                if (displayIndex == 10)
                                {
                                    displayIndex++;
                                }
                                returnValue += $"\n{displayIndex}){mutuelMenuItems[index].mutuel_name} - ${mutuelMenuItems[index].price}";

                                index++;
                                displayIndex++;

                                length += returnValue.Length;
                                Session["CurrentMutuelSelection"] = index;
                                if (index >= mutuelMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }
                            }
                        }
                        else if (selection == "0")
                        {
                            index = Convert.ToInt32(Session["PreviousMutuelSelection"]);
                            displayIndex = index >= 10 ? index + 1 : index;
                            Session["CurrentMutuelSelection"] = index;
                            while (index > 0 && (index <= mutuelMenuItems.Count) && (length + mutuelMenuItems[index].mutuel_name.Length + backValue.Length) < 180)
                            {
                                if (index >= mutuelMenuItems.Count)
                                {
                                    backValue = "\n0)back";
                                }

                                returnValue += $"\n{displayIndex  }){mutuelMenuItems[index].mutuel_name} - ${mutuelMenuItems[index].price}";

                                index--;
                                displayIndex--;
                                if (displayIndex == 10)
                                {

                                    displayIndex--;
                                }
                                length += returnValue.Length;
                                Session["PreviousMutuelSelection"] = index;


                                if (index == 1)
                                {
                                    backValue = "\n10)more";
                                }
                            }
                        }
                        break;
                        #endregion
                }
                return string.Format("{0}", returnValue + backValue);
            }
            catch
            {
                return "notfify# pas d'info\n\n#.pour return au menu principal";
            }
        }

        private string GetHomeScreen()
        {
            Session["CurrentProvinceSelection"] = null;
            Session["CurrentCitySelection"] = null;
            Session["CurrentTerritorySelection"] = null;
            Session["CurrentMutuelSelection"] = null;
            Session["SelectedProvince"] = null;
            Session["SelectedCity"] = null;
            Session["SelectedCommune"] = null;
            Session["SelectedMutuelle"] = null;

            MenuItem currentMenu = new MenuItem();
            var menu = GetMenuScreen(CampaignMenu.MutualSmart);
            subscription = new exactmobile.ussdcommon.Subscription();
            if (subscription.IsSubscribedToService(Session.MSISDN, 1))
            {
                var model = subscription.GetSubscription(Session.MSISDN, 1);
                if (model != null && model.status_id == 1)
                {
                    return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)Confirmer \n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
                }

                else
                {
                    return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)Services\n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
                }

            }
            else
            {
                return string.Format("request# Bienvenue a la Mutuelle Smart!\n\n1)souscrire\n2)Verifiez votre Balalance\n3)Se desabonner\n4)Info");
            }
        }
        public string GetMenuScreen(CampaignMenu campaignMenu)
        {
            MenuItem returnedMenu = new MenuItem();
            string menuData = Menu.ShowMenu(0, Convert.ToInt32(campaignMenu), ref returnedMenu);
            Session.LastMenuAccessed = returnedMenu;
            CurrentMenu = returnedMenu;
            return menuData;
        }

        protected Dictionary<int, string> DisplayListOfProvinces()
        {
            Dictionary<int, string> provinceMenuItems = null;
            if (Session["SubscriptionMenuItems"] != null)
            {


                provinceMenuItems = Session["ProvinceMenuItems"] as Dictionary<int, string>;

            }
            else
            {
                provinceMenuItems = new Dictionary<int, string>();

                provinceMenuItems.Add(1, "Bas - Uele");
                provinceMenuItems.Add(2, "Equateur");
                provinceMenuItems.Add(3, "Haut - Katanga");
                provinceMenuItems.Add(4, "Haut - Lomami");
                provinceMenuItems.Add(5, "Haut - Uele");
                provinceMenuItems.Add(6, "Ituri");
                provinceMenuItems.Add(7, "Kasai");
                provinceMenuItems.Add(8, "Kasai - Central");
                provinceMenuItems.Add(9, "Kasai - Oriental");
                provinceMenuItems.Add(10, "Kinshasa");
                provinceMenuItems.Add(11, "Kongo - Central");
                provinceMenuItems.Add(12, "Kwango");
                provinceMenuItems.Add(13, "Kwilu");
                provinceMenuItems.Add(14, "Lomami");
                provinceMenuItems.Add(15, "Lualaba");
                provinceMenuItems.Add(16, "Mai - Ndombe");
                provinceMenuItems.Add(17, "Maniema");
                provinceMenuItems.Add(18, "Mongala");
                provinceMenuItems.Add(19, "Nord - Kivu");
                provinceMenuItems.Add(20, "Nord - Ubangi");
                provinceMenuItems.Add(21, "Sankuru");
                provinceMenuItems.Add(22, "Sud - Kivu");
                provinceMenuItems.Add(23, "Sud - Ubangi");
                provinceMenuItems.Add(24, "Tanganyika");
                provinceMenuItems.Add(25, "Tshopo");
                provinceMenuItems.Add(26, "Tshuapa");



                Session["ProvinceMenuItems"] = provinceMenuItems;
            }


            return provinceMenuItems;
        }

        private string GoToMainMenu()
        {
            MenuItem currentMenu = new MenuItem();
            var data = Menu.ShowMenu((int)CampaignMenu.MutuelSelection, (int)CampaignMenu.MutuelSelection, ref currentMenu);
            Session.LastMenuAccessed = currentMenu;
            return data;
        }

        
    }
}
