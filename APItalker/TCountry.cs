using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//для codefirst взаимодействия с бд. Таблица страны.
namespace APItalker
{
    public class TCountry
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public double? area { get; set; }
        public int? population { get; set; }
        public int TRegionID { get; set; }
        public int TCityID { get; set; }

        public virtual TCity TCity { get; set; }
        public virtual TRegion TRegion { get; set; }

    }
}
