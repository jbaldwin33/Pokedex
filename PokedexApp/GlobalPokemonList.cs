using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp
{
    public class GlobalPokemonList
    {
        private static readonly Lazy<GlobalPokemonList> lazy = new Lazy<GlobalPokemonList>(() => new GlobalPokemonList());
        public static GlobalPokemonList Instance => lazy.Value;


        private GlobalPokemonList() { }

    }
}
