using MVVMFramework.ViewNavigator;
using Pokedex.PokedexApp.ViewModels;
using System.Linq;
using Xunit;

namespace PokedexTests
{
    public class EvolveViewModelTest
    {
        [Fact]
        public void Verify_Evolution_Line_Updates()
        {
            var setup = new TestSetup();
            var mainViewModel = new MainViewModel(Navigator.Instance);
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.FindCommand.Execute(setup.PokemonList.First(x => x.Name == "Bulbasaur"));
            mainViewModel.FindCommand.Execute(setup.PokemonList.First(x => x.Name == "Mew"));

            Assert.Empty(sut.EvolutionLine);
        }
    }
}