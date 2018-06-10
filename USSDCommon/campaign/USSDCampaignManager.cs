using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Collections.Generic;
using System.Data;
using exactmobile.ussdservice.common.exceptions;
using USSD.BLL;
using System.Linq;
namespace exactmobile.ussdservice.common.campaign
{
    public class USSDCampaignManager
    {
        #region Private Vars
        private static Dictionary<int, USSDCampaign> cacheList = new Dictionary<int, USSDCampaign>();
        #endregion

        public static USSDCampaign GetCampaignID(int ussdNumber)
        {
            if (cacheList.ContainsKey(ussdNumber))
                return cacheList[ussdNumber];

            //Database ussdDatabase = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            ///*Original code replaced with code on the line below.
            // * Received a "The number of parameters does not match number of values for stored procedure" error from the DEV database.
            // * IDataReader reader = ussdDatabase.ExecuteReader("SP_GetUSSDCampaignForNumber", new Object[] { ussdNumber });*/
            //using (IDataReader reader = ussdDatabase.ExecuteReader(CommandType.Text, String.Format("exec @FilterUSSDNumber  = {0}", ussdNumber)))            
            //{
            //    if (reader.Read())
            //    {
            //        USSDCampaign campaign = new USSDCampaign(Convert.ToInt32(reader["Id"])
            //            , Convert.ToString(reader["ProcessorType"]), Convert.ToString(reader["BackButton"])
            //            , Convert.ToDateTime(reader["CampaignStartDate"]), Convert.ToDateTime(reader["CampaignEndDate"]), Convert.ToInt32(reader["USSDNumberId"]));

            //        cacheList.Add(ussdNumber, campaign);
            //        return campaign;
            //    }
            //    else
            //        //throw new USSDUnknownCampaignForNumberException("Unknown campaign or no campaign found for number {0}", ussdNumber);
            //        return null;

            //}
            //var models = Logic.Instance.ClaimStatuses.ListFiltered(new USSD.Entities.ClaimStatus { });
            try
            {
                USSD.Entities.USSDCampaign model = Logic.Instance.Campaigns.ListFiltered(new USSD.Entities.USSDCampaign { FilterUSSDNumber = ussdNumber.ToString() }).FirstOrDefault();
                USSDCampaign campaign = new USSDCampaign(model.Id, model.ProcessorType, model.BackButton, model.CampaignStartDate ?? DateTime.MinValue, model.CampaignEndDate ?? DateTime.MaxValue, model.USSDNumberId);
                cacheList.Add(ussdNumber, campaign);
                return campaign;
            }
            catch(Exception exp) {
                exactmobile.components.logging.LogManager.LogStatus("Error getting the campaign for ussdNumber {1} \n\n Error Messge : {0}", exp.Message , ussdNumber ) ;
                throw new Exception("notify# le shortcode est incorrect! ");
                
            }
        }

        public static bool RefreshCampaignProperties(ref USSDCampaign campaign)
        {
            bool hasRefreshed = false;

            if (campaign != null)
            {
                Database ussdDatabase = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
                var cmd = ussdDatabase.GetStoredProcCommand("FetchUSSDCampaign");
                var param_campaignID = cmd.CreateParameter();
                param_campaignID.ParameterName = "@Id";
                param_campaignID.Value = campaign.CampaignID;
                param_campaignID.DbType = DbType.Int32;
                cmd.Parameters.Add(param_campaignID);
                using(cmd)
                {
                    using (var reader = ussdDatabase.ExecuteReader(cmd))
                    {
                        if (reader.Read())
                        {
                            campaign.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                            campaign.EndDateTime = Convert.ToDateTime(reader["EndDateTime"]);
                            hasRefreshed = true;
                        }
                    }
                }
            }
            return hasRefreshed;
        }
    }
}
