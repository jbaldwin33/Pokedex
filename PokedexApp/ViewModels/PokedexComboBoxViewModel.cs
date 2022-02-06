using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.ObjectModel;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class PokedexComboBoxViewModel : ViewModel
    {
        private Action<Pokemon, DexType> pokemonChanged;
        private Pokemon selectedPokemon;
        private bool withoutNotify;
        public DexType PokedexType { get; set; }
        public ObservableCollection<Pokemon> DexList { get; set; }

        public Pokemon SelectedPokemon
        {
            get => selectedPokemon;
            set
            {
                SetProperty(ref selectedPokemon, value);
                if (value != null && !withoutNotify)
                    pokemonChanged?.Invoke(value, PokedexType);
                withoutNotify = false;
            }
        }

        public PokedexComboBoxViewModel(DexType type, ObservableCollection<Pokemon> list, Action<Pokemon, DexType> changedAction)
        {
            PokedexType = type;
            DexList = list;
            pokemonChanged = changedAction;
        }

        public void UpdateComboboxWithoutNotify(Pokemon pkmn)
        {
            withoutNotify = true;
            SelectedPokemon = pkmn;
        }
    }
}
