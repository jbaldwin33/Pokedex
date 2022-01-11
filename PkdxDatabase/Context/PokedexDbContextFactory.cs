using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Pokedex.PkdxDatabase.Context
{
    public class PokedexDbContextFactory
    {
        private const string CONNECTION_STRING = "Data Source=|DataDirectory|pokedex.db;";
        private static readonly Lazy<PokedexDbContextFactory> lazy = new(() => new PokedexDbContextFactory());
        public static PokedexDbContextFactory Instance => lazy.Value;

        private PokedexDbContextFactory()
        {
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        public PokedexDBContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder().UseSqlite(CONNECTION_STRING).Options;
            return new PokedexDBContext(options);
        }
    }
}
