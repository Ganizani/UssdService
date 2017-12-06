using System;
using System.Collections.Generic;
using System.Text;

using exactmobile.ussdcommon.utility;
namespace XMLRPCTest
{
    public class Program
    {
        static void Main(string[] args)
        {                      
            int balanceAmount;
            int serverTransaction;
            int resultCode;
            string resultMessage;
            bool hasRetrieved;
            string creditCardNumber = "5412345";
            var transactionID = "4598741";
            DateTime expDate;
          
            hasRetrieved = CommonUtility.IsCreditCardValid(creditCardNumber);
        }
    }
}
