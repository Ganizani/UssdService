using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MBDInternal.Config;
namespace USSD.DAL
{
    public class Constant
    {
        public const string USSDSERVICE_MYSQLDB = "USSDSERVICE.MYSQL.DATABASE";
        public const string USSDSERVICE_MSSQLDB = "USSDSERVICE.MSSQL.DATABASE";
        public static string USSDSERVICE_MSSQLDB_CONNESCTIONSTRING = @"Data Source=BIGJOE\BIGJOE;Initial Catalog=USSDService;Integrated Security=True;";

        public const string Environment = "dev";
        public const string SUBSCRIPTIONSERVICE_MYSQLDB = "SUBSCRIPTIONSERVICE.MYSQL.DATABASE";
        public const string SUBSCRIPTIONSERVICE_MSSQLDB = "SUBSCRIPTIONSERVICE.MSSQL.DATABASE";
        public static string SUBSCRIPTIONSERVICE_MSSQLDB_CONNESCTIONSTRING = @"Data Source=BIGJOE\BIGJOE;Initial Catalog=SubscriptionService;Integrated Security=True;";

        static Constant()
        {
            USSDSERVICE_MSSQLDB_CONNESCTIONSTRING = System.Configuration.ConfigurationManager.ConnectionStrings["ExactUssdDatabase"].ConnectionString;

            SUBSCRIPTIONSERVICE_MSSQLDB_CONNESCTIONSTRING = System.Configuration.ConfigurationManager.ConnectionStrings["SubscriptionsDatabase"].ConnectionString;
        }

    }
}
