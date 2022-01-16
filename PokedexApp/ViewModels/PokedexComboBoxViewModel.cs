using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.ObjectModel;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class PokedexComboBoxViewModel : ViewModel
    {
        public DexType PokedexType { get; set; }
        public ObservableCollection<Pokemon> DexList { get; set; }
        private Action<Pokemon, DexType> pokemonChanged;
        private Pokemon selectedPokemon;
        public Pokemon SelectedPokemon
        {
            get => selectedPokemon;
            set
            {
                SetProperty(ref selectedPokemon, value);
                if (value != null)
                    pokemonChanged?.Invoke(value, PokedexType);
            }
        }

        public PokedexComboBoxViewModel(DexType type, ObservableCollection<Pokemon> list, Action<Pokemon, DexType> changedAction)
        {
            PokedexType = type;
            DexList = list;
            pokemonChanged = changedAction;
        }
    }
}
