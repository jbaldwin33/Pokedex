using Pokedex.PkdxDatabase.Models;
using System.Collections.Generic;

namespace Pokedex.PokedexApp.ViewModels
{
    public class EVYieldViewModel : TabViewModel
    {
        private List<EVYield> evYields;

        public List<EVYield> EVYields
        {
            get => evYields;
            set => SetProperty(ref evYields, value);
        }

        public EVYieldViewModel() { }

        protected override void OnPokemonChanged(Pokemon pkmn) => EVYields = pkmn.EVYields;
    }
}
