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
using System.Windows.Input;
#pragma warning disable IDE0058
namespace Pokedex.PokedexCSVCreator.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private const int STEPS_PER_HATCH_COUNTER = 256;
        private static readonly string binaryDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Binaries");
        private static readonly string filename = Path.Combine(binaryDirectory, "PokedexFile.csv");
        private static readonly string newFilePath = Path.Combine(binaryDirectory, "NewCSV.csv");
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

        private Task ReadCommandExecute() => throw new NotImplementedException();//await DoExecute(ReadCSV);

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

        //private async Task ReadCSV()
        //{
        //    if (!File.Exists(filename))
        //    {
        //        ShowMessage("CSV file doesn't exist.");
        //        return;
        //    }

        //    try
        //    {
        //        //using var sr = new StreamReader(filename);
        //        //using var csvReader = new CsvReader(sr, CultureInfo.InvariantCulture);
        //        //using var sw = new StreamWriter(newFilePath, true);
        //        //using var csvWriter = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture));

        //        //var counter = 0;
        //        //var increment = 0;
        //        //readRecords = new List<PokemonEntity>();
        //        //csvReader.Read();
        //        //csvReader.ReadHeader();

        //        //while (csvReader.Read())
        //        //{
        //        //    if (isCancelled)
        //        //    {
        //        //        DoCancel();
        //        //        return;
        //        //    }

        //        //    var record = new PokemonEntity
        //        //    {
        //        //        ID = csvReader.GetField<int>("Per"),
        //        //        NationalDex = csvReader.GetField<float>("Nat"),
        //        //        JohtoDex = string.IsNullOrEmpty(csvReader.GetField("Joh")) ? -1 : csvReader.GetField<float>("Joh"),
        //        //        HoennDex = string.IsNullOrEmpty(csvReader.GetField("Hoe")) ? -1 : csvReader.GetField<float>("Hoe"),
        //        //        SinnohDex = string.IsNullOrEmpty(csvReader.GetField("Sin")) ? -1 : csvReader.GetField<float>("Sin"),
        //        //        UnovaDex = string.IsNullOrEmpty(csvReader.GetField("Un")) ? -1 : csvReader.GetField<float>("Un"),
        //        //        KalosDex = -1,
        //        //        AlolaDex = -1,
        //        //        GalarDex = -1,
        //        //        HP = csvReader.GetField<int>("HP"),
        //        //        Atk = csvReader.GetField<int>("Atk"),
        //        //        Def = csvReader.GetField<int>("Def"),
        //        //        SpA = csvReader.GetField<int>("SpA"),
        //        //        SpD = csvReader.GetField<int>("SpD"),
        //        //        Spe = csvReader.GetField<int>("Spe"),
        //        //        Total = csvReader.GetField<int>("Total"),
        //        //        Name = csvReader.GetField("Pokemon"),
        //        //        Type1 = csvReader.GetField("Type1"),
        //        //        Type2 = csvReader.GetField("Type2"),
        //        //        Mass = csvReader.GetField("Mass"),
        //        //        LKGK = csvReader.GetField<int>("LK/GK"),
        //        //        EXPV = csvReader.GetField<int>("EXPV"),
        //        //        Color = csvReader.GetField("Color"),
        //        //        Hatch = csvReader.GetField("Hatch"),
        //        //        Gender = csvReader.GetField("EggGroup1"),
        //        //        Ability1 = csvReader.GetField("Ability1"),
        //        //        Ability2 = csvReader.GetField("Ability2"),
        //        //        HiddenAbility = csvReader.GetField("HiddenAbility"),
        //        //        EggGroup1 = csvReader.GetField("EggGroup1"),
        //        //        EggGroup2 = csvReader.GetField("EggGroup2"),
        //        //        Catch = csvReader.GetField<int>("Catch"),
        //        //        EXP = csvReader.GetField<int>("EXP"),
        //        //        Evolve = csvReader.GetField("Evolve"),
        //        //        EvolveNum = csvReader.GetField("EvolveNum"),
        //        //        EVYield = csvReader.GetField("EVYield"),
        //        //    };

        //        //    if (beforeUpdate.Contains(record.Name))
        //        //        increment++;
        //        //    readRecords.Add(record);
        //        //    counter++;
        //        //    AddLogEntry($"{counter} records(s) read.");
        //        //}

        //        //await csvWriter.WriteRecordsAsync(readRecords);
        //        //AddLogEntry($"Done reading.");
        //        //SetIsExecuting(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        AddLogEntry(ex.Message);
        //        DoCancel();
        //    }
        //}

        private async Task FillCSV()
        {
            try
            {
                Directory.CreateDirectory(binaryDirectory);
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                using var writer = new StreamWriter(newFilePath, true);
                using var csv = new CsvWriter(writer, config);
                var added = 0;
                records = new List<PokemonEntity>();
                csv.WriteHeader<PokemonEntity>();
                csv.NextRecord();
                var idCounter = 1;
                for (var i = 1; i <= 898; i++)
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
                        string defaultFormName = null;
                        if (!IsKommooLine(pkmn.Name) && !IsMrMimeLine(pkmn.Name) && pkmnSpecies.Varieties.Count > 1 && pkmn.Name.Contains('-') && pkmn.Name == pkmnSpecies.Varieties.First(x => x.IsDefault).Pokemon.Name)
                            defaultFormName = GetFormName(pkmn.Name);
                        var record = CreateEntity(ref idCounter, pkmn, pkmnSpecies, evolutionChain, growth, false, defaultFormName);
                        SetDexNumbers(record, pkmnSpecies);
                        records.Add(record);
                        csv.WriteRecord(record);
                        csv.NextRecord();
                        added++;
                        AddLogEntry($"{added} record(s) added");
                        if (pkmnSpecies.Varieties.Count > 1)
                            AddForms(csv, ref idCounter, pkmnSpecies, evolutionChain, growth);
                    }
                    catch (Exception ex)
                    {
                        AddLogEntry($"{ex.Message} {pkmn.Name}");
                    }
                }

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
            return record.ID + 1;
        }

        private PokemonEntity CreateEntity(ref int i, Pokemon pkmn, PokemonSpecies pkmnSpecies, EvolutionChain evolutionChain, GrowthRate growth, bool isForm, string formName = null)// float addToForm = 0)
        => new()
        {
            ID = i++,
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
            Ability2 = GetFriendlyName(pkmn.Abilities.FirstOrDefault(x => x.Slot == 2)?.Ability?.Name) ?? "None",
            HiddenAbility = GetFriendlyName(pkmn.Abilities.FirstOrDefault(x => x.IsHidden)?.Ability?.Name),
            EggGroup1 = pkmnSpecies.EggGroups.Count > 0 ? EggGroupName(pkmnSpecies.EggGroups[0].Name) : null,
            EggGroup2 = pkmnSpecies.EggGroups.Count > 1 ? EggGroupName(pkmnSpecies.EggGroups[1].Name) : null,
            Catch = pkmnSpecies.CaptureRate,
            EXP = growth.Levels.Last().Experience,
            Evolve = pkmnSpecies.EvolvesFromSpecies == null ? string.Empty : GetEvolveString(evolutionChain, pkmnSpecies.Name),
            EvolveNum = evolutionChain.Chain.EvolvesTo.Count > 1 ? evolutionChain.Chain.EvolvesTo.Count.ToString() : "",
            EVYield = GetEvs(pkmn),
            HasForms = pkmnSpecies.Varieties.Count > 1 ? 1 : 0,
            IsForm = isForm ? 1 : 0,
            PrevEvolution = string.Join(',', GetPreviousEvolutions(evolutionChain.Chain, pkmnSpecies.Name, false)),
            NextEvolution = string.Join(',', GetNextEvolutions(evolutionChain.Chain, pkmnSpecies.Name, false))
        };

        #region Static helper methods

        private static string GetGenderRate(int genderRate)
            => genderRate == -1 ? "None" : genderRate == 4 ? "Neutral" : genderRate > 4 ? $"F ({genderRate / 8.0f * 100}%))" : $"M ({(8 - genderRate) / 8.0f * 100}%)";

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

        private static bool IsKommooLine(string name) => name.Contains("jangmo-o") || name.Contains("hakamo-o") || name.Contains("kommo-o");
        private static bool IsMrMimeLine(string name) => name.Contains("mr-mime") || name.Contains("mime-jr") || name.Contains("mr-rime");

        private static void SetDexNumbers(PokemonEntity entity, PokemonSpecies pkmnSpecies)
        {
            entity.NationalDex = entity.JohtoDex = entity.HoennDex = entity.SinnohDex = entity.UnovaDex = entity.KalosDex = entity.AlolaDex = entity.GalarDex = -1;

            foreach (var dex in pkmnSpecies.PokedexNumbers)
            {
                switch (dex.Pokedex.Name)
                {
                    case "kanto":
                    case "letsgo-kanto":
                    case "updated-kanto":
                    case "original-johto":
                    case "updated-hoenn":
                    case "extended-sinnoh":
                    case "updated-unova":
                    case "kalos-coastal":
                    case "kalos-mountain":
                    case "updated-alola":
                    case "updated-akala":
                    case "updated-melemele":
                    case "updated-poni":
                    case "updated-ulaula":
                    case "original-akala":
                    case "original-melemele":
                    case "original-poni":
                    case "original-ulaula":
                    case "crown-tundra":
                    case "isle-of-armor":
                    case "conquest-gallery":
                        break;
                    case "national":
                        entity.NationalDex = dex.EntryNumber;
                        break;
                    case "updated-johto":
                        entity.JohtoDex = dex.EntryNumber;
                        break;
                    case "hoenn":
                        entity.HoennDex = dex.EntryNumber;
                        break;
                    case "original-sinnoh":
                        entity.SinnohDex = dex.EntryNumber;
                        break;
                    case "original-unova":
                        entity.UnovaDex = dex.EntryNumber;
                        break;
                    case "kalos-central":
                        entity.KalosDex = dex.EntryNumber;
                        break;
                    case "original-alola":
                        entity.AlolaDex = dex.EntryNumber;
                        break;
                    case "galar":
                        entity.GalarDex = dex.EntryNumber;
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(dex.Pokedex.Name), dex.Pokedex.Name, "No dex.");
                }
            }
        }

        private static string GetFormName(string name)
        {
            if (name.Contains("noice"))
                name = "eiscue-no-ice";

            var split = IsKommooLine(name) || IsMrMimeLine(name) ? StringExtensions.SplitBy(name, '-', 2).ToArray() : name.Split('-', 2);
            var s1 = split[0].FirstCharToUpper();
            var s2 = split[1].FirstCharToUpper().Insert(split[1].Length, ")").Insert(0, " (");
            return string.Concat(s1, s2);
        }

        private static string GetEvolveString(EvolutionChain evolution, string name)
        {
            var details = TraverseChain(evolution.Chain, name);
            return details.Species.Name == "melmetal" ? "400 candy in PKMN Go" : ParseAndGetEvolveString(details.EvolutionDetails[0]);
        }

        private static ChainLink TraverseChain(ChainLink chain, string name)
        {
            if (chain.Species.Name == name)
                return chain;
            if (chain.EvolvesTo.Count > 1 && chain.EvolvesTo.FirstOrDefault(x => x.Species.Name == name) is ChainLink c)
                return TraverseChain(c, name);
            return TraverseChain(GetCorrectEvolutionPath(chain.EvolvesTo, name), name);
        }

        private static ChainLink GetCorrectEvolutionPath(List<ChainLink> chain, string name)
        {
            foreach (var item in chain)
            {
                if (item.Species.Name == name)
                    return item;
                foreach (var subItem in item.EvolvesTo)
                    if (subItem.Species.Name == name)
                        return subItem;
            }
            throw new Exception("Evolution chain doesn't exist");
        }

        private static List<string> GetPreviousEvolutions(ChainLink evolutionChain, string name, bool startAdding)
        {
            if (name == "beautifly")
            {

            }
            var prevs = new List<string>();
            //if (startAdding && evolutionChain.Species.Name == name)
            //    prevs.Add(evolutionChain.Species.Name);
            if (evolutionChain.Species.Name == name)
                return prevs;
            else
                prevs.Add(evolutionChain.Species.Name);
            if (evolutionChain.EvolvesTo.Count > 1)
            {
                for (var i = 0; i < evolutionChain.EvolvesTo.Count; i++)
                {
                    if (evolutionChain.EvolvesTo[i].EvolvesTo.Count > 0)
                    {
                        if (evolutionChain.EvolvesTo[i].EvolvesTo[0].Species.Name != name)
                            continue;
                    }
                    else if (evolutionChain.Species.Name != name && evolutionChain.EvolvesTo[i].Species.Name != name)
                        continue;

                    prevs.AddRange(GetPreviousEvolutions(evolutionChain.EvolvesTo[i], name, startAdding));
                    break;
                }
            }
            else if (evolutionChain.EvolvesTo.Count == 1)
                prevs.AddRange(GetPreviousEvolutions(evolutionChain.EvolvesTo[0], name, startAdding));

            return prevs;
        }

        private static List<string> GetNextEvolutions(ChainLink evolutionChain, string name, bool startAdding)
        {
            if (name == "ralts")
            {

            }
            if (name == "pidgeot")
            {

            }
            var nexts = new List<string>();
            if (startAdding)
                nexts.Add(evolutionChain.Species.Name);
            if (evolutionChain.Species.Name == name)
                startAdding = true;
            if (evolutionChain.EvolvesTo.Count > 1)
            {
                for (var i = 0; i < evolutionChain.EvolvesTo.Count; i++)
                {
                    if (evolutionChain.Species.Name != name && evolutionChain.EvolvesTo[i].Species.Name != name && evolutionChain.EvolvesTo[i].EvolvesTo.Count > 0 && evolutionChain.EvolvesTo[i].EvolvesTo[0].Species.Name != name)
                        continue;
                    //else if (evolutionChain.EvolvesTo[i].EvolvesTo.Count > 0)
                    //{
                    //    if (evolutionChain.EvolvesTo[i].EvolvesTo[0].Species.Name != name)
                    //        continue;
                    //}
                    

                    nexts.AddRange(GetNextEvolutions(evolutionChain.EvolvesTo[i], name, startAdding));
                }
            }
            else if (evolutionChain.EvolvesTo.Count == 1)
                nexts.AddRange(GetNextEvolutions(evolutionChain.EvolvesTo[0], name, startAdding));

            return nexts;
        }

        #endregion

        #region Helper methods

        private void AddForms(CsvWriter csv, ref int i, PokemonSpecies species, EvolutionChain chain, GrowthRate growth)
        {
            for (var j = 1; j < species.Varieties.Count; j++)
            {
                if (species.Varieties[j].Pokemon.Name == "greninja-battle-bond")
                    continue;

                var pkmn = pokeClient.GetResourceAsync(species.Varieties[j].Pokemon).Result;
                var pkmnSpecies = pokeClient.GetResourceAsync(pkmn.Species).Result;
                var formName = GetFormName(pkmn.Name);
                var added = 0;
                try
                {
                    var record = CreateEntity(ref i, pkmn, pkmnSpecies, chain, growth, true, formName);
                    SetDexNumbers(record, pkmnSpecies);
                    records.Add(record);
                    csv.WriteRecord(record);
                    csv.NextRecord();
                    added++;
                    AddLogEntry($"{added} form(s) added");
                }
                catch (Exception ex)
                {
                    AddLogEntry($"{ex.Message} {pkmn.Name}");
                }
            }
        }

        private void SetIsExecuting(bool executing)
            => Application.Current.Dispatcher.Invoke(() =>
            {
                IsExecuting = executing;
                CommandManager.InvalidateRequerySuggested();
            });

        #endregion
    }
}
