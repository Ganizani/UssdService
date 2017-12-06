using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.Entities
{
    public class Commune
    {
        public int commune_id { get; set; }
        public string commune_name { get; set; }
        public int city_id { get; set; }
    }

    public class Communes
    {
        public int status { get; set; }
        public List<Commune> communes { get; set; }
    }
}
