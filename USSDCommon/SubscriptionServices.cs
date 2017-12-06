using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using exactmobile.components;
using System.Data.SqlClient;

namespace exactmobile.ussdcommon
{
    public class SubscriptionService
    {
        public static Hashtable GetActiveServices()
        {
            Hashtable hashtable = new Hashtable();

            CDB DB = new CDB("GetActiveServices");
            SqlDataReader reader = (SqlDataReader)DB.Execute("SUB_GetActiveSubscriptionServices", CDB.exmReturnTypes.RETURN_READER);

            while (reader.Read())
            {
                SubscriptionService service = new SubscriptionService(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), (BillingFrequencies)reader.GetInt32(3), (BillingTimes)reader.GetInt32(4), reader.GetDecimal(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetString(8), (Statuses)reader.GetInt32(9));
                hashtable.Add(service.ID, service);
            }

            reader.Close();
            reader = null;

            DB.Close();
            DB = null;

            return hashtable;
        }

        private int _ID = 0;
        private string _Description = "";
        private int _GroupID = 0;
        private BillingFrequencies _BillingFrequency = BillingFrequencies.Monthly;
        private BillingTimes _BillingTime = BillingTimes.Exact;
        private decimal _BillingAmount = 0M;
        private int _BillingDayOfMonth = 0;
        private int _PaymentSystemClientID = 0;
        private CQueueWrapper _ResponseQueue = null;
        private Statuses _Status = Statuses.Invalid;

        public SubscriptionService(int id)
        {
            CDB DB = new CDB("GetActiveServices");
            SqlDataReader reader = (SqlDataReader)DB.Execute("SUB_GetActiveSubscriptionService " + id, CDB.exmReturnTypes.RETURN_READER);

            if (reader.Read())
            {
                var service = new SubscriptionService(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), (BillingFrequencies)reader.GetInt32(3), (BillingTimes)reader.GetInt32(4), reader.GetDecimal(5), reader.GetInt32(6), reader.GetInt32(7), reader.GetString(8), (Statuses)reader.GetInt32(9));
                _ID = service.ID;
                _Description = service.Description;
                _GroupID = service.GroupID;
                _BillingFrequency = service.BillingFrequency;
                _BillingTime = service.BillingTime;
                _BillingAmount = service.BillingAmount;
                _BillingDayOfMonth = service.BillingDayOfMonth;
                _PaymentSystemClientID = service.PaymentSystemClientID;
                _ResponseQueue = new CQueueWrapper(service.ResponseQueue.QueueName);
                _Status = service.Status;
            }

            reader.Close();
            reader = null;

            DB.Close();
            DB = null;

        }

        public SubscriptionService(int id, string description, int groupID, BillingFrequencies billingFrequency, BillingTimes billingTime, decimal billingAmount, int billingDatOfMonth, int paymentSystemClientID, string responseQueue, Statuses status)
        {
            _ID = id;
            _Description = description;
            _GroupID = groupID;
            _BillingFrequency = billingFrequency;
            _BillingTime = billingTime;
            _BillingAmount = billingAmount;
            _BillingDayOfMonth = billingDatOfMonth;
            _PaymentSystemClientID = paymentSystemClientID;
            _ResponseQueue = new CQueueWrapper(responseQueue);
            _Status = status;
        }

        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        public int GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                _GroupID = value;
            }
        }

        public BillingFrequencies BillingFrequency
        {
            get
            {
                return _BillingFrequency;
            }
            set
            {
                _BillingFrequency = value;
            }
        }

        public BillingTimes BillingTime
        {
            get
            {
                return _BillingTime;
            }
            set
            {
                _BillingTime = value;
            }
        }

        public decimal BillingAmount
        {
            get
            {
                return _BillingAmount;
            }
            set
            {
                _BillingAmount = value;
            }
        }

        public int BillingDayOfMonth
        {
            get
            {
                return _BillingDayOfMonth;
            }
            set
            {
                _BillingDayOfMonth = value;
            }
        }

        public int PaymentSystemClientID
        {
            get
            {
                return _PaymentSystemClientID;
            }
            set
            {
                _PaymentSystemClientID = value;
            }
        }

        public CQueueWrapper ResponseQueue
        {
            get
            {
                return _ResponseQueue;
            }
            set
            {
                _ResponseQueue = value;
            }
        }

        public Statuses Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
    }
}
