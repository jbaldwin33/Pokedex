using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Pokedex.PkdxDatabase.Models;
using System.IO;
using Pokedex.PkdxDatabase.Entities;

namespace Pokedex.PkdxDatabase.Context
{
    public class PokedexDBContext : DbContext
    {
        public PokedexDBContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<PokedexClassEntity> PokedexEntries { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;initial catalog=Pokedex;Trusted_Connection=true;");

    }
}
