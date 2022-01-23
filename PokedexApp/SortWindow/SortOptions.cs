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
        public Action ListSorted { get; set; }
        public IEnumerable<Pokemon> PokemonList { get; set; }
        public SortType Sort { get; set; }
        public TypeEnum TypeToSort { get; set; }
        public EggGroupEnum EggGroupToSort { get; set; }

        public void SortList()
        {
            PokemonList = GetSortedPokemonList();
            ListSorted?.Invoke();
        }

        private IEnumerable<Pokemon> GetSortedPokemonList() => Sort switch
        {
            SortType.EVYield => SortByEV(),
            SortType.BaseStat => SortByStat(),
            SortType.PokemonType => SortByType(),
            SortType.EggGroup => SortByEggGroup(),
            _ => throw new ArgumentOutOfRangeException(nameof(Sort)),
        };

        private IEnumerable<Pokemon> SortByStat() => PokemonList.OrderByDescending(p => p.BaseStats.First().Value).ThenBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByEV() => PokemonList.OrderByDescending(p => p.EVYields.First().Value).ThenBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByType() => PokemonList.Where(p => p.Type1 == TypeToSort || p.Type2 == TypeToSort).OrderBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByEggGroup() => PokemonList.OrderByDescending(p => p.EggGroup1 == EggGroupToSort || p.EggGroup2 == EggGroupToSort).OrderBy(p => p.NationalDex);
    }
}
