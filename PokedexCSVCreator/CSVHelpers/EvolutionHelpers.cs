using PokeApiNet;
using Pokedex.PokedexCSVCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexCSVCreator.CSVHelpers
{
    public static class EvolutionHelpers
    {
        public static string GetNumberOfEvolutions(ChainLink chain, string name)
        {
            if (chain.Species.Name == name)
                return chain.EvolvesTo.Count > 1 ? chain.EvolvesTo.Count.ToString() : "";
            return GetNumberOfEvolutions(TraverseChain(chain, name), name);
        }

        public static ChainLink TraverseChain(ChainLink chain, string name) =>
            chain.Species.Name == name ? chain : TraverseChain(GetCorrectEvolutionPath(chain.EvolvesTo, name), name);

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
            var details = TraverseChain(evolution.Chain, name);
            return details.Species.Name == "melmetal" ? "400 candy in PKMN Go" : ParseAndGetEvolveString(details.EvolutionDetails[0], isAlolanForm, isGalarianForm, evolvesFromForm);
        }

        public static List<string> GetPreviousEvolutions(ChainLink evolutionChain, string name, PokemonEntity pkmn, bool startAdding)
        {
            var prevs = new List<string>();
            var sb = new StringBuilder();
            if (evolutionChain.Species.Name == name)
                return prevs;
            else
            {
                sb.Append(evolutionChain.Species.Name.FirstCharToUpper());
                sb.Append(pkmn.EvolvesFromRegionalForm ? pkmn.IsAlolanForm ? " (Alola)" : pkmn.IsGalarianForm ? " (Galar)" : string.Empty : string.Empty);
                prevs.Add(sb.ToString());
            }

            if (evolutionChain.EvolvesTo.Count > 1)
            {
                var useChain = GetCorrectEvolutionPathWithPreEvolution(evolutionChain.EvolvesTo, name);
                if (useChain == null || useChain.EvolvesTo.Count > 0 && useChain.EvolvesTo[0].Species.Name != name)
                    return prevs;

                prevs.AddRange(GetPreviousEvolutions(useChain, name, pkmn, startAdding));
            }
            else if (evolutionChain.EvolvesTo.Count == 1)
                prevs.AddRange(GetPreviousEvolutions(evolutionChain.EvolvesTo[0], name, pkmn, startAdding));

            return prevs;
        }

        public static List<string> GetNextEvolutions(ChainLink evolutionChain, string name, PokemonEntity pkmn, bool startAdding)
        {
            if (name.Contains("meowth"))
            {

            }
            var nexts = new List<string>();
            var sb = new StringBuilder();
            if (startAdding)
            {
                sb.Append(evolutionChain.Species.Name.FirstCharToUpper());
                sb.Append(pkmn.IsAlolanForm ? " (Alola)" : pkmn.IsGalarianForm && !name.Contains("meowth") && !name.Contains("yamask") ? " (Galar)" : string.Empty);
                nexts.Add(sb.ToString());
            }
            if (evolutionChain.Species.Name == name)
                startAdding = true;
            if (evolutionChain.EvolvesTo.Count > 1)
            {
                for (var i = 0; i < evolutionChain.EvolvesTo.Count; i++)
                {
                    if (evolutionChain.Species.Name != name &&
                        evolutionChain.EvolvesTo[i].Species.Name != name &&
                        evolutionChain.EvolvesTo[i].EvolvesTo.Count > 0 &&
                        evolutionChain.EvolvesTo[i].EvolvesTo[0].Species.Name != name)
                        continue;

                    nexts.AddRange(GetNextEvolutions(evolutionChain.EvolvesTo[i], name, pkmn, startAdding));
                }
            }
            else if (evolutionChain.EvolvesTo.Count == 1)
                nexts.AddRange(GetNextEvolutions(evolutionChain.EvolvesTo[0], name, pkmn, startAdding));

            return nexts;
        }

        public static ChainLink GetCorrectEvolutionPathWithPreEvolution(List<ChainLink> chain, string name)
        {
            foreach (var item in chain)
                if (TraverseForName(item, name))
                    return item;
            return null;
        }

        public static bool TraverseForName(ChainLink item, string name)
        {
            if (item.Species.Name == name)
                return true;
            foreach (var sub in item.EvolvesTo)
                return TraverseForName(sub, name);
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
            return $"{method}{(evolvesFromForm ? string.Empty : isAlolanForm ? " (Alolan form)" : isGalarianForm ? " (Galarian form)" : string.Empty)}";
        }
    }
}
