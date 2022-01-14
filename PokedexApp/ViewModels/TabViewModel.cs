using MVVMFramework.ViewModels;
using MVVMFramework.ViewNavigator;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.ViewModels
{
    public abstract class TabViewModel : ViewModel
    {
        protected MainViewModel mainViewModel;
        protected abstract void OnPokemonChanged(Pokemon pkmn);

        public override void OnLoaded()
        {
            mainViewModel = Navigator.Instance.MainViewModel as MainViewModel;
            mainViewModel.PokemonChangedAction += OnPokemonChanged;
            if (mainViewModel.SelectedPokemon != null)
                OnPokemonChanged(mainViewModel.SelectedPokemon);
            base.OnLoaded();
        }
    }
}
