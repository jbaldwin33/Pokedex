using System.Collections.Generic;

namespace Pokedex.PkdxDatabase.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public float Num { get; set; }
        public float EvolutionOrderNum { get; set; }
        public string Name { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string HiddenAbility { get; set; }
        public List<EVYield> EVYields { get; set; }
        public List<BaseStat> BaseStats { get; set; }
        public string EggGroup1 { get; set; }
        public string EggGroup2 { get; set; }
        public bool CanEvolveTo { get; set; }
        public string EvolveMethodString { get; set; }
        public byte[] Icon { get; set; }
        public int NumberOfEvolutions { get; set; }
        public bool HasMultipleEvolutions { get; set; }
    }
}
