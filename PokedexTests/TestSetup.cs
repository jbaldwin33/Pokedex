using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokedexTests
{
    public class TestSetup
    {
        public List<Pokemon> PokemonList { get; set; }
        public TestSetup()
        {
            PokemonList = Pokedex.PokedexApp.Services.PokedexProvider.Instance.GetAllPokemon().Result;
        }
    }
}
