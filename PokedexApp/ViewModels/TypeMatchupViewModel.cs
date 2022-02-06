using MVVMFramework.ViewModels;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokedex.PokedexApp.ViewModels
{
    public class TypeMatchupViewModel : TabViewModel
    {
        #region Props and fields

        private readonly List<DualTypeClass> typeCombos;
        private ObservableCollection<TypeMult> weaknesses;
        private ObservableCollection<TypeMult> resistances;
        private ObservableCollection<TypeMult> immunities;
        private ObservableCollection<TypeMult> normalDamage;
        private int weakRows;
        private int resRows;
        private int immRows;
        private int normRows;

        public ObservableCollection<TypeMult> Weaknesses
        {
            get => weaknesses;
            set => SetProperty(ref weaknesses, value);
        }

        public ObservableCollection<TypeMult> Resistances
        {
            get => resistances;
            set => SetProperty(ref resistances, value);
        }

        public ObservableCollection<TypeMult> Immunities
        {
            get => immunities;
            set => SetProperty(ref immunities, value);
        }

        public ObservableCollection<TypeMult> NormalDamage
        {
            get => normalDamage;
            set => SetProperty(ref normalDamage, value);
        }

        public int WeakRows
        {
            get => weakRows;
            set => SetProperty(ref weakRows, value);
        }

        public int ResRows
        {
            get => resRows;
            set => SetProperty(ref resRows, value);
        }

        public int ImmRows
        {
            get => immRows;
            set => SetProperty(ref immRows, value);
        }

        public int NormRows
        {
            get => normRows;
            set => SetProperty(ref normRows, value);
        }

        #endregion

        public TypeMatchupViewModel()
        {
            typeCombos = TypeMasterClass.Instance.DualTypeCombos;
        }

        protected override void OnPokemonChanged(Pokemon pkmn)
        {
            if (pkmn.Type2 == null)
            {
                Weaknesses = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == pkmn.Type1).Weaknesses);
                Resistances = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == pkmn.Type1).Resistances);
                Immunities = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == pkmn.Type1).Immunities);
                NormalDamage = new ObservableCollection<TypeMult>(TypeMasterClass.Instance.TypeClasses.First(x => x.ThisType == pkmn.Type1).NormalDamage);
            }
            else
            {
                Weaknesses = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == pkmn.Type1 && x.Type2.ThisType == pkmn.Type2).Weaknesses);
                Resistances = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == pkmn.Type1 && x.Type2.ThisType == pkmn.Type2).Resistances);
                Immunities = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == pkmn.Type1 && x.Type2.ThisType == pkmn.Type2).Immunities);
                NormalDamage = new ObservableCollection<TypeMult>(typeCombos.First(x => x.Type1.ThisType == pkmn.Type1 && x.Type2.ThisType == pkmn.Type2).NormalDamage);
            }
            WeakRows = SetRows(Weaknesses);
            ResRows = SetRows(Resistances);
            ImmRows = SetRows(Immunities);
            NormRows = SetRows(NormalDamage);

            static int SetRows(ObservableCollection<TypeMult> collection) => collection.Count > 12 ? 3 : collection.Count > 6 && collection.Count <= 12 ? 2 : 1;
        }
    }
}
