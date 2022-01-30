using Pokedex.PkdxDatabase.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pokedex.PokedexApp.ViewModels
{
    public class EvolveViewModel : TabViewModel
    {
        #region Props and fields

        private ObservableCollection<Pokemon> evolutionLine;
        private ObservableCollection<Pokemon> multipleEvolutions;

        public ObservableCollection<Pokemon> EvolutionLine
        {
            get => evolutionLine;
            set => SetProperty(ref evolutionLine, value);
        }

        public ObservableCollection<Pokemon> MultipleEvolutions
        {
            get => multipleEvolutions;
            set => SetProperty(ref multipleEvolutions, value);
        }

        #endregion

        public EvolveViewModel()
        {
            EvolutionLine = new ObservableCollection<Pokemon>();
            MultipleEvolutions = new ObservableCollection<Pokemon>();
        }

        protected override void OnPokemonChanged(Pokemon pkmn)
        {
            if (!EvolutionLine.Contains(pkmn))
            {
                EvolutionLine.Clear();
                MultipleEvolutions.Clear();
                GetEvolutionLine(pkmn).ForEach(EvolutionLine.Add);
                for (var i = 0; i < EvolutionLine.Count; i++)
                {
                    if (EvolutionLine[i].HasMultipleEvolutions)
                    {
                        //EvolutionLine needs the first of the multiple evolutions so choosing a template works (e.g. Vileplume is in EvolutionLine and MultipleEvolutions)
                        var toRemove = EvolutionLine.TakeLast(EvolutionLine.Count - 2 - i).ToList();
                        toRemove.ForEach(x => EvolutionLine.Remove(x));
                        MultipleEvolutions.Add(EvolutionLine.Last());
                        toRemove.ForEach(x => MultipleEvolutions.Add(x));
                    }
                }
            }
        }

        private List<Pokemon> GetEvolutionLine(Pokemon pkmn)
        {
            var evolutionList = new List<Pokemon>();
            foreach (var p in pkmn.PrevEvolution)
                evolutionList.Add(mainViewModel.PokemonList.First(x => x.Name.ToLower().Contains(p) && !x.IsForm));
            evolutionList.Add(pkmn);
            foreach (var p in pkmn.NextEvolution)
                evolutionList.Add(mainViewModel.PokemonList.First(x => x.Name.ToLower().Contains(p) && !x.IsForm));
            return evolutionList;
        }
    }
}
