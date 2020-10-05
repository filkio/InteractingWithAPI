using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//для codefirst взаимодействия с бд. Таблица столицы.
namespace APItalker
{
    public class TCity
    {
        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<TCountry> TCountry { get; set; }

    }
}
