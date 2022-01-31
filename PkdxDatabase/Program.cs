using Pokedex.PkdxDatabase.Context;
using System;
using System.Runtime.InteropServices;

namespace Pokedex.PkdxDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            PopulateDB.PopulateDatabase(context);
        }
    }
}
