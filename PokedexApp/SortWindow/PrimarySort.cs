using MVVMFramework.ViewModels;
using System;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.SortWindow
{
    public class PrimarySort
    {
        public SortType PrimarySortType { get; set; }
        private readonly Action getSort;
        private readonly Func<bool> getSortCanExecute;
        private RelayCommand getSecondarySortCommand;
        public RelayCommand GetSecondarySortCommand => getSecondarySortCommand ??= new RelayCommand(getSort, getSortCanExecute);
        public PrimarySort(SortType type, Action action, Func<bool> actionCanExecute)
        {
            PrimarySortType = type;
            getSort = action;
            getSortCanExecute = actionCanExecute;
        }
    }
}
