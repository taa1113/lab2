using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class EventsContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<CPE> CPEs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<SourceOfFinancing> SourcesOfFinancing { get; set; }
        public DbSet<PlannedEvent> PlannedEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigurationBuilder builder = new();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            IConfigurationRoot config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("SqlConnection");
            _ = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;
            optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
        }

    }

}
