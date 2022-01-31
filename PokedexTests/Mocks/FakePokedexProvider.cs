using Moq;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokedexTests.Mocks
{
    public class FakePokedexProvider : IPokedexProvider
    {
        public async Task<List<Pokemon>> GetAllPokemon()
        {
            return await CreateList();
        }

        private async Task<List<Pokemon>> CreateList()
        {
            return new List<Pokemon>
            {
                new Pokemon{Id = 0, Name = "Bulbasaur", CanEvolveTo = false, EvolveMethodString = "", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Ivysaur", "Venusaur"}, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Ivysaur", CanEvolveTo = true, EvolveMethodString = "Lv. 16", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Venusaur"}, PrevEvolution = new string[] { "Venusaur"} },
                new Pokemon{Id = 0, Name = "Venusaur", CanEvolveTo = true, EvolveMethodString = "Lv. 32", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Bulbasaur", "Ivysaur"} },
                new Pokemon{Id = 0, Name = "Mew", CanEvolveTo = false, EvolveMethodString = "", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Rattata", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Raticate" }, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Raticate", CanEvolveTo = true, EvolveMethodString = "Lv. 20", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Rattata"} },
                new Pokemon{Id = 0, Name = "Rattata (Alola)", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Raticate (Alola)"}, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Raticate (Alola)", CanEvolveTo = true, EvolveMethodString = "Lv. 20 (Alolan form)", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Rattata (Alola)"} },
                new Pokemon{Id = 0, Name = "Pikachu", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Raichu"}, PrevEvolution = new string[] { "Pichu"} },
                new Pokemon{Id = 0, Name = "Raichu", CanEvolveTo = true, EvolveMethodString = "Thunderstone", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Pikachu"} },
                new Pokemon{Id = 0, Name = "Raichu (Alola)", CanEvolveTo = true, EvolveMethodString = "Thunderstone (Alolan form)", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Pikachu"} },
                new Pokemon{Id = 0, Name = "Meowth", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Persian"}, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Meowth (Alola)", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Persian (Alola)" }, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Meowth (Galar)", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Perrserker" }, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Persian", CanEvolveTo = true, EvolveMethodString = "Lv. 28", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Meowth" } },
                new Pokemon{Id = 0, Name = "Persian (Alola)", CanEvolveTo = true, EvolveMethodString = "Happiness", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Meowth (Alola)" } },
                new Pokemon{Id = 0, Name = "Perrserker", CanEvolveTo = true, EvolveMethodString = "Lv. 28", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Meowth (Galar)" } },
                new Pokemon{Id = 0, Name = "Yamask", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = false, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Cofagricus" }, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Yamask (Galar)", CanEvolveTo = false, EvolveMethodString = "", HasForms = true, IsForm = true, HasMultipleEvolutions = false, NextEvolution = new string[]{ "Runerigus" }, PrevEvolution = Array.Empty<string>() },
                new Pokemon{Id = 0, Name = "Cofagricus", CanEvolveTo = true, EvolveMethodString = "Lv. 34", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Yamask" } },
                new Pokemon{Id = 0, Name = "Runerigus", CanEvolveTo = true, EvolveMethodString = "Dusty Bowl 49 damage", HasForms = false, IsForm = false, HasMultipleEvolutions = false, NextEvolution = Array.Empty<string>(), PrevEvolution = new string[]{ "Yamask (Galar)" } },
            };
        }
    }
}
