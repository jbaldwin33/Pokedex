using CsvHelper;
using CsvHelper.Configuration;
using MVVMFramework.ViewModels;
using Pokedex.PokedexCSVCreator.Models;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Input;
#pragma warning disable IDE0058
namespace Pokedex.PokedexCSVCreator.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly string[] updateNum =
        {
            "sylveon",
            "obstagoon",
            "perrserker",
            "cursola",
            "sirfetchd",
            "mr-rime"
        };

        private readonly string[] beforeUpdate =
        {
            "Glaceon",
            "Linoone",
            "Persian",
            "Corsola",
            "Farfetch'd",
            "Mr. Mime"
        };
        private const int STEPS_PER_HATCH_COUNTER = 256;
        private readonly string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Binaries", "PokedexFile.csv");
        private readonly string newFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Binaries", "NewCSV.csv");
        private readonly PokeApiClient pokeClient;
        private bool isCancelled;
        private List<PokemonEntity> records;
        private List<PokemonEntity> readRecords;
        private bool isExecuting;
        private ObservableCollection<string> logEntries;
        private RelayCommand readCommand;
        private RelayCommand writeCommand;
        private RelayCommand cancelCommand;

        public bool IsExecuting
        {
            get => isExecuting;
            set => SetProperty(ref isExecuting, value);
        }

        public ObservableCollection<string> LogEntries
        {
            get => logEntries;
            set => SetProperty(ref logEntries, value);
        }

        public RelayCommand ReadCommand => readCommand ??= new RelayCommand(async () => await ReadCommandExecute(), () => !IsExecuting);
        public RelayCommand WriteCommand => writeCommand ??= new RelayCommand(async () => await WriteCommandExecute(), () => !IsExecuting);
        public RelayCommand CancelCommand => cancelCommand ??= new RelayCommand(CancelCommandExecute, () => IsExecuting);

        public MainViewModel()
        {
            readRecords = new List<PokemonEntity>();
            LogEntries = new ObservableCollection<string>();
            pokeClient = new PokeApiClient();
        }

        public override void OnUnloaded()
        {
            pokeClient.Dispose();
            base.OnUnloaded();
        }

        private void CancelCommandExecute() => isCancelled = true;

        private async Task WriteCommandExecute() => await DoExecute(FillCSV);

        private async Task ReadCommandExecute() => await DoExecute(ReadCSV);

        private async Task DoExecute(Func<Task> func)
        {
            SetIsExecuting(true);
            LogEntries.Clear();
            try
            {
                await Task.Run(func);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                SetIsExecuting(false);
            }
        }

        private void AddLogEntry(string entry) => Application.Current.Dispatcher.Invoke(() => LogEntries.Add(entry));

        private async Task ReadCSV()
        {
            try
            {
                using var sr = new StreamReader(filename);
                using var csvReader = new CsvReader(sr, CultureInfo.InvariantCulture);
                using var sw = new StreamWriter(newFilePath, true);
                using var csvWriter = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture));

                var counter = 0;
                var increment = 0;
                readRecords = new List<PokemonEntity>();
                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    if (isCancelled)
                    {
                        DoCancel();
                        return;
                    }

                    var record = new PokemonEntity
                    {
                        ID = csvReader.GetField<float>("Per") + increment,
                        NationalDex = csvReader.GetField<float>("Nat"),
                        JohtoDex = string.IsNullOrEmpty(csvReader.GetField("Joh")) ? -1 : csvReader.GetField<float>("Joh"),
                        HoennDex = string.IsNullOrEmpty(csvReader.GetField("Hoe")) ? -1 : csvReader.GetField<float>("Hoe"),
                        SinnohDex = string.IsNullOrEmpty(csvReader.GetField("Sin")) ? -1 : csvReader.GetField<float>("Sin"),
                        UnovaDex = string.IsNullOrEmpty(csvReader.GetField("Un")) ? -1 : csvReader.GetField<float>("Un"),
                        KalosDex = -1,
                        AlolaDex = -1,
                        GalarDex = -1,
                        HP = csvReader.GetField<int>("HP"),
                        Atk = csvReader.GetField<int>("Atk"),
                        Def = csvReader.GetField<int>("Def"),
                        SpA = csvReader.GetField<int>("SpA"),
                        SpD = csvReader.GetField<int>("SpD"),
                        Spe = csvReader.GetField<int>("Spe"),
                        Total = csvReader.GetField<int>("Total"),
                        Name = csvReader.GetField("Pokemon"),
                        Type1 = csvReader.GetField("Type1"),
                        Type2 = csvReader.GetField("Type2"),
                        Mass = csvReader.GetField("Mass"),
                        LKGK = csvReader.GetField<int>("LK/GK"),
                        EXPV = csvReader.GetField<int>("EXPV"),
                        Color = csvReader.GetField("Color"),
                        Hatch = csvReader.GetField("Hatch"),
                        Gender = csvReader.GetField("EggGroup1"),
                        Ability1 = csvReader.GetField("Ability1"),
                        Ability2 = csvReader.GetField("Ability2"),
                        HiddenAbility = csvReader.GetField("HiddenAbility"),
                        EggGroup1 = csvReader.GetField("EggGroup1"),
                        EggGroup2 = csvReader.GetField("EggGroup2"),
                        Catch = csvReader.GetField<int>("Catch"),
                        EXP = csvReader.GetField<int>("EXP"),
                        Evolve = csvReader.GetField("Evolve"),
                        EvolveNum = csvReader.GetField("EvolveNum"),
                        EVYield = csvReader.GetField("EVYield"),
                    };

                    if (beforeUpdate.Contains(record.Name))
                        increment++;
                    readRecords.Add(record);
                    counter++;
                    AddLogEntry($"{counter} records(s) read.");
                }

                csvWriter.WriteRecords(readRecords);
                AddLogEntry($"Done reading.");
                SetIsExecuting(false);
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message);
                DoCancel();
            }
        }

        private async Task FillCSV()
        {
            if (!File.Exists(newFilePath))
            {
                AddLogEntry($"{Path.GetFileName(newFilePath)} doesn't exist. Run Read CSV.");
                DoCancel();
            }

            while (!IsFileReady(newFilePath))
            {
                if (isCancelled)
                {
                    DoCancel();
                    return;
                }
            }

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };
                using var writer = new StreamWriter(newFilePath, true);
                using var csv = new CsvWriter(writer, config);
                var added = 0;
                records = new List<PokemonEntity>();
                for (var i = 650; i <= 898; i++)
                {
                    if (isCancelled)
                    {
                        DoCancel();
                        return;
                    }

                    var pkmn = await pokeClient.GetResourceAsync<Pokemon>(i);
                    var pkmnSpecies = await pokeClient.GetResourceAsync<PokemonSpecies>(i);
                    var evolutionChain = await pokeClient.GetResourceAsync(pkmnSpecies.EvolutionChain);
                    var growth = await pokeClient.GetResourceAsync(pkmnSpecies.GrowthRate);

                    try
                    {
                        var record = CreateEntity(updateNum.Contains(pkmnSpecies.Name) ? GetUpdatedNumber(pkmnSpecies) : i + 6, pkmn, pkmnSpecies, evolutionChain, growth);
                        records.Add(record);
                        csv.WriteRecord(record);
                        csv.NextRecord();
                        added++;
                        AddLogEntry($"{added} record(s) added");
                        if (pkmnSpecies.Varieties.Count > 1)
                            AddForms(csv, i, pkmnSpecies, evolutionChain, growth);
                    }
                    catch (Exception ex)
                    {
                        AddLogEntry($"{ex.Message} {pkmn.Name}");
                    }
                }

                //csv.WriteRecords(records);
                AddLogEntry("done");
                SetIsExecuting(false);
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message);
                DoCancel();
            }
        }

        private void DoCancel() => Application.Current.Dispatcher.Invoke(() =>
            {
                AddLogEntry("Cancelled.");
                SetIsExecuting(false);
                isCancelled = false;
            });

        private int GetUpdatedNumber(PokemonSpecies pkmnSpecies)
        {
            using var sr = new StreamReader(filename);
            using var csvReader = new CsvReader(sr, CultureInfo.InvariantCulture);
            PokemonEntity record = null;
            record = pkmnSpecies.Name switch
            {
                "sylveon" => readRecords.First(x => x.Name == "Glaceon"),
                "obstagoon" => readRecords.First(x => x.Name == "Linoone"),
                "perrserker" => readRecords.First(x => x.Name == "Persian"),
                "cursola" => readRecords.First(x => x.Name == "Corsola"),
                "sirfetchd" => readRecords.First(x => x.Name == "Farfetch'd"),
                "mr-rime" => readRecords.First(x => x.Name == "Mr. Mime"),
                _ => throw new ArgumentOutOfRangeException(nameof(pkmnSpecies.Name), pkmnSpecies.Name, "Out of range."),
            };
            return (int)record.ID + 1;
        }

        private PokemonEntity CreateEntity(float i, Pokemon pkmn, PokemonSpecies pkmnSpecies, EvolutionChain evolutionChain, GrowthRate growth, string formName = null, float? id = null)
        => new()
        {
            ID = i,
            NationalDex = id ?? pkmn.Id,
            JohtoDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("updated-johto"))?.EntryNumber ?? -1,
            HoennDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("hoenn"))?.EntryNumber ?? -1,
            SinnohDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("updated-sinnoh"))?.EntryNumber ?? -1,
            UnovaDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("unova"))?.EntryNumber ?? -1,
            KalosDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("kalos-central"))?.EntryNumber ?? -1,
            AlolaDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("alola"))?.EntryNumber ?? -1,
            GalarDex = pkmnSpecies.PokedexNumbers.FirstOrDefault(x => x.Pokedex.Name.Contains("galar"))?.EntryNumber ?? -1,
            HP = pkmn.Stats.First(x => x.Stat.Name == "hp").BaseStat,
            Atk = pkmn.Stats.First(x => x.Stat.Name == "attack").BaseStat,
            Def = pkmn.Stats.First(x => x.Stat.Name == "defense").BaseStat,
            SpA = pkmn.Stats.First(x => x.Stat.Name == "special-attack").BaseStat,
            SpD = pkmn.Stats.First(x => x.Stat.Name == "special-defense").BaseStat,
            Spe = pkmn.Stats.First(x => x.Stat.Name == "speed").BaseStat,
            Total = pkmn.Stats.Sum(x => x.BaseStat),
            Name = formName ?? pkmnSpecies.Name.FirstCharToUpper(),
            Type1 = pkmn.Types.First(x => x.Slot == 1).Type.Name.FirstCharToUpper(),
            Type2 = pkmn.Types.FirstOrDefault(x => x.Slot == 2)?.Type?.Name?.FirstCharToUpper(),
            Mass = $"{(float)pkmn.Weight / 10} kG",
            LKGK = CalculateLKGK((float)pkmn.Weight / 10),
            EXPV = pkmn.BaseExperience,
            Color = pkmnSpecies.Color.Name.FirstCharToUpper(),
            Hatch = (pkmnSpecies.HatchCounter * STEPS_PER_HATCH_COUNTER).ToString(),
            Gender = GetGenderRate(pkmnSpecies.GenderRate),
            Ability1 = GetFriendlyName(pkmn.Abilities.First(x => x.Slot == 1).Ability.Name),
            Ability2 = GetFriendlyName(pkmn.Abilities.FirstOrDefault(x => x.Slot == 2)?.Ability?.Name),
            HiddenAbility = GetFriendlyName(pkmn.Abilities.FirstOrDefault(x => x.IsHidden)?.Ability?.Name),
            EggGroup1 = pkmnSpecies.EggGroups.Count > 0 ? EggGroupName(pkmnSpecies.EggGroups[0].Name) : null,
            EggGroup2 = pkmnSpecies.EggGroups.Count > 1 ? EggGroupName(pkmnSpecies.EggGroups[1].Name) : null,
            Catch = pkmnSpecies.CaptureRate,
            EXP = growth.Levels.Last().Experience,
            Evolve = pkmnSpecies.EvolvesFromSpecies == null ? string.Empty : GetEvolveString(evolutionChain, pkmnSpecies.Name),
            EvolveNum = evolutionChain.Chain.EvolvesTo.Count > 1 ? evolutionChain.Chain.EvolvesTo.Count.ToString() : "",
            EVYield = GetEvs(pkmn)
        };

        #region Static helper methods

        private static string GetGenderRate(int genderRate)
            => genderRate == -1 ? "None" : genderRate == 4 ? "NEUTRAL" : genderRate > 4 ? $"F ({genderRate / 8.0f * 100}%))" : $"M ({(8 - genderRate) / 8.0f * 100}%)";

        private static int CalculateLKGK(float weight) => weight switch
        {
            <= 10 => 20,
            > 10 and <= 25 => 40,
            > 25 and <= 50 => 60,
            > 50 and <= 100 => 80,
            > 100 and <= 200 => 100,
            _ => 120
        };

        private static string EggGroupName(string name) => name switch
        {
            "plant" => "Grass",
            "indeterminate" => "Amorphous",
            "no-eggs" => "Unknown",
            "ground" => "Field",
            "humanshape" => "Humanlike",
            _ => name.FirstCharToUpper(),
        };

        private static string ParseAndGetEvolveString(EvolutionDetail details)
        {
            var method = string.Empty;
            if (details.MinLevel != null)
                method = $"Lv. {details.MinLevel}";
            if (details.Gender != null)
                method = $"Lv. {details.MinLevel} {(details.Gender == 1 ? "M" : "F")}";
            if (details.HeldItem != null)
                method = $"{(details.MinLevel != null ? $"Lv. {details.MinLevel}" : "Trade")} h/ {GetFriendlyName(details.HeldItem.Name)}";
            if (details.Item != null)
                method = GetFriendlyName(details.Item.Name);
            if (details.KnownMove != null)
                method = $"Level w/ {GetFriendlyName(details.KnownMove.Name)}";
            if (details.KnownMoveType != null)
                method = $"{(details.MinHappiness != null ? "Friendship" : "Level")} w/ {GetFriendlyName(details.KnownMoveType.Name)} move";
            if (details.Location != null)
                method = $"Level a/ {GetFriendlyName(details.Location.Name)}";
            if (details.MinAffection != null)
                method = $"Friendship w/ {GetFriendlyName(details.KnownMoveType.Name)} move"; //should only be sylveon
            if (details.MinBeauty != null)
                method = "";
            if (details.MinHappiness != null)
                method = "Friendship";
            if (details.NeedsOverworldRain)
                method = $"Lv. {details.MinLevel} in rain";
            if (details.PartySpecies != null)
                method = $"Lv. {details.MinLevel} w/ {GetFriendlyName(details.PartySpecies.Name)} on team";
            if (details.PartyType != null)
                method = $"Lv. {details.MinLevel} w/ {GetFriendlyName(details.PartyType.Name)} on team";
            if (details.RelativePhysicalStats != null)
                method = "";
            if (!string.IsNullOrEmpty(details.TimeOfDay))
                method = $"{GetFriendlyName(details.TimeOfDay)} {(details.MinHappiness != null ? "Friendship" : $"Lv. {details.MinLevel}")}";
            if (details.TradeSpecies != null)
                method = $"Trade w/ {GetFriendlyName(details.TradeSpecies.Name)}";
            if (details.TurnUpsideDown)
                method = $"Lv. {details.MinLevel} upside down";
            return method;
        }

        private static string GetFriendlyName(string name) => string.IsNullOrEmpty(name) ? string.Empty : name.Replace('-', ' ').FirstCharToUpper();

        private static string ConvertStatName(string name) => name switch
        {
            "hp" => "HP",
            "attack" => "Atk",
            "defense" => "Def",
            "special-attack" => "SpA",
            "special-defense" => "SpD",
            "speed" => "Spe",
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Out of range."),
        };

        private static string GetEvs(Pokemon pkmn)
        {
            var evs = new StringBuilder();
            foreach (var stat in pkmn.Stats)
                if (stat.Effort > 0)
                    evs.Append($"{stat.Effort} {ConvertStatName(stat.Stat.Name)}/");
            evs.Remove(evs.Length - 1, 1);
            return evs.ToString();
        }

        private static bool IsFileReady(string filename)
        {
            try
            {
                using var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Helper methods

        private async void AddForms(CsvWriter csv, float i, PokemonSpecies species, EvolutionChain chain, GrowthRate growth)
        {
            var toAddToForm = 0.1f;
            for (var j = 1; j < species.Varieties.Count; j++)
            {
                if (species.Varieties[j].Pokemon.Name == "greninja-battle-bond")
                    continue;

                var pkmn = await pokeClient.GetResourceAsync(species.Varieties[j].Pokemon);
                var pkmnSpecies = await pokeClient.GetResourceAsync(pkmn.Species);
                var split = pkmn.Name.Split('-');
                var s1 = split[0].FirstCharToUpper();
                var s2 = split[1].FirstCharToUpper().Insert(split[1].Length, ")").Insert(0, " (");
                var formName = string.Concat(s1, s2);
                var added = 0;
                try
                {
                    var record = CreateEntity(i + toAddToForm, pkmn, pkmnSpecies, chain, growth, formName, i + toAddToForm);
                    records.Add(record);
                    csv.WriteRecord(record);
                    csv.NextRecord();
                    added++;
                    toAddToForm += 0.1f;
                    AddLogEntry($"{added} form(s) added");
                }
                catch (Exception ex)
                {
                    AddLogEntry($"{ex.Message} {pkmn.Name}");
                }
            }
        }

        private string GetEvolveString(EvolutionChain evolution, string name)
        {
            var details = TraverseChain(evolution.Chain, name);
            return details.Species.Name == "melmetal" ? "400 candy in PKMN Go" : ParseAndGetEvolveString(details.EvolutionDetails[0]);
        }

        private ChainLink TraverseChain(ChainLink chain, string name) => chain.Species.Name == name ? chain : chain.EvolvesTo.Count > 1 ? TraverseChain(chain.EvolvesTo.First(x => x.Species.Name == name), name) : TraverseChain(chain.EvolvesTo[0], name);

        private void SetIsExecuting(bool executing)
            => Application.Current.Dispatcher.Invoke(() =>
            {
                IsExecuting = executing;
                CommandManager.InvalidateRequerySuggested();
            });

        #endregion
    }
}
