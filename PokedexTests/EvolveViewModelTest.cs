using Moq;
using MVVMFramework.ViewNavigator;
using Pokedex.PokedexApp.Services;
using Pokedex.PokedexApp.ViewModels;
using PokedexTests.Mocks;
using System.Linq;
using Xunit;

namespace PokedexTests
{
    public class EvolveViewModelTest
    {
        [Theory]
        [InlineData("Bulbasaur", 3)]
        [InlineData("Mew", 1)]
        public void Verify_Evolution_Line_Updates(string name, int count)
        {
            var setup = new TestSetup();
            var mainViewModel = setup.MainViewModel;
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.Pokedexes[0].SelectedPokemon = mainViewModel.PokemonList.First(x => x.Name == name);

            Assert.Equal(count, sut.EvolutionLine.Count);
        }
        [Theory]
        [InlineData("Rattata", true)]
        public void Verify_Form_Change_Updates_Evolution_Line(string name, bool prevIsForm)
        {
            var setup = new TestSetup();
            var mainViewModel = setup.MainViewModel;
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.Pokedexes[0].SelectedPokemon = mainViewModel.PokemonList.First(x => x.Name == name);
            mainViewModel.FormCollection[1].FormCommand.Execute(mainViewModel.FormCollection[1].PkmnForm);

            Assert.Equal(sut.EvolutionLine[1].IsForm, prevIsForm);
        }

        [Theory]
        [InlineData("Raticate", null, 0, false)]
        [InlineData("Raticate", "Raticate (Alola)", 0, true)]
        [InlineData("Raichu", null, 1, false)]
        [InlineData("Raichu", "Raichu (Alola)", 1, true)]
        [InlineData("Persian", null, 0, false)]
        [InlineData("Persian", "Persian (Alola)", 0, true)]
        public void Verify_Evolution_Line_Correct_Regional_Form(string name, string nameWithForm, int index, bool prevIsForm)
        {
            var setup = new TestSetup();
            var mainViewModel = setup.MainViewModel;
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.Pokedexes[0].SelectedPokemon = mainViewModel.PokemonList.First(x => x.Name == name);
            if (!string.IsNullOrEmpty(nameWithForm))
                mainViewModel.FormCollection[1].FormCommand.Execute(mainViewModel.FormCollection[1].PkmnForm);

            Assert.Equal(sut.EvolutionLine[index].IsForm, prevIsForm);
        }

        [Theory]
        [InlineData("Runerigus", 2, true)]
        [InlineData("Perrserker", 2, true)]
        public void Verify_Regional_Pokemons_Previous_Form(string name, int evolutionLineCount, bool prevIsForm)
        {
            var setup = new TestSetup();
            var mainViewModel = setup.MainViewModel;
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.Pokedexes[0].SelectedPokemon = mainViewModel.PokemonList.First(x => x.Name == name);

            Assert.Equal(prevIsForm, sut.EvolutionLine[0].IsForm);
            Assert.Equal(evolutionLineCount, sut.EvolutionLine.Count);
        }

        [Theory]
        [InlineData("Meowth", null, 2, false)]
        [InlineData("Meowth", "Meowth (Alola)", 2, true)]
        [InlineData("Meowth", "Meowth (Galar)", 2, false)]
        [InlineData("Yamask", null, 2, false)]
        [InlineData("Yamask", "Yamask (Galar)", 2, false)]
        public void Verify_Regional_Pokemons_Next_Form(string name, string nameWithForm, int evolutionLineCount, bool nextIsForm)
        {
            var setup = new TestSetup();
            var mainViewModel = setup.MainViewModel;
            var sut = new EvolveViewModel();
            sut.OnLoaded();
            mainViewModel.Pokedexes[0].SelectedPokemon = mainViewModel.PokemonList.First(x => x.Name == name);
            if (!string.IsNullOrEmpty(nameWithForm))
            {
                var form = mainViewModel.FormCollection.First(x => x.FormName == nameWithForm);
                form.FormCommand.Execute(form.PkmnForm);
            }

            Assert.Equal(nextIsForm, sut.EvolutionLine[sut.EvolutionLine.Count - 1].IsForm);
            Assert.Equal(evolutionLineCount, sut.EvolutionLine.Count);
        }
    }
}