using System.Collections.Generic;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PkdxDatabase.Models
{
    public class Pokemon
    {
        public int Id { get; set; }
        public int NationalDex { get; set; }
        public int JohtoDex { get; set; }
        public int HoennDex { get; set; }
        public int SinnohDex { get; set; }
        public int UnovaDex { get; set; }
        public int KalosDex { get; set; }
        public int AlolaDex { get; set; }
        public int GalarDex { get; set; }
        public int IsleOfArmorDex { get; set; }
        public int CrownTundraDex { get; set; }
        public string PrevEvolution { get; set; }
        public string[] NextEvolution { get; set; }
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
        public bool HasForms { get; set; }
        public bool IsForm { get; set; }
        public bool IsDefaultForm { get; set; }
        public bool IsAlolanForm { get; set; }
        public bool IsGalarianForm { get; set; }
        public bool EvolvesFromRegionalForm { get; set; }
    }
}
