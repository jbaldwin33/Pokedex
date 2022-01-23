using Microsoft.EntityFrameworkCore;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Entities;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.Services
{
    public class PokedexProvider : IPokedexProvider
    {
        private static readonly Lazy<PokedexProvider> lazy = new Lazy<PokedexProvider>(() => new PokedexProvider());
        public static PokedexProvider Instance => lazy.Value;
        private PokedexProvider() { }

        public async Task<List<Pokemon>> GetAllPokemon()
        {
            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            return await context.PokedexEntries.Select(pkmn => CreatePokemon(pkmn)).ToListAsync();
        }

        private static Pokemon CreatePokemon(PokemonEntity pkmn) => new()
        {
            Id = pkmn.Id,
            Name = pkmn.Name,
            NationalDex = pkmn.NationalDex,
            Ability1 = pkmn.Ability1,
            Ability2 = pkmn.Ability2,
            CanEvolveTo = !string.IsNullOrEmpty(pkmn.EvolveMethodString) && !pkmn.EvolveMethodString.Equals("N"),
            EggGroup1 = Enum.TryParse(typeof(EggGroupEnum), pkmn.EggGroup1, out var egg1) ? (EggGroupEnum)egg1 : null,
            EggGroup2 = Enum.TryParse(typeof(EggGroupEnum), pkmn.EggGroup2, out var egg2) ? (EggGroupEnum)egg2 : null,
            EvolveMethodString = pkmn.EvolveMethodString,
            EvolutionOrderNum = pkmn.EvolutionOrderNum,
            HiddenAbility = pkmn.HiddenAbility,
            NumberOfEvolutions = pkmn.NumberOfEvolutions,
            HasMultipleEvolutions = pkmn.NumberOfEvolutions > 0,
            Icon = pkmn.Icon,
            Type1 = (TypeEnum)Enum.Parse(typeof(TypeEnum), pkmn.Type1),
            Type2 = Enum.TryParse(typeof(TypeEnum), pkmn.Type2, out var result) ? (TypeEnum)result : null,
            EVYields = GetEVYield(pkmn),
            BaseStats = GetBaseStats(pkmn)
        };

        private static List<BaseStat> GetBaseStats(PokemonEntity pkmn) => new()
        {
            new BaseStat(StatEnum.HP, pkmn.HP),
            new BaseStat(StatEnum.Atk, pkmn.Atk),
            new BaseStat(StatEnum.Def, pkmn.Def),
            new BaseStat(StatEnum.SpA, pkmn.SpA),
            new BaseStat(StatEnum.SpD, pkmn.SpD),
            new BaseStat(StatEnum.Spe, pkmn.Spe),
            new BaseStat(StatEnum.Total, pkmn.Total)
        };

        private static List<EVYield> GetEVYield(PokemonEntity pkmn)
        {
            var evDict = new List<EVYield>()
            {
                new EVYield{Stat = StatEnum.HP, Value = 0},
                new EVYield{Stat = StatEnum.Atk, Value = 0},
                new EVYield{Stat = StatEnum.Def, Value = 0},
                new EVYield{Stat = StatEnum.SpA, Value = 0},
                new EVYield{Stat = StatEnum.SpD, Value = 0},
                new EVYield{Stat = StatEnum.Spe, Value = 0},
            };

            var evs = pkmn.EVYield.Split('/');
            foreach (var ev in evs)
            {
                var split = ev.Split(' ');
                evDict.First(e => e.Stat == (StatEnum)Enum.Parse(typeof(StatEnum), split[1])).Value = int.Parse(split[0]);
            }

            return evDict;
        }
    }
}