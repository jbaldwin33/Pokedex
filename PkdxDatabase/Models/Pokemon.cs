using System.Collections.Generic;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PkdxDatabase.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public float Num { get; set; }
        public float EvolutionOrderNum { get; set; }
        public string Name { get; set; }
        public TypeEnum? Type1 { get; set; }
        public TypeEnum? Type2 { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string HiddenAbility { get; set; }
        public List<EVYield> EVYields { get; set; }
        public List<BaseStat> BaseStats { get; set; }
        public EggGroupEnum? EggGroup1 { get; set; }
        public EggGroupEnum? EggGroup2 { get; set; }
        public bool CanEvolveTo { get; set; }
        public string EvolveMethodString { get; set; }
        public byte[] Icon { get; set; }
        public int NumberOfEvolutions { get; set; }
        public bool HasMultipleEvolutions { get; set; }
    }
}
