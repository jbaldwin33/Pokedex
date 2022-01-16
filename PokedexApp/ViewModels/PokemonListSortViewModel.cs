using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.SortWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class PokemonListSortViewModel : ViewModel
    {
        private ObservableCollection<PrimarySort> primarySorts;
        private SortOptions currentSortOptions;
        private ObservableCollection<Pokemon> currentSortList;

        public ObservableCollection<PrimarySort> PrimarySorts
        {
            get => primarySorts;
            set => SetProperty(ref primarySorts, value);
        }

        public SortOptions CurrentSortOptions
        {
            get => currentSortOptions;
            set
            {
                SetProperty(ref currentSortOptions, value);
                SortOptionsChanged?.Invoke();
            }
        }

        public ObservableCollection<Pokemon> CurrentSortList
        {
            get => currentSortList;
            set => SetProperty(ref currentSortList, value);
        }

        public Action SortOptionsChanged;

        public PokemonListSortViewModel(ObservableCollection<Pokemon> list)
        {
            CurrentSortList = list;
            PrimarySorts = new ObservableCollection<PrimarySort>
            {
                new PrimarySort( SortType.EVYield, SortByEVCommandExecute, SortByEVCommandCanExecute),
                new PrimarySort( SortType.BaseStat, SortByStatCommandExecute, SortByStatCommandCanExecute)
            };
        }

        public void OnListSorted() => CurrentSortList = new ObservableCollection<Pokemon>(CurrentSortOptions.PokemonList);

        private bool SortByEVCommandCanExecute() => CurrentSortOptions.Sort != SortType.EVYield;
        private bool SortByStatCommandCanExecute() => CurrentSortOptions.Sort != SortType.BaseStat;

        public void SortByEVCommandExecute()
        {
            CurrentSortOptions = new SortOptions{Sort = SortType.EVYield, PokemonList = CurrentSortList, ListSorted = OnListSorted };
            CurrentSortOptions.SortListCommandExecute("HP");
        }
        public void SortByStatCommandExecute()
        {
            CurrentSortOptions = new SortOptions{ Sort = SortType.BaseStat, PokemonList = CurrentSortList, ListSorted = OnListSorted};
            CurrentSortOptions.SortListCommandExecute("HP");
        }
    }
}
