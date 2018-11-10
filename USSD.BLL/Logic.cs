using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USSD.DAL;
using USSD.Entities;
using NLog;


namespace USSD.BLL
{
    public class Logic : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private Logic()
        {
            if (log.IsInfoEnabled) { log.Info("MBD.Capitaldata Logic Layer initialized"); }
           
            //context = new SynergyFleet.DAL.Data.SynergyFleetEntities(SynergyFleet.DAL.Constant.tempconn);

        }

        #region Singleton Related Code

        // Note: This is based on singleton example code from http://yoda.arachsys.com/csharp/singleton.html

        public static Logic Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested() { }

            internal static readonly Logic instance = new Logic();
        }

        #endregion

        
        public void Dispose()
        {
            if (log.IsInfoEnabled) { log.Info("MBD.Capitaldata Logic Layer Disposed"); }
        }

        #region LogicLayer Properties

        //private UserLogicLayer _Users = null;
        //private readonly object _UsersLock = new object();
        //public UserLogicLayer Users
        //{
        //    get
        //    {
        //        lock (_UsersLock)
        //        {
        //            if (_Users == null)
        //            {
        //                _Users = new UserLogicLayer(Shared.da);
        //            }
        //        }
        //        return _Users;
        //    }
        //    set
        //    {
        //        lock (_UsersLock)
        //        {
        //            _Users = value;
        //        }
        //    }
        //}

        //private InstructionLogicLayer _Instructions = null;
        //private readonly object _InstructionsLock = new object();
        //public InstructionLogicLayer Instructions
        //{
        //    get
        //    {
        //        lock (_InstructionsLock)
        //        {
        //            if (_Instructions == null)
        //            {
        //                _Instructions = new InstructionLogicLayer(Shared.da);
        //            }
        //        }
        //        return _Instructions;
        //    }
        //    set
        //    {
        //        lock (_InstructionsLock)
        //        {
        //            _Instructions = value;
        //        }
        //    }
        //}

        
        private CommonLogicLayer _Commons = null;
        private readonly object _CommonsLock = new object();
        public CommonLogicLayer Commons
        {
            get
            {
                lock (_CommonsLock)
                {
                    if (_Commons == null)
                    {
                        _Commons = new CommonLogicLayer(Shared.da);
                    }
                }
                return _Commons;
            }
            set
            {
                lock (_CommonsLock)
                {
                    _Commons = value;
                }
            }
        }

        private GenericTypeLogicLayer<USSDCampaign> _Campaigns = null;
        private readonly object _CampaignsLock = new object();
        public GenericTypeLogicLayer<USSDCampaign> Campaigns
        {
            get
            {
                lock (_CampaignsLock)
                {
                    if (_Campaigns == null)
                    {
                        _Campaigns = new GenericTypeLogicLayer<USSDCampaign>(Shared.da);
                    }
                }
                return _Campaigns;
            }
            set
            {
                lock (_CampaignsLock)
                {
                    _Campaigns = value;
                }
            }
        }

        private GenericTypeLogicLayer<ClaimStatus> _ClaimStatuses = null;
        private readonly object _ClaimStatusesLock = new object();
        public GenericTypeLogicLayer<ClaimStatus> ClaimStatuses
        {
            get
            {
                lock (_ClaimStatusesLock)
                {
                    if (_ClaimStatuses == null)
                    {
                        _ClaimStatuses = new GenericTypeLogicLayer<ClaimStatus>(Shared.da);
                    }
                }
                return _ClaimStatuses;
            }
            set
            {
                lock (_ClaimStatusesLock)
                {
                    _ClaimStatuses = value;
                }
            }
        }

        private GenericTypeLogicLayer<USSDTransaction> _USSDTransactions = null;
        private readonly object _USSDTransactionsLock = new object();
        public GenericTypeLogicLayer<USSDTransaction> USSDTransactions
        {
            get
            {
                lock (_USSDTransactionsLock)
                {
                    if (_USSDTransactions == null)
                    {
                        _USSDTransactions = new GenericTypeLogicLayer<USSDTransaction>(Shared.da);
                    }
                }
                return _USSDTransactions;
            }
            set
            {
                lock (_USSDTransactionsLock)
                {
                    _USSDTransactions = value;
                }
            }
        }
        private GenericTypeLogicLayer<HTTPTransaction> _HTTPTransactions = null;
        private readonly object _HTTPTransactionsLock = new object();
        public GenericTypeLogicLayer<HTTPTransaction> HTTPTransactions
        {
            get
            {
                lock (_HTTPTransactionsLock)
                {
                    if (_HTTPTransactions == null)
                    {
                        _HTTPTransactions = new GenericTypeLogicLayer<HTTPTransaction>(Shared.da);
                    }
                }
                return _HTTPTransactions;
            }
            set
            {
                lock (_HTTPTransactionsLock)
                {
                    _HTTPTransactions = value;
                }
            }
        }
        private GenericTypeLogicLayer<MenuTransaction> _MenuTransactions = null;
        private readonly object _MenuTransactionsLock = new object();
        public GenericTypeLogicLayer<MenuTransaction> MenuTransactions
        {
            get
            {
                lock (_MenuTransactionsLock)
                {
                    if (_MenuTransactions == null)
                    {
                        _MenuTransactions = new GenericTypeLogicLayer<MenuTransaction>(Shared.da);
                    }
                }
                return _MenuTransactions;
            }
            set
            {
                lock (_MenuTransactionsLock)
                {
                    _MenuTransactions = value;
                }
            }
        }
        private GenericTypeLogicLayer<Address> _Addresses = null;
        private readonly object _AddressesLock = new object();
        public GenericTypeLogicLayer<Address> Addresses
        {
            get
            {
                lock (_AddressesLock)
                {
                    if (_Addresses == null)
                    {
                        _Addresses = new GenericTypeLogicLayer<Address>(Shared.da);
                    }
                }
                return _Addresses;
            }
            set
            {
                lock (_MenuTransactionsLock)
                {
                    _Addresses = value;
                }
            }
        }


        #endregion
    }
}
