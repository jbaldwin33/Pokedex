using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.Services
{
    public interface IPokedexProvider
    {
        Task<List<Pokemon>> GetAllPokemon();
    }
}
