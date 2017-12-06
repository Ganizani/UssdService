using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.components;
using exactmobile.ussdservice.common.menu;
using System.Web;
using System.Net;
using System.IO;
using System.Xml;
using USSD.Entities;
using System.Net.Http;

namespace exactmobile.ussdcommon
{
    sealed class AsyncHTTPRequestStateObject
    {
        public WebRequest Request { get; set; }
        public long TransactionID { get; set; }
        public int USSDCampaignID { get; set; }
        public int MenuID { get; set; }
        public long USSDTransactionID { get; set; }
        MenuItem.CommunicationType CommunicationType { get; set; }
        public int MobileNetworkID { get; set; }
        public string MobileNumber { get; set; }
        public string USSDNumber { get; set; }
        public bool SendHTTPResponseBySMS { get; set; }
    }

    public static class MenuCommunication
    {        
        public static bool SendCommunication(int ussdCampaignID, int menuID,long ussdTransactionID,MenuItem.CommunicationType communicationType,int mobileNetworkID,string mobileNumber,string communicationText,string ussdNumber,bool waitForResponse,bool sendHTTPResponseBySMS, out string response)
        {
            bool hasSent = false;
            try
            {
                response = null;
                switch (communicationType)
                {
                    case MenuItem.CommunicationType.SM2HTTP:
                        long transactionID=-1;
                        try
                        {                            
                            if (AddHTTPTransaction(ussdCampaignID, ussdTransactionID, menuID, mobileNumber, null, out transactionID))
                            {
                                StringBuilder url = new StringBuilder(communicationText);
                                url.AppendFormat("&MSISDN={0}&DESTADDRESS={1}&REFERENCE={2}", HttpUtility.UrlEncode(mobileNumber), HttpUtility.UrlEncode(ussdNumber), HttpUtility.UrlEncode(transactionID.ToString()));

                                UpdateHTTPTransaction(transactionID, url.ToString(), null, null, null, null);
                                WebResponse webResponse = null;
                                string webResponseString = string.Empty;
                                if (!waitForResponse)
                                {
                                    var webRequest = HttpWebRequest.Create(url.ToString());
                                    webRequest.Method = "GET";

                                    var asyncStateObject = new AsyncHTTPRequestStateObject(){ Request = webRequest
                                        , TransactionID = transactionID
                                        , MenuID = menuID
                                        , MobileNetworkID =mobileNetworkID
                                        , MobileNumber = mobileNumber
                                        , USSDCampaignID = ussdCampaignID
                                        , USSDNumber = ussdNumber
                                        , USSDTransactionID = ussdTransactionID
                                        , SendHTTPResponseBySMS = sendHTTPResponseBySMS};


                                    IAsyncResult asyncResult = webRequest.BeginGetResponse(new AsyncCallback( PerformAsynchronousHTTPRequestCallback), asyncStateObject);

                                    System.Threading.ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle,new System.Threading.WaitOrTimerCallback(AsynchronousHTTPRequestTimeoutCallback),asyncStateObject,asyncStateObject.Request.Timeout,true);                                    
                                    return true;
                                }
                                else
                                {
                                    try
                                    {                                        
                                        var webRequest = HttpWebRequest.Create(url.ToString()); 
                                        webRequest.Method = "GET";
                                        webResponse = webRequest.GetResponse();

                                        using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                                            webResponseString = streamReader.ReadToEnd();

                                        UpdateHTTPTransaction(transactionID, null, null, null, webResponseString, null);
                                    }
                                    finally
                                    {
                                        if (webResponse != null)
                                            webResponse.Close();
                                    }
                                    string clientMessage = string.Empty;
                                    string clientReference = string.Empty;
                                    
                                    XmlDocument responseXMLDoc = new XmlDocument();
                                    responseXMLDoc.LoadXml(webResponseString);                               
                                    
                                    if (TryGetXMLElement(responseXMLDoc, new string[] { "SM", "Message" }, out clientMessage))
                                    {
                                        response = clientMessage;
                                        UpdateHTTPTransaction(transactionID, null, null, clientMessage, null, null);
                                    }
                                    if(TryGetXMLElement(responseXMLDoc, new string[] { "SessionReference","RefNo" }, out clientReference))
                                        UpdateHTTPTransaction(transactionID, null, clientReference, null, null, null);
                                    
                                    if (sendHTTPResponseBySMS && !String.IsNullOrEmpty(clientMessage))
                                    {
                                        MenuCommunication.SendCommunication(ussdCampaignID, menuID, ussdTransactionID, MenuItem.CommunicationType.SMS, mobileNetworkID, mobileNumber, clientMessage, ussdNumber, false, false, out response);
                                        response = null;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (transactionID > 0)
                                UpdateHTTPTransaction(transactionID, null, null, null, null, ex.ToString());                  
                        }
                        break;
                    case MenuItem.CommunicationType.SMS:
                    case MenuItem.CommunicationType.MMS:
                        CQueueWrapper commsSendQueue= null;

                        if(communicationType == MenuItem.CommunicationType.MMS)
                            commsSendQueue = new CQueueWrapper("EXACTUSSDMMSCOMMUNICATIONQ");
                        else if(communicationType == MenuItem.CommunicationType.SMS)
                            commsSendQueue = new CQueueWrapper("EXACTUSSDSMSCOMMUNICATIONQ");

                        CXMLCapsule commsCapsule = new CXMLCapsule();
                        commsCapsule.SetParameter("USSDTransactionID", ussdTransactionID.ToString(), true);
                        commsCapsule.SetParameter("USSDCampaignID", ussdCampaignID.ToString(), true);
                        commsCapsule.SetParameter("USSDMenuID", menuID.ToString(), true);
                        commsCapsule.SetParameter("MobileNumber", mobileNumber, true);
                        commsCapsule.SetParameter("MobileNetworkID", mobileNetworkID.ToString(),true);
                        commsCapsule.SetParameter("CommunicationText", communicationText, true);
                        commsCapsule.SetParameter("CommunicationTypeID", Convert.ToInt32(communicationType).ToString(), true);
                        commsCapsule.SetParameter("USSDNumber", ussdNumber, true);
                        if (commsSendQueue != null)
                        {
                            commsSendQueue.Send(commsCapsule);
                            hasSent = true;
                        }
                        break;
                }
            }
            finally
            {
            }
            return hasSent;
        }

        static void PerformAsynchronousHTTPRequestCallback(IAsyncResult asyncResults)
        {
            string webResponseString;
            var asyncStateObject = (asyncResults.AsyncState as AsyncHTTPRequestStateObject);
            try
            {
                if (asyncStateObject != null)
                {
                    var webResponse = asyncStateObject.Request.GetResponse();

                    using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                        webResponseString = streamReader.ReadToEnd();
                    
                    webResponse.Close();

                    UpdateHTTPTransaction(asyncStateObject.TransactionID, null, null, null, webResponseString, null);

                    string clientMessage = string.Empty;
                    string clientReference = string.Empty;

                    XmlDocument responseXMLDoc = new XmlDocument();
                    responseXMLDoc.LoadXml(webResponseString);

                    if (TryGetXMLElement(responseXMLDoc, new string[] { "SM", "Message" }, out clientMessage))
                        UpdateHTTPTransaction(asyncStateObject.TransactionID, null, null, clientMessage, null, null);

                    if (TryGetXMLElement(responseXMLDoc, new string[] { "SessionReference", "RefNo" }, out clientReference))
                        UpdateHTTPTransaction(asyncStateObject.TransactionID, null, clientReference, null, null, null);

                    string response;
                    if (asyncStateObject.SendHTTPResponseBySMS && !String.IsNullOrEmpty(clientMessage))
                        MenuCommunication.SendCommunication(asyncStateObject.USSDCampaignID, asyncStateObject.MenuID, asyncStateObject.USSDTransactionID, MenuItem.CommunicationType.SMS, asyncStateObject.MobileNetworkID, asyncStateObject.MobileNumber, clientMessage, asyncStateObject.USSDNumber, false, false, out response);

                }
            }
            catch(Exception ex)
            {
                if(asyncStateObject != null && asyncStateObject.TransactionID>0)
                    UpdateHTTPTransaction(asyncStateObject.TransactionID, null, null, null, null, ex.ToString()); 
            }
        }

        static void AsynchronousHTTPRequestTimeoutCallback(object httpRequestStateObject, bool timedOut)
        {
            if (timedOut)
            {
                if (httpRequestStateObject is AsyncHTTPRequestStateObject)
                {
                    var requestStateObject = (httpRequestStateObject as AsyncHTTPRequestStateObject);
                    if (requestStateObject != null)
                    {
                        if (requestStateObject.TransactionID > 0)
                            UpdateHTTPTransaction(requestStateObject.TransactionID, null, null, null, null, "Asyncronous request timed out.");

                        if (requestStateObject.Request != null)
                            requestStateObject.Request.Abort();
                    }
                }
            }
        }


        static bool AddHTTPTransaction(int ussdCampaignID,long ussdTransactionID,int menuID,string mobileNumber,string error,out long transactionID)
        {
           
            transactionID = -1;
            HTTPTransaction model = new HTTPTransaction
            {
                USSDTransactionId = ussdTransactionID,
                USSDCampaignId = ussdCampaignID,
                MenuId = menuID,
                MobileNumber = mobileNumber,
                Errors = error
            };
            //using (var command = DB.ExactUSSD.GetStoredProcCommand("usp_InsertHTTPTransaction"))
            //{
            //    DB.ExactUSSD.AddInParameter(command, "@USSDCampaignIDRef", System.Data.DbType.Int32, ussdCampaignID);
            //    DB.ExactUSSD.AddInParameter(command, "@USSDTransactionIDRef", System.Data.DbType.Int64, ussdTransactionID);
            //    DB.ExactUSSD.AddInParameter(command, "@MenuIDRef", System.Data.DbType.Int32, menuID);
            //    DB.ExactUSSD.AddInParameter(command, "@MobileNumber", System.Data.DbType.String, mobileNumber);

            //    if(!String.IsNullOrEmpty(error))
            //        DB.ExactUSSD.AddInParameter(command, "@Error", System.Data.DbType.String, error);
            //    using (var reader = DB.ExactUSSD.ExecuteReader(command))
            //    {
            //        if (reader.Read())
            //            hasAdded = ((transactionID = Convert.ToInt32(reader["transactionID"])) > 0);
            //    }
            //}
            return transactionID >0 ? true :false;
        }

        static bool UpdateHTTPTransaction(long transactionID,string resolveURL,string clientReference,string clientMessage,string clientWebResponse, string error)
        {
            bool hasUpdated;
            using (var command = DB.ExactUSSD.GetStoredProcCommand("usp_UpdateHTTPTransaction"))
            {
                DB.ExactUSSD.AddInParameter(command, "@transactionID", System.Data.DbType.Int64, transactionID);

                if(!String.IsNullOrEmpty(resolveURL))
                    DB.ExactUSSD.AddInParameter(command, "@ResolvedURL", System.Data.DbType.String, resolveURL);

                if(!String.IsNullOrEmpty(clientReference))
                    DB.ExactUSSD.AddInParameter(command, "@ClientReference", System.Data.DbType.String, clientReference);

                if(!String.IsNullOrEmpty(clientMessage))
                    DB.ExactUSSD.AddInParameter(command, "@ClientMessage", System.Data.DbType.String, clientMessage);

                if(!String.IsNullOrEmpty(clientWebResponse))
                    DB.ExactUSSD.AddInParameter(command, "@ClientWebResponse", System.Data.DbType.String, clientWebResponse);

                if(!string.IsNullOrEmpty(error))
                    DB.ExactUSSD.AddInParameter(command, "@Error", System.Data.DbType.String, error);
                
                hasUpdated = (DB.ExactUSSD.ExecuteNonQuery(command) > 0);

                return hasUpdated;
            }

        }

        static bool TryGetXMLElement(XmlDocument document, string[] tagNames, out string elementValue)
        {
            elementValue = null;
            foreach (string name in tagNames)
            {
                try
                {
                    if (!string.IsNullOrEmpty(document.GetElementsByTagName(name)[0].InnerText))
                    {
                        elementValue = document.GetElementsByTagName(name)[0].InnerText;
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return (!String.IsNullOrEmpty(elementValue));
        }

  
    }
}
