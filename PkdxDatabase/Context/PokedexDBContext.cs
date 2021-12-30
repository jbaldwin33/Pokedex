using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Pokedex.PkdxDatabase.Models;

namespace Pokedex.PkdxDatabase.Context
{
  public class PokedexDBContext : DbContext
  {
    public DbSet<PokedexClass> PokedexEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;initial catalog=Pokedex;Trusted_Connection=true;");

  }
}
