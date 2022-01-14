using CsvHelper;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Entities;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pokedex.PkdxDatabase
{
    public static class PopulateDB
    {
        private static readonly string filename = "PokedexFile.csv";
        private static readonly string iconDirectory = "pokemon/main-sprites/black-white";

        public static void PopulateDatabase(PokedexDBContext context)
        {
            using var reader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Binaries", filename));
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<Pokemon>();
            var counter = 1;
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new PokemonEntity
                {
                    Id = counter,
                    Num = csv.GetField<float>("Nat"),
                    EvolutionOrderNum = csv.GetField<float>("Per"),
                    HP = csv.GetField<int>("HP"),
                    Atk = csv.GetField<int>("Atk"),
                    Def = csv.GetField<int>("Def"),
                    SpA = csv.GetField<int>("SpA"),
                    SpD = csv.GetField<int>("SpD"),
                    Spe = csv.GetField<int>("Spe"),
                    Total = csv.GetField<int>("Total"),
                    Name = csv.GetField("Pokemon"),
                    Type1 = csv.GetField("Type I"),
                    Type2 = csv.GetField("Type II"),
                    Ability1 = csv.GetField("Ability I"),
                    Ability2 = csv.GetField("Ability II"),
                    HiddenAbility = csv.GetField("Hidden Ability"),
                    EggGroup1 = csv.GetField("Egg Group I"),
                    EggGroup2 = csv.GetField("Egg Group II"),
                    EvolveMethodString = csv.GetField("Evolve"),
                    NumberOfEvolutions = !string.IsNullOrEmpty(csv.GetField("EvolveNum")) ? csv.GetField<int>("EvolveNum") : 0,
                    EVYield = csv.GetField("EV Worth")
                };
                var iconFile = GetIconFile(record.Num, record.Name);
                if (!string.IsNullOrEmpty(iconFile))
                    record.Icon = ImageToByteArray(iconFile);
                
                var entry = context.PokedexEntries.Find(record.Id);
                if (entry == null)
                {
                    context.Add(record);
                    context.SaveChanges();
                }
                counter++;
            }
        }

        private static string GetIconFile(float number, string name)
        {
            if (number > 649)
                return string.Empty;

            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Binaries", iconDirectory));
            return Math.Floor(number) != number
                ? files.First(file => Path.GetFileName(file).Equals($"{Math.Floor(number)}-{getFormName()}.png"))
                : files.First(file => Path.GetFileName(file).Equals($"{number}.png"));

            string getFormName() => name.Substring(name.IndexOf('(') + 1, name.IndexOf(')') - (name.IndexOf('(') + 1)).ToLower();
        }

        public static byte[] ImageToByteArray(string imageIn) => File.ReadAllBytes(imageIn);
    }
}
