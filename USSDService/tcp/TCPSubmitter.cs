using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace exactmobile.ussdservice.tcp
{
    public class TCPSubmitter
    {
        #region Private Vars
        #endregion

        #region Properties
        public Byte[] SendBuffer { get; set; }
        public AutoResetEvent WaitHandle { get; set; }
        #endregion

        #region Public Methods
        public TCPSubmitter(String sendData, Byte[] packetTerminator)
        {
            //this.WaitHandle = new AutoResetEvent(false);
            Byte[] sendDataBytes = Encoding.ASCII.GetBytes(sendData);
            this.SendBuffer = new byte[sendData.Length + 1];
            Buffer.BlockCopy(sendDataBytes, 0, this.SendBuffer, 0, sendData.Length);          
            this.SendBuffer[this.SendBuffer.Length - 1] = 255;
                        
            //this.SendBuffer = new Byte[sendDataBytes.Length + 3];
            //this.SendBuffer[0] = 255;
            //Buffer.BlockCopy(sendDataBytes, 0, this.SendBuffer, 1, sendDataBytes.Length);
            //this.SendBuffer[this.SendBuffer.Length - 2] = 255;
            //this.SendBuffer[this.SendBuffer.Length - 1] = 255;
            ////Buffer.BlockCopy(packetTerminator, 0, this.SendBuffer, sendDataBytes.Length, packetTerminator.Length);
        }
        #endregion
    }
}
