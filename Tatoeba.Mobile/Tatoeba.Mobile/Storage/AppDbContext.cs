using Microsoft.EntityFrameworkCore;
using System;
using Tatoeba.Mobile.Models;

namespace Tatoeba.Mobile.Storage
{
    public class AppDbContext : DbContext
    {
        public DbSet<Language> Languages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = PCLStorage.FileSystem.Current.LocalStorage.Path + @"\" + String.Format("{0}.db3", "database");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}
