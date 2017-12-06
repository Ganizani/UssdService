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
using exactmobile.ussdservice.configuration;
using exactmobile.ussdservice.configuration.tcp;
using exactmobile.ussdservice.exceptions;
using exactmobile.common.Lookups;

namespace exactmobile.ussdservice.tcp
{
    public abstract class USSDTCPListener
    {
        #region enums
        protected enum ListenerStates
        {
            Stopping,
            Stopped,
            Connecting,
            Connected,
            Running
        }
        #endregion

        #region Private Vars
        private Socket tcpClientSocket;
        private String name;
        private String address;
        private int? port;
        private int sendBufferSize;
        private int sendTimeout;
        private int receiveBufferSize;
        private int receiveTimeout;
        protected ListenerStates listenerState = ListenerStates.Connecting;
        private AutoResetEvent connectWaitEvent = new AutoResetEvent(false);
        private Byte[] packetTerminator;
        protected MobileNetworks.MobileNetwork mobileNetwork;
        private int pingInterval;
        private Thread pingThread;
        private AutoResetEvent pingThreadHandle;
        #endregion

        #region Properties
        public Boolean AutoReconnectWhenConnectionIsLost
        {
            get;
            set;
        }
        private IPEndPoint LocalIPEndPoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Any, 0);
            }
        }

        private IPEndPoint RemoteIPEndPoint
        {
            get
            {
                int tempPort = 0;
                if (port != null)
                    tempPort = port.Value;

                return new IPEndPoint(IPAddress.Parse(address), tempPort);
            }
        }

        public String Name
        {
            get { return name; }
        }
        #endregion

        #region events
        protected event EventHandler OnPing;
        #endregion

        #region Static Methods
        //public static T CreateNew<T>(USSDTcpListenerConfigurationSection ussdTcpListenerConfigurationSection)
        //    where T : USSDTCPListener, new()
        //{
        //    USSDTCPListener result = new T();
        //    result.name = ussdTcpListenerConfigurationSection.Name;
        //    result.address = ussdTcpListenerConfigurationSection.Address;
        //    result.port = ussdTcpListenerConfigurationSection.Port;
        //    result.sendBufferSize = ussdTcpListenerConfigurationSection.SendBufferSize;
        //    result.sendTimeout = ussdTcpListenerConfigurationSection.SendTimeout;
        //    result.receiveBufferSize = ussdTcpListenerConfigurationSection.ReceiveBufferSize;
        //    result.receiveTimeout = ussdTcpListenerConfigurationSection.ReceiveTimeout;
        //    return result as T;
        //}
        #endregion

        #region Public Methods
        public USSDTCPListener(USSDTcpListenerConfigurationSection ussdTcpListenerConfigurationSection)
        {
            this.AutoReconnectWhenConnectionIsLost = ussdTcpListenerConfigurationSection.AutoReconnectWhenConnectionIsLost;
            this.address = ussdTcpListenerConfigurationSection.Address;
            this.port = ussdTcpListenerConfigurationSection.Port;
            this.sendBufferSize = ussdTcpListenerConfigurationSection.SendBufferSize;
            this.sendTimeout = ussdTcpListenerConfigurationSection.SendTimeout;
            this.receiveBufferSize = ussdTcpListenerConfigurationSection.ReceiveBufferSize;
            this.receiveTimeout = ussdTcpListenerConfigurationSection.ReceiveTimeout;
            this.name = ussdTcpListenerConfigurationSection.Name;
            this.packetTerminator = System.Text.Encoding.UTF8.GetBytes(ussdTcpListenerConfigurationSection.PacketTerminator);
            this.mobileNetwork = (MobileNetworks.MobileNetwork)Enum.Parse(typeof(MobileNetworks.MobileNetwork), ussdTcpListenerConfigurationSection.MobileNetwork, true);
            this.pingInterval = ussdTcpListenerConfigurationSection.PingInterval;
        }

        //internal static string Hex2Ascii(string HexString)
        //{
        //    string str = "";
        //    for (int i = 0; i < (HexString.Length - 1); i += 2)
        //    {
        //        str = str + Convert.ToString((char)Convert.ToByte(HexString.Substring(i, 2), 0x10));
        //    }
        //    return str;
        //}

        //internal static string ToHex(string Value)
        //{
        //    char[] chArray = Value.ToCharArray();
        //    string str = "";
        //    for (int i = 0; i < Value.Length; i++)
        //    {
        //        str = str + Convert.ToString((int)chArray[i], 0x10).PadLeft(2, '0');
        //    }
        //    return str;
        //}

        public void Start()
        {
            ConnectToServer(false);
        }

        public virtual void Stop()
        {
            listenerState = ListenerStates.Stopping;

            if (pingThread != null)
            {
                if (pingThreadHandle != null)
                    pingThreadHandle.Set();
                try
                {
                    pingThread.Join(5000);
                }
                catch (Exception)
                {
                }
                finally
                {
                    pingThread = null;
                }
            }

            if (tcpClientSocket != null)
            {
                tcpClientSocket.Close();
            }
            tcpClientSocket = null;

            connectWaitEvent.Set();
            listenerState = ListenerStates.Stopped;
            LogManager.LogStatus("{0} stopped", name);
        }
        #endregion

        #region Protected Methods
        protected void ConnectToServer(Boolean isReconnect)
        {
            if (!isReconnect)
                LogManager.LogStatus("{0} starting", name);
            if (tcpClientSocket != null)
            {
                if (tcpClientSocket.Connected) return;

                try
                {
                    tcpClientSocket.Close();
                }
                catch (Exception)
                {
                }
                finally
                {
                    tcpClientSocket = null;
                }
            }
            if (isReconnect && AutoReconnectWhenConnectionIsLost)
                LogManager.LogStatus("Connection lost to server {0}. Reconnecting", RemoteIPEndPoint.ToString());

            if ((!isReconnect || (isReconnect && AutoReconnectWhenConnectionIsLost)) && listenerState != ListenerStates.Stopped && listenerState != ListenerStates.Stopping)
            {
                listenerState = ListenerStates.Connecting;
                tcpClientSocket = new Socket(LocalIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tcpClientSocket.Blocking = false;
                tcpClientSocket.BeginConnect(RemoteIPEndPoint, new AsyncCallback(OnConnectedToServer), tcpClientSocket);
            }
        }

        protected void OnConnectedToServer(IAsyncResult asyncResult)
        {
            if (tcpClientSocket == null) return;
            try
            {
                tcpClientSocket.EndConnect(asyncResult);
                LogManager.LogStatus("{0} connected to {1}{2} ", name, address, (port == null ? String.Empty : ":" + port.Value.ToString()));
                if (tcpClientSocket.Connected)
                {
                    listenerState = ListenerStates.Running;
                    if (listenerState == ListenerStates.Running)
                        OnConnectedToServer(tcpClientSocket);
                }
                else
                    ConnectToServer(true);
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut && (listenerState != ListenerStates.Stopping || listenerState != ListenerStates.Stopped))
                {
                    int retryInterval = ConfigurationAppSettingsWrapper<int>.GetValue("Tcp Connection retry interval");
                    LogManager.LogError("Timed out connecting to {0}. Retrying", RemoteIPEndPoint.ToString());
                    connectWaitEvent.Reset();
                    connectWaitEvent.WaitOne(new TimeSpan(0, 0, 0, 0, retryInterval));
                    try
                    {
                        if (tcpClientSocket != null)
                            tcpClientSocket.BeginConnect(RemoteIPEndPoint, new AsyncCallback(OnConnectedToServer), tcpClientSocket);
                    }
                    catch (Exception)
                    {
                        if (listenerState != ListenerStates.Stopping || listenerState != ListenerStates.Stopped)
                            throw;
                    }
                }
                else if (se.SocketErrorCode == SocketError.ConnectionRefused && (listenerState != ListenerStates.Stopping || listenerState != ListenerStates.Stopped))
                {
                    int retryInterval = ConfigurationAppSettingsWrapper<int>.GetValue("Tcp Connection retry interval");
                    LogManager.LogError("Unable to connect to {0}. Retrying", RemoteIPEndPoint.ToString());
                    connectWaitEvent.Reset();
                    connectWaitEvent.WaitOne(new TimeSpan(0, 0, 0, 0, retryInterval));
                    try
                    {
                        if (tcpClientSocket != null)
                            tcpClientSocket.BeginConnect(RemoteIPEndPoint, new AsyncCallback(OnConnectedToServer), tcpClientSocket);
                    }
                    catch (Exception)
                    {
                        if (listenerState != ListenerStates.Stopping || listenerState != ListenerStates.Stopped)
                            throw;
                    }
                }
                else
                    LogManager.LogError(se);
            }
        }

        protected void WaitForData()
        {
            listenerState = ListenerStates.Running;
            //TCPReceiver tcpReceiver = new TCPReceiver(receiveBufferSize);
            //tcpClientSocket.BeginReceive(tcpReceiver.ReceiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveDataFromServer), tcpReceiver);

            TCPReceiver tcpReceiver = new TCPReceiver(receiveBufferSize);
            tcpReceiver.WaitHandle = new AutoResetEvent(true);
            do
            {
                try
                {
                    //LogManager.LogStatus("Waiting for new requests");
                    //Console.Write(".");
                    tcpClientSocket.BeginReceive(tcpReceiver.ReceiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveDataFromServer), tcpReceiver);
                    tcpReceiver.WaitHandle.Reset();
                    tcpReceiver.WaitHandle.WaitOne(this.receiveTimeout);

                }
                catch (USSDTcpReceiveTimeoutException) { }
                catch (Exception ex)
                {
                    LogManager.LogError(ex);
                }
            }
            while (listenerState == ListenerStates.Running);
        }

        protected void OnReceiveDataFromServer(IAsyncResult asyncResult)
        {
            if (listenerState != ListenerStates.Running) return;

            TCPReceiver tcpReceiver = asyncResult.AsyncState as TCPReceiver;
            SocketError errorCode;
            int bytesReceived = tcpClientSocket.EndReceive(asyncResult, out errorCode);

            byte[] tmp = new byte[bytesReceived];
            tmp = tcpReceiver.ReceiveBuffer;
            string payload = Encoding.ASCII.GetString(tmp);
            //LogManager.LogStatus(" ... " + payload);

            if (!tcpClientSocket.Connected)
            {
                listenerState = ListenerStates.Stopped;
                Boolean autoReconnectWhenConnectionIsLost = AutoReconnectWhenConnectionIsLost;
                OnServerDisconnected(tcpClientSocket, ref autoReconnectWhenConnectionIsLost);
                AutoReconnectWhenConnectionIsLost = autoReconnectWhenConnectionIsLost;
                ConnectToServer(true);
                return;
            }

            if (bytesReceived != 0)
            {               
                byte[] dataChunk = new byte[] { };
                byte[] dataOverflow = new byte[] { };
               
                tcpReceiver.AddReceiveData(tmp, bytesReceived);

                string recev = Encoding.ASCII.GetString(tcpReceiver.ReceiveData);
             
                while (tcpReceiver.ReceiveData.Contains((byte)255))
                {
                    for (int x = 0; x < tcpReceiver.ReceiveData.Length; x++)
                    {
                        if (tcpReceiver.ReceiveData[x] == (byte)255)
                        {                            
                            dataChunk = new byte[x];
                            Buffer.BlockCopy(tcpReceiver.ReceiveData, 0, dataChunk, 0, dataChunk.Length);
                            dataOverflow = new byte[tcpReceiver.ReceiveData.Length - dataChunk.Length - 1];
                            Buffer.BlockCopy(tcpReceiver.ReceiveData, x + 1, dataOverflow, 0, dataOverflow.Length);
                                                     
                            tcpReceiver.ReceiveData = dataOverflow;

                            if (dataChunk.Length > 0)
                            {
                                if (dataChunk[0] == 60)
                                {
                                    string tempDAta = Encoding.ASCII.GetString(dataChunk);
                                    OnDataReceived(dataChunk);
                                }
                                else
                                {
                                    Console.WriteLine("*** PROBLEM WITH PDU ****");
                                }
                            }
                            
                            break;
                        }
                    }
                }
                

                //tcpReceiver.AddReceiveData(tcpReceivere.Buffer, e.BytesTransferred);
                //if (tcpReceiver.ReceiveData.Contains((byte)255))
                //{
                //// remove the terminator
                //int terminatorIndex = 0;
                //Byte[] dataChunk = new Byte[] { };
                //Byte[] dataOverflow = new Byte[] { };
                //Boolean completePacket = false;
                //while (terminatorIndex <= tcpReceiver.ReceiveData.Length)
                //{
                    
                //    if (tcpReceiver.ReceiveData[terminatorIndex].Equals(this.packetTerminator))
                //    {
                //        completePacket = true;

                //        Byte[] copyBuffer = new Byte[tcpReceiver.ReceiveData.Length - dataChunk.Length-1];
                //        Buffer.BlockCopy(tcpReceiver.ReceiveData, terminatorIndex, copyBuffer, 0, tcpReceiver.ReceiveData.Length - dataChunk.Length-1);
                //        dataOverflow = copyBuffer;
                //        break;
                //    }
                //    else
                //    {
                //        Byte[] copyBuffer = new Byte[dataChunk.Length + 1];
                //        Buffer.BlockCopy(tcpReceiver.ReceiveData, 0, copyBuffer, 0, copyBuffer.Length);
                //        dataChunk = copyBuffer;

                //        copyBuffer = new Byte[tcpReceiver.ReceiveData.Length - dataChunk.Length];
                //        Buffer.BlockCopy(tcpReceiver.ReceiveData, terminatorIndex, copyBuffer, 0, tcpReceiver.ReceiveData.Length - dataChunk.Length);
                //        dataOverflow = copyBuffer;
                //    }
                    
                //    terminatorIndex++;
                //}

                //if (completePacket)
                //{
                //    Byte[] copyBuffer = new Byte[Math.Max(tcpReceiver.ReceiveData.Length - dataChunk.Length, 0)];
                //    if (copyBuffer.Length > 0)
                //        Buffer.BlockCopy(tcpReceiver.ReceiveData, terminatorIndex, copyBuffer, 0, Math.Max(tcpReceiver.ReceiveData.Length - dataChunk.Length, 0));

                //    OnDataReceived(dataChunk);

                //    tcpReceiver.ReceiveData = copyBuffer;
                //    tcpReceiver.ReceiveData = dataOverflow.Where(b => b != (byte)255).ToArray();
                //}
                //// Remove thye terminator
                //tcpReceiver.ReceiveData = tcpReceiver.ReceiveData.Where(b => b != (byte)255).ToArray();
                ////OnDataReceived(tcpReceiver.ReceiveData);
                ////tcpReceiver.Clear();

                if (tcpReceiver.WaitHandle != null)
                    tcpReceiver.WaitHandle.Set();
                //}
            }

            //if (listenerState == ListenerStates.Running)
            //{
            //    if (listenerState != ListenerStates.Running) return;

            //    if (tcpReceiver.ReceiveData.Length != 0 && tcpReceiver.ReceiveBuffer.Contains(this.packetTerminator))
            //        OnDataReceived(tcpReceiver.ReceiveData);
            //    tcpClientSocket.BeginReceive(tcpReceiver.ReceiveBuffer, 0, receiveBufferSize, SocketFlags.None, new AsyncCallback(OnReceiveDataFromServer), tcpReceiver);
            //}
        }

        protected abstract void OnDataReceived(Byte[] receiveData);

        protected virtual void OnServerDisconnected(Socket socket, ref Boolean autoReconnectWhenConnectionIsLost)
        {
            // This does not need to be implemented and is left blank
        }

        protected virtual void OnConnectedToServer(Socket socket)
        {
            // This does not need to be implemented and is left blank
        }

        protected Byte[] SendAndWaitForResponse(String sendData)
        {
            Send(sendData);

            TCPReceiver tcpReceiver = new TCPReceiver(this.receiveBufferSize);

            SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
            socketArgs = new SocketAsyncEventArgs();
            socketArgs.UserToken = tcpReceiver;
            socketArgs.SetBuffer(tcpReceiver.ReceiveBuffer, 0, tcpReceiver.ReceiveBuffer.Length);
            socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveComplete);

            bool isASyncReceive = tcpClientSocket.ReceiveAsync(socketArgs);

            tcpReceiver.WaitHandle = new AutoResetEvent(false);
            if (!isASyncReceive)
                OnReceiveComplete(this, socketArgs);

            if (!tcpReceiver.WaitHandle.WaitOne(new TimeSpan(0, 0, 0, 0, this.sendTimeout)))
                throw new USSDTcpReceiveTimeoutException("Socket receive timeout");

            //WERNER
            if (tcpReceiver.ReceiveData.Contains((byte)255))
            {
                tcpReceiver.ReceiveData = tcpReceiver.ReceiveData.Where(b => b != (byte)255).ToArray();
            }
            //END WERNER

            return tcpReceiver.ReceiveData;
        }

        protected void Send(String sendData)
        {
            TCPSubmitter tcpSubmitter = new TCPSubmitter(sendData, packetTerminator);

            SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
            socketArgs.UserToken = tcpSubmitter;

            socketArgs.SetBuffer(tcpSubmitter.SendBuffer, 0, tcpSubmitter.SendBuffer.Length);
            //socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendComplete);

            bool isASyncSend = tcpClientSocket.SendAsync(socketArgs);

            tcpSubmitter.WaitHandle = new AutoResetEvent(false);

            //if (!isASyncSend)
                OnSendComplete(this, socketArgs);

            if (!tcpSubmitter.WaitHandle.WaitOne(new TimeSpan(0, 0, 0, 0, this.sendTimeout)))
                throw new USSDTcpSendTimeoutException("Socket send timeout");
        }

        protected void SendResponse(String sendData)
        {
            Send(sendData);
            //TCPReceiver tcpReceiver = new TCPReceiver(this.sendBufferSize);

            //SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
            //socketArgs.UserToken = tcpReceiver;
            //socketArgs.SetBuffer(tcpReceiver.ReceiveBuffer, 0, this.sendBufferSize);
            //socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOComplete);

            //bool isASyncSend = tcpClientSocket.SendAsync(socketArgs);

            //tcpReceiver.WaitHandle = new AutoResetEvent(false);
            //if (isSyncSend)
            //    OnReceive(socketArgs);

            //if (!tcpReceiver.WaitHandle.WaitOne(new TimeSpan(this.sendTimeout)))
            //    throw new USSDTcpSendTimeoutException("Socket send timeout");

            //return tcpReceiver.ReceiveData;
        }

        //protected Byte[] AsyncWaitForData()
        //{
        //    TCPReceiver tcpReceiver = new TCPReceiver(this.receiveBufferSize);

        //    SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
        //    socketArgs.UserToken = tcpReceiver;
        //    socketArgs.SetBuffer(tcpReceiver.ReceiveBuffer, 0, this.receiveBufferSize);
        //    socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOComplete);

        //    bool isSyncSend = tcpClientSocket.ReceiveAsync(socketArgs);

        //    tcpReceiver.WaitHandle = new AutoResetEvent(false);
        //    if (isSyncSend)
        //        OnReceive(socketArgs);

        //    if (!tcpReceiver.WaitHandle.WaitOne(new TimeSpan(this.receiveTimeout)))
        //        throw new USSDTcpSendTimeoutException("Socket receive timeout");

        //    return tcpReceiver.ReceiveData;
        //}

        //protected void OnIOComplete(object sender, SocketAsyncEventArgs e)
        //{
        //    switch (e.LastOperation)
        //    {
        //        case SocketAsyncOperation.Receive:
        //            OnReceive(e);
        //            break;
        //        case SocketAsyncOperation.Send:
        //            OnSend(e);
        //            break;
        //        default:
        //            throw new ArgumentException("The last operation completed on the socket was not a receive or send");
        //    }
        //}

        protected void OnReceiveComplete(object sender, SocketAsyncEventArgs e)
        {
            TCPReceiver tcpReceiver = e.UserToken as TCPReceiver;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                tcpReceiver.AddReceiveData(e.Buffer, e.BytesTransferred);
                //if (tcpReceiver.ReceiveData.Contains(this.packetTerminator))
                //{
                // remove the terminator
                //tcpReceiver.ReceiveData = tcpReceiver.ReceiveData.Where(b => b != this.packetTerminator).ToArray();
                if (tcpReceiver.WaitHandle != null)
                    tcpReceiver.WaitHandle.Set();
                //}
            }
            else
            {
                Boolean autoReconnectWhenConnectionIsLost = AutoReconnectWhenConnectionIsLost;
                OnServerDisconnected(tcpClientSocket, ref autoReconnectWhenConnectionIsLost);
                AutoReconnectWhenConnectionIsLost = autoReconnectWhenConnectionIsLost;
                ConnectToServer(true);
            }
        }

        protected void OnSendComplete(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                try
                {
                    if (e.UserToken != null)
                    {
                        TCPSubmitter tcpSubmitter = e.UserToken as TCPSubmitter;
                        tcpSubmitter.WaitHandle.Set();
                    }   
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("******************** SOCKET PAYLOAD ERROR **********************");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("*****************  END SOCKET PAYLOAD ERROR **********************");
                    Console.WriteLine("");                   
                }
            }
            else
            {
                Boolean autoReconnectWhenConnectionIsLost = AutoReconnectWhenConnectionIsLost;
                OnServerDisconnected(tcpClientSocket, ref autoReconnectWhenConnectionIsLost);
                AutoReconnectWhenConnectionIsLost = autoReconnectWhenConnectionIsLost;
                ConnectToServer(true);
            }
        }

        protected void StartPingThread()
        {
            pingThreadHandle = new AutoResetEvent(false);
            pingThread = new Thread(new ThreadStart(delegate()
            {
                while (listenerState == ListenerStates.Running)
                {
                    if (OnPing != null)
                        OnPing(this, EventArgs.Empty);
                    pingThreadHandle.WaitOne(this.pingInterval);
                }
            }));

            pingThread.Start();
        }
        #endregion
    }
}
