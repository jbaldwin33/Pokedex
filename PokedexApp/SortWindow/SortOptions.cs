using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.SortWindow
{
    public class SortOptions
    {
        public Action ListSorted;
        public IEnumerable<Pokemon> PokemonList;
        public SortType Sort { get; set; }

        public IEnumerable<Pokemon> SortPokemonList(string detailedSort) => Sort switch
        {
            SortType.EVYield => SortByEV(detailedSort),
            SortType.BaseStat => SortByStat(detailedSort),
            _ => throw new ArgumentOutOfRangeException(nameof(detailedSort)),
        };

        public void SortListCommandExecute(object detailedSort)
        {
            PokemonList = SortPokemonList((string)detailedSort);
            ListSorted?.Invoke();
        }

        private IEnumerable<Pokemon> SortByStat(string detailedSort)
            => PokemonList.OrderByDescending(p => p.BaseStats.First(x => x.Stat == (StatEnum)Enum.Parse(typeof(StatEnum), detailedSort)).Value).ThenBy(p => p.Num);
        private IEnumerable<Pokemon> SortByEV(string detailedSort)
            => PokemonList.OrderByDescending(p => p.EVYields.First(x => x.Stat == (StatEnum)Enum.Parse(typeof(StatEnum), detailedSort)).Value).ThenBy(p => p.Num);
    }
}
