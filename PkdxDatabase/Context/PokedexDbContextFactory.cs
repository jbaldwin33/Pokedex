using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Pokedex.PkdxDatabase.Context
{
    public class PokedexDbContextFactory
    {
        private const string CONNECTION_STRING = "Data Source=pokedex.db;";
        private static readonly Lazy<PokedexDbContextFactory> lazy = new(() => new PokedexDbContextFactory());
        public static PokedexDbContextFactory Instance => lazy.Value;

        private PokedexDbContextFactory()
        {
            //var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "Binaries"));

            //if (Directory.Exists(path))
            //    AppDomain.CurrentDomain.SetData("DataDirectory", path);
            //else
            //{
            //    Console.WriteLine("Binary directory doesn't exist. Run CSV Creator first to create the CSV file. Press \"Enter\" to close");
            //    Console.ReadLine();
            //    Environment.Exit(0);
            //}
        }

        public PokedexDBContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder().UseSqlite(CONNECTION_STRING).Options;
            return new PokedexDBContext(options);
        }
    }
}
