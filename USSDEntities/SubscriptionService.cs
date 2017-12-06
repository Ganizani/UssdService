using System;


namespace USSD.Entities
{
    public class Pricing
    {
        public int id { get; set; }
        public string product_name { get; set; }
        public int price { get; set; }
    }

    public class SubscriptionService
    {
        public int subscription_service_id { get; set; }
        public string description { get; set; }
        public string display_name { get; set; }
        public string keyword { get; set; }
        public int ussd_number { get; set; }
        public int short_number { get; set; }
        public int status_id { get; set; }
        public int payment_method_id { get; set; }
        public int unsub_sms_number { get; set; }
        public string unsub_keyword { get; set; }
        public string prs_rate { get; set; }
        public Pricing pricing { get; set; }
    }

    public class SubscriptionServices
    {
        public int status { get; set; }
        public SubscriptionService subscriptionService { get; set; }
    }
}
