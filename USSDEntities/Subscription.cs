using System;


namespace USSD.Entities
{
    

public class Subscription
    {
        public int subscription_id { get; set; }
        public string mobile_number { get; set; }
       // public string createdDate { get; set; }
        public string first_billing_date { get; set; }
        public string next_billing_date { get; set; }
        public string ref_number { get; set; } = "32222";
        public string reference_number { get; set; }
        public int status_id { get; set; }
        public int network_id { get; set; }
        public string customer_name { get; set; } = "Dasso";
        public int mutuel_id { get; set; }
        public int subscription_service_id { get; set; }

    }

    public class Subscriptions
    {
        public int status { get; set; }
        public Subscription subscription { get; set; }
    }
}
