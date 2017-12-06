using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using exactmobile.components;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace exactmobile.ussdcommon.utility
{
    public class CommonUtility
    {
        public static int ReturnNetworkID(string MSISDN)
        {
            int networkID = 0;
            DbCommand command = DB.ExactDotNet.GetStoredProcCommand("exm_GetMobileNetworkIDByMobileNumber", new object[] { MSISDN });
            using (command)
                networkID = Convert.ToInt32(DB.ExactDotNet.ExecuteScalar(command));
            return networkID;
        }

        public static string ToHex(string Value)
        {
            char[] TempChars = Value.ToCharArray();
            StringBuilder retHexVal = new StringBuilder();

            for (int i = 0; i < Value.Length; i++)
                retHexVal.Append(System.Convert.ToString(((int)TempChars[i]), 16).PadLeft(2, '0'));

            return retHexVal.ToString();
        }

        public static string GeneratePassword()
        {
            StringBuilder password = new StringBuilder();

            Random ran = new Random();

            for (int i = 0; i < 5; i++)
                password.Append((char)(byte)ran.Next(65, 90));
            return password.ToString();
        }
        /// <summary>
        /// Tries to get the app setting using the provided key.
        /// </summary>
        /// <param name="key">App setting key whose value is retrieved</param>
        /// <param name="setFalseOnStringNullEmpty">determines if a false value is returned when the read app setting value is null or empty</param>
        /// <param name="value">App setting value is read into this out param</param>
        /// <returns>Returns a boolean value if setting value is found</returns>
        public static bool TryGetAppSettings(string key, bool setFalseOnStringNullEmpty, out string value)
        {
            bool hasRead = false;
            try
            {
                value = System.Configuration.ConfigurationSettings.AppSettings[key];
                hasRead = true;
            }
            catch
            {
                value = null;
            }
            if (setFalseOnStringNullEmpty)
                hasRead = !String.IsNullOrEmpty(value);
            return hasRead;
        }
        /// <summary>
        /// Uses the LUHN algorythm to valide credit card number
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public static bool IsCreditCardValid(string cardNumber)
        {
            long validNumber;
            StringBuilder enteredNumber = new StringBuilder(cardNumber);
            int i;
            enteredNumber.Replace(" ", "");

            StringBuilder cleanNumber = new StringBuilder();
            foreach (char character in enteredNumber.ToString().ToCharArray())
            {
                if (Char.IsNumber(character))
                    cleanNumber.Append(character);
            }
            if (!long.TryParse(enteredNumber.ToString(), out validNumber))
                return false;
            if (cleanNumber.Length < 13 && cleanNumber.Length > 16)
                return false;
            for (i = cleanNumber.Length + 1; i <= 16; i++)
                cleanNumber.Insert(0, "0");
            int multiplier, digit, sum, total = 0;
            string number = cleanNumber.ToString();
            for (i = 1; i <= 16; i++)
            {
                multiplier = 1 + (i % 2);
                digit = int.Parse(number.Substring((i - 1), 1));
                sum = digit * multiplier;
                if (sum > 9)
                    sum -= 9;
                total += sum;
            }
            return (total % 10 == 0);
        }

        public static int ReturnSingleCodeIDByKeyword(string singlecodeKeyword)
        {
            int singlecodeContentID = -1;
            var db = DatabaseFactory.CreateDatabase("SubscriptionsDatabase");
            var command = db.GetStoredProcCommand("exm_GetSingleCodeIDByKeyword");

            var param_keyword = command.CreateParameter();
            param_keyword.Value = singlecodeKeyword;
            param_keyword.DbType = DbType.String;
            param_keyword.ParameterName = "@keyword";
            command.Parameters.Add(param_keyword);
            IDataReader reader;
            using (command)
                reader = db.ExecuteReader(command);
            using (reader)
            {
                if (reader.Read())
                    singlecodeContentID = Convert.ToInt32(reader.GetValue(0));
            }
            return singlecodeContentID;
        }

        public static string ReturnUSSDMenuText(int ussdCampaignID, int menuID)
        {
            string menuText;
            Database db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            DbCommand command = db.GetStoredProcCommand("usp_GetSingleMenuItem", new object[] { ussdCampaignID, menuID });

            using (command)
            {
                var reader = db.ExecuteReader(command);
                using (reader)
                {
                    if (reader.Read())
                        menuText = Convert.ToString(reader["MenuName"]);
                    else
                        menuText = string.Empty;
                }
            }
            return menuText;
        }
        public static bool ReturnNearestChargeCodeAmount(int networkID, int chargeCodeOwnerID, decimal amount, out decimal nearestAmount)
        {
            bool hasReturned = false;
            try
            {
                nearestAmount = 0M;
                Database db = DatabaseFactory.CreateDatabase("ExactPaymentDatabase");
                DbCommand command = db.GetStoredProcCommand("sp_GetNearestChargeCode");// @paymentTypeID = {0},@chargeCodeOwnerID={1}, @amount={2}", networkID, chargeCodeOwnerID, amount);
                var param_paymentTypeID = command.CreateParameter();
                var param_chargeCodeOwnerID = command.CreateParameter();
                var param_amount = command.CreateParameter();

                param_paymentTypeID.Value = networkID;
                param_paymentTypeID.DbType = DbType.Int32;
                param_paymentTypeID.ParameterName = "@paymentTypeID";

                param_amount.Value = amount;
                param_amount.DbType = DbType.Decimal;
                param_amount.ParameterName = "@amount";

                param_chargeCodeOwnerID.Value = chargeCodeOwnerID;
                param_chargeCodeOwnerID.DbType = DbType.Int32;
                param_chargeCodeOwnerID.ParameterName = "@chargeCodeOwnerID";

                command.Parameters.AddRange(new DbParameter[] { param_paymentTypeID, param_chargeCodeOwnerID, param_amount });
                using (command)
                {
                    var reader = db.ExecuteReader(command);
                    using (reader)
                    {
                        if (reader.Read())
                            hasReturned = Decimal.TryParse(Convert.ToString(CommonUtility.IsDBNull(reader["Amount"], "r")), out nearestAmount);
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hasReturned;
        }
        public static object IsDBNull(object toEvaluate, object toReplaceWith)
        {
            if (toEvaluate == DBNull.Value)
                return toReplaceWith;
            else
                return toEvaluate;
        }

        public static bool ReturnUSSDCampaignText(int ussdCampaignIDRef, string textKey, out string text, out string textDelimiter)
        {
            var db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            var cmd = db.GetStoredProcCommand("[usp_ReturnUSSDTextByKeyAndCampaign]");

            var param_ussdCampaignIDRef = cmd.CreateParameter();
            var param_textKey = cmd.CreateParameter();

            param_ussdCampaignIDRef.ParameterName = "@ussdCampaignIDRef";
            param_ussdCampaignIDRef.Value = ussdCampaignIDRef;
            param_ussdCampaignIDRef.DbType = DbType.Int32;

            param_textKey.ParameterName = "@textKey";
            param_textKey.DbType = DbType.String;
            param_textKey.Value = textKey;

            cmd.Parameters.AddRange(new DbParameter[] { param_ussdCampaignIDRef, param_textKey });
            using (cmd)
            {
                using (var reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        textDelimiter = Convert.ToString(CommonUtility.IsDBNull(reader["Delimeter"], string.Empty));
                        text = Convert.ToString(reader["text"]).Replace("\\n", "\n").Replace("\\r", "\r");
                    }
                    else
                    {
                        textDelimiter = string.Empty;
                        text = string.Empty;
                    }
                }
            }
            return (!string.IsNullOrEmpty(text));
        }
        public static bool ReturnUSSDCampaignText(int ussdCampaignIDRef, string textKey, out string text, out string textDelimiter, out int USSDTextID)
        {
            var db = DatabaseFactory.CreateDatabase("ExactUSSDDatabase");
            var cmd = db.GetStoredProcCommand("[usp_ReturnUSSDTextByKeyAndCampaign]");

            var param_ussdCampaignIDRef = cmd.CreateParameter();
            var param_textKey = cmd.CreateParameter();

            param_ussdCampaignIDRef.ParameterName = "@ussdCampaignIDRef";
            param_ussdCampaignIDRef.Value = ussdCampaignIDRef;
            param_ussdCampaignIDRef.DbType = DbType.Int32;

            param_textKey.ParameterName = "@textKey";
            param_textKey.DbType = DbType.String;
            param_textKey.Value = textKey;

            cmd.Parameters.AddRange(new DbParameter[] { param_ussdCampaignIDRef, param_textKey });
            using (cmd)
            {
                using (var reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        textDelimiter = Convert.ToString(CommonUtility.IsDBNull(reader["Delimeter"], string.Empty));
                        text = Convert.ToString(reader["text"]).Replace("\\n", "\n").Replace("\\r", "\r");
                        USSDTextID = Convert.ToInt32(reader["USSDTextID"]);
                    }
                    else
                    {
                        textDelimiter = string.Empty;
                        text = string.Empty;
                        USSDTextID = 0;
                    }
                }
            }
            return (!string.IsNullOrEmpty(text));
        }

        public static HttpClient GetHttpConnection(string name)
        {
            HttpClient cons = new HttpClient();
            switch (name)
            {
                case "CancellationAPI":
                    cons.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["CancellationAPI"].ToString());
                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case "PaymentAPI":
                    cons.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["PaymentAPI"].ToString());
                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case "SMSAPI":
                    cons.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["SMSAPI"].ToString());
                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    cons.DefaultRequestHeaders.Add("x-api-key", System.Configuration.ConfigurationManager.AppSettings["SMS-Api-key"].ToString());
                    break;
                    
                default:
                    cons.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["MutuelleSmartAPI"].ToString());
                    cons.DefaultRequestHeaders.Accept.Clear();
                    cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    break;
            }

            return cons;
        }

        public static Tuple<bool, string> SendSMSNotification(string MSISDN, string message)
        {
            var client = ussdcommon.utility.CommonUtility.GetHttpConnection("SMSAPI");
            var json = JsonConvert.SerializeObject(new Dictionary<string, string> { { "msisdn", MSISDN},{"sms",message } });
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var URL = new Uri(client.BaseAddress,"");

            using (client)
            {
                try
                {
                    var result = client.PostAsync(URL, content).Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {

                        return new Tuple<bool, string> ( true,"subscribed successfully" );
                    }

                }
                catch (Exception exp)
                {
                    return new Tuple<bool, string>(true, exp.Message);

                }
            }
            return new Tuple<bool, string>(false, "failed to subscribe");

        }
    }
}
