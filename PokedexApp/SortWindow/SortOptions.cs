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
        public SortType CurrentSortType { get; set; }
        public object SecondarySort { get; set; }

        public void SortList()
        {
            PokemonList = GetSortedPokemonList();
            ListSorted?.Invoke();
        }

        private IEnumerable<Pokemon> GetSortedPokemonList() => CurrentSortType switch
        {
            SortType.EVYield => SortByEV(),
            SortType.BaseStat => SortByStat(),
            SortType.PokemonType => SortByType(),
            SortType.EggGroup => SortByEggGroup(),
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentSortType)),
        };

        private IEnumerable<Pokemon> SortByEV() => PokemonList.OrderByDescending(p => p.EVYields.First().Value).ThenBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByStat() => PokemonList.OrderByDescending(p => p.BaseStats.First().Value).ThenBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByType() => PokemonList.Where(p => p.Type1 == (TypeEnum)SecondarySort || p.Type2 == (TypeEnum)SecondarySort).OrderBy(p => p.NationalDex);
        private IEnumerable<Pokemon> SortByEggGroup() => PokemonList.Where(p => p.EggGroup1 == (EggGroupEnum)SecondarySort || p.EggGroup2 == (EggGroupEnum)SecondarySort).OrderBy(p => p.NationalDex);
    }
}
