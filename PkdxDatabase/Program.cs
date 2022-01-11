using Pokedex.PkdxDatabase.Context;

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
