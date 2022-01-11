using MVVMFramework.ViewModels;
using MVVMFramework.ViewNavigator;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public INavigator Navigator { get; set; }
        public event PokemonChanged PokemonChangedEvent;
        public delegate void PokemonChanged(object sender, PokemonChangedEventArgs e);
        private PokedexClass selectedPokemon;
        private int currentID;
        private ObservableCollection<PokedexClass> pokemonList;
        private ObservableCollection<PokedexForm> formCollection;
        private List<PokedexClass> pokemonListWithForms;
        private RelayCommand findCommand;
        private RelayCommand nextCommand;
        private RelayCommand previousCommand;

        public PokedexClass SelectedPokemon
        {
            get => selectedPokemon;
            set
            {
                SetProperty(ref selectedPokemon, value);
                FindCommandExecute();
            }
        }

        public int CurrentID
        {
            get => currentID;
            set => SetProperty(ref currentID, value);
        }

        public ObservableCollection<PokedexClass> PokemonList
        {
            get => pokemonList;
            set => SetProperty(ref pokemonList, value);
        }

        public ObservableCollection<PokedexForm> FormCollection
        {
            get => formCollection;
            set => SetProperty(ref formCollection, value);
        }

        public MainViewModel(INavigator navigator)
        {
            Navigator = navigator;
            GetAllPokemon();
            PokemonList = new ObservableCollection<PokedexClass>(pokemonListWithForms.Where(x => x.Num == Math.Floor(x.Num)));
            FormCollection = new ObservableCollection<PokedexForm>();
        }

        #region Commands

        public RelayCommand FindCommand => findCommand ??= new RelayCommand(FindCommandExecute);
        public RelayCommand NextCommand => nextCommand ??= new RelayCommand(NextCommandExecute, () => SelectedPokemon?.Num < 649);
        public RelayCommand PreviousCommand => previousCommand ??= new RelayCommand(PreviousCommandExecute, () => SelectedPokemon?.Num > 1);

        #endregion

        public bool PokemonHasForms(PokedexClass pkmn) => pokemonListWithForms.Any(x => x.Num != pkmn.Num && Math.Truncate(x.Num) == pkmn.Num);
        public IEnumerable<PokedexClass> GetPokemonForms(PokedexClass pkmn) => pokemonListWithForms.Where(x => x.Num != pkmn.Num && Math.Truncate(x.Num) == pkmn.Num);

        private async void GetAllPokemon()
        {
            pokemonListWithForms = await PokedexProvider.Instance.GetAllPokemon();
        }


        private void FindCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Name == SelectedPokemon.Name));

        private void NextCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Num == selectedPokemon.Num + 1));

        private void PreviousCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Num == selectedPokemon.Num - 1));

        public void OnPokemonChanged(PokedexClass pkmn) => PokemonChangedEvent?.Invoke(this, new PokemonChangedEventArgs { Pkmn = pkmn });
    }

    public class PokemonChangedEventArgs
    {
        public PokedexClass Pkmn { get; set; }
    }
}
