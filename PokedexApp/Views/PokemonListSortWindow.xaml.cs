using MVVMFramework.ViewModels;
using MVVMFramework.Views;
using Pokedex.PokedexApp.SortWindow;
using Pokedex.PokedexApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                switch (viewModel.CurrentSortOptions.Sort)
                {
                    case SortType.EVYield:
                        var s1 = Enum.GetValues(typeof(StatEnum)).Cast<StatEnum>().Where(x => x != StatEnum.Total);
                        CreateColumns(s1);
                        break;
                    case SortType.BaseStat:
                        var s3 = Enum.GetValues(typeof(StatEnum)).Cast<StatEnum>();
                        CreateColumns(s3);
                        break;
                    default:
                        break;
                }
            };
            viewModel.SortByEVCommandExecute();
        }

        private void CreateColumns<T>(IEnumerable<T> s2)
        {
            var gridView = new GridView();
            var column1 = new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding("Name")
            };
            gridView.Columns.Add(column1);
            var i = 0;
            foreach (var item in s2)
            {
                var column = new GridViewColumn
                {
                    Header = item.ToString(),
                    DisplayMemberBinding = new Binding($"{viewModel.CurrentSortOptions.Sort}s[{i}].Value"),
                    Width = 50
                };
                gridView.Columns.Add(column);
                i++;
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