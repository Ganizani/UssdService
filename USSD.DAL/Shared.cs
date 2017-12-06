using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBDInternal.Data;
using MBDInternal.Config;
using NLog;
using System.Configuration;

namespace USSD.DAL
{
    public class Shared
    {
        private static readonly NLog.Logger log = LogManager.GetCurrentClassLogger();
        public static readonly DataAccess da;
        public static  DataAccess daSub;
        public static readonly DataAccess daLegalServ10;

        static Shared()
        {
            if (log.IsInfoEnabled) { log.Info("Starting up MBD.Capitaldata.Shared"); }

            InitDataAccess(out da, Constant.USSDSERVICE_MSSQLDB);
            InitDataAccess(out daSub, Constant.SUBSCRIPTIONSERVICE_MSSQLDB);
           
        }


        private static void InitDataAccess(out DataAccess dataAccess, string connectionName)
        {
            dataAccess = null;
            string environment = Constant.Environment;
            string providerName = "System.Data.SqlClient";

            try
            {
                environment = ConfigurationManager.AppSettings["AppState"].ToString(); // This can be controlled on an Web or App Config file
            }
            catch { }

            //var dbConfig = ConfigAPI.RequestDBConfig(connectionName, environment);

            //if (dbConfig != null)
            //{
            //    //if (log.IsInfoEnabled) { log.Info(string.Format("ConnectionString {0}", dbConfig.ConnectionString)); }
            //    dataAccess = new DataAccess(dbConfig.szConnectionString, dbConfig.szProviderName);

            //}
            //else
            if (!string.IsNullOrEmpty(Constant.USSDSERVICE_MSSQLDB_CONNESCTIONSTRING))
            {
                dataAccess = new DataAccess(Constant.USSDSERVICE_MSSQLDB_CONNESCTIONSTRING, providerName);
                //daSub = new DataAccess(Constant.SUBSCRIPTIONSERVICE_MSSQLDB_CONNESCTIONSTRING, providerName);
            }

            else
            {
                
                if (log.IsFatalEnabled) { log.Fatal(string.Format("Could not get DB Config for {0}", connectionName)); }
                throw new Exception("Could not get DB Config for " + connectionName);
            }
        }

    }


}
