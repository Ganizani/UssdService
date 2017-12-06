using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using exactmobile.components.logging;
using exactmobile.common;
using System.IO;
using exactmobile.ussdservice.exceptions;
using exactmobile.ussdservice.handlers;
using exactmobile.ussdservice.configuration.http;
using System.Reflection;
using exactmobile.common.Lookups;
using exactmobile.ussdservice.common.handlers;

namespace exactmobile.ussdservice.http
{
    public class USSDHttpListener
    {
        #region Enums
        private enum ListenerStates
        {
            Stopping,
            Stopped,
            Running
        }
        #endregion
        
        #region Private Vars
        private AutoResetEvent httpListenerWaitHandle;
        private HttpListener httpListener;
        private MobileNetworks.MobileNetwork mobileNetwork;
        private ListenerStates listenerState = ListenerStates.Stopped;
        #endregion

        #region Public
        public static USSDHttpListener Start(USSDHttpListenerConfigurationSection ussdHttpListenerConfiguration)
        {
            USSDHttpListener result = new USSDHttpListener();

            result.httpListenerWaitHandle = new AutoResetEvent(false);               
            result.httpListener = new HttpListener();
            result.mobileNetwork = (MobileNetworks.MobileNetwork)Enum.Parse(typeof(MobileNetworks.MobileNetwork), ussdHttpListenerConfiguration.MobileNetwork, true);

            String address = ussdHttpListenerConfiguration.Address.Trim();
            if (address.Substring(address.Length - 1) != "/")
                address += "/";

            result.httpListener.Prefixes.Add(address);

            try
            {
                result.httpListener.Start();
                LogManager.LogStatus("Creating {0} on {1}", ussdHttpListenerConfiguration.Name, ussdHttpListenerConfiguration.Address);               

                IAsyncResult asyncListener;
                for (int listenerIndex = 1; listenerIndex <= ussdHttpListenerConfiguration.NumberOfListeners; listenerIndex++)
                    asyncListener = result.httpListener.BeginGetContext(new AsyncCallback(result.OnHttpRequestReceived), result.httpListener);

                LogManager.LogStatus("{0} USSD Http Listener/s started for {1}", ussdHttpListenerConfiguration.NumberOfListeners, ussdHttpListenerConfiguration.Name);

                new Thread(new ThreadStart(delegate()
                {
                    result.listenerState = ListenerStates.Running;
                    result.httpListenerWaitHandle.WaitOne();
                    result.httpListener.Stop();
                    LogManager.LogStatus("{0} stopped", ussdHttpListenerConfiguration.Name);
                })).Start();

            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
                result.httpListenerWaitHandle.Set();
            }

            return result;
        }

        public void Stop()
        {
            listenerState = ListenerStates.Stopping;
            httpListenerWaitHandle.Set();
            httpListener.Stop();
            listenerState = ListenerStates.Stopped;
        }
        #endregion

        #region public Methods
        protected void OnHttpRequestReceived(IAsyncResult asyncResult)
        {
            if (listenerState != ListenerStates.Running) return;

            HttpListener httpListener = (HttpListener)asyncResult.AsyncState;

            //Console.WriteLine(DateTime.Now.ToString() + " Request received");

            if (httpListener.IsListening)
            {
                HttpListenerResponse response = null;
                try
                {
                    // Call EndGetContext to complete the asynchronous operation.
                    HttpListenerContext context = httpListener.EndGetContext(asyncResult);
                    HttpListenerRequest request = context.Request;
                    response = context.Response;

                    String requestData = GetRequestData(request);

                    if (requestData.Length != 0)
                    {
                        USSDHandlerRequestType.RequestTypes requestType = USSDHandlerRequestType.GetRequestType(request.HttpMethod);
                        IUSSDHandler ussdHandler = USSDHandlerFactory.GetHandler(mobileNetwork, requestData, requestType);
                        Boolean isTimeout = false;
                        Boolean isInvalid = false;
                        
                        ussdHandler.Initialize(requestData, requestType, out isTimeout, out isInvalid);

                        if (!isTimeout)
                        {
                            LogManager.LogStatus("Handler {0} found for request", ussdHandler.HandlerID);
                            String responseData = ussdHandler.ProcessRequest(requestData);
                            WriteResponse(responseData, response);
                        
                        }
                        else
                            LogManager.LogStatus("Timout request for handler {0} received", ussdHandler.HandlerID);

                    }
                    else
                        WriteResponse("Invalid/Empty Request", response);
                }
                catch (Exception ex)
                {

                    LogManager.LogError(ex);
                    if (response != null)
                        WriteResponse(ex.Message, response);
                }
                finally
                {
                    httpListener.BeginGetContext(new AsyncCallback(OnHttpRequestReceived), httpListener);
                }
            }
        }

        protected String GetRequestData(HttpListenerRequest httpListenerRequest)
        {   
            if (httpListenerRequest.HttpMethod.ToUpper() == "GET")
                return httpListenerRequest.RawUrl;
            else if (httpListenerRequest.HttpMethod.ToUpper() == "POST")
            {
                Stream requestInputStream = httpListenerRequest.InputStream;
                try
                {
                    StreamReader reader = new StreamReader(requestInputStream, httpListenerRequest.ContentEncoding);
                    try
                    {
                        return reader.ReadToEnd();
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
                finally
                {
                    requestInputStream.Close();
                }
            }
            return String.Empty;
        }

        protected void WriteResponse(String responseData, HttpListenerResponse httpListenerResponse)
        {
            try
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseData);
                httpListenerResponse.ContentLength64 = buffer.Length;
                System.IO.Stream output = httpListenerResponse.OutputStream;
                try
                {
                    output.Write(buffer, 0, buffer.Length);
                }
                finally
                {
                    // You must close the output stream.
                    output.Close();
                    //LogManager.LogStatus("Response Sent");
                    LogManager.LogStatus("--> " + responseData);
                }
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex);
            }
        }
        #endregion
    }
}
