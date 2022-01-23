using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.SortWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class PokemonListSortViewModel : ViewModel
    {
        private readonly IEnumerable<Pokemon> originalList;
        private readonly ObservableCollection<object> typeList;
        private readonly ObservableCollection<object> eggList;
        private SortOptions currentSortOptions;
        private ObservableCollection<Pokemon> currentSortList;
        private object selectedGroup;
        private ObservableCollection<object> secondarySortTypes;
        private bool sortByType;
        private RelayCommand changeSortCommand;

        public SortOptions CurrentSortOptions
        {
            get => currentSortOptions;
            set => SetProperty(ref currentSortOptions, value);
        }

        public ObservableCollection<Pokemon> CurrentSortList
        {
            get => currentSortList;
            set => SetProperty(ref currentSortList, value);
        }

        public ObservableCollection<object> SecondarySortTypes
        {
            get => secondarySortTypes;
            set => SetProperty(ref secondarySortTypes, value);
        }

        public object SelectedGroup
        {
            get => selectedGroup;
            set
            {
                SetProperty(ref selectedGroup, value);
                OnSecondarySortChanged(value);
            }
        }

        public bool SortByType
        {
            get => sortByType;
            set => SetProperty(ref sortByType, value);
        }

        public Action SortOptionsChanged { get; set; }

        public RelayCommand ChangeSortCommand => changeSortCommand ??= new RelayCommand(ChangeSortCommandExecute, ChangeSortCommandCanExecute);

        public PokemonListSortViewModel(ObservableCollection<Pokemon> list)
        {
            originalList = list;
            CurrentSortList = list;
            CurrentSortOptions = new SortOptions { ListSorted = OnListSorted };
            typeList = new ObservableCollection<object>(Enum.GetValues(typeof(TypeEnum)).Cast<object>());
            eggList = new ObservableCollection<object>(Enum.GetValues(typeof(EggGroupEnum)).Cast<object>());
        }

        public void OnListSorted() => CurrentSortList = new ObservableCollection<Pokemon>(CurrentSortOptions.PokemonList);

        public void ChangeSortCommandExecute(object param)
        {
            ResetSort(param);
            SecondarySortTypes = (SortType)param == SortType.PokemonType ? typeList : eggList;
            SelectedGroup = SecondarySortTypes?.FirstOrDefault();
        }

        private bool ChangeSortCommandCanExecute(object type) => CurrentSortOptions.CurrentSortType != (SortType)type;

        private void ResetSort(object sortType)
        {
            CurrentSortOptions.CurrentSortType = (SortType)sortType;
            CurrentSortOptions.PokemonList = originalList;
            CurrentSortOptions.SecondarySort = SecondarySortTypes?.FirstOrDefault();
            SortOptionsChanged?.Invoke();
            SortByType = CurrentSortOptions.CurrentSortType == SortType.PokemonType || CurrentSortOptions.CurrentSortType == SortType.EggGroup;
            CurrentSortOptions.SortList();
        }


        private void OnSecondarySortChanged(object secondarySort)
        {
            if (secondarySort == null)
                return;

            CurrentSortOptions.SecondarySort = secondarySort;
            CurrentSortOptions.SortList();
        }
    }
}
