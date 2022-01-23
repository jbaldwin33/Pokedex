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
        private const int MAX_POKEMON = 898;
        public INavigator Navigator { get; set; }
        public Action<Pokemon> PokemonChangedAction;
        private readonly List<Pokemon> evolutionList;
        private readonly ObservableCollection<Pokemon> pokemonList;
        private Pokemon selectedPokemon;
        private byte[] iconData;
        private ObservableCollection<PokemonForm> formCollection;
        private List<Pokemon> pokemonListWithForms;
        private Pokemon placeholder;
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
            placeholder = new Pokemon { Name = "-Select a Pokemon-", Id = -1 };
            pokemonList = new ObservableCollection<Pokemon>(pokemonListWithForms.Where(x => x.NationalDex == Math.Floor(x.NationalDex)));

            Pokedexes = new List<PokedexComboBoxViewModel>
            {
                new PokedexComboBoxViewModel(DexType.Alphabetical, new ObservableCollection<Pokemon>(pokemonList.OrderBy(p => p.Name)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.National, new ObservableCollection<Pokemon>(pokemonList.OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Kanto, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex < 152).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Johto, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 152 && p.NationalDex < 252).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Hoenn, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 252 && p.NationalDex < 387).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Sinnoh, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 387 && p.NationalDex < 494).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Unova, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 494 && p.NationalDex < 650).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Kalos, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 650 && p.NationalDex < 722).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Alola, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 722 && p.NationalDex < 810).OrderBy(p => p.NationalDex)), OnPokemonChanged),
                new PokedexComboBoxViewModel(DexType.Galar, new ObservableCollection<Pokemon>(pokemonList.Where(p => p.NationalDex >= 810).OrderBy(p => p.NationalDex)), OnPokemonChanged)
            };
            Pokedexes.ForEach(p => p.DexList.Insert(0, placeholder));
            FormCollection = new ObservableCollection<PokemonForm>();
        }

        #region Commands

        public RelayCommand FindCommand => findCommand ??= new RelayCommand(FindCommandExecute);
        public RelayCommand NextCommand => nextCommand ??= new RelayCommand(NextCommandExecute, () => SelectedPokemon?.NationalDex <= MAX_POKEMON);
        public RelayCommand PreviousCommand => previousCommand ??= new RelayCommand(PreviousCommandExecute, () => SelectedPokemon?.NationalDex > 1);
        public RelayCommand OpenSortWindowCommand => openSortWindowCommand ??= new RelayCommand(OpenSortWindowCommandExecute, () => true);

        #endregion

        public bool PokemonHasForms(Pokemon pkmn) => pokemonListWithForms.Any(x => x.NationalDex != pkmn.NationalDex && Math.Truncate(x.NationalDex) == pkmn.NationalDex);
        public IEnumerable<Pokemon> GetPokemonForms(Pokemon pkmn) => pokemonListWithForms.Where(x => x.NationalDex != pkmn.NationalDex && Math.Truncate(x.NationalDex) == pkmn.NationalDex);

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
            pokemonListWithForms.OrderBy(p => p.NationalDex);
        }

        private void FindCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.Name == SelectedPokemon.Name), CurrentDexType);

        private void NextCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.NationalDex == Math.Floor(SelectedPokemon.NationalDex) + 1), CurrentDexType);

        private void PreviousCommandExecute() => OnPokemonChanged(pokemonList.FirstOrDefault(e => e.NationalDex == Math.Floor(SelectedPokemon.NationalDex) - 1), CurrentDexType);
        private void OpenSortWindowCommandExecute()
        {
            var sortWindow = new PokemonListSortWindow()
            {
                DataContext = new PokemonListSortViewModel(new ObservableCollection<Pokemon>(pokemonListWithForms)),
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
                Width = 700,
                Height = 450
            };
            sortWindow.Initialize();
            sortWindow.ShowDialog();
        }

        private void OnPokemonChanged(Pokemon pkmn, DexType dexType)
        {
            if (pkmn.Id == -1)
                return;

            if (CurrentDexType != dexType)
                ClearComboBoxes();

            CurrentDexType = dexType;
            Pokedexes.First(p => p.PokedexType == CurrentDexType).UpdateComboboxWithoutNotify(pkmn);
            if (!isForm(pkmn) && hasForms(pkmn))
                AddForms(pkmn);
            else if (!isForm(pkmn) && !hasForms(pkmn))
                FormCollection.Clear();
            if (!isForm(pkmn))
                SelectedPokemon = pkmn;
            IconData = pkmn.Icon;
            PokemonChangedAction?.Invoke(pkmn);

            bool isForm(Pokemon pkmn) => pkmn.NationalDex != Math.Floor(pkmn.NationalDex);
            bool hasForms(Pokemon pkmn) => PokemonHasForms(pkmn);
        }

        private void ClearComboBoxes() => Pokedexes.First(pkdx => pkdx.PokedexType == CurrentDexType).SelectedPokemon = placeholder;

        private void AddForms(Pokemon pkmn)
        {
            FormCollection.Clear();
            FormCollection.Add(new PokemonForm { FormCommand = new RelayCommand(() => FormCommandExecute(pkmn), () => true), Name = pkmn.Name, Num = pkmn.NationalDex });
            var forms = GetPokemonForms(pkmn);
            foreach (var form in forms)
                FormCollection.Add(new PokemonForm { FormCommand = new RelayCommand(() => FormCommandExecute(form), () => true), Name = form.Name, Num = form.NationalDex });
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
