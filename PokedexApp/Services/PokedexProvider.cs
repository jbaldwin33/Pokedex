using Microsoft.EntityFrameworkCore;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Entities;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.Services
{
    public class PokedexProvider : IPokedexProvider
    {
        private static readonly Lazy<PokedexProvider> lazy = new Lazy<PokedexProvider>(() => new PokedexProvider());
        public static PokedexProvider Instance => lazy.Value;
        private PokedexProvider() { }

        public async Task<List<PokedexClass>> GetAllPokemon()
        {
            using var context = PokedexDbContextFactory.Instance.CreateDbContext();
            return await context.PokedexEntries.Select(pkmn => new PokedexClass
            {
                Id = pkmn.Id,
                Name = pkmn.Name,
                Num = pkmn.Num,
                Ability1 = pkmn.Ability1,
                Ability2 = pkmn.Ability2,
                CanEvolveTo = !string.IsNullOrEmpty(pkmn.EvolveMethodString) && !pkmn.EvolveMethodString.Equals("N"),
                EggGroup1 = pkmn.EggGroup1,
                EggGroup2 = pkmn.EggGroup2,
                EvolveMethodString = pkmn.EvolveMethodString,
                EvolutionOrderNum = pkmn.EvolutionOrderNum,
                HiddenAbility = pkmn.HiddenAbility,
                NumberOfEvolutions = pkmn.NumberOfEvolutions,
                HasMultipleEvolutions = pkmn.NumberOfEvolutions > 0,
                HP = pkmn.HP,
                Atk = pkmn.Atk,
                Def = pkmn.Def,
                SpA = pkmn.SpA,
                SpD = pkmn.SpD,
                Spe = pkmn.Spe,
                Total = pkmn.Total,
                Icon = pkmn.Icon,
                Type1 = pkmn.Type1,
                Type2 = pkmn.Type2,
                EVYields = GetEVYield(pkmn)
            }).ToListAsync();
        }

        private static List<EVYield> GetEVYield(PokedexClassEntity pkmn)
        {
            var evDict = new List<EVYield>();

            var evs = pkmn.EVYield.Split('/');
            foreach (var ev in evs)
            {
                var split = ev.Split(' ');
                evDict.Add(new EVYield { Stat = (StatEnum)Enum.Parse(typeof(StatEnum), split[1]), Yield = int.Parse(split[0]) });
            }

            return evDict;
        }
    }
}
