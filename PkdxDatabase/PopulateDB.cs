using CsvHelper;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pokedex.PkdxDatabase
{
    public static class PopulateDB
    {
        private static readonly string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "..\\", "..\\", "assemblies", "NewCSV.csv");
        private static CancellationToken token;
        private const int MAX_POKEMON = 898;


        public static async Task PopulateDatabase(CancellationToken t, CancellationTokenSource ts, Action cancelAction, Action<string> messageAction)
        {
            token = t;
            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            await Populate(context, cancelAction, messageAction);
        }

        private static async Task Populate(PokedexDBContext context, Action cancelAction, Action<string> messageAction)
        {
            if (token.IsCancellationRequested)
            {
                cancelAction();
                return;
            }
            try
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
                        NumberOfEvolutions = string.IsNullOrEmpty(csv.GetField("EvolveNum")) ? 0 : csv.GetField<int>("EvolveNum"),
                        EVYield = csv.GetField("EVYield"),
                        HasForms = csv.GetField<bool>("HasForms"),
                        IsForm = csv.GetField<bool>("IsForm"),
                        IsDefaultForm = csv.GetField<bool>("IsDefaultForm"),
                        IsAlolanForm = csv.GetField<bool>("IsAlolanForm"),
                        IsGalarianForm = csv.GetField<bool>("IsGalarianForm"),
                        EvolvesFromRegionalForm = csv.GetField<bool>("EvolvesFromRegionalForm")
                    };

                    records.Add(record);
                    await context.AddAsync(record);
                    await context.SaveChangesAsync();
                    messageAction($"{counter} record(s) saved.");
                    counter++;

                    if (token.IsCancellationRequested)
                    {
                        cancelAction();
                        context.Database.EnsureDeleted();
                        return;
                    }
                }

                //add icons
                var failedicons = 0;
                Parallel.ForEach(records, record =>
                {
                    record.Icon = GetIconFile(record.NationalDex, record.Name);
                    if (record.Icon == null)
                        failedicons++;
                    else
                        messageAction($"{record.Name} icon saved.");
                });

                var t = Task.Run(() => context.SaveChangesAsync());
                messageAction("Saving changes to database...");
                while (!t.IsCompleted)
                {
                    Thread.Sleep(2000);
                    messageAction("...");
                }
                messageAction($"{failed} record(s) failed.");
                messageAction($"{failedicons} icon(s) failed.");
            }
            catch (Exception ex)
            {
                messageAction(ex.ToString());
            }
        }

        private static byte[] GetIconFile(float number, string name) => number > MAX_POKEMON ? null : (byte[])Resource1.ResourceManager.GetObject(RemoveSpecialCharacters(name).ToLower());

        private static string RemoveSpecialCharacters(string name) => name.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace(".", "");
    }
}
