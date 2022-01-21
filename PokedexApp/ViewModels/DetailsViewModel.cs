using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MVVMFramework.ViewModels;
using Pokedex.PokedexLib;
using MVVMFramework.ViewNavigator;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class DetailsViewModel : TabViewModel
    {
        #region Fields and props

        private int number;
        private string name;
        private TypeEnum? type1;
        private TypeEnum? type2;
        private string ability1;
        private string ability2;
        private string hiddenAbility;
        private EggGroupEnum? eggGroup1;
        private EggGroupEnum? eggGroup2;
        private ObservableCollection<TypeMult> weaknesses;
        private ObservableCollection<TypeMult> resistances;
        private ObservableCollection<TypeMult> immunities;
        private ObservableCollection<TypeMult> normalDamage;
        private List<DualTypeClass> typeCombos;

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

        public EggGroupEnum? EggGroup1
        {
            get => eggGroup1;
            set => SetProperty(ref eggGroup1, value);
        }

        public EggGroupEnum? EggGroup2
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

        #endregion

        public DetailsViewModel()
        {
            typeCombos = TypeMasterClass.Instance.GetDualTypeCombos();
        }

        public override void OnUnloaded()
        {
            mainViewModel.PokemonChangedAction -= OnPokemonChanged;
            base.OnUnloaded();
        }

        protected override void OnPokemonChanged(Pokemon pkmn)
        {
            Name = pkmn.Name;
            Type1 = pkmn.Type1;
            Type2 = pkmn.Type2;
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
        }
    }
}
