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
    public class StatsViewModel : ViewModel
    {
        private MainViewModel mainViewModel;
        private int hpStat;
        private int atkStat;
        private int defStat;
        private int spaStat;
        private int spdStat;
        private int speStat;
        private int totalStats;

        public int HPStat
        {
            get => hpStat;
            set => SetProperty(ref hpStat, value);
        }

        public int AtkStat
        {
            get => atkStat;
            set => SetProperty(ref atkStat, value);
        }

        public int DefStat
        {
            get => defStat;
            set => SetProperty(ref defStat, value);
        }

        public int SpAStat
        {
            get => spaStat;
            set => SetProperty(ref spaStat, value);
        }

        public int SpDStat
        {
            get => spdStat;
            set => SetProperty(ref spdStat, value);
        }

        public int SpeStat
        {
            get => speStat;
            set => SetProperty(ref speStat, value);
        }

        public int TotalStats
        {
            get => totalStats;
            set => SetProperty(ref totalStats, value);
        }

        public string HPLabel => "HP";
        public string AtkLabel => "Atk";
        public string DefLabel => "Def";
        public string SpALabel => "SpA";
        public string SpDLabel => "SpD";
        public string SpeLabel => "Spe";
        public string TotalStatsLabel => "Total ";

        public StatsViewModel()
        {

        }

        public override void OnLoaded()
        {
            mainViewModel = Navigator.Instance.MainViewModel as MainViewModel;
            mainViewModel.PokemonChangedAction += OnPokemonChanged;
            if (mainViewModel.SelectedPokemon != null)
                PopulateStats(mainViewModel.SelectedPokemon);
            base.OnLoaded();
        }

        private void OnPokemonChanged(PokedexClass pkmn) => PopulateStats(pkmn);
        private void PopulateStats(PokedexClass pkmn)
        {
            HPStat = pkmn.HP;
            AtkStat = pkmn.Atk;
            DefStat = pkmn.Def;
            SpAStat = pkmn.SpA;
            SpDStat = pkmn.SpD;
            SpeStat = pkmn.Spe;
            TotalStats = pkmn.Total;
        }
    }
}
