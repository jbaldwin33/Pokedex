using CsvHelper;
using Pokedex.PkdxDatabase.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Pokedex.PokedexLib
{
    public static class PopulateDB
    {
        private static readonly string filename = "PokedexFile.csv";

        public static PokedexClass[] PopulateDatabase()
        {
            using var reader = new StreamReader(Path.Combine(Assembly.GetExecutingAssembly().Location, "../../", filename));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<PokedexClass>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new PokedexClass
                {
                    ID = csv.GetField<float>("Nat"),
                    Name = csv.GetField("Pokemon"),
                    Type1 = csv.GetField("Type I"),
                    Type2 = csv.GetField("Type II"),
                    Ability1 = csv.GetField("Ability I"),
                    Ability2 = csv.GetField("Ability II"),
                    HiddenAbility = csv.GetField("Hidden Ability"),
                    EggGroup1 = csv.GetField("Egg Group I"),
                    EggGroup2 = csv.GetField("Egg Group II"),
                    //CanEvolve = !string.IsNullOrEmpty(csv.GetField("Evolve")),
                    EvolveMethod = GetEvolveMethod(csv.GetField("Evolve"))
                };
                records.Add(record);
            }
            return records.ToArray();
        }

        private static EvolveMethodEnum GetEvolveMethod(string method)
        {
            if (method.Contains("Lv"))
                return EvolveMethodEnum.Level;
            if (method.Contains("stone"))
                return EvolveMethodEnum.Item;
            if (method.Contains("Happiness"))
                return EvolveMethodEnum.Happiness;
            if (method.Contains("Friendship"))
                return EvolveMethodEnum.Friendship;
            if (method.Contains("Trade"))
                return EvolveMethodEnum.Trade;
            return EvolveMethodEnum.Unknown;
        }
    }
}
