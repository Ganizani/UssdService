using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace exactmobile.ussdcommon
{
    public sealed class DB
    {
        private static Database exactdotnetDatabase;
        private static Database exactPaymentDatabase;
        private static Database exactUSSDDatabase;
        const string ExactDotNetDatabaseKey = "SubscriptionsDatabase";
        const string ExactPaymentDatabaseKey = "ExactPaymentDatabase";
        const string ExactUSSDDatabaseKey = "ExactUSSDDatabase";

        static DB()
        {
            if (exactdotnetDatabase == null)
                exactdotnetDatabase = DatabaseFactory.CreateDatabase(DB.ExactDotNetDatabaseKey);

            if (exactUSSDDatabase == null)
                exactUSSDDatabase = DatabaseFactory.CreateDatabase(DB.ExactUSSDDatabaseKey);

            if (exactPaymentDatabase == null)
                exactPaymentDatabase = DatabaseFactory.CreateDatabase(DB.ExactPaymentDatabaseKey);
        }

        public static Database ExactDotNet
        {
            get
            {
                if (exactdotnetDatabase == null)
                    exactdotnetDatabase = DatabaseFactory.CreateDatabase(DB.ExactDotNetDatabaseKey);
                return exactdotnetDatabase;
            }
        }

        /// <summary>
        /// Database to the Exactmobile USSD Database
        /// </summary>
        public static Database ExactUSSD
        {
            get
            {
                if (exactUSSDDatabase == null)
                    exactUSSDDatabase = DatabaseFactory.CreateDatabase(DB.ExactUSSDDatabaseKey);
                return exactUSSDDatabase;
            }
        }

        /// <summary>
        /// Database to the Exact Payment Database
        /// </summary>
        public static Database ExactPayment
        {
            get
            {
                if (exactPaymentDatabase == null)
                    exactPaymentDatabase = DatabaseFactory.CreateDatabase(DB.ExactPaymentDatabaseKey);
                return exactPaymentDatabase;
            }
        }

        public static object IsDBNull(object toEvaluate, object toReplaceWith)
        {
            if (toEvaluate == DBNull.Value)
                return toReplaceWith;
            else
                return toEvaluate;
        }
    }
}
