using Pokedex.PokedexCSVCreator.ViewModels;
using Pokedex.PokedexCSVCreator.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Pokedex.PokedexCSVCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var viewModel = new CSVViewModel();
            var window = new CSVWindow
            {
                DataContext = viewModel,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}
