using MVVMFramework.ViewNavigator;
using Pokedex.PokedexApp.ViewModels;
using PokedexTests.Mocks;

namespace PokedexTests
{
    public class TestSetup
    {
        public MainViewModel MainViewModel;
        public TestSetup()
        {
            MainViewModel = new MainViewModel(Navigator.Instance, new FakePokedexProvider());
            Navigator.Instance.MainViewModel = MainViewModel;
        }

        
    }
}
