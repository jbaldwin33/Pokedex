using Pokedex.PokedexApp;
using System.Windows;

namespace Pokedex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainViewModel();
            var window = new MainWindow
            {
                DataContext = viewModel,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.Show();
        }
    }
}
