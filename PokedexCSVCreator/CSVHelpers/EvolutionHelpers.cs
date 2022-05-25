using PokeApiNet;
using Pokedex.PokedexCSVCreator.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pokedex.PokedexCSVCreator.CSVHelpers
{
    public static class EvolutionHelpers
    {
        public static int GetDexEntryNumber(PokemonSpecies species, string dexName) => species.PokedexNumbers.First(x => x.Pokedex.Name == dexName).EntryNumber;

        public static ChainLink TraverseAndGetChain(ChainLink chain, string name) =>
            chain.Species.Name == name ? chain : TraverseAndGetChain(GetCorrectEvolutionPath(chain.EvolvesTo, name), name);

        public static ChainLink GetCorrectEvolutionPath(List<ChainLink> chain, string name)
        {
            ChainLink newChain = null;
            foreach (var item in chain)
            {
                newChain = item.Species.Name == name ? item : GetCorrectEvolutionPath(item.EvolvesTo, name);
                if (newChain != null)
                    break;
            }
            return newChain;
        }

        public static string GetEvolveString(EvolutionChain evolution, string name, bool isAlolanForm, bool isGalarianForm, bool evolvesFromForm)
        {
            var details = TraverseAndGetChain(evolution.Chain, name);
            return details.Species.Name == "melmetal" ? "400 candy in PKMN Go" : ParseAndGetEvolveString(details.EvolutionDetails[0], isAlolanForm, isGalarianForm, evolvesFromForm);
        }

        public static string GetPreviousEvolutions(PokeApiClient pokeClient, PokemonSpecies species, PokemonEntity pkmn)
        {
            var sb = new StringBuilder();
            if (species.EvolvesFromSpecies == null)
                return string.Empty;
            var prevSpecies = pokeClient.GetResourceAsync<PokemonSpecies>(species.EvolvesFromSpecies.Name).Result;
            var prevPokemon = pokeClient.GetResourceAsync<Pokemon>(prevSpecies.Id).Result;
            string name = prevPokemon.Name;
            if (pkmn.Name.Contains("Gourgeist") || pkmn.Name.Contains("Lycanroc"))
            {
                var variety = 0;
                if (pkmn.Name.Contains("Average"))
                    variety = 0;
                else if (pkmn.Name.Contains("Small") || pkmn.Name.Contains("Dusk"))
                    variety = 1;
                else if (pkmn.Name.Contains("Large"))
                    variety = 2;
                else if (pkmn.Name.Contains("Super"))
                    variety = 3;

                name = prevSpecies.Varieties[variety].Pokemon.Name.GetFormName();
            }
            
            sb.Append(name.FirstCharToUpper());
            sb.Append(pkmn.EvolvesFromRegionalForm && isAlolanForm(prevSpecies) && !prevPokemon.Name.Contains("-alola") ? " (Alola)" : pkmn.EvolvesFromRegionalForm && isGalarianForm(prevSpecies) && !prevPokemon.Name.Contains("-galar") ? " (Galar)" : string.Empty);
            return sb.ToString();

            bool isAlolanForm(PokemonSpecies pkmnSpecies) => pkmnSpecies.Varieties.Any(x => x.Pokemon.Name.EndsWith("-alola")) && pkmn.AlolaDex != -1;
            bool isGalarianForm(PokemonSpecies pkmnSpecies) => pkmnSpecies.Varieties.Any(x => x.Pokemon.Name.EndsWith("-galar")) && (pkmn.GalarDex != -1 || pkmn.IsleOfArmorDex != -1 || pkmn.CrownTundraDex != -1);
        }

        public static bool EvolutionChainContainsName(ChainLink item, string name)
        {
            if (item.Species.Name == name)
                return true;
            foreach (var sub in item.EvolvesTo)
                return EvolutionChainContainsName(sub, name);
            return false;
        }

        public static string ParseAndGetEvolveString(EvolutionDetail details, bool isAlolanForm, bool isGalarianForm, bool evolvesFromForm)
        {
            var method = string.Empty;
            if (details.MinLevel != null)
                method = $"Lv. {details.MinLevel}";
            if (details.Gender != null)
                method = $"Lv. {details.MinLevel} {(details.Gender == 1 ? "M" : "F")}";
            if (details.HeldItem != null)
                method = $"{(details.MinLevel != null ? $"Lv. {details.MinLevel}" : "Trade")} h/ {details.HeldItem.Name.GetFriendlyName()}";
            if (details.Item != null)
                method = details.Item.Name.GetFriendlyName();
            if (details.KnownMove != null)
                method = $"Level w/ {details.KnownMove.Name.GetFriendlyName()}";
            if (details.KnownMoveType != null)
                method = $"{(details.MinHappiness != null ? "Friendship" : "Level")} w/ {details.KnownMoveType.Name.GetFriendlyName()} move";
            if (details.Location != null)
                method = $"Level a/ {details.Location.Name.GetFriendlyName()}";
            if (details.MinAffection != null)
                method = $"Friendship w/ {details.KnownMoveType.Name.GetFriendlyName()} move"; //should only be sylveon
            if (details.MinBeauty != null)
                method = "";
            if (details.MinHappiness != null)
                method = "Friendship";
            if (details.NeedsOverworldRain)
                method = $"Lv. {details.MinLevel} in rain";
            if (details.PartySpecies != null)
                method = $"Lv. {details.MinLevel} w/ {details.PartySpecies.Name.GetFriendlyName()} on team";
            if (details.PartyType != null)
                method = $"Lv. {details.MinLevel} w/ {details.PartyType.Name.GetFriendlyName()} on team";
            if (details.RelativePhysicalStats != null)
                method = "";
            if (!string.IsNullOrEmpty(details.TimeOfDay))
                method = $"{details.TimeOfDay.GetFriendlyName()} {(details.MinHappiness != null ? "Friendship" : $"Lv. {details.MinLevel}")}";
            if (details.TradeSpecies != null)
                method = $"Trade w/ {details.TradeSpecies.Name.GetFriendlyName()}";
            if (details.TurnUpsideDown)
                method = $"Lv. {details.MinLevel} upside down";
            if (details.Trigger.Name == "trade")
                method = "Trade";
            return $"{method}{(evolvesFromForm ? string.Empty : isAlolanForm ? " (Alolan form)" : isGalarianForm ? " (Galarian form)" : string.Empty)}";
        }
    }
}