using MVVMFramework.ViewNavigator;
using Pokedex.PokedexApp.Services;
using Pokedex.PokedexApp.ViewModels;
using PokedexTests.Mocks;

namespace PokedexTests
{
    public class TestSetup
    {
        public MainViewModel MainViewModel;
        public TestSetup()
        {
            MainViewModel = new MainViewModel(Navigator.Instance, PokedexProvider.Instance);
            Navigator.Instance.MainViewModel = MainViewModel;
        }

        
    }
}
