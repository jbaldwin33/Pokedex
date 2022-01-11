using Microsoft.EntityFrameworkCore;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.Services
{
    public class PokedexProvider : IPokedexProvider
    {
        private static readonly Lazy<PokedexProvider> lazy = new Lazy<PokedexProvider>(() => new PokedexProvider());
        public static PokedexProvider Instance => lazy.Value;
        private PokedexProvider() { }

        public async Task<List<PokedexClass>> GetAllPokemon()
        {
            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            return await context.PokedexEntries.ToListAsync();
        }
    }
}
