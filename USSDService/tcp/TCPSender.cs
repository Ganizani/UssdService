using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdservice.tcp
{
    public class TCPSender
    {
        #region Private Vars
        #endregion

        #region Properties
        public Byte[] ReceiveBuffer { get; set; }
        public int BufferSize { get; set; }
        public Byte[] ReceiveData { get; private set; }
        #endregion

        #region Public Methods
        public TCPSender(int BufferSize)
        {
            this.BufferSize = BufferSize;
            this.ReceiveBuffer = new byte[BufferSize];
            this.ReceiveData = new Byte[] { };
        }

        public void AddReceiveData(Byte[] data)
        {
            Buffer.BlockCopy(this.ReceiveData, this.ReceiveData.Length, data, 0, data.Length);
            ClearBuffer();
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
