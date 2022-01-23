using Pokedex.PkdxDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.PokedexApp.ViewModels
{
    public class EVYieldViewModel : TabViewModel
    {
        private IEnumerable<EVYield> evYields;

        public IEnumerable<EVYield> EVYields
        {
            get => evYields;
            set => SetProperty(ref evYields, value);
        }

        protected override void OnPokemonChanged(Pokemon pkmn) => EVYields = pkmn.EVYields.Where(x => x.Value != 0);
    }
}
