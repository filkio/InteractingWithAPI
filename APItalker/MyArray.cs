using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Класс для описания стран. JSON объект десериализуется в объект этого класса.
namespace APItalker
{
    public class Countries
    {
        //Свойства класса СТРАНЫ
        public string name { get; set; }
        public string alpha3Code { get; set; }
        public string capital { get; set; }
        public double? area { get; set; }
        public int? population { get; set; }
        public string region { get; set; }

        //Переопределение метода ToString для СТРАН
        public override string ToString()
        {
            return $"Название страны: {name}.\n" +
                $"Код страны: {alpha3Code}.\n" +
                $"Столица: {capital}.\n" +
                $"Площадь: {area}.\n" +
                $"Население: {population}\n" +
                $"Регион: {region}\n" +
                $"------------------\n";
        }

    }
}
