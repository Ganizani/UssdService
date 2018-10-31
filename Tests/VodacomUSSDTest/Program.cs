using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;

namespace VodacomUSSDTest
{
    class Program
    {
        static void Main(string[] args)
        {
             var  assemblyName = new exactmobile.ussdservice.processors.MutuelSmartUSSDProcessor(null, null).GetType().AssemblyQualifiedName;
            //var assemblyName = new exactmobile.ussdservice.processors.MutuelSmartUSSDProcessor(null, null).GetType().AssemblyQualifiedName;
            //Task t = new Task(DownloadPageAsync);
            //t.Start();
            Console.WriteLine("Downloading page...");
        
            // while (true)
            // {

            //netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\user
            // Console.Write("Enter Value: ");
            //String value = Console.ReadLine();
            //WebClient webClient = new WebClient();
            //Stream webStream = webClient.OpenWrite("http://localhost:8008/Vodacom/", "POST");
            //webStream.WriteTimeout = 900000000;

            //try
            //{
            //    String mobileNumber = "27829903201";
            //    String requestData = string.Format("<msg><msisdn>{0}</msisdn><sessionid>8275</sessionid><request type=\"1\">{1}</request></msg>", mobileNumber, value);
            //    Byte[] requestBytes = ASCIIEncoding.ASCII.GetBytes(requestData);
            //    webStream.Write(requestBytes, 0, requestBytes.Length);

            //    webStream.Flush();
            //}
            //finally
            //{
            //    webStream.Close();
            //}


            // }
            while (true)
            {
                // ... Target page.
                Console.Write("Enter Value: ");
                String value = Console.ReadLine();
                // ... Use HttpClient.
                using (HttpClient client = new HttpClient())
                //using (HttpResponseMessage response = await client.GetAsync(page))
                //using (HttpContent content = response.Content)
                {
                    client.BaseAddress = new Uri("http://127.0.0.1:8008/Vodacom/");
                    String mobileNumber = "243816489909";
                    String requestData = string.Format("<msg><msisdn>{0}</msisdn><sessionid>38679</sessionid><request type=\"1\">{1}</request></msg>", mobileNumber, value);
                    Byte[] requestBytes = ASCIIEncoding.ASCII.GetBytes(requestData);
                    var result = client.PostAsync(client.BaseAddress, new StringContent(requestData, Encoding.ASCII, "applicaton/xml")).Result;
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("done");
                        //value = Console.ReadLine();
                        //requestData = string.Format("<msg><msisdn>{0}</msisdn><sessionid>8275</sessionid><request type=\"1\">{1}</request></msg>", mobileNumber, value);
                        //requestBytes = ASCIIEncoding.ASCII.GetBytes(requestData);
                        //result = await client.PostAsync(client.BaseAddress, new StringContent(requestData, Encoding.ASCII, "applicaton/xml"));
                    }


                }
            }
            Console.ReadLine();
        }

        static async void DownloadPageAsync()
        {
           
        }
    }
}
/*
namespace VodacomUSSDTest
{
    class Program

    {
        static void Main(string[] args)
        {
            string defaultAddress = "http://192.168.44.17:8100/vodacom/";
            string defaultMobileNumber = "27832122712";
            string mobileNumber = null;

            Dictionary<string, string> services = new Dictionary<string, string>();
            services.Add("Shapa","*120*33337#");
            services.Add("The Peoples Gospel","*120*41514#");
            services.Add("Awesome Tones","*120*42525#");
            services.Add("Awesome Games", "*120*33332#");
            services.Add("SuperM", "*120*4567#");
                     	                        
            while (true)
            {
                //Getting assembly qualified name
                // var assemblyName = new exactmobile.ussdservice.processors.SuperM.SuperMUSSDProcessors(null, null).GetType().AssemblyQualifiedName;

                if (mobileNumber == null)
                {
                    Console.Write(string.Format("MobileNumber (leave empty for default {0}): ",defaultMobileNumber));
                    mobileNumber = Console.ReadLine();

                    if (string.IsNullOrEmpty(mobileNumber))
                        mobileNumber = defaultMobileNumber;

                    Console.WriteLine("Please choose a service or enter any USSD address: ");

                    int counter = 0;
                    foreach(var item in services)
                    {                                                
                        Console.WriteLine(string.Format("{0}. {1,-25} {2}",++counter,item.Key, item.Value));                        
                    }

                    //Console.Write("serviceID / Address: ");
                    string value = Console.ReadLine();

                    try
                    {
                        int index = int.Parse(value) - 1;

                        value = services.ElementAt(index).Value;
                    }
                    catch
                    {

                    }
                                            
                    Post(defaultAddress, mobileNumber, value);

                    continue;
                }

                Console.Clear();
                Console.Write("Enter Value: ");
                
                Post(defaultAddress, mobileNumber, Console.ReadLine());
                                                            
            }


        }

        private static void Post(string defaultAddress, string mobileNumber, string value)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream webStream = webClient.OpenWrite(defaultAddress, "POST");
                webStream.WriteTimeout = Int32.MaxValue;

                try
                {
                    String requestData = string.Format("<msg><msisdn>{0}</msisdn><sessionid>8275</sessionid><request type=\"1\">{1}</request></msg>", mobileNumber, value);
                    Byte[] requestBytes = ASCIIEncoding.ASCII.GetBytes(requestData);
                    webStream.Write(requestBytes, 0, requestBytes.Length);

                    webStream.Flush();
                }
                finally
                {
                    webStream.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
*/
