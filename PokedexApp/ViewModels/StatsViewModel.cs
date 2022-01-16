using Pokedex.PkdxDatabase.Models;
using System.Linq;

namespace Pokedex.PokedexApp.ViewModels
{
    public class StatsViewModel : TabViewModel
    {
        #region Fields and props

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

        #endregion

        #region Labels

        public string HPLabel => "HP";
        public string AtkLabel => "Atk";
        public string DefLabel => "Def";
        public string SpALabel => "SpA";
        public string SpDLabel => "SpD";
        public string SpeLabel => "Spe";
        public string TotalStatsLabel => "Total ";

        #endregion

        public StatsViewModel() { }
        protected override void OnPokemonChanged(Pokemon pkmn)
        {
            HPStat = pkmn.BaseStats[0].Value;
            AtkStat = pkmn.BaseStats[1].Value;
            DefStat = pkmn.BaseStats[2].Value;
            SpAStat = pkmn.BaseStats[3].Value;
            SpDStat = pkmn.BaseStats[4].Value;
            SpeStat = pkmn.BaseStats[5].Value;
            TotalStats = pkmn.BaseStats[6].Value;
        }
    }
}
