using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pokedex.PkdxDatabase.Entities
{
    public class PokemonEntity
    {
        [Key]
        public int Id { get; set; }
        public float Num { get; set; }
        public float EvolutionOrderNum { get; set; }
        public string Name { get; set; }
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int SpA { get; set; }
        public int SpD { get; set; }
        public int Spe { get; set; }
        public int Total { get; set; }
        public string Type1 { get; set; }
        public string Type2 { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string HiddenAbility { get; set; }
        public string EVYield { get; set; }
        public string EggGroup1 { get; set; }
        public string EggGroup2 { get; set; }
        public string EvolveMethodString { get; set; }
        public byte[] Icon { get; set; }
        public int NumberOfEvolutions { get; set; }
    }
}
