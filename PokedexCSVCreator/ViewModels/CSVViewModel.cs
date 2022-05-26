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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Pokedex.PokedexCSVCreator.CSVHelpers.EvolutionHelpers;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase;
using System.Threading;

#pragma warning disable IDE0058
namespace Pokedex.PokedexCSVCreator.ViewModels
{
    public class CSVViewModel : ViewModel
    {
        private const int STEPS_PER_HATCH_COUNTER = 256;
        private static readonly string binaryDirectory = Path.Combine(AppContext.BaseDirectory, "..\\", "..\\", "..\\", "assemblies");
        private static readonly string filename = Path.Combine(binaryDirectory, "PokedexFile.csv");
        private static readonly string newFilePath = Path.Combine(binaryDirectory, "NewCSV.csv");
        private static readonly string[] alolanForms = new string[] { "Rattata", "Raticate", "Raichu", "Sandshrew", "Sandslash", "Vulpix", "Ninetales", "Diglett", "Dugtrio", "Geodude", "Graveler", "Golem", "Grimer", "Muk", "Exeggutor", "Marowak" };
        private static readonly string[] alolanThatEvolveFromNormalForms = new string[] { "Raichu", "Exeggutor", "Marowak" };
        private static readonly string[] galarForms = new string[] { "Meowth", "Ponyta", "Rapidash", "Slowpoke", "Slowbro", "Farfetchd", "Weezing", "Mr. Mime", "Articuno", "Zapdos", "Moltres", "Slowking", "Corsola", "Zigzagoon", "Linoone", "Darumaka", "Darmanitan", "Yamask", "Stunfisk" };
        private static readonly string[] galarEvolutions = new string[] { "Obstagoon", "Perrserker", "Cursola", "Sirfetchd", "Mr. Rime", "Runerigus" };
        private static readonly string[] galarianThatEvolveFromNormalForms = new string[] { "Weezing", "Mr. Mime" };
        private readonly PokeApiClient pokeClient;
        private List<PokemonEntity> records;
        private bool isExecuting;
        private ObservableCollection<string> logEntries;
        private RelayCommand writeCommand;
        private RelayCommand cancelCommand;
        private RelayCommand createDbCommand;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;

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

        public RelayCommand WriteCommand => writeCommand ??= new RelayCommand(async () => await WriteCommandExecute(), () => !IsExecuting);
        public RelayCommand CreateDbCommand => createDbCommand ??= new RelayCommand(async () => await CreateDbCommandExecute(), () => !IsExecuting);

        public RelayCommand CancelCommand => cancelCommand ??= new RelayCommand(CancelCommandExecute, () => IsExecuting);

        public CSVViewModel()
        {
            LogEntries = new ObservableCollection<string>();
            pokeClient = new PokeApiClient();
        }

        public override void OnUnloaded()
        {
            pokeClient.Dispose();
            base.OnUnloaded();
        }

        private async Task WriteCommandExecute()
        {
            SetIsExecuting(true);
            LogEntries.Clear();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            try
            {
                await Task.Run(FillCSV, token);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                SetIsExecuting(false);
            }
        }

        private async Task CreateDbCommandExecute()
        {
            SetIsExecuting(true);
            LogEntries.Clear();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            try
            {
                await Task.Run(() => PopulateDB.PopulateDatabase(token, tokenSource, DoCancel, s => AddLogEntry(s)), token);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                SetIsExecuting(false);
            }
        }

        private void CancelCommandExecute() => tokenSource.Cancel();

        private async Task FillCSV()
        {
            if (token.IsCancellationRequested)
            {
                AddLogEntry("Operation cancelled.");
                return;
            }

            try
            {
                Directory.CreateDirectory(binaryDirectory);
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                var added = 0;
                var idCounter = 1;
                records = new List<PokemonEntity>();

                for (var i = 1; i <= 898; i++)
                {
                    if (token.IsCancellationRequested)
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
                        var fixedName = pkmn.Name.FixName();
                        if (!Utilities.IsKommooLine(fixedName) && !Utilities.IsMrMimeLine(fixedName) && pkmnSpecies.Varieties.Count > 1 && fixedName.Contains('-') && fixedName == pkmnSpecies.Varieties.First(x => x.IsDefault).Pokemon.Name)
                            defaultFormName = fixedName.GetFormName();
                        var record = CreateEntity(ref idCounter, pkmn, pkmnSpecies, growth, false, pkmn.IsDefault, defaultFormName);
                        added++;
                        AddLogEntry($"{added} record(s) added");
                        PostAddEntity(ref idCounter, record, pkmn, pkmnSpecies, growth);
                        records.Add(record);
                    }
                    catch (Exception ex)
                    {
                        AddLogEntry($"{ex.Message} {pkmn.Name}");
                    }
                }
                AddLogEntry("done");
                SetIsExecuting(false);

                records = records.OrderBy(x => x.ID).ToList();
                FillCSVWithEvolutions();
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message);
                DoCancel();
            }
        }

        private void FillCSVWithEvolutions()
        {
            foreach (var pkmn in records)
            {
                if (pkmn.Name.Contains("Pikachu") && pkmn.IsForm)
                    continue;

                AddPokemonEvolutions(pkmn);
            }
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            using (var writer = new StreamWriter(newFilePath, true))
            {
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteHeader<PokemonEntity>();
                    csv.NextRecord();
                    csv.WriteRecords(records);
                }
            }
        }

        private void AddPokemonEvolutions(PokemonEntity pkmn)
        {
            var pkmnSpecies = pokeClient.GetResourceAsync<PokemonSpecies>(pkmn.NationalDex).Result;
            var evolutionChain = pokeClient.GetResourceAsync(pkmnSpecies.EvolutionChain).Result;

            var query = records.Where(x => x.NationalDex == pkmn.NationalDex - 1);
            if (pkmn.IsAlolanForm)
                query = addAlolanClause();
            if (pkmn.IsGalarianForm)
                query = addGalarianClause();
            var prevPokemonRecord = query.FirstOrDefault();
            PokemonSpecies prevSpecies = null;
            bool isPreEvolution = false;
            if (prevPokemonRecord != null)
                isPreEvolution = EvolutionChainContainsName(evolutionChain.Chain, getName());

            if ((prevPokemonRecord == null || !isPreEvolution) && pkmnSpecies.EvolvesFromSpecies != null)
                prevSpecies = pokeClient.GetResourceAsync(pkmnSpecies.EvolvesFromSpecies).Result;

            pkmn.EvolvesFromRegionalForm = prevSpecies != null
                ? (pkmn.IsAlolanForm || pkmn.IsGalar) && (prevSpecies.Varieties.Any(x => x.Pokemon.Name.EndsWith("-alola")) || prevSpecies.Varieties.Any(x => x.Pokemon.Name.EndsWith("-galar")))
                : prevPokemonRecord != null && isPreEvolution && ((pkmn.IsAlolanForm && prevPokemonRecord.IsAlolanForm) || (pkmn.IsGalarianForm && prevPokemonRecord.IsGalarianForm));
            pkmn.Evolve = pkmnSpecies.EvolvesFromSpecies == null ? string.Empty : GetEvolveString(evolutionChain, pkmnSpecies.Name, pkmn.IsAlolanForm, pkmn.IsGalarianForm, pkmn.EvolvesFromRegionalForm);
            pkmn.PrevEvolution = GetPreviousEvolutions(pokeClient, pkmnSpecies, pkmn);
            pkmn.NextEvolution = string.Empty;
            SetNextEvolutionofPreviousPokemon(pkmnSpecies, pkmn);

            string getName() => !prevPokemonRecord.Name.Contains(" (", StringComparison.CurrentCulture) ? prevPokemonRecord.Name.ToLower() : prevPokemonRecord.Name[..prevPokemonRecord.Name.IndexOf(" (")].ToLower();
            IEnumerable<PokemonEntity> addAlolanClause() => records.Where(x => x.IsAlolanForm);
            IEnumerable<PokemonEntity> addGalarianClause() => records.Where(x => x.IsGalarianForm);
        }

        private void SetNextEvolutionofPreviousPokemon(PokemonSpecies species, PokemonEntity pkmn)
        {
            if (species.EvolvesFromSpecies == null)
            {
                pkmn.NextEvolution = string.Empty;
                return;
            }

            var prevSpecies = pokeClient.GetResourceAsync(species.EvolvesFromSpecies).Result;
            PokemonEntity prev = null;
            if (pkmn.IsAlolanForm && !alolanThatEvolveFromNormalForms.Contains(species.Name.FirstCharToUpper()))
                prev = records.FirstOrDefault(x => x.IsAlolanForm && x.NationalDex == GetDexEntryNumber(prevSpecies, "national"));
            else if (pkmn.IsGalar && !galarianThatEvolveFromNormalForms.Contains(species.Name.FirstCharToUpper()))
                prev = records.FirstOrDefault(x => x.IsGalarianForm && x.NationalDex == GetDexEntryNumber(prevSpecies, "national"));
            else
            {
                var form = string.Empty;
                if (pkmn.Name.Contains("Average"))
                    form = "Average";
                else if (pkmn.Name.Contains("Small"))
                    form = "Small";
                else if (pkmn.Name.Contains("Large"))
                    form = "Large";
                else if (pkmn.Name.Contains("Super"))
                    form = "Super";
                else if (pkmn.Name.Contains("Own-Tempo"))
                    form = "Own-Tempo";

                prev = records.FirstOrDefault(x => x.Name.Contains(form) && x.NationalDex == GetDexEntryNumber(prevSpecies, "national"));
            }
            if (string.IsNullOrEmpty(prev.NextEvolution))
            {
                prev.NextEvolution = pkmn.Name;
                prev.EvolveNum = "1";
            }
            else
            {
                prev.NextEvolution = string.Join(",", new string[] { prev.NextEvolution, pkmn.Name });
                if (pkmn.IsDefaultForm)
                {
                    var num = int.Parse(prev.EvolveNum);
                    prev.EvolveNum = (num + 1).ToString();
                }
            }
        }


        #region Helper methods

        private void AddForms(ref int i, PokemonSpecies species, GrowthRate growth)
        {
            var added = 0;
            for (var j = 1; j < species.Varieties.Count; j++)
            {
                if (species.Varieties[j].Pokemon.Name == "greninja-battle-bond" ||
                    species.Varieties[j].Pokemon.Name.Contains("totem") ||
                    species.Varieties[j].Pokemon.Name.Contains("-low-key-gmax") ||
                    species.Varieties[j].Pokemon.Name.Contains("-meteor"))
                    continue;

                var pkmn = pokeClient.GetResourceAsync(species.Varieties[j].Pokemon).Result;
                var pkmnSpecies = pokeClient.GetResourceAsync(pkmn.Species).Result;
                var formName = pkmn.Name.FixName().GetFormName();
                try
                {
                    var record = CreateEntity(ref i, pkmn, pkmnSpecies, growth, true, pkmn.IsDefault, formName);
                    PostAddForm(record, pkmnSpecies);
                    records.Add(record);
                    added++;
                    AddLogEntry($"{added} form(s) added");
                }
                catch (Exception ex)
                {
                    AddLogEntry($"{ex.Message} {pkmn.Name}");
                }
            }
        }

        private void PostAddEntity(ref int idCounter, PokemonEntity record, Pokemon pkmn, PokemonSpecies pkmnSpecies, GrowthRate growth)
        {
            if (record.Name == "Minior (Red-Meteor)")
                record.Name = "Minior";
            if (pkmnSpecies.Name == "zarude")
                AddZarude(ref idCounter, pkmn, pkmnSpecies, growth);
            if (pkmnSpecies.Varieties.Count > 1)
                AddForms(ref idCounter, pkmnSpecies, growth);
        }

        private void PostAddForm(PokemonEntity record, PokemonSpecies pkmnSpecies)
        {
            if (record.Name == "zarude")
                record.Name = "Zarude (Dada)";
            if (record.Name.Contains("Amped-Gmax"))
            {
                record.Name = "Toxtricity (Gmax)";
                record.Ability2 = "Plus/Minus";
            }
            if ((record.IsAlolanForm || record.IsGalar) && !string.IsNullOrEmpty(record.Evolve) && !record.EvolvesFromRegionalForm)
                AddFormsToEvolutionLine(record, pkmnSpecies);
        }

        private void AddZarude(ref int i, Pokemon pkmn, PokemonSpecies pkmnSpecies, GrowthRate growth)
        {
            try
            {
                var record = CreateEntity(ref i, pkmn, pkmnSpecies, growth, true, pkmn.IsDefault, null);
                record.Name = "Zarude (Dada)";
                records.Add(record);
                AddLogEntry($"zarude form(s) added");
            }
            catch (Exception ex)
            {
                AddLogEntry($"{ex.Message} {pkmn.Name}");
            }
        }

        private void AddFormsToEvolutionLine(PokemonEntity form, PokemonSpecies pkmnSpecies)
        {
            var prevPokemon = records.First(x => x.NationalDex == form.NationalDex - 1 && !x.IsForm);
            if (pkmnSpecies.EvolvesFromSpecies == null)
                return;
            var prev = pokeClient.GetResourceAsync(pkmnSpecies.EvolvesFromSpecies).Result;
            var prevRecord = records.FirstOrDefault(x => x.Name.ToLower().Contains(prev.Name) && !x.IsForm);
            if (prevRecord == null)
                return;

            var evs = prevRecord.NextEvolution.Split(',');
            evs = evs.Append(form.Name).ToArray();
            prevRecord.NextEvolution = string.Join(',', evs);
        }

        #endregion

        #region Static helper methods

        private static PokemonEntity CreateEntity(ref int i, Pokemon pkmn, PokemonSpecies pkmnSpecies, GrowthRate growth, bool isForm, bool isDefaultForm, string formName = null)
        {
            var entity = new PokemonEntity
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
                Ability1 = pkmn.Abilities.First(x => x.Slot == 1).Ability.Name.GetFriendlyName(),
                Ability2 = pkmn.Abilities.FirstOrDefault(x => x.Slot == 2)?.Ability?.Name?.GetFriendlyName() ?? "None",
                HiddenAbility = pkmn.Abilities.FirstOrDefault(x => x.IsHidden)?.Ability?.Name?.GetFriendlyName(),
                EggGroup1 = pkmnSpecies.EggGroups.Count > 0 ? EggGroupName(pkmnSpecies.EggGroups[0].Name) : null,
                EggGroup2 = pkmnSpecies.EggGroups.Count > 1 ? EggGroupName(pkmnSpecies.EggGroups[1].Name) : null,
                Catch = pkmnSpecies.CaptureRate,
                EXP = growth.Levels[^1].Experience,
                EVYield = GetEvs(pkmn),
                HasForms = pkmnSpecies.Varieties.Count > 1,
                IsForm = isForm || formName != null && formName.Contains('('),
                IsDefaultForm = isDefaultForm,
                IsAlolanForm = pkmn.Name.Contains("-alola"),
                IsGalarianForm = pkmn.Name.Contains("-galar"),
                IsGalarianEvolution = galarEvolutions.Contains(pkmn.Name.FirstCharToUpper())
            };
            SetDexNumbers(entity, pkmnSpecies);
            return entity;
        }
        private static string GetGenderRate(int genderRate) =>
            genderRate == -1 ? "None" : genderRate == 4 ? "Neutral" : genderRate > 4 ? $"F ({genderRate / 8.0f * 100}%))" : $"M ({(8 - genderRate) / 8.0f * 100}%)";

        private static int CalculateLKGK(float weight) =>
            weight switch
            {
                <= 10 => 20,
                > 10 and <= 25 => 40,
                > 25 and <= 50 => 60,
                > 50 and <= 100 => 80,
                > 100 and <= 200 => 100,
                _ => 120
            };

        private static string EggGroupName(string name) =>
            name switch
            {
                "plant" => "Grass",
                "indeterminate" => "Amorphous",
                "no-eggs" => "Unknown",
                "ground" => "Field",
                "humanshape" => "Human-like",
                _ => name.FirstCharToUpper(),
            };

        private static string ConvertStatName(string name) =>
            name switch
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

        private static void SetDexNumbers(PokemonEntity entity, PokemonSpecies pkmnSpecies)
        {
            entity.NationalDex = entity.JohtoDex = entity.HoennDex = entity.SinnohDex = entity.UnovaDex = entity.KalosDex = entity.AlolaDex = entity.GalarDex = entity.IsleOfArmorDex = entity.CrownTundraDex = -1;

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
                    case "isle-of-armor":
                        entity.IsleOfArmorDex = dex.EntryNumber;
                        break;
                    case "crown-tundra":
                        entity.CrownTundraDex = dex.EntryNumber;
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(dex.Pokedex.Name), dex.Pokedex.Name, "No dex.");
                }
            }
        }

        #endregion

        #region Dispatcher methods

        private void AddLogEntry(string entry) =>
            Application.Current.Dispatcher.Invoke(() => LogEntries.Add(entry));

        private void DoCancel() =>
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddLogEntry("Cancelled.");
                SetIsExecuting(false);
                tokenSource.Dispose();
                tokenSource = null;
            });

        private void SetIsExecuting(bool executing) =>
            Application.Current.Dispatcher.Invoke(() =>
            {
                IsExecuting = executing;
                CommandManager.InvalidateRequerySuggested();
            });

        #endregion
    }
}