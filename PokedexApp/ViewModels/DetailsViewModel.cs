using Pokedex.PkdxDatabase.Models;
using static Pokedex.PokedexLib.Enums;

namespace Pokedex.PokedexApp.ViewModels
{
    public class DetailsViewModel : TabViewModel
    {
        #region Fields and props

        private int number;
        private TypeEnum? type1;
        private TypeEnum? type2;
        private string ability1;
        private string ability2;
        private string hiddenAbility;
        private EggGroupEnum? eggGroup1;
        private EggGroupEnum? eggGroup2;

        public int Number
        {
            get => number;
            set => SetProperty(ref number, value);
        }

        public TypeEnum? Type1
        {
            get => type1;
            set => SetProperty(ref type1, value);
        }

        public TypeEnum? Type2
        {
            get => type2;
            set => SetProperty(ref type2, value);
        }

        public string Ability1
        {
            get => ability1;
            set => SetProperty(ref ability1, value);
        }

        public string Ability2
        {
            get => ability2;
            set => SetProperty(ref ability2, value);
        }

        public string HiddenAbility
        {
            get => hiddenAbility;
            set => SetProperty(ref hiddenAbility, value);
        }

        public EggGroupEnum? EggGroup1
        {
            get => eggGroup1;
            set => SetProperty(ref eggGroup1, value);
        }

        public EggGroupEnum? EggGroup2
        {
            get => eggGroup2;
            set => SetProperty(ref eggGroup2, value);
        }

        #endregion

        protected override void OnPokemonChanged(Pokemon pkmn)
        {
            Type1 = pkmn.Type1;
            Type2 = pkmn.Type2;
            Ability1 = pkmn.Ability1;
            Ability2 = pkmn.Ability2;
            HiddenAbility = pkmn.HiddenAbility;
            EggGroup1 = pkmn.EggGroup1;
            EggGroup2 = pkmn.EggGroup2;
        }
    }
}
