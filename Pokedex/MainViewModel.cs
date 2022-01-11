using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MVVMFramework.ViewModels;
using Pokedex.PokedexLib;
using System.Windows.Data;

namespace Pokedex.PokedexApp
{
    public class MainViewModel : ViewModel
    {
        #region Fields and props

        private int number;
        private string name;
        private TypeEnum? type1;
        private TypeEnum? type2;
        private string ability1;
        private string ability2;
        private string hiddenAbility;
        private string eggGroup1;
        private string eggGroup2;
        private ObservableCollection<TypeMult> weaknesses;
        private ObservableCollection<TypeMult> resistances;
        private ObservableCollection<TypeMult> immunities;
        private ObservableCollection<TypeMult> normalDamage;
        private EvolveMethodEnum evolveMethod;
        private PokedexClass selectedPokemon;
        private int currentID;
        private ObservableCollection<PokedexClass> pokemonList;
        private ObservableCollection<PokedexForm> formCollection;

        private List<DualTypeClass> typeCombos;
        //private List<PokedexClass> _pkmnCache;
        //private List<PokedexClass> pkmnCache => _pkmnCache ??= new List<PokedexClass>();
        private RelayCommand findCommand;
        private RelayCommand nextCommand;
        private RelayCommand previousCommand;

        public int Number
        {
            get => number;
            set => SetProperty(ref number, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public TypeEnum? Type1
        {
            get => type1;
            set => SetProperty(ref type1, value);
        }

        public TypeEnum? Type2
        {
            get => type2;
            set => SetProperty(ref type2, value);
        }

        public string Ability1
        {
            get => ability1;
            set => SetProperty(ref ability1, value);
        }

        public string Ability2
        {
            get => ability2;
            set => SetProperty(ref ability2, value);
        }

        public string HiddenAbility
        {
            get => hiddenAbility;
            set => SetProperty(ref hiddenAbility, value);
        }

        public string EggGroup1
        {
            get => eggGroup1;
            set => SetProperty(ref eggGroup1, value);
        }

        public string EggGroup2
        {
            get => eggGroup2;
            set => SetProperty(ref eggGroup2, value);
        }

        public ObservableCollection<TypeMult> Weaknesses
        {
            get => weaknesses;
            set => SetProperty(ref weaknesses, value);
        }

        public ObservableCollection<TypeMult> Resistances
        {
            get => resistances;
            set => SetProperty(ref resistances, value);
        }

        public ObservableCollection<TypeMult> Immunities
        {
            get => immunities;
            set => SetProperty(ref immunities, value);
        }

        public ObservableCollection<TypeMult> NormalDamage
        {
            get => normalDamage;
            set => SetProperty(ref normalDamage, value);
        }

        public EvolveMethodEnum EvolveMethod
        {
            get => evolveMethod;
            set => SetProperty(ref evolveMethod, value);
        }

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

        #endregion

        #region Commands

        public RelayCommand FindCommand => findCommand ??= new RelayCommand(FindCommandExecute);
        public RelayCommand NextCommand => nextCommand ??= new RelayCommand(NextCommandExecute, () => SelectedPokemon?.ID < 649);
        public RelayCommand PreviousCommand => previousCommand ??= new RelayCommand(PreviousCommandExecute, () => SelectedPokemon?.ID > 1);

        #endregion

        private List<PokedexClass> pokemonListWithForms;
        public MainViewModel()
        {
            typeCombos = TypeMasterClass.Instance.GetDualTypeCombos();
            using var context = new PokedexDBContext();
            pokemonListWithForms = context.PokedexEntries.ToList();
            PokemonList = new ObservableCollection<PokedexClass>(pokemonListWithForms.Where(x => x.ID == Math.Floor(x.ID)));

            FormCollection = new ObservableCollection<PokedexForm>();
            //BindingOperations.EnableCollectionSynchronization(FormCollection, _lock);

        }

        private void FindCommandExecute() => PopulateDetails(PokemonList.FirstOrDefault(e => e.Name == SelectedPokemon.Name));

        private void NextCommandExecute() => PopulateDetails(PokemonList.FirstOrDefault(e => e.ID == selectedPokemon.ID + 1));

        private void PreviousCommandExecute() => PopulateDetails(PokemonList.FirstOrDefault(e => e.ID == selectedPokemon.ID - 1));


        private void PopulateDetails(PokedexClass pkmn)
        {
            if (!isForm(pkmn) && hasForms(pkmn))
                AddForms(pkmn);
            else if (!isForm(pkmn) && !hasForms(pkmn))
                FormCollection.Clear();
            if (!isForm(pkmn))
                CurrentID = (int)pkmn.ID - 1;
            Name = pkmn.Name;
            Type1 = (TypeEnum)Enum.Parse(typeof(TypeEnum), pkmn.Type1);
            Type2 = Enum.TryParse(typeof(TypeEnum), pkmn.Type2, out var t2)
                ? (TypeEnum?)(TypeEnum)t2
                : null;
            Ability1 = pkmn.Ability1;
            Ability2 = pkmn.Ability2;
            HiddenAbility = pkmn.HiddenAbility;
            EggGroup1 = pkmn.EggGroup1;
            EggGroup2 = pkmn.EggGroup2;
            if (Type2 == null)
            {
                Weaknesses = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == Type1).Weaknesses);
                Resistances = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == Type1).Resistances);
                Immunities = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == Type1).Immunities);
                NormalDamage = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == Type1).NormalDamage);
            }
            else
            {
                Weaknesses = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == Type1 && x.Type2.ThisType == Type2).Weaknesses);
                Resistances = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == Type1 && x.Type2.ThisType == Type2).Resistances);
                Immunities = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == Type1 && x.Type2.ThisType == Type2).Immunities);
                NormalDamage = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == Type1 && x.Type2.ThisType == Type2).NormalDamage);
            }

            bool isForm(PokedexClass pkmn) => pkmn.ID != Math.Floor(pkmn.ID);
            bool hasForms(PokedexClass pkmn) => pokemonListWithForms.Any(x => x.ID != pkmn.ID && Math.Truncate(x.ID) == pkmn.ID);
        }
        private static readonly object _lock = new object();

        private void AddForms(PokedexClass pkmn)
        {
            FormCollection.Clear();
            FormCollection.Add(new PokedexForm { FormCommand = new RelayCommand(() => FormCommandExecute(pkmn), () => true), Name = pkmn.Name, ID = pkmn.ID });
            var forms = pokemonListWithForms.Where(x => x.ID != pkmn.ID && Math.Truncate(x.ID) == pkmn.ID);
            foreach (var form in forms)
                FormCollection.Add(new PokedexForm { FormCommand = new RelayCommand(() => FormCommandExecute(form), () => true), Name = form.Name, ID = form.ID });
        }

        private void FormCommandExecute(PokedexClass form)
        {
            selectedPokemon = form;
            PopulateDetails(form);
        }
    }

    public class PokedexForm
    {
        public RelayCommand FormCommand { get; set; }
        public string Name { get; set; }
        public float ID { get; set; }
    }

    public class MultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => $"{value}x";

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value;
    }
}
