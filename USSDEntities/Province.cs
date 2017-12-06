using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
    public class Province
    {
        public int province_id { get; set; }
        public string province_name { get; set; }
    }

    public class Provinces
    {
        public int status { get; set; }
        public int message { get; set; }
        public Province data { get; set; }
    }
}
