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
                    Type1 = csv.GetField("Type1"),
                    Type2 = csv.GetField("Type2"),
                    Ability1 = csv.GetField("Ability1"),
                    Ability2 = csv.GetField("Ability2"),
                    HiddenAbility = csv.GetField("HiddenAbility"),
                    EggGroup1 = csv.GetField("EggGroup1"),
                    EggGroup2 = csv.GetField("EggGroup2"),
                    EvolveMethodString = csv.GetField("Evolve"),
                    NumberOfEvolutions = !string.IsNullOrEmpty(csv.GetField("EvolveNum")) ? csv.GetField<int>("EvolveNum") : 0,
                    EVYield = csv.GetField("EVYield")
                };
                var iconFile = GetIconFile(record.Num, record.Name);
                if (!string.IsNullOrEmpty(iconFile))
                    record.Icon = ImageToByteArray(iconFile);
                
                var entry = context.PokedexEntries.Find(record.Id);
                if (entry == null)
                {
                    context.Add(record);
                    context.SaveChanges();
                    Console.WriteLine($"{counter} record(s) saved.");
                }
                counter++;
            }
            Console.WriteLine("Done.");
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
