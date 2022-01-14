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
    public class EVYieldViewModel : ViewModel
    {
        private MainViewModel mainViewModel;
        private List<EVYield> evYields;

        public List<EVYield> EVYields
        {
            get => evYields;
            set => SetProperty(ref evYields, value);
        }

        private string evStat;
        private int evYield;

        public string EVStat
        {
            get => evStat;
            set => SetProperty(ref evStat, value);
        }

        public int EVYield
        {
            get => evYield;
            set => SetProperty(ref evYield, value);
        }

        public EVYieldViewModel()
        {

        }

        public override void OnLoaded()
        {
            mainViewModel = Navigator.Instance.MainViewModel as MainViewModel;
            mainViewModel.PokemonChangedAction += OnPokemonChanged;
            if (mainViewModel.SelectedPokemon != null)
                OnPokemonChanged(mainViewModel.SelectedPokemon);
            base.OnLoaded();
        }

        private void OnPokemonChanged(Pokemon pkmn)
        {
            EVYields = pkmn.EVYields;
        }
    }
}
