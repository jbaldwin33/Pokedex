using System;
using System.Collections.Generic;
using System.Linq;

namespace Pokedex.PokedexLib
{
    public class TypeCombos
    {
        private static readonly Lazy<TypeCombos> lazy = new Lazy<TypeCombos>(() => new TypeCombos());
        public static TypeCombos Instance => lazy.Value;
        private TypeCombos() { }

        public TypeClass Type1;
        public TypeClass Type2;
        public List<TypeMult> ResistancesChecked;
        public List<TypeMult> WeaknessesChecked;
        public DualTypeClass dualTypeClass;

        public List<DualTypeClass> GetDualTypeCombos()
        {
            var masterClass = TypeMasterClass.Instance;
            var dualTypeClasses = new List<DualTypeClass>();

            for (var i = 0; i < masterClass.TypeClasses.Length - 1; i++)
            {
                for (var j = 1; j < masterClass.TypeClasses.Length; j++)
                {
                    Type1 = masterClass.TypeClasses[i];
                    Type2 = masterClass.TypeClasses[j];
                    if (Type1.ThisType == Type2.ThisType)
                        continue;

                    ResistancesChecked = new List<TypeMult>();
                    WeaknessesChecked = new List<TypeMult>();
                    dualTypeClass = new DualTypeClass(Type1, Type2);
                    CheckResistances();
                    CheckWeaknesses();
                    SetNormalDamage();

                    dualTypeClasses.Add(dualTypeClass);
                }
            }
            return dualTypeClasses;
        }

        private void CheckResistances()
        {
            foreach (var res in Type1.Resistances)
            {
                if (Type2.Resistances.Any(x => x.ThisType == res.ThisType))
                {
                    dualTypeClass.Resistances.Add(new TypeMult(res.ThisType, 0.25));
                    ResistancesChecked.Add(res);
                }
                else if (Type2.Weaknesses.Any(x => x.ThisType == res.ThisType))
                    WeaknessesChecked.Add(res);
                else if (!Type2.Immunities.Contains(res))
                    dualTypeClass.Resistances.Add(res);
            }

            foreach (var res in Type2.Resistances)
            {
                if (ResistancesChecked.Any(x => x.ThisType == res.ThisType) || WeaknessesChecked.Any(x => x.ThisType == res.ThisType))
                    continue;

                if (Type1.Weaknesses.Any(x => x.ThisType == res.ThisType))
                    WeaknessesChecked.Add(res);
                else if (!Type1.Immunities.Any(x => x.ThisType == res.ThisType))
                    dualTypeClass.Resistances.Add(res);
            }
        }

        private void CheckWeaknesses()
        {
            foreach (var weak in Type1.Weaknesses)
            {
                if (ResistancesChecked.Any(x => x.ThisType == weak.ThisType) || WeaknessesChecked.Any(x => x.ThisType == weak.ThisType))
                    continue;

                if (Type2.Weaknesses.Any(x => x.ThisType == weak.ThisType))
                {
                    dualTypeClass.Weaknesses.Add(new TypeMult(weak.ThisType, 4));
                    WeaknessesChecked.Add(weak);
                }
                else if (!Type2.Immunities.Any(x => x.ThisType == weak.ThisType))
                    dualTypeClass.Weaknesses.Add(weak);
            }

            foreach (var weak in Type2.Weaknesses)
            {
                if (ResistancesChecked.Any(x => x.ThisType == weak.ThisType) || WeaknessesChecked.Any(x => x.ThisType == weak.ThisType))
                    continue;

                if (!Type1.Immunities.Any(x => x.ThisType == weak.ThisType))
                    dualTypeClass.Weaknesses.Add(weak);
            }
        }

        private void SetNormalDamage()
        {
            var exclude = dualTypeClass.Resistances.Select(x => x.ThisType).Concat(dualTypeClass.Weaknesses.Select(x => x.ThisType)).Concat(dualTypeClass.Immunities.Select(x => x.ThisType));
            var norms = Enum.GetValues(typeof(TypeEnum)).Cast<TypeEnum>().ToList().Except(exclude);
            foreach (var n in norms)
                dualTypeClass.NormalDamage.Add(new TypeMult(n, 1));
        }
    }
}