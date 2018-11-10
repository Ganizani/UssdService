using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using exactmobile.components;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace exactmobile.ussdcommon
{
    public class Subscription
    {
        public enum BillingFrequency:int
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3
        }

        public Tuple<string, int> Subscribe(string mobileNumber, int subscriptionServiceId, int? mutuelleId)
        {
            bool requestSumbitted;
            return Subscribe(mobileNumber, subscriptionServiceId, true, out requestSumbitted, mutuelleId);
        }

        public Tuple<string,int> Subscribe(string mobileNumber, int subscriptionServiceId, bool checkAlreadySubscribed, out bool requestSubmitted, int? mutuelleId)
        {
            string data = null;
            requestSubmitted = false;
            if (checkAlreadySubscribed)
            {
                if (IsSubscribedToService(mobileNumber, subscriptionServiceId))
                    return new Tuple<string, int>( "Dear user, you are already subscribed to this service.",-1);
            }

            if (String.IsNullOrEmpty(data))
            {
                USSD.Entities.SubscriptionService service = GetSubscriptionService( subscriptionServiceId);

                USSD.Entities.Subscription model = new USSD.Entities.Subscription
                {
                    //createdDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    first_billing_date = DateTime.Now.ToString("yyyy-MM-dd"),
                    next_billing_date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    mobile_number = mobileNumber,
                    status_id = 1,//(int)Statuses.Pending,
                    subscription_service_id = service.subscription_service_id == 0 ? 1 : service.subscription_service_id,
                    mutuel_id = 1,//mutuelleId??0,
                    network_id = 1,
                    ref_number = mobileNumber,



                };
                var originalSubscription = GetSubscription(mobileNumber, subscriptionServiceId);
                if (service != null && service.subscription_service_id > 0 && !string.IsNullOrEmpty(service.keyword) && originalSubscription==null)
                {
                    var client = ussdcommon.utility.CommonUtility.GetHttpConnection("MutuelleSmartAPI");
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var URL = new Uri(client.BaseAddress, "subscriptions");

                    using (client)
                    {
                        try
                        {
                            var result = client.PostAsync(URL, content).Result;
                            if (result.StatusCode == HttpStatusCode.OK)
                            {
                                var tempResult = result.Content.ReadAsStringAsync().Result;
                                try
                                {
                                    var entity = JsonConvert.DeserializeObject<USSD.Entities.SubscriptionResult>(tempResult);
                                    //update oneview complaint status
                                    if (entity != null && entity.status != 3)
                                    {
                                        return new Tuple<string, int>(entity.message, entity.createdSubscriptionId);


                                    }
                                }
                                catch
                                {

                                }

                            }

                        }
                        catch (Exception exp)
                        {
                        }
                    }

                    if (String.IsNullOrEmpty(service.display_name))
                        data = string.Format("Thank you, you will now be subscribed to {0}. A confirmation SMS will be sent to you when successful.", service.display_name);
                    else
                        data = string.Format("Thank you, you will now be subscribed to {0}. A confirmation SMS will be sent to you when successful.", service.display_name);

                    requestSubmitted = true;
                }
                else if (originalSubscription!= null)
                {
                    UpdateSubscription(originalSubscription, subscriptionServiceId);
                    return  new Tuple<string, int>("Re-Subscribed successfully",originalSubscription.subscription_id);
                }
                else
                {
                    data = "Service does not exist or subscribe keyword not setup.";
                }
            }

            return new Tuple<string, int>( data,-1);
        }

        public bool IsSubscribedToService(string mobileNumber, int subscriptionServiceId)
        {
            bool subscribed = false;
            USSD.Entities.Subscription model = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["MutuelleSmartAPI"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Configuration.ConfigurationManager.AppSettings["Content-Type"].ToString()));


            using (client)
            {


                var URL = new Uri(client.BaseAddress, "subscriptions/services/" +  subscriptionServiceId + "/cellnumber/" + mobileNumber);
                var response = client.GetAsync(URL.ToString()).Result;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        model = JsonConvert.DeserializeObject<USSD.Entities.Subscriptions>(result).subscription;
                        //update oneview complaint status
                        if (model != null && model.status_id != 3)
                        {
                            subscribed = true;
                        }
                    }
                    catch
                    {

                    }
                    

                }



            }

            return subscribed;

        }


        //private USSD.Entities.SubscriptionService GetSubscriptionService(int subscriptionServiceId)
        //{
        //    USSD.Entities.SubscriptionService model = null;
        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["MutuelleSmartAPI"].ToString());
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Configuration.ConfigurationManager.AppSettings["Content-Type"].ToString()));


        //    using (client)
        //    {


        //        var URL = new Uri(client.BaseAddress + "/subscriptions/services/" + subscriptionServiceId);
        //        var response = client.GetAsync(URL.ToString()).Result;


        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var result = response.Content.ReadAsStringAsync().Result;
        //            try
        //            {
        //                model = JsonConvert.DeserializeObject<USSD.Entities.SubscriptionServices>(result).subscriptionService;
        //                //update oneview complaint status

        //            }
        //            catch { }

        //        }
                


        //    }
        //    return model;
        //}

        public USSD.Entities.Subscription GetSubscription(string mobileNumber, int subscriptionServiceId)
        {
            var model = new USSD.Entities.Subscription();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["MutuelleSmartAPI"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Configuration.ConfigurationManager.AppSettings["Content-Type"].ToString()));


            using (client)
            {


                var URL = new Uri(client.BaseAddress, "subscriptions/services/" + subscriptionServiceId + "/cellnumber/" + mobileNumber);
                var response = client.GetAsync(URL.ToString()).Result;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        model = JsonConvert.DeserializeObject<USSD.Entities.Subscriptions>(result).subscription;
                        //update oneview complaint status

                    }
                    catch
                    {

                    }


                }



            }
            return model;
        }
        private USSD.Entities.SubscriptionService GetSubscriptionService(int subscriptionServiceId)
        {
            USSD.Entities.SubscriptionService model = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["MutuelleSmartAPI"].ToString());
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(System.Configuration.ConfigurationManager.AppSettings["Content-Type"].ToString()));


            using (client)
            {


                var URL = new Uri(client.BaseAddress + "subscriptionservices/" + subscriptionServiceId);
                var response = client.GetAsync(URL.ToString()).Result;


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var models = JsonConvert.DeserializeObject<USSD.Entities.SubscriptionServices>(result);
                        if (models != null)
                            model = models.subscriptionService;
                    }
                    catch { }

                }



            }
            return model;
        }


        public void UpdateSubscription(USSD.Entities.Subscription model, int statusId)
        {
            model.status_id = statusId;
            var client = ussdcommon.utility.CommonUtility.GetHttpConnection("MutuelleSmartAPI");
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var URL = new Uri(client.BaseAddress, "subscriptions");

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


        public bool SendUnsubscribeRequest(string mobileNumber, int subscriptionServiceID, out string returnMessage)
        {
            bool hasSent = false;
            SubscriptionServiceInfo subscriptionService = new SubscriptionServiceInfo(subscriptionServiceID);
            returnMessage = string.Empty;
            if (subscriptionService != null && subscriptionService.Id > 0 && !string.IsNullOrEmpty(subscriptionService.UnsubscribeKeyword))
            {
                CQueueWrapper cq = new CQueueWrapper(subscriptionService.RequestQueue);
                CXMLCapsule xmlcap = new CXMLCapsule();

                xmlcap.SetParameter("INPUTMECHANISM", "20", true);//USSD input mechanism.
                xmlcap.SetParameter("SOURCEADDRESS", mobileNumber, true);
                xmlcap.SetParameter("SHORTMESSAGE", subscriptionService.UnsubscribeKeyword, true);
                cq.Send(xmlcap);
                returnMessage = String.Format("An unsubscribe request has been sent for {0}. A confirmation SMS will be sent to you when successful", subscriptionService.Name);
                hasSent = true;
            }
            else
            {
                if (subscriptionService == null || subscriptionService.Id < 0)
                    returnMessage = "Subscription service does not exist.";
                else if (String.IsNullOrEmpty(subscriptionService.UnsubscribeKeyword))
                    returnMessage = "Unsubscribe keyword is not set up";
            }

            return hasSent;
        }
        /// <summary>
        /// Queries the database to check if a user is a member of a service by checking the subscription expiryDate
        /// against the getdate()
        /// </summary>
        /// <param name="mobileNumber">user's mobile number</param>
        /// <param name="subscriptionServiceID">subscription service in to check against</param>
        /// <returns></returns>
        public bool IsUserMemberOfSubscriptionService(string mobileNumber, int subscriptionServiceID)
        {
            bool isMember = false;
            Database database = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = database.GetStoredProcCommand("sp_IsUserMemberOfSubscriptionService", new object[] { mobileNumber, subscriptionServiceID });
            using(command)
                isMember = Convert.ToBoolean(database.ExecuteScalar(command));           
            return isMember;           
        }
        public bool IsUserMemberOfSubscriptionService(string mobileNumber, int subscriptionServiceID, out int subscriptionID)
        {
            bool isMember = false;
            subscriptionID = -1;
            Database database = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = database.GetStoredProcCommand("sp_IsUserMemberOfSubscriptionService", new object[] { mobileNumber, subscriptionServiceID });
            using (command)
            {
                using (var reader = database.ExecuteReader(command))
                {
                    if (reader.Read())
                    {
                        isMember = Convert.ToBoolean(reader["IsMember"]);
                        subscriptionID = Convert.ToInt32(reader["SubscriptionID"]);
                    }
                }                
            }
            return isMember;
        }
        
        public bool GetDoubleOptActiveStatus(int subscriptionServiceID, int mobileNetworkID)
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
                        retVal = Convert.ToBoolean(reader["USSDActive"]);
                }
            }
            return retVal;
        }
    }
    public class SubscriptionServiceInfo
    {
        private int id;
        private string name;
        private string unsubscribeKeyword;
        private string subscribeKeyword;
        private decimal amount;
        private string requestQueue;
        string billingFrequency;
        string displayName;
        public string UssdNumber { get; set; }

        #region Properties
        public int Id
        {
            get { return this.id; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string BillingFrequency
        {
            get { return this.billingFrequency; }
        }

        public string UnsubscribeKeyword
        {
            get { return this.unsubscribeKeyword; }
        }

        public string SubscribeKeyword
        {
            get { return this.subscribeKeyword; }
        }

        public decimal Amount
        {
            get { return this.amount; }
        }

        public string RequestQueue
        {
            get { return this.requestQueue; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }
        #endregion

        #region Constructors
        public SubscriptionServiceInfo()
        {
        }
        
        public SubscriptionServiceInfo(int subscriptionServiceId)
        {

            using (DbCommand command = DB.ExactDotNet.GetStoredProcCommand("SUB_GetActiveSubscriptionServiceByID"))
            {
                //("select subscriptionServiceId,	[description],	billingFrequencyIDref,	unsubKeyword,	billingAmount,	requestQueue, subscribeKeyword	from exactDB1.exactdotnet.dbo.sub_subscriptionServices where subscriptionServiceId = @subscriptionServiceId");

                DB.ExactDotNet.AddInParameter(command, "@SubscriptionServiceID", DbType.Int32, subscriptionServiceId);


                using (IDataReader reader = DB.ExactDotNet.ExecuteReader(command))
                {
                    if (reader.Read())
                    {
                        this.id = Convert.ToInt32(reader["SubscriptionServiceID"]);
                        this.name = Convert.ToString(reader["Description"]);
                        this.billingFrequency = Convert.ToString(reader["BillingFrequency"]);
                        this.unsubscribeKeyword = Convert.ToString(reader["UnsubKeyword"]);
                        this.amount = Convert.ToDecimal(reader["billingAmount"]);
                        this.requestQueue = ((reader["RequestQueue"] == DBNull.Value) ? null : Convert.ToString(reader["RequestQueue"]));
                        this.subscribeKeyword = ((reader["subscribeKeyword"] == DBNull.Value) ? null : Convert.ToString(reader["subscribeKeyword"]));
                        this.displayName = ((reader["DisplayName"] == DBNull.Value ? null : Convert.ToString(reader["DisplayName"])));
                        this.UssdNumber = ((reader["UssdNumber"] == DBNull.Value ? null : Convert.ToString(reader["UssdNumber"])));
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public List<SubscriptionServiceInfo> ReturnDoubleOptSubscriptionServices()
        {
            var retVal = new List<SubscriptionServiceInfo>();
            using (DbCommand command = DB.ExactDotNet.GetStoredProcCommand("dbo.Get_DoubleOptSubscriptionServices"))
            {
                using (IDataReader reader = DB.ExactDotNet.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        retVal.Add(new SubscriptionServiceInfo() { id = Convert.ToInt32(reader["SubscriptionServiceID"]), name = Convert.ToString(reader["Description"]), billingFrequency = Convert.ToString(reader["BillingFrequency"]), unsubscribeKeyword = Convert.ToString(reader["UnsubKeyword"]), amount = Convert.ToDecimal(reader["billingAmount"]), requestQueue = ((reader["RequestQueue"] == DBNull.Value) ? null : Convert.ToString(reader["RequestQueue"])), subscribeKeyword = ((reader["subscribeKeyword"] == DBNull.Value) ? null : Convert.ToString(reader["subscribeKeyword"])) });
                    }
                }
            }
            return retVal;
        }
        #endregion
    }


}
