using CsvHelper;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.PkdxDatabase
{
    public static class PopulateDB
    {
        private static readonly string binaryDirectory = Path.Combine(Path.GetDirectoryName(AppContext.BaseDirectory), "Binaries");
        private static readonly string filename = Path.Combine(binaryDirectory, "NewCSV.csv");
        private const int MAX_POKEMON = 898;

        public static async void PopulateDatabase(PokedexDBContext context)
        {
            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var counter = 1;
            var failed = 0;
            var records = new List<PokemonEntity>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new PokemonEntity
                {
                    Id = counter,
                    NationalDex = csv.GetField<int>("NationalDex"),
                    JohtoDex = csv.GetField<int>("JohtoDex"),
                    HoennDex = csv.GetField<int>("HoennDex"),
                    SinnohDex = csv.GetField<int>("SinnohDex"),
                    UnovaDex = csv.GetField<int>("UnovaDex"),
                    KalosDex = csv.GetField<int>("KalosDex"),
                    AlolaDex = csv.GetField<int>("AlolaDex"),
                    GalarDex = csv.GetField<int>("GalarDex"),
                    PrevEvolution = csv.GetField("PrevEvolution"),
                    NextEvolution = csv.GetField("NextEvolution"),
                    HP = csv.GetField<int>("HP"),
                    Atk = csv.GetField<int>("Atk"),
                    Def = csv.GetField<int>("Def"),
                    SpA = csv.GetField<int>("SpA"),
                    SpD = csv.GetField<int>("SpD"),
                    Spe = csv.GetField<int>("Spe"),
                    Total = csv.GetField<int>("Total"),
                    Name = csv.GetField("Name"),
                    Type1 = csv.GetField("Type1"),
                    Type2 = csv.GetField("Type2"),
                    Ability1 = csv.GetField("Ability1"),
                    Ability2 = csv.GetField("Ability2"),
                    HiddenAbility = csv.GetField("HiddenAbility"),
                    EggGroup1 = csv.GetField("EggGroup1"),
                    EggGroup2 = csv.GetField("EggGroup2"),
                    EvolveMethodString = csv.GetField("Evolve"),
                    NumberOfEvolutions = !string.IsNullOrEmpty(csv.GetField("EvolveNum")) ? csv.GetField<int>("EvolveNum") : 0,
                    EVYield = csv.GetField("EVYield"),
                    HasForms = csv.GetField<int>("HasForms") == 1,
                    IsForm = csv.GetField<int>("IsForm") == 1,
                    
                };

                records.Add(record);
                await context.AddAsync(record);
                await context.SaveChangesAsync();
                Console.WriteLine($"{counter} record(s) saved.");
                counter++;
            }

            //add icons
            Parallel.ForEach(records, record =>
            {
                record.Icon = GetIconFile(record.NationalDex, record.Name);
                Console.WriteLine($"{record.Name} icon saved.");
            });

            var t = Task.Run(() => context.SaveChangesAsync());
            Console.Write("Saving changes to database...");
            while (!t.IsCompleted)
            {
                Thread.Sleep(2000);
                Console.Write("...");
            }
            Console.WriteLine($"{failed} record(s) failed.");
            Console.WriteLine("Done.");
        }

        private static byte[] GetIconFile(float number, string name)
        {
            if (number > MAX_POKEMON)
                return null;

            return Math.Floor(number) != number
                ? (byte[])Resource1.ResourceManager.GetObject($"_{Math.Floor(number):000}_{getFormName()}")
                : (byte[])Resource1.ResourceManager.GetObject($"_{number:000}_{name.ToLower()}");

            string getFormName() => name.Substring(name.IndexOf('(') + 1, name.IndexOf(')') - (name.IndexOf('(') + 1)).ToLower();
        }
    }
}
