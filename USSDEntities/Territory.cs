using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
  
        public class Territory
        {
            public int territory_id { get; set; }
            public string territory_location { get; set; }
            public int province_id { get; set; }
        }

        public class Territories
        {
            public int status { get; set; }
            public List<Territory> territories { get; set; }
        }
     
    
}
