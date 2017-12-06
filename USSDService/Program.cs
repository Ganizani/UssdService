using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using exactmobile.ussdservice.configuration;
using System.Configuration;
using exactmobile.ussdservice.common.menu;

namespace exactmobile.ussdservice
{
    class Program
    {
        private static AutoResetEvent waitEvent = new AutoResetEvent(false);
        private static USSDManager ussdManager;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCancelKeyPress);
            ussdManager = new USSDManager();
            ussdManager.Start();

            waitEvent.WaitOne();

            Console.WriteLine("Press <Enter> to close");
            Console.ReadLine();
        }

        static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            ussdManager.Stop();

            waitEvent.Set();

            e.Cancel = true;
        }
    }
}
