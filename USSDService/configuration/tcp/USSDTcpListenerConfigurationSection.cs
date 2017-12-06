using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace exactmobile.ussdservice.configuration.tcp
{
    public sealed class USSDTcpListenerConfigurationSection : USSDListenerConfigurationSection
    {
        #region Properties
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = true)]
        public Boolean Enabled
        {
            get
            {
                return (Boolean)base["enabled"];
            }
            set
            {

                base["enabled"] = value;
            }
        }

        [ConfigurationProperty("address", DefaultValue = "127.0.0.1", IsRequired = true)]
        public String Address
        {
            get
            {
                return (String)base["address"];
            }
            set
            {

                base["address"] = value;
            }
        }

        [ConfigurationProperty("port", DefaultValue = 0, IsRequired = false)]
        public int? Port
        {
            get
            {
                return (int?)base["port"];
            }
            set
            {

                base["port"] = value;
            }
        }

        [ConfigurationProperty("mobilenetwork", DefaultValue = "", IsRequired = false)]
        public String MobileNetwork
        {
            get
            {
                return (String)base["mobilenetwork"];
            }
            set
            {

                base["mobilenetwork"] = value;
            }
        }

        [ConfigurationProperty("sendbufferSize", DefaultValue = 1024, IsRequired = false)]
        public int SendBufferSize
        {
            get
            {
                return (int)base["sendbufferSize"];
            }
            set
            {

                base["sendbufferSize"] = value;
            }
        }

        [ConfigurationProperty("sendtimeout", DefaultValue = 30000, IsRequired = false)]
        public int SendTimeout
        {
            get
            {
                return (int)base["sendtimeout"];
            }
            set
            {

                base["sendtimeout"] = value;
            }
        }

        [ConfigurationProperty("receivebufferSize", DefaultValue = 1024, IsRequired = false)]
        public int ReceiveBufferSize
        {
            get
            {
                return (int)base["receivebufferSize"];
            }
            set
            {

                base["receivebufferSize"] = value;
            }
        }

        [ConfigurationProperty("receivetimeout", DefaultValue = 30000, IsRequired = false)]
        public int ReceiveTimeout
        {
            get
            {
                return (int)base["receivetimeout"];
            }
            set
            {

                base["receivetimeout"] = value;
            }
        }

        [ConfigurationProperty("type", DefaultValue = null, IsRequired = true)]
        public String Type
        {
            get
            {
                return (String)base["type"];
            }
            set
            {

                base["type"] = value;
            }
        }

        [ConfigurationProperty("autoReconnectWhenConnectionIsLost", DefaultValue = true, IsRequired = false)]
        public Boolean AutoReconnectWhenConnectionIsLost
        {
            get
            {
                return (Boolean)base["autoReconnectWhenConnectionIsLost"];
            }
            set
            {

                base["autoReconnectWhenConnectionIsLost"] = value;
            }
        }

        [ConfigurationProperty("pinginterval", DefaultValue = 30000, IsRequired = false)]
        public int PingInterval
        {
            get
            {
                return (int)base["pinginterval"];
            }
            set
            {

                base["pinginterval"] = value;
            }
        }

        [ConfigurationProperty("user", DefaultValue = "", IsRequired = false)]
        public String User
         {
            get
            {
                return (String)base["user"];
            }
            set
            {

                base["user"] = value;
            }
        }

        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        public String Password
 {
            get
            {
                return (String)base["password"];
            }
            set
            {

                base["password"] = value;
            }
        }

        [ConfigurationProperty("rmtsys", DefaultValue = "", IsRequired = false)]
        public String Rmtsys
 {
            get
            {
                return (String)base["rmtsys"];
            }
            set
            {

                base["rmtsys"] = value;
            }
        }

        [ConfigurationProperty("nodeid", DefaultValue = "", IsRequired = false)]
        public String Nodeid
 {
            get
            {
                return (String)base["nodeid"];
            }
            set
            {

                base["nodeid"] = value;
            }
        }

        [ConfigurationProperty("cookie", DefaultValue = "", IsRequired = false)]
        public String Cookie
        {
            get
            {
                return (String)base["cookie"];
            }
            set
            {

                base["cookie"] = value;
            }
        }

        [ConfigurationProperty("packetterminator", DefaultValue="255", IsRequired = false)]
        public String PacketTerminator
        {
            get
            {
                return (String)base["packetterminator"];
            }
            set
            {

                base["packetterminator"] = value;
            }
        }
        #endregion
    }
}
