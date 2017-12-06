using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Collections.Generic;

namespace exactmobile.ussdservice.common.campaign
{
    public class USSDCampaign
    {
        public readonly int CampaignID;
        public readonly String ProcessorType;
        public readonly String BackButton;
        public readonly int USSDNumberID;
        
        public USSDCampaign(int campaignID, String processorType)
        {
            this.CampaignID = campaignID;
            this.ProcessorType = processorType;
            this.BackButton = "0";//default back button
            this.StartDateTime = DateTime.MaxValue;
            this.EndDateTime = DateTime.MinValue;
            this.MustMaintainSession = false;
        }

        public USSDCampaign(int campaignID, String processorType, String backButton, DateTime startDateTime, DateTime endDateTime,int ussdNumberID)
        {
            this.CampaignID = campaignID;
            this.ProcessorType = processorType;
            this.BackButton = backButton;
            this.StartDateTime = startDateTime;
            this.EndDateTime = endDateTime;
            this.USSDNumberID = ussdNumberID;
            this.MustMaintainSession = false;
        }
        
        public DateTime StartDateTime { get ; set; }
        public DateTime EndDateTime { get; set; }
        public Boolean MustMaintainSession { get; set; }
    }
}
