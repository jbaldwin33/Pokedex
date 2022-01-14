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
        private string[] pokemonWithMultipleEvolutions = new string[]
        {
            "Gloom", "Poliwhirl", "Slowpoke", "Tyrogue", "Eevee", "Wurmple", "Kirlia", "Nincada", "Snorunt", "Clamperl", "Burmy"
        };
        public INavigator Navigator { get; set; }
        public Action<PokedexClass> PokemonChangedAction;
        private PokedexClass selectedPokemon;
        private int currentID;
        private byte[] iconData;
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

        public byte[] IconData
        {
            get => iconData;
            set => SetProperty(ref iconData, value);
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
        private List<PokedexClass> evList = new List<PokedexClass>();

        public List<PokedexClass> GetEvolutionLine(PokedexClass pkmn)
        {
            evList.Clear();
            evList.Add(pkmn);
            CheckPrevious(pkmn);
            CheckNext(pkmn);

            evList.Sort((x1, x2) => x1.EvolutionOrderNum < x2.EvolutionOrderNum ? -1 : 1);
            return evList;
        }

        private void CheckPrevious(PokedexClass currentPkmn)
        {
            var prevPkmn = PokemonList.FirstOrDefault(p => p.EvolutionOrderNum == Math.Floor(currentPkmn.EvolutionOrderNum) - 1);
            if (currentPkmn.CanEvolveTo)
            {
                evList.Add(prevPkmn);
                CheckPrevious(prevPkmn);
            }
        }

        private void CheckNext(PokedexClass currentPkmn)
        {
            var nextPkmn = PokemonList.FirstOrDefault(p => p.EvolutionOrderNum == Math.Floor(currentPkmn.EvolutionOrderNum) + 1);
            if (nextPkmn != null && nextPkmn.CanEvolveTo)
            {
                evList.Add(nextPkmn);
                CheckNext(nextPkmn);
            }
        }

        private async void GetAllPokemon()
        {
            pokemonListWithForms = await PokedexProvider.Instance.GetAllPokemon();
            pokemonListWithForms.Sort((p1, p2) => p1.Num < p2.Num ? -1 : 1);
            //PokedexProvider.Instance.SetEvolutions(pokemonListWithForms);
        }

        private void FindCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Name == SelectedPokemon.Name));

        private void NextCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Num == Math.Floor(SelectedPokemon.Num) + 1));

        private void PreviousCommandExecute() => OnPokemonChanged(PokemonList.FirstOrDefault(e => e.Num == Math.Floor(SelectedPokemon.Num) - 1));

        private void OnPokemonChanged(PokedexClass pkmn)
        {
            if (!isForm(pkmn) && hasForms(pkmn))
                AddForms(pkmn);
            else if (!isForm(pkmn) && !hasForms(pkmn))
                FormCollection.Clear();
            if (!isForm(pkmn))
                CurrentID = (int)pkmn.Num - 1;
            IconData = pkmn.Icon;
            PokemonChangedAction?.Invoke(pkmn);

            bool isForm(PokedexClass pkmn) => pkmn.Num != Math.Floor(pkmn.Num);
            bool hasForms(PokedexClass pkmn) => PokemonHasForms(pkmn);
        }

        private void AddForms(PokedexClass pkmn)
        {
            FormCollection.Clear();
            FormCollection.Add(new PokedexForm { FormCommand = new RelayCommand(() => FormCommandExecute(pkmn), () => true), Name = pkmn.Name, Num = pkmn.Num });
            var forms = GetPokemonForms(pkmn);
            foreach (var form in forms)
                FormCollection.Add(new PokedexForm { FormCommand = new RelayCommand(() => FormCommandExecute(form), () => true), Name = form.Name, Num = form.Num });
        }

        private void FormCommandExecute(PokedexClass form)
        {
            selectedPokemon = form;
            OnPokemonChanged(form);
        }

        public class PokedexForm
        {
            public RelayCommand FormCommand { get; set; }
            public string Name { get; set; }
            public float Num { get; set; }
        }
    }
}
