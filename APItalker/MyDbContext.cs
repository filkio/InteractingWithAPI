using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Контекст для взаимодействия с БД
namespace APItalker
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("ConnectionString")
        {

        }
        public DbSet<TCountry> Countries { get; set; }
        public DbSet<TRegion> Regions { get; set; }
        public DbSet<TCity> Cities { get; set; }
    }
}
