using CsvHelper.Configuration.Attributes;

namespace Pokedex.PokedexCSVCreator.Models
{
    public class PokemonEntity
    {
        [Name("Per")]
        public float ID { get; set; }
        [Name("Nat")]
        public float NationalDex { get; set; }
        public float JohtoDex { get; set; }
        public float HoennDex { get; set; }
        public float SinnohDex { get; set; }
        public float UnovaDex { get; set; }
        public float KalosDex { get; set; }
        public float AlolaDex { get; set; }
        public float GalarDex { get; set; }
        [Name("Pokemon")]
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
        //public byte[] Icon { get; set; }
        public string EvolveNum { get; set; }
    }
}
