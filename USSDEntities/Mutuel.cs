using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
    public class Mutuel
    {
        public int mutuel_id { get; set; }
        public string mutuel_name { get; set; }
        public string mutuel_description { get; set; }
        public int commune_id { get; set; }

        public decimal price { get; set; }
    }

    public class Mutuels
    {
        public int status { get; set; }
        public List<Mutuel> mutuels { get; set; }
    }
}
