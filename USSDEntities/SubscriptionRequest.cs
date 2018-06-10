using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
   public class SubscriptionRequest
    {
        public int mut { get; set; }
        public string phonnum { get; set; }
        public string refnumbr { get; set; }
        public int status { get; set; }
        public string datecrt { get; set; }
        public string idsender { get; set; }
        public string idcenter { get; set; }
        public int subscriptionid { get; set; }
    }
}
