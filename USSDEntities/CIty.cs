using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
    public class City
    {
        public int city_id { get; set; }
        public string city_name { get; set; }
        public int province_id { get; set; }
    }

    public class Cities
    {

        public int status { get; set; }
        public List<City> cities { get; set; }
    }
}
