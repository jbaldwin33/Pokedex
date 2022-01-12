using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pokedex.PkdxDatabase.Models
{
    public enum EvolveMethodEnum
    {
        Level,
        Happiness,
        Friendship,
        Trade,
        Item,
        Unknown
    }
    public class PokedexClass
    {
        [Key]
        public int Id { get; set; }
        public float Num { get; set; }//2
        public string Name { get; set; }//3
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int SpA { get; set; }
        public int SpD { get; set; }
        public int Spe { get; set; }
        public int Total { get; set; }
        public string Type1 { get; set; }//11
        public string Type2 { get; set; }//11
        public string Ability1 { get; set; }//14
        public string Ability2 { get; set; }//15
        public string HiddenAbility { get; set; }//16
        //public string[] EVYield { get; set; }
        public string EggGroup1 { get; set; }//24
        public string EggGroup2 { get; set; }//25
        public bool CanEvolve { get; set; }//28
        public string Evolve { get; set; }
        public EvolveMethodEnum EvolveMethod { get; set; }


    }
}
