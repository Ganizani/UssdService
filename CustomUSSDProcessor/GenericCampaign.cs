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
using exactmobile.components;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;


namespace exactmobile.ussdservice.processors.CustomUSSD
{
    public class GenericCampaign : BaseUSSDProcessor, IUSSDProcessor
    {
        public GenericCampaign(USSDSession session, IUSSDHandler handler)
            : base(session, handler)
        {
        }

        public override string OnConnect()
        {           
            Session["MenuSection"] = "-1";
            return null;
        }

        public MenuItem CurrentMenu
        {
            get;
            set;
        }

      

        public override string ManualOverRide()
        {
            string data = null;
            var menu = new MenuItem();

            
            if (Helper.Message != "0")
            {
                CurrentMenu = Menu.FindMenuItemByReturnValueNEW(Session.LastMenuAccessed, Helper.Message);
                
               
                if (CurrentMenu != null)
                {
                    Session["EnteredValue"] = Helper.Message;
                    try
                    {
                        data = Menu.ShowMenu(CurrentMenu.ParentMenuId, CurrentMenu.ParentMenuId, ref menu, true,true, true, Helper.Message);

                            if (Session["data"] == null)
                                Session["data"] = "";

                            if (data == Session["data"].ToString()) //check repeated menu
                            {
                                data = Menu.ShowMenu(CurrentMenu.MenuId, CurrentMenu.MenuId, ref menu);                              
                            }
                           
                    }
                    catch
                    {
                        data = Menu.ShowMenu(CurrentMenu.MenuId, CurrentMenu.MenuId, ref menu);
                    }

                    
                }
                else
                {
                    
                    data = Menu.ShowMenu(Session.LastMenuAccessed.MenuId, Session.LastMenuAccessed.MenuId, ref menu);  
                    
                    if (menu.RangeText.Length != 0 && menu.ReturnValue.Length != 0) // Check if a last menu
                        data = Menu.ShowMenu(Session.LastMenuAccessed.ParentMenuId, Session.LastMenuAccessed.ParentMenuId, ref menu);  

                }

            }

            
            CurrentMenu = menu;
            Session.LastMenuAccessed = CurrentMenu;
            return data;

        }


        public static void sendSMS(string Msisdn, string message)
        {
            CQueueWrapper smsSender = new CQueueWrapper("CUSTOM_CAMP_BUILDEROUTQ");
            CSMSCapsule sms = new CSMSCapsule(Msisdn, "", false, CSMSCapsule.MessageTypes.DEFAULT, 1, 5, 7, 5, CUtility.GetNetworkID(Msisdn), false, CUtility.ToHex(message), false);

            smsSender.Send(sms);
            smsSender = null;
            sms = null;
        }


        private void SendEMail(string recipient, string subject, string body, string from)
        {
            try
            {
                //System.Net.Mail.SmtpClient smpt = new System.Net.Mail.SmtpClient("192.168.1.110", 25);
                //smpt.Send(from, recipient, subject, body);

                System.Web.Mail.SmtpMail.SmtpServer = "192.168.45.152";
                System.Web.Mail.SmtpMail.Send(from, recipient, subject,body);

            }
            catch
            { }
        }



    }
}
