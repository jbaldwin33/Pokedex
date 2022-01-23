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
        private SortOptions currentSortOptions;
        private ObservableCollection<Pokemon> currentSortList;
        private ObservableCollection<object> secondarySort;

        public ObservableCollection<object> SecondarySort
        {
            get => secondarySort;
            set => SetProperty(ref secondarySort, value);
        }

        private bool sortByType;
        private RelayCommand changeTypeCommand;
        private RelayCommand changeSortCommand;

        public SortOptions CurrentSortOptions
        {
            get => currentSortOptions;
            set
            {
                SetProperty(ref currentSortOptions, value);
                SortOptionsChanged?.Invoke();
                SortByType = value.Sort == SortType.PokemonType || value.Sort == SortType.EggGroup;
            }
        }

        public ObservableCollection<Pokemon> CurrentSortList
        {
            get => currentSortList;
            set => SetProperty(ref currentSortList, value);
        }

        public bool SortByType
        {
            get => sortByType;
            set => SetProperty(ref sortByType, value);
        }

        public Action SortOptionsChanged { get; set; }

        public RelayCommand ChangeTypeCommand => changeTypeCommand ??= new RelayCommand(ChangeTypeCommandExecute, ChangeTypeCommandCanExecute);
        public RelayCommand ChangeSortCommand => changeSortCommand ??= new RelayCommand(ChangeSortCommandExecute, ChangeSortCommandCanExecute);

        public PokemonListSortViewModel(ObservableCollection<Pokemon> list)
        {
            originalList = list;
            CurrentSortList = list;
        }

        public void OnListSorted() => CurrentSortList = new ObservableCollection<Pokemon>(CurrentSortOptions.PokemonList);

        public void ChangeSortCommandExecute(object param)
        {
            CurrentSortOptions = new SortOptions { Sort = (SortType)param, PokemonList = originalList, ListSorted = OnListSorted, TypeToSort = TypeEnum.Normal };
            CurrentSortOptions.SortList();
            CurrentSortOptions.Sort = (SortType)param;
            if ((SortType)param == SortType.PokemonType)
                SecondarySort = new ObservableCollection<object>(Enum.GetValues(typeof(TypeEnum)).Cast<object>());
            else if ((SortType)param == SortType.EggGroup)
                SecondarySort = new ObservableCollection<object>(Enum.GetValues(typeof(EggGroupEnum)).Cast<object>());
        }

        private bool ChangeSortCommandCanExecute(object sender) => CurrentSortOptions.Sort != (SortType)sender;

        private void ChangeTypeCommandExecute(object param)
        {
            if (param is TypeEnum type)
                CurrentSortOptions.TypeToSort = type;
            else
                CurrentSortOptions.EggGroupToSort = (EggGroupEnum)param;
            CurrentSortOptions.SortList();
        }

        private bool ChangeTypeCommandCanExecute(object param) => param is TypeEnum type
            ? CurrentSortOptions.TypeToSort != type
            : CurrentSortOptions.EggGroupToSort != (EggGroupEnum)param;
    }
}
