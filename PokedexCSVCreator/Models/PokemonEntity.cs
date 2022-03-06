using CsvHelper.Configuration.Attributes;

namespace Pokedex.PokedexCSVCreator.Models
{
    public class PokemonEntity
    {
        public int ID { get; set; }
        public int NationalDex { get; set; }
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
        public string Tier { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string HiddenAbility { get; set; }
        public string Mass { get; set; }
        [Name("LK/GK")]
        public int LKGK { get; set; }
        public string EVYield { get; set; }
        public int EXPV { get; set; }
        public string Color { get; set; }
        public string Hatch { get; set; }
        public string Gender { get; set; }
        public string EggGroup1 { get; set; }
        public string EggGroup2 { get; set; }
        public int Catch { get; set; }
        public int EXP { get; set; }
        public string Evolve { get; set; }
        public string EvolveNum { get; set; }
        public bool HasForms { get; set; }
        public bool IsForm { get; set; }
        public bool IsAlolanForm { get; set; }
        public bool IsGalarianForm { get; set; }
        public bool EvolvesFromRegionalForm { get; set; }
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
        public string NextEvolution { get; set; }
    }
}
