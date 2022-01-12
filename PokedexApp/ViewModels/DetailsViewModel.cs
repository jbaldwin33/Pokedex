﻿using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MVVMFramework.ViewModels;
using Pokedex.PokedexLib;
using MVVMFramework.ViewNavigator;

namespace Pokedex.PokedexApp.ViewModels
{
    public class DetailsViewModel : ViewModel
    {
        #region Fields and props

        private MainViewModel mainViewModel;
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

        #endregion

        public DetailsViewModel()
        {
            typeCombos = TypeMasterClass.Instance.GetDualTypeCombos();
        }

        public override void OnLoaded()
        {
            mainViewModel = Navigator.Instance.MainViewModel as MainViewModel;
            mainViewModel.PokemonChangedAction += OnPokemonChanged;
            if (mainViewModel.SelectedPokemon != null)
                PopulateDetails(mainViewModel.SelectedPokemon);
            base.OnLoaded();
        }

        public override void OnUnloaded()
        {
            mainViewModel.PokemonChangedAction -= OnPokemonChanged;
            base.OnUnloaded();
        }

        private void OnPokemonChanged(PokedexClass pkmn) => PopulateDetails(pkmn);

        public void PopulateDetails(PokedexClass pkmn)
        {
            Name = pkmn.Name;
            Type1 = (TypeEnum)Enum.Parse(typeof(TypeEnum), pkmn.Type1);
            Type2 = Enum.TryParse(typeof(TypeEnum), pkmn.Type2, out var t2)
                ? (TypeEnum?)t2
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
        }
    }
}
