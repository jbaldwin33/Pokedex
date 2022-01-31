﻿using CsvHelper;
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
#pragma warning disable IDE0058
namespace Pokedex.PokedexCSVCreator.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private const int STEPS_PER_HATCH_COUNTER = 256;
        private static readonly string binaryDirectory = Path.Combine(AppContext.BaseDirectory, "..\\", "..\\", "..\\", "assemblies");
        private static readonly string filename = Path.Combine(binaryDirectory, "PokedexFile.csv");
        private static readonly string newFilePath = Path.Combine(binaryDirectory, "NewCSV.csv");
        private readonly PokeApiClient pokeClient;
        private bool isCancelled;
        private List<PokemonEntity> records;
        private bool isExecuting;
        private ObservableCollection<string> logEntries;
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

        public RelayCommand WriteCommand => writeCommand ??= new RelayCommand(async () => await WriteCommandExecute(), () => !IsExecuting);
        public RelayCommand CancelCommand => cancelCommand ??= new RelayCommand(() => isCancelled = true, () => IsExecuting);

        public MainViewModel()
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
            try
            {
                await Task.Run(FillCSV);
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                SetIsExecuting(false);
            }
        }

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
                        var fixedName = pkmn.Name.FixName();
                        if (!IsKommooLine(fixedName) && !IsMrMimeLine(fixedName) && pkmnSpecies.Varieties.Count > 1 && fixedName.Contains('-') && fixedName == pkmnSpecies.Varieties.First(x => x.IsDefault).Pokemon.Name)
                            defaultFormName = GetFormName(fixedName);
                        var record = CreateEntity(ref idCounter, pkmn, pkmnSpecies, evolutionChain, growth, false, defaultFormName);
                        if (record.Name == "Minior (Red-Meteor)")
                            record.Name = "Minior";
                        SetDexNumbers(record, pkmnSpecies);
                        records.Add(record);
                        added++;
                        AddLogEntry($"{added} record(s) added");
                        if (pkmnSpecies.Name == "zarude")
                            AddZarude(ref idCounter, pkmn, pkmnSpecies, evolutionChain, growth);
                        if (pkmnSpecies.Varieties.Count > 1)
                            AddForms(ref idCounter, pkmnSpecies, evolutionChain, growth);
                    }
                    catch (Exception ex)
                    {
                        AddLogEntry($"{ex.Message} {pkmn.Name}");
                    }
                }
                csv.WriteRecords(records);
                AddLogEntry("done");
                SetIsExecuting(false);
            }
            catch (Exception ex)
            {
                AddLogEntry(ex.Message);
                DoCancel();
            }
        }

        private void AddZarude(ref int i, Pokemon pkmn, PokemonSpecies pkmnSpecies, EvolutionChain chain, GrowthRate growth)
        {
            try
            {
                var record = CreateEntity(ref i, pkmn, pkmnSpecies, chain, growth, true, null);
                SetDexNumbers(record, pkmnSpecies);
                record.Name = "Zarude (Dada)";
                records.Add(record);
                AddLogEntry($"zarude form(s) added");
            }
            catch (Exception ex)
            {
                AddLogEntry($"{ex.Message} {pkmn.Name}");
            }
        }

        private PokemonEntity CreateEntity(ref int i, Pokemon pkmn, PokemonSpecies pkmnSpecies, EvolutionChain evolutionChain, GrowthRate growth, bool isForm, string formName = null)
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
                EvolveNum = GetNumberOfEvolutions(evolutionChain.Chain, pkmnSpecies.Name),
                EVYield = GetEvs(pkmn),
                HasForms = pkmnSpecies.Varieties.Count > 1,
                IsForm = isForm,
                IsAlolanForm = pkmn.Name.EndsWith("-alola"),
                IsGalarianForm = pkmn.Name.EndsWith("-galar"),

            };
            var prevPokemon = records.FirstOrDefault(x => x.NationalDex == GetDexEntryNumber(pkmnSpecies, "national") - 1 && entity.IsAlolanForm ? x.IsAlolanForm : entity.IsGalarianForm ? x.IsGalarianForm : false);
            entity.EvolvesFromRegionalForm = prevPokemon != null && ((entity.IsAlolanForm && prevPokemon.IsAlolanForm) || (entity.IsGalarianForm && prevPokemon.IsGalarianForm));
            entity.Evolve = pkmnSpecies.EvolvesFromSpecies == null ? string.Empty : GetEvolveString(evolutionChain, pkmnSpecies.Name, entity.IsAlolanForm, entity.IsGalarianForm, entity.EvolvesFromRegionalForm);
            entity.PrevEvolution = string.Join(',', GetPreviousEvolutions(evolutionChain.Chain, pkmnSpecies.Name, entity, false));
            entity.NextEvolution = string.Join(',', GetNextEvolutions(evolutionChain.Chain, pkmnSpecies.Name, entity, false));
            return entity;
        }

        #region Static helper methods

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
            if (!name.Contains('-'))
                return name;

            var split = IsKommooLine(name) || IsMrMimeLine(name) ? StringExtensions.SplitBy(name, '-', 2).ToArray() : name.Split('-', 2);
            var s1 = split[0].FirstCharToUpper();
            var s2 = split[1].FirstCharToUpper().Insert(split[1].Length, ")").Insert(0, " (");
            return string.Concat(s1, s2);
        }

        private static int GetDexEntryNumber(PokemonSpecies species, string dexName) => species.PokedexNumbers.First(x => x.Pokedex.Name == dexName).EntryNumber;

        #endregion

        #region Helper methods

        private void AddForms(ref int i, PokemonSpecies species, EvolutionChain chain, GrowthRate growth)
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
                var formName = GetFormName(pkmn.Name.FixName());
                try
                {
                    var record = CreateEntity(ref i, pkmn, pkmnSpecies, chain, growth, true, formName);
                    SetDexNumbers(record, pkmnSpecies);
                    if (record.Name == "zarude")
                        record.Name = "Zarude (Dada)";
                    if (record.Name.Contains("Amped-Gmax"))
                    {
                        record.Name = "Toxtricity (Gmax)";
                        record.Ability2 = "Plus/Minus";
                    }

                    records.Add(record);
                    if ((record.IsAlolanForm || record.IsGalarianForm) && !string.IsNullOrEmpty(record.Evolve) && !record.EvolvesFromRegionalForm)
                        AddFormsToEvolutionLine(record);
                    added++;
                    AddLogEntry($"{added} form(s) added");
                }
                catch (Exception ex)
                {
                    AddLogEntry($"{ex.Message} {pkmn.Name}");
                }
            }
        }

        private void AddFormsToEvolutionLine(PokemonEntity form)
        {
            var pkmn = records.First(x => x.NationalDex == form.NationalDex - 1);
            var evs = pkmn.NextEvolution.Split(',');
            evs = evs.Append(form.Name).ToArray();
            pkmn.NextEvolution = string.Join(',', evs);
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
                isCancelled = false;
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
