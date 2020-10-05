using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

namespace APItalker
{

    class Program
    {
        static void Main(string[] args)
        {
            //Начальные настройки
            Console.Title = "Клиент к API countries";
            Console.ForegroundColor = ConsoleColor.Magenta;
            bool start = true;
            List<Countries> ListCouintries = new List<Countries>();

            //Основной цикл программы
            while (start)
            {
                Console.WriteLine(
                    "     ВЫБЕРИТЕ ДЕЙСТВИЕ:\n\n" +
                    "1. Поискать информацию о стране\n" +
                    "2. Вывести страны, добавленные в БД" +
                    "\n3. Выход"
                    );
                if (int.TryParse(Console.ReadLine(), out int result))
                {

                    switch (result)
                    {
                        case 1:
                            {
                                Console.Clear();
                                Console.WriteLine("Введите название страны:");
                                ListCouintries = GetListCountries(Console.ReadLine());
                                if (!(ListCouintries is null))
                                {
                                    PrintListCountries(ListCouintries);
                                }
                                Console.ReadKey();
                                break;
                            }
                        case 2:
                            {
                                var countries = GetCountriesFromDb();
                                foreach (TCountry item in countries)
                                {
                                    Console.WriteLine($"id: {item.id}\nname: {item.name}\narea:{item.area}\ncode:{item.code}\npopulation:{item.population}\ncapital:{item.TCity.name}\nregion:{item.TRegion.name}\n----------------");
                                }
                                Console.ReadKey();
                                break;
                            }
                        case 3:
                            {
                                start = false;
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }


                }
                Console.Clear();

            }



        }
        //Метод для получения списка стран
        public static List<Countries> GetListCountries(string countryName)
        {
            //Получаем json в переменную response
            string response;
            string url;
            url = "https://restcountries.eu/rest/v2/name/" + countryName;
            try
            {
                HttpWebRequest httpWeberRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWeberResponse = (HttpWebResponse)httpWeberRequest.GetResponse();
                using (StreamReader sr = new StreamReader(httpWeberResponse.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
                //Десериализовываем пришедший массив json в список объектов
                List<Countries> listCountries = JsonConvert.DeserializeObject<List<Countries>>(response);
                Console.Clear();
                return listCountries;
            }
            //В случае ошибки выводим статус код
            catch (WebException ex)
            {

                Console.WriteLine($"Ошибка: {(int)((HttpWebResponse)ex.Response).StatusCode}");
                return null;

            }

        }

        //Метод для вывода стран из списка и занесение в БД
        public static void PrintListCountries(List<Countries> ListCountries)
        {
            //Вывод найденных стран
            int ListCounter = 1;
            foreach (Countries country in ListCountries)
            {
                Console.WriteLine($"Страна № {ListCounter}:\n{country}");
                ListCounter++;
            }
            //Добавление страны
            Console.WriteLine($"Количество найденных стран по запросу: {ListCountries.Count}");
            Console.WriteLine("Введите номер страны, которую хотите добавить:");
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                if (result <= ListCountries.Count && result != 0) //если ввели корректный номер
                {
                    Console.Clear();
                    using (MyDbContext cont = new MyDbContext())
                    {
                        TRegion region = new TRegion()
                        {
                            name = ListCountries[result - 1].region
                        };

                        TCity city = new TCity()
                        {
                            name = ListCountries[result - 1].capital
                        };
                        //Проверяем есть ли найденный регион и столица уже в БД. если нет добавляем. если да получаем их id
                        int idRegion = 0;
                        int idCity = 0;
                        bool check = true;
                        List<TRegion> regions = GetRegionsFromDb();
                        List<TCity> cities = GetCitiesFromDb();
                        List<TCountry> countries = GetCountriesFromDb();
                        foreach (TRegion item in regions)
                        {
                            if (region.name == item.name)
                            {
                                idRegion = item.id;
                                break;
                            }

                        }
                        foreach (TCity item in cities)
                        {
                            if (city.name == item.name)
                            {
                                idCity = item.id;
                                break;
                            }
                        }
                        if (idRegion == 0)
                        {
                            cont.Regions.Add(region);
                            cont.SaveChanges();
                            idRegion = region.id;
                        }
                        if (idCity == 0)
                        {
                            cont.Cities.Add(city);
                            cont.SaveChanges();
                            idCity = city.id;

                        }

                        //Проверям наличие страны по уникальному коду страны со всеми странами из бд
                        foreach (TCountry item in countries)
                        {
                            if (ListCountries[result - 1].alpha3Code == item.code) //если найдена страна то обновляем информацию
                            {
                                var UpdateCountry = cont.Countries
                                    .Where(c => c.code == item.code)
                                    .FirstOrDefault();
                                UpdateCountry.name = ListCountries[result - 1].name;
                                UpdateCountry.area = ListCountries[result - 1].area;
                                UpdateCountry.population = ListCountries[result - 1].population;
                                cont.SaveChanges();
                                Console.WriteLine($"{ListCountries[result - 1].name} обновлена!");
                                check = false;
                                break;
                            }
                        }
                        //если не найдена страна, то добавляем в БД
                        if (check)
                        {
                            var country = new TCountry()
                            {
                                name = ListCountries[result - 1].name,
                                code = ListCountries[result - 1].alpha3Code,
                                area = ListCountries[result - 1].area,
                                population = ListCountries[result - 1].population,
                                TRegionID = idRegion,
                                TCityID = idCity
                            };
                            cont.Countries.Add(country);
                            cont.SaveChanges();
                            Console.WriteLine($"{ListCountries[result - 1].name} добавлена в базу данных!");
                        }

                    }

                }
                else //если ввели номер за диапазоном
                {
                    Console.Clear();
                    Console.WriteLine("Нет страны под таким номером!");
                }
            }
            else //если ввели невалидное значение
            {
                Console.Clear();
                Console.WriteLine("Введено не число!");
            }

        }

        #region Метод для получения списка стран из базы данных
        private static List<TCountry> GetCountriesFromDb()
        {
            Console.Clear();
            var context = new MyDbContext();
            List<TCountry> countries = context.Countries.ToList();
            return countries;
        }
        #endregion
        #region Метод для получения списка городов из базы данных
        private static List<TCity> GetCitiesFromDb()
        {
            var context = new MyDbContext();
            List<TCity> cities = context.Cities.ToList();
            return cities;
        }
        #endregion
        #region Метод для получения списка регионов из базы данных
        private static List<TRegion> GetRegionsFromDb()
        {
            var context = new MyDbContext();
            List<TRegion> regions = context.Regions.ToList();
            return regions;
        }
        #endregion
    }
}
