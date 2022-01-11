using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace Pokedex.PkdxDatabase.Context
{
    public class PokedexDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PokedexDBContext>
    {
        public PokedexDBContext CreateDbContext(string[] args)
        {
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            var options = new DbContextOptionsBuilder().UseSqlite("Data Source=|DataDirectory|pokedex.db;").Options;
            return new PokedexDBContext(options);
        }
    }
}
