using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using exactmobile.components.logging;
using exactmobile.common;
using System.IO;
using System.Net.Sockets;
using exactmobile.ussdservice.configuration.tcp;
using System.Xml;
using exactmobile.ussdservice.common.handlers;
using exactmobile.ussdservice.handlers;
using exactmobile.ussdservice.exceptions;

namespace exactmobile.ussdservice.tcp.mtn
{
    public class MtnUSSDTCPListener : USSDTCPListener
    {
        #region Constants
        private const Byte REQUEST_TERMINATOR = 0xFF;
        #endregion

        #region private vars
        private USSDTcpListenerConfigurationSection ussdTcpListenerConfigurationSection;
        private String cookie;
        private DateTime lastActivity = DateTime.MinValue;
        #endregion

        #region Static Methods
        #endregion

        #region public Methods
        public MtnUSSDTCPListener(USSDTcpListenerConfigurationSection ussdTcpListenerConfigurationSection)
            : base(ussdTcpListenerConfigurationSection)
        {
            this.ussdTcpListenerConfigurationSection = ussdTcpListenerConfigurationSection;
        }
        #endregion

        #region Protected Methods
        protected override void OnConnectedToServer(Socket socket)
        {
            lastActivity = DateTime.Now;
            listenerState = ListenerStates.Connected;
            
            if (Login())
            {
                StartPingThread();
                listenerState = ListenerStates.Running;
                OnPing += new EventHandler(DoPing);
                while (listenerState == ListenerStates.Running)
                    WaitForData();
            }
        }

        protected override void OnServerDisconnected(Socket socket, ref Boolean autoReconnectWhenConnectionIsLost)
        {
        }

        protected override void OnDataReceived(Byte[] receiveData)
        {
            MtnUSSDTCPPDU pdu = null;

            try
            {
                pdu = MtnUSSDTCPPDU.FromXML(Encoding.ASCII.GetString(receiveData));
            }
            catch(Exception ex)
            {
                LogManager.LogError("XML Invalid: " + ex.Message);
                return;
            }

            lastActivity = DateTime.Now;
            try
            {
                if (pdu != null)
                {
                    if (pdu.Pdu == MtnUSSDTCPPDU.PDUTypes.CTRL.ToString())
                    {                        
                        LogManager.LogStatus("  ** <-- ping returned   ** ");                
                        //Console.WriteLine("--> ping sent");
                    }
                    else
                    {
                        LogManager.LogStatus("Received " + Encoding.ASCII.GetString(receiveData));

                        String requestData = Encoding.ASCII.GetString(receiveData);
                        USSDHandlerRequestType.RequestTypes requestType = USSDHandlerRequestType.RequestTypes.TCP;
                        IUSSDHandler ussdHandler = USSDHandlerFactory.GetHandler(mobileNetwork, requestData, requestType);
                        Boolean isTimeout = false;
                        Boolean isInvalid = false;
                        ussdHandler.Initialize(requestData, requestType, out isTimeout, out isInvalid);
                        if (!isTimeout)
                        {
                            LogManager.LogStatus("Handler {0} found for request", ussdHandler.HandlerID);
                            String responseData = ussdHandler.ProcessRequest(requestData);
                            SendResponse(pdu, responseData);
                        }
                        else
                            LogManager.LogStatus("Timout request for handler {0} received", ussdHandler.HandlerID);
                    }
                }
                else
                    SendResponse(pdu, "Invalid/Empty Request");
            }

            catch (Exception ex)
            {
                LogManager.LogError(ex);
                SendResponse(pdu, "Invalid request. Please retry.");
            }
        }

        protected void SendResponse(MtnUSSDTCPPDU pdu, String responseText)
        {
            MtnUSSDTCPPDU.PDUTypes pduReturnType = pdu.FindResponseType((MtnUSSDTCPPDU.PDUTypes)Enum.Parse(typeof(MtnUSSDTCPPDU.PDUTypes), pdu.Pdu, true), false);
            MtnUSSDTCPPDU responsePDU = new MtnUSSDTCPPDU();
            responsePDU.Pdu = pduReturnType.ToString();
            responsePDU.Cookie = cookie;
            responsePDU.Data = responseText;
            responsePDU.MSISDN = pdu.MSISDN;
            responsePDU.RequestID = pdu.RequestID;
            responsePDU.SessionID = pdu.SessionID;
            responsePDU.Status = "0";
            responsePDU.Tariff = pdu.Tariff;
            responsePDU.Encoding = pdu.Encoding;
            LogManager.LogStatus("Response: " + responsePDU.ToXML());
            Send(responsePDU.ToXML());
        }

        protected Boolean Login()
        {
            int retries = 0;
            while (listenerState == ListenerStates.Connected)
            {
                if (retries == 0)
                    LogManager.LogStatus("{0} logging in", Name);
                else
                    LogManager.LogStatus("{0} login retry {1}", Name, retries);
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                    xmlDoc.AppendChild(xmlDeclaration);

                    XmlElement loginElement = xmlDoc.CreateElement("login");
                    xmlDoc.AppendChild(loginElement);

                    XmlAttribute cookieAttribute = xmlDoc.CreateAttribute("COOKIE");
                    cookieAttribute.Value = this.ussdTcpListenerConfigurationSection.Cookie;
                    loginElement.Attributes.Append(cookieAttribute);

                    XmlAttribute nodeIDAttribute = xmlDoc.CreateAttribute("NODE_ID");
                    nodeIDAttribute.Value = this.ussdTcpListenerConfigurationSection.Nodeid;
                    loginElement.Attributes.Append(nodeIDAttribute);

                    XmlAttribute passwordAttribute = xmlDoc.CreateAttribute("PASSWORD");
                    passwordAttribute.Value = this.ussdTcpListenerConfigurationSection.Password;
                    loginElement.Attributes.Append(passwordAttribute);

                    XmlAttribute rmtSysAttribute = xmlDoc.CreateAttribute("RMT_SYS");
                    rmtSysAttribute.Value = this.ussdTcpListenerConfigurationSection.Rmtsys;
                    loginElement.Attributes.Append(rmtSysAttribute);

                    XmlAttribute userAttribute = xmlDoc.CreateAttribute("USER");
                    userAttribute.Value = this.ussdTcpListenerConfigurationSection.User;
                    loginElement.Attributes.Append(userAttribute);

                    Byte[] receiveData = SendAndWaitForResponse(xmlDoc.OuterXml);

                  

                    XmlDocument receiveXml = new XmlDocument();
                    receiveXml.LoadXml(Encoding.ASCII.GetString(receiveData));
                    if (receiveXml.FirstChild != null && receiveXml.FirstChild.NextSibling != null && receiveXml.FirstChild.NextSibling.Name.ToUpper() == "COOKIE" && !String.IsNullOrEmpty(receiveXml.FirstChild.NextSibling.Attributes[0].Value))
                    {
                        cookie = receiveXml.FirstChild.NextSibling.Attributes[0].Value;
                        LogManager.LogStatus("Sucessfully logged in[Cookie={0}]", cookie);
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex);
                }
                retries++;
            }
            return false;
        }

        protected void DoPing(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(lastActivity).TotalMilliseconds < this.ussdTcpListenerConfigurationSection.PingInterval) return;

            MtnUSSDTCPPDU pdu = new MtnUSSDTCPPDU();
            pdu.Pdu = MtnUSSDTCPPDU.PDUTypes.CTRL.ToString();
            pdu.Data = DateTime.Now.ToString();
            try
            {
                //Console.Write(".");
                LogManager.LogStatus("  ** --> ping            ** ");                
                //LogManager.LogStatus("PDU: " + pdu.ToXML());
                Send(pdu.ToXML());
            }
            catch (USSDTcpSendTimeoutException te)
            {
                LogManager.LogError(te);
            }
        }
        #endregion
    }
}
