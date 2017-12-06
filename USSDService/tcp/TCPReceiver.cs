using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace exactmobile.ussdservice.tcp
{
    public class TCPReceiver
    {
        #region Private Vars
        #endregion

        #region Properties
        public Byte[] ReceiveBuffer { get; set; }
        public int BufferSize { get; set; }
        public Byte[] ReceiveData { get; set; }
        private AutoResetEvent waitHandle;

        public AutoResetEvent WaitHandle
        {
            get { return this.waitHandle; }
            set { this.waitHandle = value; }
        }
        #endregion

        #region Public Methods
        public TCPReceiver(int BufferSize)
        {
            this.BufferSize = BufferSize;
            this.ReceiveBuffer = new Byte[BufferSize];
            this.ReceiveData = new Byte[] { };            
        }

        public void AddReceiveData(Byte[] data, int bytesReceived)
        {
            // Byte[] copyBuffer = new Byte[ReceiveData.Length + bytesReceived-1];
            //WERNER 
            Byte[] copyBuffer = new Byte[ReceiveData.Length + bytesReceived];
            Buffer.BlockCopy(ReceiveData, 0, copyBuffer, 0, ReceiveData.Length);
            // Buffer.BlockCopy(data, 0, copyBuffer, ReceiveData.Length, bytesReceived-1);
            //WERNER
            Buffer.BlockCopy(data, 0, copyBuffer, ReceiveData.Length, bytesReceived);
            this.ReceiveData = new Byte[copyBuffer.Length];
            Buffer.BlockCopy(copyBuffer, 0, ReceiveData, 0, copyBuffer.Length);
            //ClearBuffer();
        }

        
        public void Clear()
        {
            ClearBuffer();
            this.ReceiveData = new Byte[] { };
        }

        public void ClearBuffer()
        {
            this.ReceiveBuffer = new byte[this.BufferSize];
        } 
        #endregion
    }
}
