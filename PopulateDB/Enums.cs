using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexLib
{
    public static class Enums
    {
        public enum TypeEnum
        {
            Normal,
            Fire,
            Water,
            Grass,
            Electric,
            Ice,
            Fighting,
            Poison,
            Ground,
            Flying,
            Psychic,
            Bug,
            Rock,
            Ghost,
            Dark,
            Dragon,
            Steel,
            Fairy
        }

        public enum DexType { Alphabetical, National, Kanto, Johto, Hoenn, Sinnoh, Unova }

        public enum SortType
        {
            //[Description("Type")]
            //PokemonType,
            [Description("EV Yield")]
            EVYield,
            [Description("Base Stat")]
            BaseStat
        }

        public enum StatEnum
        {
            [Description("HP")]
            HP,
            [Description("Attack")]
            Atk,
            [Description("Defense")]
            Def,
            [Description("Special Attack")]
            SpA,
            [Description("Special Defense")]
            SpD,
            [Description("Speed")]
            Spe,
            [Description("Total")]
            Total
        }
    }
}
