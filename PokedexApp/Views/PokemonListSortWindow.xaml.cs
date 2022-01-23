using MVVMFramework.Views;
using Pokedex.PokedexApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.Views
{
    /// <summary>
    /// Interaction logic for EvYieldListWindow.xaml
    /// </summary>
    public partial class PokemonListSortWindow : ViewBaseWindow
    {
        private PokemonListSortViewModel viewModel;
        private GridViewColumnHeader lastHeaderClicked;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        public PokemonListSortWindow() : base(null)
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            viewModel = DataContext as PokemonListSortViewModel;
            viewModel.SortOptionsChanged = () =>
            {
                switch (viewModel.CurrentSortOptions.CurrentSortType)
                {
                    case SortType.EVYield:
                        var values = Enum.GetValues(typeof(StatEnum)).Cast<StatEnum>().Where(x => x != StatEnum.Total);
                        CreateColumns(values);
                        break;
                    case SortType.BaseStat:
                        values = Enum.GetValues(typeof(StatEnum)).Cast<StatEnum>();
                        CreateColumns(values);
                        break;
                    case SortType.PokemonType:
                        CreateColumns<object>(null);
                        break;
                    case SortType.EggGroup:
                        CreateColumns<object>(null);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(viewModel.CurrentSortOptions.CurrentSortType));
                }
            };
            viewModel.ChangeSortCommandExecute(SortType.EVYield);
        }

        private void CreateColumns<T>(IEnumerable<T> values)
        {
            var gridView = new GridView();
            var nameColumn = new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding("Name"),
                Width = 150
            };
            gridView.Columns.Add(nameColumn);
            if (values != null)
            {
                var i = 0;
                foreach (var value in values)
                {
                    var column = new GridViewColumn
                    {
                        Header = value.ToString(),
                        DisplayMemberBinding = new Binding($"{viewModel.CurrentSortOptions.CurrentSortType}s[{i}].Value"),
                        Width = 50
                    };
                    gridView.Columns.Add(column);
                    i++;
                }
            }
            sortedListView.View = gridView;
        }

        private void sortedListView_Click(object sender, RoutedEventArgs e)
        {
            ListSortDirection direction;

            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    direction = headerClicked != lastHeaderClicked
                        ? ListSortDirection.Descending
                        : lastDirection == ListSortDirection.Descending
                            ? ListSortDirection.Ascending
                            : ListSortDirection.Descending;

                    var header = (headerClicked.Column.DisplayMemberBinding as Binding).Path.Path;
                    Sort(header, direction);

                    lastHeaderClicked = headerClicked;
                    lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            var dataView = CollectionViewSource.GetDefaultView(sortedListView.ItemsSource);
            var sd = new SortDescription(sortBy, direction);

            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}