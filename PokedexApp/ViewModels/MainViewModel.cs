using MVVMFramework.ViewModels;
using MVVMFramework.ViewNavigator;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.Services;
using Pokedex.PokedexApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{

    public class MainViewModel : ViewModel
    {
        private const int MAX_POKEMON = 721;
        public INavigator Navigator { get; set; }
        public Action<Pokemon> PokemonChangedAction;
        private readonly List<Pokemon> evolutionList;
        private readonly ObservableCollection<Pokemon> pokemonList;
        private Pokemon selectedPokemon;
        private byte[] iconData;
        private ObservableCollection<PokemonForm> formCollection;
        private List<Pokemon> pokemonListWithForms;
        private RelayCommand findCommand;
        private RelayCommand nextCommand;
        private RelayCommand previousCommand;
        private RelayCommand openSortWindowCommand;

        public Pokemon SelectedPokemon
        {
            get => selectedPokemon;
            set => SetProperty(ref selectedPokemon, value);
        }

        public byte[] IconData
        {
            get => iconData;
            set => SetProperty(ref iconData, value);
        }


        public ObservableCollection<PokemonForm> FormCollection
        {
            get => formCollection;
            set => SetProperty(ref formCollection, value);
        }
        private DexType currentDexType;

        public DexType CurrentDexType
        {
            get => currentDexType;
            set => SetProperty(ref currentDexType, value);
        }

        public List<PokedexComboBoxViewModel> Pokedexes { get; set; }


        public MainViewModel(INavigator navigator)
        {
            Navigator = navigator;
            GetAllPokemon();
            evolutionList = new List<Pokemon>();
            pokemonList = new ObservableCollection<Pokemon>(pokemonListWithForms.Where(x => x.Num == Math.Floor(x.Num)));

            Pokedexes = new List<PokedexComboBoxViewModel>
            {
                new PokedexComboBoxViewModel(DexType.Alphabetical, new ObservableCollection<Pokemon>(pokemonList.OrderBy(p => p.Name)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.National, new ObservableCollection<Pokemon>(pokemonList.OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Kanto, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num < 152).OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Johto, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num >= 152 && p.Num < 252).OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Hoenn, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num >= 252 && p.Num < 387).OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Sinnoh, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num >= 387 && p.Num < 494).OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Unova, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num >= 494 && p.Num < 650).OrderBy(p => p.Num)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Kalos, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.Num >= 650 && p.Num < 722).OrderBy(p => p.Num)), OnPokemonChanged)
            };
            FormCollection = new ObservableCollection<PokemonForm>();
        }

        #region Commands

        public RelayCommand FindCommand => findCommand ??= new RelayCommand(FindCommandExecute);
        public RelayCommand NextCommand => nextCommand ??= new RelayCommand(NextCommandExecute, () => SelectedPokemon?.Num <= MAX_POKEMON);
        public RelayCommand PreviousCommand => previousCommand ??= new RelayCommand(PreviousCommandExecute, () => SelectedPokemon?.Num > 1);
        public RelayCommand OpenSortWindowCommand => openSortWindowCommand ??= new RelayCommand(OpenSortWindowCommandExecute, () => true);

        #endregion

        public bool PokemonHasForms(Pokemon pkmn) => pokemonListWithForms.Any(x => x.Num != pkmn.Num && Math.Truncate(x.Num) == pkmn.Num);
        public IEnumerable<Pokemon> GetPokemonForms(Pokemon pkmn) => pokemonListWithForms.Where(x => x.Num != pkmn.Num && Math.Truncate(x.Num) == pkmn.Num);

        public List<Pokemon> GetEvolutionLine(Pokemon pkmn)
        {
            evolutionList.Clear();
            evolutionList.Add(pkmn);
            CheckPrevious(pkmn);
            CheckNext(pkmn);

            evolutionList.Sort((x1, x2) => x1.EvolutionOrderNum < x2.EvolutionOrderNum ? -1 : 1);
            return evolutionList;
        }

        private void CheckPrevious(Pokemon currentPkmn)
        {
            var prevPkmn = pokemonList.FirstOrDefault(p => p.EvolutionOrderNum == Math.Floor(currentPkmn.EvolutionOrderNum) - 1);
            if (currentPkmn.CanEvolveTo)
            {
                evolutionList.Add(prevPkmn);
                CheckPrevious(prevPkmn);
            }
        }

        private void CheckNext(Pokemon currentPkmn)
        {
            var nextPkmn = pokemonList.FirstOrDefault(p => p.EvolutionOrderNum == Math.Floor(currentPkmn.EvolutionOrderNum) + 1);
            if (nextPkmn != null && nextPkmn.CanEvolveTo)
            {
                evolutionList.Add(nextPkmn);
                CheckNext(nextPkmn);
            }
        }

        private async void GetAllPokemon()
        {
            pokemonListWithForms = await PokedexProvider.Instance.GetAllPokemon();
            pokemonListWithForms.OrderBy(p => p.Num);
        }

        private void FindCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.Name == SelectedPokemon.Name), CurrentDexType);

        private void NextCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.Num == Math.Floor(SelectedPokemon.Num) + 1), CurrentDexType);

        private void PreviousCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.Num == Math.Floor(SelectedPokemon.Num) - 1), CurrentDexType);
        private void OpenSortWindowCommandExecute()
        {
            var sortWindow = new PokemonListSortWindow()
            {
                DataContext = new PokemonListSortViewModel(new ObservableCollection<Pokemon>(pokemonListWithForms)),
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Width = 900,
                Height = 500
            };
            sortWindow.Initialize();
            sortWindow.Show();
        }

        private void OnPokemonChanged(Pokemon pkmn, DexType dexType)
        {
            ClearComboBoxes();
            CurrentDexType = dexType;
            if (!isForm(pkmn) && hasForms(pkmn))
                AddForms(pkmn);
            else if (!isForm(pkmn) && !hasForms(pkmn))
                FormCollection.Clear();
            if (!isForm(pkmn))
                SelectedPokemon = pkmn;
            IconData = pkmn.Icon;
            PokemonChangedAction?.Invoke(pkmn);

            bool isForm(Pokemon pkmn) => pkmn.Num != Math.Floor(pkmn.Num);
            bool hasForms(Pokemon pkmn) => PokemonHasForms(pkmn);
        }

        private void ClearComboBoxes() => Pokedexes.First(pkdx => pkdx.PokedexType == CurrentDexType).SelectedPokemon = null;

        private void AddForms(Pokemon pkmn)
        {
            FormCollection.Clear();
            FormCollection.Add(new PokemonForm { FormCommand = new RelayCommand(() => FormCommandExecute(pkmn), () => true), Name = pkmn.Name, Num = pkmn.Num });
            var forms = GetPokemonForms(pkmn);
            foreach (var form in forms)
                FormCollection.Add(new PokemonForm { FormCommand = new RelayCommand(() => FormCommandExecute(form), () => true), Name = form.Name, Num = form.Num });
        }

        private void FormCommandExecute(Pokemon form)
        {
            SelectedPokemon = form;
            OnPokemonChanged(form, CurrentDexType);
        }

        public class PokemonForm
        {
            public RelayCommand FormCommand { get; set; }
            public string Name { get; set; }
            public float Num { get; set; }
        }
    }
}
