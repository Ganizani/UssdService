using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using exactmobile.ussdservice.common.menu;
using Microsoft.Practices.EnterpriseLibrary.Data;
using CPaymentSystem;
using System.Globalization;

namespace exactmobile.ussdcommon
{
    public class Content
    {
            public const string CONST_SMSContentRequestQueue = "SMSContentRequest";
            public const string CONST_MobileGamesQueue = "MobileGameRequestQ";
            public const string CONST_SpecialContentQueue = "SpecialContentQ";
            public const string CONST_ContentQueue = "ContentQ";
            public const string CONST_SingleCodeContentQueue = "SingleCodeContentQ";

        public enum SpecialContentItemType : int
		{
			Unknown = 0,
			Content = 1,
			SingleCodeContent = 2,
			MobileGames = 3,
			InfoAlerts = 4,
			MMS = 5,
			PersonalisedColourBackgrounds = 6,
			MMSInfoAlerts = 7,
			PetFrog = 8,
			NameTones = 9		
        }
        
        public static SpecialContentItemType ReturnSpecialContentType(string code)
        {
            SpecialContentItemType retType;
            Database db = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = db.GetStoredProcCommand("exm_GetSpecialContentItem", new object[] { code });
            using (command)
            {
                IDataReader reader = db.ExecuteReader(command);
                using (reader)
                {
                    if (reader.Read())
                    {
                        int itemType = reader.GetInt32(2);
                        retType = (SpecialContentItemType)itemType;
                    }
                    else
                        retType = SpecialContentItemType.Content;
                }                
            }
            return retType;
        }
        public static int ReturnPaymentSysteContentTableID(SpecialContentItemType contentType)
        {
            CPaymentSystem.ContentTables contentTable = CPaymentSystem.ContentTables.None;
            contentTable = ReturnPaymentSystemContentTable(contentType);
            return Convert.ToInt32(contentTable);
        }
        public static CPaymentSystem.ContentTables ReturnPaymentSystemContentTable(SpecialContentItemType contentType)
        {
            CPaymentSystem.ContentTables contentTable = CPaymentSystem.ContentTables.None;
            int id = 0;
            switch (contentType)
            {
                case SpecialContentItemType.Content:                       
                    contentTable = CPaymentSystem.ContentTables.ContentCatalog;
                    break;
                case SpecialContentItemType.SingleCodeContent:                       
                    contentTable = CPaymentSystem.ContentTables.SingleCodeContent;
                    break;
                case SpecialContentItemType.MMS:
                    contentTable = CPaymentSystem.ContentTables.TExt2MMSContent;
                    break;
                case SpecialContentItemType.MobileGames:
                    contentTable = CPaymentSystem.ContentTables.MobileGames;
                    break;
                case SpecialContentItemType.PersonalisedColourBackgrounds:
                    contentTable = CPaymentSystem.ContentTables.PersonalizedBackground;
                    break;
                case SpecialContentItemType.InfoAlerts:
                    contentTable = CPaymentSystem.ContentTables.InfoAlerts;
                    break;
                default:
                    contentTable = CPaymentSystem.ContentTables.None;
                    break;
            }
            return contentTable;
        }

        public static int AddContentTransaction(string MSISDN,int catalogID,int inputMechanismID,int requestOwner)
        {
            int retVal = -1;
            Database db = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = db.GetStoredProcCommand("exm_InsertTRContentWAPOBSWAP", new object[] { System.DBNull.Value, MSISDN,catalogID,inputMechanismID,requestOwner });
            using (command)
            {
                object transactionID = db.ExecuteScalar(command);
                if (!int.TryParse(transactionID.ToString(), out retVal))
                    retVal = -1;
            }
            return retVal;
        }

        public static string ReturnServiceIndicatorMessage(string url)
        {
            string serviceIndicator = "0605040B8423F0660601AE02056A0045C60B03" + utility.CommonUtility.ToHex(url) + "00070103" + utility.CommonUtility.ToHex("Exw" + new GregorianCalendar().GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString().PadLeft(2, '0')) + "000101";
            return serviceIndicator;            
        }

        public static bool AddGlobalTransaction(int specialContentKeywordSuffixID, int singleCodeContentID, int contentCatalogID, int requestOwnerTerminationNumberID, int requestInputMechanismID, int mobileNetworkID, int requestOwnerID, int globalTransactionTypeID, int statusID, int downloaded, decimal sellingPrice, out int globalTransactionID)
        {
            globalTransactionID = -1;
            Database db = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand command = db.GetSqlStringCommand(string.Format("exm_AddGlobalTransaction {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", specialContentKeywordSuffixID == -1 ? "null" : specialContentKeywordSuffixID.ToString(), singleCodeContentID == -1 ? "null" : singleCodeContentID.ToString(), contentCatalogID == -1 ? "null" : contentCatalogID.ToString(), requestOwnerTerminationNumberID == -1 ? "null" : requestOwnerTerminationNumberID.ToString(), requestInputMechanismID, mobileNetworkID, requestOwnerID, globalTransactionTypeID, statusID, downloaded.ToString(), sellingPrice));
            object scalarReturn;
            using (command)
                scalarReturn = db.ExecuteScalar(command);
            if (!int.TryParse(Convert.ToString(scalarReturn),out globalTransactionID))
                globalTransactionID = -1;

            return globalTransactionID >= 1;

        }

        public static bool AddR100FreeFullTrackTransaction(int subscriptionServiceIDRef, int contentCatalogIDRef, string mobileNumber)
        {            
            Database db = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            DbCommand cmd = db.GetStoredProcCommand("exm_InsertFreeContentItemDownload");
         
            var param_subscriptionServiceIDRef = cmd.CreateParameter();
            var param_contentCatalogIDRef= cmd.CreateParameter();
            var param_mobileNumber = cmd.CreateParameter();

            param_subscriptionServiceIDRef.ParameterName = "@SubscriptionServiceIDRef";
            param_subscriptionServiceIDRef.DbType = DbType.Int32;
            param_subscriptionServiceIDRef.Value = subscriptionServiceIDRef;
            
            param_contentCatalogIDRef.ParameterName = "@ContentCatalogIDRef";
            param_contentCatalogIDRef.DbType = DbType.Int32;
            param_contentCatalogIDRef.Value = contentCatalogIDRef;

            param_mobileNumber.ParameterName = "@mobileNumber";
            param_mobileNumber.DbType = DbType.String;
            param_mobileNumber.Value = mobileNumber;

            cmd.Parameters.AddRange(new DbParameter[] { param_subscriptionServiceIDRef, param_contentCatalogIDRef, param_mobileNumber });
            db.ExecuteNonQuery(cmd);
            return true;
        }
    }
}
