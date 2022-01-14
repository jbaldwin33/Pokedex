using MVVMFramework.ViewModels;
using MVVMFramework.ViewNavigator;
using Pokedex.PkdxDatabase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.ViewModels
{
    public class EvolveViewModel : ViewModel
    {
        private MainViewModel mainViewModel;
        private ObservableCollection<PokedexClass> evolutionLine;
        private ObservableCollection<PokedexClass> multipleEvolutions;

        public ObservableCollection<PokedexClass> EvolutionLine
        {
            get => evolutionLine;
            set => SetProperty(ref evolutionLine, value);
        }

        public ObservableCollection<PokedexClass> MultipleEvolutions
        {
            get => multipleEvolutions;
            set => SetProperty(ref multipleEvolutions, value);
        }


        public EvolveViewModel()
        {
            EvolutionLine = new ObservableCollection<PokedexClass>();
            MultipleEvolutions = new ObservableCollection<PokedexClass>();
        }

        public override void OnLoaded()
        {
            mainViewModel = Navigator.Instance.MainViewModel as MainViewModel;
            mainViewModel.PokemonChangedAction += OnPokemonChanged;
            if (mainViewModel.SelectedPokemon != null)
                OnPokemonChanged(mainViewModel.SelectedPokemon);
            base.OnLoaded();
        }

        private void OnPokemonChanged(PokedexClass pkmn)
        {
            if (!EvolutionLine.Contains(pkmn))
            {
                EvolutionLine.Clear();
                MultipleEvolutions.Clear();
                mainViewModel.GetEvolutionLine(pkmn).ForEach(EvolutionLine.Add);
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
    }
}
